using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SynthesisvrArcade;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using WebSocketSharp;

// Token: 0x02000E27 RID: 3623
public class SynthesisArcadeObject : MonoBehaviour
{
	// Token: 0x17000873 RID: 2163
	// (get) Token: 0x0600589B RID: 22683 RVA: 0x001CC207 File Offset: 0x001CA407
	public static SynthesisArcadeObject Instance
	{
		get
		{
			return SynthesisArcadeObject._instance;
		}
	}

	// Token: 0x0600589C RID: 22684 RVA: 0x001CC210 File Offset: 0x001CA410
	public string ReadCommandLineArgument(string name)
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i] == name && commandLineArgs.Length > i + 1)
			{
				return commandLineArgs[i + 1];
			}
		}
		return null;
	}

	// Token: 0x0600589D RID: 22685 RVA: 0x001CC24C File Offset: 0x001CA44C
	internal void processLiveCommand(string message)
	{
		string[] array = message.Split(new string[] { " " }, 2, StringSplitOptions.None);
		this.processLiveCommand(array[0], array[1]);
	}

	// Token: 0x0600589E RID: 22686 RVA: 0x001CC27C File Offset: 0x001CA47C
	internal void processLiveCommand(string interactionname, string args)
	{
		List<SynthesisUdpCommand> list = this.commandDefinitions.FindAll((SynthesisUdpCommand el) => el.commandName == interactionname);
		if (list.Count > 0)
		{
			list.ForEach(delegate(SynthesisUdpCommand cmd)
			{
				cmd.extraArgs = ((args != null) ? args : "");
				SynthesisArcadeObject.udpQueue.Enqueue(cmd);
			});
			return;
		}
		this.fallbackCommandProcessor.Invoke(interactionname, args);
	}

	// Token: 0x0600589F RID: 22687 RVA: 0x001CC2E8 File Offset: 0x001CA4E8
	internal void processUnknownLiveCommand(string command, string args)
	{
		Debug.LogWarning("[" + this.TAG + "] Unknown Live Interactions Command: " + command);
	}

	// Token: 0x060058A0 RID: 22688 RVA: 0x001CC308 File Offset: 0x001CA508
	internal void SynthesisUdpReceived(IAsyncResult ar)
	{
		IPEndPoint ipendPoint = new IPEndPoint(IPAddress.Any, 0);
		byte[] array;
		try
		{
			array = this.synthesisUdpListener.EndReceive(ar, ref ipendPoint);
			if (array.Length == 0)
			{
				return;
			}
			this.synthesisUdpListener.BeginReceive(new AsyncCallback(this.SynthesisUdpReceived), null);
		}
		catch (ObjectDisposedException ex)
		{
			try
			{
				Debug.Log("[" + this.TAG + "] Port listening failed. Retry / " + ex.Message);
				this.synthesisUdpListener.BeginReceive(new AsyncCallback(this.SynthesisUdpReceived), null);
			}
			catch
			{
			}
			return;
		}
		string @string = Encoding.UTF8.GetString(array);
		SynthesisArcadeObject._instance.processLiveCommand(@string);
	}

	// Token: 0x060058A1 RID: 22689 RVA: 0x001CC3C8 File Offset: 0x001CA5C8
	internal void StartSynthesisUdpListener()
	{
		this.synthesisUdpListener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
		this.synthesisUdpListener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
		this.synthesisUdpListener.ExclusiveAddressUse = false;
		this.synthesisUdpListener.Client.EnableBroadcast = true;
		this.synthesisUdpListener.EnableBroadcast = true;
		this.synthesisUdpListener.Client.Bind(this.localUdpEndpoint);
		this.synthesisUdpListener.BeginReceive(new AsyncCallback(this.SynthesisUdpReceived), null);
		Debug.Log("[" + this.TAG + "] Step1/Listening on UDP port " + this.udpPort.ToString());
	}

	// Token: 0x060058A2 RID: 22690 RVA: 0x001CC481 File Offset: 0x001CA681
	public void UdpHelloWorld(string args)
	{
		Debug.Log("[" + this.TAG + "] Hello World -> " + args);
	}

	// Token: 0x060058A3 RID: 22691 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnApplicationPause(bool pauseStatus)
	{
	}

	// Token: 0x060058A4 RID: 22692 RVA: 0x001CC4A0 File Offset: 0x001CA6A0
	private void Update()
	{
		if (this.quitGame)
		{
			Debug.Log("[" + this.TAG + "] SynthesisVR stopping the game");
			Application.Quit();
		}
		else if (this.webSocketClientAddress != null && this.webSocketClientAddress.Length > 5 && (SynthesisArcadeObject.wsclient == null || SynthesisArcadeObject.wsclient.ReadyState == WebSocketState.Closed || SynthesisArcadeObject.wsclient.ReadyState == WebSocketState.Closing))
		{
			this.ConnectToWebSocket();
		}
		while (SynthesisArcadeObject.udpQueue.Count > 0)
		{
			SynthesisUdpCommand synthesisUdpCommand = SynthesisArcadeObject.udpQueue.Dequeue();
			if (synthesisUdpCommand.EventReceiver.GetPersistentEventCount() == 0)
			{
				UnityEvent<string> eventReceiver = synthesisUdpCommand.EventReceiver;
				if (eventReceiver != null)
				{
					eventReceiver.Invoke(synthesisUdpCommand.extraArgs);
				}
			}
			else
			{
				for (int i = 0; i < synthesisUdpCommand.EventReceiver.GetPersistentEventCount(); i++)
				{
					try
					{
						if (synthesisUdpCommand.extraArgs.Trim().Length == 0)
						{
							synthesisUdpCommand.EventReceiver.Invoke("");
						}
						else if (synthesisUdpCommand.EventReceiver.GetPersistentEventCount() > 0)
						{
							string persistentMethodName = synthesisUdpCommand.EventReceiver.GetPersistentMethodName(i);
							if (persistentMethodName != null)
							{
								((MonoBehaviour)synthesisUdpCommand.EventReceiver.GetPersistentTarget(i)).SendMessage(persistentMethodName, synthesisUdpCommand.extraArgs);
							}
						}
					}
					catch (Exception ex)
					{
						Debug.Log("[" + this.TAG + "] ERROR => " + ex.Message);
					}
				}
			}
		}
	}

	// Token: 0x060058A5 RID: 22693 RVA: 0x001CC620 File Offset: 0x001CA820
	private void OnDestroy()
	{
		try
		{
			SynthesisColumns synthesisColumns;
			if (SynthesisArcadeObject._instance == this && SynthesisArcadeObject._instance.TryGetComponent<SynthesisColumns>(out synthesisColumns))
			{
				SceneManager.sceneLoaded -= synthesisColumns.HandleSceneLoaded;
			}
			if (SynthesisArcadeObject.wsclient != null && SynthesisArcadeObject.wsclient.ReadyState == WebSocketState.Open)
			{
				SynthesisArcadeObject.wsclient.Close();
			}
		}
		catch
		{
		}
		if (this.enableLiveInteractions)
		{
			try
			{
				if (this.synthesisUdpListener.Client != null)
				{
					this.synthesisUdpListener.Close();
				}
			}
			catch
			{
			}
		}
	}

	// Token: 0x060058A6 RID: 22694 RVA: 0x001CC6C0 File Offset: 0x001CA8C0
	private void Start()
	{
		if (SynthesisArcadeObject._instance != null && SynthesisArcadeObject._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			SynthesisArcadeObject._instance = this;
		}
		bool flag = false;
		string text = Application.dataPath + "\\" + (Application.isEditor ? "Plugins" : "Managed") + "\\arcade_success.dll";
		Type type = Type.GetType("SynthesisvrArcade.ArcadeFactory");
		if (Application.isEditor)
		{
			Debug.Log("[" + this.TAG + "] DLL FILE HASH - " + this.filehash(Application.dataPath + "/SynthesisVR/SDK/arcade_success.dll"));
		}
		else if (!File.Exists(text) && flag && type != null)
		{
			Debug.Log("[" + this.TAG + "] This is an IL2CPP build");
		}
		else if (!this.filehash(text).Equals("65958aba4e88155682dab8ea50244793"))
		{
			Debug.LogError("[" + this.TAG + "] BAD DLL FILE");
			this.quitGame = true;
		}
		if (!this.quitGame)
		{
			Object.DontDestroyOnLoad(SynthesisArcadeObject._instance);
			SynthesisColumns svrColumnsManager;
			if (SynthesisArcadeObject._instance.TryGetComponent<SynthesisColumns>(out svrColumnsManager))
			{
				svrColumnsManager.AutoAssignFromResources();
				string text2 = this.ReadCommandLineArgument("-svrcolumns") ?? "{}";
				if (text2.StartsWith("file_"))
				{
					text2 = svrColumnsManager.readColumnsFromJsonFile(SynthesisArcadeObject.Instance);
				}
				try
				{
					svrColumnsManager.LoadLayoutFromJson(text2);
					if (!this.enableLiveInteractions)
					{
						this.enableLiveInteractions = true;
					}
					bool flag2 = false;
					using (List<SynthesisUdpCommand>.Enumerator enumerator = this.commandDefinitions.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.commandName == "svrreloadcolumns")
							{
								flag2 = true;
								break;
							}
						}
					}
					if (!flag2)
					{
						SynthesisUdpCommand synthesisUdpCommand = new SynthesisUdpCommand
						{
							commandName = "svrreloadcolumns",
							EventReceiver = new UnityEvent<string>(),
							extraArgs = ""
						};
						synthesisUdpCommand.EventReceiver.AddListener(delegate(string json)
						{
							if (string.IsNullOrWhiteSpace(json))
							{
								json = "1";
							}
							try
							{
								if (json == "1")
								{
									json = svrColumnsManager.readColumnsFromJsonFile(SynthesisArcadeObject.Instance);
								}
								svrColumnsManager.LoadLayoutFromJson(json);
								Debug.Log("[Columns] Layout reloaded via svrreloadcolumns");
							}
							catch (Exception ex2)
							{
								Debug.LogError("[Columns] Failed to load layout: " + ex2.Message);
							}
						});
						if (this.commandDefinitions == null)
						{
							this.commandDefinitions = new List<SynthesisUdpCommand>();
						}
						this.commandDefinitions.Add(synthesisUdpCommand);
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("[" + this.TAG + "] [COLUMNS_JSON_ERROR/3] => " + ex.Message);
				}
				svrColumnsManager._mpb = new MaterialPropertyBlock();
				SceneManager.sceneLoaded += svrColumnsManager.HandleSceneLoaded;
			}
			this.co = base.StartCoroutine(this.EnableLicensingCheckTask());
			this.webSocketClientAddress = this.ReadCommandLineArgument("wsaddr");
			this.synthesisFlags = new List<string>((this.ReadCommandLineArgument("-svrflags") ?? "").ToLowerInvariant().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
			Debug.Log("[" + this.TAG + "] webSocketClientAddress = " + this.webSocketClientAddress);
		}
	}

	// Token: 0x060058A7 RID: 22695 RVA: 0x001CC9F0 File Offset: 0x001CABF0
	private void ConnectToWebSocket()
	{
		TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1);
		if (this.webSocketClientAddress != null && this.webSocketClientAddress.Length > 5 && (SynthesisArcadeObject.wsclient == null || SynthesisArcadeObject.wsclient.ReadyState == WebSocketState.Closed || SynthesisArcadeObject.wsclient.ReadyState == WebSocketState.Closing) && timeSpan.TotalMilliseconds - this.webSocketLastConnectionAttemptEpoch >= 1000.0)
		{
			string text = this.webSocketClientAddress.Replace("#^epoch^#", timeSpan.TotalMilliseconds.ToString());
			Debug.Log("[" + this.TAG + "] webSocketClientAddress_Connect = " + text);
			SynthesisArcadeObject.wsclient = new WebSocket(text, Array.Empty<string>());
			SynthesisArcadeObject.wsclient.OnMessage += this.Wsclient_OnMessage;
			SynthesisArcadeObject.wsclient.OnError += this.Wsclient_OnError;
			SynthesisArcadeObject.wsclient.OnClose += new EventHandler<CloseEventArgs>(this.Wsclient_OnClose);
			SynthesisArcadeObject.wsclient.OnOpen += this.Wsclient_OnOpen;
			this.webSocketLastConnectionAttemptEpoch = timeSpan.TotalMilliseconds;
			SynthesisArcadeObject.wsclient.ConnectAsync();
		}
	}

	// Token: 0x060058A8 RID: 22696 RVA: 0x001CCB2A File Offset: 0x001CAD2A
	internal void WebsocketBroadcast(string sendmsg)
	{
		SynthesisArcadeObject.wsclient.SendAsync(sendmsg, new Action<bool>(SynthesisArcadeObject.onWsSend));
	}

	// Token: 0x060058A9 RID: 22697 RVA: 0x001CCB44 File Offset: 0x001CAD44
	internal bool StartMultiplayerSynchronization(Action<string> callbackAction, string syncType)
	{
		try
		{
			if (SynthesisArcadeObject.wsclient != null && SynthesisArcadeObject.wsclient.ReadyState == WebSocketState.Open)
			{
				WsProxyMessage wsProxyMessage = new WsProxyMessage
				{
					action = WS_PROXY_ACTIONS.AWAIT_SYNCHRONIZATION,
					arguments = syncType
				};
				if (callbackAction != null)
				{
					this.synchronizationAction = callbackAction;
				}
				SynthesisArcadeObject.wsclient.SendAsync(JsonUtility.ToJson(wsProxyMessage), new Action<bool>(SynthesisArcadeObject.onWsSend));
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[" + this.TAG + "] [StartMultiplayerSynchronizationException] => " + ex.Message);
		}
		return false;
	}

	// Token: 0x060058AA RID: 22698 RVA: 0x001CCBD8 File Offset: 0x001CADD8
	internal bool CancelMultiplayerSynchronization(string syncType)
	{
		try
		{
			if (SynthesisArcadeObject.wsclient != null && SynthesisArcadeObject.wsclient.ReadyState == WebSocketState.Open)
			{
				WsProxyMessage wsProxyMessage = new WsProxyMessage
				{
					action = WS_PROXY_ACTIONS.CANCEL_AWAIT_SYNCHRONIZATION,
					arguments = syncType
				};
				this.synchronizationAction = null;
				SynthesisArcadeObject.wsclient.SendAsync(JsonUtility.ToJson(wsProxyMessage), new Action<bool>(SynthesisArcadeObject.onWsSend));
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[" + this.TAG + "] [StartMultiplayerSynchronizationException] => " + ex.Message);
		}
		return false;
	}

	// Token: 0x060058AB RID: 22699 RVA: 0x001CCC68 File Offset: 0x001CAE68
	private void Wsclient_OnMessage(object sender, MessageEventArgs e)
	{
		string text = Encoding.UTF8.GetString(e.RawData).Trim();
		if (text.StartsWith("{") && text.Trim().EndsWith("}"))
		{
			try
			{
				WsProxyMessage wsProxyMessage = JsonUtility.FromJson<WsProxyMessage>(text);
				if (wsProxyMessage.action == WS_PROXY_ACTIONS.SYNCHRONIZED)
				{
					if (this.synchronizationAction != null)
					{
						this.synchronizationAction(wsProxyMessage.arguments);
					}
				}
				else
				{
					this.processLiveCommand(wsProxyMessage.interactionName, wsProxyMessage.arguments);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Concat(new string[] { "[", this.TAG, "] [WEBSOCKET_EXCEPTION->] ERROR Parsing JSON [", text, "] => ", ex.Message }));
			}
		}
	}

	// Token: 0x060058AC RID: 22700 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Wsclient_OnOpen(object sender, EventArgs e)
	{
	}

	// Token: 0x060058AD RID: 22701 RVA: 0x00002C2D File Offset: 0x00000E2D
	private static void onWsSend(bool status)
	{
	}

	// Token: 0x060058AE RID: 22702 RVA: 0x001CCD40 File Offset: 0x001CAF40
	private void Wsclient_OnError(object sender, ErrorEventArgs e)
	{
		Debug.LogError("[" + this.TAG + "] [WEBSOCKET_ONERROR] " + e.Message);
	}

	// Token: 0x060058AF RID: 22703 RVA: 0x001CCD62 File Offset: 0x001CAF62
	private void Wsclient_OnClose(object sender, EventArgs e)
	{
		Debug.LogError("[" + this.TAG + "] [WEBSOCKET_ONCLOSE]");
	}

	// Token: 0x060058B0 RID: 22704 RVA: 0x001CCD7E File Offset: 0x001CAF7E
	private IEnumerator EnableLicensingCheckTask()
	{
		this.svrInterfaceWin = ArcadeFactory.Synthesis();
		if (this.enableLiveInteractions)
		{
			this.synthesisUdpListener = new UdpClient();
			this.localUdpEndpoint = new IPEndPoint(IPAddress.Loopback, this.udpPort);
			this.fallbackCommandProcessor.AddListener(new UnityAction<string, string>(this.processUnknownLiveCommand));
			this.StartSynthesisUdpListener();
		}
		bool flag = !Application.isEditor || !this.disableSynthesisDRMInUnityEditor;
		if (flag && !this.synthesisCdnBuild && this.svrInterfaceWin != null && !this.svrInterfaceWin.IsSynthesisVRStation())
		{
			flag = false;
		}
		if (flag)
		{
			if (this.svrInterfaceWin != null)
			{
				this.allowEnteringTheLoop = true;
				this.svrInterfaceWin.Init(this.synthesisGameId, 1);
			}
			while (this.allowEnteringTheLoop)
			{
				SynthesisArcadeObject.<>c__DisplayClass46_0 CS$<>8__locals1 = new SynthesisArcadeObject.<>c__DisplayClass46_0();
				yield return new WaitForSecondsRealtime(1f);
				CS$<>8__locals1.checkTask = null;
				if (this.svrInterfaceWin != null && (!this.synthesisFlags.Contains("relaxed") || this.successfulDrmChecks <= 30) && (!this.synthesisFlags.Contains("chill") || this.successfulDrmChecks <= 5))
				{
					CS$<>8__locals1.checkTask = Task.Run<int>(() => this.svrInterfaceWin.CheckInMode());
					yield return new WaitUntil(() => CS$<>8__locals1.checkTask.IsCompleted);
					if (CS$<>8__locals1.checkTask.IsFaulted)
					{
						Debug.LogError(string.Format("[{0}] Task threw: {1}", this.TAG, CS$<>8__locals1.checkTask.Exception.Flatten().InnerException));
						continue;
					}
					int result = CS$<>8__locals1.checkTask.Result;
					if (result == 0)
					{
						this.successfulDrmChecks++;
					}
					else
					{
						this.successfulDrmChecks = 0;
					}
					if (result > 1)
					{
						Debug.Log(string.Concat(new string[]
						{
							"[",
							this.TAG,
							"] Check=",
							result.ToString(),
							this.synthesisFlags.Contains("debug") ? (" || Debug=" + this.svrInterfaceWin.DebugString()) : ""
						}));
					}
					if (!Application.isEditor && result >= 30)
					{
						this.allowEnteringTheLoop = false;
					}
				}
				CS$<>8__locals1 = null;
			}
			this.quitGame = true;
		}
		yield break;
	}

	// Token: 0x060058B1 RID: 22705 RVA: 0x001CCD90 File Offset: 0x001CAF90
	protected string filehash(string fileName)
	{
		if (!File.Exists(fileName))
		{
			return "";
		}
		try
		{
			using (MD5 md = MD5.Create())
			{
				using (FileStream fileStream = File.OpenRead(fileName))
				{
					return BitConverter.ToString(md.ComputeHash(fileStream)).Replace("-", string.Empty).ToLowerInvariant();
				}
			}
		}
		catch
		{
			Application.Quit();
		}
		return "";
	}

	// Token: 0x060058B2 RID: 22706 RVA: 0x001CCE28 File Offset: 0x001CB028
	public bool AddToLeaderboard(string data)
	{
		Debug.Log("[" + this.TAG + "] Leaderboard Data -> " + data);
		bool flag = false;
		if (this.svrInterfaceWin != null)
		{
			flag = this.svrInterfaceWin.AddToLeaderboard(data);
		}
		return flag;
	}

	// Token: 0x060058B3 RID: 22707 RVA: 0x001CCE68 File Offset: 0x001CB068
	public int sessionSecondsLeft()
	{
		int num = 1;
		if (this.svrInterfaceWin != null)
		{
			num = this.svrInterfaceWin.getSessionSecondsLeft();
		}
		return num;
	}

	// Token: 0x060058B4 RID: 22708 RVA: 0x001CCE8C File Offset: 0x001CB08C
	public bool resetBillingSession()
	{
		Debug.Log("[" + this.TAG + "] Reset PPM Session");
		bool flag = false;
		if (this.svrInterfaceWin != null)
		{
			flag = this.svrInterfaceWin.ResetBillingSession(this.synthesisGameId);
		}
		return flag;
	}

	// Token: 0x060058B5 RID: 22709 RVA: 0x001CCED0 File Offset: 0x001CB0D0
	public void setEngineData(string _key, string _value)
	{
		Debug.Log(string.Concat(new string[] { "[", this.TAG, "] Set[", _key, "][", _value, "]" }));
		this.svrInterfaceWin.setKeyValue(_key, _value);
	}

	// Token: 0x060058B6 RID: 22710 RVA: 0x001CCF2B File Offset: 0x001CB12B
	public void excludeFromDefaultBilling()
	{
		Debug.Log("[" + this.TAG + "] Stop Default Billing Logic");
		if (this.svrInterfaceWin != null)
		{
			this.manualPpmTracking = true;
			this.svrInterfaceWin.excludeFromBilling(true);
		}
	}

	// Token: 0x060058B7 RID: 22711 RVA: 0x001CCF64 File Offset: 0x001CB164
	public bool startManualPpmTracking()
	{
		Debug.Log("[" + this.TAG + "] Start PPM Tracking");
		this.excludeFromDefaultBilling();
		if (this.manualPpmTracking)
		{
			this.manualPpmTracking = this.svrInterfaceWin.timeTracking("start");
			if (!this.manualPpmTracking)
			{
				this.svrInterfaceWin.excludeFromBilling(false);
			}
		}
		return this.manualPpmTracking;
	}

	// Token: 0x060058B8 RID: 22712 RVA: 0x001CCFCC File Offset: 0x001CB1CC
	public bool stopManualPpmTracking()
	{
		Debug.Log("[" + this.TAG + "] Stop PPM Tracking");
		if (this.manualPpmTracking)
		{
			if (this.svrInterfaceWin != null)
			{
				this.manualPpmTracking = this.svrInterfaceWin.timeTracking("end");
			}
			if (this.manualPpmTracking)
			{
				this.manualPpmTracking = false;
			}
		}
		return this.manualPpmTracking;
	}

	// Token: 0x060058B9 RID: 22713 RVA: 0x00092236 File Offset: 0x00090436
	public string AndroidConfigFileRead(string filename)
	{
		return "";
	}

	// Token: 0x060058BA RID: 22714 RVA: 0x00002076 File Offset: 0x00000276
	public bool AndroidConfigFileWrite(string filename, string content)
	{
		return false;
	}

	// Token: 0x040068D5 RID: 26837
	internal string TAG = "SynthesisVRArcadeObject";

	// Token: 0x040068D6 RID: 26838
	internal static SynthesisArcadeObject _instance;

	// Token: 0x040068D7 RID: 26839
	private Coroutine co;

	// Token: 0x040068D8 RID: 26840
	public string synthesisGameId;

	// Token: 0x040068D9 RID: 26841
	public bool synthesisCdnBuild = true;

	// Token: 0x040068DA RID: 26842
	public bool disableSynthesisDRMInUnityEditor = true;

	// Token: 0x040068DB RID: 26843
	private bool quitGame;

	// Token: 0x040068DC RID: 26844
	private bool manualPpmTracking;

	// Token: 0x040068DD RID: 26845
	private string webSocketClientAddress = "";

	// Token: 0x040068DE RID: 26846
	private double webSocketLastConnectionAttemptEpoch;

	// Token: 0x040068DF RID: 26847
	private Action<string> synchronizationAction;

	// Token: 0x040068E0 RID: 26848
	private List<string> synthesisFlags = new List<string>();

	// Token: 0x040068E1 RID: 26849
	private int successfulDrmChecks;

	// Token: 0x040068E2 RID: 26850
	public bool enableLiveInteractions;

	// Token: 0x040068E3 RID: 26851
	[Range(1000f, 65535f)]
	public int udpPort = 23232;

	// Token: 0x040068E4 RID: 26852
	internal static volatile Queue<SynthesisUdpCommand> udpQueue = new Queue<SynthesisUdpCommand>();

	// Token: 0x040068E5 RID: 26853
	public List<SynthesisUdpCommand> commandDefinitions;

	// Token: 0x040068E6 RID: 26854
	public UnityEvent<string, string> fallbackCommandProcessor = new UnityEvent<string, string>();

	// Token: 0x040068E7 RID: 26855
	internal static WebSocket wsclient;

	// Token: 0x040068E8 RID: 26856
	private ArcadeInterface svrInterfaceWin;

	// Token: 0x040068E9 RID: 26857
	private UdpClient synthesisUdpListener;

	// Token: 0x040068EA RID: 26858
	private IPEndPoint localUdpEndpoint;

	// Token: 0x040068EB RID: 26859
	internal bool allowEnteringTheLoop;

	// Token: 0x02000E28 RID: 3624
	private enum DRM_VERIFICATION_MODE
	{
		// Token: 0x040068ED RID: 26861
		DEFAULT,
		// Token: 0x040068EE RID: 26862
		WEB
	}
}
