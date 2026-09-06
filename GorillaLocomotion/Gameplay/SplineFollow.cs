using System;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020011AF RID: 4527
	public sealed class SplineFollow : MonoBehaviour
	{
		// Token: 0x06007239 RID: 29241 RVA: 0x00252D38 File Offset: 0x00250F38
		public void Start()
		{
			base.transform.rotation *= this._rotationFix;
			this._smoothRotationTrackingRateExp = Mathf.Exp(this._smoothRotationTrackingRate);
			this._progress = this._splineProgressOffset;
			this._progressPerFixedUpdate = Time.fixedDeltaTime / this._duration;
			this._secondsToCycles = (double)(1f / this._duration);
			this._nativeSpline = new NativeSpline(this._unitySpline.Spline, this._unitySpline.transform.localToWorldMatrix, Allocator.Persistent);
			if (this._approximate)
			{
				this.CalculateApproximationNodes();
			}
		}

		// Token: 0x0600723A RID: 29242 RVA: 0x00252DE0 File Offset: 0x00250FE0
		private void CalculateApproximationNodes()
		{
			for (int i = 0; i < this._approximationResolution; i++)
			{
				float3 @float;
				float3 float2;
				float3 float3;
				this._nativeSpline.Evaluate((float)i / (float)this._approximationResolution, out @float, out float2, out float3);
				SplineFollow.SplineNode splineNode = new SplineFollow.SplineNode(@float, float2, float3);
				this._approximationNodes.Add(splineNode);
			}
			if (this._nativeSpline.Closed)
			{
				this._approximationNodes.Add(this._approximationNodes[0]);
			}
		}

		// Token: 0x0600723B RID: 29243 RVA: 0x00252E64 File Offset: 0x00251064
		private void FixedUpdate()
		{
			if (!this._approximate)
			{
				this.FollowSpline();
			}
		}

		// Token: 0x0600723C RID: 29244 RVA: 0x00252E74 File Offset: 0x00251074
		private void Update()
		{
			if (this._approximate)
			{
				this.FollowSpline();
			}
		}

		// Token: 0x0600723D RID: 29245 RVA: 0x00252E84 File Offset: 0x00251084
		private void FollowSpline()
		{
			if (PhotonNetwork.InRoom)
			{
				double num = PhotonNetwork.Time * this._secondsToCycles + (double)this._splineProgressOffset;
				this._progress = (float)(num % 1.0);
			}
			else
			{
				this._progress = (this._progress + this._progressPerFixedUpdate) % 1f;
			}
			SplineFollow.SplineNode splineNode = this.EvaluateSpline(this._progress);
			base.transform.position = splineNode.Position;
			Quaternion quaternion = Quaternion.LookRotation(splineNode.Tangent) * this._rotationFix;
			base.transform.rotation = Quaternion.Slerp(quaternion, base.transform.rotation, Mathf.Exp(-this._smoothRotationTrackingRateExp * Time.deltaTime));
		}

		// Token: 0x0600723E RID: 29246 RVA: 0x00252F40 File Offset: 0x00251140
		private SplineFollow.SplineNode EvaluateSpline(float t)
		{
			t %= 1f;
			if (this._approximate)
			{
				float num = t * (float)this._approximationNodes.Count;
				int num2 = (int)num;
				float num3 = num - (float)num2;
				num2 %= this._approximationNodes.Count;
				SplineFollow.SplineNode splineNode = this._approximationNodes[num2];
				SplineFollow.SplineNode splineNode2 = this._approximationNodes[(num2 + 1) % this._approximationNodes.Count];
				return SplineFollow.SplineNode.Lerp(splineNode, splineNode2, num3);
			}
			float3 @float;
			float3 float2;
			float3 float3;
			this._nativeSpline.Evaluate(t, out @float, out float2, out float3);
			return new SplineFollow.SplineNode(@float, float2, float3);
		}

		// Token: 0x0600723F RID: 29247 RVA: 0x00252FDC File Offset: 0x002511DC
		private void OnDestroy()
		{
			this._nativeSpline.Dispose();
		}

		// Token: 0x040082FC RID: 33532
		[SerializeField]
		[Tooltip("If true, approximates the spline position. Only use when exact position does not matter.")]
		private bool _approximate;

		// Token: 0x040082FD RID: 33533
		[SerializeField]
		private SplineContainer _unitySpline;

		// Token: 0x040082FE RID: 33534
		[SerializeField]
		private float _duration;

		// Token: 0x040082FF RID: 33535
		private double _secondsToCycles;

		// Token: 0x04008300 RID: 33536
		[SerializeField]
		private float _smoothRotationTrackingRate = 0.5f;

		// Token: 0x04008301 RID: 33537
		private float _smoothRotationTrackingRateExp;

		// Token: 0x04008302 RID: 33538
		private float _progressPerFixedUpdate;

		// Token: 0x04008303 RID: 33539
		[SerializeField]
		private float _splineProgressOffset;

		// Token: 0x04008304 RID: 33540
		[SerializeField]
		private Quaternion _rotationFix = Quaternion.identity;

		// Token: 0x04008305 RID: 33541
		private NativeSpline _nativeSpline;

		// Token: 0x04008306 RID: 33542
		private float _progress;

		// Token: 0x04008307 RID: 33543
		[Header("Approximate Spline Parameters")]
		[SerializeField]
		[Range(4f, 200f)]
		private int _approximationResolution = 100;

		// Token: 0x04008308 RID: 33544
		private readonly List<SplineFollow.SplineNode> _approximationNodes = new List<SplineFollow.SplineNode>();

		// Token: 0x020011B0 RID: 4528
		private struct SplineNode
		{
			// Token: 0x06007241 RID: 29249 RVA: 0x0025301C File Offset: 0x0025121C
			public SplineNode(Vector3 position, Vector3 tangent, Vector3 up)
			{
				this.Position = position;
				this.Tangent = tangent;
				this.Up = up;
			}

			// Token: 0x06007242 RID: 29250 RVA: 0x00253044 File Offset: 0x00251244
			public static SplineFollow.SplineNode Lerp(SplineFollow.SplineNode a, SplineFollow.SplineNode b, float t)
			{
				return new SplineFollow.SplineNode(Vector3.Lerp(a.Position, b.Position, t), Vector3.Lerp(a.Tangent, b.Tangent, t), Vector3.Lerp(a.Up, b.Up, t));
			}

			// Token: 0x04008309 RID: 33545
			public readonly Vector3 Position;

			// Token: 0x0400830A RID: 33546
			public readonly Vector3 Tangent;

			// Token: 0x0400830B RID: 33547
			public readonly Vector3 Up;
		}
	}
}
