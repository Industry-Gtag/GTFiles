using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000170 RID: 368
public class SITechTreeUINode : MonoBehaviour
{
	// Token: 0x170000D6 RID: 214
	// (get) Token: 0x060009BE RID: 2494 RVA: 0x0003464C File Offset: 0x0003284C
	public List<Image> UpgradeLines { get; } = new List<Image>();

	// Token: 0x170000D7 RID: 215
	// (get) Token: 0x060009BF RID: 2495 RVA: 0x00034654 File Offset: 0x00032854
	public List<SITechTreeUINode> Parents { get; } = new List<SITechTreeUINode>();

	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x060009C0 RID: 2496 RVA: 0x0003465C File Offset: 0x0003285C
	public List<SITechTreeUINode> Children { get; } = new List<SITechTreeUINode>();

	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x060009C1 RID: 2497 RVA: 0x00034664 File Offset: 0x00032864
	public bool IsConfigured
	{
		get
		{
			return this._node != null;
		}
	}

	// Token: 0x060009C2 RID: 2498 RVA: 0x00034670 File Offset: 0x00032870
	public void SetTechTreeNode(SITechTreeStation techTreeStation, SIUpgradeType nodeUpgradeType)
	{
		if (!techTreeStation.techTreeSO.TryGetNode(nodeUpgradeType, out this._node))
		{
			Debug.LogError(string.Format("Node {0} doesn't exist in tree.  Disabling.", nodeUpgradeType));
			base.gameObject.SetActive(false);
			return;
		}
		this.upgradeType = nodeUpgradeType;
		float num = (float)(Mathf.Min(this.GetMaxWordLength(this._node.Value.nickName), 14) * 4);
		Vector2 sizeDelta = this.nodeNickName.rectTransform.sizeDelta;
		if (sizeDelta.x < num)
		{
			sizeDelta.x = num;
			this.nodeNickName.rectTransform.sizeDelta = sizeDelta;
		}
		base.name = (this.nodeNickName.text = this._node.Value.nickName);
		this.button.data = this._node.Value.upgradeType.GetNodeId();
		this.button.buttonPressed.RemoveAllListeners();
		this.button.buttonPressed.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int>(techTreeStation.TouchscreenButtonPressed));
		this.SetGadgetUnlockNode(this._node.Value.unlockedGadgetPrefab);
	}

	// Token: 0x060009C3 RID: 2499 RVA: 0x0003479C File Offset: 0x0003299C
	public void SetNodeLockStateColor(Color color)
	{
		if (color == Color.red)
		{
			this.circle.sharedMaterial = this.redMat;
		}
		else if (color == Color.black)
		{
			this.circle.sharedMaterial = this.blackMat;
		}
		else if (color == Color.green)
		{
			this.circle.sharedMaterial = this.greenMat;
		}
		foreach (Image image in this.UpgradeLines)
		{
			image.color = color;
		}
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x0003484C File Offset: 0x00032A4C
	private void SetGadgetUnlockNode(bool isUnlockNode)
	{
		this.triangle.gameObject.SetActive(isUnlockNode);
	}

	// Token: 0x060009C5 RID: 2501 RVA: 0x00034860 File Offset: 0x00032A60
	private int GetMaxWordLength(string text)
	{
		string[] array = text.Split(' ', StringSplitOptions.None);
		int num = 0;
		foreach (string text2 in array)
		{
			if (text2.Length > num)
			{
				num = text2.Length;
			}
		}
		return num;
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x0003489C File Offset: 0x00032A9C
	public void AdjustPosition(Vector3 positionOffset)
	{
		base.transform.localPosition += positionOffset;
		foreach (SITechTreeUINode sitechTreeUINode in this.Children)
		{
			sitechTreeUINode.AdjustPosition(positionOffset);
		}
	}

	// Token: 0x04000BDD RID: 3037
	public SIUpgradeType upgradeType;

	// Token: 0x04000BDE RID: 3038
	public TextMeshProUGUI nodeNickName;

	// Token: 0x04000BDF RID: 3039
	public MeshRenderer circle;

	// Token: 0x04000BE0 RID: 3040
	public MeshRenderer triangle;

	// Token: 0x04000BE1 RID: 3041
	public SITouchscreenButton button;

	// Token: 0x04000BE2 RID: 3042
	public Material greenMat;

	// Token: 0x04000BE3 RID: 3043
	public Material redMat;

	// Token: 0x04000BE4 RID: 3044
	public Material blackMat;

	// Token: 0x04000BE5 RID: 3045
	public ObjectHierarchyFlattener imageFlattener;

	// Token: 0x04000BE6 RID: 3046
	public ObjectHierarchyFlattener textFlattener;

	// Token: 0x04000BEA RID: 3050
	private GraphNode<SITechTreeNode> _node;
}
