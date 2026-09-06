using System;

namespace GorillaTagScripts
{
	// Token: 0x02000F6E RID: 3950
	public struct BuilderPieceData
	{
		// Token: 0x060061DE RID: 25054 RVA: 0x001F66CC File Offset: 0x001F48CC
		public BuilderPieceData(BuilderPiece piece)
		{
			this.pieceId = piece.pieceId;
			this.pieceIndex = piece.pieceDataIndex;
			BuilderPiece parentPiece = piece.parentPiece;
			this.parentPieceIndex = ((parentPiece == null) ? (-1) : parentPiece.pieceDataIndex);
			BuilderPiece requestedParentPiece = piece.requestedParentPiece;
			this.requestedParentPieceIndex = ((requestedParentPiece == null) ? (-1) : requestedParentPiece.pieceDataIndex);
			this.preventSnapUntilMoved = piece.preventSnapUntilMoved;
			this.isBuiltIntoTable = piece.isBuiltIntoTable;
			this.state = piece.state;
			this.privatePlotIndex = piece.privatePlotIndex;
			this.isArmPiece = piece.isArmShelf;
			this.heldByActorNumber = piece.heldByPlayerActorNumber;
		}

		// Token: 0x04007091 RID: 28817
		public int pieceId;

		// Token: 0x04007092 RID: 28818
		public int pieceIndex;

		// Token: 0x04007093 RID: 28819
		public int parentPieceIndex;

		// Token: 0x04007094 RID: 28820
		public int requestedParentPieceIndex;

		// Token: 0x04007095 RID: 28821
		public int heldByActorNumber;

		// Token: 0x04007096 RID: 28822
		public int preventSnapUntilMoved;

		// Token: 0x04007097 RID: 28823
		public bool isBuiltIntoTable;

		// Token: 0x04007098 RID: 28824
		public BuilderPiece.State state;

		// Token: 0x04007099 RID: 28825
		public int privatePlotIndex;

		// Token: 0x0400709A RID: 28826
		public bool isArmPiece;
	}
}
