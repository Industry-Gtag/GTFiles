using System;
using UniLabs.Time;
using UnityEngine;

// Token: 0x02000090 RID: 144
public class RealWorldDateTimeWindow : ScriptableObject
{
	// Token: 0x06000394 RID: 916 RVA: 0x00014D72 File Offset: 0x00012F72
	public bool MatchesDate(DateTime utcDate)
	{
		return this.startTime <= utcDate && this.endTime >= utcDate;
	}

	// Token: 0x0400041A RID: 1050
	[SerializeField]
	private UDateTime startTime;

	// Token: 0x0400041B RID: 1051
	[SerializeField]
	private UDateTime endTime;
}
