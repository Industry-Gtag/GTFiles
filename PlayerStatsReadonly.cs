using System;

// Token: 0x020008EB RID: 2283
[Serializable]
public readonly struct PlayerStatsReadonly
{
	// Token: 0x1700055F RID: 1375
	// (get) Token: 0x06003BBB RID: 15291 RVA: 0x001470BF File Offset: 0x001452BF
	public bool SwapInterval
	{
		get
		{
			return this.SystemPropertiesFlags.HasFlag(SystemProperties.SwapInterval);
		}
	}

	// Token: 0x17000560 RID: 1376
	// (get) Token: 0x06003BBC RID: 15292 RVA: 0x001470D7 File Offset: 0x001452D7
	public bool HalfRefreshRate
	{
		get
		{
			return this.SystemPropertiesFlags.HasFlag(SystemProperties.HalfRefreshRate);
		}
	}

	// Token: 0x17000561 RID: 1377
	// (get) Token: 0x06003BBD RID: 15293 RVA: 0x001470EF File Offset: 0x001452EF
	public bool GPULevel
	{
		get
		{
			return this.SystemPropertiesFlags.HasFlag(SystemProperties.GPULevel);
		}
	}

	// Token: 0x17000562 RID: 1378
	// (get) Token: 0x06003BBE RID: 15294 RVA: 0x00147107 File Offset: 0x00145307
	public bool CPULevel
	{
		get
		{
			return this.SystemPropertiesFlags.HasFlag(SystemProperties.CPULevel);
		}
	}

	// Token: 0x17000563 RID: 1379
	// (get) Token: 0x06003BBF RID: 15295 RVA: 0x0014711F File Offset: 0x0014531F
	public bool Headlock
	{
		get
		{
			return this.SystemPropertiesFlags.HasFlag(SystemProperties.Headlock);
		}
	}

	// Token: 0x17000564 RID: 1380
	// (get) Token: 0x06003BC0 RID: 15296 RVA: 0x00147138 File Offset: 0x00145338
	public bool HeadlockTranslationX
	{
		get
		{
			return this.SystemPropertiesFlags.HasFlag(SystemProperties.HeadlockTranslationX);
		}
	}

	// Token: 0x17000565 RID: 1381
	// (get) Token: 0x06003BC1 RID: 15297 RVA: 0x00147151 File Offset: 0x00145351
	public bool HeadlockTranslationY
	{
		get
		{
			return this.SystemPropertiesFlags.HasFlag(SystemProperties.HeadlockTranslationY);
		}
	}

	// Token: 0x17000566 RID: 1382
	// (get) Token: 0x06003BC2 RID: 15298 RVA: 0x0014716A File Offset: 0x0014536A
	public bool HeadlockTranslationZ
	{
		get
		{
			return this.SystemPropertiesFlags.HasFlag(SystemProperties.HeadlockTranslationZ);
		}
	}

	// Token: 0x17000567 RID: 1383
	// (get) Token: 0x06003BC3 RID: 15299 RVA: 0x00147186 File Offset: 0x00145386
	public bool PhaseSyncAdditionalPadding
	{
		get
		{
			return this.SystemPropertiesFlags.HasFlag(SystemProperties.PhaseSyncAdditionalPadding);
		}
	}

	// Token: 0x17000568 RID: 1384
	// (get) Token: 0x06003BC4 RID: 15300 RVA: 0x001471A2 File Offset: 0x001453A2
	public bool PhaseSyncDelayOverride
	{
		get
		{
			return this.SystemPropertiesFlags.HasFlag(SystemProperties.PhaseSyncDelayOverride);
		}
	}

	// Token: 0x17000569 RID: 1385
	// (get) Token: 0x06003BC5 RID: 15301 RVA: 0x001471BE File Offset: 0x001453BE
	public bool PhaseSyncPredictionTime
	{
		get
		{
			return this.SystemPropertiesFlags.HasFlag(SystemProperties.PhaseSyncPredictionTime);
		}
	}

	// Token: 0x1700056A RID: 1386
	// (get) Token: 0x06003BC6 RID: 15302 RVA: 0x001471DA File Offset: 0x001453DA
	public bool PhaseSync
	{
		get
		{
			return this.SystemPropertiesFlags.HasFlag(SystemProperties.PhaseSync);
		}
	}

	// Token: 0x1700056B RID: 1387
	// (get) Token: 0x06003BC7 RID: 15303 RVA: 0x001471F6 File Offset: 0x001453F6
	public bool RefreshRate
	{
		get
		{
			return this.SystemPropertiesFlags.HasFlag(SystemProperties.RefreshRate);
		}
	}

	// Token: 0x1700056C RID: 1388
	// (get) Token: 0x06003BC8 RID: 15304 RVA: 0x00147212 File Offset: 0x00145412
	public bool PredictionTime
	{
		get
		{
			return this.SystemPropertiesFlags.HasFlag(SystemProperties.PredictionTime);
		}
	}

	// Token: 0x06003BC9 RID: 15305 RVA: 0x0014722E File Offset: 0x0014542E
	public PlayerStatsReadonly(short ping, short fps, short targetFps, SystemProperties flags)
	{
		this.Ping = ping;
		this.FPS = fps;
		this.TargetFPS = targetFps;
		this.SystemPropertiesFlags = flags;
	}

	// Token: 0x04004C5C RID: 19548
	public readonly short Ping;

	// Token: 0x04004C5D RID: 19549
	public readonly short FPS;

	// Token: 0x04004C5E RID: 19550
	public readonly short TargetFPS;

	// Token: 0x04004C5F RID: 19551
	public readonly SystemProperties SystemPropertiesFlags;
}
