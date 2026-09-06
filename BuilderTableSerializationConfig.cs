using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000624 RID: 1572
[CreateAssetMenu(fileName = "BuilderTableSerializationConfig", menuName = "Gorilla Tag/Builder/Serialization", order = 0)]
public class BuilderTableSerializationConfig : ScriptableObject
{
	// Token: 0x040032B4 RID: 12980
	public string tableConfigurationKey;

	// Token: 0x040032B5 RID: 12981
	public string titleDataKey;

	// Token: 0x040032B6 RID: 12982
	public string startingMapConfigKey;

	// Token: 0x040032B7 RID: 12983
	public List<string> scanSlotMothershipKeys;

	// Token: 0x040032B8 RID: 12984
	public string scanSlotDevKey;

	// Token: 0x040032B9 RID: 12985
	public string publishedScanMothershipKey;

	// Token: 0x040032BA RID: 12986
	public string timeAppend;

	// Token: 0x040032BB RID: 12987
	public string playfabScanKey;

	// Token: 0x040032BC RID: 12988
	public string sharedBlocksApiBaseURL;

	// Token: 0x040032BD RID: 12989
	public string recentVotesPrefsKey;

	// Token: 0x040032BE RID: 12990
	public string localMapsPrefsKey;
}
