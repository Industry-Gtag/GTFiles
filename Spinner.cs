using System;
using UnityEngine;

// Token: 0x0200002B RID: 43
public class Spinner : MonoBehaviour
{
	// Token: 0x0600009E RID: 158 RVA: 0x0000535B File Offset: 0x0000355B
	public void OnEnable()
	{
		this.m_angle = Random.Range(0f, 360f);
	}

	// Token: 0x0600009F RID: 159 RVA: 0x00005374 File Offset: 0x00003574
	public void Update()
	{
		this.m_angle += this.Speed * 360f * Time.deltaTime;
		base.transform.rotation = Quaternion.Euler(0f, -this.m_angle, 0f);
	}

	// Token: 0x040000C1 RID: 193
	public float Speed;

	// Token: 0x040000C2 RID: 194
	private float m_angle;
}
