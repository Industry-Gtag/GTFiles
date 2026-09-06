using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020005AB RID: 1451
public class RubberDuckEvents : MonoBehaviour
{
	// Token: 0x060024C3 RID: 9411 RVA: 0x000C5B28 File Offset: 0x000C3D28
	public void Init(NetPlayer player)
	{
		string text = player.UserId;
		if (string.IsNullOrEmpty(text))
		{
			bool isLocal = player.IsLocal;
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			if (isLocal && instance != null)
			{
				text = instance.GetPlayFabPlayerId();
			}
			else
			{
				text = player.NickName;
			}
		}
		this.PlayerIdString = text + "." + base.gameObject.name;
		this.PlayerId = this.PlayerIdString.GetStaticHash();
		this.Dispose();
		this.Activate = new PhotonEvent(string.Format("{0}.{1}", this.PlayerId, "Activate"));
		this.Deactivate = new PhotonEvent(string.Format("{0}.{1}", this.PlayerId, "Deactivate"));
		this.Activate.reliable = true;
		this.Deactivate.reliable = true;
	}

	// Token: 0x060024C4 RID: 9412 RVA: 0x000C5C02 File Offset: 0x000C3E02
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

	// Token: 0x060024C5 RID: 9413 RVA: 0x000C5C25 File Offset: 0x000C3E25
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

	// Token: 0x060024C6 RID: 9414 RVA: 0x000C5C48 File Offset: 0x000C3E48
	private void OnDestroy()
	{
		this.Dispose();
	}

	// Token: 0x060024C7 RID: 9415 RVA: 0x000C5C50 File Offset: 0x000C3E50
	public void Dispose()
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

	// Token: 0x0400304E RID: 12366
	public int PlayerId;

	// Token: 0x0400304F RID: 12367
	public string PlayerIdString;

	// Token: 0x04003050 RID: 12368
	public PhotonEvent Activate;

	// Token: 0x04003051 RID: 12369
	public PhotonEvent Deactivate;
}
