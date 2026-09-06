using System;
using BoingKit;
using UnityEngine;

// Token: 0x0200001A RID: 26
public class UFOCamera : MonoBehaviour
{
	// Token: 0x06000069 RID: 105 RVA: 0x000038E4 File Offset: 0x00001AE4
	private void Start()
	{
		if (this.Target == null)
		{
			return;
		}
		this.m_targetOffset = base.transform.position - this.Target.position;
		this.m_spring.Reset(base.transform.position);
	}

	// Token: 0x0600006A RID: 106 RVA: 0x00003938 File Offset: 0x00001B38
	private void FixedUpdate()
	{
		if (this.Target == null)
		{
			return;
		}
		Vector3 vector = this.Target.position + this.m_targetOffset;
		base.transform.position = this.m_spring.TrackExponential(vector, 0.02f, Time.fixedDeltaTime);
	}

	// Token: 0x0400005B RID: 91
	public Transform Target;

	// Token: 0x0400005C RID: 92
	private Vector3 m_targetOffset;

	// Token: 0x0400005D RID: 93
	private Vector3Spring m_spring;
}
