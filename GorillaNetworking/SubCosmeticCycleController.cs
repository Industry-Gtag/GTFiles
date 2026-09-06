using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020010D0 RID: 4304
	public class SubCosmeticCycleController : MonoBehaviour
	{
		// Token: 0x17000A40 RID: 2624
		// (get) Token: 0x06006B89 RID: 27529 RVA: 0x0022AAF7 File Offset: 0x00228CF7
		private CosmeticCollectionDisplay Display
		{
			get
			{
				if (this.display == null)
				{
					this.display = base.GetComponentInChildren<CosmeticCollectionDisplay>();
				}
				return this.display;
			}
		}

		// Token: 0x17000A41 RID: 2625
		// (get) Token: 0x06006B8A RID: 27530 RVA: 0x0022AB1C File Offset: 0x00228D1C
		public CosmeticsController.CosmeticItem? ActiveCollectable
		{
			get
			{
				CosmeticCollectionDisplay cosmeticCollectionDisplay = this.Display;
				if (cosmeticCollectionDisplay == null)
				{
					return null;
				}
				return cosmeticCollectionDisplay.ActiveCollectable;
			}
		}

		// Token: 0x17000A42 RID: 2626
		// (get) Token: 0x06006B8B RID: 27531 RVA: 0x0022AB42 File Offset: 0x00228D42
		public int ActiveIndex
		{
			get
			{
				CosmeticCollectionDisplay cosmeticCollectionDisplay = this.Display;
				if (cosmeticCollectionDisplay == null)
				{
					return 0;
				}
				return cosmeticCollectionDisplay.ActiveIndex;
			}
		}

		// Token: 0x17000A43 RID: 2627
		// (get) Token: 0x06006B8C RID: 27532 RVA: 0x0022AB55 File Offset: 0x00228D55
		public int Count
		{
			get
			{
				CosmeticCollectionDisplay cosmeticCollectionDisplay = this.Display;
				if (cosmeticCollectionDisplay == null)
				{
					return 0;
				}
				return cosmeticCollectionDisplay.Count;
			}
		}

		// Token: 0x17000A44 RID: 2628
		// (get) Token: 0x06006B8D RID: 27533 RVA: 0x0022AB68 File Offset: 0x00228D68
		private bool HasAuthority
		{
			get
			{
				return this.Display != null && this.Display.IsLocal;
			}
		}

		// Token: 0x06006B8E RID: 27534 RVA: 0x0022AB88 File Offset: 0x00228D88
		public string GetAppliedCosmeticID()
		{
			CosmeticsController.CosmeticItem? cosmeticItem;
			return ((this.ActiveCollectable != null) ? cosmeticItem.GetValueOrDefault().appliedCosmeticPlayFabID : null) ?? string.Empty;
		}

		// Token: 0x06006B8F RID: 27535 RVA: 0x0022ABC0 File Offset: 0x00228DC0
		public void CycleForward()
		{
			if (!this.HasAuthority || this.Display.Count <= 1)
			{
				return;
			}
			int num = (this.Display.ActiveIndex + 1) % this.Display.Count;
			this.Display.SetActiveIndex(num);
			this.SendStateRPC();
		}

		// Token: 0x06006B90 RID: 27536 RVA: 0x0022AC10 File Offset: 0x00228E10
		public void CycleBackward()
		{
			if (!this.HasAuthority || this.Display.Count <= 1)
			{
				return;
			}
			int num = (this.Display.ActiveIndex - 1 + this.Display.Count) % this.Display.Count;
			this.Display.SetActiveIndex(num);
			this.SendStateRPC();
		}

		// Token: 0x06006B91 RID: 27537 RVA: 0x0022AC6C File Offset: 0x00228E6C
		public void CycleRandom()
		{
			if (!this.HasAuthority || this.Display.Count <= 1)
			{
				return;
			}
			int num;
			do
			{
				num = Random.Range(0, this.Display.Count);
			}
			while (num == this.Display.ActiveIndex);
			this.Display.SetActiveIndex(num);
			this.SendStateRPC();
		}

		// Token: 0x06006B92 RID: 27538 RVA: 0x0022ACC2 File Offset: 0x00228EC2
		public void SetIndex(int index)
		{
			if (!this.HasAuthority)
			{
				return;
			}
			this.Display.SetActiveIndex(index);
			this.SendStateRPC();
		}

		// Token: 0x06006B93 RID: 27539 RVA: 0x0022ACDF File Offset: 0x00228EDF
		public void SetDisplayVisible(bool visible)
		{
			CosmeticCollectionDisplay cosmeticCollectionDisplay = this.Display;
			if (cosmeticCollectionDisplay == null)
			{
				return;
			}
			cosmeticCollectionDisplay.SetVisible(visible);
		}

		// Token: 0x06006B94 RID: 27540 RVA: 0x0022ACF2 File Offset: 0x00228EF2
		public void Equip(int canonicalIndex)
		{
			if (!this.HasAuthority)
			{
				return;
			}
			if (this.Display.SetEquippedAtCanonical(canonicalIndex, true))
			{
				this.SendStateRPC();
			}
		}

		// Token: 0x06006B95 RID: 27541 RVA: 0x0022AD12 File Offset: 0x00228F12
		public void Unequip(int canonicalIndex)
		{
			if (!this.HasAuthority)
			{
				return;
			}
			if (this.Display.SetEquippedAtCanonical(canonicalIndex, false))
			{
				this.SendStateRPC();
			}
		}

		// Token: 0x06006B96 RID: 27542 RVA: 0x0022AD34 File Offset: 0x00228F34
		public void EquipActive()
		{
			if (!this.HasAuthority)
			{
				return;
			}
			int activeCanonicalIndex = this.Display.ActiveCanonicalIndex;
			if (activeCanonicalIndex >= 0 && this.Display.SetEquippedAtCanonical(activeCanonicalIndex, true))
			{
				this.SendStateRPC();
			}
		}

		// Token: 0x06006B97 RID: 27543 RVA: 0x0022AD70 File Offset: 0x00228F70
		public void UnequipActive()
		{
			if (!this.HasAuthority)
			{
				return;
			}
			int activeCanonicalIndex = this.Display.ActiveCanonicalIndex;
			if (activeCanonicalIndex >= 0 && this.Display.SetEquippedAtCanonical(activeCanonicalIndex, false))
			{
				this.SendStateRPC();
			}
		}

		// Token: 0x06006B98 RID: 27544 RVA: 0x0022ADAB File Offset: 0x00228FAB
		public void EquipAll()
		{
			if (!this.HasAuthority)
			{
				return;
			}
			this.Display.SetVisibleMask(-1);
			this.SendStateRPC();
		}

		// Token: 0x06006B99 RID: 27545 RVA: 0x0022ADC8 File Offset: 0x00228FC8
		public void UnequipAll()
		{
			if (!this.HasAuthority)
			{
				return;
			}
			this.Display.SetVisibleMask(0);
			this.SendStateRPC();
		}

		// Token: 0x06006B9A RID: 27546 RVA: 0x0022ADE5 File Offset: 0x00228FE5
		public bool IsEquipped(int canonicalIndex)
		{
			return this.Display != null && this.Display.IsEquippedAtCanonical(canonicalIndex);
		}

		// Token: 0x06006B9B RID: 27547 RVA: 0x0022AE03 File Offset: 0x00229003
		public void BroadcastSignal(int signal)
		{
			if (!this.HasAuthority)
			{
				return;
			}
			this.BroadcastSignalLocal(signal);
			this.SendBroadcastSignalRPC(signal);
		}

		// Token: 0x06006B9C RID: 27548 RVA: 0x0022AE1C File Offset: 0x0022901C
		public void ReceiveNetworkSignal(int signal)
		{
			if (!this.receiveSignalLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			this.BroadcastSignalLocal(signal);
		}

		// Token: 0x06006B9D RID: 27549 RVA: 0x0022AE38 File Offset: 0x00229038
		public void BroadcastSignalLocal(int signal)
		{
			SubCosmeticSignalReceiver[] componentsInChildren = base.GetComponentsInChildren<SubCosmeticSignalReceiver>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i] != null)
				{
					componentsInChildren[i].ReceiveSignal(signal);
				}
			}
		}

		// Token: 0x06006B9E RID: 27550 RVA: 0x0022AE70 File Offset: 0x00229070
		private void SendBroadcastSignalRPC(int signal)
		{
			if (!this.syncBroadcastOverNetwork)
			{
				return;
			}
			if (this.Display == null || !this.Display.IsLocal)
			{
				return;
			}
			CosmeticCollectionDisplay cosmeticCollectionDisplay = this.Display;
			string text = ((cosmeticCollectionDisplay != null) ? cosmeticCollectionDisplay.ParentPlayFabID : null);
			if (string.IsNullOrEmpty(text) || text.Length < 5)
			{
				return;
			}
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			int num = (int)(text[0] - 'A' + '\u001a' * (text[1] - 'A' + '\u001a' * (text[2] - 'A' + '\u001a' * (text[3] - 'A' + '\u001a' * (text[4] - 'A')))));
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_BroadcastSubCosmeticSignal", RpcTarget.Others, new object[] { new int[] { num, signal } });
		}

		// Token: 0x06006B9F RID: 27551 RVA: 0x0022AF44 File Offset: 0x00229144
		private void SendStateRPC()
		{
			if (!this.syncCycleOverNetwork)
			{
				return;
			}
			if (this.Display == null || !this.Display.IsLocal)
			{
				return;
			}
			CosmeticCollectionDisplay cosmeticCollectionDisplay = this.Display;
			string text = ((cosmeticCollectionDisplay != null) ? cosmeticCollectionDisplay.ParentPlayFabID : null);
			if (string.IsNullOrEmpty(text) || text.Length < 5)
			{
				return;
			}
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			int num = this.Display.ActiveIndex;
			CosmeticsController.CosmeticItem? activeCollectable = this.Display.ActiveCollectable;
			if (activeCollectable != null && CosmeticsController.hasInstance)
			{
				int canonicalCollectableIndex = CosmeticsController.instance.GetCanonicalCollectableIndex(text, activeCollectable.Value.itemName);
				if (canonicalCollectableIndex >= 0)
				{
					num = canonicalCollectableIndex;
				}
			}
			int visibleMask = this.Display.VisibleMask;
			int num2 = (int)(text[0] - 'A' + '\u001a' * (text[1] - 'A' + '\u001a' * (text[2] - 'A' + '\u001a' * (text[3] - 'A' + '\u001a' * (text[4] - 'A')))));
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_SetCollectionCycleIndex", RpcTarget.Others, new object[] { new int[] { num2, num, visibleMask } });
		}

		// Token: 0x04007B0D RID: 31501
		[SerializeField]
		private bool syncCycleOverNetwork = true;

		// Token: 0x04007B0E RID: 31502
		[SerializeField]
		private bool syncBroadcastOverNetwork = true;

		// Token: 0x04007B0F RID: 31503
		private CosmeticCollectionDisplay display;

		// Token: 0x04007B10 RID: 31504
		private readonly CallLimiter receiveSignalLimiter = new CallLimiter(10, 1f, 0.5f);
	}
}
