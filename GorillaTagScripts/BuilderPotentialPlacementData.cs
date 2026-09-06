using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F71 RID: 3953
	public struct BuilderPotentialPlacementData
	{
		// Token: 0x060061E0 RID: 25056 RVA: 0x001F67A0 File Offset: 0x001F49A0
		public BuilderPotentialPlacement ToPotentialPlacement(BuilderTable table)
		{
			BuilderPotentialPlacement builderPotentialPlacement = new BuilderPotentialPlacement
			{
				attachPiece = table.GetPiece(this.pieceId),
				parentPiece = table.GetPiece(this.parentPieceId),
				score = this.score,
				localPosition = this.localPosition,
				localRotation = this.localRotation,
				attachIndex = this.attachIndex,
				parentAttachIndex = this.parentAttachIndex,
				attachDistance = this.attachDistance,
				attachPlaneNormal = this.attachPlaneNormal,
				attachBounds = this.attachBounds,
				parentAttachBounds = this.parentAttachBounds,
				twist = this.twist,
				bumpOffsetX = this.bumpOffsetX,
				bumpOffsetZ = this.bumpOffsetZ
			};
			if (builderPotentialPlacement.parentPiece != null)
			{
				BuilderAttachGridPlane builderAttachGridPlane = builderPotentialPlacement.parentPiece.gridPlanes[builderPotentialPlacement.parentAttachIndex];
				builderPotentialPlacement.localPosition = builderAttachGridPlane.transform.InverseTransformPoint(builderPotentialPlacement.localPosition);
				builderPotentialPlacement.localRotation = Quaternion.Inverse(builderAttachGridPlane.transform.rotation) * builderPotentialPlacement.localRotation;
			}
			return builderPotentialPlacement;
		}

		// Token: 0x040070A1 RID: 28833
		public int pieceId;

		// Token: 0x040070A2 RID: 28834
		public int parentPieceId;

		// Token: 0x040070A3 RID: 28835
		public float score;

		// Token: 0x040070A4 RID: 28836
		public Vector3 localPosition;

		// Token: 0x040070A5 RID: 28837
		public Quaternion localRotation;

		// Token: 0x040070A6 RID: 28838
		public int attachIndex;

		// Token: 0x040070A7 RID: 28839
		public int parentAttachIndex;

		// Token: 0x040070A8 RID: 28840
		public float attachDistance;

		// Token: 0x040070A9 RID: 28841
		public Vector3 attachPlaneNormal;

		// Token: 0x040070AA RID: 28842
		public SnapBounds attachBounds;

		// Token: 0x040070AB RID: 28843
		public SnapBounds parentAttachBounds;

		// Token: 0x040070AC RID: 28844
		public byte twist;

		// Token: 0x040070AD RID: 28845
		public sbyte bumpOffsetX;

		// Token: 0x040070AE RID: 28846
		public sbyte bumpOffsetZ;
	}
}
