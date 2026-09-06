using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200037C RID: 892
public static class IndirectMeshRenderer
{
	// Token: 0x060015DB RID: 5595 RVA: 0x00073758 File Offset: 0x00071958
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void _Init()
	{
		IndirectMeshRenderer._DisposeAll();
		IndirectMeshRenderer._shader = Shader.Find("GorillaTag/IndirectLit");
		if (IndirectMeshRenderer._shader == null)
		{
			Debug.LogError("[IndirectMeshRenderer] Shader 'GorillaTag/IndirectLit' not found. Add it to Always Included Shaders.");
		}
		IndirectMeshRenderer._shaderEmissive = Shader.Find("GorillaTag/IndirectLitEmissive");
		if (IndirectMeshRenderer._shaderEmissive == null)
		{
			Debug.LogError("[IndirectMeshRenderer] Shader 'GorillaTag/IndirectLitEmissive' not found. Add it to Always Included Shaders.");
		}
		Application.quitting += IndirectMeshRenderer._DisposeAll;
		TickSystem<object>.AddPostTickCallback(new IndirectMeshRenderer.PostTickCallback());
	}

	// Token: 0x060015DC RID: 5596 RVA: 0x000737D4 File Offset: 0x000719D4
	public static void Register(IndirectMeshInstance inst, int groupId = 0)
	{
		Mesh sharedMesh = inst.meshFilter.sharedMesh;
		if (sharedMesh.subMeshCount > 1)
		{
			Debug.LogError(string.Format("[IndirectMeshRenderer] Mesh '{0}' on '{1}' has {2} submeshes ", sharedMesh.name, inst.name, sharedMesh.subMeshCount) + "(likely from static batching). Disable Static on objects with IndirectMeshInstance.", inst);
			return;
		}
		Material sharedMaterial = inst.meshRenderer.sharedMaterial;
		Texture texture = (sharedMaterial.HasTexture(ShaderProps._BaseMap) ? sharedMaterial.GetTexture(ShaderProps._BaseMap) : null);
		bool flag = sharedMaterial.IsKeywordEnabled("_EMISSION");
		Shader shader = (flag ? IndirectMeshRenderer._shaderEmissive : IndirectMeshRenderer._shader);
		IndirectMeshRenderer.BatchKey batchKey = new IndirectMeshRenderer.BatchKey
		{
			meshId = sharedMesh.GetInstanceID(),
			textureId = ((texture != null) ? texture.GetInstanceID() : 0),
			shaderId = shader.GetInstanceID()
		};
		int count;
		if (!IndirectMeshRenderer._batchLookup.TryGetValue(batchKey, out count))
		{
			IndirectMeshRenderer.DrawBatch drawBatch = new IndirectMeshRenderer.DrawBatch
			{
				mesh = sharedMesh,
				submeshCount = sharedMesh.subMeshCount,
				layer = inst.gameObject.layer,
				matrices = new NativeList<Matrix4x4>(2048, Allocator.Persistent),
				groupIds = new NativeList<int>(2048, Allocator.Persistent),
				visibility = new NativeList<byte>(2048, Allocator.Persistent),
				material = new Material(shader)
				{
					name = sharedMaterial.name + " (Indirect)"
				}
			};
			if (texture != null)
			{
				drawBatch.material.SetTexture(ShaderProps._BaseMap, texture);
			}
			if (sharedMaterial.HasColor(ShaderProps._BaseColor))
			{
				drawBatch.material.SetColor(ShaderProps._BaseColor, sharedMaterial.GetColor(ShaderProps._BaseColor));
			}
			if (flag)
			{
				IndirectMeshRenderer._CopyEmissionProperties(drawBatch.material, sharedMaterial);
			}
			count = IndirectMeshRenderer._batchList.Count;
			IndirectMeshRenderer._batchLookup[batchKey] = count;
			IndirectMeshRenderer._batchList.Add(drawBatch);
			Debug.Log(string.Format("[IndirectMeshRenderer] New batch #{0}: mesh='{1}' tex='{2}' shader='{3}' layer={4} submeshes={5}", new object[]
			{
				count,
				sharedMesh.name,
				(texture != null) ? texture.name : "null",
				shader.name,
				inst.gameObject.layer,
				sharedMesh.subMeshCount
			}));
		}
		IndirectMeshRenderer.DrawBatch drawBatch2 = IndirectMeshRenderer._batchList[count];
		int length = drawBatch2.matrices.Length;
		Matrix4x4 localToWorldMatrix = inst.transform.localToWorldMatrix;
		drawBatch2.matrices.Add(in localToWorldMatrix);
		drawBatch2.groupIds.Add(in groupId);
		byte b = 1;
		drawBatch2.visibility.Add(in b);
		drawBatch2.visibleCount++;
		drawBatch2.dirty = true;
		if (inst.dynamic)
		{
			ref List<IndirectMeshRenderer.DynamicEntry> ptr = ref drawBatch2.dynamicEntries;
			if (ptr == null)
			{
				ptr = new List<IndirectMeshRenderer.DynamicEntry>();
			}
			drawBatch2.dynamicEntries.Add(new IndirectMeshRenderer.DynamicEntry
			{
				transform = inst.transform,
				matrixIndex = length
			});
		}
		IndirectMeshRenderer._batchList[count] = drawBatch2;
	}

