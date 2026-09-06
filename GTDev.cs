using System;
using System.Diagnostics;
using Cysharp.Text;
using Drawing;
using UnityEngine;

// Token: 0x02000347 RID: 839
public static class GTDev
{
	// Token: 0x0600149E RID: 5278 RVA: 0x0006E49B File Offset: 0x0006C69B
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
	private static void InitializeOnLoad()
	{
		GTDev.FetchDevID();
	}

	// Token: 0x0600149F RID: 5279 RVA: 0x00002C2D File Offset: 0x00000E2D
	[HideInCallstack]
	public static void Log<T>(T msg, string channel = null)
	{
	}

	// Token: 0x060014A0 RID: 5280 RVA: 0x00002C2D File Offset: 0x00000E2D
	[HideInCallstack]
	public static void Log<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x060014A1 RID: 5281 RVA: 0x00002C2D File Offset: 0x00000E2D
	[HideInCallstack]
	public static void LogError<T>(T msg, string channel = null)
	{
	}

	// Token: 0x060014A2 RID: 5282 RVA: 0x00002C2D File Offset: 0x00000E2D
	[HideInCallstack]
	public static void LogError<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x060014A3 RID: 5283 RVA: 0x00002C2D File Offset: 0x00000E2D
	[HideInCallstack]
	public static void LogWarning<T>(T msg, string channel = null)
	{
	}

	// Token: 0x060014A4 RID: 5284 RVA: 0x00002C2D File Offset: 0x00000E2D
	[HideInCallstack]
	public static void LogWarning<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x060014A5 RID: 5285 RVA: 0x00002C2D File Offset: 0x00000E2D
	[HideInCallstack]
	public static void LogSilent<T>(T msg, string channel = null)
	{
	}

	// Token: 0x060014A6 RID: 5286 RVA: 0x00002C2D File Offset: 0x00000E2D
	[HideInCallstack]
	public static void LogSilent<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x060014A7 RID: 5287 RVA: 0x00002C2D File Offset: 0x00000E2D
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void LogEditorOnly<T>(T msg, string channel = null)
	{
	}

	// Token: 0x060014A8 RID: 5288 RVA: 0x00002C2D File Offset: 0x00000E2D
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void LogEditorOnly<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x060014A9 RID: 5289 RVA: 0x00002C2D File Offset: 0x00000E2D
	[HideInCallstack]
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogBetaOnly<T>(T msg, string channel = null)
	{
	}

	// Token: 0x060014AA RID: 5290 RVA: 0x00002C2D File Offset: 0x00000E2D
	[HideInCallstack]
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogBetaOnly<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x060014AB RID: 5291 RVA: 0x00002C2D File Offset: 0x00000E2D
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void LogErrorEd<T>(T msg, string channel = null)
	{
	}

	// Token: 0x060014AC RID: 5292 RVA: 0x00002C2D File Offset: 0x00000E2D
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void LogErrorEd<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x060014AD RID: 5293 RVA: 0x00002C2D File Offset: 0x00000E2D
	[HideInCallstack]
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogErrorBeta<T>(T msg, string channel = null)
	{
	}

	// Token: 0x060014AE RID: 5294 RVA: 0x00002C2D File Offset: 0x00000E2D
	[HideInCallstack]
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogErrorBeta<T>(T msg, Object context, string channel = null)
	{
	}

	// Token: 0x060014AF RID: 5295 RVA: 0x00002C2D File Offset: 0x00000E2D
	[HideInCallstack]
	[Conditional("UNITY_EDITOR")]
	public static void CallEditorOnly(Action call)
	{
	}

	// Token: 0x1700020F RID: 527
	// (get) Token: 0x060014B0 RID: 5296 RVA: 0x0006E4A3 File Offset: 0x0006C6A3
	public static int DevID
	{
		get
		{
			return GTDev.FetchDevID();
		}
	}

	// Token: 0x060014B1 RID: 5297 RVA: 0x0006E4AC File Offset: 0x0006C6AC
	private static int FetchDevID()
	{
		if (GTDev.gHasDevID)
		{
			return GTDev.gDevID;
		}
		int num = StaticHash.Compute(SystemInfo.deviceUniqueIdentifier);
		int num2 = StaticHash.Compute(Environment.UserDomainName);
		int num3 = StaticHash.Compute(Environment.UserName);
		int num4 = StaticHash.Compute(Application.unityVersion);
		GTDev.gDevID = StaticHash.Compute(num, num2, num3, num4);
		GTDev.gHasDevID = true;
		return GTDev.gDevID;
	}

	// Token: 0x060014B2 RID: 5298 RVA: 0x00002C2D File Offset: 0x00000E2D
	[HideInCallstack]
	[Conditional("_GTDEV_ON_")]
	private static void _Log<T>(Action<object, Object> log, Action<object> logNoCtx, T msg, Object ctx, string channel)
	{
	}

