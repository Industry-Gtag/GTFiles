using System;

// Token: 0x0200061C RID: 1564
public class BuilderPieceInteractorFindNearby : MonoBehaviourPostTick
{
	// Token: 0x0600270F RID: 9999 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Awake()
	{
	}

	// Token: 0x06002710 RID: 10000 RVA: 0x000CEDD6 File Offset: 0x000CCFD6
	public override void PostTick()
	{
		if (this.pieceInteractor != null)
		{
			this.pieceInteractor.StartFindNearbyPieces();
		}
	}

	// Token: 0x0400329D RID: 12957
	public BuilderPieceInteractor pieceInteractor;
}
