using System;
using GorillaExtensions;
using Unity.Cinemachine;
using UnityEngine;

// Token: 0x020002DC RID: 732
public class GragerHoldable : MonoBehaviour
{
	// Token: 0x060012BF RID: 4799 RVA: 0x00063E00 File Offset: 0x00062000
	private void Start()
	{
		this.LocalRotationAxis = this.LocalRotationAxis.normalized;
		this.lastWorldPosition = base.transform.TransformPoint(this.LocalCenterOfMass);
		this.lastClackParentLocalPosition = base.transform.parent.InverseTransformPoint(this.lastWorldPosition);
		this.centerOfMassRadius = this.LocalCenterOfMass.magnitude;
		this.RotationCorrection = Quaternion.Euler(this.RotationCorrectionEuler);
	}

	// Token: 0x060012C0 RID: 4800 RVA: 0x00063E74 File Offset: 0x00062074
	private void Update()
	{
		Vector3 vector = base.transform.TransformPoint(this.LocalCenterOfMass);
		Vector3 vector2 = this.lastWorldPosition + this.velocity * Time.deltaTime * this.drag;
		Vector3 vector3 = base.transform.parent.TransformDirection(this.LocalRotationAxis);
		Vector3 vector4 = base.transform.position + (vector2 - base.transform.position).ProjectOntoPlane(vector3).normalized * this.centerOfMassRadius;
		vector4 = Vector3.MoveTowards(vector4, vector, this.localFriction * Time.deltaTime);
		this.velocity = (vector4 - this.lastWorldPosition) / Time.deltaTime;
		this.velocity += Vector3.down * this.gravity * Time.deltaTime;
		this.lastWorldPosition = vector4;
		base.transform.rotation = Quaternion.LookRotation(vector4 - base.transform.position, vector3) * this.RotationCorrection;
		Vector3 vector5 = base.transform.parent.InverseTransformPoint(base.transform.TransformPoint(this.LocalCenterOfMass));
		if ((vector5 - this.lastClackParentLocalPosition).IsLongerThan(this.distancePerClack))
		{
			this.clackAudio.GTPlayOneShot(this.allClacks[Random.Range(0, this.allClacks.Length)], 1f);
			this.lastClackParentLocalPosition = vector5;
		}
	}

	// Token: 0x040016CA RID: 5834
	[SerializeField]
	private Vector3 LocalCenterOfMass;

	// Token: 0x040016CB RID: 5835
	[SerializeField]
	private Vector3 LocalRotationAxis;

	// Token: 0x040016CC RID: 5836
	[SerializeField]
	private Vector3 RotationCorrectionEuler;

	// Token: 0x040016CD RID: 5837
	[SerializeField]
	private float drag;

	// Token: 0x040016CE RID: 5838
	[SerializeField]
	private float gravity;

	// Token: 0x040016CF RID: 5839
	[SerializeField]
	private float localFriction;

	// Token: 0x040016D0 RID: 5840
	[SerializeField]
	private float distancePerClack;

	// Token: 0x040016D1 RID: 5841
	[SerializeField]
	private AudioSource clackAudio;

	// Token: 0x040016D2 RID: 5842
	[SerializeField]
	private AudioClip[] allClacks;

	// Token: 0x040016D3 RID: 5843
	private float centerOfMassRadius;

	// Token: 0x040016D4 RID: 5844
	private Vector3 velocity;

	// Token: 0x040016D5 RID: 5845
	private Vector3 lastWorldPosition;

	// Token: 0x040016D6 RID: 5846
	private Vector3 lastClackParentLocalPosition;

	// Token: 0x040016D7 RID: 5847
	private Quaternion RotationCorrection;
}
