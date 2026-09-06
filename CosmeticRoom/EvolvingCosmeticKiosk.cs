using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GorillaNetworking;
using UnityEngine;

namespace CosmeticRoom
{
	// Token: 0x02001087 RID: 4231
	public class EvolvingCosmeticKiosk : MonoBehaviour
	{
		// Token: 0x17000A0A RID: 2570
		// (get) Token: 0x0600699F RID: 27039 RVA: 0x002200AC File Offset: 0x0021E2AC
		// (set) Token: 0x060069A0 RID: 27040 RVA: 0x002200B4 File Offset: 0x0021E2B4
		public bool Initialized { get; private set; }

		// Token: 0x17000A0B RID: 2571
		// (get) Token: 0x060069A1 RID: 27041 RVA: 0x002200BD File Offset: 0x0021E2BD
		public VRRig VRRig
		{
			get
			{
				return VRRig.LocalRig;
			}
		}

		// Token: 0x17000A0C RID: 2572
		// (get) Token: 0x060069A2 RID: 27042 RVA: 0x002200C4 File Offset: 0x0021E2C4
		// (set) Token: 0x060069A3 RID: 27043 RVA: 0x002200CC File Offset: 0x0021E2CC
		public bool CosmeticsListBuilding { get; private set; }

		// Token: 0x060069A4 RID: 27044 RVA: 0x002200D8 File Offset: 0x0021E2D8
		private void Awake()
		{
			EvolvingCosmeticKioskButtonSet[] buttonSets = this._buttonSets;
			for (int i = 0; i < buttonSets.Length; i++)
			{
				buttonSets[i].RegisterKiosk(this);
			}
			this.Initialized = true;
		}

		// Token: 0x060069A5 RID: 27045 RVA: 0x0022010C File Offset: 0x0021E30C
		private async Task BuildCosmeticsList()
		{
			this._cosmetics.Clear();
			this.UpdateButtonSets();
			this.CosmeticsListBuilding = true;
			while (CosmeticsController.instance == null || !CosmeticsController.instance.v2_isCosmeticPlayFabCatalogDataLoaded || !CosmeticsV2Spawner_Dirty.isPrepared)
			{
				await Task.Yield();
			}
			CosmeticItemRegistry registry = this.VRRig.cosmeticsObjectRegistry;
			HashSet<string> loadedCosmetics = new HashSet<string>();
			foreach (CosmeticsController.CosmeticItem item in CosmeticsController.instance.currentWornSet.items)
			{
				if (!string.IsNullOrEmpty(item.itemName) && !(item.itemName == "null"))
				{
					await Task.Yield();
					CosmeticItemInstance cosmeticItemInstance;
					try
					{
						Debug.Log("Fetching cosmetic " + item.itemName);
						cosmeticItemInstance = registry.Cosmetic(item.itemName);
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						goto IL_0269;
					}
					if (cosmeticItemInstance != null)
					{
						foreach (GameObject gameObject in cosmeticItemInstance.objects)
						{
							EvolvingCosmetic component = gameObject.GetComponent<EvolvingCosmetic>();
							if (component != null && loadedCosmetics.Add(item.itemName))
							{
								this._cosmetics.Add(new EvolvingCosmeticKiosk.CosmeticData
								{
									EvolvingCosmetic = component,
									PlayfabId = item.itemName
								});
							}
						}
						item = default(CosmeticsController.CosmeticItem);
					}
				}
				IL_0269:;
			}
			CosmeticsController.CosmeticItem[] array = null;
			Debug.Log(string.Format("EvolvingCosmetics loaded ({0} found).", this._cosmetics.Count));
			this.CosmeticsListBuilding = false;
			this.ResetButtonSets();
			this.UpdateButtonSets();
		}

		// Token: 0x060069A6 RID: 27046 RVA: 0x00220150 File Offset: 0x0021E350
		private void ResetButtonSets()
		{
			this._cosmeticIdx = 0;
			EvolvingCosmeticKioskButtonSet[] buttonSets = this._buttonSets;
			for (int i = 0; i < buttonSets.Length; i++)
			{
				buttonSets[i].Reset();
			}
		}

		// Token: 0x060069A7 RID: 27047 RVA: 0x00220184 File Offset: 0x0021E384
		private void UpdateButtonSets()
		{
			for (int i = 0; i < this._buttonSets.Length; i++)
			{
				int num = this._cosmeticIdx + i;
				if (num >= this._cosmetics.Count)
				{
					this._buttonSets[i].Reset();
				}
				else
				{
					EvolvingCosmeticKiosk.CosmeticData cosmeticData = this._cosmetics[num];
					this._buttonSets[i].SetCosmetic(cosmeticData.PlayfabId, cosmeticData.EvolvingCosmetic);
				}
			}
		}

