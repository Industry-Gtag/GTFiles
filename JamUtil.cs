using System;
using UnityEngine;

// Token: 0x020001E7 RID: 487
public static class JamUtil
{
	// Token: 0x17000134 RID: 308
	// (get) Token: 0x06000CC6 RID: 3270 RVA: 0x00046937 File Offset: 0x00044B37
	public static bool IsPlaying
	{
		get
		{
			return Application.isPlaying;
		}
	}

	// Token: 0x06000CC7 RID: 3271 RVA: 0x0004693E File Offset: 0x00044B3E
	public static void Destroy(Object obj)
	{
		Object.Destroy(obj);
	}

	// Token: 0x06000CC8 RID: 3272 RVA: 0x00046948 File Offset: 0x00044B48
	public static RaycastHit ToRaycastHit(this Collision collision)
	{
		RaycastHit raycastHit;
		if (!collision.ConvertToRaycast(out raycastHit))
		{
			GTDev.LogError<string>(string.Format("No hit! ({0})", collision), null);
		}
		return raycastHit;
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x00046974 File Offset: 0x00044B74
	public static bool ConvertToRaycast(this Collision collision, out RaycastHit hit)
	{
		ContactPoint contact = collision.GetContact(0);
		Vector3 point = contact.point;
		Vector3 normal = contact.normal;
		LayerMask layerMask = 1 << collision.gameObject.layer;
		return Physics.Raycast(new Ray(point + normal * 0.1f, -normal), out hit, 0.2f, layerMask, QueryTriggerInteraction.Ignore);
	}
}
