using System;
using UnityEngine;

// Token: 0x02000E23 RID: 3619
public class move : MonoBehaviour
{
	// Token: 0x06005897 RID: 22679 RVA: 0x001CC144 File Offset: 0x001CA344
	private void Update()
	{
		if (this.bounce)
		{
			this.cnt++;
			if (this.cnt % 50 == 0)
			{
				this.direction = ((this.direction > 0) ? (-1) : 1);
				this.cnt = 1;
			}
			Vector3 vector = base.gameObject.transform.forward * ((float)this.direction * 1f);
			base.gameObject.transform.Translate(vector);
		}
	}

	// Token: 0x06005898 RID: 22680 RVA: 0x001CC1C0 File Offset: 0x001CA3C0
	public void BounceState(string state)
	{
		this.bounce = state == "1";
	}

	// Token: 0x040068C4 RID: 26820
	private int direction = 1;

	// Token: 0x040068C5 RID: 26821
	private int cnt;

	// Token: 0x040068C6 RID: 26822
	private bool bounce = true;
}
