using System;
using System.Diagnostics;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000930 RID: 2352
public class Tappable : MonoBehaviour, IClickable
{
	// Token: 0x170005AD RID: 1453
	// (get) Token: 0x06003D99 RID: 15769 RVA: 0x0014E2D6 File Offset: 0x0014C4D6
	public bool IsLocalOnly
	{
		get
		{
			return this.localOnly;
		}
	}

	// Token: 0x06003D9A RID: 15770 RVA: 0x0014E2DE File Offset: 0x0014C4DE
	public void Validate()
	{
		this.CalculateId(true);
	}

	// Token: 0x06003D9B RID: 15771 RVA: 0x0014E2E7 File Offset: 0x0014C4E7
	protected virtual void OnEnable()
	{
		if (!this.useStaticId)
		{
			this.CalculateId(false);
		}
		TappableManager.Register(this);
	}

	// Token: 0x06003D9C RID: 15772 RVA: 0x0014E2FE File Offset: 0x0014C4FE
	protected virtual void OnDisable()
	{
		TappableManager.Unregister(this);
	}

	// Token: 0x06003D9D RID: 15773 RVA: 0x00023F0C File Offset: 0x0002210C
	public virtual bool CanTap(bool isLeftHand)
	{
		return true;
	}

	// Token: 0x06003D9E RID: 15774 RVA: 0x0014E306 File Offset: 0x0014C506
	public void OnTap()
	{
		this.OnTap(1f);
	}

	// Token: 0x06003D9F RID: 15775 RVA: 0x0014E314 File Offset: 0x0014C514
	public void OnTap(float tapStrength)
	{
		if (this.localOnly || !NetworkSystem.Instance.InRoom)
		{
			this.OnTapLocal(tapStrength, Time.time, PhotonMessageInfoWrapped.GetLocalDefault());
			return;
		}
		if (!this.manager)
		{
			return;
		}
		this.manager.photonView.RPC("SendOnTapRPC", RpcTarget.All, new object[] { this.tappableId, tapStrength });
	}

	// Token: 0x06003DA0 RID: 15776 RVA: 0x0014E388 File Offset: 0x0014C588
	public void OnGrab()
	{
		if (this.localOnly || !NetworkSystem.Instance.InRoom)
		{
			this.OnGrabLocal(Time.time, default(PhotonMessageInfoWrapped));
			return;
		}
		if (!this.manager)
		{
			return;
		}
		this.manager.photonView.RPC("SendOnGrabRPC", RpcTarget.All, new object[] { this.tappableId });
	}

	// Token: 0x06003DA1 RID: 15777 RVA: 0x0014E3F8 File Offset: 0x0014C5F8
	public void OnRelease()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			this.OnReleaseLocal(Time.time, default(PhotonMessageInfoWrapped));
			return;
		}
		if (!this.manager)
		{
			return;
		}
		this.manager.photonView.RPC("SendOnReleaseRPC", RpcTarget.All, new object[] { this.tappableId });
	}

	// Token: 0x06003DA2 RID: 15778 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped sender)
	{
	}

	// Token: 0x06003DA3 RID: 15779 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void OnGrabLocal(float tapTime, PhotonMessageInfoWrapped sender)
	{
	}

	// Token: 0x06003DA4 RID: 15780 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void OnReleaseLocal(float tapTime, PhotonMessageInfoWrapped sender)
	{
	}

	// Token: 0x06003DA5 RID: 15781 RVA: 0x0014E2DE File Offset: 0x0014C4DE
	private void EdRecalculateId()
	{
		this.CalculateId(true);
	}

	// Token: 0x06003DA6 RID: 15782 RVA: 0x0014E460 File Offset: 0x0014C660
	private void CalculateId(bool force = false)
	{
		Transform transform = base.transform;
		int hashCode = TransformUtils.ComputePathHash(transform).ToId128().GetHashCode();
		int staticHash = base.GetType().Name.GetStaticHash();
		int hashCode2 = transform.position.QuantizedId128().GetHashCode();
		int num = StaticHash.Compute(hashCode, staticHash, hashCode2);
		if (this.useStaticId)
		{
			if (string.IsNullOrEmpty(this.staticId) || force)
			{
				int instanceID = transform.GetInstanceID();
				int num2 = StaticHash.Compute(num, instanceID);
				this.staticId = string.Format("#ID_{0:X8}", num2);
			}
			this.tappableId = this.staticId.GetStaticHash();
			return;
		}
		this.tappableId = (Application.isPlaying ? num : 0);
	}

	// Token: 0x06003DA7 RID: 15783 RVA: 0x0014E52A File Offset: 0x0014C72A
	[Conditional("UNITY_EDITOR")]
	private void OnValidate()
	{
		this.CalculateId(false);
	}

	// Token: 0x06003DA8 RID: 15784 RVA: 0x0014E533 File Offset: 0x0014C733
	public void Click(bool leftHand = false)
	{
		this.OnTap();
	}

	// Token: 0x04004E60 RID: 20064
	public int tappableId;

	// Token: 0x04004E61 RID: 20065
	public string staticId;

	// Token: 0x04004E62 RID: 20066
	public bool useStaticId;

	// Token: 0x04004E63 RID: 20067
	[Tooltip("If true, tap cooldown will be ignored.  Tapping will be allowed/disallowed based on result of CanTap()")]
	public bool overrideTapCooldown;

	// Token: 0x04004E64 RID: 20068
	[Space]
	public TappableManager manager;

	// Token: 0x04004E65 RID: 20069
	public RpcTarget rpcTarget;

	// Token: 0x04004E66 RID: 20070
	[Tooltip("If true, OnTapped only fires on the client of the player who initiated the tap. Offline play counts as local, so the event still fires when not in a room.")]
	[SerializeField]
	protected bool localOnly;
}
