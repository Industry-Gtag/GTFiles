using System;

// Token: 0x02000820 RID: 2080
public class GRToolScannable : GRScannable
{
	// Token: 0x0600355E RID: 13662 RVA: 0x001255D0 File Offset: 0x001237D0
	public override void Start()
	{
		base.Start();
		if (this.gameEntity != null)
		{
			this.tool = this.gameEntity.GetComponent<GRTool>();
			this.upgradePiece = this.gameEntity.GetComponent<GRToolUpgradePiece>();
		}
	}

	// Token: 0x0600355F RID: 13663 RVA: 0x00125608 File Offset: 0x00123808
	private void FetchMetadata(GhostReactor reactor)
	{
		if (this.metadata == null)
		{
			GRToolProgressionManager.ToolParts toolParts = GRToolProgressionManager.ToolParts.None;
			if (this.tool != null)
			{
				toolParts = GRUtils.GetToolPart(this.tool.toolType);
			}
			else if (this.upgradePiece != null)
			{
				toolParts = this.upgradePiece.matchingUpgrade;
			}
			if (toolParts != GRToolProgressionManager.ToolParts.None)
			{
				this.metadata = reactor.toolProgression.GetPartMetadata(toolParts);
			}
		}
	}

	// Token: 0x06003560 RID: 13664 RVA: 0x0012566F File Offset: 0x0012386F
	public override string GetTitleText(GhostReactor reactor)
	{
		this.FetchMetadata(reactor);
		if (this.metadata == null)
		{
			return "Unknown";
		}
		return this.metadata.name;
	}

	// Token: 0x06003561 RID: 13665 RVA: 0x00125691 File Offset: 0x00123891
	public override string GetBodyText(GhostReactor reactor)
	{
		this.FetchMetadata(reactor);
		if (this.metadata == null)
		{
			return "Unknown";
		}
		return this.metadata.description;
	}

	// Token: 0x06003562 RID: 13666 RVA: 0x001256B3 File Offset: 0x001238B3
	public override string GetAnnotationText(GhostReactor reactor)
	{
		this.FetchMetadata(reactor);
		if (this.metadata == null)
		{
			return "Unknown";
		}
		return this.metadata.annotation;
	}

	// Token: 0x0400459D RID: 17821
	private GRTool tool;

	// Token: 0x0400459E RID: 17822
	private GRToolUpgradePiece upgradePiece;

	// Token: 0x0400459F RID: 17823
	private GRToolProgressionManager.ToolProgressionMetaData metadata;
}
