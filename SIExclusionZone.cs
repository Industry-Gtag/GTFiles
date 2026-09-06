using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200013F RID: 319
public class SIExclusionZone : MonoBehaviour
{
	// Token: 0x060007EA RID: 2026 RVA: 0x0002B3C8 File Offset: 0x000295C8
	private void OnDisable()
	{
		foreach (SIGadget sigadget in this.gadgetsInZone)
		{
			if (sigadget != null)
			{
				sigadget.LeaveExclusionZone(this);
			}
		}
		this.gadgetsInZone.Clear();
		if ((this.exclusionType & SIExclusionType.AffectsOthers) != (SIExclusionType)0)
		{
			foreach (SIPlayer siplayer in this.playersInZone)
			{
				if (siplayer != null)
				{
					siplayer.exclusionZoneCount--;
				}
			}
		}
		this.playersInZone.Clear();
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x0002B498 File Offset: 0x00029698
	private void OnTriggerEnter(Collider other)
	{
		SIGadget componentInParent = other.GetComponentInParent<SIGadget>();
		if (componentInParent != null)
		{
			if (!this.gadgetsInZone.Contains(componentInParent))
			{
				this.gadgetsInZone.Add(componentInParent);
			}
			componentInParent.ApplyExclusionZone(this);
		}
		SIPlayer componentInParent2 = other.GetComponentInParent<SIPlayer>();
		if (componentInParent2 != null && !this.playersInZone.Contains(componentInParent2))
		{
			this.playersInZone.Add(componentInParent2);
			if ((this.exclusionType & SIExclusionType.AffectsOthers) != (SIExclusionType)0)
			{
				componentInParent2.exclusionZoneCount++;
			}
		}
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x0002B518 File Offset: 0x00029718
	private void OnTriggerExit(Collider other)
	{
		SIGadget componentInParent = other.GetComponentInParent<SIGadget>();
		if (componentInParent != null && this.gadgetsInZone.Contains(componentInParent))
		{
			componentInParent.LeaveExclusionZone(this);
			this.gadgetsInZone.Remove(componentInParent);
		}
		SIPlayer componentInParent2 = other.GetComponentInParent<SIPlayer>();
		if (componentInParent2 != null && this.playersInZone.Contains(componentInParent2))
		{
			this.playersInZone.Remove(componentInParent2);
			if ((this.exclusionType & SIExclusionType.AffectsOthers) != (SIExclusionType)0)
			{
				componentInParent2.exclusionZoneCount--;
			}
		}
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x0002B59A File Offset: 0x0002979A
	public void ClearGadget(SIGadget gadget)
	{
		this.gadgetsInZone.Remove(gadget);
	}

	// Token: 0x04000A04 RID: 2564
	public SIExclusionType exclusionType = SIExclusionType.AffectsOthers;

	// Token: 0x04000A05 RID: 2565
	private List<SIGadget> gadgetsInZone = new List<SIGadget>();

	// Token: 0x04000A06 RID: 2566
	private List<SIPlayer> playersInZone = new List<SIPlayer>();
}
