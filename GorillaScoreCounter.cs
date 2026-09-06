using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200068E RID: 1678
public class GorillaScoreCounter : MonoBehaviour
{
	// Token: 0x060029F7 RID: 10743 RVA: 0x000E261C File Offset: 0x000E081C
	private void Awake()
	{
		this.text = base.gameObject.GetComponent<Text>();
		if (this.isRedTeam)
		{
			this.attribute = "redScore";
			return;
		}
		this.attribute = "blueScore";
	}

	// Token: 0x060029F8 RID: 10744 RVA: 0x000E2650 File Offset: 0x000E0850
	private void Update()
	{
		if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties[this.attribute] != null)
		{
			this.text.text = ((int)PhotonNetwork.CurrentRoom.CustomProperties[this.attribute]).ToString();
		}
	}

	// Token: 0x04003686 RID: 13958
	public bool isRedTeam;

	// Token: 0x04003687 RID: 13959
	public Text text;

	// Token: 0x04003688 RID: 13960
	public string attribute;
}
