using System;
using UnityEngine;

// Token: 0x0200075C RID: 1884
public class GRBarrierOverloadable : MonoBehaviour
{
	// Token: 0x06002FD1 RID: 12241 RVA: 0x001041B5 File Offset: 0x001023B5
	private void OnEnable()
	{
		this.tool.OnEnergyChange += this.OnEnergyChange;
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
	}

	// Token: 0x06002FD2 RID: 12242 RVA: 0x001041E8 File Offset: 0x001023E8
	private void OnEnergyChange(GRTool tool, int energyChange, GameEntityId chargingEntityId)
	{
		if (this.state == GRBarrierOverloadable.State.Active && tool.energy >= tool.GetEnergyMax())
		{
			this.SetState(GRBarrierOverloadable.State.Destroyed);
			if (this.gameEntity.IsAuthority())
			{
				this.gameEntity.RequestState(this.gameEntity.id, 1L);
			}
		}
	}

	// Token: 0x06002FD3 RID: 12243 RVA: 0x00104237 File Offset: 0x00102437
	private void OnEntityStateChanged(long prevState, long nextState)
	{
		if (!this.gameEntity.IsAuthority())
		{
			this.SetState((GRBarrierOverloadable.State)nextState);
		}
	}

	// Token: 0x06002FD4 RID: 12244 RVA: 0x00104250 File Offset: 0x00102450
	public void SetState(GRBarrierOverloadable.State newState)
	{
		if (this.state != newState)
		{
			this.state = newState;
			GRBarrierOverloadable.State state = this.state;
			if (state == GRBarrierOverloadable.State.Active)
			{
				this.meshRenderer.enabled = true;
				this.collider.enabled = true;
				return;
			}
			if (state != GRBarrierOverloadable.State.Destroyed)
			{
				return;
			}
			this.audioSource.Play();
			this.meshRenderer.enabled = false;
			this.collider.enabled = false;
		}
	}

	// Token: 0x04003D40 RID: 15680
	public GRTool tool;

	// Token: 0x04003D41 RID: 15681
	public GameEntity gameEntity;

	// Token: 0x04003D42 RID: 15682
	public AudioSource audioSource;

	// Token: 0x04003D43 RID: 15683
	public MeshRenderer meshRenderer;

	// Token: 0x04003D44 RID: 15684
	public Collider collider;

	// Token: 0x04003D45 RID: 15685
	private GRBarrierOverloadable.State state;

	// Token: 0x0200075D RID: 1885
	public enum State
	{
		// Token: 0x04003D47 RID: 15687
		Active,
		// Token: 0x04003D48 RID: 15688
		Destroyed
	}
}
