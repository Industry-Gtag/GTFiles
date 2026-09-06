using System;
using System.Collections.Generic;
using System.Text;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTagScripts;
using TMPro;
using UnityEngine;

// Token: 0x020008A2 RID: 2210
public class GorillaScoreBoard : MonoBehaviour
{
	// Token: 0x1700052C RID: 1324
	// (get) Token: 0x060039C5 RID: 14789 RVA: 0x0013AA00 File Offset: 0x00138C00
	public bool IsDirty
	{
		get
		{
			return this._isDirty || string.IsNullOrEmpty(this.initialGameMode);
		}
	}

	// Token: 0x060039C6 RID: 14790 RVA: 0x0013AA17 File Offset: 0x00138C17
	public void SetSleepState(bool awake)
	{
		this.boardText.enabled = awake;
		this.buttonText.enabled = awake;
		if (this.linesParent != null)
		{
			this.linesParent.SetActive(awake);
		}
		this.ToggleRoomControlButtons();
	}

	// Token: 0x060039C7 RID: 14791 RVA: 0x0013AA54 File Offset: 0x00138C54
	private string GetBeginningString()
	{
		string text = string.Format(" ({0})", 10);
		if (NetworkSystem.Instance.SessionIsSubscription)
		{
			text = string.Format(" ({0})", 20);
		}
		string text2 = ((GorillaComputer.instance != null) ? GorillaComputer.instance.GetVStumpRoomDisplayName(NetworkSystem.Instance.RoomName) : NetworkSystem.Instance.RoomName);
		return string.Concat(new string[]
		{
			"ROOM ID: ",
			NetworkSystem.Instance.SessionIsPrivate ? "-PRIVATE- GAME: " : (text2 + "   GAME: "),
			this.RoomType(),
			text,
			"\n  PLAYER     COLOR   MUTE  REPORT"
		});
	}

	// Token: 0x060039C8 RID: 14792 RVA: 0x0013AB0C File Offset: 0x00138D0C
	private string RoomType()
	{
		this.initialGameMode = RoomSystem.RoomGameMode;
		this.gmNames = GameMode.gameModeNames;
		this.gmName = "ERROR";
		int count = this.gmNames.Count;
		int num = this.initialGameMode.LastIndexOf('|');
		if (num >= 0)
		{
			this.tempGmName = this.initialGameMode.Substring(num + 1);
			for (int i = 0; i < count; i++)
			{
				if (this.tempGmName == this.gmNames[i])
				{
					this.gmName = this.tempGmName;
					break;
				}
			}
		}
		else
		{
			for (int j = 0; j < count; j++)
			{
				this.tempGmName = this.gmNames[j];
				if (this.initialGameMode.Contains(this.tempGmName))
				{
					this.gmName = this.tempGmName;
					break;
				}
			}
		}
		return this.gmName;
	}

	// Token: 0x060039C9 RID: 14793 RVA: 0x0013ABE8 File Offset: 0x00138DE8
	[ContextMenu("Toggle Room Controls")]
	public void ToggleRoomControls()
	{
		string text;
		if (!RoomControls.CanModerate(out text))
		{
			Debug.LogWarning("Cannot toggle room controls: " + text + ".");
			return;
		}
		this.roomControlsActive = !this.roomControlsActive;
		if (this.roomControlsActive)
		{
			this.weatherControlsActive = false;
		}
		this.SetDirty();
	}

	// Token: 0x060039CA RID: 14794 RVA: 0x0013AC38 File Offset: 0x00138E38
	public void CleanupRoomControls()
	{
		this.roomControlsActive = false;
		this.weatherControlsActive = false;
		this.rightPanel.SetActive(false);
		this.weatherControlsParent.SetActive(false);
		for (int i = 0; i < this.lines.Count; i++)
		{
			this.lines[i].ToggleRoomControlButtons(false, true);
		}
	}

	// Token: 0x060039CB RID: 14795 RVA: 0x0013AC94 File Offset: 0x00138E94
	private void ToggleRoomControlButtons()
	{
		bool flag = RoomControls.CanModerate();
		this.roomControlsToggle.gameObject.SetActive(flag);
		if (this.allowWeatherControls)
		{
			this.weatherControlsToggle.gameObject.SetActive(flag);
			return;
		}
		this.weatherControlsToggle.gameObject.SetActive(false);
	}

