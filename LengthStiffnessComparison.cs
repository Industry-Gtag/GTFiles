using System;
using System.Collections.Generic;
using System.Linq;
using BoingKit;
using UnityEngine;

// Token: 0x02000016 RID: 22
public class LengthStiffnessComparison : MonoBehaviour
{
	// Token: 0x0600005D RID: 93 RVA: 0x00003237 File Offset: 0x00001437
	private void Start()
	{
		this.m_timer = 0f;
	}

	// Token: 0x0600005E RID: 94 RVA: 0x00003244 File Offset: 0x00001444
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
			transform2.rotation = Quaternion.AngleAxis(this.Tilt * (1f - num3), Vector3.right);
		}
	}

	// Token: 0x04000042 RID: 66
	public float Run = 11f;

	// Token: 0x04000043 RID: 67
	public float Tilt = 15f;

	// Token: 0x04000044 RID: 68
	public float Period = 3f;

	// Token: 0x04000045 RID: 69
	public float Rest = 3f;

	// Token: 0x04000046 RID: 70
	public Transform BonesA;

	// Token: 0x04000047 RID: 71
	public Transform BonesB;

	// Token: 0x04000048 RID: 72
	private float m_timer;
}
