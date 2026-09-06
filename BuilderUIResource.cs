using System;
using GorillaTagScripts;
using TMPro;
using UnityEngine;

// Token: 0x0200066F RID: 1647
public class BuilderUIResource : MonoBehaviour
{
	// Token: 0x0600290E RID: 10510 RVA: 0x000DEECC File Offset: 0x000DD0CC
	public void SetResourceCost(BuilderResourceQuantity resourceCost, BuilderTable table)
	{
		BuilderResourceType type = resourceCost.type;
		int count = resourceCost.count;
		int availableResources = table.GetAvailableResources(type);
		if (this.resourceNameLabel != null)
		{
			this.resourceNameLabel.text = this.GetResourceName(type);
		}
		if (this.costLabel != null)
		{
			this.costLabel.text = count.ToString();
		}
		if (this.availableLabel != null)
		{
			this.availableLabel.text = availableResources.ToString();
		}
	}

	// Token: 0x0600290F RID: 10511 RVA: 0x000DEF4F File Offset: 0x000DD14F
	private string GetResourceName(BuilderResourceType type)
	{
		switch (type)
		{
		case BuilderResourceType.Basic:
			return "Basic";
		case BuilderResourceType.Decorative:
			return "Decorative";
		case BuilderResourceType.Functional:
			return "Functional";
		default:
			return "Resource Needs Name";
		}
	}

	// Token: 0x0400357A RID: 13690
	public TextMeshPro resourceNameLabel;

	// Token: 0x0400357B RID: 13691
	public TextMeshPro costLabel;

	// Token: 0x0400357C RID: 13692
	public TextMeshPro availableLabel;
}
