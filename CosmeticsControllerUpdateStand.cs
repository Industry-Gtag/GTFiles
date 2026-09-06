using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200050C RID: 1292
public class CosmeticsControllerUpdateStand : MonoBehaviour
{
	// Token: 0x06002052 RID: 8274 RVA: 0x000ADE44 File Offset: 0x000AC044
	public GameObject ReturnChildWithCosmeticNameMatch(Transform parentTransform)
	{
		GameObject gameObject = null;
		using (IEnumerator enumerator = parentTransform.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Transform child = (Transform)enumerator.Current;
				if (child.gameObject.activeInHierarchy && this.cosmeticsController.allCosmetics.FindIndex((CosmeticsController.CosmeticItem x) => child.name == x.itemName) > -1)
				{
					return child.gameObject;
				}
				gameObject = this.ReturnChildWithCosmeticNameMatch(child);
				if (gameObject != null)
				{
					return gameObject;
				}
			}
		}
		return gameObject;
	}

	// Token: 0x04002B20 RID: 11040
	public CosmeticsController cosmeticsController;

	// Token: 0x04002B21 RID: 11041
	public bool FailEntitlement;

	// Token: 0x04002B22 RID: 11042
	public bool PlayerUnlocked;

	// Token: 0x04002B23 RID: 11043
	public bool ItemNotGrantedYet;

	// Token: 0x04002B24 RID: 11044
	public bool ItemSuccessfullyGranted;

	// Token: 0x04002B25 RID: 11045
	public bool AttemptToConsumeEntitlement;

	// Token: 0x04002B26 RID: 11046
	public bool EntitlementSuccessfullyConsumed;

	// Token: 0x04002B27 RID: 11047
	public bool LockSuccessfullyCleared;

	// Token: 0x04002B28 RID: 11048
	public bool RunDebug;

	// Token: 0x04002B29 RID: 11049
	public Transform textParent;

	// Token: 0x04002B2A RID: 11050
	private CosmeticsController.CosmeticItem outItem;

	// Token: 0x04002B2B RID: 11051
	public HeadModel[] inventoryHeadModels;

	// Token: 0x04002B2C RID: 11052
	public string headModelsPrefabPath;
}