	// Token: 0x060015DD RID: 5597 RVA: 0x00073B04 File Offset: 0x00071D04
	private static void _CopyEmissionProperties(Material dst, Material src)
	{
		if (src.HasTexture(ShaderProps._EmissionMap))
		{
			dst.SetTexture(ShaderProps._EmissionMap, src.GetTexture(ShaderProps._EmissionMap));
		}
		if (src.HasColor(ShaderProps._EmissionColor))
		{
			dst.SetColor(ShaderProps._EmissionColor, src.GetColor(ShaderProps._EmissionColor));
		}
		if (src.HasVector(ShaderProps._EmissionUVScrollSpeed))
		{
			dst.SetVector(ShaderProps._EmissionUVScrollSpeed, src.GetVector(ShaderProps._EmissionUVScrollSpeed));
		}
		if (src.HasFloat(ShaderProps._EmissionDissolveEdgeSize))
		{
			dst.SetFloat(ShaderProps._EmissionDissolveEdgeSize, src.GetFloat(ShaderProps._EmissionDissolveEdgeSize));
		}
		if (src.HasFloat(ShaderProps._EmissionDissolveProgress))
		{
			dst.SetFloat(ShaderProps._EmissionDissolveProgress, src.GetFloat(ShaderProps._EmissionDissolveProgress));
		}
		if (src.HasVector(ShaderProps._EmissionDissolveAnimation))
		{
			dst.SetVector(ShaderProps._EmissionDissolveAnimation, src.GetVector(ShaderProps._EmissionDissolveAnimation));
		}
		if (src.HasFloat(ShaderProps._EmissionMaskByBaseMapAlpha))
		{
			dst.SetFloat(ShaderProps._EmissionMaskByBaseMapAlpha, src.GetFloat(ShaderProps._EmissionMaskByBaseMapAlpha));
		}
	}

	// Token: 0x060015DE RID: 5598 RVA: 0x00073C08 File Offset: 0x00071E08
	public static void SetGroupVisible(int groupId, bool visible)
	{
		byte b = (visible ? 1 : 0);
		for (int i = 0; i < IndirectMeshRenderer._batchList.Count; i++)
		{
			IndirectMeshRenderer.DrawBatch drawBatch = IndirectMeshRenderer._batchList[i];
			bool flag = false;
			int length = drawBatch.groupIds.Length;
			for (int j = 0; j < length; j++)
			{
				if (drawBatch.groupIds[j] == groupId && drawBatch.visibility[j] != b)
				{
					drawBatch.visibility[j] = b;
					drawBatch.visibleCount += (visible ? 1 : (-1));
					flag = true;
				}
			}
			if (flag)
			{
				drawBatch.dirty = true;
				IndirectMeshRenderer._batchList[i] = drawBatch;
			}
		}
	}

