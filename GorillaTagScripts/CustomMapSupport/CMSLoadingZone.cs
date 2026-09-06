using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000FB8 RID: 4024
	public class CMSLoadingZone : MonoBehaviour
	{
		// Token: 0x06006413 RID: 25619 RVA: 0x00202D1A File Offset: 0x00200F1A
		private void Start()
		{
			base.gameObject.layer = UnityLayer.GorillaTrigger.ToLayerIndex();
		}

		// Token: 0x06006414 RID: 25620 RVA: 0x00202D30 File Offset: 0x00200F30
		public void SetupLoadingZone(LoadZoneSettings settings, in string[] assetBundleSceneFilePaths)
		{
			this.scenesToLoad = this.GetSceneIndexes(settings.scenesToLoad, in assetBundleSceneFilePaths);
			this.scenesToUnload = this.CleanSceneUnloadArray(settings.scenesToUnload, settings.scenesToLoad, in assetBundleSceneFilePaths);
			this.useDynamicLighting = settings.useDynamicLighting;
			this.dynamicLightingAmbientColor = settings.UberShaderAmbientDynamicLight;
			base.gameObject.layer = UnityLayer.GorillaBoundary.ToLayerIndex();
			Collider[] components = base.gameObject.GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].isTrigger = true;
			}
		}

		// Token: 0x06006415 RID: 25621 RVA: 0x00202DB8 File Offset: 0x00200FB8
		private int[] GetSceneIndexes(List<string> sceneNames, in string[] assetBundleSceneFilePaths)
		{
			int[] array = new int[sceneNames.Count];
			for (int i = 0; i < sceneNames.Count; i++)
			{
				for (int j = 0; j < assetBundleSceneFilePaths.Length; j++)
				{
					if (string.Equals(sceneNames[i], this.GetSceneNameFromFilePath(assetBundleSceneFilePaths[j])))
					{
						array[i] = j;
						break;
					}
				}
			}
			return array;
		}

		// Token: 0x06006416 RID: 25622 RVA: 0x00202E10 File Offset: 0x00201010
		private int[] CleanSceneUnloadArray(List<string> unload, List<string> load, in string[] assetBundleSceneFilePaths)
		{
			for (int i = 0; i < load.Count; i++)
			{
				if (unload.Contains(load[i]))
				{
					unload.Remove(load[i]);
				}
			}
			return this.GetSceneIndexes(unload, in assetBundleSceneFilePaths);
		}

		// Token: 0x06006417 RID: 25623 RVA: 0x00202E54 File Offset: 0x00201054
		public void OnTriggerEnter(Collider other)
		{
			if (other == GTPlayer.Instance.bodyCollider)
			{
				if (this.useDynamicLighting)
				{
					CustomMapLoader.SetZoneDynamicLighting(true);
					GameLightingManager.instance.SetAmbientLightDynamic(this.dynamicLightingAmbientColor);
				}
				else
				{
					CustomMapLoader.SetZoneDynamicLighting(false);
					GameLightingManager.instance.SetAmbientLightDynamic(Color.black);
				}
				CustomMapManager.LoadZoneTriggered(this.scenesToLoad, this.scenesToUnload);
			}
		}

		// Token: 0x06006418 RID: 25624 RVA: 0x00202EBD File Offset: 0x002010BD
		private string GetSceneNameFromFilePath(string filePath)
		{
			string[] array = filePath.Split("/", StringSplitOptions.None);
			return array[array.Length - 1].Split(".", StringSplitOptions.None)[0];
		}

		// Token: 0x040072E8 RID: 29416
		private int[] scenesToLoad;

		// Token: 0x040072E9 RID: 29417
		private int[] scenesToUnload;

		// Token: 0x040072EA RID: 29418
		private bool useDynamicLighting;

		// Token: 0x040072EB RID: 29419
		private Color dynamicLightingAmbientColor;
	}
}
