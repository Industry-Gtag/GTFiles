using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000142 RID: 322
public class SIGadgetListEntry : MonoBehaviour
{
	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x06000812 RID: 2066 RVA: 0x0002C5AB File Offset: 0x0002A7AB
	public SITouchscreenButtonContainer ButtonContainer
	{
		get
		{
			return this.buttonContainer;
		}
	}

	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x06000813 RID: 2067 RVA: 0x0002C5B3 File Offset: 0x0002A7B3
	// (set) Token: 0x06000814 RID: 2068 RVA: 0x0002C5BB File Offset: 0x0002A7BB
	public int Id { get; private set; } = -1;

	// Token: 0x06000815 RID: 2069 RVA: 0x0002C5C4 File Offset: 0x0002A7C4
	public void Configure(ITouchScreenStation station, SITechTreePage page, Transform imageTarget, Transform textTarget, SITouchscreenButton.SITouchscreenButtonType buttonType = SITouchscreenButton.SITouchscreenButtonType.Select, int index = 0, float positionInterval = 0f, int listSize = 0)
	{
		base.name = (this.gadgetText.text = page.nickName);
		SITouchscreenButton button = this.buttonContainer.button;
		button.buttonType = buttonType;
		this.Id = (button.data = (int)page.pageId);
		button.buttonPressed.RemoveAllListeners();
		button.buttonPressed.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int>(station.TouchscreenButtonPressed));
		station.AddButton(button, false);
		float num = (float)Mathf.Max(listSize - 1, 0) * -(positionInterval / 2f);
		base.transform.localPosition += new Vector3(0f, num + (float)index * positionInterval, 0f);
		this.imageFlattener.overrideParentTransform = imageTarget;
		this.textFlattener.overrideParentTransform = textTarget;
		this.imageFlattener.enabled = true;
		this.textFlattener.enabled = true;
		this.buttonContainer.SetUsable(page.IsAllowed);
	}

	// Token: 0x04000A34 RID: 2612
	[SerializeField]
	private TextMeshProUGUI gadgetText;

	// Token: 0x04000A35 RID: 2613
	[SerializeField]
	private SITouchscreenButtonContainer buttonContainer;

	// Token: 0x04000A36 RID: 2614
	public ObjectHierarchyFlattener imageFlattener;

	// Token: 0x04000A37 RID: 2615
	public ObjectHierarchyFlattener textFlattener;

	// Token: 0x04000A38 RID: 2616
	public GameObject selectionIndicator;
}
