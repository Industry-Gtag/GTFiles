using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000029 RID: 41
public class RotationStepper : MonoBehaviour
{
	// Token: 0x0600009B RID: 155 RVA: 0x00005289 File Offset: 0x00003489
	public void OnEnable()
	{
		this.m_phase = 0f;
		Random.InitState(0);
	}

	// Token: 0x0600009C RID: 156 RVA: 0x0000529C File Offset: 0x0000349C
	public void Update()
	{
		this.m_phase += this.Frequency * Time.deltaTime;
		RotationStepper.ModeEnum mode = this.Mode;
		if (mode == RotationStepper.ModeEnum.Fixed)
		{
			base.transform.rotation = Quaternion.Euler(0f, 0f, (Mathf.Repeat(this.m_phase, 2f) < 1f) ? (-25f) : 25f);
			return;
		}
		if (mode != RotationStepper.ModeEnum.Random)
		{
			return;
		}
		while (this.m_phase >= 1f)
		{
			Random.InitState(Time.frameCount);
			base.transform.rotation = Random.rotationUniform;
			this.m_phase -= 1f;
		}
	}

	// Token: 0x040000BA RID: 186
	public RotationStepper.ModeEnum Mode;

	// Token: 0x040000BB RID: 187
	[ConditionalField("Mode", RotationStepper.ModeEnum.Fixed, null, null, null, null, null)]
	public float Angle = 25f;

	// Token: 0x040000BC RID: 188
	public float Frequency;

	// Token: 0x040000BD RID: 189
	private float m_phase;

	// Token: 0x0200002A RID: 42
	public enum ModeEnum
	{
		// Token: 0x040000BF RID: 191
		Fixed,
		// Token: 0x040000C0 RID: 192
		Random
	}
}
