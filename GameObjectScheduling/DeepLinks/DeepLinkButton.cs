using System;
using System.Collections;
using UnityEngine;

namespace GameObjectScheduling.DeepLinks
{
	// Token: 0x020013FF RID: 5119
	public class DeepLinkButton : GorillaPressableButton
	{
		// Token: 0x06008106 RID: 33030 RVA: 0x0029EB73 File Offset: 0x0029CD73
		public override void ButtonActivation()
		{
			base.ButtonActivation();
			this.sendingDeepLink = DeepLinkSender.SendDeepLink(this.deepLinkAppID, this.deepLinkPayload, new Action<string>(this.OnDeepLinkSent));
			base.StartCoroutine(this.ButtonPressed_Local());
		}

		// Token: 0x06008107 RID: 33031 RVA: 0x0029EBAB File Offset: 0x0029CDAB
		private void OnDeepLinkSent(string message)
		{
			this.sendingDeepLink = false;
			if (!this.isOn)
			{
				this.UpdateColor();
			}
		}

		// Token: 0x06008108 RID: 33032 RVA: 0x0029EBC2 File Offset: 0x0029CDC2
		private IEnumerator ButtonPressed_Local()
		{
			this.isOn = true;
			this.UpdateColor();
			yield return new WaitForSeconds(this.pressedTime);
			this.isOn = false;
			if (!this.sendingDeepLink)
			{
				this.UpdateColor();
			}
			yield break;
		}

		// Token: 0x04009204 RID: 37380
		[SerializeField]
		private ulong deepLinkAppID;

		// Token: 0x04009205 RID: 37381
		[SerializeField]
		private string deepLinkPayload = "";

		// Token: 0x04009206 RID: 37382
		[SerializeField]
		private float pressedTime = 0.2f;

		// Token: 0x04009207 RID: 37383
		private bool sendingDeepLink;
	}
}
