using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200066E RID: 1646
public class BuilderTriggerEnable : MonoBehaviour
{
	// Token: 0x0600290B RID: 10507 RVA: 0x000DED1C File Offset: 0x000DCF1C
	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null || component.OwningNetPlayer == null)
		{
			return;
		}
		if (!component.OwningNetPlayer.IsLocal)
		{
			return;
		}
		if (this.activateOnEnter != null)
		{
			for (int i = 0; i < this.activateOnEnter.Count; i++)
			{
				if (this.activateOnEnter[i] != null)
				{
					this.activateOnEnter[i].SetActive(true);
				}
			}
		}
		if (this.deactivateOnEnter != null)
		{
			for (int j = 0; j < this.deactivateOnEnter.Count; j++)
			{
				if (this.deactivateOnEnter[j] != null)
				{
					this.deactivateOnEnter[j].SetActive(false);
				}
			}
		}
	}

	// Token: 0x0600290C RID: 10508 RVA: 0x000DEDF4 File Offset: 0x000DCFF4
	private void OnTriggerExit(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null || component.OwningNetPlayer == null)
		{
			return;
		}
		if (!component.OwningNetPlayer.IsLocal)
		{
			return;
		}
		if (this.activateOnExit != null)
		{
			for (int i = 0; i < this.activateOnExit.Count; i++)
			{
				if (this.activateOnExit[i] != null)
				{
					this.activateOnExit[i].SetActive(true);
				}
			}
		}
		if (this.deactivateOnExit != null)
		{
			for (int j = 0; j < this.deactivateOnExit.Count; j++)
			{
				if (this.deactivateOnExit[j] != null)
				{
					this.deactivateOnExit[j].SetActive(false);
				}
			}
		}
	}

	// Token: 0x04003576 RID: 13686
	public List<GameObject> activateOnEnter;

	// Token: 0x04003577 RID: 13687
	public List<GameObject> deactivateOnEnter;

	// Token: 0x04003578 RID: 13688
	public List<GameObject> activateOnExit;

	// Token: 0x04003579 RID: 13689
	public List<GameObject> deactivateOnExit;
}
