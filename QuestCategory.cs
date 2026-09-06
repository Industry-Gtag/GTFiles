using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// Token: 0x02000268 RID: 616
[JsonConverter(typeof(StringEnumConverter))]
[Serializable]
public enum QuestCategory
{
	// Token: 0x040013B2 RID: 5042
	NONE,
	// Token: 0x040013B3 RID: 5043
	Social,
	// Token: 0x040013B4 RID: 5044
	Exploration,
	// Token: 0x040013B5 RID: 5045
	Gameplay,
	// Token: 0x040013B6 RID: 5046
	GameRound,
	// Token: 0x040013B7 RID: 5047
	Tag
}