	// Token: 0x060039CC RID: 14796 RVA: 0x0013ACE4 File Offset: 0x00138EE4
	[ContextMenu("Toggle Weather Controls")]
	public void ToggleWeatherControls()
	{
		string text;
		if (!RoomControls.CanModerate(out text))
		{
			Debug.LogWarning("Cannot toggle weather controls: " + text + ".");
			return;
		}
		this.weatherControlsActive = !this.weatherControlsActive;
		Debug.Log(string.Format("Weather controls active: {0}", this.weatherControlsActive));
		if (this.weatherControlsActive)
		{
			this.roomControlsActive = false;
		}
		this.SetDirty();
	}

	// Token: 0x060039CD RID: 14797 RVA: 0x0013AD50 File Offset: 0x00138F50
	public void RedrawPlayerLines()
	{
		this.stringBuilder.Clear();
		this.stringBuilder.Append(this.GetBeginningString());
		this.buttonStringBuilder.Clear();
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
		int num = 0;
		for (int i = 0; i < this.lines.Count; i++)
		{
			if (this.lines[i].gameObject.activeInHierarchy)
			{
				num++;
			}
		}
		if (this.roomControlsActive || this.weatherControlsActive)
		{
			this.leftPanel.transform.localScale = new Vector3(0.7f, 1f, 1f);
			this.leftPanel.transform.localPosition = new Vector3(this.leftPanelRoomControlXOffset, 0f);
			this.rightPanel.SetActive(true);
			if (this.roomControlsActive)
			{
				this.roomControlsText.gameObject.SetActive(true);
				this.weatherControlsParent.SetActive(false);
			}
			else if (this.weatherControlsActive)
			{
				this.roomControlsText.gameObject.SetActive(false);
				this.weatherControlsParent.SetActive(true);
			}
		}
		else
		{
			this.leftPanel.transform.localScale = Vector3.one;
			this.leftPanel.transform.localPosition = Vector3.zero;
			this.rightPanel.SetActive(false);
		}
		if (num > 10)
		{
			this.linesParent.transform.localScale = new Vector3(1f, 0.5f, 1f);
			this.linesParent.transform.localPosition = new Vector3(0f, this.bigRoomYOffset, 0f);
			this.textsParent.transform.localScale = new Vector3(1f, 0.5f, 1f);
		}
		else
		{
			this.linesParent.transform.localScale = Vector3.one;
			this.linesParent.transform.localPosition = Vector3.zero;
			this.textsParent.transform.localScale = Vector3.one;
		}
		for (int j = 0; j < this.lines.Count; j++)
		{
			if (this.lines[j].IsLineActive())
			{
				this.linesRTs[j].localPosition = new Vector3(0f, (float)(this.startingYValue - this.lineHeight * j), 0f);
				if (this.lines[j].IsPlayerInRoom())
				{
					this.stringBuilder.Append("\n ");
					SubscriptionManager.SubscriptionDetails subscriptionDetails = SubscriptionManager.GetSubscriptionDetails(this.lines[j].linePlayer);
					if (subscriptionDetails.active && subscriptionDetails.tier > 0)
					{
						this.stringBuilder.Append("<color=#ffc600>");
					}
					else
					{
						this.stringBuilder.Append("<color=#ffffff>");
					}
					this.stringBuilder.Append(flag ? this.lines[j].playerNameVisible : this.lines[j].linePlayer.DefaultName);
					this.stringBuilder.Append("</color>");
					if (this.lines[j].linePlayer != NetworkSystem.Instance.LocalPlayer)
					{
						bool flag2 = this.lines[j].IsReportButtonActive();
						if (flag2)
						{
							if (!this.roomControlsActive)
							{
								this.buttonStringBuilder.Append("MUTE                                REPORT\n");
							}
							else if (!this.lines[j].IsConfirmButtonsActive())
							{
								this.buttonStringBuilder.Append("MUTE                                REPORT                              SILENCE                      KICK                      BLOCK\n");
							}
							else if (this.lines[j].IsConfirmParentKick())
							{
								this.buttonStringBuilder.Append("MUTE                                REPORT                              SILENCE           CONFIRM            CANCEL\n");
							}
							else
							{
								this.buttonStringBuilder.Append("MUTE                                REPORT                              SILENCE                                      CONFIRM            CANCEL\n");
							}
						}
						else
						{
							this.buttonStringBuilder.Append("MUTE                HATE SPEECH    TOXICITY     CHEATING       CANCEL\n");
						}
						this.lines[j].ToggleRoomControlButtons(this.roomControlsActive && flag2, false);
					}
					else
					{
						this.buttonStringBuilder.Append("\n");
					}
				}
			}
		}
		this.boardText.text = this.stringBuilder.ToString();
		this.buttonText.text = this.buttonStringBuilder.ToString();
		this.UpdateWeatherText();
		this._isDirty = false;
	}

