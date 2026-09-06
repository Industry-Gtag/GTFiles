using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000598 RID: 1432
public class MoodRing : MonoBehaviour, ISpawnable
{
	// Token: 0x170003D0 RID: 976
	// (get) Token: 0x0600243F RID: 9279 RVA: 0x000C2A07 File Offset: 0x000C0C07
	// (set) Token: 0x06002440 RID: 9280 RVA: 0x000C2A0F File Offset: 0x000C0C0F
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170003D1 RID: 977
	// (get) Token: 0x06002441 RID: 9281 RVA: 0x000C2A18 File Offset: 0x000C0C18
	// (set) Token: 0x06002442 RID: 9282 RVA: 0x000C2A20 File Offset: 0x000C0C20
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06002443 RID: 9283 RVA: 0x00002C2D File Offset: 0x00000E2D
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06002444 RID: 9284 RVA: 0x000C2A29 File Offset: 0x000C0C29
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x06002445 RID: 9285 RVA: 0x000C2A34 File Offset: 0x000C0C34
	private void Update()
	{
		if ((this.attachedToLeftHand ? this.myRig.leftIndex.calcT : this.myRig.rightIndex.calcT) > 0.5f)
		{
			if (!this.isCycling)
			{
				this.animRedValue = this.myRig.playerColor.r;
				this.animGreenValue = this.myRig.playerColor.g;
				this.animBlueValue = this.myRig.playerColor.b;
			}
			this.isCycling = true;
			this.RainbowCycle(ref this.animRedValue, ref this.animGreenValue, ref this.animBlueValue);
			this.myRig.InitializeNoobMaterialLocal(this.animRedValue, this.animGreenValue, this.animBlueValue);
			return;
		}
		if (this.isCycling)
		{
			this.isCycling = false;
			if (this.myRig.isOfflineVRRig)
			{
				this.animRedValue = Mathf.Round(this.animRedValue * 9f) / 9f;
				this.animGreenValue = Mathf.Round(this.animGreenValue * 9f) / 9f;
				this.animBlueValue = Mathf.Round(this.animBlueValue * 9f) / 9f;
				GorillaTagger.Instance.UpdateColor(this.animRedValue, this.animGreenValue, this.animBlueValue);
				if (NetworkSystem.Instance.InRoom)
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[] { this.animRedValue, this.animGreenValue, this.animBlueValue });
				}
				PlayerPrefs.SetFloat("redValue", this.animRedValue);
				PlayerPrefs.SetFloat("greenValue", this.animGreenValue);
				PlayerPrefs.SetFloat("blueValue", this.animBlueValue);
				PlayerPrefs.Save();
			}
		}
	}

	// Token: 0x06002446 RID: 9286 RVA: 0x000C2C18 File Offset: 0x000C0E18
	private void RainbowCycle(ref float r, ref float g, ref float b)
	{
		float num = this.furCycleSpeed * Time.deltaTime;
		if (r == 1f)
		{
			if (b > 0f)
			{
				b = Mathf.Clamp01(b - num);
				return;
			}
			if (g < 1f)
			{
				g = Mathf.Clamp01(g + num);
				return;
			}
			r = Mathf.Clamp01(r - num);
			return;
		}
		else if (g == 1f)
		{
			if (r > 0f)
			{
				r = Mathf.Clamp01(r - num);
				return;
			}
			if (b < 1f)
			{
				b = Mathf.Clamp01(b + num);
				return;
			}
			g = Mathf.Clamp01(g - num);
			return;
		}
		else
		{
			if (b != 1f)
			{
				r = Mathf.Clamp01(r + num);
				return;
			}
			if (g > 0f)
			{
				g = Mathf.Clamp01(g - num);
				return;
			}
			if (r < 1f)
			{
				r = Mathf.Clamp01(r + num);
				return;
			}
			b = Mathf.Clamp01(b - num);
			return;
		}
	}

	// Token: 0x04002F84 RID: 12164
	[SerializeField]
	private bool attachedToLeftHand;

	// Token: 0x04002F85 RID: 12165
	private VRRig myRig;

	// Token: 0x04002F86 RID: 12166
	[SerializeField]
	private float rotationSpeed;

	// Token: 0x04002F87 RID: 12167
	[SerializeField]
	private float furCycleSpeed;

	// Token: 0x04002F88 RID: 12168
	private float nextFurCycleTimestamp;

	// Token: 0x04002F89 RID: 12169
	private float animRedValue;

	// Token: 0x04002F8A RID: 12170
	private float animGreenValue;

	// Token: 0x04002F8B RID: 12171
	private float animBlueValue;

	// Token: 0x04002F8C RID: 12172
	private bool isCycling;
}
