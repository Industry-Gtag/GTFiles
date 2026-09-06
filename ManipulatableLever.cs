using System;
using UnityEngine;

// Token: 0x02000532 RID: 1330
public class ManipulatableLever : ManipulatableObject
{
	// Token: 0x06002181 RID: 8577 RVA: 0x000B2F4B File Offset: 0x000B114B
	private void Awake()
	{
		this.localSpace = base.transform.worldToLocalMatrix;
	}

	// Token: 0x06002182 RID: 8578 RVA: 0x000B2F60 File Offset: 0x000B1160
	protected override bool ShouldHandDetach(GameObject hand)
	{
		Vector3 position = this.leverGrip.position;
		Vector3 position2 = hand.transform.position;
		return Vector3.SqrMagnitude(position - position2) > this.breakDistance * this.breakDistance;
	}

	// Token: 0x06002183 RID: 8579 RVA: 0x000B2FA0 File Offset: 0x000B11A0
	protected override void OnHeldUpdate(GameObject hand)
	{
		Vector3 position = hand.transform.position;
		Vector3 vector = Vector3.Normalize(this.localSpace.MultiplyPoint3x4(position) - base.transform.localPosition);
		Vector3 eulerAngles = Quaternion.LookRotation(Vector3.forward, vector).eulerAngles;
		if (eulerAngles.z > 180f)
		{
			eulerAngles.z -= 360f;
		}
		else if (eulerAngles.z < -180f)
		{
			eulerAngles.z += 360f;
		}
		eulerAngles.z = Mathf.Clamp(eulerAngles.z, this.minAngle, this.maxAngle);
		base.transform.localEulerAngles = eulerAngles;
	}

	// Token: 0x06002184 RID: 8580 RVA: 0x000B3058 File Offset: 0x000B1258
	public void SetValue(float value)
	{
		float num = Mathf.Lerp(this.minAngle, this.maxAngle, value);
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		localEulerAngles.z = num;
		base.transform.localEulerAngles = localEulerAngles;
	}

	// Token: 0x06002185 RID: 8581 RVA: 0x000B3098 File Offset: 0x000B1298
	public void SetNotch(int notchValue)
	{
		if (this.notches == null)
		{
			return;
		}
		foreach (ManipulatableLever.LeverNotch leverNotch in this.notches)
		{
			if (leverNotch.value == notchValue)
			{
				this.SetValue(Mathf.Lerp(leverNotch.minAngleValue, leverNotch.maxAngleValue, 0.5f));
				return;
			}
		}
	}

	// Token: 0x06002186 RID: 8582 RVA: 0x000B30F0 File Offset: 0x000B12F0
	public float GetValue()
	{
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		if (localEulerAngles.z > 180f)
		{
			localEulerAngles.z -= 360f;
		}
		else if (localEulerAngles.z < -180f)
		{
			localEulerAngles.z += 360f;
		}
		return Mathf.InverseLerp(this.minAngle, this.maxAngle, localEulerAngles.z);
	}

	// Token: 0x06002187 RID: 8583 RVA: 0x000B315C File Offset: 0x000B135C
	public int GetNotch()
	{
		if (this.notches == null)
		{
			return 0;
		}
		float value = this.GetValue();
		foreach (ManipulatableLever.LeverNotch leverNotch in this.notches)
		{
			if (value >= leverNotch.minAngleValue && value <= leverNotch.maxAngleValue)
			{
				return leverNotch.value;
			}
		}
		return 0;
	}

	// Token: 0x04002C49 RID: 11337
	[SerializeField]
	private float breakDistance = 0.2f;

	// Token: 0x04002C4A RID: 11338
	[SerializeField]
	private Transform leverGrip;

	// Token: 0x04002C4B RID: 11339
	[SerializeField]
	private float maxAngle = 22.5f;

	// Token: 0x04002C4C RID: 11340
	[SerializeField]
	private float minAngle = -22.5f;

	// Token: 0x04002C4D RID: 11341
	[SerializeField]
	private ManipulatableLever.LeverNotch[] notches;

	// Token: 0x04002C4E RID: 11342
	private Matrix4x4 localSpace;

	// Token: 0x02000533 RID: 1331
	[Serializable]
	public class LeverNotch
	{
		// Token: 0x04002C4F RID: 11343
		public float minAngleValue;

		// Token: 0x04002C50 RID: 11344
		public float maxAngleValue;

		// Token: 0x04002C51 RID: 11345
		public int value;
	}
}
