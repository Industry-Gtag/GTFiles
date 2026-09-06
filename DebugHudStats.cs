using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using GorillaUtil;
using TMPro;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

// Token: 0x02000D9C RID: 3484
public class DebugHudStats : MonoBehaviour
{
	// Token: 0x17000835 RID: 2101
	// (get) Token: 0x060055BD RID: 21949 RVA: 0x001C06E2 File Offset: 0x001BE8E2
	public static DebugHudStats Instance
	{
		get
		{
			return DebugHudStats._instance;
		}
	}

	// Token: 0x060055BE RID: 21950 RVA: 0x001C06EC File Offset: 0x001BE8EC
	private void Awake()
	{
		if (DebugHudStats._instance != null && DebugHudStats._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			DebugHudStats._instance = this;
			this.fixedWeathers = Enum.GetValues(typeof(BetterDayNightManager.WeatherType));
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x060055BF RID: 21951 RVA: 0x001C0748 File Offset: 0x001BE948
	private void OnDestroy()
	{
		if (DebugHudStats._instance == this)
		{
			DebugHudStats._instance = null;
			if (this.drawCallsRecorder.Valid)
			{
				this.drawCallsRecorder.Dispose();
			}
			if (this.trisRecorder.Valid)
			{
				this.trisRecorder.Dispose();
			}
		}
	}

	// Token: 0x060055C0 RID: 21952 RVA: 0x001C0798 File Offset: 0x001BE998
	private void LateUpdate()
	{
		if (GTPlayerTransform.Instance != null)
		{
			base.transform.LookAt(Camera.main.transform.position, GTPlayerTransform.Instance.GravityUp);
		}
		else
		{
			base.transform.LookAt(Camera.main.transform.position, Vector3.up);
		}
		if (!ControllerInputPoller.HandTrackingActive())
		{
			if (this.currentState == DebugHudStats.State.timeAdjust)
			{
				bool flag = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
				bool flag2 = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
				bool flag3 = ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.5f;
				bool flag4 = ControllerInputPoller.GripFloat(XRNode.RightHand) > 0.5f;
				bool flag5 = ControllerInputPoller.Primary2DAxis(XRNode.LeftHand).x > 0.5f;
				bool flag6 = ControllerInputPoller.Primary2DAxis(XRNode.LeftHand).x < -0.5f;
				bool flag7 = ControllerInputPoller.Primary2DAxis(XRNode.LeftHand).y > 0.5f;
				bool flag8 = ControllerInputPoller.Primary2DAxis(XRNode.LeftHand).y < -0.5f;
				if (this.button1Down && !flag)
				{
					GorillaComputer.instance.AddSeverTime(flag4 ? (-60) : 60);
				}
				if (this.button2Down && !flag2)
				{
					GorillaComputer.instance.AddSeverTime(flag4 ? (-1) : 5);
				}
				if (this.button3Down && !flag3)
				{
					GorillaComputer.instance.AddSeverTime(flag4 ? (-1440) : 1440);
				}
				if (!this.button5Down && flag5)
				{
					this.ChangeTOD(1);
				}
				if (!this.button6Down && flag6)
				{
					this.ChangeTOD(-1);
				}
				if (!this.button7Down && flag7)
				{
					this.ChangeWeather(1);
				}
				if (!this.button8Down && flag8)
				{
					this.ChangeWeather(-1);
				}
				this.button1Down = flag;
				this.button2Down = flag2;
				this.button3Down = flag3;
				this.button5Down = flag5;
				this.button6Down = flag6;
				this.button7Down = flag7;
				this.button8Down = flag8;
			}
			if (this.currentState == DebugHudStats.State.TitleDataMonitor || this.currentState == DebugHudStats.State.ShowLog || this.currentState == DebugHudStats.State.ShowError)
			{
				bool flag9 = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
				bool flag10 = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
				if (this.button1Down && !flag9)
				{
					this.logging.pageToDisplay = ((this.logging.pageToDisplay < this.logging.textInfo.pageCount) ? (this.logging.pageToDisplay + 1) : 1);
					this.updateLogTitle();
				}
				if (this.button2Down && !flag10)
				{
					this.logging.pageToDisplay = ((this.logging.pageToDisplay > 1) ? (this.logging.pageToDisplay - 1) : this.logging.textInfo.pageCount);
					this.updateLogTitle();
				}
				this.button1Down = flag9;
				this.button2Down = flag10;
			}
			bool flag11 = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
			bool flag12 = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
			if ((this.buttonDown && !flag11) || (this.buttonDownBack && !flag12))
			{
				this.NextState(this.buttonDown);
				if (this.currentState == DebugHudStats.State.ShowStats)
				{
					this.distanceMoved = (this.distanceSwam = 0f);
					PlayerGameEvents.OnPlayerMoved += this.OnPlayerMoved;
					PlayerGameEvents.OnPlayerSwam += this.OnPlayerSwam;
				}
				this.text.gameObject.SetActive(this.currentState > DebugHudStats.State.Inactive);
				if (RigidbodyHighlighter.Instance != null)
				{
					RigidbodyHighlighter.Instance.Active = this.currentState == DebugHudStats.State.ShowRBs;
				}
				this.btnDownTime = 0f;
			}
			this.buttonDown = flag11;
			this.buttonDownBack = flag12;
		}
		if (this.firstAwake == 0f)
		{
			this.firstAwake = Time.time;
		}
		if (this.updateTimer < this.delayUpdateRate)
		{
			this.updateTimer += Time.deltaTime;
			return;
		}
		int num = Mathf.RoundToInt(1f / Time.smoothDeltaTime);
		if (num < DebugHudStats.FPS_THRESHOLD)
		{
			this.lowFps++;
		}
		else
		{
			this.lowFps = 0;
		}
		this.fpsWarning.gameObject.SetActive(this.lowFps > 5 && this.currentState == DebugHudStats.State.Inactive);
		if (this.currentState != DebugHudStats.State.Inactive)
		{
			this.builder.Clear();
			this.builder.Append("gt: ");
			this.builder.Append(GorillaComputer.instance.version);
			this.builder.Append(":");
			this.builder.Append(GorillaComputer.instance.buildCode);
			this.builder.AppendLine(this.spoofIds ? " <color=\"red\">*Spoofing IDs*</color>" : string.Empty);
			num = Mathf.Min(num, 90);
			this.builder.Append((num < DebugHudStats.FPS_THRESHOLD) ? "<color=\"red\">" : "<color=\"white\">");
			this.builder.Append(num);
			this.builder.Append(string.Format(" fps / {0} fps</color> ", DebugHudStats.FPS_THRESHOLD + 1));
			this.builder.AppendLine(string.Format("sfps: {0} (Health: {1})", GorillaTagger.Instance.SmoothedFramerate, GorillaTagger.Instance.FramerateHealth));
			float eyeTextureResolutionScale = XRSettings.eyeTextureResolutionScale;
			float renderViewportScale = XRSettings.renderViewportScale;
			float renderScale = (GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset).renderScale;
			this.builder.AppendLine(string.Format("draw calls: {0} tris: {1} ", this.drawCallsRecorder.LastValue, this.trisRecorder.LastValue) + string.Format("rs: {0}/{1}/{2} ", eyeTextureResolutionScale, renderViewportScale, renderScale));
			this.builder.AppendLine(string.Format("Memory: {0}M", Profiler.GetMonoUsedSizeLong() / 1048576L));
			if (GorillaComputer.instance != null)
			{
				DateTime serverTime = GorillaComputer.instance.GetServerTime();
				this.builder.AppendLine(string.Format("<color={0}>{1}</color>", (serverTime.Year > 2020) ? "#00FFAA" : "#FF3333", serverTime));
			}
			else
			{
				this.builder.AppendLine("<color=#FF3333>Server Time Unavailable</color>");
			}
			ZoneDef currentNode = GorillaTagger.Instance.offlineVRRig.zoneEntity.currentNode;
			if (currentNode != null)
			{
				this.zones = string.Format("{0}/{1}/{2}", currentNode.gameObject.name.ToUpperInvariant(), currentNode.zoneId, currentNode.subZoneId);
			}
			if (NetworkSystem.Instance.IsMasterClient)
			{
				this.builder.Append("H");
			}
			if (NetworkSystem.Instance.InRoom)
			{
				if (NetworkSystem.Instance.SessionIsPrivate)
				{
					this.builder.Append("Pri ");
				}
				else
				{
					this.builder.Append("Pub ");
				}
			}
			else
			{
				this.builder.Append("DC ");
			}
			this.builder.Append("z: <color=\"green\">");
			this.builder.Append(this.zones);
			this.builder.AppendLine("</color>");
			if (NetworkSystem.Instance.InRoom)
			{
				GorillaGameManager instance = GorillaGameManager.instance;
				if (instance != null)
				{
					GorillaTagCompetitiveManager gorillaTagCompetitiveManager = instance as GorillaTagCompetitiveManager;
					if (gorillaTagCompetitiveManager != null)
					{
						this.builder.Append("Ranked Mode ELO: ");
						this.builder.Append(gorillaTagCompetitiveManager.GetScoring().Progression.GetEloScore().ToString());
						this.builder.Append("  Tier: ");
						this.builder.AppendLine(gorillaTagCompetitiveManager.GetScoring().Progression.GetRankedProgressionTierName());
						RankedMultiplayerScore.PlayerScoreInRound inGameScoreForSelf = gorillaTagCompetitiveManager.GetScoring().GetInGameScoreForSelf();
						this.builder.Append("Tags: ");
						this.builder.Append(inGameScoreForSelf.NumTags.ToString());
						this.builder.Append("  Defense: ");
						this.builder.Append(Mathf.RoundToInt(inGameScoreForSelf.PointsOnDefense).ToString());
						this.builder.Append("  Score: ");
						this.builder.AppendLine(Mathf.RoundToInt(gorillaTagCompetitiveManager.GetScoring().ComputeGameScore(inGameScoreForSelf.NumTags, inGameScoreForSelf.PointsOnDefense)).ToString());
						if (gorillaTagCompetitiveManager.ShowDebugPing)
						{
							this.builder.AppendLine("Server MatchID Ping!");
						}
					}
				}
			}
			switch (this.currentState)
			{
			case DebugHudStats.State.ShowStats:
			{
				this.builder.AppendLine("\nStats:\n");
				Vector3 vector = GTPlayer.Instance.AveragedVelocity;
				Vector3 headCenterPosition = GTPlayer.Instance.HeadCenterPosition;
				float magnitude = vector.magnitude;
				this.groundVelocity = vector;
				this.groundVelocity.y = 0f;
				this.builder.AppendLine(string.Format("v: {0:F1} m/s\t\todo: {1:F2}m\tswam: {2:F2}m", magnitude, this.distanceMoved, this.distanceSwam));
				this.builder.AppendLine(string.Format("ground: {0:F1} m/s\thead: {1:F2}", this.groundVelocity.magnitude, headCenterPosition));
				break;
			}
			case DebugHudStats.State.ShowRBs:
				this.builder.AppendLine("\nRigid Body Locator\n");
				break;
			case DebugHudStats.State.timeAdjust:
				this.builder.AppendLine("\nAdjust Time\n");
				this.builder.AppendLine("Press [A] to advance one hour [+ R Grip to go back one hour]");
				this.builder.AppendLine("Press [B] to advance five minutes [+ R Grip to go back one minute]");
				this.builder.AppendLine("Press [R] Trigger to advance one day [+ R Grip to go back one day]");
				this.builder.AppendLine(string.Format("\nAdjust Environment {0}/{1} : {2} \n", BetterDayNightManager.instance.currentTimeIndex + 1, BetterDayNightManager.instance.timeOfDayRange.Length, BetterDayNightManager.instance.CurrentWeather()));
				this.builder.AppendLine("[L STICK L/R] to change Time Of Day. [L STICK U/D] to change Weather.");
				break;
			case DebugHudStats.State.RecordingMode:
				this.builder.AppendLine("\nMo-Cap Recording:\n");
				break;
			}
			this.text.text = this.builder.ToString();
		}
		this.updateTimer = 0f;
		if (this.buttonDown && this.currentState != DebugHudStats.State.RecordingMode)
		{
			this.btnDownTime += Time.deltaTime / this.delayUpdateRate;
			if (this.btnDownTime >= 15f)
			{
				base.gameObject.SetActive(false);
			}
			this.dismiss.text = string.Format("let go of that button in the next {0:0.0} seconds or the debug hud will vanish forever", 15f - this.btnDownTime);
		}
		this.dismiss.gameObject.SetActive(this.buttonDown && this.currentState != DebugHudStats.State.RecordingMode && this.btnDownTime > 5f);
	}

	// Token: 0x060055C1 RID: 21953 RVA: 0x001C127C File Offset: 0x001BF47C
	private void ChangeTOD(int v)
	{
		int num = (BetterDayNightManager.instance.currentTimeIndex + BetterDayNightManager.instance.timeOfDayRange.Length + v) % BetterDayNightManager.instance.timeOfDayRange.Length;
		BetterDayNightManager.instance.SetTimeOfDay(num, false);
		BetterDayNightManager.instance.SetOverrideIndex(num);
		BetterDayNightManager.instance.SetFixedWeather((BetterDayNightManager.WeatherType)this.fixedWeathers.GetValue(this.fixedWeatherIndex), false);
	}

	// Token: 0x060055C2 RID: 21954 RVA: 0x001C12F4 File Offset: 0x001BF4F4
	private void ChangeWeather(int v)
	{
		this.fixedWeatherIndex = (this.fixedWeatherIndex + this.fixedWeathers.Length + v) % this.fixedWeathers.Length;
		BetterDayNightManager.instance.SetFixedWeather((BetterDayNightManager.WeatherType)this.fixedWeathers.GetValue(this.fixedWeatherIndex), false);
	}

	// Token: 0x060055C3 RID: 21955 RVA: 0x001C134C File Offset: 0x001BF54C
	private void NextState(bool fwd)
	{
		PlayerGameEvents.OnPlayerMoved -= this.OnPlayerMoved;
		PlayerGameEvents.OnPlayerSwam -= this.OnPlayerSwam;
		this.logging.gameObject.SetActive(false);
		this.logging.pageToDisplay = 1;
		if (this.currentState == DebugHudStats.State.timeAdjust)
		{
			BetterDayNightManager.instance.ClearFixedWeather(false);
		}
		switch (this.currentState)
		{
		case DebugHudStats.State.Inactive:
			this.currentState = (fwd ? DebugHudStats.State.Active : DebugHudStats.State.timeAdjust);
			break;
		case DebugHudStats.State.Active:
			this.currentState = (fwd ? DebugHudStats.State.ShowLog : DebugHudStats.State.Inactive);
			break;
		case DebugHudStats.State.ShowLog:
			this.currentState = (fwd ? DebugHudStats.State.ShowError : DebugHudStats.State.Active);
			break;
		case DebugHudStats.State.ShowError:
			this.currentState = (fwd ? DebugHudStats.State.ShowStats : DebugHudStats.State.ShowLog);
			break;
		case DebugHudStats.State.ShowStats:
			this.currentState = (fwd ? DebugHudStats.State.ShowRBs : DebugHudStats.State.ShowError);
			break;
		case DebugHudStats.State.ShowRBs:
			this.currentState = (fwd ? DebugHudStats.State.TitleDataMonitor : DebugHudStats.State.ShowStats);
			break;
		case DebugHudStats.State.timeAdjust:
			this.currentState = (fwd ? DebugHudStats.State.Inactive : DebugHudStats.State.TitleDataMonitor);
			break;
		case DebugHudStats.State.RecordingMode:
			this.currentState = (fwd ? DebugHudStats.State.Inactive : DebugHudStats.State.timeAdjust);
			break;
		case DebugHudStats.State.TitleDataMonitor:
			this.currentState = (fwd ? DebugHudStats.State.timeAdjust : DebugHudStats.State.ShowRBs);
			break;
		}
		if (this.currentState == DebugHudStats.State.timeAdjust)
		{
			BetterDayNightManager.instance.SetFixedWeather((BetterDayNightManager.WeatherType)this.fixedWeathers.GetValue(this.fixedWeatherIndex), false);
		}
		this.UpdateLog();
	}

	// Token: 0x060055C4 RID: 21956 RVA: 0x001C149C File Offset: 0x001BF69C
	private void DisplayLog(List<string> log)
	{
		this.logging.gameObject.SetActive(true);
		this.logging.text = string.Empty;
		for (int i = log.Count - 1; i >= 0; i--)
		{
			TMP_Text tmp_Text = this.logging;
			tmp_Text.text = tmp_Text.text + log[i] + "\n";
		}
		this.updateLogTitle();
	}

	// Token: 0x060055C5 RID: 21957 RVA: 0x001C1508 File Offset: 0x001BF708
	private async void updateLogTitle()
	{
		await Task.Yield();
		this.logPage.text = string.Format("{0} <<[B] turn page [A]>> ({1}/{2})", this.logTitleFromState(this.currentState), this.logging.pageToDisplay, this.logging.textInfo.pageCount);
	}

	// Token: 0x060055C6 RID: 21958 RVA: 0x001C153F File Offset: 0x001BF73F
	private string logTitleFromState(DebugHudStats.State s)
	{
		if (s == DebugHudStats.State.ShowLog)
		{
			return "Debug Log";
		}
		if (s == DebugHudStats.State.ShowError)
		{
			return "Error Log";
		}
		if (s != DebugHudStats.State.TitleDataMonitor)
		{
			return string.Empty;
		}
		return "Title Data Log";
	}

	// Token: 0x060055C7 RID: 21959 RVA: 0x001C1568 File Offset: 0x001BF768
	private string colorFromState(DebugHudStats.State s)
	{
		switch (s)
		{
		case DebugHudStats.State.ShowLog:
			return "\"yellow\"";
		case DebugHudStats.State.ShowError:
			return "\"orange\"";
		case DebugHudStats.State.ShowStats:
			return "\"green\"";
		case DebugHudStats.State.ShowRBs:
			return "\"red\"";
		case DebugHudStats.State.RecordingMode:
			return "\"purple\"";
		case DebugHudStats.State.TitleDataMonitor:
			return "#00ffff";
		}
		return "#ffffff";
	}

	// Token: 0x060055C8 RID: 21960 RVA: 0x001C15C4 File Offset: 0x001BF7C4
	private void OnPlayerSwam(float distance, float speed)
	{
		if (distance > 0.005f)
		{
			this.distanceSwam += distance;
		}
	}

	// Token: 0x060055C9 RID: 21961 RVA: 0x001C15DC File Offset: 0x001BF7DC
	private void OnPlayerMoved(float distance, float speed)
	{
		if (distance > 0.005f)
		{
			this.distanceMoved += distance;
		}
	}

	// Token: 0x060055CA RID: 21962 RVA: 0x001C15F4 File Offset: 0x001BF7F4
	private void OnEnable()
	{
		Application.logMessageReceived += this.LogMessageReceived;
		PlayFabTitleDataCache.OnValueRetieved = (Action<string, string>)Delegate.Combine(PlayFabTitleDataCache.OnValueRetieved, new Action<string, string>(this.TDValueRetrieved));
		PlayFabTitleDataCache.OnCachedValueRetieved = (Action<string, string>)Delegate.Combine(PlayFabTitleDataCache.OnCachedValueRetieved, new Action<string, string>(this.TDCachedValueRetrieved));
	}

	// Token: 0x060055CB RID: 21963 RVA: 0x001C1654 File Offset: 0x001BF854
	private void TDValueRetrieved(string arg1, string arg2)
	{
		this.logTD.Add(string.Format(" >{0:F2}> TitleData[ <color=#ffaaff>{1}</color> ] = {2}", Time.realtimeSinceStartup, arg1, arg2));
		if (this.logTD.Count > 1000)
		{
			this.logTD.RemoveAt(0);
		}
		this.UpdateLog();
	}

	// Token: 0x060055CC RID: 21964 RVA: 0x001C16A8 File Offset: 0x001BF8A8
	private void TDCachedValueRetrieved(string arg1, string arg2)
	{
		this.logTD.Add(string.Format(" >{0:F2}> TitleData[ <color=#00ffff>{1}</color> ] = {2}", Time.realtimeSinceStartup, arg1, arg2));
		if (this.logTD.Count > 1000)
		{
			this.logTD.RemoveAt(0);
		}
		this.UpdateLog();
	}

	// Token: 0x060055CD RID: 21965 RVA: 0x001C16FC File Offset: 0x001BF8FC
	private void OnDisable()
	{
		PlayFabTitleDataCache.OnValueRetieved = (Action<string, string>)Delegate.Remove(PlayFabTitleDataCache.OnValueRetieved, new Action<string, string>(this.TDValueRetrieved));
		PlayFabTitleDataCache.OnCachedValueRetieved = (Action<string, string>)Delegate.Remove(PlayFabTitleDataCache.OnCachedValueRetieved, new Action<string, string>(this.TDCachedValueRetrieved));
		Application.logMessageReceived -= this.LogMessageReceived;
	}

	// Token: 0x060055CE RID: 21966 RVA: 0x001C175C File Offset: 0x001BF95C
	private void LogMessageReceived(string condition, string stackTrace, LogType type)
	{
		string text = string.Format(" >{0:F2}> {1}{2}</color>", Time.realtimeSinceStartup, this.getColorStringFromLogType(type), condition);
		if (this.pLog != condition)
		{
			this.logMessage.Add(text);
		}
		else
		{
			this.logMessage[this.logMessage.Count - 1] = text;
		}
		this.pLog = condition;
		if (this.logMessage.Count > 100)
		{
			this.logMessage.RemoveAt(0);
		}
		if (type == LogType.Error || type == LogType.Assert || type == LogType.Exception)
		{
			this.logError.Add(text + "\n" + stackTrace);
			if (this.logError.Count > 100)
			{
				this.logError.RemoveAt(0);
			}
		}
		this.UpdateLog();
	}

	// Token: 0x060055CF RID: 21967 RVA: 0x001C1820 File Offset: 0x001BFA20
	private void UpdateLog()
	{
		DebugHudStats.State state = this.currentState;
		if (state == DebugHudStats.State.ShowLog)
		{
			this.DisplayLog(this.logMessage);
			return;
		}
		if (state == DebugHudStats.State.ShowError)
		{
			this.DisplayLog(this.logError);
			return;
		}
		if (state != DebugHudStats.State.TitleDataMonitor)
		{
			return;
		}
		this.DisplayLog(this.logTD);
	}

	// Token: 0x060055D0 RID: 21968 RVA: 0x001C1867 File Offset: 0x001BFA67
	private string getColorStringFromLogType(LogType type)
	{
		switch (type)
		{
		case LogType.Error:
		case LogType.Assert:
		case LogType.Exception:
			return "<color=\"red\">";
		case LogType.Warning:
			return "<color=\"yellow\">";
		}
		return "<color=\"white\">";
	}

	// Token: 0x060055D1 RID: 21969 RVA: 0x001C1898 File Offset: 0x001BFA98
	private void OnZoneChanged(ZoneData[] zoneData)
	{
		this.zones = string.Empty;
		for (int i = 0; i < zoneData.Length; i++)
		{
			if (zoneData[i].active)
			{
				this.zones = this.zones + zoneData[i].zone.ToString().ToUpper() + "; ";
			}
		}
	}

	// Token: 0x04006707 RID: 26375
	public static int FPS_THRESHOLD = 89;

	// Token: 0x04006708 RID: 26376
	private static DebugHudStats _instance;

	// Token: 0x04006709 RID: 26377
	[SerializeField]
	public TMP_Text text;

	// Token: 0x0400670A RID: 26378
	[SerializeField]
	public TMP_Text logging;

	// Token: 0x0400670B RID: 26379
	[SerializeField]
	public TMP_Text logPage;

	// Token: 0x0400670C RID: 26380
	[SerializeField]
	private TMP_Text fpsWarning;

	// Token: 0x0400670D RID: 26381
	[SerializeField]
	private TMP_Text dismiss;

	// Token: 0x0400670E RID: 26382
	[SerializeField]
	private float delayUpdateRate = 0.25f;

	// Token: 0x0400670F RID: 26383
	private float updateTimer;

	// Token: 0x04006710 RID: 26384
	public float sessionAnytrackingLost;

	// Token: 0x04006711 RID: 26385
	public float last30SecondsTrackingLost;

	// Token: 0x04006712 RID: 26386
	private float firstAwake;

	// Token: 0x04006713 RID: 26387
	private bool leftHandTracked;

	// Token: 0x04006714 RID: 26388
	private bool rightHandTracked;

	// Token: 0x04006715 RID: 26389
	private StringBuilder builder;

	// Token: 0x04006716 RID: 26390
	private Vector3 averagedVelocity;

	// Token: 0x04006717 RID: 26391
	private Vector3 groundVelocity;

	// Token: 0x04006718 RID: 26392
	private Vector3 centerHeadPos;

	// Token: 0x04006719 RID: 26393
	private float distanceMoved;

	// Token: 0x0400671A RID: 26394
	private float distanceSwam;

	// Token: 0x0400671B RID: 26395
	private List<string> logMessage = new List<string>();

	// Token: 0x0400671C RID: 26396
	private List<string> logError = new List<string>();

	// Token: 0x0400671D RID: 26397
	private List<string> logTD = new List<string>();

	// Token: 0x0400671E RID: 26398
	private bool buttonDown;

	// Token: 0x0400671F RID: 26399
	private bool buttonDownBack;

	// Token: 0x04006720 RID: 26400
	private bool spoofIds;

	// Token: 0x04006721 RID: 26401
	private int lowFps;

	// Token: 0x04006722 RID: 26402
	private string zones;

	// Token: 0x04006723 RID: 26403
	private GroupJoinZoneAB lastGroupJoinZone;

	// Token: 0x04006724 RID: 26404
	private DebugHudStats.State currentState = DebugHudStats.State.Active;

	// Token: 0x04006725 RID: 26405
	private ProfilerRecorder drawCallsRecorder;

	// Token: 0x04006726 RID: 26406
	private ProfilerRecorder trisRecorder;

	// Token: 0x04006727 RID: 26407
	private string pLog;

	// Token: 0x04006728 RID: 26408
	private bool button1Down;

	// Token: 0x04006729 RID: 26409
	private bool button2Down;

	// Token: 0x0400672A RID: 26410
	private bool button3Down;

	// Token: 0x0400672B RID: 26411
	private bool button5Down;

	// Token: 0x0400672C RID: 26412
	private bool button6Down;

	// Token: 0x0400672D RID: 26413
	private bool button7Down;

	// Token: 0x0400672E RID: 26414
	private bool button8Down;

	// Token: 0x0400672F RID: 26415
	[SerializeField]
	private StringTable betaTitleDataOveride;

	// Token: 0x04006730 RID: 26416
	private Array fixedWeathers;

	// Token: 0x04006731 RID: 26417
	private int fixedWeatherIndex;

	// Token: 0x04006732 RID: 26418
	private float btnDownTime;

	// Token: 0x02000D9D RID: 3485
	private enum State
	{
		// Token: 0x04006734 RID: 26420
		Inactive,
		// Token: 0x04006735 RID: 26421
		Active,
		// Token: 0x04006736 RID: 26422
		ShowLog,
		// Token: 0x04006737 RID: 26423
		ShowError,
		// Token: 0x04006738 RID: 26424
		ShowStats,
		// Token: 0x04006739 RID: 26425
		ShowRBs,
		// Token: 0x0400673A RID: 26426
		timeAdjust,
		// Token: 0x0400673B RID: 26427
		RecordingMode,
		// Token: 0x0400673C RID: 26428
		TitleDataMonitor
	}
}
