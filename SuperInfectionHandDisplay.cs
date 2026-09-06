using System;
using UnityEngine;

// Token: 0x02000182 RID: 386
public class SuperInfectionHandDisplay : MonoBehaviour
{
	// Token: 0x06000A54 RID: 2644 RVA: 0x000376E0 File Offset: 0x000358E0
	public void EnableHands(bool on)
	{
		for (int i = 0; i < this.gameObjects.Length; i++)
		{
			this.gameObjects[i].SetActive(on);
		}
	}

	// Token: 0x04000C7E RID: 3198
	[SerializeField]
	private GameObject[] gameObjects;
}
