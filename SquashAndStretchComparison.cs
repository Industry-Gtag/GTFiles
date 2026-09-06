using System;
using System.Collections.Generic;
using System.Linq;
using BoingKit;
using UnityEngine;

// Token: 0x02000018 RID: 24
public class SquashAndStretchComparison : MonoBehaviour
{
	// Token: 0x06000063 RID: 99 RVA: 0x00003674 File Offset: 0x00001874
	private void Start()
	{
		this.m_timer = 0f;
	}

	// Token: 0x06000064 RID: 100 RVA: 0x00003684 File Offset: 0x00001884
	private void FixedUpdate()
	{
		BoingBones[] components = this.BonesA.GetComponents<BoingBones>();
		BoingBones[] components2 = this.BonesB.GetComponents<BoingBones>();
		Transform[] array = new Transform[]
		{
			this.BonesA.transform,
			this.BonesB.transform
		};
		IEnumerable<BoingBones> enumerable = components.Concat(components2);
		float fixedDeltaTime = Time.fixedDeltaTime;
		float num = 0.5f * this.Run;
		this.m_timer += fixedDeltaTime;
		if (this.m_timer > this.Period + this.Rest)
		{
			this.m_timer = Mathf.Repeat(this.m_timer, this.Period + this.Rest);
			foreach (Transform transform in array)
			{
				Vector3 position = transform.position;
				position.z = -num;
				transform.position = position;
			}
			foreach (BoingBones boingBones in enumerable)
			{
				boingBones.Reboot();
			}
		}
		float num2 = Mathf.Min(1f, this.m_timer * MathUtil.InvSafe(this.Period));
		float num3 = 1f - Mathf.Pow(1f - num2, 6f);
		foreach (Transform transform2 in array)
		{
			Vector3 position2 = transform2.position;
			position2.z = Mathf.Lerp(-num, num, num3);
			transform2.position = position2;
		}
	}

	// Token: 0x04000052 RID: 82
	public float Run = 11f;

	// Token: 0x04000053 RID: 83
	public float Period = 3f;

	// Token: 0x04000054 RID: 84
	public float Rest = 3f;

	// Token: 0x04000055 RID: 85
	public Transform BonesA;

	// Token: 0x04000056 RID: 86
	public Transform BonesB;

	// Token: 0x04000057 RID: 87
	private float m_timer;
}
