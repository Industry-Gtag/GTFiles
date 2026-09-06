using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000E68 RID: 3688
public class TransformOscillation : MonoBehaviour
{
	// Token: 0x060059FC RID: 23036 RVA: 0x001D2A1A File Offset: 0x001D0C1A
	private void Awake()
	{
		if (this.useRigidbodyMotion && !this.targetRigidbody)
		{
			this.targetRigidbody = base.GetComponent<Rigidbody>();
		}
		this.lastRotOffs = Quaternion.identity;
		this.startTime = Time.time;
		this.isRunning = false;
	}

	// Token: 0x060059FD RID: 23037 RVA: 0x001D2A5A File Offset: 0x001D0C5A
	private void OnEnable()
	{
		this.lastPosOffs = Vector3.zero;
		this.lastRotOffs = Quaternion.identity;
		if (this.startOnEnable)
		{
			this.StartOscillation();
			return;
		}
		this.isRunning = false;
	}

	// Token: 0x060059FE RID: 23038 RVA: 0x001D2A88 File Offset: 0x001D0C88
	public void StartOscillation()
	{
		this.startTime = Time.time;
		this.isRunning = true;
	}

	// Token: 0x060059FF RID: 23039 RVA: 0x001D2A9C File Offset: 0x001D0C9C
	private float GetTimeSeconds()
	{
		if (!this.useServerTime)
		{
			return Time.timeSinceLevelLoad;
		}
		if (GorillaComputer.instance == null)
		{
			return Time.timeSinceLevelLoad;
		}
		this.dt = GorillaComputer.instance.GetServerTime();
		return (float)this.dt.Minute * 60f + (float)this.dt.Second + (float)this.dt.Millisecond / 1000f;
	}

	// Token: 0x06005A00 RID: 23040 RVA: 0x001D2B10 File Offset: 0x001D0D10
	private void ComputeOffsets(float t)
	{
		this.offsPos.x = this.PosAmp.x * Mathf.Sin(t * this.PosFreq.x);
		this.offsPos.y = this.PosAmp.y * Mathf.Sin(t * this.PosFreq.y);
		this.offsPos.z = this.PosAmp.z * Mathf.Sin(t * this.PosFreq.z);
		this.offsRot.x = this.RotAmp.x * Mathf.Sin(t * this.RotFreq.x);
		this.offsRot.y = this.RotAmp.y * Mathf.Sin(t * this.RotFreq.y);
		this.offsRot.z = this.RotAmp.z * Mathf.Sin(t * this.RotFreq.z);
	}

	// Token: 0x06005A01 RID: 23041 RVA: 0x001D2C14 File Offset: 0x001D0E14
	private void LateUpdate()
	{
		if (!this.isRunning)
		{
			return;
		}
		if (this.useTimeLimit && Time.time - this.startTime >= this.timer)
		{
			return;
		}
		if (this.useRigidbodyMotion && this.targetRigidbody)
		{
			return;
		}
		float timeSeconds = this.GetTimeSeconds();
		this.ComputeOffsets(timeSeconds);
		Transform transform = base.transform;
		Quaternion quaternion = Quaternion.Euler(this.offsRot);
		Vector3 vector = transform.localPosition - this.lastPosOffs;
		Quaternion quaternion2 = transform.localRotation * Quaternion.Inverse(this.lastRotOffs);
		transform.localPosition = vector + this.offsPos;
		transform.localRotation = quaternion2 * quaternion;
		this.lastPosOffs = this.offsPos;
		this.lastRotOffs = quaternion;
	}

	// Token: 0x06005A02 RID: 23042 RVA: 0x001D2CD8 File Offset: 0x001D0ED8
	private void FixedUpdate()
	{
		if (!this.isRunning)
		{
			return;
		}
		if (this.useTimeLimit && Time.time - this.startTime >= this.timer)
		{
			return;
		}
		if (!this.useRigidbodyMotion || !this.targetRigidbody)
		{
			return;
		}
		float timeSeconds = this.GetTimeSeconds();
		this.ComputeOffsets(timeSeconds);
		Transform transform = base.transform;
		Quaternion quaternion = Quaternion.Euler(this.offsRot);
		Transform parent = transform.parent;
		Vector3 vector = (parent ? parent.TransformVector(this.lastPosOffs) : this.lastPosOffs);
		Quaternion quaternion2 = (parent ? (parent.rotation * this.lastRotOffs * Quaternion.Inverse(parent.rotation)) : this.lastRotOffs);
		Vector3 vector2 = transform.position - vector;
		Quaternion quaternion3 = transform.rotation * Quaternion.Inverse(quaternion2);
		Vector3 vector3 = (parent ? parent.TransformVector(this.offsPos) : this.offsPos);
		Quaternion quaternion4 = (parent ? (parent.rotation * quaternion * Quaternion.Inverse(parent.rotation)) : quaternion);
		this.targetRigidbody.MovePosition(vector2 + vector3);
		this.targetRigidbody.MoveRotation(quaternion3 * quaternion4);
		this.lastPosOffs = this.offsPos;
		this.lastRotOffs = quaternion;
	}

	// Token: 0x04006A43 RID: 27203
	[SerializeField]
	private Vector3 PosAmp;

	// Token: 0x04006A44 RID: 27204
	[SerializeField]
	private Vector3 PosFreq;

	// Token: 0x04006A45 RID: 27205
	[SerializeField]
	private Vector3 RotAmp;

	// Token: 0x04006A46 RID: 27206
	[SerializeField]
	private Vector3 RotFreq;

	// Token: 0x04006A47 RID: 27207
	[SerializeField]
	private bool useServerTime;

	// Token: 0x04006A48 RID: 27208
	[Header("Rigidbody Motion (optional)")]
	[Tooltip("If true and a Rigidbody is present, applies motion using Rigidbody.MovePosition/MoveRotation in FixedUpdate.")]
	[SerializeField]
	private bool useRigidbodyMotion;

	// Token: 0x04006A49 RID: 27209
	[SerializeField]
	private Rigidbody targetRigidbody;

	// Token: 0x04006A4A RID: 27210
	[Header("Activation Timer (optional)")]
	[Tooltip("If true, oscillation only runs for 'activeDurationSeconds' after OnEnable; otherwise it runs indefinitely.")]
	[SerializeField]
	private bool useTimeLimit;

	// Token: 0x04006A4B RID: 27211
	[SerializeField]
	private float timer = 2f;

	// Token: 0x04006A4C RID: 27212
	[Header("Start Behavior (optional)")]
	[Tooltip("If true, oscillation starts automatically on OnEnable(). If false, call StartOscillation() manually.")]
	[SerializeField]
	private bool startOnEnable = true;

	// Token: 0x04006A4D RID: 27213
	private Vector3 lastPosOffs = Vector3.zero;

	// Token: 0x04006A4E RID: 27214
	private Quaternion lastRotOffs = Quaternion.identity;

	// Token: 0x04006A4F RID: 27215
	private Vector3 offsPos;

	// Token: 0x04006A50 RID: 27216
	private Vector3 offsRot;

	// Token: 0x04006A51 RID: 27217
	private DateTime dt;

	// Token: 0x04006A52 RID: 27218
	private float startTime;

	// Token: 0x04006A53 RID: 27219
	private bool isRunning;
}
