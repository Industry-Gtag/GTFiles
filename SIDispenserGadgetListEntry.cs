using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200013C RID: 316
public class SIDispenserGadgetListEntry : MonoBehaviour
{
	// Token: 0x1700009D RID: 157
	// (get) Token: 0x060007E4 RID: 2020 RVA: 0x0002B256 File Offset: 0x00029456
	public SITouchscreenButtonContainer DispenseButton
	{
		get
		{
			return this.dispenseButton;
		}
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x0002B260 File Offset: 0x00029460
	public void SetStation(ITouchScreenStation station, Transform imageTarget, Transform textTarget)
	{
		this.dispenseButton.button.buttonPressed.RemoveAllListeners();
		this.dispenseButton.button.buttonPressed.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int>(station.TouchscreenButtonPressed));
		this.infoButton.button.buttonPressed.RemoveAllListeners();
		this.infoButton.button.buttonPressed.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int>(station.TouchscreenButtonPressed));
		station.AddButton(this.dispenseButton.button, false);
		station.AddButton(this.infoButton.button, false);
		this.image1.overrideParentTransform = imageTarget;
		this.image2.overrideParentTransform = imageTarget;
		this.text1.overrideParentTransform = textTarget;
		this.text2.overrideParentTransform = textTarget;
		this.image1.enabled = true;
		this.image2.enabled = true;
		this.text1.enabled = true;
		this.text2.enabled = true;
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x0002B360 File Offset: 0x00029560
	public void SetTechTreeNode(SITechTreeNode node)
	{
		base.name = (this.gadgetText.text = node.nickName);
		int nodeId = node.upgradeType.GetNodeId();
		SIDispenserGadgetListEntry.<SetTechTreeNode>g__ConfigureButton|10_0(this.dispenseButton.button, SITouchscreenButton.SITouchscreenButtonType.Dispense, nodeId);
		SIDispenserGadgetListEntry.<SetTechTreeNode>g__ConfigureButton|10_0(this.infoButton.button, SITouchscreenButton.SITouchscreenButtonType.Select, nodeId);
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x0002B3B7 File Offset: 0x000295B7
	[CompilerGenerated]
	internal static void <SetTechTreeNode>g__ConfigureButton|10_0(SITouchscreenButton button, SITouchscreenButton.SITouchscreenButtonType type, int data)
	{
		button.buttonType = type;
		button.data = data;
	}

	// Token: 0x040009FA RID: 2554
	[SerializeField]
	private TextMeshProUGUI gadgetText;

	// Token: 0x040009FB RID: 2555
	[SerializeField]
	private SITouchscreenButtonContainer dispenseButton;

	// Token: 0x040009FC RID: 2556
	[SerializeField]
	private SITouchscreenButtonContainer infoButton;

	// Token: 0x040009FD RID: 2557
	public ObjectHierarchyFlattener image1;

	// Token: 0x040009FE RID: 2558
	public ObjectHierarchyFlattener image2;

	// Token: 0x040009FF RID: 2559
	public ObjectHierarchyFlattener text1;

	// Token: 0x04000A00 RID: 2560
	public ObjectHierarchyFlattener text2;
}
