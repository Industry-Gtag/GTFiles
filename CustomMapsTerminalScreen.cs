using System;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000ACB RID: 2763
public abstract class CustomMapsTerminalScreen : MonoBehaviour
{
	// Token: 0x060046E0 RID: 18144
	public abstract void Initialize();

	// Token: 0x060046E1 RID: 18145 RVA: 0x0017E9B0 File Offset: 0x0017CBB0
	public virtual void Show()
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
			CustomMapsKeyboard customMapsKeyboard = this.terminalKeyboard;
			if (customMapsKeyboard != null)
			{
				customMapsKeyboard.OnKeyPressed.AddListener(new UnityAction<CustomMapKeyboardBinding>(this.PressButton));
			}
		}
		this.showTime = Time.time;
	}

	// Token: 0x060046E2 RID: 18146 RVA: 0x0017EA04 File Offset: 0x0017CC04
	public virtual void Hide()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(false);
			CustomMapsKeyboard customMapsKeyboard = this.terminalKeyboard;
			if (customMapsKeyboard != null)
			{
				customMapsKeyboard.OnKeyPressed.RemoveListener(new UnityAction<CustomMapKeyboardBinding>(this.PressButton));
			}
		}
		this.showTime = 0f;
	}

	// Token: 0x060046E3 RID: 18147 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void PressButton(CustomMapKeyboardBinding pressedButton)
	{
	}

	// Token: 0x04005970 RID: 22896
	public CustomMapsKeyboard terminalKeyboard;

	// Token: 0x04005971 RID: 22897
	[SerializeField]
	protected float activationTime = 0.25f;

	// Token: 0x04005972 RID: 22898
	protected float showTime;
}
