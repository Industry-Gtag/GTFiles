using System;
using UnityEngine;

// Token: 0x0200002C RID: 44
public class WASD : MonoBehaviour
{
	// Token: 0x1700000E RID: 14
	// (get) Token: 0x060000A1 RID: 161 RVA: 0x000053C1 File Offset: 0x000035C1
	public Vector3 Velocity
	{
		get
		{
			return this.m_velocity;
		}
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x000053CC File Offset: 0x000035CC
	public void Update()
	{
		Vector3 zero = Vector3.zero;
		float num = 0f;
		if (Input.GetKey(KeyCode.W))
		{
			zero.z += 1f;
		}
		if (Input.GetKey(KeyCode.A))
		{
			zero.x -= 1f;
		}
		if (Input.GetKey(KeyCode.S))
		{
			zero.z -= 1f;
		}
		if (Input.GetKey(KeyCode.D))
		{
			zero.x += 1f;
		}
		Vector3 vector = ((zero.sqrMagnitude > 0f) ? (zero.normalized * this.Speed * Time.deltaTime) : Vector3.zero);
		Quaternion quaternion = Quaternion.AngleAxis(num * this.Omega * 57.29578f * Time.deltaTime, Vector3.up);
		this.m_velocity = vector / Time.deltaTime;
		base.transform.position += vector;
		base.transform.rotation = quaternion * base.transform.rotation;
	}

	// Token: 0x040000C3 RID: 195
	public float Speed = 1f;

	// Token: 0x040000C4 RID: 196
	public float Omega = 1f;

	// Token: 0x040000C5 RID: 197
	public Vector3 m_velocity;
}
