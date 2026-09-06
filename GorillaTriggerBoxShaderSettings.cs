using System;
using GorillaTag.Rendering;
using UnityEngine;

// Token: 0x020005E7 RID: 1511
public class GorillaTriggerBoxShaderSettings : GorillaTriggerBox
{
	// Token: 0x060025C1 RID: 9665 RVA: 0x000C886C File Offset: 0x000C6A6C
	private void Awake()
	{
		if (this.sameSceneSettingsRef != null)
		{
			this.settings = this.sameSceneSettingsRef;
			return;
		}
		this.settingsRef.TryResolve<ZoneShaderSettings>(out this.settings);
	}

	// Token: 0x060025C2 RID: 9666 RVA: 0x000C889C File Offset: 0x000C6A9C
	public override void OnBoxTriggered()
	{
		if (this.settings == null)
		{
			if (this.sameSceneSettingsRef != null)
			{
				this.settings = this.sameSceneSettingsRef;
			}
			else
			{
				this.settingsRef.TryResolve<ZoneShaderSettings>(out this.settings);
			}
		}
		if (this.settings != null)
		{
			this.settings.BecomeActiveInstance(false);
			return;
		}
		ZoneShaderSettings.ActivateDefaultSettings();
	}

	// Token: 0x0400315B RID: 12635
	[SerializeField]
	private XSceneRef settingsRef;

	// Token: 0x0400315C RID: 12636
	[SerializeField]
	private ZoneShaderSettings sameSceneSettingsRef;

	// Token: 0x0400315D RID: 12637
	private ZoneShaderSettings settings;
}
