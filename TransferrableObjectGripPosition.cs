using System;
using UnityEngine;

// Token: 0x02000556 RID: 1366
public class TransferrableObjectGripPosition : MonoBehaviour
{
	// Token: 0x060022C5 RID: 8901 RVA: 0x000BB00F File Offset: 0x000B920F
	private void Awake()
	{
		if (this.parentObject == null)
		{
			this.parentObject = base.transform.parent.GetComponent<TransferrableItemSlotTransformOverride>();
		}
		this.parentObject.AddGripPosition(this.attachmentType, this);
	}

	// Token: 0x060022C6 RID: 8902 RVA: 0x000BB047 File Offset: 0x000B9247
	public SubGrabPoint CreateSubGrabPoint(SlotTransformOverride overrideContainer)
	{
		return new SubGrabPoint();
	}

	// Token: 0x04002DE5 RID: 11749
	[SerializeField]
	private TransferrableItemSlotTransformOverride parentObject;

	// Token: 0x04002DE6 RID: 11750
	[SerializeField]
	private TransferrableObject.PositionState attachmentType;
}
