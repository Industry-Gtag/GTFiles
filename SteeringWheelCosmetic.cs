using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000CE RID: 206
public class SteeringWheelCosmetic : MonoBehaviour
{
	// Token: 0x060004FA RID: 1274 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Start()
	{
	}

	// Token: 0x060004FB RID: 1275 RVA: 0x0001BB6A File Offset: 0x00019D6A
	public void TryHornHit()
	{
		if (Time.time > this.lastHornTime + this.cooldown)
		{
			this.lastHornTime = Time.time;
			UnityEvent unityEvent = this.onHornHit;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x060004FC RID: 1276 RVA: 0x0001BB9C File Offset: 0x00019D9C
	private void Update()
	{
		float z = base.transform.localEulerAngles.z;
		if (Mathf.Abs(Mathf.DeltaAngle(this.lastZAngle, z)) >= this.dramaticTurnThreshold)
		{
			UnityEvent unityEvent = this.onDramaticTurn;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
		}
		this.lastZAngle = z;
	}

	// Token: 0x040005A4 RID: 1444
	[SerializeField]
	private float cooldown = 1.5f;

	// Token: 0x040005A5 RID: 1445
	[SerializeField]
	private float dramaticTurnThreshold = 35f;

	// Token: 0x040005A6 RID: 1446
	[SerializeField]
	private UnityEvent onHornHit;

	// Token: 0x040005A7 RID: 1447
	[SerializeField]
	private UnityEvent onDramaticTurn;

	// Token: 0x040005A8 RID: 1448
	private float lastHornTime = -999f;

	// Token: 0x040005A9 RID: 1449
	private float lastZAngle;
}
