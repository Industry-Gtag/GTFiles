using System;
using ExitGames.Client.Photon;
using Photon.Voice;
using Photon.Voice.Unity;
using POpusCodec.Enums;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020010A4 RID: 4260
	[CreateAssetMenu(fileName = "VoiceSettings", menuName = "Gorilla Tag/VoiceSettings")]
	public class SO_NetworkVoiceSettings : ScriptableObject
	{
		// Token: 0x040079A5 RID: 31141
		[Header("Voice settings")]
		public bool AutoConnectAndJoin = true;

		// Token: 0x040079A6 RID: 31142
		public bool AutoLeaveAndDisconnect = true;

		// Token: 0x040079A7 RID: 31143
		public bool WorkInOfflineMode = true;

		// Token: 0x040079A8 RID: 31144
		public DebugLevel LogLevel = DebugLevel.ERROR;

		// Token: 0x040079A9 RID: 31145
		public DebugLevel GlobalRecordersLogLevel = DebugLevel.INFO;

		// Token: 0x040079AA RID: 31146
		public DebugLevel GlobalSpeakersLogLevel = DebugLevel.INFO;

		// Token: 0x040079AB RID: 31147
		public bool CreateSpeakerIfNotFound;

		// Token: 0x040079AC RID: 31148
		public int UpdateInterval = 50;

		// Token: 0x040079AD RID: 31149
		public bool SupportLogger;

		// Token: 0x040079AE RID: 31150
		public int BackgroundTimeout = 60000;

		// Token: 0x040079AF RID: 31151
		[Header("Recorder Settings")]
		public bool RecordOnlyWhenEnabled;

		// Token: 0x040079B0 RID: 31152
		public bool RecordOnlyWhenJoined = true;

		// Token: 0x040079B1 RID: 31153
		public bool StopRecordingWhenPaused;

		// Token: 0x040079B2 RID: 31154
		public bool TransmitEnabled = true;

		// Token: 0x040079B3 RID: 31155
		public bool AutoStart = true;

		// Token: 0x040079B4 RID: 31156
		public bool Encrypt;

		// Token: 0x040079B5 RID: 31157
		public byte InterestGroup;

		// Token: 0x040079B6 RID: 31158
		public bool DebugEcho;

		// Token: 0x040079B7 RID: 31159
		public bool ReliableMode;

		// Token: 0x040079B8 RID: 31160
		[Header("Recorder Codec Parameters")]
		public OpusCodec.FrameDuration FrameDuration = OpusCodec.FrameDuration.Frame60ms;

		// Token: 0x040079B9 RID: 31161
		public SamplingRate SamplingRate = SamplingRate.Sampling16000;

		// Token: 0x040079BA RID: 31162
		[Range(6000f, 510000f)]
		public int Bitrate = 20000;

		// Token: 0x040079BB RID: 31163
		[Space]
		public SamplingRate SubsSamplingRate = SamplingRate.Sampling24000;

		// Token: 0x040079BC RID: 31164
		[Range(6000f, 510000f)]
		public int SubsBitrate = 30000;

		// Token: 0x040079BD RID: 31165
		[Header("Recorder Audio Source Settings")]
		public Recorder.InputSourceType InputSourceType;

		// Token: 0x040079BE RID: 31166
		public Recorder.MicType MicrophoneType;

		// Token: 0x040079BF RID: 31167
		public bool UseFallback = true;

		// Token: 0x040079C0 RID: 31168
		public bool Detect = true;

		// Token: 0x040079C1 RID: 31169
		[Range(0f, 1f)]
		public float Threshold = 0.07f;

		// Token: 0x040079C2 RID: 31170
		public int Delay = 500;
	}
}
