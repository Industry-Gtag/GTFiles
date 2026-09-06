using System;
using UnityEngine;

// Token: 0x02000BDE RID: 3038
public class KIDUI_TooYoungToPlay : MonoBehaviour
{
	// Token: 0x06004C8A RID: 19594 RVA: 0x00191956 File Offset: 0x0018FB56
	public void ShowTooYoungToPlayScreen()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x06004C8B RID: 19595 RVA: 0x00193E22 File Offset: 0x00192022
	public void OnQuitPressed()
	{
		Application.Quit();
	}
}
