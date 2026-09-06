using System;
using System.Collections.Generic;
using Drawing;
using GorillaTag;
using UnityEngine;

// Token: 0x02000B1E RID: 2846
public class VolumeCast : MonoBehaviourGizmos
{
	// Token: 0x06004925 RID: 18725 RVA: 0x0018747C File Offset: 0x0018567C
	public bool CheckOverlaps()
	{
		Transform transform = base.transform;
		Vector3 lossyScale = transform.lossyScale;
		Quaternion rotation = transform.rotation;
		int num = (int)this.physicsMask;
		QueryTriggerInteraction queryTriggerInteraction = (this.includeTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore);
		Vector3 vector;
		Vector3 vector2;
		float num2;
		VolumeCast.GetEndsAndRadius(transform, this.center, this.height, this.radius, out vector, out vector2, out num2);
		VolumeCast.VolumeShape volumeShape = this.shape;
		Vector3 vector3;
		Vector3 vector4;
		if (volumeShape != VolumeCast.VolumeShape.Box)
		{
			if (volumeShape != VolumeCast.VolumeShape.Cylinder)
			{
				return false;
			}
			vector3 = (vector + vector2) * 0.5f;
			vector4 = new Vector3(num2, Vector3.Distance(vector, vector2) * 0.5f, num2);
		}
		else
		{
			vector3 = transform.TransformPoint(this.center);
			vector4 = Vector3.Scale(lossyScale, this.size * 0.5f).Abs();
		}
		Array.Clear(this._boxOverlaps, 0, 8);
		this._boxHits = Physics.OverlapBoxNonAlloc(vector3, vector4, this._boxOverlaps, rotation, num, queryTriggerInteraction);
		if (this.shape != VolumeCast.VolumeShape.Cylinder)
		{
			return this._colliding = this._boxHits > 0;
		}
		this._hits = 0;
		Array.Clear(this._capOverlaps, 0, 8);
		Array.Clear(this._overlaps, 0, 8);
		this._capHits = Physics.OverlapCapsuleNonAlloc(vector, vector2, num2, this._capOverlaps, num, queryTriggerInteraction);
		this._set.Clear();
		int num3 = Math.Max(this._capHits, this._boxHits);
		Collider[] array = ((this._capHits < this._boxHits) ? this._capOverlaps : this._boxOverlaps);
		Collider[] array2 = ((this._capHits < this._boxHits) ? this._boxOverlaps : this._capOverlaps);
		for (int i = 0; i < num3; i++)
		{
			Collider collider = array[i];
			if (collider && !this._set.Add(collider))
			{
				Collider[] overlaps = this._overlaps;
				int num4 = this._hits;
				this._hits = num4 + 1;
				overlaps[num4] = collider;
			}
			Collider collider2 = array2[i];
			if (collider2 && !this._set.Add(collider2))
			{
				Collider[] overlaps2 = this._overlaps;
				int num4 = this._hits;
				this._hits = num4 + 1;
				overlaps2[num4] = collider2;
			}
		}
		return this._colliding = this._hits > 0;
	}

	// Token: 0x06004926 RID: 18726 RVA: 0x001876C0 File Offset: 0x001858C0
	private static void GetEndsAndRadius(Transform t, Vector3 center, float height, float radius, out Vector3 a, out Vector3 b, out float r)
	{
		float num = height * 0.5f;
		Vector3 lossyScale = t.lossyScale;
		a = t.TransformPoint(center + Vector3.down * num);
		b = t.TransformPoint(center + Vector3.up * num);
		r = Math.Max(Math.Abs(lossyScale.x), Math.Abs(lossyScale.z)) * radius;
	}

	// Token: 0x04005B44 RID: 23364
	public VolumeCast.VolumeShape shape;

	// Token: 0x04005B45 RID: 23365
	[Space]
	public Vector3 center;

	// Token: 0x04005B46 RID: 23366
	public Vector3 size = Vector3.one;

	// Token: 0x04005B47 RID: 23367
	public float height = 1f;

	// Token: 0x04005B48 RID: 23368
	public float radius = 1f;

	// Token: 0x04005B49 RID: 23369
	private const int MAX_HITS = 8;

	// Token: 0x04005B4A RID: 23370
	[Space]
	public UnityLayerMask physicsMask = UnityLayerMask.Everything;

	// Token: 0x04005B4B RID: 23371
	public bool includeTriggers;

	// Token: 0x04005B4C RID: 23372
	[Space]
	[SerializeField]
	private bool _simulateInEditMode;

	// Token: 0x04005B4D RID: 23373
	[DebugReadout]
	[NonSerialized]
	private int _capHits;

	// Token: 0x04005B4E RID: 23374
	[DebugReadout]
	[NonSerialized]
	private Collider[] _capOverlaps = new Collider[8];

	// Token: 0x04005B4F RID: 23375
	[DebugReadout]
	[NonSerialized]
	private int _boxHits;

	// Token: 0x04005B50 RID: 23376
	[DebugReadout]
	[NonSerialized]
	private Collider[] _boxOverlaps = new Collider[8];

	// Token: 0x04005B51 RID: 23377
	[DebugReadout]
	[NonSerialized]
	private int _hits;

	// Token: 0x04005B52 RID: 23378
	[DebugReadout]
	[NonSerialized]
	private Collider[] _overlaps = new Collider[8];

	// Token: 0x04005B53 RID: 23379
	[DebugReadout]
	[NonSerialized]
	private bool _colliding;

	// Token: 0x04005B54 RID: 23380
	[NonSerialized]
	private HashSet<Collider> _set = new HashSet<Collider>(8);

	// Token: 0x02000B1F RID: 2847
	public enum VolumeShape
	{
		// Token: 0x04005B56 RID: 23382
		Box,
		// Token: 0x04005B57 RID: 23383
		Cylinder
	}
}
