using System;
using BoingKit;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02001038 RID: 4152
	public class BuilderPieceOrientUp : MonoBehaviour, IBuilderPieceComponent
	{
		// Token: 0x06006772 RID: 26482 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPieceCreate(int pieceType, int pieceId)
		{
		}

		// Token: 0x06006773 RID: 26483 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06006774 RID: 26484 RVA: 0x00214DD8 File Offset: 0x00212FD8
		public void OnPiecePlacementDeserialized()
		{
			if (this.alwaysFaceUp != null)
			{
				Quaternion quaternion;
				Quaternion quaternion2;
				QuaternionUtil.DecomposeSwingTwist(this.alwaysFaceUp.parent.rotation, Vector3.up, out quaternion, out quaternion2);
				this.alwaysFaceUp.rotation = quaternion2;
			}
		}

		// Token: 0x06006775 RID: 26485 RVA: 0x00214E20 File Offset: 0x00213020
		public void OnPieceActivate()
		{
			if (this.alwaysFaceUp != null)
			{
				Quaternion quaternion;
				Quaternion quaternion2;
				QuaternionUtil.DecomposeSwingTwist(this.alwaysFaceUp.parent.rotation, Vector3.up, out quaternion, out quaternion2);
				this.alwaysFaceUp.rotation = quaternion2;
			}
		}

		// Token: 0x06006776 RID: 26486 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPieceDeactivate()
		{
		}

		// Token: 0x0400768D RID: 30349
		[SerializeField]
		private Transform alwaysFaceUp;
	}
}
