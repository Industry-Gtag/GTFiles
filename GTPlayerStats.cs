using System;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;
using Utilities;

// Token: 0x020008EC RID: 2284
public class GTPlayerStats : MonoBehaviourPostTick
{
	// Token: 0x1700056D RID: 1389
	// (get) Token: 0x06003BCA RID: 15306 RVA: 0x0014724D File Offset: 0x0014544D
	// (set) Token: 0x06003BCB RID: 15307 RVA: 0x00147254 File Offset: 0x00145454
	public static short Ping { get; private set; }

	// Token: 0x1700056E RID: 1390
	// (get) Token: 0x06003BCC RID: 15308 RVA: 0x0014725C File Offset: 0x0014545C
	// (set) Token: 0x06003BCD RID: 15309 RVA: 0x00147263 File Offset: 0x00145463
	public static short FPS { get; private set; }

	// Token: 0x1700056F RID: 1391
	// (get) Token: 0x06003BCE RID: 15310 RVA: 0x0014726B File Offset: 0x0014546B
	// (set) Token: 0x06003BCF RID: 15311 RVA: 0x00147272 File Offset: 0x00145472
	public static short TargetFPS { get; private set; }

	// Token: 0x17000570 RID: 1392
	// (get) Token: 0x06003BD0 RID: 15312 RVA: 0x0014727A File Offset: 0x0014547A
	// (set) Token: 0x06003BD1 RID: 15313 RVA: 0x00147281 File Offset: 0x00145481
	public static SystemProperties SystemPropertiesFlags
	{
		get
		{
			return GTPlayerStats.s_systemPropertiesFlags;
		}
		private set
		{
			GTPlayerStats.s_systemPropertiesFlags = value;
		}
	}

	// Token: 0x06003BD2 RID: 15314 RVA: 0x00147289 File Offset: 0x00145489
	public static long GetPackedValues()
	{
		return 0L | (long)GTPlayerStats.Ping | ((long)GTPlayerStats.FPS << 16) | ((long)GTPlayerStats.TargetFPS << 32);
	}

	// Token: 0x06003BD3 RID: 15315 RVA: 0x001472A8 File Offset: 0x001454A8
	public static PlayerStatsReadonly UnPackValues(long values, int flags)
	{
		short num = (short)values;
		short num2 = (short)(values >> 16);
		short num3 = (short)(values >> 32);
		return new PlayerStatsReadonly(num, num2, num3, (SystemProperties)flags);
	}

	// Token: 0x06003BD4 RID: 15316 RVA: 0x001472CB File Offset: 0x001454CB
	private void Awake()
	{
		this.m_periodicUpdate.callback = new Action(this.DelayedUpdate);
	}

	// Token: 0x06003BD5 RID: 15317 RVA: 0x001472E4 File Offset: 0x001454E4
	public override void OnEnable()
	{
		this.m_periodicUpdate.Start();
		this.DelayedUpdate();
	}

	// Token: 0x06003BD6 RID: 15318 RVA: 0x001472F7 File Offset: 0x001454F7
	public override void OnDisable()
	{
		this.m_periodicUpdate.Stop();
	}

	// Token: 0x06003BD7 RID: 15319 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void PostTick()
	{
	}

	// Token: 0x06003BD8 RID: 15320 RVA: 0x00147304 File Offset: 0x00145504
	private void DelayedUpdate()
	{
		float smoothDeltaTime = Time.smoothDeltaTime;
		if (smoothDeltaTime > 0f)
		{
			float num = 1f / smoothDeltaTime;
			this.m_fps.AddSample(num);
		}
		GTPlayerStats.FPS = (short)Mathf.RoundToInt(this.m_fps.Average);
		short fps = GTPlayerStats.FPS;
		int fps_THRESHOLD = DebugHudStats.FPS_THRESHOLD;
		int num2 = 0;
		if (PhotonNetwork.IsConnectedAndReady)
		{
			num2 = PhotonNetwork.GetPing();
		}
		this.m_ping.AddSample(num2);
		GTPlayerStats.Ping = (short)this.m_ping.Average;
		GTPlayerStats.TargetFPS = (short)Screen.currentResolution.refreshRateRatio.value;
		if (!XRSettings.enabled)
		{
			int vSyncCount = QualitySettings.vSyncCount;
			if (vSyncCount > 0)
			{
				GTPlayerStats.TargetFPS /= (short)vSyncCount;
				return;
			}
			if (Application.targetFrameRate < 0)
			{
				GTPlayerStats.TargetFPS = -1;
			}
		}
	}

	// Token: 0x04004C63 RID: 19555
	private static SystemProperties s_systemPropertiesFlags;

	// Token: 0x04004C64 RID: 19556
	private FloatAverages m_fps = new FloatAverages(30);

	// Token: 0x04004C65 RID: 19557
	private IntAverages m_ping = new IntAverages(10);

	// Token: 0x04004C66 RID: 19558
	private TickSystemTimer m_periodicUpdate = new TickSystemTimer(0.1f);
}
