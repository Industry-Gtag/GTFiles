using System;
using UnityEngine;

// Token: 0x02000A4D RID: 2637
public class CustomMapAccessDoor : MonoBehaviour
{
	// Token: 0x060043AF RID: 17327 RVA: 0x0016868E File Offset: 0x0016688E
	public void OpenDoor()
	{
		if (this.openDoorObject != null)
		{
			this.openDoorObject.SetActive(true);
		}
		if (this.closedDoorObject != null)
		{
			this.closedDoorObject.SetActive(false);
		}
	}

	// Token: 0x060043B0 RID: 17328 RVA: 0x001686C4 File Offset: 0x001668C4
	public void CloseDoor()
	{
		if (this.openDoorObject != null)
		{
			this.openDoorObject.SetActive(false);
		}
		if (this.closedDoorObject != null)
		{
			this.closedDoorObject.SetActive(true);
		}
	}

	// Token: 0x040055B4 RID: 21940
	public GameObject openDoorObject;

	// Token: 0x040055B5 RID: 21941
	public GameObject closedDoorObject;
}
