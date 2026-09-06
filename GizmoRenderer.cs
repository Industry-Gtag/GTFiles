using System;
using Drawing;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000AF3 RID: 2803
public class GizmoRenderer : MonoBehaviour
{
	// Token: 0x060047DB RID: 18395 RVA: 0x00183EA4 File Offset: 0x001820A4
	private void Update()
	{
		this.RenderGizmos();
	}

	// Token: 0x060047DC RID: 18396 RVA: 0x00183EAC File Offset: 0x001820AC
	private unsafe void RenderGizmos()
	{
		if (this.renderMode == GizmoRenderer.RenderMode.Never)
		{
			return;
		}
		if (this.gizmos == null)
		{
			return;
		}
		int num = this.gizmos.Length;
		if (num == 0)
		{
			return;
		}
		CommandBuilder commandBuilder = *Draw.ingame;
		Transform transform = base.transform;
		for (int i = 0; i < num; i++)
		{
			GizmoRenderer.GizmoInfo gizmoInfo = this.gizmos[i];
			if (gizmoInfo.render)
			{
				Transform transform2 = (gizmoInfo.target ? gizmoInfo.target : transform);
				using (commandBuilder.InLocalSpace(transform2))
				{
					using (commandBuilder.WithLineWidth(gizmoInfo.lineWidth, false))
					{
						GizmoRenderer.gRenderFuncs[(int)gizmoInfo.type](commandBuilder, gizmoInfo);
					}
				}
			}
		}
	}

	// Token: 0x060047DD RID: 18397 RVA: 0x00183F98 File Offset: 0x00182198
	private static void RenderPlaneWire(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.WirePlane(gizmo.center, gizmo.rotation, gizmo.size.xz, gizmo.color);
	}

	// Token: 0x060047DE RID: 18398 RVA: 0x00183FBE File Offset: 0x001821BE
	private static void RenderPlaneSolid(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.SolidPlane(gizmo.center, gizmo.rotation, gizmo.size.xz, gizmo.color);
	}

	// Token: 0x060047DF RID: 18399 RVA: 0x00183FE4 File Offset: 0x001821E4
	private static void RenderGridWire(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.WireGrid(gizmo.center, gizmo.rotation, gizmo.gridCells, gizmo.size.xz, gizmo.color);
	}

	// Token: 0x060047E0 RID: 18400 RVA: 0x00184010 File Offset: 0x00182210
	private static void RenderBoxWire(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.WireBox(gizmo.center, gizmo.rotation, gizmo.size, gizmo.color);
	}

	// Token: 0x060047E1 RID: 18401 RVA: 0x00184031 File Offset: 0x00182231
	private static void RenderBoxSolid(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.SolidBox(gizmo.center, gizmo.rotation, gizmo.size, gizmo.color);
	}

	// Token: 0x060047E2 RID: 18402 RVA: 0x00184052 File Offset: 0x00182252
	private static void RenderSphereWire(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.WireSphere(gizmo.center, gizmo.radius * 0.5f, gizmo.color);
	}

	// Token: 0x060047E3 RID: 18403 RVA: 0x00184074 File Offset: 0x00182274
	private static void RenderSphereSolid(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		Matrix4x4 matrix4x = Matrix4x4.TRS(gizmo.center, quaternion.identity, new float3(gizmo.radius));
		using (draw.WithMatrix(matrix4x))
		{
			draw.SolidMesh(GizmoRenderer.gSphereMesh, gizmo.color);
		}
	}

	// Token: 0x060047E4 RID: 18404 RVA: 0x001840E8 File Offset: 0x001822E8
	private static void RenderLabel3D(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.Label3D(gizmo.center, gizmo.rotation, gizmo.text, gizmo.textSize * 0.1f, GizmoRenderer.gLabelAligns[(int)gizmo.textAlign], gizmo.color);
	}

	// Token: 0x060047E5 RID: 18405 RVA: 0x00184125 File Offset: 0x00182325
	private static void RenderLabel2D(CommandBuilder draw, GizmoRenderer.GizmoInfo gizmo)
	{
		draw.Label2D(gizmo.center, gizmo.text, gizmo.textSize * gizmo.textPPU, GizmoRenderer.gLabelAligns[(int)gizmo.textAlign], gizmo.color);
	}

	// Token: 0x060047E6 RID: 18406 RVA: 0x0018415F File Offset: 0x0018235F
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitializeOnLoad()
	{
		GizmoRenderer.gSphereMesh = Resources.GetBuiltinResource<Mesh>("New-Sphere.fbx");
	}

	// Token: 0x060047E7 RID: 18407 RVA: 0x00184170 File Offset: 0x00182370
	private static Color GetRandomColor()
	{
		Color color = Color.HSVToRGB((float)(DateTime.UtcNow.Ticks % 65536L) / 65535f, 1f, 1f, true);
		color.a = 1f;
		return color;
	}

