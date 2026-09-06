using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

// Token: 0x0200012A RID: 298
[CreateAssetMenu(fileName = "TechTreeGadgetGraph", menuName = "SuperInfection/TechTree Gadget Graph")]
public class TechTreeGadgetGraph : NodeGraph
{
	// Token: 0x17000083 RID: 131
	// (get) Token: 0x06000765 RID: 1893 RVA: 0x00029B81 File Offset: 0x00027D81
	public GadgetNode[] GadgetNodes
	{
		get
		{
			return this.nodes.Select((Node n) => n as GadgetNode).ToArray<GadgetNode>();
		}
	}

	// Token: 0x17000084 RID: 132
	// (get) Token: 0x06000766 RID: 1894 RVA: 0x00029BB4 File Offset: 0x00027DB4
	public bool IsValid
	{
		get
		{
			EAssetReleaseTier eassetReleaseTier = this.releaseTier;
			if (eassetReleaseTier != EAssetReleaseTier.Disabled && eassetReleaseTier <= EAssetReleaseTier.PublicRC)
			{
				List<Node> nodes = this.nodes;
				return nodes != null && nodes.Count > 0;
			}
			return false;
		}
	}

	// Token: 0x0400099A RID: 2458
	public string nickName;

	// Token: 0x0400099B RID: 2459
	public SITechTreePageId pageId;

	// Token: 0x0400099C RID: 2460
	public Sprite icon;

	// Token: 0x0400099D RID: 2461
	public float costMultiplier = 1f;

	// Token: 0x0400099E RID: 2462
	public ESuperGameModes excludedGameModes;

	// Token: 0x0400099F RID: 2463
	public EAssetReleaseTier releaseTier;

	// Token: 0x040009A0 RID: 2464
	private const float XLayoutStep = 300f;

	// Token: 0x040009A1 RID: 2465
	private const float YLayoutStep = 250f;
}
