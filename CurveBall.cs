using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000022 RID: 34
public class CurveBall : MonoBehaviour
{
	// Token: 0x06000085 RID: 133 RVA: 0x00004580 File Offset: 0x00002780
	public void Reset()
	{
		float num = Random.Range(0f, MathUtil.TwoPi);
		float num2 = Mathf.Cos(num);
		float num3 = Mathf.Sin(num);
		this.m_speedX = 40f * num2;
		this.m_speedZ = 40f * num3;
		this.m_timer = 0f;
		Vector3 position = base.transform.position;
		position.x = -10f * num2;
		position.z = -10f * num3;
		base.transform.position = position;
	}

	// Token: 0x06000086 RID: 134 RVA: 0x00004602 File Offset: 0x00002802
	public void Start()
	{
		this.Reset();
	}

	// Token: 0x06000087 RID: 135 RVA: 0x0000460C File Offset: 0x0000280C
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.m_timer > this.Interval)
		{
			this.Reset();
		}
		Vector3 position = base.transform.position;
		position.x += this.m_speedX * deltaTime;
		position.z += this.m_speedZ * deltaTime;
		base.transform.position = position;
		this.m_timer += deltaTime;
	}

	// Token: 0x04000084 RID: 132
	public float Interval = 2f;

	// Token: 0x04000085 RID: 133
	private float m_speedX;

	// Token: 0x04000086 RID: 134
	private float m_speedZ;

	// Token: 0x04000087 RID: 135
	private float m_timer;
}
