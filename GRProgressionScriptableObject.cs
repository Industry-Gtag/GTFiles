using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007DA RID: 2010
public class GRProgressionScriptableObject : ScriptableObject
{
	// Token: 0x0400428F RID: 17039
	[SerializeField]
	[Header("Progression Tiers")]
	public List<GRPlayer.ProgressionLevels> progressionData;
}
