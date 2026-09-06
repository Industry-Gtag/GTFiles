using System;
using UnityEngine;

// Token: 0x02000036 RID: 54
public class CameraShakeDispatcher : MonoBehaviour
{
	// Token: 0x060000CB RID: 203 RVA: 0x00005985 File Offset: 0x00003B85
	private void OnEnable()
	{
		if (this.shakeOnEnable)
		{
			if (this.maxDistance > 0f)
			{
				this.ShakeInProximity(this.maxDistance);
				return;
			}
			this.Shake();
		}
	}

	// Token: 0x060000CC RID: 204 RVA: 0x000059AF File Offset: 0x00003BAF
	private void OnDisable()
	{
		if (this.haltOnDisable)
		{
			this.Halt();
		}
	}

	// Token: 0x060000CD RID: 205 RVA: 0x000059BF File Offset: 0x00003BBF
	public void Shake()
	{
		CameraShaker.Shake(this.duration, this.magnitude, this.freqRange, this.rollOffOverDuration);
	}

	// Token: 0x060000CE RID: 206 RVA: 0x000059DE File Offset: 0x00003BDE
	public void ShakeInProximity(float distance)
	{
		CameraShaker.ShakeInProximity(this.duration, this.magnitude, this.freqRange, this.rollOffOverDuration, base.transform, distance);
	}

	// Token: 0x060000CF RID: 207 RVA: 0x00005A04 File Offset: 0x00003C04
	public void Halt()
	{
		CameraShaker.Halt();
	}

	// Token: 0x040000E2 RID: 226
	[SerializeField]
	private float magnitude = 1f;

	// Token: 0x040000E3 RID: 227
	[SerializeField]
	private float duration = 0.5f;

	// Token: 0x040000E4 RID: 228
	[SerializeField]
	private bool rollOffOverDuration = true;

	// Token: 0x040000E5 RID: 229
	[SerializeField]
	private bool shakeOnEnable;

	// Token: 0x040000E6 RID: 230
	[SerializeField]
	private bool haltOnDisable;

	// Token: 0x040000E7 RID: 231
	[SerializeField]
	private Vector2 freqRange = new Vector2(0.02f, 0.1f);

	// Token: 0x040000E8 RID: 232
	[SerializeField]
	private float maxDistance;
}
