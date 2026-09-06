using System;
using UnityEngine;

// Token: 0x020000C0 RID: 192
public class CosmeticCritterButterfly : CosmeticCritter
{
	// Token: 0x1700005C RID: 92
	// (get) Token: 0x060004BA RID: 1210 RVA: 0x0001A8DF File Offset: 0x00018ADF
	public ParticleSystem.EmitParams GetEmitParams
	{
		get
		{
			return this.emitParams;
		}
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x0001A8E7 File Offset: 0x00018AE7
	public void SetStartPos(Vector3 initialPos)
	{
		this.startPosition = initialPos;
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x0001A8F0 File Offset: 0x00018AF0
	public override void SetRandomVariables()
	{
		this.direction = Random.insideUnitSphere;
		this.emitParams.startColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
		this.particleSystem.Emit(this.emitParams, 1);
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x0001A94D File Offset: 0x00018B4D
	public override void Tick()
	{
		base.transform.position = this.startPosition + (float)base.GetAliveTime() * this.speed * this.direction;
	}

	// Token: 0x04000537 RID: 1335
	[Tooltip("The speed this Butterfly will move at.")]
	[SerializeField]
	private float speed = 1f;

	// Token: 0x04000538 RID: 1336
	[Tooltip("Emit one particle from this particle system when spawning.")]
	[SerializeField]
	private ParticleSystem particleSystem;

	// Token: 0x04000539 RID: 1337
	private Vector3 startPosition;

	// Token: 0x0400053A RID: 1338
	private Vector3 direction;

	// Token: 0x0400053B RID: 1339
	private ParticleSystem.EmitParams emitParams;
}
