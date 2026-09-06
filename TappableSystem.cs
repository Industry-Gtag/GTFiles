using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000933 RID: 2355
public class TappableSystem : GTSystem<Tappable>
{
	// Token: 0x06003DC0 RID: 15808 RVA: 0x0014ED48 File Offset: 0x0014CF48
	[PunRPC]
	public void SendOnTapRPC(int key, float tapStrength, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "SendOnTapRPC");
		if (key < 0 || key >= this._instances.Count || !float.IsFinite(tapStrength))
		{
			return;
		}
		tapStrength = Mathf.Clamp(tapStrength, 0f, 1f);
		this._instances[key].OnTapLocal(tapStrength, Time.time, new PhotonMessageInfoWrapped(info));
	}
}
