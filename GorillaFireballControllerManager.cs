using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

// Token: 0x02000A45 RID: 2629
public class GorillaFireballControllerManager : MonoBehaviour
{
	// Token: 0x06004385 RID: 17285 RVA: 0x0016722C File Offset: 0x0016542C
	private void Update()
	{
		if (!this.hasInitialized)
		{
			this.hasInitialized = true;
			List<InputDevice> list = new List<InputDevice>();
			List<InputDevice> list2 = new List<InputDevice>();
			InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, list);
			InputDevices.GetDevicesAtXRNode(XRNode.RightHand, list2);
			if (list.Count == 1)
			{
				this.leftHand = list[0];
			}
			if (list2.Count == 1)
			{
				this.rightHand = list2[0];
			}
		}
		float num = SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(SteamVR_Input_Sources.LeftHand);
		if (this.leftHandLastState <= this.throwingThreshold && num > this.throwingThreshold)
		{
			this.CreateFireball(true);
		}
		else if (this.leftHandLastState >= this.throwingThreshold && num < this.throwingThreshold)
		{
			this.TryThrowFireball(true);
		}
		this.leftHandLastState = num;
		num = SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(SteamVR_Input_Sources.RightHand);
		if (this.rightHandLastState <= this.throwingThreshold && num > this.throwingThreshold)
		{
			this.CreateFireball(false);
		}
		else if (this.rightHandLastState >= this.throwingThreshold && num < this.throwingThreshold)
		{
			this.TryThrowFireball(false);
		}
		this.rightHandLastState = num;
	}

	// Token: 0x06004386 RID: 17286 RVA: 0x00167330 File Offset: 0x00165530
	public void TryThrowFireball(bool isLeftHand)
	{
		if (isLeftHand && GorillaPlaySpace.Instance.myVRRig.leftHandTransform.GetComponentInChildren<GorillaFireball>() != null)
		{
			GorillaPlaySpace.Instance.myVRRig.leftHandTransform.GetComponentInChildren<GorillaFireball>().ThrowThisThingo();
			return;
		}
		if (!isLeftHand && GorillaPlaySpace.Instance.myVRRig.rightHandTransform.GetComponentInChildren<GorillaFireball>() != null)
		{
			GorillaPlaySpace.Instance.myVRRig.rightHandTransform.GetComponentInChildren<GorillaFireball>().ThrowThisThingo();
		}
	}

	// Token: 0x06004387 RID: 17287 RVA: 0x001673B0 File Offset: 0x001655B0
	public void CreateFireball(bool isLeftHand)
	{
		object[] array = new object[1];
		Vector3 vector;
		if (isLeftHand)
		{
			array[0] = true;
			vector = GorillaPlaySpace.Instance.myVRRig.leftHandTransform.position;
		}
		else
		{
			array[0] = false;
			vector = GorillaPlaySpace.Instance.myVRRig.rightHandTransform.position;
		}
		PhotonNetwork.Instantiate("GorillaPrefabs/GorillaFireball", vector, Quaternion.identity, 0, array);
	}

	// Token: 0x0400555B RID: 21851
	public InputDevice leftHand;

	// Token: 0x0400555C RID: 21852
	public InputDevice rightHand;

	// Token: 0x0400555D RID: 21853
	public bool hasInitialized;

	// Token: 0x0400555E RID: 21854
	public float leftHandLastState;

	// Token: 0x0400555F RID: 21855
	public float rightHandLastState;

	// Token: 0x04005560 RID: 21856
	public float throwingThreshold = 0.9f;
}
