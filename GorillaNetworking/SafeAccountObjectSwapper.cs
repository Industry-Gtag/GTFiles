using System;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020010CF RID: 4303
	public class SafeAccountObjectSwapper : MonoBehaviour
	{
		// Token: 0x06006B85 RID: 27525 RVA: 0x0022A9F6 File Offset: 0x00228BF6
		public void Start()
		{
			if (PlayFabAuthenticator.instance.GetSafety())
			{
				this.SwitchToSafeMode();
			}
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			instance.OnSafetyUpdate = (Action<bool>)Delegate.Combine(instance.OnSafetyUpdate, new Action<bool>(this.SafeAccountUpdated));
		}

		// Token: 0x06006B86 RID: 27526 RVA: 0x0022AA34 File Offset: 0x00228C34
		public void SafeAccountUpdated(bool isSafety)
		{
			if (isSafety)
			{
				this.SwitchToSafeMode();
			}
		}

		// Token: 0x06006B87 RID: 27527 RVA: 0x0022AA40 File Offset: 0x00228C40
		public void SwitchToSafeMode()
		{
			foreach (GameObject gameObject in this.UnSafeGameObjects)
			{
				if (gameObject != null)
				{
					gameObject.SetActive(false);
				}
			}
			foreach (GameObject gameObject2 in this.UnSafeTexts)
			{
				if (gameObject2 != null)
				{
					gameObject2.SetActive(false);
				}
			}
			foreach (GameObject gameObject3 in this.SafeTexts)
			{
				if (gameObject3 != null)
				{
					gameObject3.SetActive(true);
				}
			}
			foreach (GameObject gameObject4 in this.SafeModeObjects)
			{
				if (gameObject4 != null)
				{
					gameObject4.SetActive(true);
				}
			}
		}

		// Token: 0x04007B09 RID: 31497
		public GameObject[] UnSafeGameObjects;

		// Token: 0x04007B0A RID: 31498
		public GameObject[] UnSafeTexts;

		// Token: 0x04007B0B RID: 31499
		public GameObject[] SafeTexts;

		// Token: 0x04007B0C RID: 31500
		public GameObject[] SafeModeObjects;
	}
}
