using System;

// Token: 0x0200090D RID: 2317
public interface ISpeakerLoudness
{
	// Token: 0x17000586 RID: 1414
	// (get) Token: 0x06003CC0 RID: 15552
	bool IsSpeaking { get; }

	// Token: 0x17000587 RID: 1415
	// (get) Token: 0x06003CC1 RID: 15553
	float Loudness { get; }

	// Token: 0x17000588 RID: 1416
	// (get) Token: 0x06003CC2 RID: 15554
	bool IsMicEnabled { get; }
}
