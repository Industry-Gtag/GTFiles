using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// Token: 0x02000267 RID: 615
[JsonConverter(typeof(StringEnumConverter))]
[Serializable]
public enum QuestType
{
	// Token: 0x040013A1 RID: 5025
	none,
	// Token: 0x040013A2 RID: 5026
	gameModeObjective,
	// Token: 0x040013A3 RID: 5027
	gameModeRound,
	// Token: 0x040013A4 RID: 5028
	grabObject,
	// Token: 0x040013A5 RID: 5029
	dropObject,
	// Token: 0x040013A6 RID: 5030
	eatObject,
	// Token: 0x040013A7 RID: 5031
	tapObject,
	// Token: 0x040013A8 RID: 5032
	launchedProjectile,
	// Token: 0x040013A9 RID: 5033
	moveDistance,
	// Token: 0x040013AA RID: 5034
	swimDistance,
	// Token: 0x040013AB RID: 5035
	triggerHandEffect,
	// Token: 0x040013AC RID: 5036
	enterLocation,
	// Token: 0x040013AD RID: 5037
	misc,
	// Token: 0x040013AE RID: 5038
	critter,
	// Token: 0x040013AF RID: 5039
	fetchObject,
	// Token: 0x040013B0 RID: 5040
	playerInteraction
}