	// Token: 0x060039CE RID: 14798 RVA: 0x0013B1A8 File Offset: 0x001393A8
	private void Awake()
	{
		this.linesRTs.Clear();
		for (int i = 0; i < this.lines.Count; i++)
		{
			this.linesRTs.Add(this.lines[i].GetComponent<RectTransform>());
		}
	}

	// Token: 0x060039CF RID: 14799 RVA: 0x0013B1F2 File Offset: 0x001393F2
	private void Start()
	{
		GorillaScoreboardTotalUpdater.RegisterScoreboard(this);
		this.CheckZoneForControls();
	}

	// Token: 0x060039D0 RID: 14800 RVA: 0x0013B200 File Offset: 0x00139400
	private void CheckZoneForControls()
	{
		ZoneDef zoneDef = ZoneGraphBSP.Instance.FindZoneAtPoint(base.transform.position);
		this.allowWeatherControls = false;
		for (int i = 0; i < this.allowedWeatherControlZones.Length; i++)
		{
			if (zoneDef.zoneId == this.allowedWeatherControlZones[i])
			{
				this.allowWeatherControls = true;
				return;
			}
		}
	}

	// Token: 0x060039D1 RID: 14801 RVA: 0x0013B258 File Offset: 0x00139458
	private void OnEnable()
	{
		GorillaScoreboardTotalUpdater.RegisterScoreboard(this);
		this.SetDirty();
		SubscriptionManager.OnSubscriptionData = (Action)Delegate.Remove(SubscriptionManager.OnSubscriptionData, new Action(this.OnSubscribeReady));
		SubscriptionManager.OnSubscriptionData = (Action)Delegate.Combine(SubscriptionManager.OnSubscriptionData, new Action(this.OnSubscribeReady));
		NetworkSystem.Instance.OnMasterClientSwitchedEvent -= this.OnMasterClientSwitched;
		NetworkSystem.Instance.OnMasterClientSwitchedEvent += this.OnMasterClientSwitched;
		RoomControls.OnRoomControlsEnabledChanged -= new Action<bool>(this.OnRoomControlsEnabledChanged);
		RoomControls.OnRoomControlsEnabledChanged += new Action<bool>(this.OnRoomControlsEnabledChanged);
		this.roomControlsToggle.gameObject.SetActive(false);
		this.weatherControlsToggle.gameObject.SetActive(false);
	}

	// Token: 0x060039D2 RID: 14802 RVA: 0x0013B34C File Offset: 0x0013954C
	private void OnDisable()
	{
		GorillaScoreboardTotalUpdater.UnregisterScoreboard(this);
		SubscriptionManager.OnSubscriptionData = (Action)Delegate.Remove(SubscriptionManager.OnSubscriptionData, new Action(this.OnSubscribeReady));
		NetworkSystem.Instance.OnMasterClientSwitchedEvent -= this.OnMasterClientSwitched;
		RoomControls.OnRoomControlsEnabledChanged -= new Action<bool>(this.OnRoomControlsEnabledChanged);
	}

	// Token: 0x060039D3 RID: 14803 RVA: 0x0013B3BB File Offset: 0x001395BB
	private void OnSubscribeReady()
	{
		this.RefreshRoomControlUI();
	}

	// Token: 0x060039D4 RID: 14804 RVA: 0x0013B3BB File Offset: 0x001395BB
	private void OnMasterClientSwitched(NetPlayer newMasterClient)
	{
		this.RefreshRoomControlUI();
	}

	// Token: 0x060039D5 RID: 14805 RVA: 0x0013B3BB File Offset: 0x001395BB
	private void OnRoomControlsEnabledChanged(bool enabled)
	{
		this.RefreshRoomControlUI();
	}

	// Token: 0x060039D6 RID: 14806 RVA: 0x0013B3C3 File Offset: 0x001395C3
	private void RefreshRoomControlUI()
	{
		if (this.roomControlsActive && !RoomControls.CanModerate())
		{
			this.roomControlsActive = false;
		}
		this.SetDirty();
		this.ToggleRoomControlButtons();
	}

	// Token: 0x060039D7 RID: 14807 RVA: 0x0013B3E7 File Offset: 0x001395E7
	public void SetTimeOfDay(int timeOfDay)
	{
		BetterDayNightManager.instance.SetTimeOfDayNetworked(timeOfDay);
	}

