using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200017D RID: 381
public class SpawnRegion<TItem, TRegion> : MonoBehaviour where TItem : Object where TRegion : SpawnRegion<TItem, TRegion>
{
	// Token: 0x170000DF RID: 223
	// (get) Token: 0x060009F8 RID: 2552 RVA: 0x00035C88 File Offset: 0x00033E88
	public static List<TRegion> Regions
	{
		get
		{
			return SpawnRegion<TItem, TRegion>._regions;
		}
	}

	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x060009F9 RID: 2553 RVA: 0x00035C8F File Offset: 0x00033E8F
	// (set) Token: 0x060009FA RID: 2554 RVA: 0x00035C97 File Offset: 0x00033E97
	public int MaxItems { get; private set; } = 10;

	// Token: 0x170000E1 RID: 225
	// (get) Token: 0x060009FB RID: 2555 RVA: 0x00035CA0 File Offset: 0x00033EA0
	private bool HasSpawnOrigins
	{
		get
		{
			Transform[] array = this.spawnOrigins;
			return array != null && array.Length != 0;
		}
	}

	// Token: 0x170000E2 RID: 226
	// (get) Token: 0x060009FC RID: 2556 RVA: 0x00035CB2 File Offset: 0x00033EB2
	public List<TItem> Items
	{
		get
		{
			return this._items;
		}
	}

	// Token: 0x170000E3 RID: 227
	// (get) Token: 0x060009FD RID: 2557 RVA: 0x00035CBA File Offset: 0x00033EBA
	public int ItemCount
	{
		get
		{
			return this._items.Count;
		}
	}

	// Token: 0x170000E4 RID: 228
	// (get) Token: 0x060009FE RID: 2558 RVA: 0x00035CC7 File Offset: 0x00033EC7
	// (set) Token: 0x060009FF RID: 2559 RVA: 0x00035CCF File Offset: 0x00033ECF
	public int ID { get; private set; }

	// Token: 0x06000A00 RID: 2560 RVA: 0x00035CD8 File Offset: 0x00033ED8
	private void OnEnable()
	{
		Transform[] array = this.spawnOrigins;
		this._useSpawnOrigins = array != null && array.Length != 0;
		this._testAgainstGeo = !this._useSpawnOrigins && this.geoTestPoint;
		if (this._testAgainstGeo && this._hitTestBuffer == null)
		{
			this._hitTestBuffer = new RaycastHit[20];
		}
		SpawnRegion<TItem, TRegion>.RegisterRegion((TRegion)((object)this));
	}

	// Token: 0x06000A01 RID: 2561 RVA: 0x00035D40 File Offset: 0x00033F40
	private void OnDisable()
	{
		SpawnRegion<TItem, TRegion>.UnregisterRegion((TRegion)((object)this));
		foreach (TItem titem in this._items)
		{
			if (titem)
			{
				SpawnRegion<TItem, TRegion>._itemRegionLookup.Remove(titem);
			}
		}
		this._items.Clear();
	}

	// Token: 0x06000A02 RID: 2562 RVA: 0x00035DBC File Offset: 0x00033FBC
	private static void RegisterRegion(TRegion region)
	{
		SpawnRegion<TItem, TRegion>._regionLookup[region.ID] = region;
		SpawnRegion<TItem, TRegion>._regions.Add(region);
	}

	// Token: 0x06000A03 RID: 2563 RVA: 0x00035DDF File Offset: 0x00033FDF
	private static void UnregisterRegion(TRegion region)
	{
		SpawnRegion<TItem, TRegion>._regionLookup.Remove(region.ID);
		SpawnRegion<TItem, TRegion>._regions.Remove(region);
	}

	// Token: 0x06000A04 RID: 2564 RVA: 0x00035E04 File Offset: 0x00034004
	public static void AddItemToRegion(TItem item, int regionId)
	{
		TRegion tregion;
		if (SpawnRegion<TItem, TRegion>._regionLookup.TryGetValue(regionId, out tregion))
		{
			tregion.AddItem(item);
		}
	}

	// Token: 0x06000A05 RID: 2565 RVA: 0x00035E2C File Offset: 0x0003402C
	public static void RemoveItemFromRegion(TItem item)
	{
		int num;
		TRegion tregion;
		if (SpawnRegion<TItem, TRegion>._itemRegionLookup.TryGetValue(item, out num) && SpawnRegion<TItem, TRegion>._regionLookup.TryGetValue(num, out tregion))
		{
			tregion.RemoveItem(item);
		}
	}

	// Token: 0x06000A06 RID: 2566 RVA: 0x00035E63 File Offset: 0x00034063
	public void AddItem(TItem item)
	{
		this._items.Add(item);
		SpawnRegion<TItem, TRegion>._itemRegionLookup[item] = this.ID;
	}

	// Token: 0x06000A07 RID: 2567 RVA: 0x00035E82 File Offset: 0x00034082
	public void RemoveItem(TItem item)
	{
		this._items.Remove(item);
		SpawnRegion<TItem, TRegion>._itemRegionLookup.Remove(item);
	}

