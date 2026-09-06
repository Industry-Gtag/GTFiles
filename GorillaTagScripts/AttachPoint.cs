using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000F4D RID: 3917
	public class AttachPoint : MonoBehaviour
	{
		// Token: 0x0600603C RID: 24636 RVA: 0x001E8671 File Offset: 0x001E6871
		private void Start()
		{
			base.transform.parent.parent = null;
		}

		// Token: 0x0600603D RID: 24637 RVA: 0x001E8684 File Offset: 0x001E6884
		private void OnTriggerEnter(Collider other)
		{
			if (this.attachPoint.childCount == 0)
			{
				this.UpdateHookState(false);
			}
			DecorativeItem componentInParent = other.GetComponentInParent<DecorativeItem>();
			if (componentInParent == null || componentInParent.InHand())
			{
				return;
			}
			if (this.IsHooked())
			{
				return;
			}
			this.UpdateHookState(true);
			componentInParent.SnapItem(true, this.attachPoint.position);
		}

		// Token: 0x0600603E RID: 24638 RVA: 0x001E86E0 File Offset: 0x001E68E0
		private void OnTriggerExit(Collider other)
		{
			DecorativeItem componentInParent = other.GetComponentInParent<DecorativeItem>();
			if (componentInParent == null || !componentInParent.InHand())
			{
				return;
			}
			this.UpdateHookState(false);
			componentInParent.SnapItem(false, Vector3.zero);
		}

		// Token: 0x0600603F RID: 24639 RVA: 0x001E8719 File Offset: 0x001E6919
		private void UpdateHookState(bool isHooked)
		{
			this.SetIsHook(isHooked);
		}

		// Token: 0x06006040 RID: 24640 RVA: 0x001E8722 File Offset: 0x001E6922
		internal void SetIsHook(bool isHooked)
		{
			this.isHooked = isHooked;
			UnityAction unityAction = this.onHookedChanged;
			if (unityAction == null)
			{
				return;
			}
			unityAction();
		}

		// Token: 0x06006041 RID: 24641 RVA: 0x001E873B File Offset: 0x001E693B
		public bool IsHooked()
		{
			return this.isHooked || this.attachPoint.childCount != 0;
		}

		// Token: 0x04006EDA RID: 28378
		public Transform attachPoint;

		// Token: 0x04006EDB RID: 28379
		public UnityAction onHookedChanged;

		// Token: 0x04006EDC RID: 28380
		private bool isHooked;

		// Token: 0x04006EDD RID: 28381
		private bool wasHooked;

		// Token: 0x04006EDE RID: 28382
		public bool inForest;
	}
}
