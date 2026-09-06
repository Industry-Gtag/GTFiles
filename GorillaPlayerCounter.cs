using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200068D RID: 1677
public class GorillaPlayerCounter : MonoBehaviour
{
	// Token: 0x060029F4 RID: 10740 RVA: 0x000E2574 File Offset: 0x000E0774
	private void Awake()
	{
		this.text = base.gameObject.GetComponent<Text>();
	}

	// Token: 0x060029F5 RID: 10741 RVA: 0x000E2588 File Offset: 0x000E0788
	private void Update()
	{
		if (PhotonNetwork.CurrentRoom != null)
		{
			int num = 0;
			foreach (KeyValuePair<int, Player> keyValuePair in PhotonNetwork.CurrentRoom.Players)
			{
				if ((bool)keyValuePair.Value.CustomProperties["isRedTeam"] == this.isRedTeam)
				{
					num++;
				}
			}
			this.text.text = num.ToString();
		}
	}

	// Token: 0x04003683 RID: 13955
	public bool isRedTeam;

	// Token: 0x04003684 RID: 13956
	public Text text;

	// Token: 0x04003685 RID: 13957
	public string attribute;
}
