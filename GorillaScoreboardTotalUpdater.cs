using System;
using System.Collections.Generic;
using GorillaNetworking;
using GorillaTag;
using UnityEngine;

// Token: 0x02000A37 RID: 2615
public class GorillaScoreboardTotalUpdater : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x1700065A RID: 1626
	// (get) Token: 0x06004325 RID: 17189 RVA: 0x00165880 File Offset: 0x00163A80
	public static GorillaScoreboardTotalUpdater instance
	{
		get
		{
			GorillaScoreboardTotalUpdater gorillaScoreboardTotalUpdater;
			if ((gorillaScoreboardTotalUpdater = GorillaScoreboardTotalUpdater._instance) == null)
			{
				gorillaScoreboardTotalUpdater = (GorillaScoreboardTotalUpdater._instance = GorillaScoreboardTotalUpdater.CreateManager());
			}
			return gorillaScoreboardTotalUpdater;
		}
	}

	// Token: 0x1700065B RID: 1627
	// (get) Token: 0x06004326 RID: 17190 RVA: 0x00165896 File Offset: 0x00163A96
	public static bool hasInstance
	{
		get
		{
			return GorillaScoreboardTotalUpdater.instance != null;
		}
	}

	// Token: 0x06004327 RID: 17191 RVA: 0x001658A4 File Offset: 0x00163AA4
	public void UpdateLineState(GorillaPlayerScoreboardLine line)
	{
		if (line.playerActorNumber == -1)
		{
			return;
		}
		if (this.reportDict.ContainsKey(line.playerActorNumber))
		{
			this.reportDict[line.playerActorNumber] = new GorillaScoreboardTotalUpdater.PlayerReports(this.reportDict[line.playerActorNumber], line);
			return;
		}
		this.reportDict.Add(line.playerActorNumber, new GorillaScoreboardTotalUpdater.PlayerReports(line));
	}

	// Token: 0x06004328 RID: 17192 RVA: 0x0016590E File Offset: 0x00163B0E
	protected void Awake()
	{
		if (GorillaScoreboardTotalUpdater._instance == this)
		{
			return;
		}
		if (!(GorillaScoreboardTotalUpdater._instance == null))
		{
			Object.Destroy(this);
			return;
		}
		GorillaScoreboardTotalUpdater._instance = this;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(this);
			return;
		}
	}

	// Token: 0x06004329 RID: 17193 RVA: 0x00165948 File Offset: 0x00163B48
	private void Start()
	{
		RoomSystem.JoinedRoomEvent += new Action(this.JoinedRoom);
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
		RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerEnteredRoom);
		RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
	}

	// Token: 0x0600432A RID: 17194 RVA: 0x001659C4 File Offset: 0x00163BC4
	private static GorillaScoreboardTotalUpdater CreateManager()
	{
		GorillaScoreboardTotalUpdater gorillaScoreboardTotalUpdater = new GameObject("GorillaScoreboardTotalUpdater").AddComponent<GorillaScoreboardTotalUpdater>();
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(gorillaScoreboardTotalUpdater);
		}
		return gorillaScoreboardTotalUpdater;
	}

	// Token: 0x0600432B RID: 17195 RVA: 0x001659EF File Offset: 0x00163BEF
	public static void RegisterSL(GorillaPlayerScoreboardLine sL)
	{
		if (!GorillaScoreboardTotalUpdater.allScoreboardLines.Contains(sL))
		{
			GorillaScoreboardTotalUpdater.allScoreboardLines.Add(sL);
		}
	}

	// Token: 0x0600432C RID: 17196 RVA: 0x00165A09 File Offset: 0x00163C09
	public static void UnregisterSL(GorillaPlayerScoreboardLine sL)
	{
		if (GorillaScoreboardTotalUpdater.allScoreboardLines.Contains(sL))
		{
			GorillaScoreboardTotalUpdater.allScoreboardLines.Remove(sL);
		}
	}

	// Token: 0x0600432D RID: 17197 RVA: 0x00165A24 File Offset: 0x00163C24
	public static void RegisterScoreboard(GorillaScoreBoard sB)
	{
		if (!GorillaScoreboardTotalUpdater.allScoreboards.Contains(sB))
		{
			GorillaScoreboardTotalUpdater.allScoreboards.Add(sB);
			GorillaScoreboardTotalUpdater.instance.UpdateScoreboard(sB);
		}
	}

	// Token: 0x0600432E RID: 17198 RVA: 0x00165A49 File Offset: 0x00163C49
	public static void UnregisterScoreboard(GorillaScoreBoard sB)
	{
		if (GorillaScoreboardTotalUpdater.allScoreboards.Contains(sB))
		{
			GorillaScoreboardTotalUpdater.allScoreboards.Remove(sB);
		}
	}

	// Token: 0x0600432F RID: 17199 RVA: 0x00165A64 File Offset: 0x00163C64
	public void UpdateActiveScoreboards()
	{
		for (int i = 0; i < GorillaScoreboardTotalUpdater.allScoreboards.Count; i++)
		{
			this.UpdateScoreboard(GorillaScoreboardTotalUpdater.allScoreboards[i]);
		}
	}

	// Token: 0x06004330 RID: 17200 RVA: 0x00165A97 File Offset: 0x00163C97
	public void SetOfflineFailureText(string failureText)
	{
		this.offlineTextErrorString = failureText;
		this.UpdateActiveScoreboards();
	}

	// Token: 0x06004331 RID: 17201 RVA: 0x00165AA6 File Offset: 0x00163CA6
	public void ClearOfflineFailureText()
	{
		this.offlineTextErrorString = null;
		this.UpdateActiveScoreboards();
	}

	// Token: 0x06004332 RID: 17202 RVA: 0x00165AB8 File Offset: 0x00163CB8
	public void UpdateScoreboard(GorillaScoreBoard sB)
	{
		sB.SetSleepState(this.joinedRoom);
		if (GorillaComputer.instance == null)
		{
			return;
		}
		if (!this.joinedRoom)
		{
			if (sB.notInRoomText != null)
			{
				sB.notInRoomText.gameObject.SetActive(true);
				sB.notInRoomText.text = ((this.offlineTextErrorString != null) ? this.offlineTextErrorString : GorillaComputer.instance.offlineTextInitialString);
			}
			for (int i = 0; i < sB.lines.Count; i++)
			{
				sB.lines[i].ResetData();
			}
			sB.CleanupRoomControls();
			return;
		}
		if (sB.notInRoomText != null)
		{
			sB.notInRoomText.gameObject.SetActive(false);
		}
		for (int j = 0; j < sB.lines.Count; j++)
		{
			GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine = sB.lines[j];
			if (j < this.playersInRoom.Count)
			{
				gorillaPlayerScoreboardLine.gameObject.SetActive(true);
				gorillaPlayerScoreboardLine.SetLineData(this.playersInRoom[j]);
			}
			else
			{
				gorillaPlayerScoreboardLine.ResetData();
				gorillaPlayerScoreboardLine.gameObject.SetActive(false);
			}
		}
		sB.RedrawPlayerLines();
	}

	// Token: 0x06004333 RID: 17203 RVA: 0x00019260 File Offset: 0x00017460
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06004334 RID: 17204 RVA: 0x00019269 File Offset: 0x00017469
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06004335 RID: 17205 RVA: 0x00165BE8 File Offset: 0x00163DE8
	public void SliceUpdate()
	{
		if (GorillaScoreboardTotalUpdater.allScoreboardLines.Count == 0)
		{
			return;
		}
		for (int i = 0; i < 2; i++)
		{
			if (GorillaScoreboardTotalUpdater.lineIndex >= GorillaScoreboardTotalUpdater.allScoreboardLines.Count)
			{
				GorillaScoreboardTotalUpdater.lineIndex = 0;
			}
			GorillaScoreboardTotalUpdater.allScoreboardLines[GorillaScoreboardTotalUpdater.lineIndex].UpdateLine();
			GorillaScoreboardTotalUpdater.lineIndex++;
		}
		for (int j = 0; j < GorillaScoreboardTotalUpdater.allScoreboards.Count; j++)
		{
			if (GorillaScoreboardTotalUpdater.allScoreboards[j].IsDirty)
			{
				this.UpdateScoreboard(GorillaScoreboardTotalUpdater.allScoreboards[j]);
			}
		}
	}

	// Token: 0x06004336 RID: 17206 RVA: 0x00165C7D File Offset: 0x00163E7D
	private void OnPlayerEnteredRoom(NetPlayer netPlayer)
	{
		if (netPlayer == null)
		{
			Debug.LogError("Null netplayer");
		}
		if (!this.playersInRoom.Contains(netPlayer))
		{
			this.playersInRoom.Add(netPlayer);
		}
		this.UpdateActiveScoreboards();
	}

	// Token: 0x06004337 RID: 17207 RVA: 0x00165CAC File Offset: 0x00163EAC
	private void OnPlayerLeftRoom(NetPlayer netPlayer)
	{
		if (netPlayer == null)
		{
			Debug.LogError("Null NetPlayer.");
		}
		this.playersInRoom.Remove(netPlayer);
		this.UpdateActiveScoreboards();
		ReportMuteTimer reportMuteTimer;
		if (GorillaScoreboardTotalUpdater.m_reportMuteTimerDict.TryGetValue(netPlayer.ActorNumber, out reportMuteTimer))
		{
			GorillaScoreboardTotalUpdater.m_reportMuteTimerDict.Remove(netPlayer.ActorNumber);
			GorillaScoreboardTotalUpdater.m_reportMuteTimerPool.Return(reportMuteTimer);
		}
	}

	// Token: 0x06004338 RID: 17208 RVA: 0x00165D0C File Offset: 0x00163F0C
	internal void JoinedRoom()
	{
		this.joinedRoom = true;
		foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
		{
			this.playersInRoom.Add(netPlayer);
		}
		this.playersInRoom.Sort((NetPlayer x, NetPlayer y) => x.ActorNumber.CompareTo(y.ActorNumber));
		foreach (GorillaScoreBoard gorillaScoreBoard in GorillaScoreboardTotalUpdater.allScoreboards)
		{
			this.UpdateScoreboard(gorillaScoreBoard);
		}
	}

	// Token: 0x06004339 RID: 17209 RVA: 0x00165DBC File Offset: 0x00163FBC
	private void OnLeftRoom()
	{
		this.joinedRoom = false;
		this.playersInRoom.Clear();
		this.reportDict.Clear();
		foreach (GorillaScoreBoard gorillaScoreBoard in GorillaScoreboardTotalUpdater.allScoreboards)
		{
			this.UpdateScoreboard(gorillaScoreBoard);
		}
		foreach (KeyValuePair<int, ReportMuteTimer> keyValuePair in GorillaScoreboardTotalUpdater.m_reportMuteTimerDict)
		{
			GorillaScoreboardTotalUpdater.m_reportMuteTimerPool.Return(keyValuePair.Value);
		}
		GorillaScoreboardTotalUpdater.m_reportMuteTimerDict.Clear();
	}

	// Token: 0x0600433A RID: 17210 RVA: 0x00165E80 File Offset: 0x00164080
	public static void ReportMute(NetPlayer player, int muted)
	{
		ReportMuteTimer reportMuteTimer;
		if (GorillaScoreboardTotalUpdater.m_reportMuteTimerDict.TryGetValue(player.ActorNumber, out reportMuteTimer))
		{
			reportMuteTimer.Muted = muted;
			if (!reportMuteTimer.Running)
			{
				reportMuteTimer.Start();
			}
			return;
		}
		reportMuteTimer = GorillaScoreboardTotalUpdater.m_reportMuteTimerPool.Take();
		reportMuteTimer.SetReportData(player.UserId, player.NickName, muted);
		reportMuteTimer.coolDown = 5f;
		reportMuteTimer.Start();
		GorillaScoreboardTotalUpdater.m_reportMuteTimerDict[player.ActorNumber] = reportMuteTimer;
	}

	// Token: 0x04005506 RID: 21766
	private static GorillaScoreboardTotalUpdater _instance = null;

	// Token: 0x04005507 RID: 21767
	public static readonly List<GorillaPlayerScoreboardLine> allScoreboardLines = new List<GorillaPlayerScoreboardLine>();

	// Token: 0x04005508 RID: 21768
	public static int lineIndex = 0;

	// Token: 0x04005509 RID: 21769
	private const int linesPerFrame = 2;

	// Token: 0x0400550A RID: 21770
	public static List<GorillaScoreBoard> allScoreboards = new List<GorillaScoreBoard>();

	// Token: 0x0400550B RID: 21771
	private List<NetPlayer> playersInRoom = new List<NetPlayer>();

	// Token: 0x0400550C RID: 21772
	private bool joinedRoom;

	// Token: 0x0400550D RID: 21773
	private bool wasGameManagerNull;

	// Token: 0x0400550E RID: 21774
	public string offlineTextErrorString;

	// Token: 0x0400550F RID: 21775
	public Dictionary<int, GorillaScoreboardTotalUpdater.PlayerReports> reportDict = new Dictionary<int, GorillaScoreboardTotalUpdater.PlayerReports>();

	// Token: 0x04005510 RID: 21776
	private static readonly Dictionary<int, ReportMuteTimer> m_reportMuteTimerDict = new Dictionary<int, ReportMuteTimer>(20);

	// Token: 0x04005511 RID: 21777
	private static readonly ObjectPool<ReportMuteTimer> m_reportMuteTimerPool = new ObjectPool<ReportMuteTimer>(20);

	// Token: 0x02000A38 RID: 2616
	public struct PlayerReports
	{
		// Token: 0x0600433D RID: 17213 RVA: 0x00165F50 File Offset: 0x00164150
		public PlayerReports(GorillaScoreboardTotalUpdater.PlayerReports reportToUpdate, GorillaPlayerScoreboardLine lineToUpdate)
		{
			this.cheating = reportToUpdate.cheating || lineToUpdate.reportedCheating;
			this.toxicity = reportToUpdate.toxicity || lineToUpdate.reportedToxicity;
			this.hateSpeech = reportToUpdate.hateSpeech || lineToUpdate.reportedHateSpeech;
			this.pressedReport = lineToUpdate.reportInProgress;
		}

		// Token: 0x0600433E RID: 17214 RVA: 0x00165FAE File Offset: 0x001641AE
		public PlayerReports(GorillaPlayerScoreboardLine lineToUpdate)
		{
			this.cheating = lineToUpdate.reportedCheating;
			this.toxicity = lineToUpdate.reportedToxicity;
			this.hateSpeech = lineToUpdate.reportedHateSpeech;
			this.pressedReport = lineToUpdate.reportInProgress;
		}

		// Token: 0x04005512 RID: 21778
		public bool cheating;

		// Token: 0x04005513 RID: 21779
		public bool toxicity;

		// Token: 0x04005514 RID: 21780
		public bool hateSpeech;

		// Token: 0x04005515 RID: 21781
		public bool pressedReport;
	}
}
