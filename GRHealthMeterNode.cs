using System;
using UnityEngine;

// Token: 0x020007BF RID: 1983
public class GRHealthMeterNode : MonoBehaviour
{
	// Token: 0x060032AB RID: 12971 RVA: 0x00115CC0 File Offset: 0x00113EC0
	public void Setup()
	{
		this.isEmpty = true;
		this.SetEmpty(false);
	}

	// Token: 0x060032AC RID: 12972 RVA: 0x00115CD0 File Offset: 0x00113ED0
	public void SetEmpty(bool empty)
	{
		if (this.isEmpty == empty)
		{
			return;
		}
		this.isEmpty = empty;
		if (this.showFull != null)
		{
			this.showFull.SetActive(!this.isEmpty);
		}
		if (this.showEmpty != null)
		{
			this.showEmpty.SetActive(this.isEmpty);
		}
	}

	// Token: 0x040041AD RID: 16813
	public GameObject showFull;

	// Token: 0x040041AE RID: 16814
	public GameObject showEmpty;

	// Token: 0x040041AF RID: 16815
	private bool isEmpty;
}
