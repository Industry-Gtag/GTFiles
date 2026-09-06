using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000625 RID: 1573
public class BuilderWaterVolume : MonoBehaviour, IBuilderPieceComponent
{
	// Token: 0x0600272A RID: 10026 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnPieceCreate(int pieceType, int pieceId)
	{
	}

	// Token: 0x0600272B RID: 10027 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnPieceDestroy()
	{
	}

	// Token: 0x0600272C RID: 10028 RVA: 0x000CF254 File Offset: 0x000CD454
	public void OnPiecePlacementDeserialized()
	{
		bool flag = (double)Vector3.Dot(Vector3.up, base.transform.up) > 0.5 && !this.piece.IsPieceMoving();
		this.waterVolume.SetActive(flag);
		this.waterMesh.SetActive(flag);
		if (this.floatingObjects != null)
		{
			this.floatingObjects.localPosition = (flag ? this.floating.localPosition : this.sunk.localPosition);
		}
	}

	// Token: 0x0600272D RID: 10029 RVA: 0x000CF2E0 File Offset: 0x000CD4E0
	public void OnPieceActivate()
	{
		bool flag = (double)Vector3.Dot(Vector3.up, base.transform.up) > 0.5 && !this.piece.IsPieceMoving();
		this.waterVolume.SetActive(flag);
		this.waterMesh.SetActive(flag);
		if (this.floatingObjects != null)
		{
			this.floatingObjects.localPosition = (flag ? this.floating.localPosition : this.sunk.localPosition);
		}
	}

	// Token: 0x0600272E RID: 10030 RVA: 0x000CF36C File Offset: 0x000CD56C
	public void OnPieceDeactivate()
	{
		this.waterVolume.SetActive(false);
		this.waterMesh.SetActive(true);
		if (this.floatingObjects != null)
		{
			this.floatingObjects.localPosition = this.floating.localPosition;
		}
	}

	// Token: 0x040032BF RID: 12991
	[SerializeField]
	private BuilderPiece piece;

	// Token: 0x040032C0 RID: 12992
	[SerializeField]
	private GameObject waterVolume;

	// Token: 0x040032C1 RID: 12993
	[SerializeField]
	private GameObject waterMesh;

	// Token: 0x040032C2 RID: 12994
	[FormerlySerializedAs("lillyPads")]
	[SerializeField]
	private Transform floatingObjects;

	// Token: 0x040032C3 RID: 12995
	[SerializeField]
	private Transform floating;

	// Token: 0x040032C4 RID: 12996
	[SerializeField]
	private Transform sunk;
}
