using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200003C RID: 60
public class ColliderOffsetOverride : MonoBehaviour
{
	// Token: 0x060000F7 RID: 247 RVA: 0x000061C8 File Offset: 0x000043C8
	private void Awake()
	{
		if (this.autoSearch)
		{
			this.FindColliders();
		}
		foreach (Collider collider in this.colliders)
		{
			if (collider != null)
			{
				collider.contactOffset = 0.01f * this.targetScale;
			}
		}
	}

	// Token: 0x060000F8 RID: 248 RVA: 0x00006240 File Offset: 0x00004440
	public void FindColliders()
	{
		foreach (Collider collider in base.gameObject.GetComponents<Collider>().ToList<Collider>())
		{
			if (!this.colliders.Contains(collider))
			{
				this.colliders.Add(collider);
			}
		}
	}

	// Token: 0x060000F9 RID: 249 RVA: 0x000062B0 File Offset: 0x000044B0
	public void FindCollidersRecursively()
	{
		foreach (Collider collider in base.gameObject.GetComponentsInChildren<Collider>().ToList<Collider>())
		{
			if (!this.colliders.Contains(collider))
			{
				this.colliders.Add(collider);
			}
		}
	}

	// Token: 0x060000FA RID: 250 RVA: 0x00006320 File Offset: 0x00004520
	private void AutoDisabled()
	{
		this.autoSearch = true;
	}

	// Token: 0x060000FB RID: 251 RVA: 0x00006329 File Offset: 0x00004529
	private void AutoEnabled()
	{
		this.autoSearch = false;
	}

	// Token: 0x04000108 RID: 264
	public List<Collider> colliders;

	// Token: 0x04000109 RID: 265
	[HideInInspector]
	public bool autoSearch;

	// Token: 0x0400010A RID: 266
	public float targetScale = 1f;
}
