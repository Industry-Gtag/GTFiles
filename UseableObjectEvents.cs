using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020005B0 RID: 1456
public class UseableObjectEvents : MonoBehaviour
{
	// Token: 0x060024E8 RID: 9448 RVA: 0x000C6214 File Offset: 0x000C4414
	public void Init(NetPlayer player)
	{
		bool isLocal = player.IsLocal;
		PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
		string text;
		if (isLocal && instance != null)
		{
			text = instance.GetPlayFabPlayerId();
		}
		else
		{
			text = player.NickName;
		}
		this.PlayerIdString = text + "." + base.gameObject.name;
		this.PlayerId = this.PlayerIdString.GetStaticHash();
		this.DisposeEvents();
		this.Activate = new PhotonEvent(this.PlayerId.ToString() + ".Activate");
		this.Deactivate = new PhotonEvent(this.PlayerId.ToString() + ".Deactivate");
		this.Activate.reliable = false;
		this.Deactivate.reliable = false;
	}

	// Token: 0x060024E9 RID: 9449 RVA: 0x000C62D5 File Offset: 0x000C44D5
	private void OnEnable()
	{
		PhotonEvent activate = this.Activate;
		if (activate != null)
		{
			activate.Enable();
		}
		PhotonEvent deactivate = this.Deactivate;
		if (deactivate == null)
		{
			return;
		}
		deactivate.Enable();
	}

	// Token: 0x060024EA RID: 9450 RVA: 0x000C62F8 File Offset: 0x000C44F8
	private void OnDisable()
	{
		PhotonEvent activate = this.Activate;
		if (activate != null)
		{
			activate.Disable();
		}
		PhotonEvent deactivate = this.Deactivate;
		if (deactivate == null)
		{
			return;
		}
		deactivate.Disable();
	}

	// Token: 0x060024EB RID: 9451 RVA: 0x000C631B File Offset: 0x000C451B
	private void OnDestroy()
	{
		this.DisposeEvents();
	}

	// Token: 0x060024EC RID: 9452 RVA: 0x000C6323 File Offset: 0x000C4523
	private void DisposeEvents()
	{
		PhotonEvent activate = this.Activate;
		if (activate != null)
		{
			activate.Dispose();
		}
		this.Activate = null;
		PhotonEvent deactivate = this.Deactivate;
		if (deactivate != null)
		{
			deactivate.Dispose();
		}
		this.Deactivate = null;
	}

	// Token: 0x0400306E RID: 12398
	[NonSerialized]
	private string PlayerIdString;

	// Token: 0x0400306F RID: 12399
	[NonSerialized]
	private int PlayerId;

	// Token: 0x04003070 RID: 12400
	public PhotonEvent Activate;

	// Token: 0x04003071 RID: 12401
	public PhotonEvent Deactivate;
}
