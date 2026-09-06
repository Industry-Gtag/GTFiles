using System;
using UnityEngine;

// Token: 0x02000A1F RID: 2591
public class GorillaColorSlider : MonoBehaviour
{
	// Token: 0x0600426F RID: 17007 RVA: 0x00162551 File Offset: 0x00160751
	private void Start()
	{
		if (!this.setRandomly)
		{
			this.startingLocation = base.transform.position;
		}
	}

	// Token: 0x06004270 RID: 17008 RVA: 0x0016256C File Offset: 0x0016076C
	public void SetPosition(float speed)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		float num3 = (speed - this.minValue) * (num2 - num) / (this.maxValue - this.minValue) + num;
		base.transform.position = new Vector3(num3, this.startingLocation.y, this.startingLocation.z);
		this.valueImReporting = this.InterpolateValue(base.transform.position.x);
	}

	// Token: 0x06004271 RID: 17009 RVA: 0x0016260C File Offset: 0x0016080C
	public float InterpolateValue(float value)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		return (value - num) / (num2 - num) * (this.maxValue - this.minValue) + this.minValue;
	}

	// Token: 0x06004272 RID: 17010 RVA: 0x00162668 File Offset: 0x00160868
	public void OnSliderRelease()
	{
		if (this.zRange != 0f && (base.transform.position - this.startingLocation).magnitude > this.zRange / 2f)
		{
			if (base.transform.position.x > this.startingLocation.x)
			{
				base.transform.position = new Vector3(this.startingLocation.x + this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
			}
			else
			{
				base.transform.position = new Vector3(this.startingLocation.x - this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
			}
		}
		this.valueImReporting = this.InterpolateValue(base.transform.position.x);
	}

	// Token: 0x04005420 RID: 21536
	public bool setRandomly;

	// Token: 0x04005421 RID: 21537
	public float zRange;

	// Token: 0x04005422 RID: 21538
	public float maxValue;

	// Token: 0x04005423 RID: 21539
	public float minValue;

	// Token: 0x04005424 RID: 21540
	public Vector3 startingLocation;

	// Token: 0x04005425 RID: 21541
	public int valueIndex;

	// Token: 0x04005426 RID: 21542
	public float valueImReporting;

	// Token: 0x04005427 RID: 21543
	public GorillaTriggerBox gorilla;

	// Token: 0x04005428 RID: 21544
	private float startingZ;
}
