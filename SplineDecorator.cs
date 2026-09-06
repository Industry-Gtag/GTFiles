using System;
using UnityEngine;

// Token: 0x02000DE2 RID: 3554
public class SplineDecorator : MonoBehaviour
{
	// Token: 0x0600571C RID: 22300 RVA: 0x001C78F8 File Offset: 0x001C5AF8
	private void Awake()
	{
		if (this.frequency <= 0 || this.items == null || this.items.Length == 0)
		{
			return;
		}
		float num = (float)(this.frequency * this.items.Length);
		if (this.spline.Loop || num == 1f)
		{
			num = 1f / num;
		}
		else
		{
			num = 1f / (num - 1f);
		}
		int num2 = 0;
		for (int i = 0; i < this.frequency; i++)
		{
			int j = 0;
			while (j < this.items.Length)
			{
				Transform transform = Object.Instantiate<Transform>(this.items[j]);
				Vector3 point = this.spline.GetPoint((float)num2 * num);
				transform.transform.localPosition = point;
				if (this.lookForward)
				{
					transform.transform.LookAt(point + this.spline.GetDirection((float)num2 * num));
				}
				transform.transform.parent = base.transform;
				j++;
				num2++;
			}
		}
	}

	// Token: 0x040067E1 RID: 26593
	public BezierSpline spline;

	// Token: 0x040067E2 RID: 26594
	public int frequency;

	// Token: 0x040067E3 RID: 26595
	public bool lookForward;

	// Token: 0x040067E4 RID: 26596
	public Transform[] items;
}
