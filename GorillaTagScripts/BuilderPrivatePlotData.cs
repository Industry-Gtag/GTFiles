using System;

namespace GorillaTagScripts
{
	// Token: 0x02000F70 RID: 3952
	public struct BuilderPrivatePlotData
	{
		// Token: 0x060061DF RID: 25055 RVA: 0x001F6777 File Offset: 0x001F4977
		public BuilderPrivatePlotData(BuilderPiecePrivatePlot plot)
		{
			this.plotState = plot.plotState;
			this.ownerActorNumber = plot.GetOwnerActorNumber();
			this.isUnderCapacityLeft = false;
			this.isUnderCapacityRight = false;
		}

		// Token: 0x0400709D RID: 28829
		public BuilderPiecePrivatePlot.PlotState plotState;

		// Token: 0x0400709E RID: 28830
		public int ownerActorNumber;

		// Token: 0x0400709F RID: 28831
		public bool isUnderCapacityLeft;

		// Token: 0x040070A0 RID: 28832
		public bool isUnderCapacityRight;
	}
}
