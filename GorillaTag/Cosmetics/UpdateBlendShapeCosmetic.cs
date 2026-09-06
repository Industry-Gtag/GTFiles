using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001367 RID: 4967
	public class UpdateBlendShapeCosmetic : MonoBehaviour
	{
		// Token: 0x06007C73 RID: 31859 RVA: 0x0028A7D3 File Offset: 0x002889D3
		private void Awake()
		{
			this.targetWeight = this.blendStartWeight;
			this.currentWeight = 0f;
		}

		// Token: 0x06007C74 RID: 31860 RVA: 0x0028A7EC File Offset: 0x002889EC
		private void Update()
		{
			this.currentWeight = Mathf.Lerp(this.currentWeight, this.targetWeight, Time.deltaTime * this.blendSpeed);
			this.skinnedMeshRenderer.SetBlendShapeWeight(this.blendShapeIndex, this.currentWeight);
		}

		// Token: 0x06007C75 RID: 31861 RVA: 0x0028A828 File Offset: 0x00288A28
		public void SetBlendValue(bool leftHand, float value)
		{
			this.targetWeight = Mathf.Clamp01(this.invertPassedBlend ? (1f - value) : value) * this.maxBlendShapeWeight;
		}

		// Token: 0x06007C76 RID: 31862 RVA: 0x0028A84E File Offset: 0x00288A4E
		public void SetBlendValue(float value)
		{
			this.targetWeight = Mathf.Clamp01(this.invertPassedBlend ? (1f - value) : value) * this.maxBlendShapeWeight;
		}

		// Token: 0x06007C77 RID: 31863 RVA: 0x0028A874 File Offset: 0x00288A74
		public void FullyBlend()
		{
			this.targetWeight = this.maxBlendShapeWeight;
		}

		// Token: 0x06007C78 RID: 31864 RVA: 0x0028A882 File Offset: 0x00288A82
		public void ResetBlend()
		{
			this.targetWeight = 0f;
		}

		// Token: 0x06007C79 RID: 31865 RVA: 0x0028A88F File Offset: 0x00288A8F
		public float GetBlendValue()
		{
			return this.skinnedMeshRenderer.GetBlendShapeWeight(this.blendShapeIndex);
		}

		// Token: 0x04008EE4 RID: 36580
		[Tooltip("The SkinnedMeshRenderer whose BlendShape weight will be updated. This must reference a mesh that has BlendShapes defined in its import settings.")]
		[SerializeField]
		private SkinnedMeshRenderer skinnedMeshRenderer;

		// Token: 0x04008EE5 RID: 36581
		[Tooltip("Maximum blend shape weight applied when fully blended. Usually 100 for standard Unity BlendShapes.")]
		public float maxBlendShapeWeight = 100f;

		// Token: 0x04008EE6 RID: 36582
		[Tooltip("Index of the BlendShape to control. You can find this index in the SkinnedMeshRenderer inspector under 'BlendShapes'.")]
		[SerializeField]
		private int blendShapeIndex;

		// Token: 0x04008EE7 RID: 36583
		[Tooltip("Speed at which the BlendShape transitions toward its target weight. Higher values make blending more responsive, lower values make it smoother.")]
		[SerializeField]
		private float blendSpeed = 10f;

		// Token: 0x04008EE8 RID: 36584
		[Tooltip("Initial BlendShape weight set when the component awakens. Useful for setting a default deformation state.")]
		[SerializeField]
		private float blendStartWeight;

		// Token: 0x04008EE9 RID: 36585
		[Tooltip("If enabled, inverts the incoming blend value (e.g. 0 → 1, 0.2 → 0.8). Useful when an input should drive the opposite direction of deformation.")]
		[SerializeField]
		private bool invertPassedBlend;

		// Token: 0x04008EEA RID: 36586
		private float targetWeight;

		// Token: 0x04008EEB RID: 36587
		private float currentWeight;
	}
}
