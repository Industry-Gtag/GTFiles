using System;
using UnityEngine;

// Token: 0x020002BD RID: 701
public class Crossbow : ProjectileWeapon
{
	// Token: 0x06001219 RID: 4633 RVA: 0x0006134C File Offset: 0x0005F54C
	protected override void Awake()
	{
		base.Awake();
		TransferrableObjectHoldablePart_Crank[] array = this.cranks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetOnCrankedCallback(new Action<float>(this.OnCrank));
		}
		this.SetReloadFraction(0f);
	}

	// Token: 0x0600121A RID: 4634 RVA: 0x00061394 File Offset: 0x0005F594
	public void SetReloadFraction(float newFraction)
	{
		this.loadFraction = Mathf.Clamp01(newFraction);
		this.animator.SetFloat(this.ReloadFractionHashID, this.loadFraction);
		if (this.loadFraction == 1f && !this.dummyProjectile.enabled)
		{
			this.shootSfx.GTPlayOneShot(this.reloadComplete_audioClip, 1f);
			this.dummyProjectile.enabled = true;
			return;
		}
		if (this.loadFraction < 1f && this.dummyProjectile.enabled)
		{
			this.dummyProjectile.enabled = false;
		}
	}

	// Token: 0x0600121B RID: 4635 RVA: 0x0006142C File Offset: 0x0005F62C
	private void OnCrank(float degrees)
	{
		if (this.loadFraction == 1f)
		{
			return;
		}
		this.totalCrankDegrees += degrees;
		this.crankSoundDegrees += degrees;
		if (Mathf.Abs(this.crankSoundDegrees) > this.crankSoundDegreesThreshold)
		{
			this.playingCrankSoundUntilTimestamp = Time.time + this.crankSoundContinueDuration;
			this.crankSoundDegrees = 0f;
		}
		if (!this.reloadAudio.isPlaying && Time.time < this.playingCrankSoundUntilTimestamp)
		{
			this.reloadAudio.GTPlay();
		}
		this.SetReloadFraction(Mathf.Abs(this.totalCrankDegrees / this.crankTotalDegreesToReload));
		if (this.loadFraction >= 1f)
		{
			this.totalCrankDegrees = 0f;
		}
	}

	// Token: 0x0600121C RID: 4636 RVA: 0x000614E8 File Offset: 0x0005F6E8
	protected override Vector3 GetLaunchPosition()
	{
		return this.launchPosition.position;
	}

	// Token: 0x0600121D RID: 4637 RVA: 0x000614F5 File Offset: 0x0005F6F5
	protected override Vector3 GetLaunchVelocity()
	{
		return this.launchPosition.forward * this.launchSpeed * base.myRig.scaleFactor;
	}

	// Token: 0x0600121E RID: 4638 RVA: 0x00061520 File Offset: 0x0005F720
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!base.InHand())
		{
			this.wasPressingTrigger = false;
			return;
		}
		if ((base.InLeftHand() ? base.myRig.leftIndex.calcT : base.myRig.rightIndex.calcT) > 0.5f)
		{
			if (this.loadFraction == 1f && !this.wasPressingTrigger)
			{
				this.SetReloadFraction(0f);
				this.animator.SetTrigger(this.FireHashID);
				base.LaunchProjectile();
			}
			this.wasPressingTrigger = true;
		}
		else
		{
			this.wasPressingTrigger = false;
		}
		if (this.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			if (this.loadFraction < 1f)
			{
				this.itemState &= (TransferrableObject.ItemStates)(-2);
				return;
			}
		}
		else if (this.loadFraction == 1f)
		{
			this.itemState |= TransferrableObject.ItemStates.State0;
		}
	}

	// Token: 0x0600121F RID: 4639 RVA: 0x00061610 File Offset: 0x0005F810
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (!base.InHand())
		{
			return;
		}
		if (this.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			this.SetReloadFraction(1f);
			return;
		}
		if (this.loadFraction == 1f)
		{
			this.SetReloadFraction(0f);
		}
	}

	// Token: 0x06001220 RID: 4640 RVA: 0x00061668 File Offset: 0x0005F868
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.reloadAudio.isPlaying && Time.time > this.playingCrankSoundUntilTimestamp)
		{
			this.reloadAudio.GTStop();
		}
	}

	// Token: 0x040015DD RID: 5597
	[SerializeField]
	private Transform launchPosition;

	// Token: 0x040015DE RID: 5598
	[SerializeField]
	private float launchSpeed;

	// Token: 0x040015DF RID: 5599
	[SerializeField]
	private Animator animator;

	// Token: 0x040015E0 RID: 5600
	[SerializeField]
	private float crankTotalDegreesToReload;

	// Token: 0x040015E1 RID: 5601
	[SerializeField]
	private TransferrableObjectHoldablePart_Crank[] cranks;

	// Token: 0x040015E2 RID: 5602
	[SerializeField]
	private MeshRenderer dummyProjectile;

	// Token: 0x040015E3 RID: 5603
	[SerializeField]
	private AudioSource reloadAudio;

	// Token: 0x040015E4 RID: 5604
	[SerializeField]
	private AudioClip reloadComplete_audioClip;

	// Token: 0x040015E5 RID: 5605
	[SerializeField]
	private float crankSoundContinueDuration = 0.1f;

	// Token: 0x040015E6 RID: 5606
	[SerializeField]
	private float crankSoundDegreesThreshold = 0.1f;

	// Token: 0x040015E7 RID: 5607
	private AnimHashId FireHashID = "Fire";

	// Token: 0x040015E8 RID: 5608
	private AnimHashId ReloadFractionHashID = "ReloadFraction";

	// Token: 0x040015E9 RID: 5609
	private float totalCrankDegrees;

	// Token: 0x040015EA RID: 5610
	private float loadFraction;

	// Token: 0x040015EB RID: 5611
	private float playingCrankSoundUntilTimestamp;

	// Token: 0x040015EC RID: 5612
	private float crankSoundDegrees;

	// Token: 0x040015ED RID: 5613
	private bool wasPressingTrigger;
}
