using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Voxels
{
	// Token: 0x0200137D RID: 4989
	[Serializable]
	public class Chunk : IDisposable
	{
		// Token: 0x17000C2D RID: 3117
		// (get) Token: 0x06007CDD RID: 31965 RVA: 0x0028D6D6 File Offset: 0x0028B8D6
		// (set) Token: 0x06007CDE RID: 31966 RVA: 0x0028D6DE File Offset: 0x0028B8DE
		public ChunkComponent Component { get; private set; }

		// Token: 0x17000C2E RID: 3118
		// (get) Token: 0x06007CDF RID: 31967 RVA: 0x0028D6E7 File Offset: 0x0028B8E7
		// (set) Token: 0x06007CE0 RID: 31968 RVA: 0x0028D6EF File Offset: 0x0028B8EF
		public GameObject GameObject { get; private set; }

		// Token: 0x17000C2F RID: 3119
		// (get) Token: 0x06007CE1 RID: 31969 RVA: 0x0028D6F8 File Offset: 0x0028B8F8
		// (set) Token: 0x06007CE2 RID: 31970 RVA: 0x0028D700 File Offset: 0x0028B900
		public MeshFilter MeshFilter { get; private set; }

		// Token: 0x17000C30 RID: 3120
		// (get) Token: 0x06007CE3 RID: 31971 RVA: 0x0028D709 File Offset: 0x0028B909
		// (set) Token: 0x06007CE4 RID: 31972 RVA: 0x0028D711 File Offset: 0x0028B911
		public MeshRenderer MeshRenderer { get; private set; }

		// Token: 0x17000C31 RID: 3121
		// (get) Token: 0x06007CE5 RID: 31973 RVA: 0x0028D71A File Offset: 0x0028B91A
		// (set) Token: 0x06007CE6 RID: 31974 RVA: 0x0028D722 File Offset: 0x0028B922
		public MeshCollider MeshCollider { get; private set; }

		// Token: 0x17000C32 RID: 3122
		// (get) Token: 0x06007CE7 RID: 31975 RVA: 0x0028D72B File Offset: 0x0028B92B
		// (set) Token: 0x06007CE8 RID: 31976 RVA: 0x0028D732 File Offset: 0x0028B932
		public static int3 DefaultSize { get; set; } = 32;

		// Token: 0x17000C33 RID: 3123
		// (get) Token: 0x06007CE9 RID: 31977 RVA: 0x0028D73A File Offset: 0x0028B93A
		// (set) Token: 0x06007CEA RID: 31978 RVA: 0x0028D741 File Offset: 0x0028B941
		public static int Pad { get; set; } = 1;

		// Token: 0x17000C34 RID: 3124
		// (get) Token: 0x06007CEB RID: 31979 RVA: 0x0028D749 File Offset: 0x0028B949
		public ChunkState State
		{
			get
			{
				if (!this.IsDataGenerated)
				{
					return ChunkState.Created;
				}
				if (!this.IsMeshGenerated)
				{
					return ChunkState.VoxelDataGenerated;
				}
				if (!this.IsMeshCreated)
				{
					return ChunkState.MeshDataGenerated;
				}
				if (!this.IsCollisionBaked)
				{
					return ChunkState.MeshCreated;
				}
				if (!this.IsMeshAssigned)
				{
					return ChunkState.CollisionBaked;
				}
				return ChunkState.MeshAssigned;
			}
		}

		// Token: 0x06007CEC RID: 31980 RVA: 0x0028D780 File Offset: 0x0028B980
		public Chunk(ChunkDTO dto)
		{
			this.Id = dto.Id;
			this.Size = dto.Size;
			this.Dimensions = dto.Dimensions;
			this.VoxelCount = this.Dimensions.x * this.Dimensions.y * this.Dimensions.z;
			this.Density = dto.Density;
			this.Material = dto.Material;
			this.VertexData = default(NativeArray<MeshVertexData>);
			this.TriangleData = default(NativeArray<ushort>);
			this.GenericMeshData = null;
		}

		// Token: 0x06007CED RID: 31981 RVA: 0x0028D820 File Offset: 0x0028BA20
		public Chunk(int3 id, int3 size, int padding = -1)
		{
			if (padding < 0)
			{
				padding = Chunk.Pad;
			}
			this.Id = id;
			this.Size = size;
			this.Dimensions = size + padding;
			this.VoxelCount = this.Dimensions.x * this.Dimensions.y * this.Dimensions.z;
			this.Density = default(NativeArray<byte>);
			this.Material = default(NativeArray<byte>);
			this.VertexData = default(NativeArray<MeshVertexData>);
			this.TriangleData = default(NativeArray<ushort>);
			this.GenericMeshData = null;
		}

		// Token: 0x06007CEE RID: 31982 RVA: 0x0028D8C0 File Offset: 0x0028BAC0
		public void SetFrom(ChunkDTO dto)
		{
			this.Id = dto.Id;
			this.Size = dto.Size;
			this.Dimensions = dto.Dimensions;
			this.VoxelCount = this.Dimensions.x * this.Dimensions.y * this.Dimensions.z;
			this.Dispose();
			this.Density = dto.Density;
			this.Material = dto.Material;
			this.IsDataGenerated = true;
			this.IsDataChanged = true;
			this.IsMeshGenerated = false;
			this.IsMeshCreated = false;
			this.IsCollisionBaked = false;
			this.IsMeshAssigned = false;
			this.IsDirty = true;
			this.VertexCount = 0;
			this.Mesh = null;
		}

		// Token: 0x06007CEF RID: 31983 RVA: 0x0028D978 File Offset: 0x0028BB78
		public void UpdateFrom(ChunkDTO dto)
		{
			this.Id = dto.Id;
			this.Size = dto.Size;
			this.Dimensions = dto.Dimensions;
			this.VoxelCount = this.Dimensions.x * this.Dimensions.y * this.Dimensions.z;
			this.DisposeAllExceptComponent();
			this.Density = dto.Density;
			this.Material = dto.Material;
			this.IsDataGenerated = true;
			this.IsDataChanged = true;
			this.IsMeshGenerated = false;
			this.IsMeshCreated = false;
			this.IsCollisionBaked = false;
			this.IsMeshAssigned = false;
			this.IsDirty = true;
			this.VertexCount = 0;
			this.Mesh = null;
		}

		// Token: 0x06007CF0 RID: 31984 RVA: 0x0028DA30 File Offset: 0x0028BC30
		public void Clear()
		{
			this.DisposeMeshData();
			this.IsDataGenerated = false;
			this.IsDataChanged = false;
			this.IsMeshGenerated = false;
			this.IsMeshCreated = false;
			this.IsCollisionBaked = false;
			this.IsMeshAssigned = false;
			this.IsDirty = true;
			this.VertexCount = 0;
			this.Mesh = null;
		}

		// Token: 0x06007CF1 RID: 31985 RVA: 0x0028DA84 File Offset: 0x0028BC84
		public void SetComponent(ChunkComponent chunkComponent)
		{
			this.Component = chunkComponent;
			if (chunkComponent)
			{
				this.GameObject = chunkComponent.gameObject;
				this.MeshFilter = chunkComponent.meshFilter;
				this.MeshRenderer = chunkComponent.meshRenderer;
				this.MeshCollider = chunkComponent.meshCollider;
				this.Component.name = Chunk.GetChunkName(this.Id);
				this.Component.World = this.World;
				this.Component.meshRenderer.sharedMaterial = this.World.MaterialSet.Material;
				return;
			}
			this.GameObject = null;
			this.MeshFilter = null;
			this.MeshRenderer = null;
			this.MeshCollider = null;
		}

		// Token: 0x06007CF2 RID: 31986 RVA: 0x0028DB34 File Offset: 0x0028BD34
		public void Dispose()
		{
			if (this.Density.IsCreated)
			{
				this.Density.Dispose();
				this.Density = default(NativeArray<byte>);
			}
			if (this.Material.IsCreated)
			{
				this.Material.Dispose();
				this.Material = default(NativeArray<byte>);
			}
			this.DisposeMeshData();
			if (this.Component)
			{
				Object.Destroy(this.Component.gameObject);
			}
		}

		// Token: 0x06007CF3 RID: 31987 RVA: 0x0028DBAC File Offset: 0x0028BDAC
		public void DisposeAllExceptComponent()
		{
			if (this.Density.IsCreated)
			{
				this.Density.Dispose();
				this.Density = default(NativeArray<byte>);
			}
			if (this.Material.IsCreated)
			{
				this.Material.Dispose();
				this.Material = default(NativeArray<byte>);
			}
			this.DisposeMeshData();
		}

		// Token: 0x06007CF4 RID: 31988 RVA: 0x0028DC08 File Offset: 0x0028BE08
		public void AllocateVertexData(int length)
		{
			if (this.VertexData.IsCreated)
			{
				if (this.VertexData.Length == length)
				{
					return;
				}
				NativeArrayPool<MeshVertexData>.Return(this.VertexData);
				this.VertexData = default(NativeArray<MeshVertexData>);
			}
			this.VertexData = NativeArrayPool<MeshVertexData>.Get(length);
		}

		// Token: 0x06007CF5 RID: 31989 RVA: 0x0028DC54 File Offset: 0x0028BE54
		public void AllocateTriangleData(int length)
		{
			if (this.TriangleData.IsCreated)
			{
				if (this.TriangleData.Length == length)
				{
					return;
				}
				NativeArrayPool<ushort>.Return(this.TriangleData);
				this.TriangleData = default(NativeArray<ushort>);
			}
			this.TriangleData = NativeArrayPool<ushort>.Get(length);
		}

		// Token: 0x06007CF6 RID: 31990 RVA: 0x0028DCA0 File Offset: 0x0028BEA0
		public void DisposeMeshData()
		{
			if (this.VertexData.IsCreated)
			{
				NativeArrayPool<MeshVertexData>.Return(this.VertexData);
				this.VertexData = default(NativeArray<MeshVertexData>);
			}
			if (this.TriangleData.IsCreated)
			{
				NativeArrayPool<ushort>.Return(this.TriangleData);
				this.TriangleData = default(NativeArray<ushort>);
			}
			IDisposable disposable = this.GenericMeshData as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
				this.GenericMeshData = null;
			}
		}

		// Token: 0x06007CF7 RID: 31991 RVA: 0x0028DD11 File Offset: 0x0028BF11
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3 GetLocalPosition(int3 voxelPosition)
		{
			return voxelPosition - this.Id * this.Size;
		}

		// Token: 0x06007CF8 RID: 31992 RVA: 0x0028DD2C File Offset: 0x0028BF2C
		public override string ToString()
		{
			return string.Format("Chunk ({0}, {1}, {2}) [{3}{4}{5}{6}{7}{8}]", new object[]
			{
				this.Id.x,
				this.Id.y,
				this.Id.z,
				this.IsDataGenerated ? "D" : "_",
				this.IsDataChanged ? "C" : "_",
				this.IsMeshGenerated ? "G" : "_",
				this.IsMeshCreated ? "M" : "_",
				this.IsCollisionBaked ? "B" : "_",
				this.IsMeshAssigned ? "A" : "_"
			});
		}

		// Token: 0x06007CF9 RID: 31993 RVA: 0x0028DE0D File Offset: 0x0028C00D
		public static string GetChunkName(int3 id)
		{
			return string.Format("Chunk_{0}_{1}_{2}", id.x, id.y, id.z);
		}

		// Token: 0x04008FB7 RID: 36791
		public VoxelWorld World;

		// Token: 0x04008FB8 RID: 36792
		public int3 Id;

		// Token: 0x04008FB9 RID: 36793
		public int3 Size;

		// Token: 0x04008FBA RID: 36794
		public int3 Dimensions;

		// Token: 0x04008FBB RID: 36795
		public int VoxelCount;

		// Token: 0x04008FBC RID: 36796
		public NativeArray<byte> Density;

		// Token: 0x04008FBD RID: 36797
		public NativeArray<byte> Material;

		// Token: 0x04008FBE RID: 36798
		public NativeArray<MeshVertexData> VertexData;

		// Token: 0x04008FBF RID: 36799
		public NativeArray<ushort> TriangleData;

		// Token: 0x04008FC0 RID: 36800
		public object GenericMeshData;

		// Token: 0x04008FC1 RID: 36801
		public bool IsDataGenerated;

		// Token: 0x04008FC2 RID: 36802
		public bool IsDataChanged;

		// Token: 0x04008FC3 RID: 36803
		public bool IsMeshGenerated;

		// Token: 0x04008FC4 RID: 36804
		public bool IsMeshCreated;

		// Token: 0x04008FC5 RID: 36805
		public bool IsCollisionBaked;

		// Token: 0x04008FC6 RID: 36806
		public bool IsMeshAssigned;

		// Token: 0x04008FC7 RID: 36807
		public bool IsDirty = true;

		// Token: 0x04008FC8 RID: 36808
		public int VertexCount;

		// Token: 0x04008FC9 RID: 36809
		public Mesh Mesh;
	}
}
