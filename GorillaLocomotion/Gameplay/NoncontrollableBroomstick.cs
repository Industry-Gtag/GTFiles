using System;
using Photon.Pun;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020011AC RID: 4524
	public class NoncontrollableBroomstick : MonoBehaviour, IGorillaGrabable
	{
		// Token: 0x06007212 RID: 29202 RVA: 0x00252068 File Offset: 0x00250268
		private void Start()
		{
			this.smoothRotationTrackingRateExp = Mathf.Exp(this.smoothRotationTrackingRate);
			this.progressPerFixedUpdate = Time.fixedDeltaTime / this.duration;
			this.progress = this.SplineProgressOffet;
			this.secondsToCycles = 1.0 / (double)this.duration;
			if (this.unitySpline != null)
			{
				this.nativeSpline = new NativeSpline(this.unitySpline.Spline, this.unitySpline.transform.localToWorldMatrix, Allocator.Persistent);
			}
		}

		// Token: 0x06007213 RID: 29203 RVA: 0x002520F8 File Offset: 0x002502F8
		protected virtual void FixedUpdate()
		{
			if (PhotonNetwork.InRoom)
			{
				double num = PhotonNetwork.Time * this.secondsToCycles + (double)this.SplineProgressOffet;
				this.progress = (float)(num % 1.0);
			}
			else
			{
				this.progress = (this.progress + this.progressPerFixedUpdate) % 1f;
			}
			Quaternion quaternion = Quaternion.identity;
			if (this.unitySpline != null)
			{
				float3 @float;
				float3 float2;
				float3 float3;
				this.nativeSpline.Evaluate(this.progress, out @float, out float2, out float3);
				base.transform.position = @float;
				if (this.lookForward)
				{
					quaternion = Quaternion.LookRotation(new Vector3(float2.x, float2.y, float2.z));
				}
			}
			else if (this.spline != null)
			{
				Vector3 point = this.spline.GetPoint(this.progress, this.constantVelocity);
				base.transform.position = point;
				if (this.lookForward)
				{
					quaternion = Quaternion.LookRotation(this.spline.GetDirection(this.progress, this.constantVelocity));
				}
			}
			if (this.lookForward)
			{
				base.transform.rotation = Quaternion.Slerp(quaternion, base.transform.rotation, Mathf.Exp(-this.smoothRotationTrackingRateExp * Time.deltaTime));
			}
		}

		// Token: 0x06007214 RID: 29204 RVA: 0x00023F0C File Offset: 0x0002210C
		bool IGorillaGrabable.CanBeGrabbed(GorillaGrabber grabber)
		{
			return true;
		}

		// Token: 0x06007215 RID: 29205 RVA: 0x00252241 File Offset: 0x00250441
		void IGorillaGrabable.OnGrabbed(GorillaGrabber g, out Transform grabbedObject, out Vector3 grabbedLocalPosition)
		{
			grabbedObject = base.transform;
			grabbedLocalPosition = base.transform.InverseTransformPoint(g.transform.position);
		}

		// Token: 0x06007216 RID: 29206 RVA: 0x00002C2D File Offset: 0x00000E2D
		void IGorillaGrabable.OnGrabReleased(GorillaGrabber g)
		{
		}

		// Token: 0x06007217 RID: 29207 RVA: 0x00252267 File Offset: 0x00250467
		private void OnDestroy()
		{
			this.nativeSpline.Dispose();
		}

		// Token: 0x06007218 RID: 29208 RVA: 0x00252274 File Offset: 0x00250474
		public bool MomentaryGrabOnly()
		{
			return this.momentaryGrabOnly;
		}

		// Token: 0x0600721A RID: 29210 RVA: 0x00014B5B File Offset: 0x00012D5B
		string IGorillaGrabable.get_name()
		{
			return base.name;
		}

		// Token: 0x040082DC RID: 33500
		public SplineContainer unitySpline;

		// Token: 0x040082DD RID: 33501
		public BezierSpline spline;

		// Token: 0x040082DE RID: 33502
		public float duration = 30f;

		// Token: 0x040082DF RID: 33503
		public float smoothRotationTrackingRate = 0.5f;

		// Token: 0x040082E0 RID: 33504
		public bool lookForward = true;

		// Token: 0x040082E1 RID: 33505
		[SerializeField]
		private float SplineProgressOffet;

		// Token: 0x040082E2 RID: 33506
		private float progress;

		// Token: 0x040082E3 RID: 33507
		private float smoothRotationTrackingRateExp;

		// Token: 0x040082E4 RID: 33508
		[SerializeField]
		private bool constantVelocity;

		// Token: 0x040082E5 RID: 33509
		private float progressPerFixedUpdate;

		// Token: 0x040082E6 RID: 33510
		private double secondsToCycles;

		// Token: 0x040082E7 RID: 33511
		private NativeSpline nativeSpline;

		// Token: 0x040082E8 RID: 33512
		[SerializeField]
		private bool momentaryGrabOnly = true;
	}
}
