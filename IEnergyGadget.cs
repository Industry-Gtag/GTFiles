using System;

// Token: 0x020000FF RID: 255
public interface IEnergyGadget
{
	// Token: 0x17000067 RID: 103
	// (get) Token: 0x060005F5 RID: 1525
	bool UsesEnergy { get; }

	// Token: 0x17000068 RID: 104
	// (get) Token: 0x060005F6 RID: 1526
	bool IsFull { get; }

	// Token: 0x060005F7 RID: 1527
	void UpdateRecharge(float dt);
}