	// Token: 0x060015DF RID: 5599 RVA: 0x00073CC4 File Offset: 0x00071EC4
	private static void _Render()
	{
		if (IndirectMeshRenderer._batchList.Count == 0)
		{
			return;
		}
		if (!IndirectMeshRenderer._loggedFirstRender)
		{
			IndirectMeshRenderer._loggedFirstRender = true;
			int num = 0;
			for (int i = 0; i < IndirectMeshRenderer._batchList.Count; i++)
			{
				num += IndirectMeshRenderer._batchList[i].visibleCount;
			}
			Debug.Log(string.Format("[IndirectMeshRenderer] First render: {0} batch(es), {1} visible instance(s), stereoMul={2}", IndirectMeshRenderer._batchList.Count, num, 2));
		}
		for (int j = 0; j < IndirectMeshRenderer._batchList.Count; j++)
		{
			IndirectMeshRenderer.DrawBatch drawBatch = IndirectMeshRenderer._batchList[j];
			if (drawBatch.dynamicEntries != null)
			{
				for (int k = drawBatch.dynamicEntries.Count - 1; k >= 0; k--)
				{
					IndirectMeshRenderer.DynamicEntry dynamicEntry = drawBatch.dynamicEntries[k];
					if (dynamicEntry.transform == null)
					{
						if (drawBatch.visibility[dynamicEntry.matrixIndex] != 0)
						{
							drawBatch.visibility[dynamicEntry.matrixIndex] = 0;
							drawBatch.visibleCount--;
							drawBatch.dirty = true;
						}
						int num2 = drawBatch.dynamicEntries.Count - 1;
						drawBatch.dynamicEntries[k] = drawBatch.dynamicEntries[num2];
						drawBatch.dynamicEntries.RemoveAt(num2);
					}
					else
					{
						drawBatch.matrices[dynamicEntry.matrixIndex] = dynamicEntry.transform.localToWorldMatrix;
					}
				}
				if (!drawBatch.dirty && drawBatch.dynamicEntries.Count > 0)
				{
					drawBatch.needsUpload = true;
				}
			}
			if (drawBatch.visibleCount == 0)
			{
				if (drawBatch.dirty)
				{
					IndirectMeshRenderer._DisposeBatchBuffers(ref drawBatch);
					drawBatch.dirty = false;
					drawBatch.needsUpload = false;
				}
				IndirectMeshRenderer._batchList[j] = drawBatch;
			}
			else
			{
				if (drawBatch.dirty)
				{
					IndirectMeshRenderer._RebuildBatch(ref drawBatch);
				}
				else if (drawBatch.needsUpload)
				{
					IndirectMeshRenderer._UploadBatch(ref drawBatch);
				}
				IndirectMeshRenderer._batchList[j] = drawBatch;
				Graphics.RenderMeshIndirect(in drawBatch.renderParams, drawBatch.mesh, drawBatch.commandBuffer, drawBatch.submeshCount, 0);
			}
		}
	}

