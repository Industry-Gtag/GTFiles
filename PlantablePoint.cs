using System;
using UnityEngine;

// Token: 0x02000367 RID: 871
public class PlantablePoint : MonoBehaviour
{
	// Token: 0x0600155A RID: 5466 RVA: 0x0007191A File Offset: 0x0006FB1A
	private void OnTriggerEnter(Collider other)
	{
		if ((this.floorMask & (1 << other.gameObject.layer)) != 0)
		{
			this.plantableObject.SetPlanted(true);
		}
	}

	// Token: 0x0600155B RID: 5467 RVA: 0x00071946 File Offset: 0x0006FB46
	public void OnTriggerExit(Collider other)
	{
		if ((this.floorMask & (1 << other.gameObject.layer)) != 0)
		{
			this.plantableObject.SetPlanted(false);
		}
	}

	// Token: 0x04001A33 RID: 6707
	public bool shouldBeSet;

	// Token: 0x04001A34 RID: 6708
	public LayerMask floorMask;

	// Token: 0x04001A35 RID: 6709
	public PlantableObject plantableObject;
}
