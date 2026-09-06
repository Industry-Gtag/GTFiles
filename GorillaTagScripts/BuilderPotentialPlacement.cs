using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F61 RID: 3937
	public struct BuilderPotentialPlacement
	{
		// Token: 0x060060DE RID: 24798 RVA: 0x001EBD8C File Offset: 0x001E9F8C
		public void Reset()
		{
			this.attachPiece = null;
			this.parentPiece = null;
			this.attachIndex = -1;
			this.parentAttachIndex = -1;
			this.localPosition = Vector3.zero;
			this.localRotation = Quaternion.identity;
			this.attachDistance = float.MaxValue;
			this.attachPlaneNormal = Vector3.zero;
			this.score = float.MinValue;
			this.twist = 0;
			this.bumpOffsetX = 0;
			this.bumpOffsetZ = 0;
		}

		// Token: 0x04006F86 RID: 28550
		public BuilderPiece attachPiece;

		// Token: 0x04006F87 RID: 28551
		public BuilderPiece parentPiece;

		// Token: 0x04006F88 RID: 28552
		public int attachIndex;

		// Token: 0x04006F89 RID: 28553
		public int parentAttachIndex;

		// Token: 0x04006F8A RID: 28554
		public Vector3 localPosition;

		// Token: 0x04006F8B RID: 28555
		public Quaternion localRotation;

		// Token: 0x04006F8C RID: 28556
		public Vector3 attachPlaneNormal;

		// Token: 0x04006F8D RID: 28557
		public float attachDistance;

		// Token: 0x04006F8E RID: 28558
		public float score;

		// Token: 0x04006F8F RID: 28559
		public SnapBounds attachBounds;

		// Token: 0x04006F90 RID: 28560
		public SnapBounds parentAttachBounds;

		// Token: 0x04006F91 RID: 28561
		public byte twist;

		// Token: 0x04006F92 RID: 28562
		public sbyte bumpOffsetX;

		// Token: 0x04006F93 RID: 28563
		public sbyte bumpOffsetZ;
	}
}
