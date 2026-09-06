using System;
using UnityEngine;

// Token: 0x020003CA RID: 970
public class ZoneConditionalGameObjectEnabling : MonoBehaviour
{
	// Token: 0x0600174C RID: 5964 RVA: 0x00086D92 File Offset: 0x00084F92
	private void Start()
	{
		this.OnZoneChanged();
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x0600174D RID: 5965 RVA: 0x00086DC0 File Offset: 0x00084FC0
	private void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x0600174E RID: 5966 RVA: 0x00086DE8 File Offset: 0x00084FE8
	private void OnZoneChanged()
	{
		if (this.invisibleWhileLoaded)
		{
			if (this.gameObjects != null)
			{
				for (int i = 0; i < this.gameObjects.Length; i++)
				{
					this.gameObjects[i].SetActive(!ZoneManagement.IsInZone(this.zone));
				}
				return;
			}
		}
		else if (this.gameObjects != null)
		{
			for (int j = 0; j < this.gameObjects.Length; j++)
			{
				this.gameObjects[j].SetActive(ZoneManagement.IsInZone(this.zone));
			}
		}
	}

	// Token: 0x04002285 RID: 8837
	[SerializeField]
	private GTZone zone;

	// Token: 0x04002286 RID: 8838
	[SerializeField]
	private bool invisibleWhileLoaded;

	// Token: 0x04002287 RID: 8839
	[SerializeField]
	private GameObject[] gameObjects;
}
