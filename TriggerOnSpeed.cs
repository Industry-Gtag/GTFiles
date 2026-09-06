using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000302 RID: 770
public class TriggerOnSpeed : MonoBehaviour, ITickSystemTick
{
	// Token: 0x060013A3 RID: 5027 RVA: 0x00041CF3 File Offset: 0x0003FEF3
	private void OnEnable()
	{
		TickSystem<object>.AddCallbackTarget(this);
	}

	// Token: 0x060013A4 RID: 5028 RVA: 0x00041CFB File Offset: 0x0003FEFB
	private void OnDisable()
	{
		TickSystem<object>.RemoveCallbackTarget(this);
	}

	// Token: 0x060013A5 RID: 5029 RVA: 0x00067C2C File Offset: 0x00065E2C
	public void Tick()
	{
		bool flag = this.velocityEstimator.linearVelocity.IsLongerThan(this.speedThreshold);
		if (flag != this.wasFaster)
		{
			if (flag)
			{
				this.onFaster.Invoke();
			}
			else
			{
				this.onSlower.Invoke();
			}
			this.wasFaster = flag;
		}
	}

	// Token: 0x170001F2 RID: 498
	// (get) Token: 0x060013A6 RID: 5030 RVA: 0x00067C7B File Offset: 0x00065E7B
	// (set) Token: 0x060013A7 RID: 5031 RVA: 0x00067C83 File Offset: 0x00065E83
	public bool TickRunning { get; set; }

	// Token: 0x04001814 RID: 6164
	[SerializeField]
	private float speedThreshold;

	// Token: 0x04001815 RID: 6165
	[SerializeField]
	private UnityEvent onFaster;

	// Token: 0x04001816 RID: 6166
	[SerializeField]
	private UnityEvent onSlower;

	// Token: 0x04001817 RID: 6167
	[SerializeField]
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04001818 RID: 6168
	private bool wasFaster;
}
