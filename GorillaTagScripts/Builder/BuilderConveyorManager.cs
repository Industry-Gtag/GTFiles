using System;
using Photon.Pun;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Splines;

namespace GorillaTagScripts.Builder
{
	// Token: 0x0200102B RID: 4139
	public class BuilderConveyorManager : MonoBehaviour
	{
		// Token: 0x170009DD RID: 2525
		// (get) Token: 0x060066F0 RID: 26352 RVA: 0x00210EE0 File Offset: 0x0020F0E0
		// (set) Token: 0x060066F1 RID: 26353 RVA: 0x00210EE7 File Offset: 0x0020F0E7
		public static BuilderConveyorManager instance { get; private set; }

		// Token: 0x060066F2 RID: 26354 RVA: 0x00210EEF File Offset: 0x0020F0EF
		private void Awake()
		{
			if (BuilderConveyorManager.instance != null && BuilderConveyorManager.instance != this)
			{
				Object.Destroy(this);
			}
			if (BuilderConveyorManager.instance == null)
			{
				BuilderConveyorManager.instance = this;
			}
		}

		// Token: 0x060066F3 RID: 26355 RVA: 0x00210F24 File Offset: 0x0020F124
		public void UpdateManager()
		{
			foreach (BuilderConveyor builderConveyor in this.table.conveyors)
			{
				builderConveyor.UpdateConveyor();
			}
			bool flag = false;
			bool flag2 = this.pieceTransforms.length >= this.pieceTransforms.capacity - 5;
			for (int i = this.jobSplineTimes.Length - 1; i >= 0; i--)
			{
				BuilderConveyor builderConveyor2 = this.table.conveyors[this.conveyorIndices[i]];
				float num = Time.deltaTime * builderConveyor2.GetFrameMovement();
				float num2 = this.jobSplineTimes[i] + num;
				this.jobSplineTimes[i] = Mathf.Clamp(num2, 0f, 1f);
				if (PhotonNetwork.IsMasterClient && (!flag || flag2) && (double)num2 > 0.999)
				{
					builderConveyor2.RemovePieceFromConveyor(this.pieceTransforms[i]);
					this.RemovePieceFromJobAtIndex(i);
					flag = true;
				}
			}
			for (int j = this.shelfSlice; j < this.table.conveyors.Count; j += BuilderTable.SHELF_SLICE_BUCKETS)
			{
				this.table.conveyors[j].UpdateShelfSliced();
			}
			this.shelfSlice = (this.shelfSlice + 1) % BuilderTable.SHELF_SLICE_BUCKETS;
		}

