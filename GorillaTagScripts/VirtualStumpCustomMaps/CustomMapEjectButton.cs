using System;
using System.Collections;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000FCA RID: 4042
	public class CustomMapEjectButton : GorillaPressableButton
	{
		// Token: 0x06006473 RID: 25715 RVA: 0x00204C1C File Offset: 0x00202E1C
		public override void ButtonActivation()
		{
			base.ButtonActivation();
			base.StartCoroutine(this.ButtonPressed_Local());
			if (!this.processing)
			{
				this.HandleTeleport();
			}
		}

		// Token: 0x06006474 RID: 25716 RVA: 0x00204C3F File Offset: 0x00202E3F
		private IEnumerator ButtonPressed_Local()
		{
			this.isOn = true;
			this.UpdateColor();
			yield return new WaitForSeconds(this.debounceTime);
			this.isOn = false;
			this.UpdateColor();
			yield break;
		}

		// Token: 0x06006475 RID: 25717 RVA: 0x00204C4E File Offset: 0x00202E4E
		private void HandleTeleport()
		{
			if (this.processing)
			{
				return;
			}
			this.processing = true;
			CustomMapManager.ReturnToVirtualStump();
			this.processing = false;
		}

		// Token: 0x06006476 RID: 25718 RVA: 0x00204C6C File Offset: 0x00202E6C
		public void CopySettings(CustomMapEjectButtonSettings customMapEjectButtonSettings)
		{
			this.ejectType = (CustomMapEjectButton.EjectType)customMapEjectButtonSettings.ejectType;
		}

		// Token: 0x04007332 RID: 29490
		[SerializeField]
		private CustomMapEjectButton.EjectType ejectType;

		// Token: 0x04007333 RID: 29491
		private bool processing;

		// Token: 0x02000FCB RID: 4043
		public enum EjectType
		{
			// Token: 0x04007335 RID: 29493
			EjectFromVirtualStump,
			// Token: 0x04007336 RID: 29494
			ReturnToVirtualStump
		}
	}
}
