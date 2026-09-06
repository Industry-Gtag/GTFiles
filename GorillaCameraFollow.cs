using System;
using GorillaLocomotion;
using Unity.Cinemachine;
using UnityEngine;

// Token: 0x020005D8 RID: 1496
public class GorillaCameraFollow : MonoBehaviour
{
	// Token: 0x06002599 RID: 9625 RVA: 0x000C83B8 File Offset: 0x000C65B8
	private void Start()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			this.cameraParent.SetActive(false);
		}
		if (this.cinemachineCamera != null)
		{
			this.cinemachineFollow = this.cinemachineCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
			this.baseCameraRadius = this.cinemachineFollow.CameraRadius;
			this.baseFollowDistance = this.cinemachineFollow.CameraDistance;
			this.baseVerticalArmLength = this.cinemachineFollow.VerticalArmLength;
			this.baseShoulderOffset = this.cinemachineFollow.ShoulderOffset;
		}
	}

	// Token: 0x0600259A RID: 9626 RVA: 0x000C8440 File Offset: 0x000C6640
	private void LateUpdate()
	{
		if (this.cinemachineFollow != null)
		{
			float scale = GTPlayer.Instance.scale;
			this.cinemachineFollow.CameraRadius = this.baseCameraRadius * scale;
			this.cinemachineFollow.CameraDistance = this.baseFollowDistance * scale;
			this.cinemachineFollow.VerticalArmLength = this.baseVerticalArmLength * scale;
			this.cinemachineFollow.ShoulderOffset = this.baseShoulderOffset * scale;
		}
	}

	// Token: 0x04003104 RID: 12548
	public Transform playerHead;

	// Token: 0x04003105 RID: 12549
	public GameObject cameraParent;

	// Token: 0x04003106 RID: 12550
	public Vector3 headOffset;

	// Token: 0x04003107 RID: 12551
	public Vector3 eulerRotationOffset;

	// Token: 0x04003108 RID: 12552
	public CinemachineVirtualCamera cinemachineCamera;

	// Token: 0x04003109 RID: 12553
	private Cinemachine3rdPersonFollow cinemachineFollow;

	// Token: 0x0400310A RID: 12554
	private float baseCameraRadius = 0.2f;

	// Token: 0x0400310B RID: 12555
	private float baseFollowDistance = 2f;

	// Token: 0x0400310C RID: 12556
	private float baseVerticalArmLength = 0.4f;

	// Token: 0x0400310D RID: 12557
	private Vector3 baseShoulderOffset = new Vector3(0.5f, -0.4f, 0f);
}
