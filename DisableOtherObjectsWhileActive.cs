using System;
using UnityEngine;

// Token: 0x0200032F RID: 815
public class DisableOtherObjectsWhileActive : MonoBehaviour
{
	// Token: 0x0600143E RID: 5182 RVA: 0x0006D80D File Offset: 0x0006BA0D
	private void OnEnable()
	{
		this.SetAllActive(false);
	}

	// Token: 0x0600143F RID: 5183 RVA: 0x0006D816 File Offset: 0x0006BA16
	private void OnDisable()
	{
		this.SetAllActive(true);
	}

	// Token: 0x06001440 RID: 5184 RVA: 0x0006D820 File Offset: 0x0006BA20
	private void SetAllActive(bool active)
	{
		for (int i = 0; i < this.otherObjects.Length; i++)
		{
			GameObject gameObject = this.otherObjects[i];
			if (gameObject != null)
			{
				gameObject.SetActive(active);
			}
		}
		for (int j = 0; j < this.otherXSceneObjects.Length; j++)
		{
			XSceneRef xsceneRef = this.otherXSceneObjects[j];
			GameObject gameObject2;
			if (xsceneRef.TryResolve(out gameObject2) && gameObject2 != null)
			{
				gameObject2.SetActive(active);
			}
		}
	}

	// Token: 0x0400190F RID: 6415
	public const string preErr = "[GT/DisableOtherObjectsWhileActive]  ERROR!!!  ";

	// Token: 0x04001910 RID: 6416
	public GameObject[] otherObjects;

	// Token: 0x04001911 RID: 6417
	public XSceneRef[] otherXSceneObjects;
}
