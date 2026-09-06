using System;

// Token: 0x0200063B RID: 1595
public interface IBuilderPieceComponent
{
	// Token: 0x060027BF RID: 10175
	void OnPieceCreate(int pieceType, int pieceId);

	// Token: 0x060027C0 RID: 10176
	void OnPieceDestroy();

	// Token: 0x060027C1 RID: 10177
	void OnPiecePlacementDeserialized();

	// Token: 0x060027C2 RID: 10178
	void OnPieceActivate();

	// Token: 0x060027C3 RID: 10179
	void OnPieceDeactivate();
}
