using System;
using System.Collections;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000A35 RID: 2613
public class GorillaScoreboardSpawner : MonoBehaviour
{
	// Token: 0x06004316 RID: 17174 RVA: 0x001655EB File Offset: 0x001637EB
	public void Awake()
	{
		base.StartCoroutine(this.UpdateBoard());
	}

	// Token: 0x06004317 RID: 17175 RVA: 0x001655FA File Offset: 0x001637FA
	private void Start()
	{
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
	}

	// Token: 0x06004318 RID: 17176 RVA: 0x00164494 File Offset: 0x00162694
	public bool IsCurrentScoreboard()
	{
		return base.gameObject.activeInHierarchy;
	}

	// Token: 0x06004319 RID: 17177 RVA: 0x00165634 File Offset: 0x00163834
	public void OnJoinedRoom()
	{
		Debug.Log("SCOREBOARD JOIN ROOM");
		if (this.IsCurrentScoreboard())
		{
			this.notInRoomText.SetActive(false);
			this.currentScoreboard = Object.Instantiate<GameObject>(this.scoreboardPrefab, base.transform).GetComponent<GorillaScoreBoard>();
			this.currentScoreboard.transform.rotation = base.transform.rotation;
			if (this.includeMMR)
			{
				this.currentScoreboard.GetComponent<GorillaScoreBoard>().includeMMR = true;
				this.currentScoreboard.GetComponent<Text>().text = "Player                     Color         Level        MMR";
			}
		}
	}

	// Token: 0x0600431A RID: 17178 RVA: 0x001656C4 File Offset: 0x001638C4
	public bool IsVisible()
	{
		if (!this.forOverlay)
		{
			return this.controllingParentGameObject.activeSelf;
		}
		return GTPlayer.Instance.inOverlay;
	}

	// Token: 0x0600431B RID: 17179 RVA: 0x001656E4 File Offset: 0x001638E4
	private IEnumerator UpdateBoard()
	{
		for (;;)
		{
			try
			{
				if (this.currentScoreboard != null)
				{
					bool flag = this.IsVisible();
					foreach (GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine in this.currentScoreboard.lines)
					{
						if (flag != gorillaPlayerScoreboardLine.lastVisible)
						{
							gorillaPlayerScoreboardLine.lastVisible = flag;
						}
					}
					if (this.currentScoreboard.boardText.enabled != flag)
					{
						this.currentScoreboard.boardText.enabled = flag;
					}
					if (this.currentScoreboard.buttonText.enabled != flag)
					{
						this.currentScoreboard.buttonText.enabled = flag;
					}
				}
			}
			catch
			{
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x0600431C RID: 17180 RVA: 0x001656F3 File Offset: 0x001638F3
	public void OnLeftRoom()
	{
		this.Cleanup();
		if (this.notInRoomText)
		{
			this.notInRoomText.SetActive(true);
		}
	}

	// Token: 0x0600431D RID: 17181 RVA: 0x00165714 File Offset: 0x00163914
	public void Cleanup()
	{
		if (this.currentScoreboard != null)
		{
			Object.Destroy(this.currentScoreboard.gameObject);
			this.currentScoreboard = null;
		}
	}

	// Token: 0x040054FA RID: 21754
	public string gameType;

	// Token: 0x040054FB RID: 21755
	public bool includeMMR;

	// Token: 0x040054FC RID: 21756
	public GameObject scoreboardPrefab;

	// Token: 0x040054FD RID: 21757
	public GameObject notInRoomText;

	// Token: 0x040054FE RID: 21758
	public GameObject controllingParentGameObject;

	// Token: 0x040054FF RID: 21759
	public bool isActive = true;

	// Token: 0x04005500 RID: 21760
	public GorillaScoreBoard currentScoreboard;

	// Token: 0x04005501 RID: 21761
	public bool lastVisible;

	// Token: 0x04005502 RID: 21762
	public bool forOverlay;
}
