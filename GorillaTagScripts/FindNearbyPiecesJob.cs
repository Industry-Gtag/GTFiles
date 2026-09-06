using System;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace GorillaTagScripts
{
	// Token: 0x02000F74 RID: 3956
	[BurstCompile]
	internal struct FindNearbyPiecesJob : IJobParallelForTransform
	{
		// Token: 0x060061E9 RID: 25065 RVA: 0x001F712C File Offset: 0x001F532C
		public void Execute(int index, TransformAccess transform)
		{
			if (!transform.isValid)
			{
				return;
			}
			this.CheckGridPlane(index, this.leftPieceInHandIndex, transform, this.leftHandPos, true, this.leftHandGridPlanes);
			this.CheckGridPlane(index, this.rightPieceInHandIndex, transform, this.rightHandPos, false, this.rightHandGridPlanes);
		}

		// Token: 0x060061EA RID: 25066 RVA: 0x001F717C File Offset: 0x001F537C
		private void CheckGridPlane(int gridPlaneIndex, int handPieceIndex, TransformAccess transform, Vector3 handPos, bool isLeft, NativeList<BuilderGridPlaneData>.ParallelWriter checkGridPlanes)
		{
			if (handPieceIndex < 0)
			{
				return;
			}
			if ((transform.position - handPos).sqrMagnitude > this.distanceThreshSq)
			{
				return;
			}
			BuilderGridPlaneData builderGridPlaneData = this.gridPlaneData[gridPlaneIndex];
			int pieceIndex = builderGridPlaneData.pieceIndex;
			int rootPieceIndex = this.GetRootPieceIndex(pieceIndex);
			if (rootPieceIndex == handPieceIndex)
			{
				return;
			}
			if (!this.CanPiecesPotentiallySnap(this.localPlayerActorNumber, handPieceIndex, pieceIndex, rootPieceIndex, this.pieceData[pieceIndex].requestedParentPieceIndex, isLeft))
			{
				return;
			}
			transform.GetPositionAndRotation(out builderGridPlaneData.position, out builderGridPlaneData.rotation);
			checkGridPlanes.AddNoResize(builderGridPlaneData);
		}

		// Token: 0x060061EB RID: 25067 RVA: 0x001F7210 File Offset: 0x001F5410
		public bool CanPiecesPotentiallySnap(int localActorNumber, int pieceInHandIndex, int attachToPieceIndex, int attachToPieceRootIndex, int requestedParentPieceIndex, bool isLeft)
		{
			return this.CanPlayerAttachToRootPiece(localActorNumber, attachToPieceRootIndex, isLeft) && (requestedParentPieceIndex == -1 || pieceInHandIndex != this.GetRootPieceIndex(requestedParentPieceIndex));
		}

		// Token: 0x060061EC RID: 25068 RVA: 0x001F723C File Offset: 0x001F543C
		public bool CanPlayerAttachToRootPiece(int playerActorNumber, int attachToPieceRootIndex, bool isLeft)
		{
			BuilderPieceData builderPieceData = this.pieceData[attachToPieceRootIndex];
			if (builderPieceData.state != BuilderPiece.State.AttachedAndPlaced && builderPieceData.privatePlotIndex < 0 && builderPieceData.state != BuilderPiece.State.AttachedToArm)
			{
				return true;
			}
			int attachedBuiltInPiece = this.GetAttachedBuiltInPiece(attachToPieceRootIndex);
			if (attachedBuiltInPiece == -1)
			{
				return true;
			}
			BuilderPieceData builderPieceData2 = this.pieceData[attachedBuiltInPiece];
			if (builderPieceData2.privatePlotIndex < 0 && !builderPieceData2.isArmPiece)
			{
				return true;
			}
			if (builderPieceData2.isArmPiece)
			{
				if (builderPieceData2.heldByActorNumber == playerActorNumber)
				{
					int playerIndex = this.GetPlayerIndex(playerActorNumber);
					return playerIndex >= 0 && this.playerData[playerIndex].scale >= 1f;
				}
				return false;
			}
			else
			{
				if (builderPieceData2.privatePlotIndex < 0)
				{
					return true;
				}
				if (!this.CanPlayerAttachToPlot(builderPieceData2.privatePlotIndex, playerActorNumber))
				{
					return false;
				}
				if (!isLeft)
				{
					return this.privatePlotData[builderPieceData2.privatePlotIndex].isUnderCapacityRight;
				}
				return this.privatePlotData[builderPieceData2.privatePlotIndex].isUnderCapacityLeft;
			}
		}

		// Token: 0x060061ED RID: 25069 RVA: 0x001F732C File Offset: 0x001F552C
		public bool CanPlayerAttachToPlot(int privatePlotIndex, int actorNumber)
		{
			BuilderPrivatePlotData builderPrivatePlotData = this.privatePlotData[privatePlotIndex];
			return (builderPrivatePlotData.plotState == BuilderPiecePrivatePlot.PlotState.Occupied && builderPrivatePlotData.ownerActorNumber == actorNumber) || (builderPrivatePlotData.plotState == BuilderPiecePrivatePlot.PlotState.Vacant && this.localPlayerPlotIndex < 0);
		}

		// Token: 0x060061EE RID: 25070 RVA: 0x001F7370 File Offset: 0x001F5570
		private int GetPlayerIndex(int playerActorNumber)
		{
			for (int i = 0; i < this.playerData.Length; i++)
			{
				if (this.playerData[i].playerActorNumber == playerActorNumber)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x060061EF RID: 25071 RVA: 0x001F73AC File Offset: 0x001F55AC
		public int GetAttachedBuiltInPiece(int pieceIndex)
		{
			BuilderPieceData builderPieceData = this.pieceData[pieceIndex];
			if (builderPieceData.isBuiltIntoTable)
			{
				return pieceIndex;
			}
			if (builderPieceData.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return -1;
			}
			int num = this.GetRootPieceIndex(pieceIndex);
			int parentPieceIndex = this.pieceData[num].parentPieceIndex;
			if (parentPieceIndex != -1)
			{
				num = parentPieceIndex;
			}
			if (this.pieceData[num].isBuiltIntoTable)
			{
				return num;
			}
			return -1;
		}

		// Token: 0x060061F0 RID: 25072 RVA: 0x001F7410 File Offset: 0x001F5610
		private int GetRootPieceIndex(int pieceIndex)
		{
			int num = pieceIndex;
			while (num != -1 && this.pieceData[num].parentPieceIndex != -1 && !this.pieceData[this.pieceData[num].parentPieceIndex].isBuiltIntoTable)
			{
				num = this.pieceData[num].parentPieceIndex;
			}
			return num;
		}

		// Token: 0x040070B8 RID: 28856
		[ReadOnly]
		public float distanceThreshSq;

		// Token: 0x040070B9 RID: 28857
		[ReadOnly]
		public Vector3 leftHandPos;

		// Token: 0x040070BA RID: 28858
		[ReadOnly]
		public int leftPieceInHandIndex;

		// Token: 0x040070BB RID: 28859
		[ReadOnly]
		public Vector3 rightHandPos;

		// Token: 0x040070BC RID: 28860
		[ReadOnly]
		public int rightPieceInHandIndex;

		// Token: 0x040070BD RID: 28861
		[ReadOnly]
		public int localPlayerPlotIndex;

		// Token: 0x040070BE RID: 28862
		[ReadOnly]
		public int localPlayerActorNumber;

		// Token: 0x040070BF RID: 28863
		[ReadOnly]
		public NativeArray<BuilderPieceData> pieceData;

		// Token: 0x040070C0 RID: 28864
		[ReadOnly]
		public NativeArray<BuilderGridPlaneData> gridPlaneData;

		// Token: 0x040070C1 RID: 28865
		[ReadOnly]
		public NativeArray<BuilderPrivatePlotData> privatePlotData;

		// Token: 0x040070C2 RID: 28866
		[ReadOnly]
		public NativeArray<BuilderPlayerData> playerData;

		// Token: 0x040070C3 RID: 28867
		public NativeList<BuilderGridPlaneData>.ParallelWriter leftHandGridPlanes;

		// Token: 0x040070C4 RID: 28868
		public NativeList<BuilderGridPlaneData>.ParallelWriter rightHandGridPlanes;
	}
}
