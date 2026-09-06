using System;
using UnityEngine;

// Token: 0x0200008E RID: 142
public class PeriodicFoodTopUpper : MonoBehaviour
{
	// Token: 0x0600038E RID: 910 RVA: 0x00014C0C File Offset: 0x00012E0C
	private void Awake()
	{
		this.food = base.GetComponentInParent<CrittersFood>();
	}

	// Token: 0x0600038F RID: 911 RVA: 0x00014C1C File Offset: 0x00012E1C
	private void Update()
	{
		if (!CrittersManager.instance.LocalAuthority())
		{
			return;
		}
		if (!this.waitingToRefill && this.food.currentFood == 0f)
		{
			this.waitingToRefill = true;
			this.timeFoodEmpty = Time.time;
		}
		if (this.waitingToRefill && Time.time > this.timeFoodEmpty + this.waitToRefill)
		{
			this.waitingToRefill = false;
			this.food.Initialize();
		}
	}

	// Token: 0x0400040E RID: 1038
	private CrittersFood food;

	// Token: 0x0400040F RID: 1039
	private float timeFoodEmpty;

	// Token: 0x04000410 RID: 1040
	private bool waitingToRefill;

	// Token: 0x04000411 RID: 1041
	public float waitToRefill = 10f;

	// Token: 0x04000412 RID: 1042
	public GameObject foodObject;
}
