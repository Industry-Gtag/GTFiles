using System;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x020001F0 RID: 496
public struct VoxelOperation
{
	// Token: 0x06000D00 RID: 3328 RVA: 0x000474E4 File Offset: 0x000456E4
	public VoxelOperation(Vector3 origin, VoxelAction action)
	{
		this.origin = (int3)(origin * 256f);
		this.operationType = action.operation;
		this.radius = (short)(action.radius * 256f);
		this.strength = (short)(action.strength * 256f);
		this.material = action.material;
	}

	// Token: 0x06000D01 RID: 3329 RVA: 0x0004754A File Offset: 0x0004574A
	public bool IsValid()
	{
		return this.radius > 0 && this.strength > 0 && (this.operationType == OperationType.Add || this.operationType == OperationType.Subtract);
	}

	// Token: 0x06000D02 RID: 3330 RVA: 0x00047574 File Offset: 0x00045774
	public override string ToString()
	{
		return string.Join(", ", new object[] { this.origin, this.operationType, this.radius, this.strength, this.material });
	}

	// Token: 0x04000FAB RID: 4011
	public int3 origin;

	// Token: 0x04000FAC RID: 4012
	public OperationType operationType;

	// Token: 0x04000FAD RID: 4013
	public short radius;

	// Token: 0x04000FAE RID: 4014
	public short strength;

	// Token: 0x04000FAF RID: 4015
	public byte material;
}
