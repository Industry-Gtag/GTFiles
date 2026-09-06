using System;
using System.Collections.Generic;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020010A7 RID: 4263
	public class CosmeticItemRegistry
	{
		// Token: 0x17000A20 RID: 2592
		// (get) Token: 0x06006A40 RID: 27200 RVA: 0x00221EBC File Offset: 0x002200BC
		public VRRig Rig
		{
			get
			{
				return this.rig;
			}
		}

		// Token: 0x06006A41 RID: 27201 RVA: 0x00221EC4 File Offset: 0x002200C4
		public void RefreshRig()
		{
			this.rig.RefreshCosmetics();
		}

		// Token: 0x06006A42 RID: 27202 RVA: 0x00221ED1 File Offset: 0x002200D1
		public CosmeticItemRegistry(VRRig _rig)
		{
			this.rig = _rig;
		}

		// Token: 0x06006A43 RID: 27203 RVA: 0x00221EF8 File Offset: 0x002200F8
		public void InitializeCosmetic(GameObject cosmeticGObj, bool isOverride)
		{
			if (this.initializedCosmetics.Contains(cosmeticGObj))
			{
				return;
			}
			this.initializedCosmetics.Add(cosmeticGObj);
			if (!isOverride)
			{
				foreach (GameObject gameObject in this.rig.overrideCosmetics)
				{
					if (cosmeticGObj.name == gameObject.name)
					{
						cosmeticGObj.name = "OVERRIDDEN";
						return;
					}
				}
			}
			string text = cosmeticGObj.name.Replace("LEFT.", "").Replace("RIGHT.", "").TrimEnd();
			CosmeticItemInstance cosmeticItemInstance;
			if (this._nameToCosmeticMap.ContainsKey(text))
			{
				cosmeticItemInstance = this._nameToCosmeticMap[text];
			}
			else
			{
				cosmeticItemInstance = new CosmeticItemInstance();
				CosmeticSO cosmeticSOFromDisplayName = CosmeticsController.instance.GetCosmeticSOFromDisplayName(text);
				cosmeticItemInstance.clippingOffsets = ((cosmeticSOFromDisplayName != null) ? cosmeticSOFromDisplayName.info.anchorAntiIntersectOffsets : CosmeticsController.instance.defaultClipOffsets);
				cosmeticItemInstance.isHoldableItem = cosmeticSOFromDisplayName != null && cosmeticSOFromDisplayName.info.hasHoldableParts;
				this._nameToCosmeticMap.Add(text, cosmeticItemInstance);
			}
			HoldableObject component = cosmeticGObj.GetComponent<HoldableObject>();
			bool flag = cosmeticGObj.name.Contains("LEFT.");
			bool flag2 = cosmeticGObj.name.Contains("RIGHT.");
			if (cosmeticItemInstance.isHoldableItem && component != null)
			{
				if (component is SnowballThrowable || component is TransferrableObject)
				{
					cosmeticItemInstance.holdableObjects.Add(cosmeticGObj);
				}
				else if (flag)
				{
					cosmeticItemInstance.leftObjects.Add(cosmeticGObj);
				}
				else if (flag2)
				{
					cosmeticItemInstance.rightObjects.Add(cosmeticGObj);
				}
				else
				{
					cosmeticItemInstance.objects.Add(cosmeticGObj);
				}
			}
			else if (flag)
			{
				cosmeticItemInstance.leftObjects.Add(cosmeticGObj);
			}
			else if (flag2)
			{
				cosmeticItemInstance.rightObjects.Add(cosmeticGObj);
			}
			else
			{
				cosmeticItemInstance.objects.Add(cosmeticGObj);
			}
			cosmeticItemInstance.dbgname = text;
			Renderer[] componentsInChildren = cosmeticGObj.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].enabled)
				{
					cosmeticItemInstance.allRenderers.Add(componentsInChildren[i]);
				}
			}
			ParticleSystem[] componentsInChildren2 = cosmeticGObj.GetComponentsInChildren<ParticleSystem>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				if (componentsInChildren2[j].emission.enabled)
				{
					cosmeticItemInstance.allParticles.Add(componentsInChildren2[j]);
				}
			}
		}

		// Token: 0x06006A44 RID: 27204 RVA: 0x0022217C File Offset: 0x0022037C
		public CosmeticItemInstance Cosmetic(string itemName)
		{
			if (string.IsNullOrEmpty(itemName) || itemName == "NOTHING")
			{
				return null;
			}
			CosmeticItemInstance cosmeticItemInstance;
			if (!this._nameToCosmeticMap.TryGetValue(itemName, out cosmeticItemInstance))
			{
				CosmeticsV2Spawner_Dirty.ProcessLoadOpInfos(this.rig, itemName, this);
				return null;
			}
			return cosmeticItemInstance;
		}

		// Token: 0x040079D3 RID: 31187
		private Dictionary<string, CosmeticItemInstance> _nameToCosmeticMap = new Dictionary<string, CosmeticItemInstance>();

		// Token: 0x040079D4 RID: 31188
		private HashSet<GameObject> initializedCosmetics = new HashSet<GameObject>();

		// Token: 0x040079D5 RID: 31189
		private GameObject _nullItem;

		// Token: 0x040079D6 RID: 31190
		private VRRig rig;
	}
}
