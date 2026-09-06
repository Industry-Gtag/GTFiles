using System;
using System.Collections.Generic;

namespace GorillaTagScripts
{
	// Token: 0x02000F79 RID: 3961
	[Serializable]
	public class BuilderTableData
	{
		// Token: 0x0600623D RID: 25149 RVA: 0x001F9EB0 File Offset: 0x001F80B0
		public BuilderTableData()
		{
			this.version = 4;
			this.numEdits = 0;
			this.numPieces = 0;
			this.pieceType = new List<int>(1024);
			this.pieceId = new List<int>(1024);
			this.parentId = new List<int>(1024);
			this.attachIndex = new List<int>(1024);
			this.parentAttachIndex = new List<int>(1024);
			this.placement = new List<int>(1024);
			this.materialType = new List<int>(1024);
			this.overlapingPieces = new List<int>(1024);
			this.overlappedPieces = new List<int>(1024);
			this.overlapInfo = new List<long>(1024);
			this.timeOffset = new List<int>(1024);
		}

		// Token: 0x0600623E RID: 25150 RVA: 0x001F9F88 File Offset: 0x001F8188
		public void Clear()
		{
			this.numPieces = 0;
			this.pieceType.Clear();
			this.pieceId.Clear();
			this.parentId.Clear();
			this.attachIndex.Clear();
			this.parentAttachIndex.Clear();
			this.placement.Clear();
			this.materialType.Clear();
			this.overlapingPieces.Clear();
			this.overlappedPieces.Clear();
			this.overlapInfo.Clear();
			this.timeOffset.Clear();
		}

		// Token: 0x040070FC RID: 28924
		public const int BUILDER_TABLE_DATA_VERSION = 4;

		// Token: 0x040070FD RID: 28925
		public int version;

		// Token: 0x040070FE RID: 28926
		public int numEdits;

		// Token: 0x040070FF RID: 28927
		public int numPieces;

		// Token: 0x04007100 RID: 28928
		public List<int> pieceType;

		// Token: 0x04007101 RID: 28929
		public List<int> pieceId;

		// Token: 0x04007102 RID: 28930
		public List<int> parentId;

		// Token: 0x04007103 RID: 28931
		public List<int> attachIndex;

		// Token: 0x04007104 RID: 28932
		public List<int> parentAttachIndex;

		// Token: 0x04007105 RID: 28933
		public List<int> placement;

		// Token: 0x04007106 RID: 28934
		public List<int> materialType;

		// Token: 0x04007107 RID: 28935
		public List<int> overlapingPieces;

		// Token: 0x04007108 RID: 28936
		public List<int> overlappedPieces;

		// Token: 0x04007109 RID: 28937
		public List<long> overlapInfo;

		// Token: 0x0400710A RID: 28938
		public List<int> timeOffset;
	}
}
