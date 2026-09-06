using System;
using GorillaNetworking;
using PlayFab;
using UnityEngine;

// Token: 0x020009F2 RID: 2546
internal static class SpeakerVoiceToLoudnessConfig
{
	// Token: 0x1700062B RID: 1579
	// (get) Token: 0x0600415F RID: 16735 RVA: 0x0015C09A File Offset: 0x0015A29A
	public static bool EnableLoudnessLimit
	{
		get
		{
			return SpeakerVoiceToLoudnessConfig.k_config.EnableLoudnessLimit;
		}
	}

	// Token: 0x1700062C RID: 1580
	// (get) Token: 0x06004160 RID: 16736 RVA: 0x0015C0A6 File Offset: 0x0015A2A6
	public static float LoudnessLimitThreshold
	{
		get
		{
			return SpeakerVoiceToLoudnessConfig.k_config.LoudnessLimitThreshold;
		}
	}

	// Token: 0x06004161 RID: 16737 RVA: 0x0015C0B2 File Offset: 0x0015A2B2
	[RuntimeInitializeOnLoadMethod]
	private static void StaticLoad()
	{
		PlayFabTitleDataCache.RegisterOnLoad(new Action<PlayFabTitleDataCache>(SpeakerVoiceToLoudnessConfig.OnTitleDataCacheReady));
	}

	// Token: 0x06004162 RID: 16738 RVA: 0x0015C0C5 File Offset: 0x0015A2C5
	private static void OnTitleDataCacheReady(PlayFabTitleDataCache titleDataCache)
	{
		titleDataCache.GetTitleData("SpeakerVoiceToLoudnessConfig", new Action<string>(SpeakerVoiceToLoudnessConfig.OnTitleDataCacheResponse), new Action<PlayFabError>(SpeakerVoiceToLoudnessConfig.OnTitleDataCacheError), false);
	}

	// Token: 0x06004163 RID: 16739 RVA: 0x0015C0EC File Offset: 0x0015A2EC
	private static void OnTitleDataCacheResponse(string json)
	{
		SpeakerVoiceToLoudnessConfig.SerializedConfig serializedConfig = default(SpeakerVoiceToLoudnessConfig.SerializedConfig);
		try
		{
			serializedConfig = JsonUtility.FromJson<SpeakerVoiceToLoudnessConfig.SerializedConfig>(json);
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
			serializedConfig = SpeakerVoiceToLoudnessConfig.k_config;
		}
		finally
		{
			SpeakerVoiceToLoudnessConfig.k_config = serializedConfig;
		}
	}

	// Token: 0x06004164 RID: 16740 RVA: 0x00002C2D File Offset: 0x00000E2D
	private static void OnTitleDataCacheError(PlayFabError errorMsg)
	{
	}

	// Token: 0x040051FF RID: 20991
	private static SpeakerVoiceToLoudnessConfig.SerializedConfig k_config = new SpeakerVoiceToLoudnessConfig.SerializedConfig
	{
		EnableLoudnessLimit = true,
		LoudnessLimitThreshold = 0.5f
	};

	// Token: 0x04005200 RID: 20992
	public static StaticArrayBag<float> StaticArrays = new StaticArrayBag<float>();

	// Token: 0x04005201 RID: 20993
	private const string k_titleDataKey = "SpeakerVoiceToLoudnessConfig";

	// Token: 0x020009F3 RID: 2547
	[Serializable]
	private struct SerializedConfig
	{
		// Token: 0x04005202 RID: 20994
		public bool EnableLoudnessLimit;

		// Token: 0x04005203 RID: 20995
		public float LoudnessLimitThreshold;
	}
}
