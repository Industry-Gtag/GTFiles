using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

// Token: 0x02000D6E RID: 3438
public static class FPSInput
{
	// Token: 0x1700081E RID: 2078
	// (get) Token: 0x060054DF RID: 21727 RVA: 0x001BD8BE File Offset: 0x001BBABE
	public static bool IsMouseCaptured
	{
		get
		{
			return Cursor.lockState == CursorLockMode.Locked;
		}
	}

	// Token: 0x060054E0 RID: 21728 RVA: 0x001BD8C8 File Offset: 0x001BBAC8
	public static void SetMouseCaptured(bool captured)
	{
		Cursor.lockState = (captured ? CursorLockMode.Locked : CursorLockMode.None);
	}

	// Token: 0x060054E1 RID: 21729 RVA: 0x001BD8D8 File Offset: 0x001BBAD8
	public static bool IsPressed(FpsKey key)
	{
		ButtonControl control = FPSInput.GetControl(key);
		return control != null && control.isPressed;
	}

	// Token: 0x060054E2 RID: 21730 RVA: 0x001BD8F8 File Offset: 0x001BBAF8
	public static bool WasPressedThisFrame(FpsKey key)
	{
		ButtonControl control = FPSInput.GetControl(key);
		return control != null && control.wasPressedThisFrame;
	}

	// Token: 0x060054E3 RID: 21731 RVA: 0x001BD918 File Offset: 0x001BBB18
	public static bool WasReleasedThisFrame(FpsKey key)
	{
		ButtonControl control = FPSInput.GetControl(key);
		return control != null && control.wasReleasedThisFrame;
	}

	// Token: 0x060054E4 RID: 21732 RVA: 0x001BD937 File Offset: 0x001BBB37
	public static float Value(FpsKey key)
	{
		if (!FPSInput.IsPressed(key))
		{
			return 0f;
		}
		return 1f;
	}

	// Token: 0x1700081F RID: 2079
	// (get) Token: 0x060054E5 RID: 21733 RVA: 0x001BD94C File Offset: 0x001BBB4C
	public static bool LeftButton
	{
		get
		{
			return Mouse.current != null && Mouse.current.leftButton.isPressed;
		}
	}

	// Token: 0x17000820 RID: 2080
	// (get) Token: 0x060054E6 RID: 21734 RVA: 0x001BD966 File Offset: 0x001BBB66
	public static bool LeftButtonDown
	{
		get
		{
			return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
		}
	}

	// Token: 0x17000821 RID: 2081
	// (get) Token: 0x060054E7 RID: 21735 RVA: 0x001BD980 File Offset: 0x001BBB80
	public static bool RightButton
	{
		get
		{
			return Mouse.current != null && Mouse.current.rightButton.isPressed;
		}
	}

	// Token: 0x17000822 RID: 2082
	// (get) Token: 0x060054E8 RID: 21736 RVA: 0x001BD99A File Offset: 0x001BBB9A
	public static bool RightButtonDown
	{
		get
		{
			return Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame;
		}
	}

	// Token: 0x17000823 RID: 2083
	// (get) Token: 0x060054E9 RID: 21737 RVA: 0x001BD9B4 File Offset: 0x001BBBB4
	public static Vector2 MouseDelta
	{
		get
		{
			if (Mouse.current == null)
			{
				return Vector2.zero;
			}
			return Mouse.current.delta.ReadValue();
		}
	}

	// Token: 0x17000824 RID: 2084
	// (get) Token: 0x060054EA RID: 21738 RVA: 0x001BD9D2 File Offset: 0x001BBBD2
	public static Vector2 MousePosition
	{
		get
		{
			if (Mouse.current == null)
			{
				return Vector2.zero;
			}
			return Mouse.current.position.ReadValue();
		}
	}

	// Token: 0x060054EB RID: 21739 RVA: 0x001BD9F0 File Offset: 0x001BBBF0
	private static ButtonControl GetControl(FpsKey key)
	{
		Mouse current = Mouse.current;
		switch (key)
		{
		case FpsKey.LMouse:
			if (current == null)
			{
				return null;
			}
			return current.leftButton;
		case FpsKey.RMouse:
			if (current == null)
			{
				return null;
			}
			return current.rightButton;
		case FpsKey.MMouse:
			if (current == null)
			{
				return null;
			}
			return current.middleButton;
		}
		Keyboard current2 = Keyboard.current;
		if (current2 == null)
		{
			return null;
		}
		switch (key)
		{
		case FpsKey.Shift:
			return current2.shiftKey;
		case FpsKey.Ctrl:
			return current2.ctrlKey;
		case FpsKey.Alt:
			return current2.altKey;
		default:
		{
			Key key2 = FPSInput.ToInputSystemKey(key);
			if (key2 != Key.None)
			{
				return current2[key2];
			}
			return null;
		}
		}
	}

