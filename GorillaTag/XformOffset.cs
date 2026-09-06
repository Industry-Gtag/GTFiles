using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x020011E8 RID: 4584
	[Serializable]
	public struct XformOffset
	{
		// Token: 0x17000B4E RID: 2894
		// (get) Token: 0x06007485 RID: 29829 RVA: 0x0025E16D File Offset: 0x0025C36D
		// (set) Token: 0x06007486 RID: 29830 RVA: 0x0025E175 File Offset: 0x0025C375
		[Tooltip("The rotation of the offset relative to the parent bone.")]
		public Quaternion rot
		{
			get
			{
				return this._rotQuat;
			}
			set
			{
				this._rotQuat = value;
			}
		}

		// Token: 0x06007487 RID: 29831 RVA: 0x0025E17E File Offset: 0x0025C37E
		public XformOffset(Vector3 pos, Quaternion rot, Vector3 scale)
		{
			this.pos = pos;
			this._rotQuat = rot;
			this._rotEulerAngles = rot.eulerAngles;
			this.scale = scale;
		}

		// Token: 0x06007488 RID: 29832 RVA: 0x0025E1A2 File Offset: 0x0025C3A2
		public XformOffset(Vector3 pos, Vector3 rot, Vector3 scale)
		{
			this.pos = pos;
			this._rotQuat = Quaternion.Euler(rot);
			this._rotEulerAngles = rot;
			this.scale = scale;
		}

		// Token: 0x06007489 RID: 29833 RVA: 0x0025E1C5 File Offset: 0x0025C3C5
		public XformOffset(Vector3 pos, Quaternion rot)
		{
			this.pos = pos;
			this._rotQuat = rot;
			this._rotEulerAngles = rot.eulerAngles;
			this.scale = Vector3.one;
		}

		// Token: 0x0600748A RID: 29834 RVA: 0x0025E1ED File Offset: 0x0025C3ED
		public XformOffset(Vector3 pos, Vector3 rot)
		{
			this.pos = pos;
			this._rotQuat = Quaternion.Euler(rot);
			this._rotEulerAngles = rot;
			this.scale = Vector3.one;
		}

		// Token: 0x0600748B RID: 29835 RVA: 0x0025E214 File Offset: 0x0025C414
		public XformOffset(Transform parentXform, Transform childXform)
		{
			this.pos = parentXform.InverseTransformPoint(childXform.position);
			this._rotQuat = Quaternion.Inverse(parentXform.rotation) * childXform.rotation;
			this._rotEulerAngles = this._rotQuat.eulerAngles;
			this.scale = childXform.lossyScale.SafeDivide(parentXform.lossyScale);
		}

		// Token: 0x0600748C RID: 29836 RVA: 0x0025E278 File Offset: 0x0025C478
		public XformOffset(Matrix4x4 matrix)
		{
			this.pos = matrix.GetPosition();
			this.scale = matrix.lossyScale;
			if (Vector3.Dot(Vector3.Cross(matrix.GetColumn(0), matrix.GetColumn(1)), matrix.GetColumn(2)) < 0f)
			{
				this.scale = -this.scale;
			}
			Matrix4x4 matrix4x = matrix;
			matrix4x.SetColumn(0, matrix4x.GetColumn(0) / this.scale.x);
			matrix4x.SetColumn(1, matrix4x.GetColumn(1) / this.scale.y);
			matrix4x.SetColumn(2, matrix4x.GetColumn(2) / this.scale.z);
			this._rotQuat = Quaternion.LookRotation(matrix4x.GetColumn(2), matrix4x.GetColumn(1));
			this._rotEulerAngles = this._rotQuat.eulerAngles;
		}

		// Token: 0x0600748D RID: 29837 RVA: 0x0025E380 File Offset: 0x0025C580
		public bool Approx(XformOffset other)
		{
			return this.pos.Approx(other.pos, 1E-05f) && this._rotQuat.Approx(other._rotQuat, 1E-06f) && this.scale.Approx(other.scale, 1E-05f);
		}

		// Token: 0x04008459 RID: 33881
		[Tooltip("The position of the offset relative to the parent bone.")]
		public Vector3 pos;

		// Token: 0x0400845A RID: 33882
		[FormerlySerializedAs("_edRotQuat")]
		[FormerlySerializedAs("rot")]
		[HideInInspector]
		[SerializeField]
		private Quaternion _rotQuat;

		// Token: 0x0400845B RID: 33883
		[FormerlySerializedAs("_edRotEulerAngles")]
		[FormerlySerializedAs("_edRotEuler")]
		[HideInInspector]
		[SerializeField]
		private Vector3 _rotEulerAngles;

		// Token: 0x0400845C RID: 33884
		[Tooltip("The scale of the offset relative to the parent bone.")]
		public Vector3 scale;

		// Token: 0x0400845D RID: 33885
		public static readonly XformOffset Identity = new XformOffset
		{
			_rotQuat = Quaternion.identity,
			scale = Vector3.one
		};
	}
}
