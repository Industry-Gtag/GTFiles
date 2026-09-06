using System;

// Token: 0x0200083C RID: 2108
public class GRUtils
{
	// Token: 0x06003632 RID: 13874 RVA: 0x0012BB60 File Offset: 0x00129D60
	public static string GetToolName(GRTool.GRToolType toolType)
	{
		switch (toolType)
		{
		case GRTool.GRToolType.Club:
			return "Baton";
		case GRTool.GRToolType.Collector:
			return "Collector";
		case GRTool.GRToolType.Flash:
			return "Flash";
		case GRTool.GRToolType.Lantern:
			return "Lantern";
		case GRTool.GRToolType.Revive:
			return "Revive";
		case GRTool.GRToolType.ShieldGun:
			return "Shield";
		case GRTool.GRToolType.DirectionalShield:
			return "Deflector";
		case GRTool.GRToolType.DockWrist:
			return "Dock";
		case GRTool.GRToolType.HockeyStick:
			return "Stick";
		}
		return "Unknown";
	}

	// Token: 0x06003633 RID: 13875 RVA: 0x0012BBE0 File Offset: 0x00129DE0
	public static GRToolProgressionManager.ToolParts GetToolPart(GRTool.GRToolType toolType)
	{
		switch (toolType)
		{
		case GRTool.GRToolType.Club:
			return GRToolProgressionManager.ToolParts.Baton;
		case GRTool.GRToolType.Collector:
			return GRToolProgressionManager.ToolParts.Collector;
		case GRTool.GRToolType.Flash:
			return GRToolProgressionManager.ToolParts.Flash;
		case GRTool.GRToolType.Lantern:
			return GRToolProgressionManager.ToolParts.Lantern;
		case GRTool.GRToolType.Revive:
			return GRToolProgressionManager.ToolParts.Revive;
		case GRTool.GRToolType.ShieldGun:
			return GRToolProgressionManager.ToolParts.ShieldGun;
		case GRTool.GRToolType.DirectionalShield:
			return GRToolProgressionManager.ToolParts.DirectionalShield;
		case GRTool.GRToolType.DockWrist:
			return GRToolProgressionManager.ToolParts.DockWrist;
		case GRTool.GRToolType.HockeyStick:
			return GRToolProgressionManager.ToolParts.HockeyStick;
		}
		return GRToolProgressionManager.ToolParts.None;
	}
}
