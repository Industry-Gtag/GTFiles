using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000D2C RID: 3372
internal class GuardianRPCs : RPCNetworkBase
{
	// Token: 0x06005371 RID: 21361 RVA: 0x001B7C82 File Offset: 0x001B5E82
	public override void SetClassTarget(IWrappedSerializable target, GorillaWrappedSerializer netHandler)
	{
		this.guardianManager = (GorillaGuardianManager)target;
		this.serializer = (GameModeSerializer)netHandler;
	}

	// Token: 0x06005372 RID: 21362 RVA: 0x001B7C9C File Offset: 0x001B5E9C
	[PunRPC]
	public void GuardianRequestEject(PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "GuardianRequestEject");
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		if (photonMessageInfoWrapped.Sender != null)
		{
			this.guardianManager.EjectGuardian(photonMessageInfoWrapped.Sender);
		}
	}

	// Token: 0x06005373 RID: 21363 RVA: 0x001B7CD8 File Offset: 0x001B5ED8
	[PunRPC]
	public void GuardianLaunchPlayer(Vector3 velocity, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "GuardianLaunchPlayer");
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		if (!this.guardianManager.IsPlayerGuardian(photonMessageInfoWrapped.Sender))
		{
			MonkeAgent.instance.SendReport("Sent LaunchPlayer when not a guardian", photonMessageInfoWrapped.Sender.UserId, photonMessageInfoWrapped.Sender.NickName);
			return;
		}
		float num = 10000f;
		if (!(in velocity).IsValid(in num))
		{
			return;
		}
		if (!this.launchCallLimit.CheckCallTime(Time.time))
		{
			return;
		}
		this.guardianManager.LaunchPlayer(photonMessageInfoWrapped.Sender, velocity);
	}

	// Token: 0x06005374 RID: 21364 RVA: 0x001B7D6C File Offset: 0x001B5F6C
	[PunRPC]
	public void ShowSlapEffects(Vector3 location, Vector3 direction, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "ShowSlapEffects");
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		if (!this.guardianManager.IsPlayerGuardian(photonMessageInfoWrapped.Sender))
		{
			MonkeAgent.instance.SendReport("Sent ShowSlapEffects when not a guardian", photonMessageInfoWrapped.Sender.UserId, photonMessageInfoWrapped.Sender.NickName);
			return;
		}
		float num = 10000f;
		if ((in location).IsValid(in num))
		{
			float num2 = 10000f;
			if ((in direction).IsValid(in num2))
			{
				if (!this.slapFXCallLimit.CheckCallTime(Time.time))
				{
					return;
				}
				this.guardianManager.PlaySlapEffect(location, direction);
				return;
			}
		}
	}

	// Token: 0x06005375 RID: 21365 RVA: 0x001B7E0C File Offset: 0x001B600C
	[PunRPC]
	public void ShowSlamEffect(Vector3 location, Vector3 direction, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "ShowSlamEffect");
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		if (!this.guardianManager.IsPlayerGuardian(photonMessageInfoWrapped.Sender))
		{
			MonkeAgent.instance.SendReport("Sent ShowSlamEffect when not a guardian", photonMessageInfoWrapped.Sender.UserId, photonMessageInfoWrapped.Sender.NickName);
			return;
		}
		float num = 10000f;
		if ((in location).IsValid(in num))
		{
			float num2 = 10000f;
			if ((in direction).IsValid(in num2))
			{
				if (!this.slamFXCallLimit.CheckCallTime(Time.time))
				{
					return;
				}
				this.guardianManager.PlaySlamEffect(location, direction);
				return;
			}
		}
	}

	// Token: 0x0400650E RID: 25870
	private GameModeSerializer serializer;

	// Token: 0x0400650F RID: 25871
	private GorillaGuardianManager guardianManager;

	// Token: 0x04006510 RID: 25872
	private CallLimiter launchCallLimit = new CallLimiter(5, 0.5f, 0.5f);

	// Token: 0x04006511 RID: 25873
	private CallLimiter slapFXCallLimit = new CallLimiter(5, 0.5f, 0.5f);

	// Token: 0x04006512 RID: 25874
	private CallLimiter slamFXCallLimit = new CallLimiter(5, 0.5f, 0.5f);
}