	// Token: 0x060014B3 RID: 5299 RVA: 0x0006E509 File Offset: 0x0006C709
	private static Mesh SphereMesh()
	{
		if (!GTDev.gSphereMesh)
		{
			GTDev.gSphereMesh = Resources.GetBuiltinResource<Mesh>("New-Sphere.fbx");
		}
		return GTDev.gSphereMesh;
	}

	// Token: 0x060014B4 RID: 5300 RVA: 0x0006E52C File Offset: 0x0006C72C
	[Conditional("_GTDEV_ON_")]
	public unsafe static void Ping3D(this Collider col, Color color = default(Color), float duration = 8f)
	{
		if (color == default(Color))
		{
			color = GTDev.gDefaultColor;
		}
		if (color.a.Approx0(1E-06f))
		{
			return;
		}
		Matrix4x4 localToWorldMatrix = col.transform.localToWorldMatrix;
		SRand srand = new SRand(localToWorldMatrix.QuantizedId128().GetHashCode());
		color.r = srand.NextFloat();
		color.g = srand.NextFloat();
		color.b = srand.NextFloat();
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithDuration(duration))
		{
			commandBuilder.PushMatrix(localToWorldMatrix);
			commandBuilder.PushLineWidth(2f, true);
			commandBuilder.PushColor(color);
			BoxCollider boxCollider = col as BoxCollider;
			if (boxCollider == null)
			{
				SphereCollider sphereCollider = col as SphereCollider;
				if (sphereCollider == null)
				{
					CapsuleCollider capsuleCollider = col as CapsuleCollider;
					if (capsuleCollider != null)
					{
						commandBuilder.WireCapsule(capsuleCollider.center, Vector3.up, capsuleCollider.height, capsuleCollider.radius, color);
					}
				}
				else
				{
					commandBuilder.WireSphere(sphereCollider.center, sphereCollider.radius, color);
				}
			}
			else
			{
				commandBuilder.WireBox(boxCollider.center, boxCollider.size);
			}
			commandBuilder.Label2D(Vector3.zero, col.name, 16f, LabelAlignment.Center);
			commandBuilder.PopColor();
			commandBuilder.PopLineWidth();
			commandBuilder.PopMatrix();
		}
	}

	// Token: 0x060014B5 RID: 5301 RVA: 0x0006E6D0 File Offset: 0x0006C8D0
	[Conditional("_GTDEV_ON_")]
	public unsafe static void Ping3D(this Vector3 vec, Color color = default(Color), float duration = 8f)
	{
		if (color == default(Color))
		{
			color = GTDev.gDefaultColor;
		}
		else
		{
			color.a = GTDev.gDefaultColor.a;
		}
		string text = ZString.Format<float, float, float>("{{ X: {0:##0.0000}, Y: {1:##0.0000}, Z: {2:##0.0000} }}", vec.x, vec.y, vec.z);
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithDuration(duration))
		{
			using (commandBuilder.WithLineWidth(2f, true))
			{
				commandBuilder.Cross(vec, 0.64f, color);
			}
			commandBuilder.Label2D(vec + Vector3.down * 0.64f, text, 16f, LabelAlignment.Center, color);
		}
	}

	// Token: 0x060014B6 RID: 5302 RVA: 0x0006E7C4 File Offset: 0x0006C9C4
	[Conditional("_GTDEV_ON_")]
	public unsafe static void Ping3D<T>(this T value, Vector3 position, Color color = default(Color), float duration = 8f)
	{
		if (color == default(Color))
		{
			color = GTDev.gDefaultColor;
		}
		string text = ZString.Concat<T>(value);
		CommandBuilder commandBuilder = *Draw.ingame;
		using (commandBuilder.WithDuration(duration))
		{
			commandBuilder.Label2D(position, text, 16f, LabelAlignment.Center, color);
		}
	}

	// Token: 0x04001962 RID: 6498
	[OnEnterPlay_Set(0)]
	private static int gDevID;

	// Token: 0x04001963 RID: 6499
	[OnEnterPlay_Set(false)]
	private static bool gHasDevID;

	// Token: 0x04001964 RID: 6500
	private static readonly Color gDefaultColor = new Color(0f, 1f, 1f, 0.32f);

	// Token: 0x04001965 RID: 6501
	private const string kFormatF = "{{ X: {0:##0.0000}, Y: {1:##0.0000}, Z: {2:##0.0000} }}";

	// Token: 0x04001966 RID: 6502
	private const float kDuration = 8f;

	// Token: 0x04001967 RID: 6503
	private static Mesh gSphereMesh;
}
