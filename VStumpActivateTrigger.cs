using System;
using GorillaLocomotion;
using GorillaTagScripts.VirtualStumpCustomMaps;
using UnityEngine;

// Token: 0x02000AD6 RID: 2774
public class VStumpActivateTrigger : MonoBehaviour
{
	// Token: 0x06004741 RID: 18241 RVA: 0x0018087D File Offset: 0x0017EA7D
	public void OnTriggerEnter(Collider other)
	{
		if (!this.armed)
		{
			return;
		}
		if (other != GTPlayer.Instance.headCollider)
		{
			return;
		}
		this.armed = false;
		CustomMapManager.Activate(this.mode, true);
	}

	// Token: 0x06004742 RID: 18242 RVA: 0x001808AE File Offset: 0x0017EAAE
	public void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			this.armed = true;
		}
	}

	// Token: 0x040059C4 RID: 22980
	[Tooltip("Which hallway this is: FeatureA -> featured map 0, FeatureB -> featured map 1, Custom -> open the stump with no auto-load.")]
	[SerializeField]
	private VirtualStumpActivateMode mode = VirtualStumpActivateMode.FeatureA;

	// Token: 0x040059C5 RID: 22981
	private bool armed = true;
}