	// Token: 0x060015E0 RID: 5600 RVA: 0x00073EE4 File Offset: 0x000720E4
	private static void _RebuildBatch(ref IndirectMeshRenderer.DrawBatch batch)
	{
		int length = batch.matrices.Length;
		int num = batch.visibleCount * 2;
		IndirectMeshRenderer._DisposeBatchBuffers(ref batch);
		batch.matrixBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, num, 64);
		batch.commandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, batch.submeshCount, 20);
		if (!batch.gpuMatrices.IsCreated || batch.gpuMatrices.Length < num)
		{
			if (batch.gpuMatrices.IsCreated)
			{
				batch.gpuMatrices.Dispose();
			}
			batch.gpuMatrices = new NativeArray<Matrix4x4>(num, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		}
		Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		int num2 = 0;
		for (int i = 0; i < length; i++)
		{
			if (batch.visibility[i] != 0)
			{
				Matrix4x4 matrix4x = batch.matrices[i];
				Vector3 vector3 = new Vector3(matrix4x.m03, matrix4x.m13, matrix4x.m23);
				vector = Vector3.Min(vector, vector3);
				vector2 = Vector3.Max(vector2, vector3);
				int num3 = num2 * 2;
				batch.gpuMatrices[num3] = matrix4x;
				batch.gpuMatrices[num3 + 1] = matrix4x;
				num2++;
			}
		}
		batch.matrixBuffer.SetData<Matrix4x4>(batch.gpuMatrices, 0, 0, num);
		Vector3 vector4 = Vector3.one * 10f;
		Bounds bounds = new Bounds((vector + vector2) * 0.5f, vector2 - vector + vector4);
		if (!batch.commandData.IsCreated || batch.commandData.Length != batch.submeshCount)
		{
			if (batch.commandData.IsCreated)
			{
				batch.commandData.Dispose();
			}
			batch.commandData = new NativeArray<GraphicsBuffer.IndirectDrawIndexedArgs>(batch.submeshCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		}
		for (int j = 0; j < batch.submeshCount; j++)
		{
			batch.commandData[j] = new GraphicsBuffer.IndirectDrawIndexedArgs
			{
				indexCountPerInstance = batch.mesh.GetIndexCount(j),
				startIndex = batch.mesh.GetIndexStart(j),
				baseVertexIndex = batch.mesh.GetBaseVertex(j),
				startInstance = 0U,
				instanceCount = (uint)num
			};
		}
		batch.commandBuffer.SetData<GraphicsBuffer.IndirectDrawIndexedArgs>(batch.commandData);
		batch.renderParams = new RenderParams(batch.material)
		{
			worldBounds = bounds,
			layer = batch.layer,
			shadowCastingMode = ShadowCastingMode.Off,
			receiveShadows = false,
			matProps = new MaterialPropertyBlock()
		};
		batch.renderParams.matProps.SetBuffer(IndirectMeshRenderer._spId_Matrices, batch.matrixBuffer);
		batch.dirty = false;
		batch.needsUpload = false;
	}

	// Token: 0x060015E1 RID: 5601 RVA: 0x000741C0 File Offset: 0x000723C0
	private static void _UploadBatch(ref IndirectMeshRenderer.DrawBatch batch)
	{
		int length = batch.matrices.Length;
		Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		int num = 0;
		for (int i = 0; i < length; i++)
		{
			if (batch.visibility[i] != 0)
			{
				Matrix4x4 matrix4x = batch.matrices[i];
				Vector3 vector3 = new Vector3(matrix4x.m03, matrix4x.m13, matrix4x.m23);
				vector = Vector3.Min(vector, vector3);
				vector2 = Vector3.Max(vector2, vector3);
				int num2 = num * 2;
				batch.gpuMatrices[num2] = matrix4x;
				batch.gpuMatrices[num2 + 1] = matrix4x;
				num++;
			}
		}
		batch.matrixBuffer.SetData<Matrix4x4>(batch.gpuMatrices, 0, 0, num * 2);
		Vector3 vector4 = Vector3.one * 10f;
		batch.renderParams.worldBounds = new Bounds((vector + vector2) * 0.5f, vector2 - vector + vector4);
		batch.needsUpload = false;
	}

	// Token: 0x060015E2 RID: 5602 RVA: 0x000742E9 File Offset: 0x000724E9
	private static void _DisposeBatchBuffers(ref IndirectMeshRenderer.DrawBatch batch)
	{
		GraphicsBuffer matrixBuffer = batch.matrixBuffer;
		if (matrixBuffer != null)
		{
			matrixBuffer.Dispose();
		}
		batch.matrixBuffer = null;
		GraphicsBuffer commandBuffer = batch.commandBuffer;
		if (commandBuffer != null)
		{
			commandBuffer.Dispose();
		}
		batch.commandBuffer = null;
	}

	// Token: 0x060015E3 RID: 5603 RVA: 0x0007431C File Offset: 0x0007251C
	private static void _DisposeBatch(ref IndirectMeshRenderer.DrawBatch batch)
	{
		IndirectMeshRenderer._DisposeBatchBuffers(ref batch);
		if (batch.matrices.IsCreated)
		{
			batch.matrices.Dispose();
		}
		if (batch.groupIds.IsCreated)
		{
			batch.groupIds.Dispose();
		}
		if (batch.visibility.IsCreated)
		{
			batch.visibility.Dispose();
		}
		if (batch.gpuMatrices.IsCreated)
		{
			batch.gpuMatrices.Dispose();
		}
		if (batch.commandData.IsCreated)
		{
			batch.commandData.Dispose();
		}
		if (batch.material != null)
		{
			Object.Destroy(batch.material);
		}
		batch.dynamicEntries = null;
	}

	// Token: 0x060015E4 RID: 5604 RVA: 0x000743C8 File Offset: 0x000725C8
	private static void _DisposeAll()
	{
		for (int i = 0; i < IndirectMeshRenderer._batchList.Count; i++)
		{
			IndirectMeshRenderer.DrawBatch drawBatch = IndirectMeshRenderer._batchList[i];
			IndirectMeshRenderer._DisposeBatch(ref drawBatch);
		}
		IndirectMeshRenderer._batchList.Clear();
		IndirectMeshRenderer._batchLookup.Clear();
	}

	// Token: 0x04001AA5 RID: 6821
	private const string SHADER_NAME = "GorillaTag/IndirectLit";

	// Token: 0x04001AA6 RID: 6822
	private const string SHADER_NAME_EMISSIVE = "GorillaTag/IndirectLitEmissive";

	// Token: 0x04001AA7 RID: 6823
	private const int _k_instancesPerXform = 2;

	// Token: 0x04001AA8 RID: 6824
	private static readonly int _spId_Matrices = Shader.PropertyToID("_Matrices");

	// Token: 0x04001AA9 RID: 6825
	private static Shader _shader;

	// Token: 0x04001AAA RID: 6826
	private static Shader _shaderEmissive;

	// Token: 0x04001AAB RID: 6827
	private static readonly Dictionary<IndirectMeshRenderer.BatchKey, int> _batchLookup = new Dictionary<IndirectMeshRenderer.BatchKey, int>();

	// Token: 0x04001AAC RID: 6828
	private static readonly List<IndirectMeshRenderer.DrawBatch> _batchList = new List<IndirectMeshRenderer.DrawBatch>();

	// Token: 0x04001AAD RID: 6829
	private static bool _loggedFirstRender;

	// Token: 0x0200037D RID: 893
	private struct BatchKey : IEquatable<IndirectMeshRenderer.BatchKey>
	{
		// Token: 0x060015E6 RID: 5606 RVA: 0x00074436 File Offset: 0x00072636
		public bool Equals(IndirectMeshRenderer.BatchKey other)
		{
			return this.meshId == other.meshId && this.textureId == other.textureId && this.shaderId == other.shaderId;
		}

		// Token: 0x060015E7 RID: 5607 RVA: 0x00074464 File Offset: 0x00072664
		public override int GetHashCode()
		{
			return (((this.meshId * 397) ^ this.textureId) * 397) ^ this.shaderId;
		}

		// Token: 0x060015E8 RID: 5608 RVA: 0x00074488 File Offset: 0x00072688
		public override bool Equals(object obj)
		{
			if (obj is IndirectMeshRenderer.BatchKey)
			{
				IndirectMeshRenderer.BatchKey batchKey = (IndirectMeshRenderer.BatchKey)obj;
				return this.Equals(batchKey);
			}
			return false;
		}

		// Token: 0x04001AAE RID: 6830
		public int meshId;

		// Token: 0x04001AAF RID: 6831
		public int textureId;

		// Token: 0x04001AB0 RID: 6832
		public int shaderId;
	}

	// Token: 0x0200037E RID: 894
	private struct DynamicEntry
	{
		// Token: 0x04001AB1 RID: 6833
		public Transform transform;

		// Token: 0x04001AB2 RID: 6834
		public int matrixIndex;
	}

	// Token: 0x0200037F RID: 895
	private struct DrawBatch
	{
		// Token: 0x04001AB3 RID: 6835
		public Mesh mesh;

		// Token: 0x04001AB4 RID: 6836
		public Material material;

		// Token: 0x04001AB5 RID: 6837
		public int submeshCount;

		// Token: 0x04001AB6 RID: 6838
		public int layer;

		// Token: 0x04001AB7 RID: 6839
		public NativeList<Matrix4x4> matrices;

		// Token: 0x04001AB8 RID: 6840
		public NativeList<int> groupIds;

		// Token: 0x04001AB9 RID: 6841
		public NativeList<byte> visibility;

		// Token: 0x04001ABA RID: 6842
		public int visibleCount;

		// Token: 0x04001ABB RID: 6843
		public NativeArray<Matrix4x4> gpuMatrices;

		// Token: 0x04001ABC RID: 6844
		public GraphicsBuffer matrixBuffer;

		// Token: 0x04001ABD RID: 6845
		public GraphicsBuffer commandBuffer;

		// Token: 0x04001ABE RID: 6846
		public NativeArray<GraphicsBuffer.IndirectDrawIndexedArgs> commandData;

		// Token: 0x04001ABF RID: 6847
		public RenderParams renderParams;

		// Token: 0x04001AC0 RID: 6848
		public bool dirty;

		// Token: 0x04001AC1 RID: 6849
		public bool needsUpload;

		// Token: 0x04001AC2 RID: 6850
		public List<IndirectMeshRenderer.DynamicEntry> dynamicEntries;
	}

	// Token: 0x02000380 RID: 896
	private sealed class PostTickCallback : ITickSystemPost
	{
		// Token: 0x1700022E RID: 558
		// (get) Token: 0x060015E9 RID: 5609 RVA: 0x000744AD File Offset: 0x000726AD
		// (set) Token: 0x060015EA RID: 5610 RVA: 0x000744B5 File Offset: 0x000726B5
		public bool PostTickRunning { get; set; }

		// Token: 0x060015EB RID: 5611 RVA: 0x000744BE File Offset: 0x000726BE
		public void PostTick()
		{
			IndirectMeshRenderer._Render();
		}
	}
}
