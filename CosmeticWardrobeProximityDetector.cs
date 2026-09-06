using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000584 RID: 1412
[RequireComponent(typeof(SphereCollider))]
public class CosmeticWardrobeProximityDetector : MonoBehaviour
{
	// Token: 0x060023D8 RID: 9176 RVA: 0x000C13FE File Offset: 0x000BF5FE
	private void OnEnable()
	{
		if (this.wardrobeNearbyCollider != null)
		{
			CosmeticWardrobeProximityDetector.wardrobeNearbyDetection.Add(this.wardrobeNearbyCollider);
		}
	}

	// Token: 0x060023D9 RID: 9177 RVA: 0x000C141E File Offset: 0x000BF61E
	private void OnDisable()
	{
		if (this.wardrobeNearbyCollider != null)
		{
			CosmeticWardrobeProximityDetector.wardrobeNearbyDetection.Remove(this.wardrobeNearbyCollider);
		}
	}

	// Token: 0x060023DA RID: 9178 RVA: 0x000C1440 File Offset: 0x000BF640
	public static bool IsUserNearWardrobe(int actorNr)
	{
		LayerMask.GetMask(new string[] { "Gorilla Tag Collider" });
		LayerMask.GetMask(new string[] { "Gorilla Body Collider" });
		VRRigCache.Instance.GetActiveRigs(CosmeticWardrobeProximityDetector.rigs);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetPlayer.Get(actorNr).GetPlayerRef(), out rigContainer))
		{
			return false;
		}
		foreach (SphereCollider sphereCollider in CosmeticWardrobeProximityDetector.wardrobeNearbyDetection)
		{
			if ((rigContainer.HeadCollider.transform.position - sphereCollider.transform.position).magnitude <= sphereCollider.radius)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04002F1F RID: 12063
	[SerializeField]
	private SphereCollider wardrobeNearbyCollider;

	// Token: 0x04002F20 RID: 12064
	private static List<VRRig> rigs = new List<VRRig>();

	// Token: 0x04002F21 RID: 12065
	private static List<SphereCollider> wardrobeNearbyDetection = new List<SphereCollider>();

	// Token: 0x04002F22 RID: 12066
	private static readonly Collider[] overlapColliders = new Collider[20];
}
