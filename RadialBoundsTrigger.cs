using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020009D9 RID: 2521
public class RadialBoundsTrigger : MonoBehaviour
{
	// Token: 0x060040B0 RID: 16560 RVA: 0x00158812 File Offset: 0x00156A12
	public void TestOverlap()
	{
		this.TestOverlap(this._raiseEvents);
	}

	// Token: 0x060040B1 RID: 16561 RVA: 0x00158820 File Offset: 0x00156A20
	public void TestOverlap(bool raiseEvents)
	{
		if (!this.object1 || !this.object2)
		{
			this._overlapping = false;
			this._timeOverlapStarted = -1f;
			this._timeOverlapStopped = -1f;
			this._timeSpentInOverlap = 0f;
			return;
		}
		float time = Time.time;
		float num = this.object1.radius + this.object2.radius;
		bool flag = (this.object2.center - this.object1.center).sqrMagnitude <= num * num;
		if (this._overlapping && flag)
		{
			this._overlapping = true;
			this._timeSpentInOverlap = time - this._timeOverlapStarted;
			if (raiseEvents)
			{
				UnityEvent<RadialBounds, float> onOverlapStay = this.object1.onOverlapStay;
				if (onOverlapStay != null)
				{
					onOverlapStay.Invoke(this.object2, this._timeSpentInOverlap);
				}
				UnityEvent<RadialBounds, float> onOverlapStay2 = this.object2.onOverlapStay;
				if (onOverlapStay2 == null)
				{
					return;
				}
				onOverlapStay2.Invoke(this.object1, this._timeSpentInOverlap);
				return;
			}
		}
		else if (!this._overlapping && flag)
		{
			if (time - this._timeOverlapStopped < this.hysteresis)
			{
				return;
			}
			this._overlapping = true;
			this._timeOverlapStarted = time;
			this._timeOverlapStopped = -1f;
			this._timeSpentInOverlap = 0f;
			if (raiseEvents)
			{
				UnityEvent<RadialBounds> onOverlapEnter = this.object1.onOverlapEnter;
				if (onOverlapEnter != null)
				{
					onOverlapEnter.Invoke(this.object2);
				}
				UnityEvent<RadialBounds> onOverlapEnter2 = this.object2.onOverlapEnter;
				if (onOverlapEnter2 == null)
				{
					return;
				}
				onOverlapEnter2.Invoke(this.object1);
				return;
			}
		}
		else if (!flag && this._overlapping)
		{
			this._overlapping = false;
			this._timeOverlapStarted = -1f;
			this._timeOverlapStopped = time;
			this._timeSpentInOverlap = 0f;
			if (raiseEvents)
			{
				UnityEvent<RadialBounds> onOverlapExit = this.object1.onOverlapExit;
				if (onOverlapExit != null)
				{
					onOverlapExit.Invoke(this.object2);
				}
				UnityEvent<RadialBounds> onOverlapExit2 = this.object2.onOverlapExit;
				if (onOverlapExit2 == null)
				{
					return;
				}
				onOverlapExit2.Invoke(this.object1);
			}
		}
	}

	// Token: 0x060040B2 RID: 16562 RVA: 0x00158A0C File Offset: 0x00156C0C
	private void FixedUpdate()
	{
		this.TestOverlap();
	}

	// Token: 0x060040B3 RID: 16563 RVA: 0x00158A14 File Offset: 0x00156C14
	private void OnDisable()
	{
		if (this._raiseEvents && this.object1 && this.object2 && this._overlapping)
		{
			UnityEvent<RadialBounds> onOverlapExit = this.object1.onOverlapExit;
			if (onOverlapExit != null)
			{
				onOverlapExit.Invoke(this.object2);
			}
			UnityEvent<RadialBounds> onOverlapExit2 = this.object2.onOverlapExit;
			if (onOverlapExit2 != null)
			{
				onOverlapExit2.Invoke(this.object1);
			}
		}
		this._timeOverlapStarted = -1f;
		this._timeSpentInOverlap = 0f;
		this._overlapping = false;
	}

	// Token: 0x04005133 RID: 20787
	[SerializeField]
	private Id32 _triggerID;

	// Token: 0x04005134 RID: 20788
	[Space]
	public RadialBounds object1 = new RadialBounds();

	// Token: 0x04005135 RID: 20789
	[Space]
	public RadialBounds object2 = new RadialBounds();

	// Token: 0x04005136 RID: 20790
	[Space]
	public float hysteresis = 0.5f;

	// Token: 0x04005137 RID: 20791
	[SerializeField]
	private bool _raiseEvents = true;

	// Token: 0x04005138 RID: 20792
	[Space]
	private bool _overlapping;

	// Token: 0x04005139 RID: 20793
	private float _timeSpentInOverlap;

	// Token: 0x0400513A RID: 20794
	[Space]
	private float _timeOverlapStarted;

	// Token: 0x0400513B RID: 20795
	private float _timeOverlapStopped;
}
