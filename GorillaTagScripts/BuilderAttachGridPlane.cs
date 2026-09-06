using System;
using GorillaTagScripts.Builder;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F55 RID: 3925
	public class BuilderAttachGridPlane : MonoBehaviour
	{
		// Token: 0x06006073 RID: 24691 RVA: 0x001E9145 File Offset: 0x001E7345
		private void Awake()
		{
			if (this.center == null)
			{
				this.center = base.transform;
			}
		}

		// Token: 0x06006074 RID: 24692 RVA: 0x001E9164 File Offset: 0x001E7364
		public void Setup(BuilderPiece piece, int attachIndex, float gridSize)
		{
			this.piece = piece;
			this.attachIndex = attachIndex;
			this.pieceToGridPosition = piece.transform.InverseTransformPoint(base.transform.position);
			this.pieceToGridRotation = Quaternion.Inverse(piece.transform.rotation) * base.transform.rotation;
			float num = (float)(this.width + 2) * gridSize;
			float num2 = (float)(this.length + 2) * gridSize;
			this.boundingRadius = Mathf.Sqrt(num * num + num2 * num2);
			this.connected = new bool[this.width * this.length];
			this.widthOffset = ((this.width % 2 == 0) ? (gridSize / 2f) : 0f);
			this.lengthOffset = ((this.length % 2 == 0) ? (gridSize / 2f) : 0f);
			this.gridPlaneDataIndex = -1;
			this.childPieceCount = 0;
		}

		// Token: 0x06006075 RID: 24693 RVA: 0x001E9250 File Offset: 0x001E7450
		public void OnReturnToPool(BuilderPool pool)
		{
			SnapOverlap nextOverlap = this.firstOverlap;
			while (nextOverlap != null)
			{
				SnapOverlap snapOverlap = nextOverlap;
				nextOverlap = nextOverlap.nextOverlap;
				if (snapOverlap.otherPlane != null)
				{
					snapOverlap.otherPlane.RemoveSnapsWithPiece(this.piece, pool);
				}
				this.SetConnected(snapOverlap.bounds, false);
				pool.DestroySnapOverlap(snapOverlap);
			}
			this.firstOverlap = null;
			int num = this.width * this.length;
			for (int i = 0; i < num; i++)
			{
				this.connected[i] = false;
			}
			this.childPieceCount = 0;
		}

		// Token: 0x06006076 RID: 24694 RVA: 0x001E92D8 File Offset: 0x001E74D8
		public Vector3 GetGridPosition(int x, int z, float gridSize)
		{
			float num = ((this.width % 2 == 0) ? (gridSize / 2f) : 0f);
			float num2 = ((this.length % 2 == 0) ? (gridSize / 2f) : 0f);
			return this.center.position + this.center.rotation * new Vector3((float)x * gridSize - num, (this.male ? 0.002f : (-0.002f)) * gridSize, (float)z * gridSize - num2);
		}

		// Token: 0x06006077 RID: 24695 RVA: 0x001E935E File Offset: 0x001E755E
		public int GetChildCount()
		{
			return this.childPieceCount;
		}

		// Token: 0x06006078 RID: 24696 RVA: 0x001E9368 File Offset: 0x001E7568
		public void ChangeChildPieceCount(int delta)
		{
			this.childPieceCount += delta;
			if (this.piece.parentPiece == null)
			{
				return;
			}
			if (this.piece.parentAttachIndex < 0 || this.piece.parentAttachIndex >= this.piece.parentPiece.gridPlanes.Count)
			{
				return;
			}
			this.piece.parentPiece.gridPlanes[this.piece.parentAttachIndex].ChangeChildPieceCount(delta);
		}

		// Token: 0x06006079 RID: 24697 RVA: 0x001E93EE File Offset: 0x001E75EE
		public void AddSnapOverlap(SnapOverlap newOverlap)
		{
			if (this.firstOverlap == null)
			{
				this.firstOverlap = newOverlap;
			}
			else
			{
				newOverlap.nextOverlap = this.firstOverlap;
				this.firstOverlap = newOverlap;
			}
			this.SetConnected(newOverlap.bounds, true);
		}

		// Token: 0x0600607A RID: 24698 RVA: 0x001E9424 File Offset: 0x001E7624
		public void RemoveSnapsWithDifferentRoot(BuilderPiece root, BuilderPool pool)
		{
			if (this.firstOverlap == null)
			{
				return;
			}
			if (pool == null)
			{
				return;
			}
			SnapOverlap snapOverlap = null;
			SnapOverlap snapOverlap2 = this.firstOverlap;
			while (snapOverlap2 != null)
			{
				if (snapOverlap2.otherPlane == null || snapOverlap2.otherPlane.piece == null)
				{
					SnapOverlap snapOverlap3 = snapOverlap2;
					if (snapOverlap == null)
					{
						this.firstOverlap = snapOverlap2.nextOverlap;
						snapOverlap2 = this.firstOverlap;
					}
					else
					{
						snapOverlap.nextOverlap = snapOverlap2.nextOverlap;
						snapOverlap2 = snapOverlap.nextOverlap;
					}
					this.SetConnected(snapOverlap3.bounds, false);
					pool.DestroySnapOverlap(snapOverlap3);
				}
				else if (root == null || snapOverlap2.otherPlane.piece.GetRootPiece() != root)
				{
					SnapOverlap snapOverlap4 = snapOverlap2;
					if (snapOverlap == null)
					{
						this.firstOverlap = snapOverlap2.nextOverlap;
						snapOverlap2 = this.firstOverlap;
					}
					else
					{
						snapOverlap.nextOverlap = snapOverlap2.nextOverlap;
						snapOverlap2 = snapOverlap.nextOverlap;
					}
					this.SetConnected(snapOverlap4.bounds, false);
					snapOverlap4.otherPlane.RemoveSnapsWithPiece(this.piece, pool);
					pool.DestroySnapOverlap(snapOverlap4);
				}
				else
				{
					snapOverlap = snapOverlap2;
					snapOverlap2 = snapOverlap2.nextOverlap;
				}
			}
		}

		// Token: 0x0600607B RID: 24699 RVA: 0x001E953C File Offset: 0x001E773C
		public void RemoveSnapsWithPiece(BuilderPiece piece, BuilderPool pool)
		{
			if (this.firstOverlap == null)
			{
				return;
			}
			if (piece == null || pool == null)
			{
				return;
			}
			SnapOverlap snapOverlap = null;
			SnapOverlap snapOverlap2 = this.firstOverlap;
			while (snapOverlap2 != null)
			{
				if (snapOverlap2.otherPlane == null || snapOverlap2.otherPlane.piece == null)
				{
					SnapOverlap snapOverlap3 = snapOverlap2;
					if (snapOverlap == null)
					{
						this.firstOverlap = snapOverlap2.nextOverlap;
						snapOverlap2 = this.firstOverlap;
					}
					else
					{
						snapOverlap.nextOverlap = snapOverlap2.nextOverlap;
						snapOverlap2 = snapOverlap.nextOverlap;
					}
					this.SetConnected(snapOverlap3.bounds, false);
					pool.DestroySnapOverlap(snapOverlap3);
				}
				else if (snapOverlap2.otherPlane.piece == piece)
				{
					SnapOverlap snapOverlap4 = snapOverlap2;
					if (snapOverlap == null)
					{
						this.firstOverlap = snapOverlap2.nextOverlap;
						snapOverlap2 = this.firstOverlap;
					}
					else
					{
						snapOverlap.nextOverlap = snapOverlap2.nextOverlap;
						snapOverlap2 = snapOverlap.nextOverlap;
					}
					this.SetConnected(snapOverlap4.bounds, false);
					pool.DestroySnapOverlap(snapOverlap4);
				}
				else
				{
					snapOverlap = snapOverlap2;
					snapOverlap2 = snapOverlap2.nextOverlap;
				}
			}
		}

		// Token: 0x0600607C RID: 24700 RVA: 0x001E963C File Offset: 0x001E783C
		private void SetConnected(SnapBounds bounds, bool connect)
		{
			int num = this.width / 2 - ((this.width % 2 == 0) ? 1 : 0);
			int num2 = this.length / 2 - ((this.length % 2 == 0) ? 1 : 0);
			int num3 = this.connected.Length;
			for (int i = bounds.min.x; i <= bounds.max.x; i++)
			{
				for (int j = bounds.min.y; j <= bounds.max.y; j++)
				{
					int num4 = (num + i) * this.length + (j + num2);
					if (num4 >= num3 || num4 < 0)
					{
						if (this.piece != null)
						{
							int pieceId = this.piece.pieceId;
						}
						return;
					}
					this.connected[num4] = connect;
				}
			}
		}

		// Token: 0x0600607D RID: 24701 RVA: 0x001E970C File Offset: 0x001E790C
		public bool IsConnected(SnapBounds bounds)
		{
			int num = this.width / 2 - ((this.width % 2 == 0) ? 1 : 0);
			int num2 = this.length / 2 - ((this.length % 2 == 0) ? 1 : 0);
			int num3 = this.connected.Length;
			for (int i = bounds.min.x; i <= bounds.max.x; i++)
			{
				for (int j = bounds.min.y; j <= bounds.max.y; j++)
				{
					int num4 = (num + i) * this.length + (j + num2);
					if (num4 < 0 || num4 >= num3)
					{
						if (this.piece != null)
						{
							int pieceId = this.piece.pieceId;
						}
						return false;
					}
					if (this.connected[num4])
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600607E RID: 24702 RVA: 0x001E97E0 File Offset: 0x001E79E0
		public void CalcGridOverlap(BuilderAttachGridPlane otherGridPlane, Vector3 otherPieceLocalPos, Quaternion otherPieceLocalRot, float gridSize, out Vector2Int min, out Vector2Int max)
		{
			int num = otherGridPlane.width;
			int num2 = otherGridPlane.length;
			Quaternion quaternion = otherPieceLocalRot * otherGridPlane.pieceToGridRotation;
			Vector3 lossyScale = base.transform.lossyScale;
			otherPieceLocalPos.Scale(base.transform.lossyScale);
			Vector3 vector = otherPieceLocalPos + otherPieceLocalRot * otherGridPlane.pieceToGridPosition;
			if (Mathf.Abs(Vector3.Dot(quaternion * Vector3.forward, Vector3.forward)) < 0.707f)
			{
				num = otherGridPlane.length;
				num2 = otherGridPlane.width;
			}
			float num3 = ((num % 2 == 0) ? (gridSize / 2f) : 0f);
			float num4 = ((num2 % 2 == 0) ? (gridSize / 2f) : 0f);
			float num5 = ((this.width % 2 == 0) ? (gridSize / 2f) : 0f);
			float num6 = ((this.length % 2 == 0) ? (gridSize / 2f) : 0f);
			float num7 = num3 - num5;
			float num8 = num4 - num6;
			int num9 = Mathf.RoundToInt((vector.x - num7) / gridSize);
			int num10 = Mathf.RoundToInt((vector.z - num8) / gridSize);
			int num11 = num9 + Mathf.FloorToInt((float)num / 2f);
			int num12 = num10 + Mathf.FloorToInt((float)num2 / 2f);
			int num13 = num11 - (num - 1);
			int num14 = num12 - (num2 - 1);
			int num15 = Mathf.FloorToInt((float)this.width / 2f);
			int num16 = Mathf.FloorToInt((float)this.length / 2f);
			int num17 = num15 - (this.width - 1);
			int num18 = num16 - (this.length - 1);
			min = new Vector2Int(Mathf.Max(num13, num17), Mathf.Max(num14, num18));
			max = new Vector2Int(Mathf.Min(num11, num15), Mathf.Min(num12, num16));
		}

		// Token: 0x0600607F RID: 24703 RVA: 0x001E99A4 File Offset: 0x001E7BA4
		public bool IsAttachedToMovingGrid()
		{
			return this.piece.state == BuilderPiece.State.AttachedAndPlaced && !this.piece.isBuiltIntoTable && (this.isMoving || (!(this.piece.parentPiece == null) && this.piece.parentAttachIndex >= 0 && this.piece.parentAttachIndex < this.piece.parentPiece.gridPlanes.Count && this.piece.parentPiece.gridPlanes[this.piece.parentAttachIndex].IsAttachedToMovingGrid()));
		}

		// Token: 0x06006080 RID: 24704 RVA: 0x001E9A48 File Offset: 0x001E7C48
		public BuilderAttachGridPlane GetMovingParentGrid()
		{
			if (this.piece.isBuiltIntoTable)
			{
				return null;
			}
			if (this.movesOnPlace && this.movingPart != null && !this.movingPart.IsAnchoredToTable())
			{
				return this;
			}
			if (this.piece.parentPiece == null)
			{
				return null;
			}
			if (this.piece.parentAttachIndex < 0 || this.piece.parentAttachIndex >= this.piece.parentPiece.gridPlanes.Count)
			{
				return null;
			}
			return this.piece.parentPiece.gridPlanes[this.piece.parentAttachIndex].GetMovingParentGrid();
		}

		// Token: 0x04006F0B RID: 28427
		[Tooltip("Are the snap points in this grid \"outies\"")]
		public bool male;

		// Token: 0x04006F0C RID: 28428
		[Tooltip("(Optional) midpoint of the grid")]
		public Transform center;

		// Token: 0x04006F0D RID: 28429
		[Tooltip("number of snap points wide (local X-axis)")]
		public int width;

		// Token: 0x04006F0E RID: 28430
		[Tooltip("number of snap points long (local z-axis)")]
		public int length;

		// Token: 0x04006F0F RID: 28431
		[NonSerialized]
		public int gridPlaneDataIndex;

		// Token: 0x04006F10 RID: 28432
		[NonSerialized]
		public BuilderItem item;

		// Token: 0x04006F11 RID: 28433
		[NonSerialized]
		public BuilderPiece piece;

		// Token: 0x04006F12 RID: 28434
		[NonSerialized]
		public int attachIndex;

		// Token: 0x04006F13 RID: 28435
		[NonSerialized]
		public float boundingRadius;

		// Token: 0x04006F14 RID: 28436
		[NonSerialized]
		public Vector3 pieceToGridPosition;

		// Token: 0x04006F15 RID: 28437
		[NonSerialized]
		public Quaternion pieceToGridRotation;

		// Token: 0x04006F16 RID: 28438
		[NonSerialized]
		public bool[] connected;

		// Token: 0x04006F17 RID: 28439
		[NonSerialized]
		public SnapOverlap firstOverlap;

		// Token: 0x04006F18 RID: 28440
		[NonSerialized]
		public float widthOffset;

		// Token: 0x04006F19 RID: 28441
		[NonSerialized]
		public float lengthOffset;

		// Token: 0x04006F1A RID: 28442
		private int childPieceCount;

		// Token: 0x04006F1B RID: 28443
		[HideInInspector]
		public bool isMoving;

		// Token: 0x04006F1C RID: 28444
		[HideInInspector]
		public bool movesOnPlace;

		// Token: 0x04006F1D RID: 28445
		[HideInInspector]
		public BuilderMovingPart movingPart;
	}
}
