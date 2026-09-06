using System;
using UnityEngine;

namespace Voxels
{
	// Token: 0x020013BA RID: 5050
	public static class VoxelEvents
	{
		// Token: 0x140000CC RID: 204
		// (add) Token: 0x06007E2E RID: 32302 RVA: 0x00295C68 File Offset: 0x00293E68
		// (remove) Token: 0x06007E2F RID: 32303 RVA: 0x00295C9C File Offset: 0x00293E9C
		public static event VoxelEvents.ResourcesMinedAuthorityDelegate OnResourcesMinedAuthority;

		// Token: 0x140000CD RID: 205
		// (add) Token: 0x06007E30 RID: 32304 RVA: 0x00295CD0 File Offset: 0x00293ED0
		// (remove) Token: 0x06007E31 RID: 32305 RVA: 0x00295D04 File Offset: 0x00293F04
		public static event VoxelEvents.ResourcesMinedDelegate OnResourcesMined;

		// Token: 0x06007E32 RID: 32306 RVA: 0x00295D37 File Offset: 0x00293F37
		public static void HandleResourceMinedAuthority(NetPlayer player, VoxelWorld world, int[] amounts)
		{
			VoxelEvents.ResourcesMinedAuthorityDelegate onResourcesMinedAuthority = VoxelEvents.OnResourcesMinedAuthority;
			if (onResourcesMinedAuthority == null)
			{
				return;
			}
			onResourcesMinedAuthority(player, world, amounts);
		}

		// Token: 0x06007E33 RID: 32307 RVA: 0x00295D4B File Offset: 0x00293F4B
		public static void HandleResourceMined(VoxelWorld world, Vector3 hitPoint, Vector3 hitNormal, int[] amounts)
		{
			world.MaterialSet.PlayDigFX(hitPoint, hitNormal, amounts);
			VoxelEvents.ResourcesMinedDelegate onResourcesMined = VoxelEvents.OnResourcesMined;
			if (onResourcesMined == null)
			{
				return;
			}
			onResourcesMined(world, hitPoint, hitNormal, amounts);
		}

		// Token: 0x020013BB RID: 5051
		// (Invoke) Token: 0x06007E35 RID: 32309
		public delegate void ResourcesMinedAuthorityDelegate(NetPlayer player, VoxelWorld world, int[] amounts);

		// Token: 0x020013BC RID: 5052
		// (Invoke) Token: 0x06007E39 RID: 32313
		public delegate void ResourcesMinedDelegate(VoxelWorld world, Vector3 hitPoint, Vector3 hitNormal, int[] amounts);
	}
}
