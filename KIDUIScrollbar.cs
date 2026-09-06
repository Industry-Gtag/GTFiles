using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

// Token: 0x020003E9 RID: 1001
[AddComponentMenu("UI/KIDUI Scrollbar", 37)]
public class KIDUIScrollbar : Scrollbar, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x17000250 RID: 592
	// (get) Token: 0x060017C4 RID: 6084 RVA: 0x00088695 File Offset: 0x00086895
	private XRUIInputModule InputModule
	{
		get
		{
			return EventSystem.current.currentInputModule as XRUIInputModule;
		}
	}

	// Token: 0x17000251 RID: 593
	// (get) Token: 0x060017C5 RID: 6085 RVA: 0x000886A6 File Offset: 0x000868A6
	private KIDUIScrollbar.Axis axis
	{
		get
		{
			if (base.direction != Scrollbar.Direction.LeftToRight && base.direction != Scrollbar.Direction.RightToLeft)
			{
				return KIDUIScrollbar.Axis.Vertical;
			}
			return KIDUIScrollbar.Axis.Horizontal;
		}
	}

	// Token: 0x060017C6 RID: 6086 RVA: 0x000886BC File Offset: 0x000868BC
	protected override void OnEnable()
	{
		base.OnEnable();
		this.containerRect = base.handleRect.parent.GetComponent<RectTransform>();
		if (GorillaTagger.Instance)
		{
			this.thirdPersonCamera = GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>();
		}
		if (ControllerBehaviour.Instance != null)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x060017C7 RID: 6087 RVA: 0x00088729 File Offset: 0x00086929
	protected override void OnDisable()
	{
		base.OnDisable();
		if (ControllerBehaviour.Instance != null)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
		this._isPointerInside = false;
		this._currentPointerData = null;
	}

	// Token: 0x060017C8 RID: 6088 RVA: 0x00088764 File Offset: 0x00086964
	private void PostUpdate()
	{
		if (!this._isPointerInside && !ControllerBehaviour.Instance.TriggerDown)
		{
			this._isHolding = false;
			return;
		}
		if (!base.interactable || !ControllerBehaviour.Instance.TriggerDown || this._currentPointerData == null)
		{
			return;
		}
		if (!this._isHolding && this._isPointerInside && ControllerBehaviour.Instance.TriggerDown)
		{
			this._isHolding = true;
		}
		if (!this._isHolding || !this.IsInteractable() || this.InputModule == null)
		{
			return;
		}
		XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(this._currentPointerData.pointerId) as XRRayInteractor;
		RaycastResult raycastResult;
		if (xrrayInteractor != null && xrrayInteractor.TryGetCurrentUIRaycastResult(out raycastResult))
		{
			Vector2 vector;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.containerRect, raycastResult.screenPosition, this.thirdPersonCamera, out vector);
			Vector2 zero = Vector2.zero;
			Vector2 vector2 = vector - zero - this.containerRect.rect.position - (base.handleRect.rect.size - base.handleRect.sizeDelta) * 0.5f;
			float num = ((this.axis == KIDUIScrollbar.Axis.Horizontal) ? this.containerRect.rect.width : this.containerRect.rect.height) * (1f - base.size);
			if (num <= 0f)
			{
				return;
			}
			this.UpdateDrag(vector2, num);
		}
	}

	// Token: 0x060017C9 RID: 6089 RVA: 0x000888E8 File Offset: 0x00086AE8
	private void UpdateDrag(Vector2 handleCorner, float remainingSize)
	{
		switch (base.direction)
		{
		case Scrollbar.Direction.LeftToRight:
			base.value = Mathf.Clamp01(handleCorner.x / remainingSize);
			return;
		case Scrollbar.Direction.RightToLeft:
			base.value = Mathf.Clamp01(1f - handleCorner.x / remainingSize);
			return;
		case Scrollbar.Direction.BottomToTop:
			base.value = Mathf.Clamp01(handleCorner.y / remainingSize);
			return;
		case Scrollbar.Direction.TopToBottom:
			base.value = Mathf.Clamp01(1f - handleCorner.y / remainingSize);
			return;
		default:
			return;
		}
	}

	// Token: 0x060017CA RID: 6090 RVA: 0x00088970 File Offset: 0x00086B70
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this._isPointerInside = true;
		this._currentPointerData = eventData;
		if (this.IsInteractable() && this.InputModule != null)
		{
			XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(eventData.pointerId) as XRRayInteractor;
			if (xrrayInteractor != null)
			{
				xrrayInteractor.xrController.SendHapticImpulse(this._highlightedVibrationStrength, this._highlightedVibrationDuration);
			}
		}
	}

	// Token: 0x060017CB RID: 6091 RVA: 0x000889DA File Offset: 0x00086BDA
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this._isPointerInside = false;
	}

	// Token: 0x04002307 RID: 8967
	private float _highlightedVibrationStrength = 0.1f;

	// Token: 0x04002308 RID: 8968
	private float _highlightedVibrationDuration = 0.1f;

	// Token: 0x04002309 RID: 8969
	private RectTransform containerRect;

	// Token: 0x0400230A RID: 8970
	private bool _isPointerInside;

	// Token: 0x0400230B RID: 8971
	private bool _isHolding;

	// Token: 0x0400230C RID: 8972
	private PointerEventData _currentPointerData;

	// Token: 0x0400230D RID: 8973
	private Camera thirdPersonCamera;

	// Token: 0x020003EA RID: 1002
	private enum Axis
	{
		// Token: 0x0400230F RID: 8975
		Horizontal,
		// Token: 0x04002310 RID: 8976
		Vertical
	}
}
