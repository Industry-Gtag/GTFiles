using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000504 RID: 1284
public class BetaChecker : MonoBehaviour
{
	// Token: 0x0600203B RID: 8251 RVA: 0x000AD753 File Offset: 0x000AB953
	private void Start()
	{
		if (PlayerPrefs.GetString("CheckedBox2") == "true")
		{
			this.doNotEnable = true;
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600203C RID: 8252 RVA: 0x000AD780 File Offset: 0x000AB980
	private void Update()
	{
		if (!this.doNotEnable)
		{
			if (CosmeticsController.instance.confirmedDidntPlayInBeta)
			{
				PlayerPrefs.SetString("CheckedBox2", "true");
				PlayerPrefs.Save();
				base.gameObject.SetActive(false);
				return;
			}
			if (CosmeticsController.instance.playedInBeta)
			{
				GameObject[] array = this.objectsToEnable;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(true);
				}
				this.doNotEnable = true;
			}
		}
	}

	// Token: 0x04002B0A RID: 11018
	public GameObject[] objectsToEnable;

	// Token: 0x04002B0B RID: 11019
	public bool doNotEnable;
}
