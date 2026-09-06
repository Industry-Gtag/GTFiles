using System;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020007E2 RID: 2018
public class GRReviveStation : MonoBehaviour
{
	// Token: 0x170004AE RID: 1198
	// (get) Token: 0x06003384 RID: 13188 RVA: 0x00119BC8 File Offset: 0x00117DC8
	// (set) Token: 0x06003385 RID: 13189 RVA: 0x00119BD0 File Offset: 0x00117DD0
	public int Index { get; set; }

	// Token: 0x06003386 RID: 13190 RVA: 0x00119BD9 File Offset: 0x00117DD9
	public void Init(GhostReactor reactor, int index)
	{
		this.reactor = reactor;
		this.Index = index;
	}

	// Token: 0x06003387 RID: 13191 RVA: 0x00119BE9 File Offset: 0x00117DE9
	public void SetReviveCooldownSeconds(double seconds)
	{
		this.reviveCooldownSeconds = seconds;
	}

	// Token: 0x06003388 RID: 13192 RVA: 0x00119BF2 File Offset: 0x00117DF2
	public double GetReviveCooldownSeconds()
	{
		return this.reviveCooldownSeconds;
	}

	// Token: 0x06003389 RID: 13193 RVA: 0x00119BFC File Offset: 0x00117DFC
	public double CalculateRemainingReviveCooldownSeconds(int ActorNumber)
	{
		if (this.reviveCooldownSeconds == 0.0)
		{
			return 0.0;
		}
		if (this.cooldownStartTime.ContainsKey(ActorNumber))
		{
			return this.reviveCooldownSeconds - (GorillaComputer.instance.GetServerTime() - this.cooldownStartTime[ActorNumber]).TotalSeconds;
		}
		return 0.0;
	}

	// Token: 0x0600338A RID: 13194 RVA: 0x00119C68 File Offset: 0x00117E68
	public void RevivePlayer(GRPlayer player)
	{
		if (player != null)
		{
			int actorNumber = player.gamePlayer.rig.OwningNetPlayer.ActorNumber;
			this.cooldownStartTime[actorNumber] = GorillaComputer.instance.GetServerTime();
			if (player.State != GRPlayer.GRPlayerState.Alive || player.Hp < player.MaxHp)
			{
				player.OnPlayerRevive(this.reactor.grManager);
				if (this.audioSource != null)
				{
					this.audioSource.Play();
				}
				if (this.particleEffects != null)
				{
					for (int i = 0; i < this.particleEffects.Length; i++)
					{
						this.particleEffects[i].Play();
					}
				}
			}
		}
	}

	// Token: 0x0600338B RID: 13195 RVA: 0x00119D18 File Offset: 0x00117F18
	private void OnTriggerEnter(Collider collider)
	{
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			VRRig component = attachedRigidbody.GetComponent<VRRig>();
			if (component != null)
			{
				GRPlayer component2 = component.GetComponent<GRPlayer>();
				if (component2 != null && (component2.State != GRPlayer.GRPlayerState.Alive || component2.Hp < component2.MaxHp))
				{
					if (!NetworkSystem.Instance.InRoom && component == VRRig.LocalRig)
					{
						this.RevivePlayer(component2);
					}
					if (this.reactor.grManager.IsAuthority() && this.CalculateRemainingReviveCooldownSeconds(component2.gamePlayer.rig.OwningNetPlayer.ActorNumber) <= 0.0)
					{
						this.reactor.grManager.RequestPlayerRevive(this, component2);
					}
				}
			}
		}
	}

	// Token: 0x040042D9 RID: 17113
	public AudioSource audioSource;

	// Token: 0x040042DA RID: 17114
	public ParticleSystem[] particleEffects;

	// Token: 0x040042DB RID: 17115
	[SerializeField]
	private double reviveCooldownSeconds;

	// Token: 0x040042DC RID: 17116
	private Dictionary<int, DateTime> cooldownStartTime = new Dictionary<int, DateTime>();

	// Token: 0x040042DE RID: 17118
	private GhostReactor reactor;
}
