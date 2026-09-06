using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000099 RID: 153
public static class DebugExtension
{
	// Token: 0x060003B1 RID: 945 RVA: 0x00015354 File Offset: 0x00013554
	public static void DebugPoint(Vector3 position, Color color, float scale = 1f, float duration = 0f, bool depthTest = true)
	{
		color = ((color == default(Color)) ? Color.white : color);
		Debug.DrawRay(position + Vector3.up * (scale * 0.5f), -Vector3.up * scale, color, duration, depthTest);
		Debug.DrawRay(position + Vector3.right * (scale * 0.5f), -Vector3.right * scale, color, duration, depthTest);
		Debug.DrawRay(position + Vector3.forward * (scale * 0.5f), -Vector3.forward * scale, color, duration, depthTest);
	}

	// Token: 0x060003B2 RID: 946 RVA: 0x0001540C File Offset: 0x0001360C
	public static void DebugPoint(Vector3 position, float scale = 1f, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugPoint(position, Color.white, scale, duration, depthTest);
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x0001541C File Offset: 0x0001361C
	public static void DebugBounds(Bounds bounds, Color color, float duration = 0f, bool depthTest = true)
	{
		Vector3 center = bounds.center;
		float x = bounds.extents.x;
		float y = bounds.extents.y;
		float z = bounds.extents.z;
		Vector3 vector = center + new Vector3(x, y, z);
		Vector3 vector2 = center + new Vector3(x, y, -z);
		Vector3 vector3 = center + new Vector3(-x, y, z);
		Vector3 vector4 = center + new Vector3(-x, y, -z);
		Vector3 vector5 = center + new Vector3(x, -y, z);
		Vector3 vector6 = center + new Vector3(x, -y, -z);
		Vector3 vector7 = center + new Vector3(-x, -y, z);
		Vector3 vector8 = center + new Vector3(-x, -y, -z);
		Debug.DrawLine(vector, vector3, color, duration, depthTest);
		Debug.DrawLine(vector, vector2, color, duration, depthTest);
		Debug.DrawLine(vector3, vector4, color, duration, depthTest);
		Debug.DrawLine(vector2, vector4, color, duration, depthTest);
		Debug.DrawLine(vector, vector5, color, duration, depthTest);
		Debug.DrawLine(vector2, vector6, color, duration, depthTest);
		Debug.DrawLine(vector3, vector7, color, duration, depthTest);
		Debug.DrawLine(vector4, vector8, color, duration, depthTest);
		Debug.DrawLine(vector5, vector7, color, duration, depthTest);
		Debug.DrawLine(vector5, vector6, color, duration, depthTest);
		Debug.DrawLine(vector7, vector8, color, duration, depthTest);
		Debug.DrawLine(vector8, vector6, color, duration, depthTest);
	}

	// Token: 0x060003B4 RID: 948 RVA: 0x0001556E File Offset: 0x0001376E
	public static void DebugBounds(Bounds bounds, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugBounds(bounds, Color.white, duration, depthTest);
	}

	// Token: 0x060003B5 RID: 949 RVA: 0x00015580 File Offset: 0x00013780
	public static void DebugLocalCube(Transform transform, Vector3 size, Color color, Vector3 center = default(Vector3), float duration = 0f, bool depthTest = true)
	{
		Vector3 vector = transform.TransformPoint(center + -size * 0.5f);
		Vector3 vector2 = transform.TransformPoint(center + new Vector3(size.x, -size.y, -size.z) * 0.5f);
		Vector3 vector3 = transform.TransformPoint(center + new Vector3(size.x, -size.y, size.z) * 0.5f);
		Vector3 vector4 = transform.TransformPoint(center + new Vector3(-size.x, -size.y, size.z) * 0.5f);
		Vector3 vector5 = transform.TransformPoint(center + new Vector3(-size.x, size.y, -size.z) * 0.5f);
		Vector3 vector6 = transform.TransformPoint(center + new Vector3(size.x, size.y, -size.z) * 0.5f);
		Vector3 vector7 = transform.TransformPoint(center + size * 0.5f);
		Vector3 vector8 = transform.TransformPoint(center + new Vector3(-size.x, size.y, size.z) * 0.5f);
		Debug.DrawLine(vector, vector2, color, duration, depthTest);
		Debug.DrawLine(vector2, vector3, color, duration, depthTest);
		Debug.DrawLine(vector3, vector4, color, duration, depthTest);
		Debug.DrawLine(vector4, vector, color, duration, depthTest);
		Debug.DrawLine(vector5, vector6, color, duration, depthTest);
		Debug.DrawLine(vector6, vector7, color, duration, depthTest);
		Debug.DrawLine(vector7, vector8, color, duration, depthTest);
		Debug.DrawLine(vector8, vector5, color, duration, depthTest);
		Debug.DrawLine(vector, vector5, color, duration, depthTest);
		Debug.DrawLine(vector2, vector6, color, duration, depthTest);
		Debug.DrawLine(vector3, vector7, color, duration, depthTest);
		Debug.DrawLine(vector4, vector8, color, duration, depthTest);
	}

	// Token: 0x060003B6 RID: 950 RVA: 0x0001577F File Offset: 0x0001397F
	public static void DebugLocalCube(Transform transform, Vector3 size, Vector3 center = default(Vector3), float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugLocalCube(transform, size, Color.white, center, duration, depthTest);
	}

	// Token: 0x060003B7 RID: 951 RVA: 0x00015794 File Offset: 0x00013994
	public static void DebugLocalCube(Matrix4x4 space, Vector3 size, Color color, Vector3 center = default(Vector3), float duration = 0f, bool depthTest = true)
	{
		color = ((color == default(Color)) ? Color.white : color);
		Vector3 vector = space.MultiplyPoint3x4(center + -size * 0.5f);
		Vector3 vector2 = space.MultiplyPoint3x4(center + new Vector3(size.x, -size.y, -size.z) * 0.5f);
		Vector3 vector3 = space.MultiplyPoint3x4(center + new Vector3(size.x, -size.y, size.z) * 0.5f);
		Vector3 vector4 = space.MultiplyPoint3x4(center + new Vector3(-size.x, -size.y, size.z) * 0.5f);
		Vector3 vector5 = space.MultiplyPoint3x4(center + new Vector3(-size.x, size.y, -size.z) * 0.5f);
		Vector3 vector6 = space.MultiplyPoint3x4(center + new Vector3(size.x, size.y, -size.z) * 0.5f);
		Vector3 vector7 = space.MultiplyPoint3x4(center + size * 0.5f);
		Vector3 vector8 = space.MultiplyPoint3x4(center + new Vector3(-size.x, size.y, size.z) * 0.5f);
		Debug.DrawLine(vector, vector2, color, duration, depthTest);
		Debug.DrawLine(vector2, vector3, color, duration, depthTest);
		Debug.DrawLine(vector3, vector4, color, duration, depthTest);
		Debug.DrawLine(vector4, vector, color, duration, depthTest);
		Debug.DrawLine(vector5, vector6, color, duration, depthTest);
		Debug.DrawLine(vector6, vector7, color, duration, depthTest);
		Debug.DrawLine(vector7, vector8, color, duration, depthTest);
		Debug.DrawLine(vector8, vector5, color, duration, depthTest);
		Debug.DrawLine(vector, vector5, color, duration, depthTest);
		Debug.DrawLine(vector2, vector6, color, duration, depthTest);
		Debug.DrawLine(vector3, vector7, color, duration, depthTest);
		Debug.DrawLine(vector4, vector8, color, duration, depthTest);
	}

	// Token: 0x060003B8 RID: 952 RVA: 0x000159B7 File Offset: 0x00013BB7
	public static void DebugLocalCube(Matrix4x4 space, Vector3 size, Vector3 center = default(Vector3), float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugLocalCube(space, size, Color.white, center, duration, depthTest);
	}

	// Token: 0x060003B9 RID: 953 RVA: 0x000159CC File Offset: 0x00013BCC
	public static void DebugCircle(Vector3 position, Vector3 up, Color color, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		Vector3 vector = up.normalized * radius;
		Vector3 vector2 = Vector3.Slerp(vector, -vector, 0.5f);
		Vector3 vector3 = Vector3.Cross(vector, vector2).normalized * radius;
		Matrix4x4 matrix4x = default(Matrix4x4);
		matrix4x[0] = vector3.x;
		matrix4x[1] = vector3.y;
		matrix4x[2] = vector3.z;
		matrix4x[4] = vector.x;
		matrix4x[5] = vector.y;
		matrix4x[6] = vector.z;
		matrix4x[8] = vector2.x;
		matrix4x[9] = vector2.y;
		matrix4x[10] = vector2.z;
		Vector3 vector4 = position + matrix4x.MultiplyPoint3x4(new Vector3(Mathf.Cos(0f), 0f, Mathf.Sin(0f)));
		Vector3 vector5 = Vector3.zero;
		color = ((color == default(Color)) ? Color.white : color);
		for (int i = 0; i < 91; i++)
		{
			vector5.x = Mathf.Cos((float)(i * 4) * 0.017453292f);
			vector5.z = Mathf.Sin((float)(i * 4) * 0.017453292f);
			vector5.y = 0f;
			vector5 = position + matrix4x.MultiplyPoint3x4(vector5);
			Debug.DrawLine(vector4, vector5, color, duration, depthTest);
			vector4 = vector5;
		}
	}

	// Token: 0x060003BA RID: 954 RVA: 0x00015B56 File Offset: 0x00013D56
	public static void DebugCircle(Vector3 position, Color color, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugCircle(position, Vector3.up, color, radius, duration, depthTest);
	}

	// Token: 0x060003BB RID: 955 RVA: 0x00015B68 File Offset: 0x00013D68
	public static void DebugCircle(Vector3 position, Vector3 up, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugCircle(position, up, Color.white, radius, duration, depthTest);
	}

	// Token: 0x060003BC RID: 956 RVA: 0x00015B7A File Offset: 0x00013D7A
	public static void DebugCircle(Vector3 position, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugCircle(position, Vector3.up, Color.white, radius, duration, depthTest);
	}

	// Token: 0x060003BD RID: 957 RVA: 0x00015B90 File Offset: 0x00013D90
	public static void DebugWireSphere(Vector3 position, Color color, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		float num = 10f;
		Vector3 vector = new Vector3(position.x, position.y + radius * Mathf.Sin(0f), position.z + radius * Mathf.Cos(0f));
		Vector3 vector2 = new Vector3(position.x + radius * Mathf.Cos(0f), position.y, position.z + radius * Mathf.Sin(0f));
		Vector3 vector3 = new Vector3(position.x + radius * Mathf.Cos(0f), position.y + radius * Mathf.Sin(0f), position.z);
		for (int i = 1; i < 37; i++)
		{
			Vector3 vector4 = new Vector3(position.x, position.y + radius * Mathf.Sin(num * (float)i * 0.017453292f), position.z + radius * Mathf.Cos(num * (float)i * 0.017453292f));
			Vector3 vector5 = new Vector3(position.x + radius * Mathf.Cos(num * (float)i * 0.017453292f), position.y, position.z + radius * Mathf.Sin(num * (float)i * 0.017453292f));
			Vector3 vector6 = new Vector3(position.x + radius * Mathf.Cos(num * (float)i * 0.017453292f), position.y + radius * Mathf.Sin(num * (float)i * 0.017453292f), position.z);
			Debug.DrawLine(vector, vector4, color, duration, depthTest);
			Debug.DrawLine(vector2, vector5, color, duration, depthTest);
			Debug.DrawLine(vector3, vector6, color, duration, depthTest);
			vector = vector4;
			vector2 = vector5;
			vector3 = vector6;
		}
	}

	// Token: 0x060003BE RID: 958 RVA: 0x00015D3D File Offset: 0x00013F3D
	public static void DebugWireSphere(Vector3 position, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugWireSphere(position, Color.white, radius, duration, depthTest);
	}

	// Token: 0x060003BF RID: 959 RVA: 0x00015D50 File Offset: 0x00013F50
	public static void DebugCylinder(Vector3 start, Vector3 end, Color color, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		Vector3 vector = (end - start).normalized * radius;
		Vector3 vector2 = Vector3.Slerp(vector, -vector, 0.5f);
		Vector3 vector3 = Vector3.Cross(vector, vector2).normalized * radius;
		DebugExtension.DebugCircle(start, vector, color, radius, duration, depthTest);
		DebugExtension.DebugCircle(end, -vector, color, radius, duration, depthTest);
		DebugExtension.DebugCircle((start + end) * 0.5f, vector, color, radius, duration, depthTest);
		Debug.DrawLine(start + vector3, end + vector3, color, duration, depthTest);
		Debug.DrawLine(start - vector3, end - vector3, color, duration, depthTest);
		Debug.DrawLine(start + vector2, end + vector2, color, duration, depthTest);
		Debug.DrawLine(start - vector2, end - vector2, color, duration, depthTest);
		Debug.DrawLine(start - vector3, start + vector3, color, duration, depthTest);
		Debug.DrawLine(start - vector2, start + vector2, color, duration, depthTest);
		Debug.DrawLine(end - vector3, end + vector3, color, duration, depthTest);
		Debug.DrawLine(end - vector2, end + vector2, color, duration, depthTest);
	}

	// Token: 0x060003C0 RID: 960 RVA: 0x00015E97 File Offset: 0x00014097
	public static void DebugCylinder(Vector3 start, Vector3 end, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugCylinder(start, end, Color.white, radius, duration, depthTest);
	}

	// Token: 0x060003C1 RID: 961 RVA: 0x00015EAC File Offset: 0x000140AC
	public static void DebugCone(Vector3 position, Vector3 direction, Color color, float angle = 45f, float duration = 0f, bool depthTest = true)
	{
		float magnitude = direction.magnitude;
		Vector3 vector = direction;
		Vector3 vector2 = Vector3.Slerp(vector, -vector, 0.5f);
		Vector3 vector3 = Vector3.Cross(vector, vector2).normalized * magnitude;
		direction = direction.normalized;
		Vector3 vector4 = Vector3.Slerp(vector, vector2, angle / 90f);
		Plane plane = new Plane(-direction, position + vector);
		Ray ray = new Ray(position, vector4);
		float num;
		plane.Raycast(ray, out num);
		Debug.DrawRay(position, vector4.normalized * num, color);
		Debug.DrawRay(position, Vector3.Slerp(vector, -vector2, angle / 90f).normalized * num, color, duration, depthTest);
		Debug.DrawRay(position, Vector3.Slerp(vector, vector3, angle / 90f).normalized * num, color, duration, depthTest);
		Debug.DrawRay(position, Vector3.Slerp(vector, -vector3, angle / 90f).normalized * num, color, duration, depthTest);
		DebugExtension.DebugCircle(position + vector, direction, color, (vector - vector4.normalized * num).magnitude, duration, depthTest);
		DebugExtension.DebugCircle(position + vector * 0.5f, direction, color, (vector * 0.5f - vector4.normalized * (num * 0.5f)).magnitude, duration, depthTest);
	}

	// Token: 0x060003C2 RID: 962 RVA: 0x00016041 File Offset: 0x00014241
	public static void DebugCone(Vector3 position, Vector3 direction, float angle = 45f, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugCone(position, direction, Color.white, angle, duration, depthTest);
	}

	// Token: 0x060003C3 RID: 963 RVA: 0x00016053 File Offset: 0x00014253
	public static void DebugCone(Vector3 position, Color color, float angle = 45f, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugCone(position, Vector3.up, color, angle, duration, depthTest);
	}

	// Token: 0x060003C4 RID: 964 RVA: 0x00016065 File Offset: 0x00014265
	public static void DebugCone(Vector3 position, float angle = 45f, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugCone(position, Vector3.up, Color.white, angle, duration, depthTest);
	}

	// Token: 0x060003C5 RID: 965 RVA: 0x0001607A File Offset: 0x0001427A
	public static void DebugArrow(Vector3 position, Vector3 direction, Color color, float duration = 0f, bool depthTest = true)
	{
		Debug.DrawRay(position, direction, color, duration, depthTest);
		DebugExtension.DebugCone(position + direction, -direction * 0.333f, color, 15f, duration, depthTest);
	}

	// Token: 0x060003C6 RID: 966 RVA: 0x000160AC File Offset: 0x000142AC
	public static void DebugArrow(Vector3 position, Vector3 direction, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugArrow(position, direction, Color.white, duration, depthTest);
	}

	// Token: 0x060003C7 RID: 967 RVA: 0x000160BC File Offset: 0x000142BC
	public static void DebugCapsule(Vector3 start, Vector3 end, Color color, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		Vector3 vector = (end - start).normalized * radius;
		Vector3 vector2 = Vector3.Slerp(vector, -vector, 0.5f);
		Vector3 vector3 = Vector3.Cross(vector, vector2).normalized * radius;
		float magnitude = (start - end).magnitude;
		float num = Mathf.Max(0f, magnitude * 0.5f - radius);
		Vector3 vector4 = (end + start) * 0.5f;
		start = vector4 + (start - vector4).normalized * num;
		end = vector4 + (end - vector4).normalized * num;
		DebugExtension.DebugCircle(start, vector, color, radius, duration, depthTest);
		DebugExtension.DebugCircle(end, -vector, color, radius, duration, depthTest);
		Debug.DrawLine(start + vector3, end + vector3, color, duration, depthTest);
		Debug.DrawLine(start - vector3, end - vector3, color, duration, depthTest);
		Debug.DrawLine(start + vector2, end + vector2, color, duration, depthTest);
		Debug.DrawLine(start - vector2, end - vector2, color, duration, depthTest);
		for (int i = 1; i < 26; i++)
		{
			Debug.DrawLine(Vector3.Slerp(vector3, -vector, (float)i / 25f) + start, Vector3.Slerp(vector3, -vector, (float)(i - 1) / 25f) + start, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(-vector3, -vector, (float)i / 25f) + start, Vector3.Slerp(-vector3, -vector, (float)(i - 1) / 25f) + start, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(vector2, -vector, (float)i / 25f) + start, Vector3.Slerp(vector2, -vector, (float)(i - 1) / 25f) + start, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(-vector2, -vector, (float)i / 25f) + start, Vector3.Slerp(-vector2, -vector, (float)(i - 1) / 25f) + start, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(vector3, vector, (float)i / 25f) + end, Vector3.Slerp(vector3, vector, (float)(i - 1) / 25f) + end, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(-vector3, vector, (float)i / 25f) + end, Vector3.Slerp(-vector3, vector, (float)(i - 1) / 25f) + end, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(vector2, vector, (float)i / 25f) + end, Vector3.Slerp(vector2, vector, (float)(i - 1) / 25f) + end, color, duration, depthTest);
			Debug.DrawLine(Vector3.Slerp(-vector2, vector, (float)i / 25f) + end, Vector3.Slerp(-vector2, vector, (float)(i - 1) / 25f) + end, color, duration, depthTest);
		}
	}

	// Token: 0x060003C8 RID: 968 RVA: 0x0001642A File Offset: 0x0001462A
	public static void DebugCapsule(Vector3 start, Vector3 end, float radius = 1f, float duration = 0f, bool depthTest = true)
	{
		DebugExtension.DebugCapsule(start, end, Color.white, radius, duration, depthTest);
	}

	// Token: 0x060003C9 RID: 969 RVA: 0x0001643C File Offset: 0x0001463C
	public static void DrawPoint(Vector3 position, Color color, float scale = 1f)
	{
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawRay(position + Vector3.up * (scale * 0.5f), -Vector3.up * scale);
		Gizmos.DrawRay(position + Vector3.right * (scale * 0.5f), -Vector3.right * scale);
		Gizmos.DrawRay(position + Vector3.forward * (scale * 0.5f), -Vector3.forward * scale);
		Gizmos.color = color2;
	}

	// Token: 0x060003CA RID: 970 RVA: 0x000164DD File Offset: 0x000146DD
	public static void DrawPoint(Vector3 position, float scale = 1f)
	{
		DebugExtension.DrawPoint(position, Color.white, scale);
	}

	// Token: 0x060003CB RID: 971 RVA: 0x000164EC File Offset: 0x000146EC
	public static void DrawBounds(Bounds bounds, Color color)
	{
		Vector3 center = bounds.center;
		float x = bounds.extents.x;
		float y = bounds.extents.y;
		float z = bounds.extents.z;
		Vector3 vector = center + new Vector3(x, y, z);
		Vector3 vector2 = center + new Vector3(x, y, -z);
		Vector3 vector3 = center + new Vector3(-x, y, z);
		Vector3 vector4 = center + new Vector3(-x, y, -z);
		Vector3 vector5 = center + new Vector3(x, -y, z);
		Vector3 vector6 = center + new Vector3(x, -y, -z);
		Vector3 vector7 = center + new Vector3(-x, -y, z);
		Vector3 vector8 = center + new Vector3(-x, -y, -z);
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawLine(vector, vector3);
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector3, vector4);
		Gizmos.DrawLine(vector2, vector4);
		Gizmos.DrawLine(vector, vector5);
		Gizmos.DrawLine(vector2, vector6);
		Gizmos.DrawLine(vector3, vector7);
		Gizmos.DrawLine(vector4, vector8);
		Gizmos.DrawLine(vector5, vector7);
		Gizmos.DrawLine(vector5, vector6);
		Gizmos.DrawLine(vector7, vector8);
		Gizmos.DrawLine(vector8, vector6);
		Gizmos.color = color2;
	}