	// Token: 0x060039D8 RID: 14808 RVA: 0x0013B3F6 File Offset: 0x001395F6
	public void SetWeatherClear()
	{
		this.SetWeather(BetterDayNightManager.WeatherType.None);
	}

	// Token: 0x060039D9 RID: 14809 RVA: 0x0013B3FF File Offset: 0x001395FF
	public void SetWeatherRain()
	{
		this.SetWeather(BetterDayNightManager.WeatherType.Raining);
	}

	// Token: 0x060039DA RID: 14810 RVA: 0x0013B408 File Offset: 0x00139608
	private void SetWeather(BetterDayNightManager.WeatherType weather)
	{
		BetterDayNightManager.instance.SetFixedWeatherNetworked(weather);
	}

	// Token: 0x060039DB RID: 14811 RVA: 0x0013B417 File Offset: 0x00139617
	private void UpdateWeatherText()
	{
		this.weatherControlsText.text = "TIME OF DAY: " + BetterDayNightManager.instance.GetTimeOfDayString() + "   WEATHER: " + BetterDayNightManager.instance.GetWeatherString();
	}

	// Token: 0x060039DC RID: 14812 RVA: 0x0013B44B File Offset: 0x0013964B
	public void SetDirty()
	{
		this._isDirty = true;
	}

	// Token: 0x040049D6 RID: 18902
	public GameObject scoreBoardLinePrefab;

	// Token: 0x040049D7 RID: 18903
	public int startingYValue;

	// Token: 0x040049D8 RID: 18904
	public int lineHeight;

	// Token: 0x040049D9 RID: 18905
	public bool includeMMR;

	// Token: 0x040049DA RID: 18906
	public bool isActive;

	// Token: 0x040049DB RID: 18907
	public GameObject leftPanel;

	// Token: 0x040049DC RID: 18908
	public float leftPanelRoomControlXOffset = -36f;

	// Token: 0x040049DD RID: 18909
	public GameObject rightPanel;

	// Token: 0x040049DE RID: 18910
	[Space]
	public GameObject linesParent;

	// Token: 0x040049DF RID: 18911
	public float bigRoomYOffset = 32.5f;

	// Token: 0x040049E0 RID: 18912
	[SerializeField]
	public List<GorillaPlayerScoreboardLine> lines;

	// Token: 0x040049E1 RID: 18913
	private List<RectTransform> linesRTs = new List<RectTransform>();

	// Token: 0x040049E2 RID: 18914
	public GameObject textsParent;

	// Token: 0x040049E3 RID: 18915
	public GTZone[] allowedWeatherControlZones;

	// Token: 0x040049E4 RID: 18916
	public GameObject weatherControlsParent;

	// Token: 0x040049E5 RID: 18917
	public TextMeshPro boardText;

	// Token: 0x040049E6 RID: 18918
	public TextMeshPro buttonText;

	// Token: 0x040049E7 RID: 18919
	public TextMeshPro roomControlsText;

	// Token: 0x040049E8 RID: 18920
	public TextMeshPro weatherControlsText;

	// Token: 0x040049E9 RID: 18921
	public GameObject roomControlsToggle;

	// Token: 0x040049EA RID: 18922
	public GameObject weatherControlsToggle;

	// Token: 0x040049EB RID: 18923
	public bool needsUpdate;

	// Token: 0x040049EC RID: 18924
	public TextMeshPro notInRoomText;

	// Token: 0x040049ED RID: 18925
	public string initialGameMode;

	// Token: 0x040049EE RID: 18926
	private bool roomControlsActive;

	// Token: 0x040049EF RID: 18927
	private bool allowWeatherControls = true;

	// Token: 0x040049F0 RID: 18928
	private bool weatherControlsActive;

	// Token: 0x040049F1 RID: 18929
	private string tempGmName;

	// Token: 0x040049F2 RID: 18930
	private string gmName;

	// Token: 0x040049F3 RID: 18931
	private const string error = "ERROR";

	// Token: 0x040049F4 RID: 18932
	private List<string> gmNames;

	// Token: 0x040049F5 RID: 18933
	private bool _isDirty = true;

	// Token: 0x040049F6 RID: 18934
	private StringBuilder stringBuilder = new StringBuilder(220);

	// Token: 0x040049F7 RID: 18935
	private StringBuilder buttonStringBuilder = new StringBuilder(720);
}
