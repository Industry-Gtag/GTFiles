using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000144 RID: 324
public class SIHandScannerTrigger : MonoBehaviour, IClickable
{
	// Token: 0x06000819 RID: 2073 RVA: 0x0002C6F8 File Offset: 0x0002A8F8
	private void Awake()
	{
		if (this.parentScanner == null)
		{
			this.parentScanner = base.GetComponentInParent<SIHandScanner>();
		}
	}

	// Token: 0x0600081A RID: 2074 RVA: 0x0002C714 File Offset: 0x0002A914
	private void OnTriggerEnter(Collider other)
	{
		SIScannableHand component = other.GetComponent<SIScannableHand>();
		if (component == null)
		{
			return;
		}
		this.OnPlayerScanned(component.parentPlayer);
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x0002C73E File Offset: 0x0002A93E
	private void OnPlayerScanned(SIPlayer player)
	{
		this.parentScanner.HandScanned(player);
		this.onHandScanned.Invoke();
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x0002C757 File Offset: 0x0002A957
	public void Click(bool leftHand = false)
	{
		this.OnPlayerScanned(VRRig.LocalRig.GetComponent<SIPlayer>());
	}

	// Token: 0x04000A3B RID: 2619
	public SIHandScanner parentScanner;

	// Token: 0x04000A3C RID: 2620
	public UnityEvent onHandScanned;
}
