using System;
using System.Collections.Generic;

// Token: 0x02000B49 RID: 2889
public struct TelemetryData
{
	// Token: 0x04005BFB RID: 23547
	public string EventName;

	// Token: 0x04005BFC RID: 23548
	public string[] CustomTags;

	// Token: 0x04005BFD RID: 23549
	public Dictionary<string, string> BodyData;
}
