using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000558 RID: 1368
public class TransferrableObjectSyncedBool : TransferrableObject
{
	// Token: 0x060022D1 RID: 8913 RVA: 0x000BB15B File Offset: 0x000B935B
	internal override void OnEnable()
	{
		base.OnEnable();
		this.OnItemStateBoolFalse.AddListener(new UnityAction(this.OnItemStateChanged));
		this.OnItemStateBoolTrue.AddListener(new UnityAction(this.OnItemStateChanged));
	}

	// Token: 0x060022D2 RID: 8914 RVA: 0x000BB191 File Offset: 0x000B9391
	internal override void OnDisable()
	{
		base.OnDisable();
		this.OnItemStateBoolFalse.RemoveListener(new UnityAction(this.OnItemStateChanged));
		this.OnItemStateBoolTrue.RemoveListener(new UnityAction(this.OnItemStateChanged));
	}

	// Token: 0x060022D3 RID: 8915 RVA: 0x000BB1C8 File Offset: 0x000B93C8
	public void SetItemState(bool state)
	{
		base.SetItemStateBool(state);
	}

	// Token: 0x060022D4 RID: 8916 RVA: 0x000BB1DC File Offset: 0x000B93DC
	public void ToggleItemState()
	{
		base.ToggleNetworkedItemStateBool();
	}

	// Token: 0x060022D5 RID: 8917 RVA: 0x000BB1EF File Offset: 0x000B93EF
	private void OnItemStateChanged()
	{
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			UnityEvent onItemStateSetFalse = this.OnItemStateSetFalse;
			if (onItemStateSetFalse == null)
			{
				return;
			}
			onItemStateSetFalse.Invoke();
			return;
		}
		else
		{
			UnityEvent onItemStateSetTrue = this.OnItemStateSetTrue;
			if (onItemStateSetTrue == null)
			{
				return;
			}
			onItemStateSetTrue.Invoke();
			return;
		}
	}

	// Token: 0x04002DEA RID: 11754
	[SerializeField]
	private bool deprecatedWarning = true;

	// Token: 0x04002DEB RID: 11755
	[SerializeField]
	private UnityEvent OnItemStateSetTrue;

	// Token: 0x04002DEC RID: 11756
	[SerializeField]
	private UnityEvent OnItemStateSetFalse;
}
