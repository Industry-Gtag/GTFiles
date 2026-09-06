using System;
using UnityEngine;

// Token: 0x0200019F RID: 415
public class Pendulum : MonoBehaviour
{
	// Token: 0x06000B2D RID: 2861 RVA: 0x0003BE8C File Offset: 0x0003A08C
	private void Start()
	{
		this.pendulum = (this.ClockPendulum = base.gameObject.GetComponent<Transform>());
	}

	// Token: 0x06000B2E RID: 2862 RVA: 0x0003BEB4 File Offset: 0x0003A0B4
	private void Update()
	{
		if (this.pendulum)
		{
			float num = this.MaxAngleDeflection * Mathf.Sin(Time.time * this.SpeedOfPendulum);
			this.pendulum.localRotation = Quaternion.Euler(0f, 0f, num);
			return;
		}
	}

	// Token: 0x04000D71 RID: 3441
	public float MaxAngleDeflection = 10f;

	// Token: 0x04000D72 RID: 3442
	public float SpeedOfPendulum = 1f;

	// Token: 0x04000D73 RID: 3443
	public Transform ClockPendulum;

	// Token: 0x04000D74 RID: 3444
	private Transform pendulum;
}
