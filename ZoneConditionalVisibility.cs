using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003CB RID: 971
public class ZoneConditionalVisibility : MonoBehaviour
{
	// Token: 0x06001750 RID: 5968 RVA: 0x00086E67 File Offset: 0x00085067
	private void Awake()
	{
		if (this.renderersOnly)
		{
			this.renderers = new List<Renderer>(32);
			base.GetComponentsInChildren<Renderer>(false, this.renderers);
		}
	}

	// Token: 0x06001751 RID: 5969 RVA: 0x00086E8B File Offset: 0x0008508B
	private void Start()
	{
		this.OnZoneChanged();
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06001752 RID: 5970 RVA: 0x00086EB9 File Offset: 0x000850B9
	private void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06001753 RID: 5971 RVA: 0x00086EE4 File Offset: 0x000850E4
	private void OnZoneChanged()
	{
		bool flag = ((this.zones == null || this.zones.Length == 0) ? ZoneManagement.IsInZone(this.zone) : this.InAnyZone());
		if (this.invisibleWhileLoaded)
		{
			if (this.renderersOnly)
			{
				for (int i = 0; i < this.renderers.Count; i++)
				{
					if (this.renderers[i] != null)
					{
						this.renderers[i].enabled = !flag;
					}
				}
				return;
			}
			base.gameObject.SetActive(!flag);
			return;
		}
		else
		{
			if (this.renderersOnly)
			{
				for (int j = 0; j < this.renderers.Count; j++)
				{
					if (this.renderers[j] != null)
					{
						this.renderers[j].enabled = flag;
					}
				}
				return;
			}
			base.gameObject.SetActive(flag);
			return;
		}
	}

	// Token: 0x06001754 RID: 5972 RVA: 0x00086FC8 File Offset: 0x000851C8
	private bool InAnyZone()
	{
		for (int i = 0; i < this.zones.Length; i++)
		{
			if (ZoneManagement.IsInZone(this.zones[i]))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04002288 RID: 8840
	[SerializeField]
	private GTZone zone;

	// Token: 0x04002289 RID: 8841
	[SerializeField]
	private GTZone[] zones;

	// Token: 0x0400228A RID: 8842
	[SerializeField]
	private bool invisibleWhileLoaded;

	// Token: 0x0400228B RID: 8843
	[SerializeField]
	private bool renderersOnly;

	// Token: 0x0400228C RID: 8844
	private List<Renderer> renderers;
}
