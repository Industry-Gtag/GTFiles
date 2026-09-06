using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000680 RID: 1664
public abstract class CosmeticCritterHoldable : MonoBehaviour
{
	// Token: 0x17000433 RID: 1075
	// (get) Token: 0x060029A8 RID: 10664 RVA: 0x000E11B3 File Offset: 0x000DF3B3
	// (set) Token: 0x060029A9 RID: 10665 RVA: 0x000E11BB File Offset: 0x000DF3BB
	public int OwnerID { get; private set; }

	// Token: 0x17000434 RID: 1076
	// (get) Token: 0x060029AA RID: 10666 RVA: 0x000E11C4 File Offset: 0x000DF3C4
	public bool IsLocal
	{
		get
		{
			return this.transferrableObject.IsLocalObject();
		}
	}

	// Token: 0x060029AB RID: 10667 RVA: 0x000E11D1 File Offset: 0x000DF3D1
	public bool OwningPlayerMatches(PhotonMessageInfoWrapped info)
	{
		return this.transferrableObject.targetRig.creator == info.Sender;
	}

	// Token: 0x060029AC RID: 10668 RVA: 0x000E11EB File Offset: 0x000DF3EB
	protected virtual CallLimiter CreateCallLimiter()
	{
		return new CallLimiter(10, 2f, 0.5f);
	}

	// Token: 0x060029AD RID: 10669 RVA: 0x000E11FE File Offset: 0x000DF3FE
	public void ResetCallLimiter()
	{
		this.callLimiter.Reset();
	}

	// Token: 0x060029AE RID: 10670 RVA: 0x000E120C File Offset: 0x000DF40C
	private void TrySetID()
	{
		if (this.IsLocal)
		{
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			if (instance != null)
			{
				string playFabPlayerId = instance.GetPlayFabPlayerId();
				Type type = base.GetType();
				this.OwnerID = (playFabPlayerId + ((type != null) ? type.ToString() : null)).GetStaticHash();
				return;
			}
		}
		else if (this.transferrableObject.targetRig != null && this.transferrableObject.targetRig.creator != null)
		{
			string userId = this.transferrableObject.targetRig.creator.UserId;
			Type type2 = base.GetType();
			this.OwnerID = (userId + ((type2 != null) ? type2.ToString() : null)).GetStaticHash();
		}
	}

	// Token: 0x060029AF RID: 10671 RVA: 0x000E12BA File Offset: 0x000DF4BA
	protected virtual void Awake()
	{
		this.transferrableObject = base.GetComponentInParent<TransferrableObject>();
		this.callLimiter = this.CreateCallLimiter();
		if (this.IsLocal)
		{
			CosmeticCritterManager.Instance.RegisterLocalHoldable(this);
		}
	}

	// Token: 0x060029B0 RID: 10672 RVA: 0x000E12E7 File Offset: 0x000DF4E7
	protected virtual void OnEnable()
	{
		this.TrySetID();
	}

	// Token: 0x060029B1 RID: 10673 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnDisable()
	{
	}

	// Token: 0x0400363A RID: 13882
	protected TransferrableObject transferrableObject;

	// Token: 0x0400363C RID: 13884
	protected CallLimiter callLimiter;
}
