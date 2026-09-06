using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F6D RID: 3949
	public struct BuilderGridPlaneData
	{
		// Token: 0x060061DD RID: 25053 RVA: 0x001F6634 File Offset: 0x001F4834
		public BuilderGridPlaneData(BuilderAttachGridPlane gridPlane, int pieceIndex)
		{
			gridPlane.center.transform.GetPositionAndRotation(out this.position, out this.rotation);
			this.localPosition = gridPlane.pieceToGridPosition;
			this.localRotation = gridPlane.pieceToGridRotation;
			this.width = gridPlane.width;
			this.length = gridPlane.length;
			this.male = gridPlane.male;
			this.pieceId = gridPlane.piece.pieceId;
			this.pieceIndex = pieceIndex;
			this.boundingRadius = gridPlane.boundingRadius;
			this.attachIndex = gridPlane.attachIndex;
		}

		// Token: 0x04007086 RID: 28806
		public int width;

		// Token: 0x04007087 RID: 28807
		public int length;

		// Token: 0x04007088 RID: 28808
		public bool male;

		// Token: 0x04007089 RID: 28809
		public int pieceId;

		// Token: 0x0400708A RID: 28810
		public int pieceIndex;

		// Token: 0x0400708B RID: 28811
		public float boundingRadius;

		// Token: 0x0400708C RID: 28812
		public int attachIndex;

		// Token: 0x0400708D RID: 28813
		public Vector3 position;

		// Token: 0x0400708E RID: 28814
		public Quaternion rotation;

		// Token: 0x0400708F RID: 28815
		public Vector3 localPosition;

		// Token: 0x04007090 RID: 28816
		public Quaternion localRotation;
	}
}
