using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000429 RID: 1065
public class NativeSizeChanger : MonoBehaviour
{
	// Token: 0x06001954 RID: 6484 RVA: 0x0008E97D File Offset: 0x0008CB7D
	public void Activate(NativeSizeChangerSettings settings)
	{
		settings.WorldPosition = base.transform.position;
		settings.ActivationTime = Time.time;
		GTPlayer.Instance.SetNativeScale(settings);
	}
}
