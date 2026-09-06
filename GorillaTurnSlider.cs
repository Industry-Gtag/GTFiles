using System;
using UnityEngine;

// Token: 0x02000A40 RID: 2624
public class GorillaTurnSlider : MonoBehaviour
{
	// Token: 0x0600435E RID: 17246 RVA: 0x001668A1 File Offset: 0x00164AA1
	private void Awake()
	{
		this.startingLocation = base.transform.position;
		this.SetPosition(this.gorillaTurn.currentSpeed);
	}

	// Token: 0x0600435F RID: 17247 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void FixedUpdate()
	{
	}

	// Token: 0x06004360 RID: 17248 RVA: 0x001668C8 File Offset: 0x00164AC8
	public void SetPosition(float speed)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		float num3 = (speed - this.minValue) * (num2 - num) / (this.maxValue - this.minValue) + num;
		base.transform.position = new Vector3(num3, this.startingLocation.y, this.startingLocation.z);
	}

	// Token: 0x06004361 RID: 17249 RVA: 0x0016694C File Offset: 0x00164B4C
	public float InterpolateValue(float value)
	{
		float num = this.startingLocation.x - this.zRange / 2f;
		float num2 = this.startingLocation.x + this.zRange / 2f;
		return (value - num) / (num2 - num) * (this.maxValue - this.minValue) + this.minValue;
	}

	// Token: 0x06004362 RID: 17250 RVA: 0x001669A8 File Offset: 0x00164BA8
	public void OnSliderRelease()
	{
		if (this.zRange != 0f && (base.transform.position - this.startingLocation).magnitude > this.zRange / 2f)
		{
			if (base.transform.position.x > this.startingLocation.x)
			{
				base.transform.position = new Vector3(this.startingLocation.x + this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
				return;
			}
			base.transform.position = new Vector3(this.startingLocation.x - this.zRange / 2f, this.startingLocation.y, this.startingLocation.z);
		}
	}

	// Token: 0x04005542 RID: 21826
	public float zRange;

	// Token: 0x04005543 RID: 21827
	public float maxValue;

	// Token: 0x04005544 RID: 21828
	public float minValue;

	// Token: 0x04005545 RID: 21829
	public GorillaTurning gorillaTurn;

	// Token: 0x04005546 RID: 21830
	private float startingZ;

	// Token: 0x04005547 RID: 21831
	public Vector3 startingLocation;
}
