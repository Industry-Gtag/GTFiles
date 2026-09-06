using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000D18 RID: 3352
public class SimpleKeyboard : ObservableBehavior
{
	// Token: 0x06005326 RID: 21286 RVA: 0x001B6E78 File Offset: 0x001B5078
	private void Start()
	{
		for (int i = 0; i < this.buttons.Length; i++)
		{
			this.btnPos.Add(this.buttons[i], this.buttons[i].transform.localPosition);
		}
	}

	// Token: 0x06005327 RID: 21287 RVA: 0x001B6EC0 File Offset: 0x001B50C0
	protected override void UnityOnEnable()
	{
		for (int i = 0; i < this.buttons.Length; i++)
		{
			SimpleKeyboardButton simpleKeyboardButton = this.buttons[i];
			simpleKeyboardButton.OnKeyPress = (Action<SimpleKeyboardButton, bool>)Delegate.Combine(simpleKeyboardButton.OnKeyPress, new Action<SimpleKeyboardButton, bool>(this.buttonPress));
		}
	}

	// Token: 0x06005328 RID: 21288 RVA: 0x001B6F0C File Offset: 0x001B510C
	private void buttonPress(SimpleKeyboardButton b, bool isLeft)
	{
		if (Time.time - this.pressTime < this.coolDown)
		{
			return;
		}
		this.pressTime = Time.time;
		this.lastButton = b;
		switch (b.Function)
		{
		case SimpleKeyboardButton.ButtonFunction.CURSOR_BACK:
			this.typingTarget.MoveCursor(-1);
			break;
		case SimpleKeyboardButton.ButtonFunction.CURSOR_FWD:
			this.typingTarget.MoveCursor(1);
			break;
		case SimpleKeyboardButton.ButtonFunction.DELETE:
			this.typingTarget.Delete();
			break;
		default:
			this.typingTarget.Append(b.KeyValue);
			break;
		}
		if (this.audioClipIndex > 0)
		{
			GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.audioClipIndex, isLeft, 0.05f);
			GorillaTagger.Instance.StartVibration(isLeft, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x06005329 RID: 21289 RVA: 0x001B6FE0 File Offset: 0x001B51E0
	protected override void UnityOnDisable()
	{
		for (int i = 0; i < this.buttons.Length; i++)
		{
			SimpleKeyboardButton simpleKeyboardButton = this.buttons[i];
			simpleKeyboardButton.OnKeyPress = (Action<SimpleKeyboardButton, bool>)Delegate.Remove(simpleKeyboardButton.OnKeyPress, new Action<SimpleKeyboardButton, bool>(this.buttonPress));
		}
	}

	// Token: 0x0600532A RID: 21290 RVA: 0x001B702C File Offset: 0x001B522C
	protected override void OnLostObservable()
	{
		for (int i = 0; i < this.buttons.Length; i++)
		{
			this.buttons[i].transform.localPosition = this.btnPos[this.buttons[i]];
		}
	}

	// Token: 0x0600532B RID: 21291 RVA: 0x001B7074 File Offset: 0x001B5274
	protected override void OnBecameObservable()
	{
		for (int i = 0; i < this.buttons.Length; i++)
		{
			this.buttons[i].transform.localPosition = this.btnPos[this.buttons[i]];
		}
	}

	// Token: 0x0600532C RID: 21292 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void ObservableSliceUpdate()
	{
	}

	// Token: 0x0600532D RID: 21293 RVA: 0x001B70BC File Offset: 0x001B52BC
	private void LateUpdate()
	{
		if (!this.observable)
		{
			return;
		}
		if (this.lastButton == null)
		{
			return;
		}
		if (Time.time - this.pressTime >= this.coolDown)
		{
			this.lastButton.transform.localPosition = this.btnPos[this.lastButton];
			this.lastButton = null;
			return;
		}
		this.lastButton.transform.localPosition = this.btnPos[this.lastButton] + new Vector3(0f, (Time.time - this.pressTime) / this.coolDown * -this.keyTravel, 0f);
	}

	// Token: 0x040064C6 RID: 25798
	private SimpleKeyboardButton lastButton;

	// Token: 0x040064C7 RID: 25799
	private float pressTime;

	// Token: 0x040064C8 RID: 25800
	[SerializeField]
	private float coolDown = 0.1f;

	// Token: 0x040064C9 RID: 25801
	[SerializeField]
	private TypingTarget typingTarget;

	// Token: 0x040064CA RID: 25802
	[SerializeField]
	private SimpleKeyboardButton[] buttons;

	// Token: 0x040064CB RID: 25803
	[SerializeField]
	private int audioClipIndex = 67;

	// Token: 0x040064CC RID: 25804
	private Dictionary<SimpleKeyboardButton, Vector3> btnPos = new Dictionary<SimpleKeyboardButton, Vector3>();

	// Token: 0x040064CD RID: 25805
	[SerializeField]
	private float keyTravel = 0.01f;
}