	// Token: 0x060003CC RID: 972 RVA: 0x0001662A File Offset: 0x0001482A
	public static void DrawBounds(Bounds bounds)
	{
		DebugExtension.DrawBounds(bounds, Color.white);
	}

	// Token: 0x060003CD RID: 973 RVA: 0x00016638 File Offset: 0x00014838
	public static void DrawLocalCube(Transform transform, Vector3 size, Color color, Vector3 center = default(Vector3))
	{
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Vector3 vector = transform.TransformPoint(center + -size * 0.5f);
		Vector3 vector2 = transform.TransformPoint(center + new Vector3(size.x, -size.y, -size.z) * 0.5f);
		Vector3 vector3 = transform.TransformPoint(center + new Vector3(size.x, -size.y, size.z) * 0.5f);
		Vector3 vector4 = transform.TransformPoint(center + new Vector3(-size.x, -size.y, size.z) * 0.5f);
		Vector3 vector5 = transform.TransformPoint(center + new Vector3(-size.x, size.y, -size.z) * 0.5f);
		Vector3 vector6 = transform.TransformPoint(center + new Vector3(size.x, size.y, -size.z) * 0.5f);
		Vector3 vector7 = transform.TransformPoint(center + size * 0.5f);
		Vector3 vector8 = transform.TransformPoint(center + new Vector3(-size.x, size.y, size.z) * 0.5f);
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector2, vector3);
		Gizmos.DrawLine(vector3, vector4);
		Gizmos.DrawLine(vector4, vector);
		Gizmos.DrawLine(vector5, vector6);
		Gizmos.DrawLine(vector6, vector7);
		Gizmos.DrawLine(vector7, vector8);
		Gizmos.DrawLine(vector8, vector5);
		Gizmos.DrawLine(vector, vector5);
		Gizmos.DrawLine(vector2, vector6);
		Gizmos.DrawLine(vector3, vector7);
		Gizmos.DrawLine(vector4, vector8);
		Gizmos.color = color2;
	}

	// Token: 0x060003CE RID: 974 RVA: 0x0001680B File Offset: 0x00014A0B
	public static void DrawLocalCube(Transform transform, Vector3 size, Vector3 center = default(Vector3))
	{
		DebugExtension.DrawLocalCube(transform, size, Color.white, center);
	}

	// Token: 0x060003CF RID: 975 RVA: 0x0001681C File Offset: 0x00014A1C
	public static void DrawLocalCube(Matrix4x4 space, Vector3 size, Color color, Vector3 center = default(Vector3))
	{
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Vector3 vector = space.MultiplyPoint3x4(center + -size * 0.5f);
		Vector3 vector2 = space.MultiplyPoint3x4(center + new Vector3(size.x, -size.y, -size.z) * 0.5f);
		Vector3 vector3 = space.MultiplyPoint3x4(center + new Vector3(size.x, -size.y, size.z) * 0.5f);
		Vector3 vector4 = space.MultiplyPoint3x4(center + new Vector3(-size.x, -size.y, size.z) * 0.5f);
		Vector3 vector5 = space.MultiplyPoint3x4(center + new Vector3(-size.x, size.y, -size.z) * 0.5f);
		Vector3 vector6 = space.MultiplyPoint3x4(center + new Vector3(size.x, size.y, -size.z) * 0.5f);
		Vector3 vector7 = space.MultiplyPoint3x4(center + size * 0.5f);
		Vector3 vector8 = space.MultiplyPoint3x4(center + new Vector3(-size.x, size.y, size.z) * 0.5f);
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector2, vector3);
		Gizmos.DrawLine(vector3, vector4);
		Gizmos.DrawLine(vector4, vector);
		Gizmos.DrawLine(vector5, vector6);
		Gizmos.DrawLine(vector6, vector7);
		Gizmos.DrawLine(vector7, vector8);
		Gizmos.DrawLine(vector8, vector5);
		Gizmos.DrawLine(vector, vector5);
		Gizmos.DrawLine(vector2, vector6);
		Gizmos.DrawLine(vector3, vector7);
		Gizmos.DrawLine(vector4, vector8);
		Gizmos.color = color2;
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x000169F7 File Offset: 0x00014BF7
	public static void DrawLocalCube(Matrix4x4 space, Vector3 size, Vector3 center = default(Vector3))
	{
		DebugExtension.DrawLocalCube(space, size, Color.white, center);
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x00016A08 File Offset: 0x00014C08
	public static void DrawCircle(Vector3 position, Vector3 up, Color color, float radius = 1f)
	{
		up = ((up == Vector3.zero) ? Vector3.up : up).normalized * radius;
		Vector3 vector = Vector3.Slerp(up, -up, 0.5f);
		Vector3 vector2 = Vector3.Cross(up, vector).normalized * radius;
		Matrix4x4 matrix4x = default(Matrix4x4);
		matrix4x[0] = vector2.x;
		matrix4x[1] = vector2.y;
		matrix4x[2] = vector2.z;
		matrix4x[4] = up.x;
		matrix4x[5] = up.y;
		matrix4x[6] = up.z;
		matrix4x[8] = vector.x;
		matrix4x[9] = vector.y;
		matrix4x[10] = vector.z;
		Vector3 vector3 = position + matrix4x.MultiplyPoint3x4(new Vector3(Mathf.Cos(0f), 0f, Mathf.Sin(0f)));
		Vector3 vector4 = Vector3.zero;
		Color color2 = Gizmos.color;
		Gizmos.color = ((color == default(Color)) ? Color.white : color);
		for (int i = 0; i < 91; i++)
		{
			vector4.x = Mathf.Cos((float)(i * 4) * 0.017453292f);
			vector4.z = Mathf.Sin((float)(i * 4) * 0.017453292f);
			vector4.y = 0f;
			vector4 = position + matrix4x.MultiplyPoint3x4(vector4);
			Gizmos.DrawLine(vector3, vector4);
			vector3 = vector4;
		}
		Gizmos.color = color2;
	}

	// Token: 0x060003D2 RID: 978 RVA: 0x00016BB3 File Offset: 0x00014DB3
	public static void DrawCircle(Vector3 position, Color color, float radius = 1f)
	{
		DebugExtension.DrawCircle(position, Vector3.up, color, radius);
	}

	// Token: 0x060003D3 RID: 979 RVA: 0x00016BC2 File Offset: 0x00014DC2
	public static void DrawCircle(Vector3 position, Vector3 up, float radius = 1f)
	{
		DebugExtension.DrawCircle(position, position, Color.white, radius);
	}

	// Token: 0x060003D4 RID: 980 RVA: 0x00016BD1 File Offset: 0x00014DD1
	public static void DrawCircle(Vector3 position, float radius = 1f)
	{
		DebugExtension.DrawCircle(position, Vector3.up, Color.white, radius);
	}

	// Token: 0x060003D5 RID: 981 RVA: 0x00016BE4 File Offset: 0x00014DE4
	public static void DrawCylinder(Vector3 start, Vector3 end, Color color, float radius = 1f)
	{
		Vector3 vector = (end - start).normalized * radius;
		Vector3 vector2 = Vector3.Slerp(vector, -vector, 0.5f);
		Vector3 vector3 = Vector3.Cross(vector, vector2).normalized * radius;
		DebugExtension.DrawCircle(start, vector, color, radius);
		DebugExtension.DrawCircle(end, -vector, color, radius);
		DebugExtension.DrawCircle((start + end) * 0.5f, vector, color, radius);
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawLine(start + vector3, end + vector3);
		Gizmos.DrawLine(start - vector3, end - vector3);
		Gizmos.DrawLine(start + vector2, end + vector2);
		Gizmos.DrawLine(start - vector2, end - vector2);
		Gizmos.DrawLine(start - vector3, start + vector3);
		Gizmos.DrawLine(start - vector2, start + vector2);
		Gizmos.DrawLine(end - vector3, end + vector3);
		Gizmos.DrawLine(end - vector2, end + vector2);
		Gizmos.color = color2;
	}

	// Token: 0x060003D6 RID: 982 RVA: 0x00016D07 File Offset: 0x00014F07
	public static void DrawCylinder(Vector3 start, Vector3 end, float radius = 1f)
	{
		DebugExtension.DrawCylinder(start, end, Color.white, radius);
	}

	// Token: 0x060003D7 RID: 983 RVA: 0x00016D18 File Offset: 0x00014F18
	public static void DrawCone(Vector3 position, Vector3 direction, Color color, float angle = 45f)
	{
		float magnitude = direction.magnitude;
		Vector3 vector = direction;
		Vector3 vector2 = Vector3.Slerp(vector, -vector, 0.5f);
		Vector3 vector3 = Vector3.Cross(vector, vector2).normalized * magnitude;
		direction = direction.normalized;
		Vector3 vector4 = Vector3.Slerp(vector, vector2, angle / 90f);
		Plane plane = new Plane(-direction, position + vector);
		Ray ray = new Ray(position, vector4);
		float num;
		plane.Raycast(ray, out num);
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawRay(position, vector4.normalized * num);
		Gizmos.DrawRay(position, Vector3.Slerp(vector, -vector2, angle / 90f).normalized * num);
		Gizmos.DrawRay(position, Vector3.Slerp(vector, vector3, angle / 90f).normalized * num);
		Gizmos.DrawRay(position, Vector3.Slerp(vector, -vector3, angle / 90f).normalized * num);
		DebugExtension.DrawCircle(position + vector, direction, color, (vector - vector4.normalized * num).magnitude);
		DebugExtension.DrawCircle(position + vector * 0.5f, direction, color, (vector * 0.5f - vector4.normalized * (num * 0.5f)).magnitude);
		Gizmos.color = color2;
	}

	// Token: 0x060003D8 RID: 984 RVA: 0x00016EA5 File Offset: 0x000150A5
	public static void DrawCone(Vector3 position, Vector3 direction, float angle = 45f)
	{
		DebugExtension.DrawCone(position, direction, Color.white, angle);
	}

	// Token: 0x060003D9 RID: 985 RVA: 0x00016EB4 File Offset: 0x000150B4
	public static void DrawCone(Vector3 position, Color color, float angle = 45f)
	{
		DebugExtension.DrawCone(position, Vector3.up, color, angle);
	}

	// Token: 0x060003DA RID: 986 RVA: 0x00016EC3 File Offset: 0x000150C3
	public static void DrawCone(Vector3 position, float angle = 45f)
	{
		DebugExtension.DrawCone(position, Vector3.up, Color.white, angle);
	}

	// Token: 0x060003DB RID: 987 RVA: 0x00016ED6 File Offset: 0x000150D6
	public static void DrawArrow(Vector3 position, Vector3 direction, Color color)
	{
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawRay(position, direction);
		DebugExtension.DrawCone(position + direction, -direction * 0.333f, color, 15f);
		Gizmos.color = color2;
	}

	// Token: 0x060003DC RID: 988 RVA: 0x00016F11 File Offset: 0x00015111
	public static void DrawArrow(Vector3 position, Vector3 direction)
	{
		DebugExtension.DrawArrow(position, direction, Color.white);
	}

	// Token: 0x060003DD RID: 989 RVA: 0x00016F20 File Offset: 0x00015120
	public static void DrawCapsule(Vector3 start, Vector3 end, Color color, float radius = 1f)
	{
		Vector3 vector = (end - start).normalized * radius;
		Vector3 vector2 = Vector3.Slerp(vector, -vector, 0.5f);
		Vector3 vector3 = Vector3.Cross(vector, vector2).normalized * radius;
		Color color2 = Gizmos.color;
		Gizmos.color = color;
		float magnitude = (start - end).magnitude;
		float num = Mathf.Max(0f, magnitude * 0.5f - radius);
		Vector3 vector4 = (end + start) * 0.5f;
		start = vector4 + (start - vector4).normalized * num;
		end = vector4 + (end - vector4).normalized * num;
		DebugExtension.DrawCircle(start, vector, color, radius);
		DebugExtension.DrawCircle(end, -vector, color, radius);
		Gizmos.DrawLine(start + vector3, end + vector3);
		Gizmos.DrawLine(start - vector3, end - vector3);
		Gizmos.DrawLine(start + vector2, end + vector2);
		Gizmos.DrawLine(start - vector2, end - vector2);
		for (int i = 1; i < 26; i++)
		{
			Gizmos.DrawLine(Vector3.Slerp(vector3, -vector, (float)i / 25f) + start, Vector3.Slerp(vector3, -vector, (float)(i - 1) / 25f) + start);
			Gizmos.DrawLine(Vector3.Slerp(-vector3, -vector, (float)i / 25f) + start, Vector3.Slerp(-vector3, -vector, (float)(i - 1) / 25f) + start);
			Gizmos.DrawLine(Vector3.Slerp(vector2, -vector, (float)i / 25f) + start, Vector3.Slerp(vector2, -vector, (float)(i - 1) / 25f) + start);
			Gizmos.DrawLine(Vector3.Slerp(-vector2, -vector, (float)i / 25f) + start, Vector3.Slerp(-vector2, -vector, (float)(i - 1) / 25f) + start);
			Gizmos.DrawLine(Vector3.Slerp(vector3, vector, (float)i / 25f) + end, Vector3.Slerp(vector3, vector, (float)(i - 1) / 25f) + end);
			Gizmos.DrawLine(Vector3.Slerp(-vector3, vector, (float)i / 25f) + end, Vector3.Slerp(-vector3, vector, (float)(i - 1) / 25f) + end);
			Gizmos.DrawLine(Vector3.Slerp(vector2, vector, (float)i / 25f) + end, Vector3.Slerp(vector2, vector, (float)(i - 1) / 25f) + end);
			Gizmos.DrawLine(Vector3.Slerp(-vector2, vector, (float)i / 25f) + end, Vector3.Slerp(-vector2, vector, (float)(i - 1) / 25f) + end);
		}
		Gizmos.color = color2;
	}

	// Token: 0x060003DE RID: 990 RVA: 0x0001725E File Offset: 0x0001545E
	public static void DrawCapsule(Vector3 start, Vector3 end, float radius = 1f)
	{
		DebugExtension.DrawCapsule(start, end, Color.white, radius);
	}

	// Token: 0x060003DF RID: 991 RVA: 0x00017270 File Offset: 0x00015470
	public static string MethodsOfObject(object obj, bool includeInfo = false)
	{
		string text = "";
		MethodInfo[] methods = obj.GetType().GetMethods();
		for (int i = 0; i < methods.Length; i++)
		{
			if (includeInfo)
			{
				string text2 = text;
				MethodInfo methodInfo = methods[i];
				text = text2 + ((methodInfo != null) ? methodInfo.ToString() : null) + "\n";
			}
			else
			{
				text = text + methods[i].Name + "\n";
			}
		}
		return text;
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x000172D4 File Offset: 0x000154D4
	public static string MethodsOfType(Type type, bool includeInfo = false)
	{
		string text = "";
		MethodInfo[] methods = type.GetMethods();
		for (int i = 0; i < methods.Length; i++)
		{
			if (includeInfo)
			{
				string text2 = text;
				MethodInfo methodInfo = methods[i];
				text = text2 + ((methodInfo != null) ? methodInfo.ToString() : null) + "\n";
			}
			else
			{
				text = text + methods[i].Name + "\n";
			}
		}
		return text;
	}
}
