using System;
using System.Text;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000841 RID: 2113
public class GRToolStatusWatch : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x0600364D RID: 13901 RVA: 0x0012C1CC File Offset: 0x0012A3CC
	public void OnEntityInit()
	{
		if (this.gameEntity == null)
		{
			this.gameEntity = base.GetComponent<GameEntity>();
		}
		this.UpdateVisuals();
		this.progression = this.gameEntity.manager.GetComponent<GhostReactorManager>().reactor.toolProgression;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnSnapped = (Action)Delegate.Combine(gameEntity.OnSnapped, new Action(this.UpdateSnappedPlayer));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnUnsnapped = (Action)Delegate.Combine(gameEntity2.OnUnsnapped, new Action(this.RemoveSnappedPlayer));
	}

	// Token: 0x0600364E RID: 13902 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x0600364F RID: 13903 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long newState)
	{
	}

	// Token: 0x06003650 RID: 13904 RVA: 0x0012C268 File Offset: 0x0012A468
	public void UpdateSnappedPlayer()
	{
		this.currentPlayer = GRPlayer.Get(this.gameEntity.snappedByActorNumber);
		this.lastKills = -1;
		this.lastCredits = -1;
		this.lastJuice = -1;
		this.lastGrade = -1;
		if (this.currentPlayer == GRPlayer.GetLocal())
		{
			this.state = GRToolStatusWatch.WatchState.SnappedLocal;
		}
		else
		{
			this.state = GRToolStatusWatch.WatchState.SnappedRemote;
		}
		this.disabledText.text = "LEAVE ME ALONE!\n\nTHIS IS ONLY FOR MY OWNER!!!";
		this.UpdateVisuals();
	}

	// Token: 0x06003651 RID: 13905 RVA: 0x0012C2DF File Offset: 0x0012A4DF
	public void RemoveSnappedPlayer()
	{
		this.currentPlayer = null;
		this.state = GRToolStatusWatch.WatchState.Dropped;
		this.disabledText.text = "LOW POWER\n\nPUT ME ON";
		this.UpdateVisuals();
	}

	// Token: 0x06003652 RID: 13906 RVA: 0x0012C305 File Offset: 0x0012A505
	private void Update()
	{
		if (this.currentPlayer == null)
		{
			return;
		}
		this.UpdateVisuals();
	}

	// Token: 0x06003653 RID: 13907 RVA: 0x0012C31C File Offset: 0x0012A51C
	private void UpdateVisuals()
	{
		bool flag = this.state == GRToolStatusWatch.WatchState.SnappedLocal || this.state == GRToolStatusWatch.WatchState.SnappedRemote;
		if (this.disabledVisuals.activeSelf == flag)
		{
			this.disabledVisuals.SetActive(!flag);
		}
		if (this.enabledVisuals.activeSelf != flag)
		{
			this.enabledVisuals.SetActive(flag);
		}
		if (this.state != GRToolStatusWatch.WatchState.SnappedLocal)
		{
			return;
		}
		if (this.visibleHP != this.currentPlayer.Hp / 100)
		{
			this.visibleHP = this.currentPlayer.Hp / 100;
			for (int i = 0; i < this.healthHearts.Length; i++)
			{
				if (this.healthHearts[i].activeSelf != i < this.visibleHP)
				{
					this.healthHearts[i].SetActive(i < this.visibleHP);
				}
			}
		}
		if (this.visibleShield != this.currentPlayer.ShieldHp / 100)
		{
			this.visibleShield = this.currentPlayer.ShieldHp / 100;
			if (this.shieldSymbol.activeSelf != this.visibleShield > 0)
			{
				this.shieldSymbol.SetActive(this.visibleShield > 0);
			}
		}
		this.gimbaledCompass.LookAt(this.homeBase, Vector3.up);
		int num = (int)this.currentPlayer.synchronizedSessionStats[5];
		int shiftCredits = this.currentPlayer.ShiftCredits;
		int numberOfResearchPoints = this.progression.GetNumberOfResearchPoints();
		ValueTuple<int, int, int, int> gradePointDetails = GhostReactorProgression.GetGradePointDetails(this.currentPlayer.CurrentProgression.redeemedPoints);
		int item = gradePointDetails.Item1;
		int item2 = gradePointDetails.Item2;
		if (num == this.lastKills && shiftCredits == this.lastCredits && numberOfResearchPoints == this.lastJuice && item2 == this.lastGrade)
		{
			return;
		}
		this.sb.Clear();
		this.sb.Append(num);
		this.sb.Append("\n\n");
		this.sb.Append(numberOfResearchPoints);
		this.sb.Append("\n\n");
		this.sb.Append(shiftCredits);
		this.sb.Append("\n\n\n");
		this.sb.Append(GhostReactorProgression.GetTitleNameFromLevel(item)[0]);
		this.sb.Append(item2);
		this.statsText.text = this.sb.ToString();
		this.lastKills = num;
		this.lastCredits = shiftCredits;
		this.lastJuice = numberOfResearchPoints;
		this.lastGrade = item2;
	}

	// Token: 0x04004703 RID: 18179
	public GameEntity gameEntity;

	// Token: 0x04004704 RID: 18180
	private GRPlayer currentPlayer;

	// Token: 0x04004705 RID: 18181
	private int visibleHP;

	// Token: 0x04004706 RID: 18182
	private int visibleShield;

	// Token: 0x04004707 RID: 18183
	public GameObject disabledVisuals;

	// Token: 0x04004708 RID: 18184
	public GameObject enabledVisuals;

	// Token: 0x04004709 RID: 18185
	public GameObject[] healthHearts;

	// Token: 0x0400470A RID: 18186
	public GameObject shieldSymbol;

	// Token: 0x0400470B RID: 18187
	public Vector3 homeBase;

	// Token: 0x0400470C RID: 18188
	public Transform gimbaledCompass;

	// Token: 0x0400470D RID: 18189
	public TextMeshPro statsText;

	// Token: 0x0400470E RID: 18190
	public TextMeshPro disabledText;

	// Token: 0x0400470F RID: 18191
	private int lastKills;

	// Token: 0x04004710 RID: 18192
	private int lastCredits;

	// Token: 0x04004711 RID: 18193
	private int lastJuice;

	// Token: 0x04004712 RID: 18194
	private int lastGrade;

	// Token: 0x04004713 RID: 18195
	private StringBuilder sb = new StringBuilder();

	// Token: 0x04004714 RID: 18196
	private GRToolStatusWatch.WatchState state;

	// Token: 0x04004715 RID: 18197
	private GRToolProgressionManager progression;

	// Token: 0x02000842 RID: 2114
	private enum WatchState
	{
		// Token: 0x04004717 RID: 18199
		Dropped,
		// Token: 0x04004718 RID: 18200
		SnappedLocal,
		// Token: 0x04004719 RID: 18201
		SnappedRemote
	}
}
