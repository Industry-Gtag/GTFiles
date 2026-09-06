using System;

// Token: 0x020001EF RID: 495
[Serializable]
public struct VoxelAction
{
	// Token: 0x06000CFE RID: 3326 RVA: 0x0004746D File Offset: 0x0004566D
	public VoxelAction(OperationType operation, float radius, float strength, byte material = 0)
	{
		this.operation = operation;
		this.radius = radius;
		this.strength = strength;
		this.material = material;
	}

	// Token: 0x06000CFF RID: 3327 RVA: 0x0004748C File Offset: 0x0004568C
	public bool IsValid()
	{
		return float.IsFinite(this.radius) && this.radius > 0f && float.IsFinite(this.strength) && this.strength > 0f && (this.operation == OperationType.Add || this.operation == OperationType.Subtract);
	}

	// Token: 0x04000FA7 RID: 4007
	public OperationType operation;

	// Token: 0x04000FA8 RID: 4008
	public float radius;

	// Token: 0x04000FA9 RID: 4009
	public float strength;

	// Token: 0x04000FAA RID: 4010
	public byte material;
}
