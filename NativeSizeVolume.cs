using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200042C RID: 1068
public class NativeSizeVolume : MonoBehaviour
{
	// Token: 0x0600195D RID: 6493 RVA: 0x0008E9FC File Offset: 0x0008CBFC
	private void OnTriggerEnter(Collider other)
	{
		GTPlayer componentInParent = other.GetComponentInParent<GTPlayer>();
		if (componentInParent == null)
		{
			return;
		}
		NativeSizeVolume.NativeSizeVolumeAction onEnterAction = this.OnEnterAction;
		if (onEnterAction == NativeSizeVolume.NativeSizeVolumeAction.ApplySettings)
		{
			this.settings.WorldPosition = base.transform.position;
			componentInParent.SetNativeScale(this.settings);
			return;
		}
		if (onEnterAction != NativeSizeVolume.NativeSizeVolumeAction.ResetSize)
		{
			return;
		}
		componentInParent.SetNativeScale(null);
	}

	// Token: 0x0600195E RID: 6494 RVA: 0x0008EA54 File Offset: 0x0008CC54
	private void OnTriggerExit(Collider other)
	{
		GTPlayer componentInParent = other.GetComponentInParent<GTPlayer>();
		if (componentInParent == null)
		{
			return;
		}
		NativeSizeVolume.NativeSizeVolumeAction onExitAction = this.OnExitAction;
		if (onExitAction == NativeSizeVolume.NativeSizeVolumeAction.ApplySettings)
		{
			this.settings.WorldPosition = base.transform.position;
			componentInParent.SetNativeScale(this.settings);
			return;
		}
		if (onExitAction != NativeSizeVolume.NativeSizeVolumeAction.ResetSize)
		{
			return;
		}
		componentInParent.SetNativeScale(null);
	}

	// Token: 0x0400246A RID: 9322
	[SerializeField]
	private Collider triggerVolume;

	// Token: 0x0400246B RID: 9323
	[SerializeField]
	private NativeSizeChangerSettings settings;

	// Token: 0x0400246C RID: 9324
	[SerializeField]
	private NativeSizeVolume.NativeSizeVolumeAction OnEnterAction;

	// Token: 0x0400246D RID: 9325
	[SerializeField]
	private NativeSizeVolume.NativeSizeVolumeAction OnExitAction;

	// Token: 0x0200042D RID: 1069
	[Serializable]
	private enum NativeSizeVolumeAction
	{
		// Token: 0x0400246F RID: 9327
		None,
		// Token: 0x04002470 RID: 9328
		ApplySettings,
		// Token: 0x04002471 RID: 9329
		ResetSize
	}
}
