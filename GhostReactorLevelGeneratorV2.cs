using System;
using System.Collections.Generic;
using JetBrains.Annotations;

// Token: 0x02000710 RID: 1808
public class GhostReactorLevelGeneratorV2
{
	// Token: 0x02000711 RID: 1809
	[Serializable]
	public struct TreeLevelConfig
	{
		// Token: 0x06002D93 RID: 11667 RVA: 0x000F66B4 File Offset: 0x000F48B4
		public bool ValidateDatetime([CanBeNull] string timestamp)
		{
			DateTime dateTime;
			return string.IsNullOrEmpty(timestamp) || DateTime.TryParse(timestamp, out dateTime);
		}

		// Token: 0x04003A47 RID: 14919
		[CanBeNull]
		public string EnableAfterDatetime;

		// Token: 0x04003A48 RID: 14920
		[CanBeNull]
		public string DisableAfterDatetime;

		// Token: 0x04003A49 RID: 14921
		public int minHubs;

		// Token: 0x04003A4A RID: 14922
		public int maxHubs;

		// Token: 0x04003A4B RID: 14923
		public int minCaps;

		// Token: 0x04003A4C RID: 14924
		public int maxCaps;

		// Token: 0x04003A4D RID: 14925
		public List<GhostReactorSpawnConfig> sectionSpawnConfigs;

		// Token: 0x04003A4E RID: 14926
		public List<GhostReactorSpawnConfig> endCapSpawnConfigs;

		// Token: 0x04003A4F RID: 14927
		public List<GhostReactorLevelSection> hubs;

		// Token: 0x04003A50 RID: 14928
		public List<GhostReactorLevelSection> endCaps;

		// Token: 0x04003A51 RID: 14929
		public List<GhostReactorLevelSection> blockers;

		// Token: 0x04003A52 RID: 14930
		public List<GhostReactorLevelSectionConnector> connectors;
	}
}
