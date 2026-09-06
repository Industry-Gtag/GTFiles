using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02001447 RID: 5191
	public class BoingReactorFieldGPUSampler : MonoBehaviour
	{
		// Token: 0x060082D9 RID: 33497 RVA: 0x002ACC10 File Offset: 0x002AAE10
		public void OnEnable()
		{
			BoingManager.Register(this);
		}

		// Token: 0x060082DA RID: 33498 RVA: 0x002ACC18 File Offset: 0x002AAE18
		public void OnDisable()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x060082DB RID: 33499 RVA: 0x002ACC20 File Offset: 0x002AAE20
		public void Update()
		{
			if (this.ReactorField == null)
			{
				return;
			}
			BoingReactorField component = this.ReactorField.GetComponent<BoingReactorField>();
			if (component == null)
			{
				return;
			}
			if (component.HardwareMode != BoingReactorField.HardwareModeEnum.GPU)
			{
				return;
			}
			if (this.m_fieldResourceSetId != component.GpuResourceSetId)
			{
				if (this.m_matProps == null)
				{
					this.m_matProps = new MaterialPropertyBlock();
				}
				if (component.UpdateShaderConstants(this.m_matProps, this.PositionSampleMultiplier, this.RotationSampleMultiplier))
				{
					this.m_fieldResourceSetId = component.GpuResourceSetId;
					foreach (Renderer renderer in new Renderer[]
					{
						base.GetComponent<MeshRenderer>(),
						base.GetComponent<SkinnedMeshRenderer>()
					})
					{
						if (!(renderer == null))
						{
							renderer.SetPropertyBlock(this.m_matProps);
						}
					}
				}
			}
		}

		// Token: 0x040093EC RID: 37868
		public BoingReactorField ReactorField;

		// Token: 0x040093ED RID: 37869
		[Range(0f, 10f)]
		[Tooltip("Multiplier on positional samples from reactor field.\n1.0 means 100%.")]
		public float PositionSampleMultiplier = 1f;

		// Token: 0x040093EE RID: 37870
		[Range(0f, 10f)]
		[Tooltip("Multiplier on rotational samples from reactor field.\n1.0 means 100%.")]
		public float RotationSampleMultiplier = 1f;

		// Token: 0x040093EF RID: 37871
		private MaterialPropertyBlock m_matProps;

		// Token: 0x040093F0 RID: 37872
		private int m_fieldResourceSetId = -1;
	}
}
