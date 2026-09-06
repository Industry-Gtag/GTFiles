using System;

// Token: 0x020000F7 RID: 247
public static class EAssetReleaseTier_Extensions
{
	// Token: 0x060005F0 RID: 1520 RVA: 0x000226BD File Offset: 0x000208BD
	public static bool ShouldIncludeInBuild(this EAssetReleaseTier assetTier, EBuildReleaseTier buildTier)
	{
		return assetTier != EAssetReleaseTier.Disabled && assetTier <= (EAssetReleaseTier)buildTier;
	}
}
