using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000020 RID: 32
public class ScaleSpring : MonoBehaviour
{
	// Token: 0x0600007B RID: 123 RVA: 0x00003F98 File Offset: 0x00002198
	public void Tick()
	{
		this.m_targetScale = ((this.m_targetScale == ScaleSpring.kSmallScale) ? ScaleSpring.kLargeScale : ScaleSpring.kSmallScale);
		this.m_lastTickTime = Time.time;
		base.GetComponent<BoingEffector>().MoveDistance = ScaleSpring.kMoveDistance * ((this.m_targetScale == ScaleSpring.kSmallScale) ? (-1f) : 1f);
	}

	// Token: 0x0600007C RID: 124 RVA: 0x00003FF9 File Offset: 0x000021F9
	public void Start()
	{
		this.Tick();
		this.m_spring.Reset(this.m_targetScale * Vector3.one);
	}

	// Token: 0x0600007D RID: 125 RVA: 0x0000401C File Offset: 0x0000221C
	public void FixedUpdate()
	{
		if (Time.time - this.m_lastTickTime > ScaleSpring.kInterval)
		{
			this.Tick();
		}
		this.m_spring.TrackHalfLife(this.m_targetScale * Vector3.one, 6f, 0.05f, Time.fixedDeltaTime);
		base.transform.localScale = this.m_spring.Value;
		base.GetComponent<BoingEffector>().MoveDistance *= Mathf.Min(0.99f, 35f * Time.fixedDeltaTime);
	}

	// Token: 0x04000070 RID: 112
	private static readonly float kInterval = 2f;

	// Token: 0x04000071 RID: 113
	private static readonly float kSmallScale = 0.6f;

	// Token: 0x04000072 RID: 114
	private static readonly float kLargeScale = 2f;

	// Token: 0x04000073 RID: 115
	private static readonly float kMoveDistance = 30f;

	// Token: 0x04000074 RID: 116
	private Vector3Spring m_spring;

	// Token: 0x04000075 RID: 117
	private float m_targetScale;

	// Token: 0x04000076 RID: 118
	private float m_lastTickTime;
}
