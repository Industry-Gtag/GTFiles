using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000D19 RID: 3353
public class SimpleKeyboardButton : SimpleButton
{
	// Token: 0x170007D6 RID: 2006
	// (get) Token: 0x0600532F RID: 21295 RVA: 0x001B719F File Offset: 0x001B539F
	public string KeyValue
	{
		get
		{
			return this.keyValue;
		}
	}

	// Token: 0x170007D7 RID: 2007
	// (get) Token: 0x06005330 RID: 21296 RVA: 0x001B71A7 File Offset: 0x001B53A7
	public SimpleKeyboardButton.ButtonFunction Function
	{
		get
		{
			return this.buttonFunction;
		}
	}

	// Token: 0x06005331 RID: 21297 RVA: 0x001B71AF File Offset: 0x001B53AF
	protected override void handlePress(bool isLeft)
	{
		UnityEvent<string> keyPress = this.KeyPress;
		if (keyPress != null)
		{
			keyPress.Invoke(this.keyValue);
		}
		Action<SimpleKeyboardButton, bool> onKeyPress = this.OnKeyPress;
		if (onKeyPress == null)
		{
			return;
		}
		onKeyPress(this, isLeft);
	}

	// Token: 0x040064CE RID: 25806
	[SerializeField]
	private string keyValue;

	// Token: 0x040064CF RID: 25807
	[SerializeField]
	private SimpleKeyboardButton.ButtonFunction buttonFunction;

	// Token: 0x040064D0 RID: 25808
	[SerializeField]
	private UnityEvent<string> KeyPress;

	// Token: 0x040064D1 RID: 25809
	public Action<SimpleKeyboardButton, bool> OnKeyPress;

	// Token: 0x02000D1A RID: 3354
	public enum ButtonFunction
	{
		// Token: 0x040064D3 RID: 25811
		NONE,
		// Token: 0x040064D4 RID: 25812
		CURSOR_BACK,
		// Token: 0x040064D5 RID: 25813
		CURSOR_FWD,
		// Token: 0x040064D6 RID: 25814
		DELETE
	}
}
