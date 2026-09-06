using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001ED RID: 493
public class TriggerZoneObjectToggler : MonoBehaviour
{
	// Token: 0x06000CF6 RID: 3318 RVA: 0x00047392 File Offset: 0x00045592
	private void Awake()
	{
		this.ToggleObject.SetActive(false);
	}

	// Token: 0x06000CF7 RID: 3319 RVA: 0x000473A0 File Offset: 0x000455A0
	private void OnDisable()
	{
		if (!ApplicationQuittingState.IsQuitting)
		{
			this.HandleExit();
		}
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x000473AF File Offset: 0x000455AF
	private void OnTriggerEnter(Collider other)
	{
		if (this.IsMatchingTrigger(other))
		{
			this.HandleEnter();
		}
	}

	// Token: 0x06000CF9 RID: 3321 RVA: 0x000473C0 File Offset: 0x000455C0
	private void OnTriggerExit(Collider other)
	{
		if (this.IsMatchingTrigger(other))
		{
			this.HandleExit();
		}
	}

	// Token: 0x06000CFA RID: 3322 RVA: 0x000473D1 File Offset: 0x000455D1
	private void HandleEnter()
	{
		if (this._inTriggerZone)
		{
			return;
		}
		this._inTriggerZone = true;
		this.ToggleObject.SetActive(true);
		UnityEvent onEnter = this.OnEnter;
		if (onEnter == null)
		{
			return;
		}
		onEnter.Invoke();
	}

	// Token: 0x06000CFB RID: 3323 RVA: 0x000473FF File Offset: 0x000455FF
	private void HandleExit()
	{
		if (!this._inTriggerZone)
		{
			return;
		}
		this._inTriggerZone = false;
		this.ToggleObject.SetActive(false);
		UnityEvent onExit = this.OnExit;
		if (onExit == null)
		{
			return;
		}
		onExit.Invoke();
	}

	// Token: 0x06000CFC RID: 3324 RVA: 0x00047430 File Offset: 0x00045630
	private bool IsMatchingTrigger(Collider other)
	{
		NamedTriggerZone component = other.GetComponent<NamedTriggerZone>();
		return component != null && component.TriggerName == this.TriggerName;
	}

	// Token: 0x04000F9F RID: 3999
	public string TriggerName = "Trigger";

	// Token: 0x04000FA0 RID: 4000
	public GameObject ToggleObject;

	// Token: 0x04000FA1 RID: 4001
	public UnityEvent OnEnter;

	// Token: 0x04000FA2 RID: 4002
	public UnityEvent OnExit;

	// Token: 0x04000FA3 RID: 4003
	private bool _inTriggerZone;
}
