using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000626 RID: 1574
public class BuilderZoneRenderers : MonoBehaviour
{
	// Token: 0x06002730 RID: 10032 RVA: 0x000CF3AC File Offset: 0x000CD5AC
	private void Start()
	{
		this.allRenderers.Clear();
		this.allRenderers.AddRange(this.renderers);
		foreach (GameObject gameObject in this.rootObjects)
		{
			this.allRenderers.AddRange(gameObject.GetComponentsInChildren<Renderer>(true));
		}
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		this.inBuilderZone = true;
		this.OnZoneChanged();
	}

	// Token: 0x06002731 RID: 10033 RVA: 0x000CF45C File Offset: 0x000CD65C
	private void OnDestroy()
	{
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
	}

	// Token: 0x06002732 RID: 10034 RVA: 0x000CF494 File Offset: 0x000CD694
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks);
		if (flag && !this.inBuilderZone)
		{
			this.inBuilderZone = flag;
			foreach (Renderer renderer in this.allRenderers)
			{
				renderer.enabled = true;
			}
			using (List<Canvas>.Enumerator enumerator2 = this.canvases.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Canvas canvas = enumerator2.Current;
					canvas.enabled = true;
				}
				return;
			}
		}
		if (!flag && this.inBuilderZone)
		{
			this.inBuilderZone = flag;
			foreach (Renderer renderer2 in this.allRenderers)
			{
				renderer2.enabled = false;
			}
			foreach (Canvas canvas2 in this.canvases)
			{
				canvas2.enabled = false;
			}
		}
	}

	// Token: 0x040032C5 RID: 12997
	public List<Renderer> renderers;

	// Token: 0x040032C6 RID: 12998
	public List<Canvas> canvases;

	// Token: 0x040032C7 RID: 12999
	public List<GameObject> rootObjects;

	// Token: 0x040032C8 RID: 13000
	private bool inBuilderZone;

	// Token: 0x040032C9 RID: 13001
	private List<Renderer> allRenderers = new List<Renderer>(200);
}
