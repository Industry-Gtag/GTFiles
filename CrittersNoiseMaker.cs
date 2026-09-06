using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200006D RID: 109
public class CrittersNoiseMaker : CrittersToolThrowable
{
	// Token: 0x06000270 RID: 624 RVA: 0x0000EA2E File Offset: 0x0000CC2E
	protected override void OnImpact(Vector3 hitPosition, Vector3 hitNormal)
	{
		if (CrittersManager.instance.LocalAuthority())
		{
			if (this.destroyOnImpact || this.playOnce)
			{
				this.PlaySingleNoise();
				return;
			}
			this.StartPlayingRepeatNoise();
		}
	}

	// Token: 0x06000271 RID: 625 RVA: 0x0000EA5B File Offset: 0x0000CC5B
	protected override void OnImpactCritter(CrittersPawn impactedCritter)
	{
		this.OnImpact(impactedCritter.transform.position, impactedCritter.transform.up);
	}

	// Token: 0x06000272 RID: 626 RVA: 0x0000EA79 File Offset: 0x0000CC79
	protected override void OnPickedUp()
	{
		this.StopPlayRepeatNoise();
	}

	// Token: 0x06000273 RID: 627 RVA: 0x0000EA84 File Offset: 0x0000CC84
	private void PlaySingleNoise()
	{
		CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)CrittersManager.instance.SpawnActor(CrittersActor.CrittersActorType.LoudNoise, this.soundSubIndex);
		if (crittersLoudNoise == null)
		{
			return;
		}
		crittersLoudNoise.MoveActor(base.transform.position, base.transform.rotation, false, true, true);
		crittersLoudNoise.SetImpulseVelocity(Vector3.zero, Vector3.zero);
		CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.NoiseMakerTriggered, this.actorId, base.transform.position);
	}

	// Token: 0x06000274 RID: 628 RVA: 0x0000EB01 File Offset: 0x0000CD01
	private void StartPlayingRepeatNoise()
	{
		this.StopPlayRepeatNoise();
		this.repeatPlayNoise = base.StartCoroutine(this.PlayRepeatNoise());
	}

	// Token: 0x06000275 RID: 629 RVA: 0x0000EB1B File Offset: 0x0000CD1B
	private void StopPlayRepeatNoise()
	{
		if (this.repeatPlayNoise != null)
		{
			base.StopCoroutine(this.repeatPlayNoise);
			this.repeatPlayNoise = null;
		}
	}

	// Token: 0x06000276 RID: 630 RVA: 0x0000EB38 File Offset: 0x0000CD38
	private IEnumerator PlayRepeatNoise()
	{
		int num = Mathf.FloorToInt(this.repeatNoiseDuration / this.repeatNoiseRate);
		int num2;
		for (int i = num; i > 0; i = num2 - 1)
		{
			this.PlaySingleNoise();
			yield return new WaitForSeconds(this.repeatNoiseRate);
			num2 = i;
		}
		if (this.destroyAfterPlayingRepeatNoise)
		{
			this.shouldDisable = true;
		}
		yield break;
	}

	// Token: 0x040002C1 RID: 705
	[Header("Noise Maker")]
	public int soundSubIndex = 3;

	// Token: 0x040002C2 RID: 706
	public bool playOnce = true;

	// Token: 0x040002C3 RID: 707
	public float repeatNoiseDuration;

	// Token: 0x040002C4 RID: 708
	public float repeatNoiseRate;

	// Token: 0x040002C5 RID: 709
	public bool destroyAfterPlayingRepeatNoise = true;

	// Token: 0x040002C6 RID: 710
	private Coroutine repeatPlayNoise;
}