	// Token: 0x06000A08 RID: 2568 RVA: 0x00035EA0 File Offset: 0x000340A0
	[return: TupleElementNames(new string[] { "isOnGround", "position", "normal" })]
	public ValueTuple<bool, Vector3, Vector3> GetSpawnPointWithNormal(int maxTries = 5)
	{
		for (int i = 0; i < maxTries; i++)
		{
			RaycastHit raycastHit;
			if (this.TryGetSpawnPoint(out raycastHit))
			{
				return new ValueTuple<bool, Vector3, Vector3>(true, raycastHit.point, raycastHit.normal);
			}
		}
		float num = this._scale / 2f;
		Vector3 vector = base.transform.TransformPoint(new Vector3(Random.Range(-num, num), num, Random.Range(-num, num)));
		return new ValueTuple<bool, Vector3, Vector3>(false, vector, Vector3.up);
	}

	// Token: 0x06000A09 RID: 2569 RVA: 0x00035F14 File Offset: 0x00034114
	private bool TryGetSpawnPoint(out RaycastHit spawnPoint)
	{
		float num = base.transform.lossyScale.y * this._scale;
		if (this._useSpawnOrigins)
		{
			Vector3 vector = this.spawnOrigins[Random.Range(0, this.spawnOrigins.Length)].position;
			if (this.TryGetSpawnPoint(vector, Random.onUnitSphere, Mathf.Max(num, 100f), out spawnPoint))
			{
				return spawnPoint.normal.y > 0f || this.TryGetSpawnPoint(spawnPoint.point, Vector3.down, num, out spawnPoint);
			}
			spawnPoint = default(RaycastHit);
			return false;
		}
		else
		{
			float num2 = this._scale / 2f;
			Vector3 vector = base.transform.TransformPoint(new Vector3(Random.Range(-num2, num2), num2, Random.Range(-num2, num2)));
			if (this._testAgainstGeo && this.IsInsideGeo(vector))
			{
				spawnPoint = default(RaycastHit);
				return false;
			}
			return this.TryGetSpawnPoint(vector, Vector3.down, num, out spawnPoint);
		}
	}

	// Token: 0x06000A0A RID: 2570 RVA: 0x00036000 File Offset: 0x00034200
	private bool TryGetSpawnPoint(Vector3 origin, Vector3 direction, float distance, out RaycastHit spawnPoint)
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(origin, direction, out raycastHit, distance, -1, QueryTriggerInteraction.Ignore))
		{
			Debug.DrawLine(origin, raycastHit.point, Color.green, 5f);
			spawnPoint = raycastHit;
			return true;
		}
		Debug.DrawLine(origin, origin + direction * distance, Color.red, 5f);
		spawnPoint = default(RaycastHit);
		return false;
	}

	// Token: 0x06000A0B RID: 2571 RVA: 0x00036064 File Offset: 0x00034264
	private bool IsInsideGeo(Vector3 point)
	{
		Vector3 position = this.geoTestPoint.position;
		Vector3 vector = position - point;
		int num;
		int num2;
		for (;;)
		{
			num = Physics.RaycastNonAlloc(point, vector, this._hitTestBuffer, vector.magnitude, -1, QueryTriggerInteraction.Ignore);
			num2 = Physics.RaycastNonAlloc(position, -vector, this._hitTestBuffer, vector.magnitude, -1, QueryTriggerInteraction.Ignore);
			if (num < this._hitTestBuffer.Length && num2 < this._hitTestBuffer.Length)
			{
				break;
			}
			this._hitTestBuffer = new RaycastHit[this._hitTestBuffer.Length * 2];
		}
		bool flag = (num + num2) % 2 != 0;
		Debug.DrawLine(point, position, flag ? Color.red : Color.green, 5f);
		return flag;
	}

	// Token: 0x04000C4F RID: 3151
	private static List<TRegion> _regions = new List<TRegion>();

	// Token: 0x04000C50 RID: 3152
	private static Dictionary<int, TRegion> _regionLookup = new Dictionary<int, TRegion>();

	// Token: 0x04000C51 RID: 3153
	private static Dictionary<TItem, int> _itemRegionLookup = new Dictionary<TItem, int>();

	// Token: 0x04000C52 RID: 3154
	[SerializeField]
	private float _scale = 10f;

	// Token: 0x04000C54 RID: 3156
	[SerializeField]
	[Tooltip("If set, spawn points will be created via raycasts from one of these points.")]
	private Transform[] spawnOrigins;

	// Token: 0x04000C55 RID: 3157
	[SerializeField]
	[Tooltip("If set, all spawn points will be tested against this transform to see if they're inside geo.  Ignored if spawn origins are configured.")]
	private Transform geoTestPoint;

	// Token: 0x04000C56 RID: 3158
	private List<TItem> _items = new List<TItem>();

	// Token: 0x04000C58 RID: 3160
	private bool _useSpawnOrigins;

	// Token: 0x04000C59 RID: 3161
	private bool _testAgainstGeo;

	// Token: 0x04000C5A RID: 3162
	private RaycastHit[] _hitTestBuffer;
}
