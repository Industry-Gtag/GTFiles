using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GorillaGameModes;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000C9C RID: 3228
public class LuauHud : MonoBehaviour
{
	// Token: 0x1700076C RID: 1900
	// (get) Token: 0x06004FB4 RID: 20404 RVA: 0x001A7634 File Offset: 0x001A5834
	public static LuauHud Instance
	{
		get
		{
			return LuauHud._instance;
		}
	}

	// Token: 0x06004FB5 RID: 20405 RVA: 0x001A763C File Offset: 0x001A583C
	private void Awake()
	{
		if (LuauHud._instance != null && LuauHud._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		LuauHud._instance = this;
		this.path = Path.Combine(Application.persistentDataPath, "script.luau");
	}

	// Token: 0x06004FB6 RID: 20406 RVA: 0x001A768A File Offset: 0x001A588A
	private void OnDestroy()
	{
		if (LuauHud._instance == this)
		{
			LuauHud._instance = null;
		}
	}

	// Token: 0x06004FB7 RID: 20407 RVA: 0x001A76A0 File Offset: 0x001A58A0
	private void Start()
	{
		this.useLuauHud = true;
		DebugHudStats instance = DebugHudStats.Instance;
		instance.enabled = false;
		this.debugHud = instance.gameObject;
		this.text = instance.text;
		this.text.gameObject.SetActive(false);
		this.text.gameObject.transform.Rotate(180f * Vector3.up, Space.World);
		this.builder = new StringBuilder(50);
	}

	// Token: 0x06004FB8 RID: 20408 RVA: 0x001A771C File Offset: 0x001A591C
	private void Update()
	{
		if (!CustomMapLoader.IsDevModeEnabled())
		{
			if (this.showLog && this.useLuauHud)
			{
				this.showLog = false;
				DebugHudStats instance = DebugHudStats.Instance;
				if (instance != null)
				{
					instance.gameObject.SetActive(false);
				}
				this.text.gameObject.SetActive(false);
			}
			return;
		}
		GorillaGameManager instance2 = GorillaGameManager.instance;
		if (instance2 == null || instance2.GameType() != GameModeType.Custom)
		{
			return;
		}
		bool flag = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
		bool flag2 = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
		if (flag != this.buttonDown && this.useLuauHud)
		{
			this.buttonDown = flag;
			if (!this.buttonDown)
			{
				if (!this.text.gameObject.activeInHierarchy)
				{
					DebugHudStats instance3 = DebugHudStats.Instance;
					if (instance3 != null)
					{
						instance3.gameObject.SetActive(true);
					}
					this.text.gameObject.SetActive(true);
					this.showLog = true;
				}
				else
				{
					DebugHudStats instance4 = DebugHudStats.Instance;
					if (instance4 != null)
					{
						instance4.gameObject.SetActive(false);
					}
					this.text.gameObject.SetActive(false);
					this.showLog = false;
				}
			}
		}
		if (!flag || !flag2)
		{
			this.resetTimer = Time.time;
		}
		if (Time.time - this.resetTimer > 2f && CustomGameMode.GameModeInitialized)
		{
			this.RestartLuauScript();
			this.resetTimer = Time.time;
		}
		if (this.useLuauHud && this.showLog)
		{
			this.builder.Clear();
			this.builder.AppendLine();
			for (int i = 0; i < this.luauLogs.Count; i++)
			{
				this.builder.AppendLine(this.luauLogs[i]);
			}
			this.text.text = this.builder.ToString();
		}
	}

	// Token: 0x06004FB9 RID: 20409 RVA: 0x001A78D4 File Offset: 0x001A5AD4
	public void RestartLuauScript()
	{
		this.LuauLog("Restarting Luau Script");
		LuauScriptRunner gameScriptRunner = CustomGameMode.gameScriptRunner;
		if (gameScriptRunner != null && gameScriptRunner.ShouldTick)
		{
			CustomGameMode.StopScript();
		}
		this.script = this.LoadLocalScript();
		if (this.script != "")
		{
			this.LuauLog("Loaded script from: " + this.path);
			this.LuauLog("Loaded Script Text: \n" + this.script);
			CustomGameMode.LuaScript = this.script;
		}
		CustomGameMode.LuaStart();
	}

	// Token: 0x06004FBA RID: 20410 RVA: 0x001A7960 File Offset: 0x001A5B60
	public string LoadLocalScript()
	{
		string text = "";
		if (File.Exists(this.path))
		{
			text = File.ReadAllText(this.path);
		}
		return text;
	}

	// Token: 0x06004FBB RID: 20411 RVA: 0x001A798D File Offset: 0x001A5B8D
	public void LuauLog(string log)
	{
		Debug.Log(log);
		this.luauLogs.Add(log);
		if (this.luauLogs.Count > 6)
		{
			this.luauLogs.RemoveAt(0);
		}
	}

	// Token: 0x040061E4 RID: 25060
	private bool useLuauHud;

	// Token: 0x040061E5 RID: 25061
	private bool buttonDown;

	// Token: 0x040061E6 RID: 25062
	private bool showLog;

	// Token: 0x040061E7 RID: 25063
	private GameObject debugHud;

	// Token: 0x040061E8 RID: 25064
	private TMP_Text text;

	// Token: 0x040061E9 RID: 25065
	private StringBuilder builder;

	// Token: 0x040061EA RID: 25066
	private float resetTimer;

	// Token: 0x040061EB RID: 25067
	private string path = "";

	// Token: 0x040061EC RID: 25068
	private string script = "";

	// Token: 0x040061ED RID: 25069
	private static LuauHud _instance;

	// Token: 0x040061EE RID: 25070
	private List<string> luauLogs = new List<string>();
}
