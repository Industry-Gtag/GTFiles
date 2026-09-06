using System;
using System.Collections.Generic;
using GorillaNetworking;
using GorillaTagScripts.GhostReactor.SoakTasks;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000726 RID: 1830
public class GhostReactorSoak
{
	// Token: 0x06002EA4 RID: 11940 RVA: 0x000FF45C File Offset: 0x000FD65C
	public void Setup(GRPlayer grPlayer)
	{
		this.grPlayer = grPlayer;
		GhostReactorSoak.instance = this;
		if (this.IsSoaking())
		{
			Debug.LogFormat("Soak Setup {0} InRoom {1} Auth {2}", new object[]
			{
				this.state,
				this.grManager != null && this.grManager.IsAuthority(),
				PhotonNetwork.InRoom
			});
		}
		this._soakTasks.Add(new SoakTaskGrabThrow(grPlayer));
		this._soakTasks.Add(new SoakTaskDepositCollectibles(grPlayer));
		this._soakTasks.Add(new SoakTaskBreakable(grPlayer));
		this._soakTasks.Add(new SoakTaskHitEnemy(grPlayer));
	}

	// Token: 0x06002EA5 RID: 11941 RVA: 0x00002076 File Offset: 0x00000276
	public bool IsSoaking()
	{
		return false;
	}

	// Token: 0x06002EA6 RID: 11942 RVA: 0x000FF514 File Offset: 0x000FD714
	public void OnUpdate()
	{
		if (!this.IsSoaking())
		{
			return;
		}
		GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this.grPlayer.gamePlayer.rig.zoneEntity.currentZone);
		if (managerForZone == null)
		{
			return;
		}
		this.grManager = managerForZone.ghostReactorManager;
		if (this.grManager == null)
		{
			return;
		}
		double timeAsDouble = Time.timeAsDouble;
		switch (this.state)
		{
		case GhostReactorSoak.State.Disconnected:
			if (!PhotonNetwork.InRoom && timeAsDouble > this.reconnectTime)
			{
				this.SetState(GhostReactorSoak.State.Connecting);
				return;
			}
			break;
		case GhostReactorSoak.State.Connecting:
			if (this.grManager.IsZoneActive())
			{
				this.SetState(GhostReactorSoak.State.Active);
				return;
			}
			if (timeAsDouble > this.stateStartTime + 15.0)
			{
				this.SetState(GhostReactorSoak.State.Disconnected);
				return;
			}
			break;
		case GhostReactorSoak.State.Active:
			this.UpdateActive();
			if (timeAsDouble > this.disconnectTime)
			{
				this.SetState(GhostReactorSoak.State.Disconnected);
				return;
			}
			if (!PhotonNetwork.InRoom)
			{
				this.SetState(GhostReactorSoak.State.Disconnected);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002EA7 RID: 11943 RVA: 0x000FF5FC File Offset: 0x000FD7FC
	private int GetActorNumber()
	{
		if (this.grPlayer.gamePlayer.rig.OwningNetPlayer == null)
		{
			return -1;
		}
		return this.grPlayer.gamePlayer.rig.OwningNetPlayer.ActorNumber;
	}

	// Token: 0x06002EA8 RID: 11944 RVA: 0x000FF634 File Offset: 0x000FD834
	public void SetState(GhostReactorSoak.State newState)
	{
		this.state = newState;
		this.stateStartTime = Time.timeAsDouble;
		Debug.LogFormat("Soak Set State {0} Player {1} InRoom {2} Auth {3}", new object[]
		{
			this.state,
			this.GetActorNumber(),
			this.grManager != null && this.grManager.IsAuthority(),
			PhotonNetwork.InRoom
		});
		switch (this.state)
		{
		case GhostReactorSoak.State.Disconnected:
			this.LeaveRoom();
			this.reconnectTime = this.stateStartTime + (double)Random.Range(3f, 6f);
			return;
		case GhostReactorSoak.State.Connecting:
			this.JoinRoom();
			return;
		case GhostReactorSoak.State.Active:
			this.disconnectTime = this.stateStartTime + (double)Random.Range(5f, 60f);
			return;
		default:
			return;
		}
	}

	// Token: 0x06002EA9 RID: 11945 RVA: 0x000FF712 File Offset: 0x000FD912
	public void JoinRoom()
	{
		Debug.LogFormat("Soak Join Room {0}", new object[] { "AKJSOAK" });
		PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("AKJSOAK", JoinType.Solo);
	}

	// Token: 0x06002EAA RID: 11946 RVA: 0x000FF73E File Offset: 0x000FD93E
	public void LeaveRoom()
	{
		Debug.LogFormat("Soak Leave Room", Array.Empty<object>());
		NetworkSystem.Instance.ReturnToSinglePlayer();
	}

	// Token: 0x06002EAB RID: 11947 RVA: 0x000FF75C File Offset: 0x000FD95C
	private void UpdateActive()
	{
		if (this._activeTask != null)
		{
			bool flag = false;
			if (!this._activeTask.Update())
			{
				Debug.LogError(string.Format("Failed to execute soak task of type {0}", this._activeTask.GetType()));
				flag = true;
			}
			if (flag || this._activeTask.Complete)
			{
				this._activeTask.Reset();
				this._activeTask = null;
				return;
			}
		}
		else if (Random.value <= 0.005f)
		{
			int num = Random.Range(0, this._soakTasks.Count);
			this._activeTask = this._soakTasks[num];
		}
	}

	// Token: 0x04003B64 RID: 15204
	public static GhostReactorSoak instance;

	// Token: 0x04003B65 RID: 15205
	private const string SOAK_ROOM = "AKJSOAK";

	// Token: 0x04003B66 RID: 15206
	private const float MIN_CONNECTED_TIME = 5f;

	// Token: 0x04003B67 RID: 15207
	private const float MAX_CONNECTED_TIME = 60f;

	// Token: 0x04003B68 RID: 15208
	private const float MIN_DISCONNECTED_TIME = 3f;

	// Token: 0x04003B69 RID: 15209
	private const float MAX_DISCONNECTED_TIME = 6f;

	// Token: 0x04003B6A RID: 15210
	public GRPlayer grPlayer;

	// Token: 0x04003B6B RID: 15211
	public GhostReactorManager grManager;

	// Token: 0x04003B6C RID: 15212
	public GhostReactorSoak.State state;

	// Token: 0x04003B6D RID: 15213
	public double stateStartTime;

	// Token: 0x04003B6E RID: 15214
	public double reconnectTime;

	// Token: 0x04003B6F RID: 15215
	public double disconnectTime;

	// Token: 0x04003B70 RID: 15216
	public const float START_NEW_TASK_ODDS = 0.005f;

	// Token: 0x04003B71 RID: 15217
	private IGhostReactorSoakTask _activeTask;

	// Token: 0x04003B72 RID: 15218
	private readonly List<IGhostReactorSoakTask> _soakTasks = new List<IGhostReactorSoakTask>();

	// Token: 0x02000727 RID: 1831
	public enum State
	{
		// Token: 0x04003B74 RID: 15220
		Disconnected,
		// Token: 0x04003B75 RID: 15221
		Connecting,
		// Token: 0x04003B76 RID: 15222
		Active
	}
}