		// Token: 0x060069A8 RID: 27048 RVA: 0x002201F0 File Offset: 0x0021E3F0
		public async void OnHandScanned(NetPlayer player)
		{
			if (player.IsLocal)
			{
				await this.BuildCosmeticsList();
			}
		}

		// Token: 0x060069A9 RID: 27049 RVA: 0x0022022F File Offset: 0x0021E42F
		public void ScrollForward()
		{
			this.Scroll(1);
		}

		// Token: 0x060069AA RID: 27050 RVA: 0x00220238 File Offset: 0x0021E438
		public void ScrollBackward()
		{
			this.Scroll(-1);
		}

		// Token: 0x060069AB RID: 27051 RVA: 0x00220241 File Offset: 0x0021E441
		private void Scroll(int direction)
		{
			this._cosmeticIdx = Math.Clamp(this._cosmeticIdx + direction, 0, this._cosmetics.Count - 1);
			this.UpdateButtonSets();
		}

		// Token: 0x04007960 RID: 31072
		[SerializeField]
		private EvolvingCosmeticKioskButtonSet[] _buttonSets;

		// Token: 0x04007961 RID: 31073
		private readonly List<EvolvingCosmeticKiosk.CosmeticData> _cosmetics = new List<EvolvingCosmeticKiosk.CosmeticData>();

		// Token: 0x04007963 RID: 31075
		private int _cosmeticIdx;

		// Token: 0x02001088 RID: 4232
		[NullableContext(1)]
		[Nullable(0)]
		private class CosmeticData : IEquatable<EvolvingCosmeticKiosk.CosmeticData>
		{
			// Token: 0x17000A0D RID: 2573
			// (get) Token: 0x060069AD RID: 27053 RVA: 0x0022027D File Offset: 0x0021E47D
			[CompilerGenerated]
			protected virtual Type EqualityContract
			{
				[CompilerGenerated]
				get
				{
					return typeof(EvolvingCosmeticKiosk.CosmeticData);
				}
			}

			// Token: 0x060069AE RID: 27054 RVA: 0x0022028C File Offset: 0x0021E48C
			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("CosmeticData");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x060069AF RID: 27055 RVA: 0x002202D8 File Offset: 0x0021E4D8
			[CompilerGenerated]
			protected virtual bool PrintMembers(StringBuilder builder)
			{
				RuntimeHelpers.EnsureSufficientExecutionStack();
				builder.Append("EvolvingCosmetic = ");
				builder.Append(this.EvolvingCosmetic);
				builder.Append(", PlayfabId = ");
				builder.Append(this.PlayfabId);
				return true;
			}

			// Token: 0x060069B0 RID: 27056 RVA: 0x00220312 File Offset: 0x0021E512
			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator !=(EvolvingCosmeticKiosk.CosmeticData left, EvolvingCosmeticKiosk.CosmeticData right)
			{
				return !(left == right);
			}

			// Token: 0x060069B1 RID: 27057 RVA: 0x0022031E File Offset: 0x0021E51E
			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator ==(EvolvingCosmeticKiosk.CosmeticData left, EvolvingCosmeticKiosk.CosmeticData right)
			{
				return left == right || (left != null && left.Equals(right));
			}

			// Token: 0x060069B2 RID: 27058 RVA: 0x00220332 File Offset: 0x0021E532
			[CompilerGenerated]
			public override int GetHashCode()
			{
				return (EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<EvolvingCosmetic>.Default.GetHashCode(this.EvolvingCosmetic)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.PlayfabId);
			}

			// Token: 0x060069B3 RID: 27059 RVA: 0x00220372 File Offset: 0x0021E572
			[NullableContext(2)]
			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return this.Equals(obj as EvolvingCosmeticKiosk.CosmeticData);
			}

			// Token: 0x060069B4 RID: 27060 RVA: 0x00220380 File Offset: 0x0021E580
			[NullableContext(2)]
			[CompilerGenerated]
			public virtual bool Equals(EvolvingCosmeticKiosk.CosmeticData other)
			{
				return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<EvolvingCosmetic>.Default.Equals(this.EvolvingCosmetic, other.EvolvingCosmetic) && EqualityComparer<string>.Default.Equals(this.PlayfabId, other.PlayfabId));
			}

			// Token: 0x060069B6 RID: 27062 RVA: 0x002203E1 File Offset: 0x0021E5E1
			[CompilerGenerated]
			protected CosmeticData(EvolvingCosmeticKiosk.CosmeticData original)
			{
				this.EvolvingCosmetic = original.EvolvingCosmetic;
				this.PlayfabId = original.PlayfabId;
			}

			// Token: 0x060069B7 RID: 27063 RVA: 0x00002050 File Offset: 0x00000250
			public CosmeticData()
			{
			}

			// Token: 0x04007964 RID: 31076
			[Nullable(0)]
			public EvolvingCosmetic EvolvingCosmetic;

			// Token: 0x04007965 RID: 31077
			[Nullable(0)]
			public string PlayfabId;
		}
	}
}
