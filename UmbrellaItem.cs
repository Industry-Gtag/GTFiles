using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000559 RID: 1369
public class UmbrellaItem : TransferrableObject
{
	// Token: 0x060022D7 RID: 8919 RVA: 0x000BB22A File Offset: 0x000B942A
	protected override void Start()
	{
		base.Start();
		this.itemState = TransferrableObject.ItemStates.State1;
	}

	// Token: 0x060022D8 RID: 8920 RVA: 0x000BB23C File Offset: 0x000B943C
	public override void OnActivate()
	{
		base.OnActivate();
		float num = GorillaTagger.Instance.tapHapticStrength / 4f;
		float fixedDeltaTime = Time.fixedDeltaTime;
		float num2 = 0.08f;
		int num3;
		if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			num3 = this.SoundIdOpen;
			this.itemState = TransferrableObject.ItemStates.State0;
			BetterDayNightManager.instance.collidersToAddToWeatherSystems.Add(this.umbrellaRainDestroyTrigger);
		}
		else
		{
			num3 = this.SoundIdClose;
			this.itemState = TransferrableObject.ItemStates.State1;
			BetterDayNightManager.instance.collidersToAddToWeatherSystems.Remove(this.umbrellaRainDestroyTrigger);
		}
		base.ActivateItemFX(num, fixedDeltaTime, num3, num2);
		this.OnUmbrellaStateChanged();
	}

	// Token: 0x060022D9 RID: 8921 RVA: 0x000BB2D4 File Offset: 0x000B94D4
	internal override void OnEnable()
	{
		base.OnEnable();
		this.OnUmbrellaStateChanged();
	}

	// Token: 0x060022DA RID: 8922 RVA: 0x000BB2E2 File Offset: 0x000B94E2
	internal override void OnDisable()
	{
		base.OnDisable();
		BetterDayNightManager.instance.collidersToAddToWeatherSystems.Remove(this.umbrellaRainDestroyTrigger);
	}

	// Token: 0x060022DB RID: 8923 RVA: 0x000BB302 File Offset: 0x000B9502
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		BetterDayNightManager.instance.collidersToAddToWeatherSystems.Remove(this.umbrellaRainDestroyTrigger);
		this.itemState = TransferrableObject.ItemStates.State1;
		this.OnUmbrellaStateChanged();
	}

	// Token: 0x060022DC RID: 8924 RVA: 0x000BB32F File Offset: 0x000B952F
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (base.InHand())
		{
			return false;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			this.OnActivate();
		}
		return true;
	}

	// Token: 0x060022DD RID: 8925 RVA: 0x000BB358 File Offset: 0x000B9558
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		UmbrellaItem.UmbrellaStates itemState = (UmbrellaItem.UmbrellaStates)this.itemState;
		if (itemState != this.previousUmbrellaState)
		{
			this.OnUmbrellaStateChanged();
		}
		this.UpdateAngles((itemState == UmbrellaItem.UmbrellaStates.UmbrellaOpen) ? this.startingAngles : this.endingAngles, this.lerpValue);
		this.previousUmbrellaState = itemState;
	}

	// Token: 0x060022DE RID: 8926 RVA: 0x000BB3A8 File Offset: 0x000B95A8
	protected virtual void OnUmbrellaStateChanged()
	{
		bool flag = this.itemState == TransferrableObject.ItemStates.State0;
		GameObject[] array = this.gameObjectsActivatedOnOpen;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(flag);
		}
		ParticleSystem[] array2;
		if (flag)
		{
			array2 = this.particlesEmitOnOpen;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Play();
			}
			return;
		}
		array2 = this.particlesEmitOnOpen;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Stop();
		}
	}

	// Token: 0x060022DF RID: 8927 RVA: 0x000BB41C File Offset: 0x000B961C
	protected virtual void UpdateAngles(Quaternion[] toAngles, float t)
	{
		for (int i = 0; i < this.umbrellaBones.Length; i++)
		{
			this.umbrellaBones[i].localRotation = Quaternion.Lerp(this.umbrellaBones[i].localRotation, toAngles[i], t);
		}
	}

	// Token: 0x060022E0 RID: 8928 RVA: 0x000BB464 File Offset: 0x000B9664
	protected void GenerateAngles()
	{
		this.startingAngles = new Quaternion[this.umbrellaBones.Length];
		for (int i = 0; i < this.endingAngles.Length; i++)
		{
			this.startingAngles[i] = this.umbrellaToCopy.startingAngles[i];
		}
		this.endingAngles = new Quaternion[this.umbrellaBones.Length];
		for (int j = 0; j < this.endingAngles.Length; j++)
		{
			this.endingAngles[j] = this.umbrellaToCopy.endingAngles[j];
		}
	}

	// Token: 0x060022E1 RID: 8929 RVA: 0x00023F0C File Offset: 0x0002210C
	public override bool CanActivate()
	{
		return true;
	}

	// Token: 0x060022E2 RID: 8930 RVA: 0x00023F0C File Offset: 0x0002210C
	public override bool CanDeactivate()
	{
		return true;
	}

	// Token: 0x04002DED RID: 11757
	[AssignInCorePrefab]
	public Transform[] umbrellaBones;

	// Token: 0x04002DEE RID: 11758
	[AssignInCorePrefab]
	public Quaternion[] startingAngles;

	// Token: 0x04002DEF RID: 11759
	[AssignInCorePrefab]
	public Quaternion[] endingAngles;

	// Token: 0x04002DF0 RID: 11760
	[AssignInCorePrefab]
	[Tooltip("Assign to use the 'Generate Angles' button")]
	private UmbrellaItem umbrellaToCopy;

	// Token: 0x04002DF1 RID: 11761
	[AssignInCorePrefab]
	public float lerpValue = 0.25f;

	// Token: 0x04002DF2 RID: 11762
	[AssignInCorePrefab]
	public Collider umbrellaRainDestroyTrigger;

	// Token: 0x04002DF3 RID: 11763
	[AssignInCorePrefab]
	public GameObject[] gameObjectsActivatedOnOpen;

	// Token: 0x04002DF4 RID: 11764
	[AssignInCorePrefab]
	public ParticleSystem[] particlesEmitOnOpen;

	// Token: 0x04002DF5 RID: 11765
	[GorillaSoundLookup]
	public int SoundIdOpen = 64;

	// Token: 0x04002DF6 RID: 11766
	[GorillaSoundLookup]
	public int SoundIdClose = 65;

	// Token: 0x04002DF7 RID: 11767
	private UmbrellaItem.UmbrellaStates previousUmbrellaState = UmbrellaItem.UmbrellaStates.UmbrellaOpen;

	// Token: 0x0200055A RID: 1370
	private enum UmbrellaStates
	{
		// Token: 0x04002DF9 RID: 11769
		UmbrellaOpen = 1,
		// Token: 0x04002DFA RID: 11770
		UmbrellaClosed
	}
}
