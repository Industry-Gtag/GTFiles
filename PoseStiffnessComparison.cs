using System;
using System.Collections.Generic;
using System.Linq;
using BoingKit;
using UnityEngine;

// Token: 0x02000017 RID: 23
public class PoseStiffnessComparison : MonoBehaviour
{
	// Token: 0x06000060 RID: 96 RVA: 0x00003424 File Offset: 0x00001624
	private void Start()
	{
		this.m_timer = 0f;
		this.m_yA = this.BonesA.position.y;
		this.m_yB = this.BonesB.position.y;
	}

	// Token: 0x06000061 RID: 97 RVA: 0x00003460 File Offset: 0x00001660
	private void FixedUpdate()
	{
		BoingBones[] components = this.BonesA.GetComponents<BoingBones>();
		BoingBones[] components2 = this.BonesB.GetComponents<BoingBones>();
		Transform[] array = new Transform[]
		{
			this.BonesA.transform,
			this.BonesB.transform
		};
		float[] array2 = new float[] { this.m_yA, this.m_yB };
		IEnumerable<BoingBones> enumerable = components.Concat(components2);
		float fixedDeltaTime = Time.fixedDeltaTime;
		float num = 0.5f * this.Run;
		this.m_timer += fixedDeltaTime;
		if (this.m_timer > this.Period + this.Rest)
		{
			this.m_timer = Mathf.Repeat(this.m_timer, this.Period + this.Rest);
			for (int i = 0; i < 2; i++)
			{
				Transform transform = array.ElementAt(i);
				float num2 = array2.ElementAt(i);
				Vector3 position = transform.position;
				position.y = num2;
				position.z = -num;
				transform.position = position;
			}
			foreach (BoingBones boingBones in enumerable)
			{
				boingBones.Reboot();
			}
		}
		float num3 = Mathf.Min(1f, this.m_timer * MathUtil.InvSafe(this.Period));
		float num4 = 1f - Mathf.Pow(1f - num3, 1.5f);
		for (int j = 0; j < 2; j++)
		{
			Transform transform2 = array.ElementAt(j);
			float num5 = array2.ElementAt(j);
			Vector3 position2 = transform2.position;
			position2.y = num5 + 2f * Mathf.Sin(12.566371f * num4);
			position2.z = Mathf.Lerp(-num, num, num4);
			transform2.position = position2;
		}
	}

	// Token: 0x04000049 RID: 73
	public float Run = 11f;

	// Token: 0x0400004A RID: 74
	public float Tilt = 15f;

	// Token: 0x0400004B RID: 75
	public float Period = 3f;

	// Token: 0x0400004C RID: 76
	public float Rest = 3f;

	// Token: 0x0400004D RID: 77
	public Transform BonesA;

	// Token: 0x0400004E RID: 78
	public Transform BonesB;

	// Token: 0x0400004F RID: 79
	private float m_yA;

	// Token: 0x04000050 RID: 80
	private float m_yB;

	// Token: 0x04000051 RID: 81
	private float m_timer;
}
