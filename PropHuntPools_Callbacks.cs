using System;

// Token: 0x02000280 RID: 640
internal class PropHuntPools_Callbacks
{
	// Token: 0x06001155 RID: 4437 RVA: 0x0005D406 File Offset: 0x0005B606
	internal void ListenForZoneChanged()
	{
		if (PropHuntPools_Callbacks._isListeningForZoneChanged)
		{
			return;
		}
		ZoneManagement.OnZoneChange += this._OnZoneChanged;
	}

	// Token: 0x06001156 RID: 4438 RVA: 0x0005D424 File Offset: 0x0005B624
	private void _OnZoneChanged(ZoneData[] zoneDatas)
	{
		if (VRRigCache.Instance == null || VRRigCache.Instance.localRig == null || VRRigCache.Instance.localRig.Rig == null || VRRigCache.Instance.localRig.Rig.zoneEntity.currentZone != GTZone.bayou)
		{
			return;
		}
		PropHuntPools_Callbacks._isListeningForZoneChanged = false;
		ZoneManagement.OnZoneChange -= this._OnZoneChanged;
		PropHuntPools.OnLocalPlayerEnteredBayou();
	}

	// Token: 0x040014A6 RID: 5286
	private const string preLog = "PropHuntPools_Callbacks: ";

	// Token: 0x040014A7 RID: 5287
	private const string preLogEd = "(editor only log) PropHuntPools_Callbacks: ";

	// Token: 0x040014A8 RID: 5288
	private const string preLogBeta = "(beta only log) PropHuntPools_Callbacks: ";

	// Token: 0x040014A9 RID: 5289
	private const string preErr = "ERROR!!!  PropHuntPools_Callbacks: ";

	// Token: 0x040014AA RID: 5290
	private const string preErrEd = "ERROR!!!  (editor only log) PropHuntPools_Callbacks: ";

	// Token: 0x040014AB RID: 5291
	private const string preErrBeta = "ERROR!!!  (beta only log) PropHuntPools_Callbacks: ";

	// Token: 0x040014AC RID: 5292
	internal static readonly PropHuntPools_Callbacks instance = new PropHuntPools_Callbacks();

	// Token: 0x040014AD RID: 5293
	private static bool _isListeningForZoneChanged;
}
