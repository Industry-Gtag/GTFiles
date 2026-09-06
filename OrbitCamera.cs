using System;
using BoingKit;
using UnityEngine;

// Token: 0x0200001F RID: 31
public class OrbitCamera : MonoBehaviour
{
	// Token: 0x06000077 RID: 119 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void Start()
	{
	}

	// Token: 0x06000078 RID: 120 RVA: 0x00003EEC File Offset: 0x000020EC
	public void Update()
	{
		this.m_phase += OrbitCamera.kOrbitSpeed * MathUtil.TwoPi * Time.deltaTime;
		base.transform.position = new Vector3(-4f * Mathf.Cos(this.m_phase), 6f, 4f * Mathf.Sin(this.m_phase));
		base.transform.rotation = Quaternion.LookRotation((new Vector3(0f, 3f, 0f) - base.transform.position).normalized);
	}

	// Token: 0x0400006E RID: 110
	private static readonly float kOrbitSpeed = 0.01f;

	// Token: 0x0400006F RID: 111
	private float m_phase;
}
