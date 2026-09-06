using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using Voxels;

// Token: 0x020001F2 RID: 498
public static class VoxelExtensions
{
	// Token: 0x06000D05 RID: 3333 RVA: 0x00047636 File Offset: 0x00045836
	public static void Mine(this VoxelWorld world, Collision collision, VoxelAction action)
	{
		world.Mine(collision.ToRaycastHit(), action);
	}

	// Token: 0x06000D06 RID: 3334 RVA: 0x00047648 File Offset: 0x00045848
	public static void Mine(this VoxelWorld world, RaycastHit hit, VoxelAction action)
	{
		MeshGenerationMode meshGenerationMode = world.MeshGenerationMode;
		if (meshGenerationMode == MeshGenerationMode.MarchingCubes)
		{
			world.Mine_MarchingCubes(hit, action);
			return;
		}
		if (meshGenerationMode != MeshGenerationMode.SurfaceNets)
		{
			throw new ArgumentOutOfRangeException();
		}
		world.Mine_SurfaceNets(hit, action);
	}

	// Token: 0x06000D07 RID: 3335 RVA: 0x0004767C File Offset: 0x0004587C
	private static void Mine_MarchingCubes(this VoxelWorld world, RaycastHit hit, VoxelAction action)
	{
		action.radius /= world.Scale;
		Vector3 vector = hit.point;
		VoxelExtensions._lastHitPoint = vector;
		int triangleIndex = hit.triangleIndex;
		if (VoxelExtensions._showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, vector, Color.red, 20f);
		}
		MeshCollider meshCollider = hit.collider as MeshCollider;
		if (meshCollider != null)
		{
			Mesh sharedMesh = meshCollider.sharedMesh;
			if (!sharedMesh)
			{
				Debug.LogError(string.Format("{0} has no mesh!?", meshCollider));
				return;
			}
			if (VoxelExtensions._tris == null)
			{
				VoxelExtensions._tris = new List<int>(65535);
			}
			if (VoxelExtensions._verts == null)
			{
				VoxelExtensions._verts = new List<Vector3>(65535);
			}
			sharedMesh.GetTriangles(VoxelExtensions._tris, 0);
			sharedMesh.GetVertices(VoxelExtensions._verts);
			if (triangleIndex < 0 || triangleIndex >= VoxelExtensions._tris.Count / 3)
			{
				Debug.LogError(string.Format("Invalid triangle index {0} for mesh {1}", triangleIndex, sharedMesh.name));
				return;
			}
			Vector3 vector2 = meshCollider.transform.InverseTransformPoint(vector);
			Vector3 vector3 = VoxelExtensions._verts[VoxelExtensions._tris[triangleIndex * 3]];
			Vector3 vector4 = VoxelExtensions._verts[VoxelExtensions._tris[triangleIndex * 3 + 1]];
			Vector3 vector5 = VoxelExtensions._verts[VoxelExtensions._tris[triangleIndex * 3 + 2]];
			Vector3 vector6 = (((vector2 - vector3).sqrMagnitude < (vector2 - vector4).sqrMagnitude) ? (((vector2 - vector3).sqrMagnitude < (vector2 - vector5).sqrMagnitude) ? vector3 : vector5) : (((vector2 - vector4).sqrMagnitude < (vector2 - vector5).sqrMagnitude) ? vector4 : vector5));
			vector6 = (VoxelExtensions._lastVertex = meshCollider.transform.TransformPoint(vector6));
			if ((hit.point - vector6).sqrMagnitude > world.Scale * 1.6f)
			{
				vector6 = (VoxelExtensions._lastVertex = hit.point);
			}
			if (VoxelExtensions._showDebug)
			{
				Debug.Log(string.Format("Closest vertex to {0}: {1}", vector, vector6));
			}
			if (VoxelExtensions._showDebug)
			{
				Debug.DrawLine(vector, vector6, Color.blue, 20f);
			}
			vector6 = world.GetLocalPosition(vector6);
			vector = vector6.SnapToInt();
			if (VoxelExtensions._cascade && !world.GetDensityAt(vector).IsSolid())
			{
				if (VoxelExtensions._showDebug)
				{
					Debug.Log(string.Format("Hit air at {0}, moving to next voxel", vector));
				}
				if (VoxelExtensions._showDebug)
				{
					Debug.DrawLine(vector, vector + (vector6 - vector).normalized, Color.cyan, 20f);
				}
				vector += (vector6 - vector).normalized;
			}
		}
		VoxelExtensions._lastGridPoint = vector;
		if (VoxelExtensions._showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, vector, Color.white, 20f);
		}
		int3 @int = vector.RoundToInt();
		if (VoxelExtensions._showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, @int.ToVector3(), Color.yellow, 20f);
		}
		Vector3Int vector3Int = Vector3Int.one * Mathf.CeilToInt(action.radius);
		Vector3Int vector3Int2 = @int.ToVectorInt() - vector3Int;
		VoxelExtensions._lastBounds = new global::UnityEngine.BoundsInt(vector3Int2, vector3Int * 2);
		VoxelManager.Mine(world, hit.point, hit.normal, vector, action);
	}

	// Token: 0x06000D08 RID: 3336 RVA: 0x00047A1C File Offset: 0x00045C1C
	private static void Mine_SurfaceNets(this VoxelWorld world, RaycastHit hit, VoxelAction action)
	{
		action.radius /= world.Scale;
		Vector3 vector = hit.point;
		VoxelExtensions._lastHitPoint = vector;
		if (VoxelExtensions._showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, vector, Color.red, 20f);
		}
		Vector3 localPosition = world.GetLocalPosition(VoxelExtensions.GetTriangleCenter(hit));
		vector = localPosition.SnapToInt();
		if (VoxelExtensions._cascade && world.GetDensityAt(vector) == 0)
		{
			if (VoxelExtensions._showDebug)
			{
				Debug.Log(string.Format("Hit air at {0}, moving to next voxel", vector));
			}
			int3 closestCardinalNeighbour = vector.ToInt3().GetClosestCardinalNeighbour(localPosition - hit.normal * 0.5f);
			if (VoxelExtensions._showDebug)
			{
				Debug.DrawLine(vector, closestCardinalNeighbour.ToFloat3(), Color.cyan, 20f);
			}
			vector = closestCardinalNeighbour.ToFloat3();
		}
		VoxelExtensions._lastGridPoint = vector;
		if (VoxelExtensions._showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, vector, Color.white, 20f);
		}
		int3 @int = vector.RoundToInt();
		if (VoxelExtensions._showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, @int.ToVector3(), Color.yellow, 20f);
		}
		Vector3Int vector3Int = Vector3Int.one * Mathf.CeilToInt(action.radius);
		Vector3Int vector3Int2 = @int.ToVectorInt() - vector3Int;
		VoxelExtensions._lastBounds = new global::UnityEngine.BoundsInt(vector3Int2, vector3Int * 2);
		VoxelManager.Mine(world, hit.point, hit.normal, vector, action);
	}

	// Token: 0x06000D09 RID: 3337 RVA: 0x00047BB0 File Offset: 0x00045DB0
	private static void AddMined(byte material, int amount)
	{
		if ((int)material < VoxelExtensions._opMined.Length)
		{
			VoxelExtensions._opTotalMined += amount;
			VoxelExtensions._opMined[(int)material] += amount;
		}
	}

	// Token: 0x06000D0A RID: 3338 RVA: 0x00047BE8 File Offset: 0x00045DE8
	public static int[] PerformLocalMiningOperation(this VoxelWorld world, VoxelManager.VoxelMineOperation mineOp, bool immediate = true)
	{
		VoxelOperation op = mineOp.op;
		Vector3 worldPosition = world.GetWorldPosition(mineOp.localHitPoint);
		Vector3 hitNormal = mineOp.hitNormal;
		VoxelExtensions._lastBounds = world.GetBounds(op.origin, (int)op.radius);
		VoxelExtensions._opMaterialSet = world.MaterialSet;
		VoxelExtensions._op = op;
		VoxelExtensions._opTotalMined = 0;
		VoxelExtensions._opMined = VoxelManager.GetIntArray(world.MaterialSet.Materials.Length);
		OperationType operationType = VoxelExtensions._op.operationType;
		if (operationType != OperationType.Subtract)
		{
			if (operationType != OperationType.Add)
			{
				throw new ArgumentOutOfRangeException();
			}
			world.SetVoxelDataCustom(VoxelExtensions._lastBounds, new Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>>(VoxelExtensions.UnMineAt), immediate);
		}
		else
		{
			world.SetVoxelDataCustom(VoxelExtensions._lastBounds, new Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>>(VoxelExtensions.MineAt), immediate);
			if (VoxelExtensions._opTotalMined > 0)
			{
				VoxelEvents.HandleResourceMined(world, worldPosition, hitNormal, VoxelExtensions._opMined);
			}
		}
		return VoxelExtensions._opMined;
	}

	// Token: 0x06000D0B RID: 3339 RVA: 0x00047CC0 File Offset: 0x00045EC0
	public static void PerformLocalOperation(this VoxelWorld world, Vector3 localPosition, VoxelAction action, bool immediate = true)
	{
		global::UnityEngine.BoundsInt bounds = world.GetBounds(localPosition, action.radius);
		VoxelExtensions._opAction = action;
		VoxelExtensions._opOrigin = localPosition;
		OperationType operation = VoxelExtensions._opAction.operation;
		if (operation == OperationType.Subtract)
		{
			world.SetVoxelDensityCustom(bounds, new Func<int3, byte, byte>(VoxelExtensions.SubtractAt), immediate);
			return;
		}
		if (operation != OperationType.Add)
		{
			throw new ArgumentOutOfRangeException();
		}
		world.SetVoxelDensityCustom(bounds, new Func<int3, byte, byte>(VoxelExtensions.AddAt), immediate);
	}

	// Token: 0x06000D0C RID: 3340 RVA: 0x00047D30 File Offset: 0x00045F30
	[return: TupleElementNames(new string[] { "density", "material" })]
	private static ValueTuple<byte, byte> MineAt(int3 point, [TupleElementNames(new string[] { "density", "material" })] ValueTuple<byte, byte> data)
	{
		byte item = data.Item1;
		byte item2 = data.Item2;
		int num = (int)VoxelExtensions._op.strength / VoxelExtensions._opMaterialSet.GetHardness(item2);
		int num2 = VoxelExtensions.FastDistance(VoxelExtensions._op.origin, point * 256);
		byte b = ((num2 > (int)VoxelExtensions._op.radius) ? item : ((byte)math.clamp((int)item - num * VoxelExtensions.IntLerp(255, 0, num2, (int)VoxelExtensions._op.radius) / 256, 0, 255)));
		if (VoxelExtensions._showDebug && item != b)
		{
			Debug.Log(string.Format("Hit at {0}->{1}=d{2:F2} with density {3}[{4}] -> {5}[{6}]", new object[]
			{
				VoxelExtensions._op.origin,
				point,
				num2,
				item,
				item.ToFloat(),
				b,
				b.ToFloat()
			}));
		}
		if (item.IsSolid())
		{
			int num3 = (int)((item - b) / 10);
			VoxelExtensions.AddMined(item2, num3);
		}
		return new ValueTuple<byte, byte>(b, item2);
	}

	// Token: 0x06000D0D RID: 3341 RVA: 0x00047E54 File Offset: 0x00046054
	[return: TupleElementNames(new string[] { "density", "material" })]
	private static ValueTuple<byte, byte> UnMineAt(int3 point, [TupleElementNames(new string[] { "density", "material" })] ValueTuple<byte, byte> data)
	{
		byte item = data.Item1;
		byte b = data.Item2;
		int num = (int)VoxelExtensions._op.strength / VoxelExtensions._opMaterialSet.GetHardness(b);
		int num2 = VoxelExtensions.FastDistance(VoxelExtensions._op.origin, point * 256);
		byte b2 = (((float)num2 > VoxelExtensions._opAction.radius) ? item : ((byte)math.clamp((int)item + num * VoxelExtensions.IntLerp(255, 0, num2, (int)VoxelExtensions._op.radius) / 256, 0, 255)));
		if (item != b2)
		{
			b = VoxelExtensions._opAction.material;
		}
		if (VoxelExtensions._showDebug && item != b2)
		{
			Debug.Log(string.Format("Unmined at {0}->{1}=d{2:F2} with density {3}[{4}] -> {5}[{6}]", new object[]
			{
				VoxelExtensions._op.origin,
				point,
				num2,
				item,
				item.ToFloat(),
				b2,
				b2.ToFloat()
			}));
		}
		if (!item.IsSolid() && b2.IsSolid())
		{
			int num3 = (int)((float)(b2 - item) / 10f);
			VoxelExtensions.AddMined(b, num3);
		}
		return new ValueTuple<byte, byte>(b2, b);
	}

	// Token: 0x06000D0E RID: 3342 RVA: 0x00047F94 File Offset: 0x00046194
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte SubtractAt(int3 point, byte density)
	{
		float num = math.distance(VoxelExtensions._opOrigin, point);
		if (num <= VoxelExtensions._opAction.radius)
		{
			return (byte)math.clamp((float)density - VoxelExtensions._opAction.strength * math.lerp(255f, 0f, num / VoxelExtensions._opAction.radius), 0f, 255f);
		}
		return density;
	}

	// Token: 0x06000D0F RID: 3343 RVA: 0x00048000 File Offset: 0x00046200
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte AddAt(int3 point, byte density)
	{
		float num = math.distance(VoxelExtensions._opOrigin, point);
		if (num <= VoxelExtensions._opAction.radius)
		{
			return (byte)math.clamp((float)density + VoxelExtensions._opAction.strength * math.lerp(255f, 0f, num / VoxelExtensions._opAction.radius), 0f, 255f);
		}
		return density;
	}

	// Token: 0x06000D10 RID: 3344 RVA: 0x0004806B File Offset: 0x0004626B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[return: TupleElementNames(new string[] { "density", "materialID" })]
	private static ValueTuple<byte, byte> SetVoxelAt(int3 point, [TupleElementNames(new string[] { "density", "materialId" })] ValueTuple<byte, byte> data)
	{
		return new ValueTuple<byte, byte>(VoxelExtensions._opDensity, VoxelExtensions._opMaterialId);
	}

	// Token: 0x06000D11 RID: 3345 RVA: 0x0004807C File Offset: 0x0004627C
	public static void PerformAction(this VoxelWorld world, Vector3 position, VoxelAction action)
	{
		VoxelManager.PerformOperation(world, position, action);
	}

	// Token: 0x06000D12 RID: 3346 RVA: 0x00048086 File Offset: 0x00046286
	public static void Dig(this VoxelWorld world, Vector3 position, float radius, float strength)
	{
		VoxelManager.PerformOperation(world, position, new VoxelAction(OperationType.Subtract, radius, strength, 0));
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x00048098 File Offset: 0x00046298
	public static void Add(this VoxelWorld world, Vector3 position, float radius, float strength)
	{
		VoxelManager.PerformOperation(world, position, new VoxelAction(OperationType.Add, radius, strength, 0));
	}

	// Token: 0x06000D14 RID: 3348 RVA: 0x000480AC File Offset: 0x000462AC
	public static void SetVoxel(this VoxelWorld world, int x, int y, int z, byte density, byte materialId)
	{
		VoxelExtensions.<>c__DisplayClass32_0 CS$<>8__locals1 = new VoxelExtensions.<>c__DisplayClass32_0();
		CS$<>8__locals1.density = density;
		CS$<>8__locals1.materialId = materialId;
		Vector3Int vector3Int = new Vector3Int(x, y, z);
		global::UnityEngine.BoundsInt boundsInt = new global::UnityEngine.BoundsInt(vector3Int, Vector3Int.zero);
		world.SetVoxelDataCustom(boundsInt, new Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>>(CS$<>8__locals1.<SetVoxel>g__SetVoxelAt|0), true);
	}

	// Token: 0x06000D15 RID: 3349 RVA: 0x000480FA File Offset: 0x000462FA
	public static void SetVoxels(this VoxelWorld world, global::UnityEngine.BoundsInt worldBounds, byte density, byte materialId, bool immediate = true)
	{
		VoxelExtensions._opDensity = density;
		VoxelExtensions._opMaterialId = materialId;
		world.SetVoxelDataCustom(worldBounds, new Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>>(VoxelExtensions.SetVoxelAt), immediate);
	}

	// Token: 0x06000D16 RID: 3350 RVA: 0x0004811D File Offset: 0x0004631D
	public static void SetVoxels(this VoxelWorld world, int3[] voxels, byte density, byte materialId, bool immediate = true)
	{
		VoxelExtensions._opDensity = density;
		VoxelExtensions._opMaterialId = materialId;
		world.SetVoxelDataCustom(voxels, new Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>>(VoxelExtensions.SetVoxelAt), immediate);
	}

	// Token: 0x06000D17 RID: 3351 RVA: 0x00048140 File Offset: 0x00046340
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetVoxelCount(this global::UnityEngine.BoundsInt bounds)
	{
		return (bounds.max.x - bounds.min.x + 1) * (bounds.max.y - bounds.min.y + 1) * (bounds.max.z - bounds.min.z + 1);
	}

	// Token: 0x06000D18 RID: 3352 RVA: 0x000481B2 File Offset: 0x000463B2
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Contains(this global::UnityEngine.BoundsInt a, global::UnityEngine.BoundsInt b)
	{
		return a.Contains(b.min) && a.Contains(b.max);
	}

	// Token: 0x06000D19 RID: 3353 RVA: 0x000481D4 File Offset: 0x000463D4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static global::UnityEngine.BoundsInt Union(this global::UnityEngine.BoundsInt a, global::UnityEngine.BoundsInt b)
	{
		return new global::UnityEngine.BoundsInt(VectorUtilities.Min(a.min, b.min), VectorUtilities.Max(a.max, b.max));
	}

	// Token: 0x06000D1A RID: 3354 RVA: 0x00048204 File Offset: 0x00046404
	public static global::UnityEngine.BoundsInt GetBounds(this VoxelWorld world, int3 point, int radius)
	{
		int3 @int = point / 256;
		int3 int2 = point - @int * 256;
		@int += new int3(VoxelExtensions.<GetBounds>g__Round|38_0(int2.x), VoxelExtensions.<GetBounds>g__Round|38_0(int2.y), VoxelExtensions.<GetBounds>g__Round|38_0(int2.z));
		int num = VoxelExtensions.<GetBounds>g__Ceil|38_1(radius);
		return new global::UnityEngine.BoundsInt((@int - num).ToVectorInt(), new int3(num * 2).ToVectorInt());
	}

	// Token: 0x06000D1B RID: 3355 RVA: 0x00048284 File Offset: 0x00046484
	public static global::UnityEngine.BoundsInt GetBounds(this VoxelWorld world, float3 point, float radius)
	{
		int3 voxelForLocalPosition = world.GetVoxelForLocalPosition(point);
		int num = Mathf.CeilToInt(radius);
		return new global::UnityEngine.BoundsInt((voxelForLocalPosition - num).ToVectorInt(), new int3(num * 2).ToVectorInt());
	}

	// Token: 0x06000D1C RID: 3356 RVA: 0x000482C4 File Offset: 0x000464C4
	private static Vector3 GetTriangleCenter(RaycastHit hit)
	{
		ValueTuple<Vector3, Vector3, Vector3> worldTriangle = VoxelExtensions.GetWorldTriangle(hit);
		Vector3 item = worldTriangle.Item1;
		Vector3 item2 = worldTriangle.Item2;
		Vector3 item3 = worldTriangle.Item3;
		return (item + item2 + item3) / 3f;
	}

	// Token: 0x06000D1D RID: 3357 RVA: 0x00048304 File Offset: 0x00046504
	[return: TupleElementNames(new string[] { "v1", "v2", "v3" })]
	private static ValueTuple<Vector3, Vector3, Vector3> GetWorldTriangle(RaycastHit hit)
	{
		MeshCollider meshCollider = hit.collider as MeshCollider;
		if (meshCollider == null)
		{
			return new ValueTuple<Vector3, Vector3, Vector3>(Vector3.zero, Vector3.zero, Vector3.zero);
		}
		int triangleIndex = hit.triangleIndex;
		Mesh sharedMesh = meshCollider.sharedMesh;
		if (!sharedMesh)
		{
			Debug.LogError(string.Format("{0} has no mesh!?", meshCollider));
			return default(ValueTuple<Vector3, Vector3, Vector3>);
		}
		if (VoxelExtensions._tris == null)
		{
			VoxelExtensions._tris = new List<int>(65535);
		}
		if (VoxelExtensions._verts == null)
		{
			VoxelExtensions._verts = new List<Vector3>(65535);
		}
		sharedMesh.GetTriangles(VoxelExtensions._tris, 0);
		sharedMesh.GetVertices(VoxelExtensions._verts);
		if (triangleIndex < 0 || triangleIndex >= VoxelExtensions._tris.Count / 3)
		{
			Debug.LogError(string.Format("Invalid triangle index {0} for mesh {1}", triangleIndex, sharedMesh.name));
			return default(ValueTuple<Vector3, Vector3, Vector3>);
		}
		Transform transform = meshCollider.transform;
		Vector3 vector = transform.TransformPoint(VoxelExtensions._verts[VoxelExtensions._tris[triangleIndex * 3]]);
		Vector3 vector2 = transform.TransformPoint(VoxelExtensions._verts[VoxelExtensions._tris[triangleIndex * 3 + 1]]);
		Vector3 vector3 = transform.TransformPoint(VoxelExtensions._verts[VoxelExtensions._tris[triangleIndex * 3 + 2]]);
		return new ValueTuple<Vector3, Vector3, Vector3>(vector, vector2, vector3);
	}

	// Token: 0x06000D1E RID: 3358 RVA: 0x00048456 File Offset: 0x00046656
	public static string GetFullPath(this Component component)
	{
		if (!component)
		{
			return "";
		}
		return component.gameObject.GetFullPath() + "/" + component.GetType().Name;
	}

	// Token: 0x06000D1F RID: 3359 RVA: 0x00048488 File Offset: 0x00046688
	public static string GetFullPath(this GameObject go)
	{
		if (!go)
		{
			return "";
		}
		string text = go.name;
		Transform transform = go.transform.parent;
		while (transform)
		{
			text = transform.name + "/" + text;
			transform = transform.parent;
		}
		return go.scene.name + "/" + text;
	}

	// Token: 0x06000D20 RID: 3360 RVA: 0x000484F2 File Offset: 0x000466F2
	public static int GenerateHashcodeFromPath(this Component component)
	{
		return component.GetFullPath().GetHashCode();
	}

	// Token: 0x06000D21 RID: 3361 RVA: 0x000484FF File Offset: 0x000466FF
	public static int GenerateHashcodeFromPath(this GameObject go)
	{
		return go.GetFullPath().GetHashCode();
	}

	// Token: 0x06000D22 RID: 3362 RVA: 0x0004850C File Offset: 0x0004670C
	public static int FastDistance(int3 a, int3 b)
	{
		int3 @int = math.abs(a - b);
		int num = @int.x;
		int num2 = @int.y;
		int num3 = @int.z;
		if (num < num2)
		{
			int num4 = num2;
			int num5 = num;
			num = num4;
			num2 = num5;
		}
		if (num < num3)
		{
			int num6 = num3;
			int num5 = num;
			num = num6;
			num3 = num5;
		}
		if (num2 < num3)
		{
			int num7 = num3;
			int num5 = num2;
			num2 = num7;
			num3 = num5;
		}
		int num8 = num;
		int num9 = num2;
		int num10 = num3;
		return num8 + (num9 >> 1) + (num10 >> 2);
	}

	// Token: 0x06000D23 RID: 3363 RVA: 0x0004856C File Offset: 0x0004676C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IntLerp(int start, int end, int t, int tMax)
	{
		t = t * 256 / tMax;
		t = math.clamp(t, 0, 256);
		return start + (int)((long)t * (long)(end - start) / 256L);
	}

	// Token: 0x06000D24 RID: 3364 RVA: 0x00048598 File Offset: 0x00046798
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IntLerp(int start, int end, int t)
	{
		t = math.clamp(t, 0, 256);
		return start + (int)((long)t * (long)(end - start) / 256L);
	}

	// Token: 0x06000D26 RID: 3366 RVA: 0x000485C4 File Offset: 0x000467C4
	[CompilerGenerated]
	internal static int <GetBounds>g__Round|38_0(int value)
	{
		int num = 128;
		if (value < -num)
		{
			return -1;
		}
		if (value <= num)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x06000D27 RID: 3367 RVA: 0x000485E8 File Offset: 0x000467E8
	[CompilerGenerated]
	internal static int <GetBounds>g__Ceil|38_1(int value)
	{
		int num = value / 256;
		if (value - num * 256 > 0)
		{
			num++;
		}
		return num;
	}

	// Token: 0x04000FB5 RID: 4021
	private static global::UnityEngine.BoundsInt _lastBounds;

	// Token: 0x04000FB6 RID: 4022
	private static Vector3 _lastHitPoint;

	// Token: 0x04000FB7 RID: 4023
	private static Vector3 _lastGridPoint;

	// Token: 0x04000FB8 RID: 4024
	private static Vector3 _lastVertex;

	// Token: 0x04000FB9 RID: 4025
	private static bool _showDebug;

	// Token: 0x04000FBA RID: 4026
	private static bool _centerOnly;

	// Token: 0x04000FBB RID: 4027
	private static bool _cascade = true;

	// Token: 0x04000FBC RID: 4028
	private static List<int> _tris;

	// Token: 0x04000FBD RID: 4029
	private static List<Vector3> _verts;

	// Token: 0x04000FBE RID: 4030
	private static VoxelMaterialSet _opMaterialSet;

	// Token: 0x04000FBF RID: 4031
	private static VoxelOperation _op;

	// Token: 0x04000FC0 RID: 4032
	private static VoxelAction _opAction;

	// Token: 0x04000FC1 RID: 4033
	private static Vector3 _opOrigin;

	// Token: 0x04000FC2 RID: 4034
	private static int _opTotalMined;

	// Token: 0x04000FC3 RID: 4035
	private static int[] _opMined;

	// Token: 0x04000FC4 RID: 4036
	private static byte _opDensity;

	// Token: 0x04000FC5 RID: 4037
	private static byte _opMaterialId;
}
