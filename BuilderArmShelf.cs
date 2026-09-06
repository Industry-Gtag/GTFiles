using System;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x0200062A RID: 1578
public class BuilderArmShelf : MonoBehaviour
{
	// Token: 0x0600273F RID: 10047 RVA: 0x000CF92C File Offset: 0x000CDB2C
	private void Start()
	{
		this.ownerRig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x06002740 RID: 10048 RVA: 0x000CF93A File Offset: 0x000CDB3A
	public bool IsOwnedLocally()
	{
		return this.ownerRig != null && this.ownerRig.isLocal;
	}

	// Token: 0x06002741 RID: 10049 RVA: 0x000CF957 File Offset: 0x000CDB57
	public bool CanAttachToArmPiece()
	{
		return this.ownerRig != null && this.ownerRig.scaleFactor >= 1f;
	}

	// Token: 0x06002742 RID: 10050 RVA: 0x000CF980 File Offset: 0x000CDB80
	public void DropAttachedPieces()
	{
		if (this.ownerRig != null && this.piece != null)
		{
			Vector3 vector = Vector3.zero;
			if (this.piece.firstChildPiece == null)
			{
				return;
			}
			BuilderTable table = this.piece.GetTable();
			Vector3 vector2 = table.roomCenter.position - this.piece.transform.position;
			vector2.Normalize();
			Vector3 vector3 = Quaternion.Euler(0f, 180f, 0f) * vector2;
			vector = BuilderTable.DROP_ZONE_REPEL * vector3;
			BuilderPiece builderPiece = this.piece.firstChildPiece;
			while (builderPiece != null)
			{
				table.RequestDropPiece(builderPiece, builderPiece.transform.position + vector3 * 0.1f, builderPiece.transform.rotation, vector, Vector3.zero);
				builderPiece = builderPiece.nextSiblingPiece;
			}
		}
	}

	// Token: 0x040032E4 RID: 13028
	[HideInInspector]
	public BuilderPiece piece;

	// Token: 0x040032E5 RID: 13029
	public Transform pieceAnchor;

	// Token: 0x040032E6 RID: 13030
	private VRRig ownerRig;
}
