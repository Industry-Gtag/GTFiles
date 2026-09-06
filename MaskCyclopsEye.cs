using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000BC RID: 188
public class MaskCyclopsEye : MonoBehaviour
{
	// Token: 0x06000494 RID: 1172 RVA: 0x00019F60 File Offset: 0x00018160
	private void OnEnable()
	{
		this.ScheduleNextBlink();
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnDisable()
	{
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x00019F68 File Offset: 0x00018168
	public void Update()
	{
		if (Time.time >= this.nextBlinkTime)
		{
			UnityEvent onBlink = this.OnBlink;
			if (onBlink != null)
			{
				onBlink.Invoke();
			}
			this.ScheduleNextBlink();
		}
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x00019F68 File Offset: 0x00018168
	public void Tick()
	{
		if (Time.time >= this.nextBlinkTime)
		{
			UnityEvent onBlink = this.OnBlink;
			if (onBlink != null)
			{
				onBlink.Invoke();
			}
			this.ScheduleNextBlink();
		}
	}

	// Token: 0x06000498 RID: 1176 RVA: 0x00019F90 File Offset: 0x00018190
	private void ScheduleNextBlink()
	{
		float num = Random.Range(this.minWaitTime, this.maxWaitTime);
		this.nextBlinkTime = Time.time + num;
	}

	// Token: 0x040004F2 RID: 1266
	[Tooltip("Invoked when it's time to trigger a blink (e.g., play animation one-shot).")]
	public UnityEvent OnBlink;

	// Token: 0x040004F3 RID: 1267
	[Tooltip("Minimum time in seconds between blinks.")]
	[SerializeField]
	private float minWaitTime = 3f;

	// Token: 0x040004F4 RID: 1268
	[Tooltip("Maximum time in seconds between blinks.")]
	[SerializeField]
	private float maxWaitTime = 5f;

	// Token: 0x040004F5 RID: 1269
	private float nextBlinkTime;
}
