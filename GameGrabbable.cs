using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006DF RID: 1759
public class GameGrabbable : MonoBehaviour
{
	// Token: 0x06002C64 RID: 11364 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Awake()
	{
	}

	// Token: 0x06002C65 RID: 11365 RVA: 0x000EFE74 File Offset: 0x000EE074
	public bool GetBestGrabPoint(Vector3 handPos, Quaternion handRot, int handIndex, out GameGrab grab)
	{
		float num = 0.15f;
		bool flag = false;
		grab = default(GameGrab);
		grab.position = base.transform.position;
		grab.rotation = base.transform.rotation;
		bool flag2 = GamePlayer.IsLeftHand(handIndex);
		if (this.snapGrabPoints != null)
		{
			for (int i = 0; i < this.snapGrabPoints.Count; i++)
			{
				GameGrabbable.SnapGrabPoints snapGrabPoints = this.snapGrabPoints[i];
				if (snapGrabPoints.isLeftHand == flag2 && Vector3.Dot(snapGrabPoints.handTransform.rotation * GameGrabbable.GRAB_UP, handRot * GameGrabbable.GRAB_UP) >= 0f && Vector3.Dot(snapGrabPoints.handTransform.rotation * GameGrabbable.GRAB_PALM, handRot * GameGrabbable.GRAB_PALM) >= 0f && (double)(handPos - snapGrabPoints.handTransform.position).sqrMagnitude <= 0.0225)
				{
					grab.position = handPos + handRot * Quaternion.Inverse(snapGrabPoints.handTransform.localRotation) * -snapGrabPoints.handTransform.localPosition;
					grab.rotation = handRot * Quaternion.Inverse(snapGrabPoints.handTransform.localRotation);
					flag = true;
				}
			}
		}
		if (!flag)
		{
			return false;
		}
		Vector3 vector = grab.position - handPos;
		if (vector.sqrMagnitude > num * num)
		{
			grab.position = handPos + vector.normalized * num;
		}
		return true;
	}

	// Token: 0x040038DC RID: 14556
	public GameEntity gameEntity;

	// Token: 0x040038DD RID: 14557
	public List<GameGrabbable.SnapGrabPoints> snapGrabPoints;

	// Token: 0x040038DE RID: 14558
	private static readonly Vector3 GRAB_UP = new Vector3(0f, 0f, 1f);

	// Token: 0x040038DF RID: 14559
	private static readonly Vector3 GRAB_PALM = new Vector3(1f, 0f, 0f);

	// Token: 0x020006E0 RID: 1760
	[Serializable]
	public class SnapGrabPoints
	{
		// Token: 0x040038E0 RID: 14560
		public bool isLeftHand;

		// Token: 0x040038E1 RID: 14561
		public Transform handTransform;
	}
}
