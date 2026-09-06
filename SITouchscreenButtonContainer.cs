using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000179 RID: 377
public class SITouchscreenButtonContainer : MonoBehaviour
{
	// Token: 0x170000DE RID: 222
	// (get) Token: 0x060009E4 RID: 2532 RVA: 0x000353CE File Offset: 0x000335CE
	// (set) Token: 0x060009E5 RID: 2533 RVA: 0x000353D6 File Offset: 0x000335D6
	public bool isUsable { get; private set; }

	// Token: 0x060009E6 RID: 2534 RVA: 0x000353E0 File Offset: 0x000335E0
	private void Start()
	{
		if (Application.isPlaying && this.button != null && this.button.buttonMode == SITouchscreenButton.ButtonMode.Toggle)
		{
			this.button.buttonToggled.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int, bool>(this.OnToggleStateChanged));
			this.UpdateToggleVisual(this.button.IsToggledOn);
		}
	}

	// Token: 0x060009E7 RID: 2535 RVA: 0x0003543D File Offset: 0x0003363D
	private void OnToggleStateChanged(SITouchscreenButton.SITouchscreenButtonType type, int data, int actorNr, bool isToggledOn)
	{
		this.UpdateToggleVisual(isToggledOn);
	}

	// Token: 0x060009E8 RID: 2536 RVA: 0x00035447 File Offset: 0x00033647
	public void UpdateToggleVisual()
	{
		this.UpdateToggleVisual(this.button.IsToggledOn);
	}

	// Token: 0x060009E9 RID: 2537 RVA: 0x0003545C File Offset: 0x0003365C
	private void UpdateToggleVisual(bool isToggledOn)
	{
		if (this._cachedForegroundColor.r < 0f)
		{
			this._cachedForegroundColor = this.foreGround.color;
		}
		this.foreGround.color = (isToggledOn ? this.toggleOnColor : this.toggleOffColor);
		this.buttonText.text = (isToggledOn ? this.toggleOnText : this.toggleOffText);
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x000354C4 File Offset: 0x000336C4
	public void SetUsable(bool newIsUsable)
	{
		if (this._cachedForegroundColor.r < 0f)
		{
			this._cachedForegroundColor = this.foreGround.color;
		}
		this.isUsable = newIsUsable;
		if (this.button.buttonMode == SITouchscreenButton.ButtonMode.Normal)
		{
			this.foreGround.color = (newIsUsable ? this._cachedForegroundColor : Color.gray);
		}
		this.button.isUsable = newIsUsable;
	}

	// Token: 0x04000C1B RID: 3099
	public SITouchscreenButton.SITouchscreenButtonType type;

	// Token: 0x04000C1C RID: 3100
	public string buttonTextString;

	// Token: 0x04000C1D RID: 3101
	public int data;

	// Token: 0x04000C1E RID: 3102
	public RectTransform backGround;

	// Token: 0x04000C1F RID: 3103
	public RectTransform backgroundShadow;

	// Token: 0x04000C20 RID: 3104
	public Image foreGround;

	// Token: 0x04000C21 RID: 3105
	public TextMeshProUGUI buttonText;

	// Token: 0x04000C22 RID: 3106
	public ITouchScreenStation station;

	// Token: 0x04000C23 RID: 3107
	[Header("Toggle Visual Settings")]
	public Color toggleOnColor = new Color(0f, 1f, 0.345098f);

	// Token: 0x04000C24 RID: 3108
	public Color toggleOffColor = new Color(0.5f, 0.5f, 0.5f);

	// Token: 0x04000C25 RID: 3109
	[Header("Toggle Text Settings")]
	[Tooltip("Text to display when toggle is ON")]
	public string toggleOnText = "ON";

	// Token: 0x04000C26 RID: 3110
	[Tooltip("Text to display when toggle is OFF")]
	public string toggleOffText = "OFF";

	// Token: 0x04000C27 RID: 3111
	public SITouchscreenButton button;

	// Token: 0x04000C28 RID: 3112
	[SerializeField]
	private bool autoConfigure = true;

	// Token: 0x04000C2A RID: 3114
	[NonSerialized]
	private Color _cachedForegroundColor = new Color(-1f, -1f, -1f);
}
