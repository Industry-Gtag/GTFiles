using System;
using BoingKit;
using FastSurfaceNets;
using GorillaLocomotion.Gameplay;
using GorillaTag.Rendering;
using GorillaTagScripts;
using GorillaTagScripts.Builder;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Voxels;

// Token: 0x020014AC RID: 5292
[DOTSCompilerGenerated]
internal class __JobReflectionRegistrationOutput__1221673671587648887
{
	// Token: 0x06008422 RID: 33826 RVA: 0x002B21D0 File Offset: 0x002B03D0
	public static void CreateJobReflectionData()
	{
		try
		{
			IJobParallelForExtensions.EarlyJobInit<MeshExtensions.FaceNormalJob>();
			IJobExtensions.EarlyJobInit<MeshExtensions.SplitJob>();
			IJobParallelForExtensions.EarlyJobInit<MeshExtensions.TriNormalJob>();
			IJobParallelForExtensions.EarlyJobInit<MeshExtensions.BuildAdjJob>();
			IJobParallelForExtensions.EarlyJobInit<MeshExtensions.VertexNormalJob>();
			IJobParallelForExtensions.EarlyJobInit<HandEffectsTriggerRegistry.HandEffectsJob>();
			IJobParallelForTransformExtensions.EarlyJobInit<BuilderRenderer.SetupInstanceDataForMesh>();
			IJobParallelForTransformExtensions.EarlyJobInit<BuilderRenderer.SetupInstanceDataForMeshStatic>();
			IJobParallelForExtensions.EarlyJobInit<GorillaIKMgr.IKJob>();
			IJobParallelForTransformExtensions.EarlyJobInit<GorillaIKMgr.IKTransformJob>();
			IJobExtensions.EarlyJobInit<DayNightCycle.LerpBakedLightingJob>();
			IJobParallelForTransformExtensions.EarlyJobInit<VRRigJobManager.VRRigTransformJob>();
			IJobParallelForExtensions.EarlyJobInit<BuilderFindPotentialSnaps>();
			IJobParallelForTransformExtensions.EarlyJobInit<FindNearbyPiecesJob>();
			IJobParallelForTransformExtensions.EarlyJobInit<BuilderConveyorManager.EvaluateSplineJob>();
			IJobExtensions.EarlyJobInit<SolveRopeJob>();
			IJobExtensions.EarlyJobInit<VectorizedSolveRopeJob>();
			IJobExtensions.EarlyJobInit<EdMeshCombinerPrefab.CopyMeshJob>();
			IJobParallelForExtensions.EarlyJobInit<FillChunkJob>();
			IJobExtensions.EarlyJobInit<CollisionJob>();
			IJobExtensions.EarlyJobInit<MarchingCubesMeshingJob>();
			IJobParallelForExtensions.EarlyJobInit<PerlinVoxelGenerator.VoxelDataJob>();
			IJobParallelForExtensions.EarlyJobInit<SDFVoxelGenerator.VoxelDataJob>();
			IJobExtensions.EarlyJobInit<SortChunksJob>();
			IJobParallelForExtensions.EarlyJobInit<MeshUtilities.FaceNormalJob>();
			IJobExtensions.EarlyJobInit<MeshUtilities.SplitJob>();
			IJobExtensions.EarlyJobInit<MeshUtilities.SplitVoxelMeshJob>();
			IJobParallelForExtensions.EarlyJobInit<MeshUtilities.TriNormalJob>();
			IJobParallelForExtensions.EarlyJobInit<MeshUtilities.BuildAdjJob>();
			IJobParallelForExtensions.EarlyJobInit<MeshUtilities.VertexNormalJob>();
			IJobExtensions.EarlyJobInit<SurfaceNetsJob>();
			IJobExtensions.EarlyJobInit<AssembleVertexDataJob>();
			IJobParallelForExtensions.EarlyJobInit<BoingWorkAsynchronous.BehaviorJob>();
			IJobParallelForExtensions.EarlyJobInit<BoingWorkAsynchronous.ReactorJob>();
		}
		catch (Exception ex)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex);
		}
	}

	// Token: 0x06008423 RID: 33827 RVA: 0x002B22AC File Offset: 0x002B04AC
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	public static void EarlyInit()
	{
		__JobReflectionRegistrationOutput__1221673671587648887.CreateJobReflectionData();
	}
}
