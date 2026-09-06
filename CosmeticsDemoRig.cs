using System;
using System.Collections.Generic;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000305 RID: 773
[ExecuteInEditMode]
public class CosmeticsDemoRig : MonoBehaviour
{
	// Token: 0x04001826 RID: 6182
	[SerializeField]
	private VRRig _vrRig;

	// Token: 0x04001827 RID: 6183
	private Transform[] _vrRigBoneXforms;

	// Token: 0x04001828 RID: 6184
	private Transform[] _vrRigSlotXforms;

	// Token: 0x04001829 RID: 6185
	[SerializeField]
	private Transform chestOffset;

	// Token: 0x0400182A RID: 6186
	[SerializeField]
	private Transform leftArmOffset;

	// Token: 0x0400182B RID: 6187
	[SerializeField]
	private Transform rightArmOffset;

	// Token: 0x0400182C RID: 6188
	private Vector3 badgeDefaultPos;

	// Token: 0x0400182D RID: 6189
	private Quaternion badgeDefaultRot;

	// Token: 0x0400182E RID: 6190
	private bool isInitialized;

	// Token: 0x0400182F RID: 6191
	private CosmeticsDemoRig.EdSpawnedCosmetic emptyCosmetic;

	// Token: 0x04001830 RID: 6192
	private Material defaultFaceMaterial;

	// Token: 0x04001831 RID: 6193
	[SerializeField]
	[HideInInspector]
	private Material myDefaultSkinMaterialInstance;

	// Token: 0x04001832 RID: 6194
	[SerializeField]
	[HideInInspector]
	private Material materialToChangeTo0;

	// Token: 0x04001833 RID: 6195
	[SerializeField]
	[HideInInspector]
	private Color monkeColor = new Color(0f, 0f, 0f);

	// Token: 0x04001834 RID: 6196
	[SerializeField]
	[HideInInspector]
	private GorillaSkin currentSkin;

	// Token: 0x04001835 RID: 6197
	[SerializeField]
	[HideInInspector]
	private GorillaSkin defaultSkin;

	// Token: 0x04001836 RID: 6198
	[SerializeField]
	[HideInInspector]
	private Material[] faceMaterialSwaps = new Material[10];

	// Token: 0x04001837 RID: 6199
	[HideInInspector]
	public int materialIndex;

	// Token: 0x04001838 RID: 6200
	private int selectedMouth;

	// Token: 0x04001839 RID: 6201
	[HideInInspector]
	public UnityEvent<Color> OnColorChange;

	// Token: 0x0400183A RID: 6202
	[SerializeField]
	private CosmeticsDemoRig.EdSpawnedCosmetic[] spawnedCosmetics = new CosmeticsDemoRig.EdSpawnedCosmetic[16];

	// Token: 0x02000306 RID: 774
	[Serializable]
	private struct EdSpawnedCosmetic
	{
		// Token: 0x0400183B RID: 6203
		public string itemName;

		// Token: 0x0400183C RID: 6204
		public CosmeticSO so;

		// Token: 0x0400183D RID: 6205
		public List<GameObject> objects;

		// Token: 0x0400183E RID: 6206
		public List<GameObject> holdableObjects;

		// Token: 0x0400183F RID: 6207
		public bool isEmpty;
	}
}
