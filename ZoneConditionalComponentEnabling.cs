using System;
using UnityEngine;

// Token: 0x020003C9 RID: 969
public class ZoneConditionalComponentEnabling : MonoBehaviour
{
	// Token: 0x06001748 RID: 5960 RVA: 0x00086C5F File Offset: 0x00084E5F
	private void Start()
	{
		this.OnZoneChanged();
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06001749 RID: 5961 RVA: 0x00086C8D File Offset: 0x00084E8D
	private void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x0600174A RID: 5962 RVA: 0x00086CB8 File Offset: 0x00084EB8
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.IsInZone(this.zone);
		bool flag2 = (this.invisibleWhileLoaded ? (!flag) : flag);
		if (this.components != null)
		{
			for (int i = 0; i < this.components.Length; i++)
			{
				if (this.components[i] != null)
				{
					this.components[i].enabled = flag2;
				}
			}
		}
		if (this.m_renderers != null)
		{
			for (int j = 0; j < this.m_renderers.Length; j++)
			{
				if (this.m_renderers[j] != null)
				{
					this.m_renderers[j].enabled = flag2;
				}
			}
		}
		if (this.m_colliders != null)
		{
			for (int k = 0; k < this.m_colliders.Length; k++)
			{
				if (this.m_colliders[k] != null)
				{
					this.m_colliders[k].enabled = flag2;
				}
			}
		}
	}

	// Token: 0x04002280 RID: 8832
	[SerializeField]
	private GTZone zone;

	// Token: 0x04002281 RID: 8833
	[SerializeField]
	private bool invisibleWhileLoaded;

	// Token: 0x04002282 RID: 8834
	[SerializeField]
	private Behaviour[] components;

	// Token: 0x04002283 RID: 8835
	[SerializeField]
	private Renderer[] m_renderers;

	// Token: 0x04002284 RID: 8836
	[SerializeField]
	private Collider[] m_colliders;
}
