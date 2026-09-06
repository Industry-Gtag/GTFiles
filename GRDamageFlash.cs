using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000788 RID: 1928
[Serializable]
public class GRDamageFlash
{
	// Token: 0x060030D8 RID: 12504 RVA: 0x00108F68 File Offset: 0x00107168
	public void Setup()
	{
		this.flashRendererDefaultMaterial = new List<Material>(this.flashRenderers.Count);
		this.stateMachine = new SimpleStateMachine<GRDamageFlash.State>();
		for (int i = 0; i < this.flashRenderers.Count; i++)
		{
			this.flashRendererDefaultMaterial.Add(this.flashRenderers[i].sharedMaterial);
		}
		this.stateMachine.Setup(GRDamageFlash.State.Idle, new Action<GRDamageFlash.State>(this.OnStateStart), new Action<GRDamageFlash.State>(this.OnStateEnd), new Action<GRDamageFlash.State>(this.OnStateUpdate));
	}

	// Token: 0x060030D9 RID: 12505 RVA: 0x00108FF8 File Offset: 0x001071F8
	public void Play()
	{
		if (this.stateMachine.GetState() == GRDamageFlash.State.Idle)
		{
			this.stateMachine.SetState(GRDamageFlash.State.Playing, false);
		}
	}

	// Token: 0x060030DA RID: 12506 RVA: 0x00109014 File Offset: 0x00107214
	public void OnStateStart(GRDamageFlash.State state)
	{
		if (state == GRDamageFlash.State.Playing)
		{
			for (int i = 0; i < this.flashRenderers.Count; i++)
			{
				this.flashRenderers[i].material = this.flashMaterial;
			}
		}
	}

	// Token: 0x060030DB RID: 12507 RVA: 0x00109054 File Offset: 0x00107254
	public void OnStateEnd(GRDamageFlash.State state)
	{
		if (state == GRDamageFlash.State.Playing)
		{
			for (int i = 0; i < this.flashRenderers.Count; i++)
			{
				this.flashRenderers[i].material = this.flashRendererDefaultMaterial[i];
			}
		}
	}

	// Token: 0x060030DC RID: 12508 RVA: 0x00109098 File Offset: 0x00107298
	public void OnStateUpdate(GRDamageFlash.State state)
	{
		if (state != GRDamageFlash.State.Playing)
		{
			if (state != GRDamageFlash.State.Cooldown)
			{
				return;
			}
			if (this.stateMachine.IsStateFinished(Time.timeAsDouble, this.flashCooldown))
			{
				this.stateMachine.SetState(GRDamageFlash.State.Idle, false);
			}
		}
		else if (this.stateMachine.IsStateFinished(Time.timeAsDouble, this.flashDuration))
		{
			this.stateMachine.SetState((this.flashCooldown > 0f) ? GRDamageFlash.State.Cooldown : GRDamageFlash.State.Idle, false);
			return;
		}
	}

	// Token: 0x060030DD RID: 12509 RVA: 0x00109109 File Offset: 0x00107309
	public void Stop()
	{
		this.stateMachine.SetState(GRDamageFlash.State.Idle, false);
	}

	// Token: 0x060030DE RID: 12510 RVA: 0x00109118 File Offset: 0x00107318
	public void Update()
	{
		this.stateMachine.Update();
	}

	// Token: 0x04003E7A RID: 15994
	public Material flashMaterial;

	// Token: 0x04003E7B RID: 15995
	public float flashDuration = 0.1f;

	// Token: 0x04003E7C RID: 15996
	public float flashCooldown = 0.1f;

	// Token: 0x04003E7D RID: 15997
	public List<Renderer> flashRenderers;

	// Token: 0x04003E7E RID: 15998
	private SimpleStateMachine<GRDamageFlash.State> stateMachine;

	// Token: 0x04003E7F RID: 15999
	private List<Material> flashRendererDefaultMaterial;

	// Token: 0x02000789 RID: 1929
	public enum State
	{
		// Token: 0x04003E81 RID: 16001
		Idle,
		// Token: 0x04003E82 RID: 16002
		Playing,
		// Token: 0x04003E83 RID: 16003
		Cooldown
	}
}