	// Token: 0x04005A50 RID: 23120
	public GizmoRenderer.RenderMode renderMode = GizmoRenderer.RenderMode.Always;

	// Token: 0x04005A51 RID: 23121
	public bool includeInBuild;

	// Token: 0x04005A52 RID: 23122
	public GizmoRenderer.GizmoInfo[] gizmos = new GizmoRenderer.GizmoInfo[0];

	// Token: 0x04005A53 RID: 23123
	private static readonly Action<CommandBuilder, GizmoRenderer.GizmoInfo>[] gRenderFuncs = new Action<CommandBuilder, GizmoRenderer.GizmoInfo>[]
	{
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderBoxWire),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderBoxSolid),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderSphereWire),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderSphereSolid),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderLabel3D),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderLabel2D),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderGridWire),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderPlaneSolid),
		new Action<CommandBuilder, GizmoRenderer.GizmoInfo>(GizmoRenderer.RenderPlaneWire)
	};

	// Token: 0x04005A54 RID: 23124
	private static readonly LabelAlignment[] gLabelAligns = new LabelAlignment[]
	{
		LabelAlignment.Center,
		LabelAlignment.MiddleRight,
		LabelAlignment.MiddleLeft,
		LabelAlignment.BottomCenter,
		LabelAlignment.BottomRight,
		LabelAlignment.BottomLeft,
		LabelAlignment.TopRight,
		LabelAlignment.TopLeft,
		LabelAlignment.TopCenter
	};

	// Token: 0x04005A55 RID: 23125
	private static Mesh gSphereMesh;

	// Token: 0x02000AF4 RID: 2804
	[Serializable]
	public class GizmoInfo
	{
		// Token: 0x04005A56 RID: 23126
		public bool render = true;

		// Token: 0x04005A57 RID: 23127
		public GizmoRenderer.GizmoType type;

		// Token: 0x04005A58 RID: 23128
		public Color color = GizmoRenderer.GetRandomColor();

		// Token: 0x04005A59 RID: 23129
		public uint lineWidth = 1U;

		// Token: 0x04005A5A RID: 23130
		[Space]
		public Transform target;

		// Token: 0x04005A5B RID: 23131
		[Space]
		public float3 center = float3.zero;

		// Token: 0x04005A5C RID: 23132
		public float3 size = Vector3.one;

		// Token: 0x04005A5D RID: 23133
		public float radius = 1f;

		// Token: 0x04005A5E RID: 23134
		public quaternion rotation = quaternion.identity;

		// Token: 0x04005A5F RID: 23135
		[Space]
		public string text = string.Empty;

		// Token: 0x04005A60 RID: 23136
		public float textSize = 4f;

		// Token: 0x04005A61 RID: 23137
		public GizmoRenderer.TextAlign textAlign;

		// Token: 0x04005A62 RID: 23138
		public uint textPPU = 24U;

		// Token: 0x04005A63 RID: 23139
		[Space]
		public int2 gridCells = new int2(4);
	}

	// Token: 0x02000AF5 RID: 2805
	[Flags]
	public enum RenderMode : uint
	{
		// Token: 0x04005A65 RID: 23141
		Never = 0U,
		// Token: 0x04005A66 RID: 23142
		InEditor = 1U,
		// Token: 0x04005A67 RID: 23143
		InBuild = 2U,
		// Token: 0x04005A68 RID: 23144
		Always = 3U
	}

	// Token: 0x02000AF6 RID: 2806
	public enum GizmoType : uint
	{
		// Token: 0x04005A6A RID: 23146
		BoxWire,
		// Token: 0x04005A6B RID: 23147
		BoxSolid,
		// Token: 0x04005A6C RID: 23148
		SphereWire,
		// Token: 0x04005A6D RID: 23149
		SphereSolid,
		// Token: 0x04005A6E RID: 23150
		Label3D,
		// Token: 0x04005A6F RID: 23151
		Label2D,
		// Token: 0x04005A70 RID: 23152
		GridWire,
		// Token: 0x04005A71 RID: 23153
		PlaneSolid,
		// Token: 0x04005A72 RID: 23154
		PlaneWire
	}

	// Token: 0x02000AF7 RID: 2807
	public enum TextAlign : uint
	{
		// Token: 0x04005A74 RID: 23156
		Center,
		// Token: 0x04005A75 RID: 23157
		MiddleRight,
		// Token: 0x04005A76 RID: 23158
		MiddleLeft,
		// Token: 0x04005A77 RID: 23159
		BottomCenter,
		// Token: 0x04005A78 RID: 23160
		BottomRight,
		// Token: 0x04005A79 RID: 23161
		BottomLeft,
		// Token: 0x04005A7A RID: 23162
		TopRight,
		// Token: 0x04005A7B RID: 23163
		TopLeft,
		// Token: 0x04005A7C RID: 23164
		TopCenter
	}
}
