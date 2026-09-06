using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag.Rendering;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x02000A54 RID: 2644
public class CMSZoneShaderSettingsTrigger : MonoBehaviour
{
	// Token: 0x060043D9 RID: 17369 RVA: 0x00169101 File Offset: 0x00167301
	public void OnEnable()
	{
		if (this.activateOnEnable)
		{
			this.ActivateShaderSettings();
		}
	}

	// Token: 0x060043DA RID: 17370 RVA: 0x00169114 File Offset: 0x00167314
	public void CopySettings(ZoneShaderTriggerSettings triggerSettings)
	{
		base.gameObject.layer = UnityLayer.GorillaBoundary.ToLayerIndex();
		this.activateOnEnable = triggerSettings.activateOnEnable;
		if (triggerSettings.activationType == ZoneShaderTriggerSettings.ActivationType.ActivateCustomMapDefaults)
		{
			this.activateCustomMapDefaults = true;
			return;
		}
		GameObject zoneShaderSettingsObject = triggerSettings.zoneShaderSettingsObject;
		if (zoneShaderSettingsObject.IsNotNull())
		{
			this.shaderSettingsObject = zoneShaderSettingsObject;
		}
	}

	// Token: 0x060043DB RID: 17371 RVA: 0x00169166 File Offset: 0x00167366
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.bodyCollider)
		{
			this.ActivateShaderSettings();
		}
	}

	// Token: 0x060043DC RID: 17372 RVA: 0x00169180 File Offset: 0x00167380
	private void ActivateShaderSettings()
	{
		if (this.activateCustomMapDefaults)
		{
			CustomMapManager.ActivateDefaultZoneShaderSettings();
			return;
		}
		if (this.shaderSettingsObject.IsNotNull())
		{
			ZoneShaderSettings component = this.shaderSettingsObject.GetComponent<ZoneShaderSettings>();
			if (component.IsNotNull())
			{
				component.BecomeActiveInstance(false);
			}
		}
	}

	// Token: 0x040055D1 RID: 21969
	public GameObject shaderSettingsObject;

	// Token: 0x040055D2 RID: 21970
	public bool activateCustomMapDefaults;

	// Token: 0x040055D3 RID: 21971
	public bool activateOnEnable;
}
