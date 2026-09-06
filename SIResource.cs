using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000152 RID: 338
public class SIResource : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060008E9 RID: 2281 RVA: 0x0003078C File Offset: 0x0002E98C
	private void Awake()
	{
		if (this.myGameEntity == null)
		{
			this.myGameEntity = base.GetComponent<GameEntity>();
		}
		if (this.myGameEntity == null)
		{
			return;
		}
		GameEntity gameEntity = this.myGameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.SetLastGrabbed));
		this._rb = base.GetComponent<Rigidbody>();
		this.myGameEntity.onEntityDestroyed += this.HandleOnDestroyed;
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x0003080C File Offset: 0x0002EA0C
	public void SliceUpdate()
	{
		if (this.isSleeping || !this.shouldSleep)
		{
			return;
		}
		if (Time.time < this.timeReleased + this.sleepTime)
		{
			return;
		}
		this._rb.isKinematic = true;
		this.isSleeping = true;
	}

	// Token: 0x060008EB RID: 2283 RVA: 0x00030847 File Offset: 0x0002EA47
	public void SetLastGrabbed()
	{
		this.lastPlayerHeld = SIPlayer.Get(this.myGameEntity.lastHeldByActorNumber);
		if (this.lastPlayerHeld == SIPlayer.LocalPlayer)
		{
			this.localEverGrabbed = true;
		}
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x00030878 File Offset: 0x0002EA78
	protected virtual void OnEnable()
	{
		GameEntity gameEntity = this.myGameEntity;
		gameEntity.OnSnapped = (Action)Delegate.Combine(gameEntity.OnSnapped, new Action(this.GrabInitialization));
		GameEntity gameEntity2 = this.myGameEntity;
		gameEntity2.OnGrabbed = (Action)Delegate.Combine(gameEntity2.OnGrabbed, new Action(this.GrabInitialization));
		GameEntity gameEntity3 = this.myGameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.ReleaseInitialization));
		GameEntity gameEntity4 = this.myGameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.ReleaseInitialization));
		this.timeReleased = Time.time;
		this._rb.isKinematic = true;
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x00030940 File Offset: 0x0002EB40
	private void OnDisable()
	{
		GameEntity gameEntity = this.myGameEntity;
		gameEntity.OnSnapped = (Action)Delegate.Remove(gameEntity.OnSnapped, new Action(this.GrabInitialization));
		GameEntity gameEntity2 = this.myGameEntity;
		gameEntity2.OnGrabbed = (Action)Delegate.Remove(gameEntity2.OnGrabbed, new Action(this.GrabInitialization));
		GameEntity gameEntity3 = this.myGameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Remove(gameEntity3.OnReleased, new Action(this.ReleaseInitialization));
		GameEntity gameEntity4 = this.myGameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Remove(gameEntity4.OnUnsnapped, new Action(this.ReleaseInitialization));
		SpawnRegion<GameEntity, SIResourceRegion>.RemoveItemFromRegion(this.myGameEntity);
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060008EE RID: 2286 RVA: 0x000309FC File Offset: 0x0002EBFC
	public void GrabInitialization()
	{
		this.isSleeping = false;
		this.shouldSleep = false;
	}

	// Token: 0x060008EF RID: 2287 RVA: 0x00030A0C File Offset: 0x0002EC0C
	public void ReleaseInitialization()
	{
		this.shouldSleep = true;
		this.isSleeping = false;
		this.timeReleased = Time.time;
	}

	// Token: 0x060008F0 RID: 2288 RVA: 0x00030A27 File Offset: 0x0002EC27
	public virtual bool CanDeposit()
	{
		return this.lastPlayerHeld != null && this.lastPlayerHeld.gamePlayer.IsLocal() && !this.localDeposited && SIPlayer.LocalPlayer.CanLimitedResourceBeDeposited(this.limitedDepositType);
	}

	// Token: 0x060008F1 RID: 2289 RVA: 0x00030A63 File Offset: 0x0002EC63
	public virtual void HandleDepositLocal(SIPlayer depositingPlayer)
	{
		this.localDeposited = true;
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void HandleDepositAuth(SIPlayer depositingPlayer)
	{
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x00030A6C File Offset: 0x0002EC6C
	private void HandleOnDestroyed(GameEntity entity)
	{
		if (!this.localEverGrabbed || this.localDeposited || !entity.manager.IsZoneActive() || !PhotonNetwork.InRoom)
		{
			return;
		}
		if (this.type == SIResource.ResourceType.StrangeWood)
		{
			PlayerGameEvents.MiscEvent("SIHelpOtherCollectStrangeWood", 1);
			return;
		}
		if (this.type == SIResource.ResourceType.WeirdGear)
		{
			PlayerGameEvents.MiscEvent("SIHelpOtherCollectWeirdGears", 1);
			return;
		}
		if (this.type == SIResource.ResourceType.FloppyMetal)
		{
			PlayerGameEvents.MiscEvent("SIHelpOtherCollectFloppyMetal", 1);
			return;
		}
		if (this.type == SIResource.ResourceType.BouncySand)
		{
			PlayerGameEvents.MiscEvent("SIHelpOtherCollectBouncySand", 1);
			return;
		}
		if (this.type == SIResource.ResourceType.VibratingSpring)
		{
			PlayerGameEvents.MiscEvent("SIHelpOtherCollectVibratingSpring", 1);
		}
	}

	// Token: 0x060008F4 RID: 2292 RVA: 0x00030B08 File Offset: 0x0002ED08
	public static List<SIResource.ResourceCost> GetSum(params IList<SIResource.ResourceCost>[] costs)
	{
		List<SIResource.ResourceCost> list = new List<SIResource.ResourceCost>();
		if (costs == null)
		{
			return list;
		}
		foreach (IList<SIResource.ResourceCost> list2 in costs)
		{
			if (list2 != null)
			{
				foreach (SIResource.ResourceCost resourceCost in list2)
				{
					list.AddResourceCost(resourceCost);
				}
			}
		}
		return list;
	}

	// Token: 0x060008F5 RID: 2293 RVA: 0x00030B7C File Offset: 0x0002ED7C
	public static List<SIResource.ResourceCost> GetMax(params IList<SIResource.ResourceCost>[] costs)
	{
		List<SIResource.ResourceCost> list = new List<SIResource.ResourceCost>();
		if (costs == null)
		{
			return list;
		}
		for (int i = 0; i < costs.Length; i++)
		{
			foreach (SIResource.ResourceCost resourceCost in costs[i])
			{
				int num = Mathf.Max(list.GetAmount(resourceCost.type), resourceCost.amount);
				list.SetAmount(resourceCost.type, num);
			}
		}
		return list;
	}

	// Token: 0x060008F6 RID: 2294 RVA: 0x00030C08 File Offset: 0x0002EE08
	public static bool CategoryCostsMatch(IList<SIResource.ResourceCost> cost1, IList<SIResource.ResourceCost> cost2)
	{
		return cost1.GetCategoryCosts() == cost2.GetCategoryCosts();
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x00030C1C File Offset: 0x0002EE1C
	public static bool CostsAreEqual(IList<SIResource.ResourceCost> cost1, IList<SIResource.ResourceCost> cost2, bool matchOrder = true)
	{
		if (cost1.Count != cost2.Count)
		{
			return false;
		}
		if (!matchOrder)
		{
			foreach (SIResource.ResourceCost resourceCost in cost1)
			{
				if (cost2.GetAmount(resourceCost.type) != resourceCost.amount)
				{
					return false;
				}
			}
			return true;
		}
		for (int i = 0; i < cost1.Count; i++)
		{
			if (!cost1[i].Equals(cost2[i]))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060008F8 RID: 2296 RVA: 0x00030CBC File Offset: 0x0002EEBC
	public static SIResource.ResourceCost[] GenerateCostsFrom(Dictionary<SIResource.ResourceType, int> costDictionary)
	{
		List<SIResource.ResourceCost> list = new List<SIResource.ResourceCost>();
		foreach (KeyValuePair<SIResource.ResourceType, int> keyValuePair in costDictionary)
		{
			list.Add(new SIResource.ResourceCost(keyValuePair.Key, keyValuePair.Value));
		}
		list.Sort();
		return list.ToArray();
	}

	// Token: 0x060008F9 RID: 2297 RVA: 0x00030D30 File Offset: 0x0002EF30
	public static string PrintCost(IEnumerable<SIResource.ResourceCost> costs)
	{
		return "[" + string.Join<SIResource.ResourceCost>(", ", costs) + "]";
	}

	// Token: 0x04000B03 RID: 2819
	public SIPlayer lastPlayerHeld;

	// Token: 0x04000B04 RID: 2820
	public GameEntity myGameEntity;

	// Token: 0x04000B05 RID: 2821
	public SIResource.ResourceType type;

	// Token: 0x04000B06 RID: 2822
	public SIResource.LimitedDepositType limitedDepositType;

	// Token: 0x04000B07 RID: 2823
	public bool localDeposited;

	// Token: 0x04000B08 RID: 2824
	public bool localEverGrabbed;

	// Token: 0x04000B09 RID: 2825
	[Tooltip("The amount of pitch offset allowed during spawn, in degrees.  With this set to 0, item will always spawn aligned with surface.")]
	public float spawnPitchVariance;

	// Token: 0x04000B0A RID: 2826
	public float sleepTime = 10f;

	// Token: 0x04000B0B RID: 2827
	private bool shouldSleep = true;

	// Token: 0x04000B0C RID: 2828
	private bool isSleeping;

	// Token: 0x04000B0D RID: 2829
	private float timeReleased;

	// Token: 0x04000B0E RID: 2830
	private Rigidbody _rb;

	// Token: 0x02000153 RID: 339
	[Serializable]
	public struct ResourceCost : IComparable<SIResource.ResourceCost>, IEquatable<SIResource.ResourceCost>
	{
		// Token: 0x060008FB RID: 2299 RVA: 0x00030D66 File Offset: 0x0002EF66
		public ResourceCost(SIResource.ResourceType type, int amount)
		{
			this.type = type;
			this.amount = amount;
		}

		// Token: 0x060008FC RID: 2300 RVA: 0x00030D78 File Offset: 0x0002EF78
		public int CompareTo(SIResource.ResourceCost other)
		{
			int num = this.type.CompareTo(other.type);
			if (num != 0)
			{
				return num;
			}
			return this.amount.CompareTo(other.amount);
		}

		// Token: 0x060008FD RID: 2301 RVA: 0x00030DB8 File Offset: 0x0002EFB8
		public bool Equals(SIResource.ResourceCost other)
		{
			return this.type == other.type && this.amount == other.amount;
		}

		// Token: 0x060008FE RID: 2302 RVA: 0x00030DD8 File Offset: 0x0002EFD8
		public override bool Equals(object obj)
		{
			if (obj is SIResource.ResourceCost)
			{
				SIResource.ResourceCost resourceCost = (SIResource.ResourceCost)obj;
				return this.Equals(resourceCost);
			}
			return false;
		}

		// Token: 0x060008FF RID: 2303 RVA: 0x00030DFD File Offset: 0x0002EFFD
		public override int GetHashCode()
		{
			return HashCode.Combine<int, int>((int)this.type, this.amount);
		}

		// Token: 0x06000900 RID: 2304 RVA: 0x00030E10 File Offset: 0x0002F010
		public override string ToString()
		{
			return string.Format("{0}: {1}", this.type.ToString(), this.amount);
		}

		// Token: 0x04000B0F RID: 2831
		public SIResource.ResourceType type;

		// Token: 0x04000B10 RID: 2832
		public int amount;
	}

	// Token: 0x02000154 RID: 340
	public struct ResourceCategoryCost : IComparable<SIResource.ResourceCategoryCost>, IEquatable<SIResource.ResourceCategoryCost>
	{
		// Token: 0x06000901 RID: 2305 RVA: 0x00030E38 File Offset: 0x0002F038
		public ResourceCategoryCost(int techPoints, int misc)
		{
			this.techPoints = techPoints;
			this.misc = misc;
		}

		// Token: 0x06000902 RID: 2306 RVA: 0x00030E48 File Offset: 0x0002F048
		public int CompareTo(SIResource.ResourceCategoryCost other)
		{
			int num = this.techPoints.CompareTo(other.techPoints);
			if (num != 0)
			{
				return num;
			}
			return this.misc.CompareTo(other.misc);
		}

		// Token: 0x06000903 RID: 2307 RVA: 0x00030E7D File Offset: 0x0002F07D
		public bool Equals(SIResource.ResourceCategoryCost other)
		{
			return this.techPoints == other.techPoints && this.misc == other.misc;
		}

		// Token: 0x06000904 RID: 2308 RVA: 0x00030E9D File Offset: 0x0002F09D
		public static bool operator ==(SIResource.ResourceCategoryCost left, SIResource.ResourceCategoryCost right)
		{
			return left.Equals(right);
		}

		// Token: 0x06000905 RID: 2309 RVA: 0x00030EA7 File Offset: 0x0002F0A7
		public static bool operator !=(SIResource.ResourceCategoryCost left, SIResource.ResourceCategoryCost right)
		{
			return !left.Equals(right);
		}

		// Token: 0x06000906 RID: 2310 RVA: 0x00030EB4 File Offset: 0x0002F0B4
		public static SIResource.ResourceCategoryCost operator +(SIResource.ResourceCategoryCost left, SIResource.ResourceCategoryCost right)
		{
			return new SIResource.ResourceCategoryCost(left.techPoints + right.techPoints, left.misc + right.misc);
		}

		// Token: 0x06000907 RID: 2311 RVA: 0x00030ED5 File Offset: 0x0002F0D5
		public static SIResource.ResourceCategoryCost operator -(SIResource.ResourceCategoryCost left, SIResource.ResourceCategoryCost right)
		{
			return new SIResource.ResourceCategoryCost(left.techPoints - right.techPoints, left.misc - right.misc);
		}

		// Token: 0x06000908 RID: 2312 RVA: 0x00030EF6 File Offset: 0x0002F0F6
		public static SIResource.ResourceCategoryCost operator *(SIResource.ResourceCategoryCost cost, int multiple)
		{
			return new SIResource.ResourceCategoryCost(cost.techPoints * multiple, cost.misc * multiple);
		}

		// Token: 0x06000909 RID: 2313 RVA: 0x00030F0D File Offset: 0x0002F10D
		public static SIResource.ResourceCategoryCost operator *(int multiple, SIResource.ResourceCategoryCost cost)
		{
			return new SIResource.ResourceCategoryCost(cost.techPoints * multiple, cost.misc * multiple);
		}

		// Token: 0x0600090A RID: 2314 RVA: 0x00030F24 File Offset: 0x0002F124
		public static SIResource.ResourceCategoryCost Max(SIResource.ResourceCategoryCost left, SIResource.ResourceCategoryCost right)
		{
			return new SIResource.ResourceCategoryCost(Mathf.Max(left.techPoints, right.techPoints), Mathf.Max(left.misc, right.misc));
		}

		// Token: 0x0600090B RID: 2315 RVA: 0x00030F4D File Offset: 0x0002F14D
		public override int GetHashCode()
		{
			return HashCode.Combine<int, int>(this.techPoints, this.misc);
		}

		// Token: 0x04000B11 RID: 2833
		public int techPoints;

		// Token: 0x04000B12 RID: 2834
		public int misc;
	}

	// Token: 0x02000155 RID: 341
	public enum ResourceType
	{
		// Token: 0x04000B14 RID: 2836
		TechPoint,
		// Token: 0x04000B15 RID: 2837
		StrangeWood,
		// Token: 0x04000B16 RID: 2838
		WeirdGear,
		// Token: 0x04000B17 RID: 2839
		VibratingSpring,
		// Token: 0x04000B18 RID: 2840
		BouncySand,
		// Token: 0x04000B19 RID: 2841
		FloppyMetal,
		// Token: 0x04000B1A RID: 2842
		Count
	}

	// Token: 0x02000156 RID: 342
	public enum LimitedDepositType
	{
		// Token: 0x04000B1C RID: 2844
		None,
		// Token: 0x04000B1D RID: 2845
		MonkeIdol,
		// Token: 0x04000B1E RID: 2846
		Count
	}
}
