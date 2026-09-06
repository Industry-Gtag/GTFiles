using System;
using GorillaTag.CosmeticSystem;

namespace GorillaTag
{
	// Token: 0x020011E7 RID: 4583
	public interface ISpawnable
	{
		// Token: 0x17000B4C RID: 2892
		// (get) Token: 0x0600747F RID: 29823
		// (set) Token: 0x06007480 RID: 29824
		bool IsSpawned { get; set; }

		// Token: 0x17000B4D RID: 2893
		// (get) Token: 0x06007481 RID: 29825
		// (set) Token: 0x06007482 RID: 29826
		ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x06007483 RID: 29827
		void OnSpawn(VRRig rig);

		// Token: 0x06007484 RID: 29828
		void OnDespawn();
	}
}
