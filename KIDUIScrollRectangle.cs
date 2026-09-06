using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

// Token: 0x020003EB RID: 1003
[AddComponentMenu("UI/KIDUI Scroll Rect", 37)]
public class KIDUIScrollRectangle : ScrollRect, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x17000252 RID: 594
	// (get) Token: 0x060017CD RID: 6093 RVA: 0x00088695 File Offset: 0x00086895
	private XRUIInputModule InputModule
	{
		get
		{
			return EventSystem.current.currentInputModule as XRUIInputModule;
		}
	}

	// Token: 0x060017CE RID: 6094 RVA: 0x00088A08 File Offset: 0x00086C08
	protected override void OnEnable()
	{
		base.OnEnable();
		this.thirdPersonCamera = GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>();
		if (ControllerBehaviour.Instance != null)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x060017CF RID: 6095 RVA: 0x00088A48 File Offset: 0x00086C48
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

	// Token: 0x060017D0 RID: 6096 RVA: 0x00088A84 File Offset: 0x00086C84
	private void PostUpdate()
	{
		if (this._currentPointerData == null || this.InputModule == null)
		{
			return;
		}
		if (this._currentPointerData.hovered.Contains(base.viewport.gameObject) && !this._currentPointerData.hovered.Contains(base.verticalScrollbar.gameObject))
		{
			this._isPointerInside = true;
		}
		else
		{
			this._isPointerInside = false;
		}
		if (!ControllerBehaviour.Instance.TriggerDown)
		{
			this._isHolding = false;
			return;
		}
		XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(this._currentPointerData.pointerId) as XRRayInteractor;
		if (xrrayInteractor == null)
		{
			return;
		}
		XRRayInteractor xrrayInteractor2 = xrrayInteractor;
		RaycastResult raycastResult;
		if (!xrrayInteractor2.TryGetCurrentUIRaycastResult(out raycastResult))
		{
			return;
		}
		Vector2 vector;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(base.viewRect, raycastResult.screenPosition, this.thirdPersonCamera, out vector);
		if (!this._isHolding && this._isPointerInside && ControllerBehaviour.Instance.TriggerDown)
		{
			this._isHolding = true;
			this.m_PointerStartLocalCursor = vector;
			this.m_ContentStartPosition = base.content.anchoredPosition;
		}
		if (!this._isHolding)
		{
			return;
		}
		base.UpdateBounds();
		Vector2 vector2 = vector - this.m_PointerStartLocalCursor;
		Vector2 vector3 = this.m_ContentStartPosition + vector2;
		this.SetContentAnchoredPosition(vector3);
	}

	// Token: 0x060017D1 RID: 6097 RVA: 0x00088BC0 File Offset: 0x00086DC0
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (eventData.hovered.Contains(base.viewport.gameObject))
		{
			this._isPointerInside = true;
			this._currentPointerData = eventData;
		}
	}

	// Token: 0x060017D2 RID: 6098 RVA: 0x00088BE8 File Offset: 0x00086DE8
	public void OnPointerExit(PointerEventData eventData)
	{
		this._isPointerInside = false;
	}

	// Token: 0x04002311 RID: 8977
	private bool _isPointerInside;

	// Token: 0x04002312 RID: 8978
	private bool _isHolding;

	// Token: 0x04002313 RID: 8979
	private PointerEventData _currentPointerData;

	// Token: 0x04002314 RID: 8980
	private Vector2 m_PointerStartLocalCursor = Vector2.zero;

	// Token: 0x04002315 RID: 8981
	private Camera thirdPersonCamera;
}