	// Token: 0x060054EC RID: 21740 RVA: 0x001BDA8C File Offset: 0x001BBC8C
	private static Key ToInputSystemKey(FpsKey key)
	{
		switch (key)
		{
		case FpsKey.Back:
			return Key.Backspace;
		case FpsKey.Tab:
			return Key.Tab;
		case (FpsKey)10:
		case (FpsKey)11:
		case (FpsKey)12:
		case (FpsKey)14:
		case (FpsKey)15:
		case FpsKey.Shift:
		case FpsKey.Ctrl:
		case FpsKey.Alt:
		case (FpsKey)19:
		case (FpsKey)20:
		case (FpsKey)21:
		case (FpsKey)22:
		case (FpsKey)23:
		case (FpsKey)24:
		case (FpsKey)25:
		case (FpsKey)26:
		case (FpsKey)28:
		case (FpsKey)29:
		case (FpsKey)30:
		case (FpsKey)31:
		case (FpsKey)33:
		case (FpsKey)34:
		case (FpsKey)35:
		case (FpsKey)36:
		case (FpsKey)41:
		case (FpsKey)42:
		case (FpsKey)43:
		case (FpsKey)44:
		case (FpsKey)45:
		case (FpsKey)46:
		case (FpsKey)47:
		case (FpsKey)58:
		case (FpsKey)59:
		case (FpsKey)60:
		case (FpsKey)61:
		case (FpsKey)62:
		case (FpsKey)63:
		case (FpsKey)64:
			break;
		case FpsKey.Return:
			return Key.Enter;
		case FpsKey.Escape:
			return Key.Escape;
		case FpsKey.Space:
			return Key.Space;
		case FpsKey.Left:
			return Key.LeftArrow;
		case FpsKey.Up:
			return Key.UpArrow;
		case FpsKey.Right:
			return Key.RightArrow;
		case FpsKey.Down:
			return Key.DownArrow;
		case FpsKey.Digit0:
			return Key.Digit0;
		case FpsKey.Digit1:
			return Key.Digit1;
		case FpsKey.Digit2:
			return Key.Digit2;
		case FpsKey.Digit3:
			return Key.Digit3;
		case FpsKey.Digit4:
			return Key.Digit4;
		case FpsKey.Digit5:
			return Key.Digit5;
		case FpsKey.Digit6:
			return Key.Digit6;
		case FpsKey.Digit7:
			return Key.Digit7;
		case FpsKey.Digit8:
			return Key.Digit8;
		case FpsKey.Digit9:
			return Key.Digit9;
		case FpsKey.A:
			return Key.A;
		case FpsKey.B:
			return Key.B;
		case FpsKey.C:
			return Key.C;
		case FpsKey.D:
			return Key.D;
		case FpsKey.E:
			return Key.E;
		case FpsKey.F:
			return Key.F;
		case FpsKey.G:
			return Key.G;
		case FpsKey.H:
			return Key.H;
		case FpsKey.I:
			return Key.I;
		case FpsKey.J:
			return Key.J;
		case FpsKey.K:
			return Key.K;
		case FpsKey.L:
			return Key.L;
		case FpsKey.M:
			return Key.M;
		case FpsKey.N:
			return Key.N;
		case FpsKey.O:
			return Key.O;
		case FpsKey.P:
			return Key.P;
		case FpsKey.Q:
			return Key.Q;
		case FpsKey.R:
			return Key.R;
		case FpsKey.S:
			return Key.S;
		case FpsKey.T:
			return Key.T;
		case FpsKey.U:
			return Key.U;
		case FpsKey.V:
			return Key.V;
		case FpsKey.W:
			return Key.W;
		case FpsKey.X:
			return Key.X;
		case FpsKey.Y:
			return Key.Y;
		case FpsKey.Z:
			return Key.Z;
		default:
			switch (key)
			{
			case FpsKey.F1:
				return Key.F1;
			case FpsKey.F2:
				return Key.F2;
			case FpsKey.F3:
				return Key.F3;
			case FpsKey.F4:
				return Key.F4;
			case FpsKey.F5:
				return Key.F5;
			case FpsKey.F6:
				return Key.F6;
			case FpsKey.F7:
				return Key.F7;
			case FpsKey.F8:
				return Key.F8;
			case FpsKey.F9:
				return Key.F9;
			case FpsKey.F10:
				return Key.F10;
			case FpsKey.F11:
				return Key.F11;
			case FpsKey.F12:
				return Key.F12;
			default:
				switch (key)
				{
				case FpsKey.LeftShift:
					return Key.LeftShift;
				case FpsKey.RightShift:
					return Key.RightShift;
				case FpsKey.LeftCtrl:
					return Key.LeftCtrl;
				case FpsKey.RightCtrl:
					return Key.RightCtrl;
				case FpsKey.LeftAlt:
					return Key.LeftAlt;
				case FpsKey.RightAlt:
					return Key.RightAlt;
				}
				break;
			}
			break;
		}
		return Key.None;
	}
}
