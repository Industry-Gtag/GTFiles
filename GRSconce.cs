using System;
using UnityEngine;

// Token: 0x020007E5 RID: 2021
public class GRSconce : MonoBehaviour
{
	// Token: 0x06003394 RID: 13204 RVA: 0x00119E90 File Offset: 0x00118090
	private void Awake()
	{
		if (this.tool != null)
		{
			this.tool.OnEnergyChange += this.OnEnergyChange;
		}
		if (this.gameEntity != null)
		{
			this.gameEntity.OnStateChanged += this.OnStateChange;
		}
		this.state = GRSconce.State.Off;
		this.StopLight();
	}

	// Token: 0x06003395 RID: 13205 RVA: 0x00119EF4 File Offset: 0x001180F4
	private bool IsAuthority()
	{
		return this.gameEntity.IsAuthority();
	}

	// Token: 0x06003396 RID: 13206 RVA: 0x00119F04 File Offset: 0x00118104
	private void SetState(GRSconce.State newState)
	{
		this.state = newState;
		GRSconce.State state = this.state;
		if (state != GRSconce.State.Off)
		{
			if (state == GRSconce.State.On)
			{
				this.StartLight();
			}
		}
		else
		{
			this.StopLight();
		}
		if (this.IsAuthority())
		{
			this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
		}
	}

	// Token: 0x06003397 RID: 13207 RVA: 0x00119F58 File Offset: 0x00118158
	private void StartLight()
	{
		this.gameLight.gameObject.SetActive(true);
		this.audioSource.volume = this.lightOnSoundVolume;
		this.audioSource.clip = this.lightOnSound;
		this.audioSource.Play();
		this.meshRenderer.material = this.onMaterial;
	}

	// Token: 0x06003398 RID: 13208 RVA: 0x00119FB4 File Offset: 0x001181B4
	private void StopLight()
	{
		this.gameLight.gameObject.SetActive(false);
		this.meshRenderer.material = this.offMaterial;
	}

	// Token: 0x06003399 RID: 13209 RVA: 0x00119FD8 File Offset: 0x001181D8
	private void OnEnergyChange(GRTool tool, int energy, GameEntityId chargingEntityId)
	{
		if (this.IsAuthority() && this.state == GRSconce.State.Off && tool.IsEnergyFull())
		{
			this.SetState(GRSconce.State.On);
		}
	}

	// Token: 0x0600339A RID: 13210 RVA: 0x00119FFC File Offset: 0x001181FC
	private void OnStateChange(long prevState, long nextState)
	{
		if (!this.IsAuthority())
		{
			GRSconce.State state = (GRSconce.State)nextState;
			this.SetState(state);
		}
	}

	// Token: 0x040042E4 RID: 17124
	public GameEntity gameEntity;

	// Token: 0x040042E5 RID: 17125
	public GameLight gameLight;

	// Token: 0x040042E6 RID: 17126
	public GRTool tool;

	// Token: 0x040042E7 RID: 17127
	public MeshRenderer meshRenderer;

	// Token: 0x040042E8 RID: 17128
	public Material offMaterial;

	// Token: 0x040042E9 RID: 17129
	public Material onMaterial;

	// Token: 0x040042EA RID: 17130
	public AudioSource audioSource;

	// Token: 0x040042EB RID: 17131
	public AudioClip lightOnSound;

	// Token: 0x040042EC RID: 17132
	public float lightOnSoundVolume;

	// Token: 0x040042ED RID: 17133
	private GRSconce.State state;

	// Token: 0x020007E6 RID: 2022
	private enum State
	{
		// Token: 0x040042EF RID: 17135
		Off,
		// Token: 0x040042F0 RID: 17136
		On
	}
}
