using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02001446 RID: 5190
	public class BoingReactorFieldCPUSampler : MonoBehaviour
	{
		// Token: 0x060082D4 RID: 33492 RVA: 0x002ACAF6 File Offset: 0x002AACF6
		public void OnEnable()
		{
			BoingManager.Register(this);
		}

		// Token: 0x060082D5 RID: 33493 RVA: 0x002ACAFE File Offset: 0x002AACFE
		public void OnDisable()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x060082D6 RID: 33494 RVA: 0x002ACB08 File Offset: 0x002AAD08
		public void SampleFromField()
		{
			this.m_objPosition = base.transform.position;
			this.m_objRotation = base.transform.rotation;
			if (this.ReactorField == null)
			{
				return;
			}
			BoingReactorField component = this.ReactorField.GetComponent<BoingReactorField>();
			if (component == null)
			{
				return;
			}
			if (component.HardwareMode != BoingReactorField.HardwareModeEnum.CPU)
			{
				return;
			}
			Vector3 vector;
			Vector4 vector2;
			if (!component.SampleCpuGrid(base.transform.position, out vector, out vector2))
			{
				return;
			}
			base.transform.position = this.m_objPosition + vector * this.PositionSampleMultiplier;
			base.transform.rotation = QuaternionUtil.Pow(QuaternionUtil.FromVector4(vector2, true), this.RotationSampleMultiplier) * this.m_objRotation;
		}

		// Token: 0x060082D7 RID: 33495 RVA: 0x002ACBC7 File Offset: 0x002AADC7
		public void Restore()
		{
			base.transform.position = this.m_objPosition;
			base.transform.rotation = this.m_objRotation;
		}

		// Token: 0x040093E6 RID: 37862
		public BoingReactorField ReactorField;

		// Token: 0x040093E7 RID: 37863
		[Tooltip("Match this mode with how you update your object's transform.\n\nUpdate - Use this mode if you update your object's transform in Update(). This uses variable Time.detalTime. Use FixedUpdate if physics simulation becomes unstable.\n\nFixed Update - Use this mode if you update your object's transform in FixedUpdate(). This uses fixed Time.fixedDeltaTime. Also, use this mode if the game object is affected by Unity physics (i.e. has a rigid body component), which uses fixed updates.")]
		public BoingManager.UpdateMode UpdateMode = BoingManager.UpdateMode.LateUpdate;

		// Token: 0x040093E8 RID: 37864
		[Range(0f, 10f)]
		[Tooltip("Multiplier on positional samples from reactor field.\n1.0 means 100%.")]
		public float PositionSampleMultiplier = 1f;

		// Token: 0x040093E9 RID: 37865
		[Range(0f, 10f)]
		[Tooltip("Multiplier on rotational samples from reactor field.\n1.0 means 100%.")]
		public float RotationSampleMultiplier = 1f;

		// Token: 0x040093EA RID: 37866
		private Vector3 m_objPosition;

		// Token: 0x040093EB RID: 37867
		private Quaternion m_objRotation;
	}
}
