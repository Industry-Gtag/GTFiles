using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000171 RID: 369
public class SITechTreeUIPage : MonoBehaviour
{
	// Token: 0x060009C8 RID: 2504 RVA: 0x00034930 File Offset: 0x00032B30
	public void Configure(SITechTreeStation techTreeStation, SITechTreePage treePage, Transform imageTarget, Transform textTarget)
	{
		SITechTreeUIPage.<>c__DisplayClass5_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.techTreeStation = techTreeStation;
		CS$<>8__locals1.imageTarget = imageTarget;
		CS$<>8__locals1.textTarget = textTarget;
		base.name = treePage.nickName;
		this.id = treePage.pageId;
		int count = treePage.Roots.Count;
		Vector3 vector = new Vector3(0f, this.nodeContainer.rect.min.y + 20f, 0f);
		if (count < 2)
		{
			float num = (float)(treePage.Roots[0].GetSubtreeWidth(int.MaxValue) * 50 + 100);
			if (num > this.nodeContainer.rect.width)
			{
				float num2 = (this.nodeContainer.rect.width - num) / 2f;
				vector.x += num2;
			}
		}
		float num3 = this.nodeContainer.rect.width / (float)count;
		for (int i = 0; i < count; i++)
		{
			float num4 = ((count < 2) ? 0f : (-22f + -num3 * (float)(count - 1) / 2f + num3 * (float)i));
			this.<Configure>g__AddNodes|5_0(null, treePage.Roots[i], vector + new Vector3(num4, 0f, 0f), ref CS$<>8__locals1);
		}
		foreach (SITechTreeUINode sitechTreeUINode in this._pageNodes)
		{
			this.<Configure>g__AddUpgradeLines|5_1(sitechTreeUINode, ref CS$<>8__locals1);
			sitechTreeUINode.SetNodeLockStateColor(Color.black);
			CS$<>8__locals1.techTreeStation.AddButton(sitechTreeUINode.button, false);
		}
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x00034B00 File Offset: 0x00032D00
	private SITechTreeUINode GetUINode(SIUpgradeType upgradeType)
	{
		foreach (SITechTreeUINode sitechTreeUINode in this._pageNodes)
		{
			if (sitechTreeUINode.upgradeType == upgradeType)
			{
				return sitechTreeUINode;
			}
		}
		return null;
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x00034B5C File Offset: 0x00032D5C
	public void PopulateDefaultNodeData()
	{
		foreach (SITechTreeUINode sitechTreeUINode in this._pageNodes)
		{
			sitechTreeUINode.SetNodeLockStateColor(Color.black);
		}
	}

	// Token: 0x060009CB RID: 2507 RVA: 0x00034BB4 File Offset: 0x00032DB4
	public void PopulatePlayerNodeData(SIPlayer player)
	{
		foreach (SITechTreeUINode sitechTreeUINode in this._pageNodes)
		{
			Color color = (player.NodeResearched(sitechTreeUINode.upgradeType) ? Color.green : (player.NodeParentsUnlocked(sitechTreeUINode.upgradeType) ? Color.red : Color.black));
			sitechTreeUINode.SetNodeLockStateColor(color);
		}
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x00034C4C File Offset: 0x00032E4C
	[CompilerGenerated]
	private void <Configure>g__AddNodes|5_0(GraphNode<SITechTreeNode> parent, GraphNode<SITechTreeNode> node, Vector3 position, ref SITechTreeUIPage.<>c__DisplayClass5_0 A_4)
	{
		float num = (float)((parent == null) ? 40 : 25);
		int num2 = ((parent == null) ? 10 : 5);
		SITechTreeUIPage.<>c__DisplayClass5_1 CS$<>8__locals1;
		CS$<>8__locals1.subtreeWidths = new List<float>();
		float num3 = 50f;
		foreach (GraphNode<SITechTreeNode> graphNode in node.Children)
		{
			CS$<>8__locals1.subtreeWidths.Add(num3 * (float)graphNode.GetSubtreeWidth(int.MaxValue));
		}
		SITechTreeUINode sitechTreeUINode = this.<Configure>g__GetOrInstantiateUINode|5_2(node.Value.upgradeType, ref A_4);
		if (parent != null)
		{
			SITechTreeUINode uinode = this.GetUINode(parent.Value.upgradeType);
			sitechTreeUINode.Parents.Add(uinode);
			uinode.Children.Add(sitechTreeUINode);
		}
		if (sitechTreeUINode.IsConfigured)
		{
			if (sitechTreeUINode.Parents.Count > 1)
			{
				float num4 = 0f;
				foreach (SITechTreeUINode sitechTreeUINode2 in sitechTreeUINode.Parents)
				{
					num4 += sitechTreeUINode2.transform.localPosition.x;
				}
				position.x = num4 / (float)sitechTreeUINode.Parents.Count;
			}
			position.y = Mathf.Max(sitechTreeUINode.transform.localPosition.y, position.y);
			sitechTreeUINode.AdjustPosition(position - sitechTreeUINode.transform.localPosition);
			return;
		}
		sitechTreeUINode.transform.localPosition = position;
		sitechTreeUINode.SetTechTreeNode(A_4.techTreeStation, node.Value.upgradeType);
		this._pageNodes.Add(sitechTreeUINode);
		int count = node.Children.Count;
		float num5 = 0f;
		if (count > 1)
		{
			int num6 = 0;
			for (int i = 0; i < count; i++)
			{
				float num7 = CS$<>8__locals1.subtreeWidths[num6];
				float num8 = ((i == 0 || i == count - 1) ? (num7 / 2f) : num7);
				num5 -= num8 / 2f;
			}
		}
		for (int j = 0; j < count; j++)
		{
			float num9 = num + (float)((j + 1) % 2 * num2);
			GraphNode<SITechTreeNode> graphNode2 = node.Children[j];
			Vector3 vector = position + new Vector3(num5, num9, 0f);
			this.<Configure>g__AddNodes|5_0(node, graphNode2, vector, ref A_4);
			num5 += SITechTreeUIPage.<Configure>g__GetSpacing|5_3(j, count, ref CS$<>8__locals1);
		}
		sitechTreeUINode.imageFlattener.overrideParentTransform = A_4.imageTarget;
		sitechTreeUINode.textFlattener.overrideParentTransform = A_4.textTarget;
		sitechTreeUINode.imageFlattener.enabled = true;
		sitechTreeUINode.textFlattener.enabled = true;
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x00034F24 File Offset: 0x00033124
	[CompilerGenerated]
	private SITechTreeUINode <Configure>g__GetOrInstantiateUINode|5_2(SIUpgradeType upgradeType, ref SITechTreeUIPage.<>c__DisplayClass5_0 A_2)
	{
		SITechTreeUINode uinode = this.GetUINode(upgradeType);
		if (uinode)
		{
			return uinode;
		}
		return Object.Instantiate<SITechTreeUINode>(this.nodePrefab, this.nodeContainer);
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x00034F54 File Offset: 0x00033154
	[CompilerGenerated]
	internal static float <Configure>g__GetSpacing|5_3(int index, int childCount, ref SITechTreeUIPage.<>c__DisplayClass5_1 A_2)
	{
		int num = index + 1;
		float num2 = ((index >= 0 && index < childCount) ? A_2.subtreeWidths[index] : 0f);
		float num3 = ((num >= 0 && num < childCount) ? A_2.subtreeWidths[num] : 0f);
		return (num2 + num3) / 2f;
	}

	// Token: 0x060009D0 RID: 2512 RVA: 0x00034FA4 File Offset: 0x000331A4
	[CompilerGenerated]
	private void <Configure>g__AddUpgradeLines|5_1(SITechTreeUINode uiNode, ref SITechTreeUIPage.<>c__DisplayClass5_0 A_2)
	{
		foreach (SITechTreeUINode sitechTreeUINode in uiNode.Parents)
		{
			Vector3 localPosition = sitechTreeUINode.transform.localPosition;
			Vector3 vector = uiNode.transform.localPosition - localPosition;
			Vector3 normalized = vector.normalized;
			Image image = Object.Instantiate<Image>(this.upgradeLinePrefab, this.nodeContainer);
			ObjectHierarchyFlattener component = image.GetComponent<ObjectHierarchyFlattener>();
			image.transform.SetSiblingIndex(0);
			uiNode.UpgradeLines.Add(image);
			RectTransform rectTransform = image.rectTransform;
			rectTransform.localPosition = localPosition + vector * 0.5f;
			rectTransform.localRotation = Quaternion.FromToRotation(Vector3.up, normalized);
			Vector2 sizeDelta = rectTransform.sizeDelta;
			sizeDelta.y = vector.magnitude - 20f;
			rectTransform.sizeDelta = sizeDelta;
			component.overrideParentTransform = A_2.imageTarget;
			component.enabled = true;
		}
	}

	// Token: 0x04000BEB RID: 3051
	[SerializeField]
	private SITechTreeUINode nodePrefab;

	// Token: 0x04000BEC RID: 3052
	[SerializeField]
	private Image upgradeLinePrefab;

	// Token: 0x04000BED RID: 3053
	[SerializeField]
	private RectTransform nodeContainer;

	// Token: 0x04000BEE RID: 3054
	public SITechTreePageId id;

	// Token: 0x04000BEF RID: 3055
	private readonly List<SITechTreeUINode> _pageNodes = new List<SITechTreeUINode>();
}
