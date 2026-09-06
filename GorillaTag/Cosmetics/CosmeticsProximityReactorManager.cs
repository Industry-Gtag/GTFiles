using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200131E RID: 4894
	public class CosmeticsProximityReactorManager : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x17000C05 RID: 3077
		// (get) Token: 0x06007AD1 RID: 31441 RVA: 0x00280CAC File Offset: 0x0027EEAC
		public static CosmeticsProximityReactorManager Instance
		{
			get
			{
				return CosmeticsProximityReactorManager._instance;
			}
		}

		// Token: 0x17000C06 RID: 3078
		// (get) Token: 0x06007AD2 RID: 31442 RVA: 0x00280CB3 File Offset: 0x0027EEB3
		public IReadOnlyList<CosmeticsProximityReactor> Cosmetics
		{
			get
			{
				return this.cosmetics;
			}
		}

		// Token: 0x140000CA RID: 202
		// (add) Token: 0x06007AD3 RID: 31443 RVA: 0x00280CBC File Offset: 0x0027EEBC
		// (remove) Token: 0x06007AD4 RID: 31444 RVA: 0x00280CF0 File Offset: 0x0027EEF0
		public static event Action<CosmeticsProximityReactor> OnCosmeticRegistered;

		// Token: 0x06007AD5 RID: 31445 RVA: 0x00280D23 File Offset: 0x0027EF23
		private void Awake()
		{
			if (CosmeticsProximityReactorManager._instance != null && CosmeticsProximityReactorManager._instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			CosmeticsProximityReactorManager._instance = this;
		}

		// Token: 0x06007AD6 RID: 31446 RVA: 0x00019260 File Offset: 0x00017460
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06007AD7 RID: 31447 RVA: 0x00280D51 File Offset: 0x0027EF51
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			if (CosmeticsProximityReactorManager._instance == this)
			{
				CosmeticsProximityReactorManager._instance = null;
			}
		}

		// Token: 0x06007AD8 RID: 31448 RVA: 0x00280D70 File Offset: 0x0027EF70
		public void Register(CosmeticsProximityReactor cosmetic)
		{
			if (cosmetic == null)
			{
				return;
			}
			if (cosmetic.IsGorillaBody())
			{
				if (!this.gorillaBodyPart.Contains(cosmetic))
				{
					this.gorillaBodyPart.Add(cosmetic);
				}
				return;
			}
			if (!this.cosmetics.Contains(cosmetic))
			{
				this.cosmetics.Add(cosmetic);
				Action<CosmeticsProximityReactor> onCosmeticRegistered = CosmeticsProximityReactorManager.OnCosmeticRegistered;
				if (onCosmeticRegistered != null)
				{
					onCosmeticRegistered(cosmetic);
				}
			}
			IReadOnlyList<string> types = cosmetic.GetTypes();
			for (int i = 0; i < types.Count; i++)
			{
				string text = types[i];
				if (!string.IsNullOrEmpty(text))
				{
					List<CosmeticsProximityReactor> list;
					if (!this.byType.TryGetValue(text, out list))
					{
						list = new List<CosmeticsProximityReactor>();
						this.byType[text] = list;
					}
					if (!list.Contains(cosmetic))
					{
						list.Add(cosmetic);
						this.typeKeysDirty = true;
					}
				}
			}
		}

		// Token: 0x06007AD9 RID: 31449 RVA: 0x00280E38 File Offset: 0x0027F038
		public void Unregister(CosmeticsProximityReactor cosmetic)
		{
			if (cosmetic == null)
			{
				return;
			}
			this.cosmetics.Remove(cosmetic);
			this.gorillaBodyPart.Remove(cosmetic);
			this.matchedFrame.Remove(cosmetic);
			foreach (KeyValuePair<string, List<CosmeticsProximityReactor>> keyValuePair in this.byType)
			{
				if (keyValuePair.Value.Remove(cosmetic))
				{
					this.typeKeysDirty = true;
				}
			}
		}

		// Token: 0x06007ADA RID: 31450 RVA: 0x00280ECC File Offset: 0x0027F0CC
		public void SliceUpdate()
		{
			if (this.cosmetics.Count == 0)
			{
				return;
			}
			if (this.AnyGroupHasTwo())
			{
				if (this.typeKeysDirty)
				{
					this.RebuildTypeKeysCache();
				}
				if (this.typeKeysCache.Count > 0)
				{
					for (int i = 0; i < this.typeKeysCache.Count; i++)
					{
						string text = this.typeKeysCache[i];
						List<CosmeticsProximityReactor> list;
						if (this.byType.TryGetValue(text, out list) && list != null && list.Count > 0)
						{
							this.ProcessOneGroup(list);
						}
					}
				}
			}
			if (this.gorillaBodyPart.Count > 0)
			{
				for (int j = 0; j < this.cosmetics.Count; j++)
				{
					CosmeticsProximityReactor cosmeticsProximityReactor = this.cosmetics[j];
					if (!(cosmeticsProximityReactor == null))
					{
						if (!cosmeticsProximityReactor.AcceptsAnySource())
						{
							cosmeticsProximityReactor.OnSourceAboveAll();
						}
						else
						{
							bool flag = false;
							Vector3 vector = default(Vector3);
							for (int k = 0; k < this.gorillaBodyPart.Count; k++)
							{
								CosmeticsProximityReactor cosmeticsProximityReactor2 = this.gorillaBodyPart[k];
								if (!(cosmeticsProximityReactor2 == null) && cosmeticsProximityReactor.AcceptsThisSource(cosmeticsProximityReactor2.gorillaBodyParts))
								{
									bool flag2;
									float sourceThresholdFor = cosmeticsProximityReactor.GetSourceThresholdFor(cosmeticsProximityReactor2, out flag2);
									Vector3 vector2;
									if (flag2 && CosmeticsProximityReactorManager.AreCollidersWithinThreshold(cosmeticsProximityReactor2, cosmeticsProximityReactor, sourceThresholdFor, out vector2))
									{
										cosmeticsProximityReactor.OnSourceBelow(vector2, cosmeticsProximityReactor2.gorillaBodyParts, cosmeticsProximityReactor2.GetComponentInParent<VRRig>());
										vector = vector2;
										flag = true;
									}
								}
							}
							if (flag)
							{
								cosmeticsProximityReactor.WhileSourceBelow(vector, CosmeticsProximityReactor.GorillaBodyPart.HandLeft | CosmeticsProximityReactor.GorillaBodyPart.HandRight | CosmeticsProximityReactor.GorillaBodyPart.Mouth, (this.gorillaBodyPart[0] != null) ? this.gorillaBodyPart[0].GetComponentInParent<VRRig>() : null);
							}
							else
							{
								cosmeticsProximityReactor.OnSourceAboveAll();
							}
						}
					}
				}
			}
			if (this.typeKeysDirty)
			{
				this.RebuildTypeKeysCache();
			}
			for (int l = 0; l < this.typeKeysCache.Count; l++)
			{
				string text2 = this.typeKeysCache[l];
				List<CosmeticsProximityReactor> list2;
				if (this.byType.TryGetValue(text2, out list2) && list2 != null && list2.Count > 0)
				{
					this.BreakTheBoundForGroup(list2);
				}
			}
		}

		// Token: 0x06007ADB RID: 31451 RVA: 0x002810D7 File Offset: 0x0027F2D7
		private void ProcessOneGroup(List<CosmeticsProximityReactor> group)
		{
			if (!this.CheckProximity(group))
			{
				this.BreakTheBoundForGroup(group);
			}
		}

		// Token: 0x06007ADC RID: 31452 RVA: 0x002810EC File Offset: 0x0027F2EC
		private bool CheckProximity(List<CosmeticsProximityReactor> group)
		{
			bool flag = false;
			for (int i = 0; i < group.Count; i++)
			{
				CosmeticsProximityReactor cosmeticsProximityReactor = group[i];
				if (!(cosmeticsProximityReactor == null))
				{
					for (int j = i + 1; j < group.Count; j++)
					{
						CosmeticsProximityReactor cosmeticsProximityReactor2 = group[j];
						if (!(cosmeticsProximityReactor2 == null) && !CosmeticsProximityReactorManager.ShouldSkipSameIdPair(cosmeticsProximityReactor, cosmeticsProximityReactor2))
						{
							bool flag2;
							float cosmeticPairThresholdWith = cosmeticsProximityReactor.GetCosmeticPairThresholdWith(cosmeticsProximityReactor2, out flag2);
							bool flag3;
							float cosmeticPairThresholdWith2 = cosmeticsProximityReactor2.GetCosmeticPairThresholdWith(cosmeticsProximityReactor, out flag3);
							if (flag2 || flag3)
							{
								float num = float.MaxValue;
								if (flag2 && cosmeticPairThresholdWith < num)
								{
									num = cosmeticPairThresholdWith;
								}
								if (flag3 && cosmeticPairThresholdWith2 < num)
								{
									num = cosmeticPairThresholdWith2;
								}
								Vector3 vector;
								if (CosmeticsProximityReactorManager.AreCollidersWithinThreshold(cosmeticsProximityReactor, cosmeticsProximityReactor2, num, out vector))
								{
									cosmeticsProximityReactor.OnCosmeticBelowWith(cosmeticsProximityReactor2, vector);
									cosmeticsProximityReactor2.OnCosmeticBelowWith(cosmeticsProximityReactor, vector);
									if (cosmeticsProximityReactor.IsBelow)
									{
										cosmeticsProximityReactor.RefreshAggregateMatched();
										this.matchedFrame[cosmeticsProximityReactor] = Time.frameCount;
										flag = true;
									}
									if (cosmeticsProximityReactor2.IsBelow)
									{
										cosmeticsProximityReactor2.RefreshAggregateMatched();
										this.matchedFrame[cosmeticsProximityReactor2] = Time.frameCount;
										flag = true;
									}
								}
							}
						}
					}
				}
			}
			return flag;
		}

		// Token: 0x06007ADD RID: 31453 RVA: 0x00281210 File Offset: 0x0027F410
		private void BreakTheBoundForGroup(List<CosmeticsProximityReactor> group)
		{
			for (int i = 0; i < group.Count; i++)
			{
				CosmeticsProximityReactor cosmeticsProximityReactor = group[i];
				int num;
				if (!(cosmeticsProximityReactor == null) && cosmeticsProximityReactor.HasAnyCosmeticMatch() && (!this.matchedFrame.TryGetValue(cosmeticsProximityReactor, out num) || num != Time.frameCount))
				{
					CosmeticsProximityReactor cosmeticsProximityReactor2;
					Vector3 vector;
					if (this.TryFindAnyCosmeticPartner(cosmeticsProximityReactor, out cosmeticsProximityReactor2, out vector))
					{
						cosmeticsProximityReactor.WhileCosmeticBelowWith(cosmeticsProximityReactor2, vector);
						cosmeticsProximityReactor2.WhileCosmeticBelowWith(cosmeticsProximityReactor, vector);
					}
					else
					{
						cosmeticsProximityReactor.OnCosmeticAboveAll();
					}
				}
			}
		}

		// Token: 0x06007ADE RID: 31454 RVA: 0x00281288 File Offset: 0x0027F488
		private bool TryFindAnyCosmeticPartner(CosmeticsProximityReactor a, out CosmeticsProximityReactor partner, out Vector3 contact)
		{
			partner = null;
			contact = default(Vector3);
			IReadOnlyList<string> types = a.GetTypes();
			for (int i = 0; i < types.Count; i++)
			{
				string text = types[i];
				List<CosmeticsProximityReactor> list;
				if (!string.IsNullOrEmpty(text) && this.byType.TryGetValue(text, out list) && list != null)
				{
					for (int j = 0; j < list.Count; j++)
					{
						CosmeticsProximityReactor cosmeticsProximityReactor = list[j];
						if (!(cosmeticsProximityReactor == null) && !(cosmeticsProximityReactor == a) && !CosmeticsProximityReactorManager.ShouldSkipSameIdPair(a, cosmeticsProximityReactor))
						{
							bool flag;
							float cosmeticPairThresholdWith = a.GetCosmeticPairThresholdWith(cosmeticsProximityReactor, out flag);
							if (flag)
							{
								float num = cosmeticPairThresholdWith;
								Vector3 vector;
								if (CosmeticsProximityReactorManager.AreCollidersWithinThreshold(a, cosmeticsProximityReactor, num, out vector))
								{
									partner = cosmeticsProximityReactor;
									contact = vector;
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06007ADF RID: 31455 RVA: 0x00281354 File Offset: 0x0027F554
		private static bool ShouldSkipSameIdPair(CosmeticsProximityReactor a, CosmeticsProximityReactor b)
		{
			return (a.ignoreSameCosmeticInstances || b.ignoreSameCosmeticInstances) && !string.IsNullOrEmpty(a.PlayFabID) && !string.IsNullOrEmpty(b.PlayFabID) && string.Equals(a.PlayFabID, b.PlayFabID, StringComparison.Ordinal);
		}

		// Token: 0x06007AE0 RID: 31456 RVA: 0x002813A4 File Offset: 0x0027F5A4
		private static bool AreCollidersWithinThreshold(CosmeticsProximityReactor a, CosmeticsProximityReactor b, float threshold, out Vector3 contactPoint)
		{
			Vector3 vector = ((b.collider == null) ? b.transform.position : b.collider.ClosestPoint(a.transform.position));
			Vector3 vector2 = ((a.collider == null) ? a.transform.position : a.collider.ClosestPoint(vector));
			contactPoint = (vector2 + vector) * 0.5f;
			return Vector3.Distance(vector2, vector) <= threshold;
		}

		// Token: 0x06007AE1 RID: 31457 RVA: 0x00281430 File Offset: 0x0027F630
		private bool AnyGroupHasTwo()
		{
			foreach (KeyValuePair<string, List<CosmeticsProximityReactor>> keyValuePair in this.byType)
			{
				List<CosmeticsProximityReactor> value = keyValuePair.Value;
				if (value != null && value.Count >= 2)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06007AE2 RID: 31458 RVA: 0x00281498 File Offset: 0x0027F698
		private void RebuildTypeKeysCache()
		{
			this.typeKeysCache.Clear();
			foreach (KeyValuePair<string, List<CosmeticsProximityReactor>> keyValuePair in this.byType)
			{
				List<CosmeticsProximityReactor> value = keyValuePair.Value;
				if (value != null && value.Count > 0)
				{
					this.typeKeysCache.Add(keyValuePair.Key);
				}
			}
			this.typeKeysDirty = false;
			if (this.groupCursor >= this.typeKeysCache.Count)
			{
				this.groupCursor = 0;
			}
		}

		// Token: 0x04008C6C RID: 35948
		private static CosmeticsProximityReactorManager _instance;

		// Token: 0x04008C6D RID: 35949
		private readonly List<CosmeticsProximityReactor> cosmetics = new List<CosmeticsProximityReactor>();

		// Token: 0x04008C6E RID: 35950
		private readonly List<CosmeticsProximityReactor> gorillaBodyPart = new List<CosmeticsProximityReactor>();

		// Token: 0x04008C70 RID: 35952
		private readonly Dictionary<string, List<CosmeticsProximityReactor>> byType = new Dictionary<string, List<CosmeticsProximityReactor>>(StringComparer.Ordinal);

		// Token: 0x04008C71 RID: 35953
		private readonly Dictionary<CosmeticsProximityReactor, int> matchedFrame = new Dictionary<CosmeticsProximityReactor, int>();

		// Token: 0x04008C72 RID: 35954
		private readonly List<string> typeKeysCache = new List<string>();

		// Token: 0x04008C73 RID: 35955
		private bool typeKeysDirty;

		// Token: 0x04008C74 RID: 35956
		private int groupCursor;

		// Token: 0x04008C75 RID: 35957
		internal static readonly List<string> SharedKeysCache = new List<string>();
	}
}
