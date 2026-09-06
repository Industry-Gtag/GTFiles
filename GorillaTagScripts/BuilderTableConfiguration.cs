using System;

namespace GorillaTagScripts
{
	// Token: 0x02000F60 RID: 3936
	[Serializable]
	public class BuilderTableConfiguration
	{
		// Token: 0x060060DD RID: 24797 RVA: 0x001EBD58 File Offset: 0x001E9F58
		public BuilderTableConfiguration()
		{
			this.version = 0;
			this.TableResourceLimits = new int[3];
			this.PlotResourceLimits = new int[3];
			this.updateCountdownDate = string.Empty;
		}

		// Token: 0x04006F80 RID: 28544
		public const int CONFIGURATION_VERSION = 0;

		// Token: 0x04006F81 RID: 28545
		public int version;

		// Token: 0x04006F82 RID: 28546
		public int[] TableResourceLimits;

		// Token: 0x04006F83 RID: 28547
		public int[] PlotResourceLimits;

		// Token: 0x04006F84 RID: 28548
		public int DroppedPieceLimit;

		// Token: 0x04006F85 RID: 28549
		public string updateCountdownDate;
	}
}