		// Token: 0x060066F4 RID: 26356 RVA: 0x002110A0 File Offset: 0x0020F2A0
		public void Setup(BuilderTable mytable)
		{
			if (this.isSetup)
			{
				return;
			}
			this.table = mytable;
			this.conveyorSplines = new NativeArray<NativeSpline>(this.table.conveyors.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.conveyorRotations = new NativeArray<Quaternion>(this.table.conveyors.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			int num = 0;
			for (int i = 0; i < this.table.conveyors.Count; i++)
			{
				this.conveyorSplines[i] = this.table.conveyors[i].nativeSpline;
				this.conveyorRotations[i] = this.table.conveyors[i].GetSpawnTransform().rotation;
				num += this.table.conveyors[i].GetMaxItemsOnConveyor();
			}
			this.maxItemCount = num;
			this.conveyorIndices = new NativeList<int>(this.maxItemCount, Allocator.Persistent);
			this.jobSplineTimes = new NativeList<float>(this.maxItemCount, Allocator.Persistent);
			this.jobShelfOffsets = new NativeList<Vector3>(this.maxItemCount, Allocator.Persistent);
			this.pieceTransforms = new TransformAccessArray(this.maxItemCount, 3);
			this.isSetup = true;
		}

		// Token: 0x060066F5 RID: 26357 RVA: 0x002111DC File Offset: 0x0020F3DC
		public float GetSplineProgressForPiece(BuilderPiece piece)
		{
			for (int i = 0; i < this.pieceTransforms.length; i++)
			{
				if (this.pieceTransforms[i] == piece.transform)
				{
					return this.jobSplineTimes[i];
				}
			}
			return 1f;
		}

		// Token: 0x060066F6 RID: 26358 RVA: 0x0021122C File Offset: 0x0020F42C
		public int GetPieceCreateTimestamp(BuilderPiece piece)
		{
			for (int i = 0; i < this.pieceTransforms.length; i++)
			{
				if (this.pieceTransforms[i] == piece.transform)
				{
					BuilderConveyor builderConveyor = this.table.conveyors[this.conveyorIndices[i]];
					int num = Mathf.RoundToInt(this.jobSplineTimes[i] / builderConveyor.GetFrameMovement() * 1000f);
					return PhotonNetwork.ServerTimestamp - num;
				}
			}
			return PhotonNetwork.ServerTimestamp - 5000;
		}

		// Token: 0x060066F7 RID: 26359 RVA: 0x002112B8 File Offset: 0x0020F4B8
		public void OnClearTable()
		{
			if (!this.isSetup)
			{
				return;
			}
			foreach (BuilderConveyor builderConveyor in this.table.conveyors)
			{
				builderConveyor.OnClearTable();
			}
			for (int i = this.pieceTransforms.length - 1; i >= 0; i--)
			{
				this.pieceTransforms.RemoveAtSwapBack(i);
			}
			this.jobSplineTimes.Clear();
			this.jobShelfOffsets.Clear();
			this.conveyorIndices.Clear();
		}

		// Token: 0x060066F8 RID: 26360 RVA: 0x0021135C File Offset: 0x0020F55C
		private void OnDestroy()
		{
			this.conveyorSplines.Dispose();
			this.conveyorRotations.Dispose();
			this.conveyorIndices.Dispose();
			this.jobSplineTimes.Dispose();
			this.jobShelfOffsets.Dispose();
			this.pieceTransforms.Dispose();
		}

		// Token: 0x060066F9 RID: 26361 RVA: 0x002113AC File Offset: 0x0020F5AC
		public JobHandle ConstructJobHandle()
		{
			BuilderConveyorManager.EvaluateSplineJob evaluateSplineJob = new BuilderConveyorManager.EvaluateSplineJob
			{
				conveyorRotations = this.conveyorRotations,
				conveyorIndices = this.conveyorIndices,
				shelfOffsets = this.jobShelfOffsets,
				splineTimes = this.jobSplineTimes
			};
			for (int i = 0; i < this.conveyorSplines.Length; i++)
			{
				evaluateSplineJob.SetSplineAt(i, this.conveyorSplines[i]);
			}
			return evaluateSplineJob.Schedule(this.pieceTransforms, default(JobHandle));
		}

		// Token: 0x060066FA RID: 26362 RVA: 0x00211438 File Offset: 0x0020F638
		public void AddPieceToJob(BuilderPiece piece, float splineTime, int conveyorID)
		{
			if (this.pieceTransforms.length >= this.pieceTransforms.capacity)
			{
				Debug.LogError("Too many pieces on conveyor!");
			}
			this.pieceTransforms.Add(piece.transform);
			this.conveyorIndices.Add(in conveyorID);
			this.jobShelfOffsets.Add(in piece.desiredShelfOffset);
			this.jobSplineTimes.Add(in splineTime);
		}

		// Token: 0x060066FB RID: 26363 RVA: 0x002114A3 File Offset: 0x0020F6A3
		public void RemovePieceFromJobAtIndex(int index)
		{
			BuilderRenderer.RemoveAt(this.pieceTransforms, index);
			this.jobShelfOffsets.RemoveAt(index);
			this.jobSplineTimes.RemoveAt(index);
			this.conveyorIndices.RemoveAt(index);
		}

		// Token: 0x060066FC RID: 26364 RVA: 0x002114D8 File Offset: 0x0020F6D8
		public void RemovePieceFromJob(BuilderPiece piece)
		{
			for (int i = 0; i < this.pieceTransforms.length; i++)
			{
				if (this.pieceTransforms[i] == piece.transform)
				{
					BuilderRenderer.RemoveAt(this.pieceTransforms, i);
					this.jobShelfOffsets.RemoveAt(i);
					this.jobSplineTimes.RemoveAt(i);
					this.conveyorIndices.RemoveAt(i);
					return;
				}
			}
		}

		// Token: 0x040075D0 RID: 30160
		private NativeArray<NativeSpline> conveyorSplines;

		// Token: 0x040075D1 RID: 30161
		private NativeArray<Quaternion> conveyorRotations;

		// Token: 0x040075D2 RID: 30162
		private NativeList<int> conveyorIndices;

		// Token: 0x040075D3 RID: 30163
		private NativeList<float> jobSplineTimes;

		// Token: 0x040075D4 RID: 30164
		private NativeList<Vector3> jobShelfOffsets;

		// Token: 0x040075D5 RID: 30165
		private TransformAccessArray pieceTransforms;

		// Token: 0x040075D6 RID: 30166
		private BuilderTable table;

		// Token: 0x040075D7 RID: 30167
		private bool isSetup;

		// Token: 0x040075D8 RID: 30168
		private int maxItemCount;

		// Token: 0x040075D9 RID: 30169
		private int shelfSlice;

		// Token: 0x0200102C RID: 4140
		[BurstCompile]
		public struct EvaluateSplineJob : IJobParallelForTransform
		{
			// Token: 0x060066FE RID: 26366 RVA: 0x00211545 File Offset: 0x0020F745
			public NativeSpline GetSplineAt(int index)
			{
				switch (index)
				{
				case 0:
					return this.conveyorSpline0;
				case 1:
					return this.conveyorSpline1;
				case 2:
					return this.conveyorSpline2;
				case 3:
					return this.conveyorSpline3;
				default:
					return this.conveyorSpline0;
				}
			}

			// Token: 0x060066FF RID: 26367 RVA: 0x00211581 File Offset: 0x0020F781
			public void SetSplineAt(int index, NativeSpline s)
			{
				switch (index)
				{
				case 0:
					this.conveyorSpline0 = s;
					return;
				case 1:
					this.conveyorSpline1 = s;
					return;
				case 2:
					this.conveyorSpline2 = s;
					return;
				case 3:
					this.conveyorSpline3 = s;
					return;
				default:
					return;
				}
			}

			// Token: 0x06006700 RID: 26368 RVA: 0x002115BC File Offset: 0x0020F7BC
			public void Execute(int index, TransformAccess transform)
			{
				float num = this.splineTimes[index];
				Vector3 vector = this.shelfOffsets[index];
				int num2 = this.conveyorIndices[index];
				NativeSpline splineAt = this.GetSplineAt(num2);
				Quaternion quaternion = this.conveyorRotations[num2];
				float num3;
				Vector3 vector2 = CurveUtility.EvaluatePosition(splineAt.GetCurve(splineAt.SplineToCurveT(num, out num3)), num3) + quaternion * vector;
				transform.position = vector2;
			}

			// Token: 0x040075DB RID: 30171
			public NativeSpline conveyorSpline0;

			// Token: 0x040075DC RID: 30172
			public NativeSpline conveyorSpline1;

			// Token: 0x040075DD RID: 30173
			public NativeSpline conveyorSpline2;

			// Token: 0x040075DE RID: 30174
			public NativeSpline conveyorSpline3;

			// Token: 0x040075DF RID: 30175
			[ReadOnly]
			public NativeArray<Quaternion> conveyorRotations;

			// Token: 0x040075E0 RID: 30176
			[ReadOnly]
			public NativeList<int> conveyorIndices;

			// Token: 0x040075E1 RID: 30177
			[ReadOnly]
			public NativeList<float> splineTimes;

			// Token: 0x040075E2 RID: 30178
			[ReadOnly]
			public NativeList<Vector3> shelfOffsets;
		}
	}
}
