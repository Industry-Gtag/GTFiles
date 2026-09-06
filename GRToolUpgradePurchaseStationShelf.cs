using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x0200082A RID: 2090
public class GRToolUpgradePurchaseStationShelf : MonoBehaviour
{
	// Token: 0x060035B9 RID: 13753 RVA: 0x00128AF4 File Offset: 0x00126CF4
	public void Awake()
	{
		for (int i = 0; i < this.gRPurchaseSlots.Count; i++)
		{
			Renderer[] componentsInChildren = this.gRPurchaseSlots[i].SlotPivot.gameObject.GetComponentsInChildren<Renderer>();
			this.slotRenderers.Add(componentsInChildren);
			Material[][] array = new Material[componentsInChildren.Length][];
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				array[j] = componentsInChildren[j].sharedMaterials;
			}
			this.slotOriginalMaterials.Add(array);
		}
	}

	// Token: 0x060035BA RID: 13754 RVA: 0x00128B70 File Offset: 0x00126D70
	public void SetMaterialOverride(int slotID, Material overrideMaterial)
	{
		if (slotID < 0 || slotID >= this.gRPurchaseSlots.Count)
		{
			return;
		}
		if (this.gRPurchaseSlots[slotID].overrideMaterial == overrideMaterial)
		{
			return;
		}
		if (slotID >= this.slotRenderers.Count)
		{
			return;
		}
		this.gRPurchaseSlots[slotID].overrideMaterial = overrideMaterial;
		for (int i = 0; i < this.slotRenderers[slotID].Length; i++)
		{
			Renderer renderer = this.slotRenderers[slotID][i];
			if (overrideMaterial == null)
			{
				renderer.materials = this.slotOriginalMaterials[slotID][i];
			}
			else
			{
				Material[] array = new Material[renderer.sharedMaterials.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = overrideMaterial;
				}
				renderer.materials = array;
			}
		}
	}

	// Token: 0x060035BB RID: 13755 RVA: 0x00128C3C File Offset: 0x00126E3C
	public void SetBacklightStateAndMaterial(int slotID, bool isEnabled, Material materialOverride)
	{
		if (slotID < 0 || slotID >= this.gRPurchaseSlots.Count)
		{
			return;
		}
		if (this.gRPurchaseSlots[slotID].BacklightRenderer != null)
		{
			if (!isEnabled)
			{
				this.gRPurchaseSlots[slotID].BacklightRenderer.enabled = false;
				return;
			}
			this.gRPurchaseSlots[slotID].BacklightRenderer.enabled = true;
			this.gRPurchaseSlots[slotID].BacklightRenderer.sharedMaterial = materialOverride;
		}
	}

	// Token: 0x0400463C RID: 17980
	public string ShelfName;

	// Token: 0x0400463D RID: 17981
	private List<Material[][]> slotOriginalMaterials = new List<Material[][]>();

	// Token: 0x0400463E RID: 17982
	private List<Renderer[]> slotRenderers = new List<Renderer[]>();

	// Token: 0x0400463F RID: 17983
	public List<GRToolUpgradePurchaseStationShelf.GRPurchaseSlot> gRPurchaseSlots;

	// Token: 0x0200082B RID: 2091
	[Serializable]
	public class GRPurchaseSlot
	{
		// Token: 0x04004640 RID: 17984
		public TMP_Text Name;

		// Token: 0x04004641 RID: 17985
		public TMP_Text Price;

		// Token: 0x04004642 RID: 17986
		public Transform SlotPivot;

		// Token: 0x04004643 RID: 17987
		public GRToolProgressionManager.ToolParts PurchaseID;

		// Token: 0x04004644 RID: 17988
		public GameEntity ToolEntityPrefab;

		// Token: 0x04004645 RID: 17989
		public float RopeYaw;

		// Token: 0x04004646 RID: 17990
		public float RopePitch;

		// Token: 0x04004647 RID: 17991
		public MeshRenderer BacklightRenderer;

		// Token: 0x04004648 RID: 17992
		[NonSerialized]
		public Material overrideMaterial;

		// Token: 0x04004649 RID: 17993
		[NonSerialized]
		public bool canAfford;

		// Token: 0x0400464A RID: 17994
		[NonSerialized]
		public string purchaseText = "";
	}
}
