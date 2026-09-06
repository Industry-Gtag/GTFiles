using System;
using UnityEngine.Events;

// Token: 0x02000A3E RID: 2622
public class GorillaToggleActionButton : GorillaPressableButton
{
	// Token: 0x06004357 RID: 17239 RVA: 0x001667BB File Offset: 0x001649BB
	public override void Start()
	{
		this.BindToggleAction();
	}

	// Token: 0x06004358 RID: 17240 RVA: 0x001667C4 File Offset: 0x001649C4
	private void BindToggleAction()
	{
		if (this.ToggleAction == null || !this.ToggleAction.IsValid)
		{
			return;
		}
		this.ToggleAction.Cache();
		this.onPressButton = new UnityEvent();
		this.onPressButton.AddListener(new UnityAction(this.ExecuteToggleAction));
	}

	// Token: 0x06004359 RID: 17241 RVA: 0x00166814 File Offset: 0x00164A14
	private void ExecuteToggleAction()
	{
		ComponentFunctionReference<bool> toggleAction = this.ToggleAction;
		this.isOn = toggleAction != null && toggleAction.Invoke();
		this.UpdateColor();
	}

	// Token: 0x0400553C RID: 21820
	public ComponentFunctionReference<bool> ToggleAction;

	// Token: 0x0400553D RID: 21821
	private Func<bool> toggleFunc;
}
