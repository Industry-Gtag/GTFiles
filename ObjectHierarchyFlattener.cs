using System;
using GorillaTag;
using UnityEngine;

// Token: 0x0200035E RID: 862
[DefaultExecutionOrder(2001)]
public class ObjectHierarchyFlattener : MonoBehaviour, IGorillaSimpleBackgroundWorker
{
	// Token: 0x06001516 RID: 5398 RVA: 0x000706F8 File Offset: 0x0006E8F8
	private void ResetTransform()
	{
		if (!this.initialized || (this.originalParentGO != null && this.originalParentGO.activeInHierarchy))
		{
			return;
		}
		base.transform.SetParent(this.originalParentTransform);
		this.isAttachedToOverride = false;
		base.transform.localPosition = this.originalLocalPosition;
		base.transform.localRotation = this.originalLocalRotation;
		base.transform.localScale = this.originalScale;
		this.initialized = false;
	}

	// Token: 0x06001517 RID: 5399 RVA: 0x0007077B File Offset: 0x0006E97B
	public void CrumbDisabled()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.trackTransformOfParent)
		{
			ObjectHierarchyFlattenerManager.UnregisterOHF(this);
		}
		if (this != null)
		{
			base.Invoke("ResetTransform", 0f);
		}
	}

	// Token: 0x06001518 RID: 5400 RVA: 0x000707AC File Offset: 0x0006E9AC
	public void InvokeLateUpdate()
	{
		if (this.maintainRelativeScale)
		{
			base.transform.localScale = Vector3.Scale(this.originalParentTransform.lossyScale, this.originalScale);
		}
		base.transform.rotation = this.originalParentTransform.rotation * this.originalLocalRotation;
		base.transform.position = this.originalParentTransform.position + base.transform.rotation * this.calcOffset * (this.originalParentTransform.lossyScale.x / this.originalParentScale) * this.originalParentScale;
	}

	// Token: 0x06001519 RID: 5401 RVA: 0x0007085B File Offset: 0x0006EA5B
	private void OnEnable()
	{
		this.abandonWork = false;
		GorillaSimpleBackgroundWorkerManager.WorkerSignup(this);
	}

	// Token: 0x0600151A RID: 5402 RVA: 0x0007086A File Offset: 0x0006EA6A
	private void OnDisable()
	{
		this.abandonWork = true;
		ObjectHierarchyFlattenerManager.UnregisterOHF(this);
		if (base.enabled)
		{
			base.Invoke("ResetTransformIfStillDisabled", 0f);
		}
	}

	// Token: 0x0600151B RID: 5403 RVA: 0x00070894 File Offset: 0x0006EA94
	private void OnDestroy()
	{
		base.CancelInvoke();
	}

	// Token: 0x0600151C RID: 5404 RVA: 0x0007089C File Offset: 0x0006EA9C
	private void ResetTransformIfStillDisabled()
	{
		if (!base.isActiveAndEnabled)
		{
			this.ResetTransform();
		}
	}

	// Token: 0x0600151D RID: 5405 RVA: 0x000708AC File Offset: 0x0006EAAC
	public void SimpleWork()
	{
		if (this.initialized || this.abandonWork)
		{
			return;
		}
		if (this.trackTransformOfParent)
		{
			ObjectHierarchyFlattenerManager.RegisterOHF(this);
		}
		if (!this.isAttachedToOverride)
		{
			this.originalParentTransform = base.transform.parent;
			this.originalParentGO = this.originalParentTransform.gameObject;
			this.originalLocalPosition = base.transform.localPosition;
			this.originalLocalRotation = base.transform.localRotation;
			this.originalParentScale = base.transform.parent.lossyScale.x;
			this.originalScale = base.transform.localScale;
			this.calcOffset = Vector3.Scale(this.originalLocalPosition, this.originalScale);
			FlattenerCrumb flattenerCrumb = this.originalParentGO.GetComponent<FlattenerCrumb>();
			if (flattenerCrumb == null)
			{
				flattenerCrumb = this.originalParentGO.AddComponent<FlattenerCrumb>();
			}
			flattenerCrumb.AddFlattenerReference(this);
		}
		base.transform.SetParent((this.overrideParentTransform != null) ? this.overrideParentTransform : null);
		this.isAttachedToOverride = true;
		this.initialized = true;
	}

	// Token: 0x040019FA RID: 6650
	public const int k_monoDefaultExecutionOrder = 2001;

	// Token: 0x040019FB RID: 6651
	[DebugReadout]
	private GameObject originalParentGO;

	// Token: 0x040019FC RID: 6652
	private Transform originalParentTransform;

	// Token: 0x040019FD RID: 6653
	private Vector3 originalLocalPosition;

	// Token: 0x040019FE RID: 6654
	private Vector3 calcOffset;

	// Token: 0x040019FF RID: 6655
	private Quaternion originalLocalRotation;

	// Token: 0x04001A00 RID: 6656
	private Vector3 originalScale;

	// Token: 0x04001A01 RID: 6657
	private float originalParentScale;

	// Token: 0x04001A02 RID: 6658
	public bool trackTransformOfParent;

	// Token: 0x04001A03 RID: 6659
	public bool maintainRelativeScale;

	// Token: 0x04001A04 RID: 6660
	private FlattenerCrumb crumb;

	// Token: 0x04001A05 RID: 6661
	public Transform overrideParentTransform;

	// Token: 0x04001A06 RID: 6662
	private bool isAttachedToOverride;

	// Token: 0x04001A07 RID: 6663
	private bool initialized;

	// Token: 0x04001A08 RID: 6664
	private bool abandonWork = true;
}
