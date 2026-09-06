using System;

// Token: 0x020000E0 RID: 224
public interface SIGadgetBlasterType
{
	// Token: 0x06000543 RID: 1347
	void OnUpdateAuthority(float dt);

	// Token: 0x06000544 RID: 1348
	void OnUpdateRemote(float dt);

	// Token: 0x06000545 RID: 1349
	void SetStateShared();

	// Token: 0x06000546 RID: 1350
	void NetworkFireProjectile(object[] data);

	// Token: 0x06000547 RID: 1351
	void ApplyUpgradeNodes(SIUpgradeSet withUpgrades);
}
