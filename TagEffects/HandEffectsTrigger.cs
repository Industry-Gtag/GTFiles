using System;
using GorillaExtensions;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02001177 RID: 4471
	public class HandEffectsTrigger : MonoBehaviour, IHandEffectsTrigger
	{
		// Token: 0x17000AB1 RID: 2737
		// (get) Token: 0x06007014 RID: 28692 RVA: 0x0024294B File Offset: 0x00240B4B
		public bool Static
		{
			get
			{
				return this.isStatic;
			}
		}

		// Token: 0x17000AB2 RID: 2738
		// (get) Token: 0x06007015 RID: 28693 RVA: 0x00242954 File Offset: 0x00240B54
		public bool FingersDown
		{
			get
			{
				return !(this.rig == null) && ((this.rightHand && this.rig.IsMakingFistRight()) || (!this.rightHand && this.rig.IsMakingFistLeft()));
			}
		}

		// Token: 0x17000AB3 RID: 2739
		// (get) Token: 0x06007016 RID: 28694 RVA: 0x002429A0 File Offset: 0x00240BA0
		public bool FingersUp
		{
			get
			{
				return !(this.rig == null) && ((this.rightHand && this.rig.IsMakingFiveRight()) || (!this.rightHand && this.rig.IsMakingFiveLeft()));
			}
		}

		// Token: 0x17000AB4 RID: 2740
		// (get) Token: 0x06007017 RID: 28695 RVA: 0x002429EC File Offset: 0x00240BEC
		public Vector3 Velocity
		{
			get
			{
				if (this.velocityEstimator != null && this.rig != null && this.rig.scaleFactor > 0.001f)
				{
					return this.velocityEstimator.linearVelocity / this.rig.scaleFactor;
				}
				return Vector3.zero;
			}
		}

		// Token: 0x17000AB5 RID: 2741
		// (get) Token: 0x06007018 RID: 28696 RVA: 0x00242A48 File Offset: 0x00240C48
		bool IHandEffectsTrigger.RightHand
		{
			get
			{
				return this.rightHand;
			}
		}

		// Token: 0x17000AB6 RID: 2742
		// (get) Token: 0x06007019 RID: 28697 RVA: 0x00242A50 File Offset: 0x00240C50
		// (set) Token: 0x0600701A RID: 28698 RVA: 0x00242A58 File Offset: 0x00240C58
		public Action<IHandEffectsTrigger.Mode> OnTrigger { get; set; }

		// Token: 0x17000AB7 RID: 2743
		// (get) Token: 0x0600701B RID: 28699 RVA: 0x00242A61 File Offset: 0x00240C61
		public IHandEffectsTrigger.Mode EffectMode { get; }

		// Token: 0x17000AB8 RID: 2744
		// (get) Token: 0x0600701C RID: 28700 RVA: 0x000874AD File Offset: 0x000856AD
		public Transform Transform
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x17000AB9 RID: 2745
		// (get) Token: 0x0600701D RID: 28701 RVA: 0x00242A69 File Offset: 0x00240C69
		public VRRig Rig
		{
			get
			{
				return this.rig;
			}
		}

		// Token: 0x17000ABA RID: 2746
		// (get) Token: 0x0600701E RID: 28702 RVA: 0x00242A71 File Offset: 0x00240C71
		public TagEffectPack CosmeticEffectPack
		{
			get
			{
				if (this.rig == null)
				{
					return null;
				}
				return this.rig.CosmeticEffectPack;
			}
		}

		// Token: 0x0600701F RID: 28703 RVA: 0x00242A90 File Offset: 0x00240C90
		private void Awake()
		{
			this.rig = base.GetComponentInParent<VRRig>();
			if (this.velocityEstimator == null)
			{
				this.velocityEstimator = base.GetComponentInParent<GorillaVelocityEstimator>();
			}
			for (int i = 0; i < this.debugVisuals.Length; i++)
			{
				this.debugVisuals[i].SetActive(TagEffectsLibrary.DebugMode);
			}
		}

		// Token: 0x06007020 RID: 28704 RVA: 0x00087511 File Offset: 0x00085711
		private void OnEnable()
		{
			if (!HandEffectsTriggerRegistry.HasInstance)
			{
				HandEffectsTriggerRegistry.FindInstance();
			}
			HandEffectsTriggerRegistry.Instance.Register(this);
		}

		// Token: 0x06007021 RID: 28705 RVA: 0x0008752A File Offset: 0x0008572A
		private void OnDisable()
		{
			HandEffectsTriggerRegistry.Instance.Unregister(this);
		}

		// Token: 0x06007022 RID: 28706 RVA: 0x00242AE8 File Offset: 0x00240CE8
		public void OnTriggerEntered(IHandEffectsTrigger other)
		{
			if (this.rig == other.Rig)
			{
				return;
			}
			if (this.FingersDown && other.FingersDown && (other.Static || (Vector3.Dot(Vector3.Dot(this.Velocity, base.transform.up) * base.transform.up - Vector3.Dot(other.Velocity, other.Transform.up) * other.Transform.up, -other.Transform.up) > TagEffectsLibrary.FistBumpSpeedThreshold && Vector3.Dot(base.transform.up, other.Transform.up) < -0.01f)))
			{
				this.PlayHandEffects(TagEffectsLibrary.EffectType.FIST_BUMP, other);
			}
			if (this.FingersUp && other.FingersUp && (other.Static || Mathf.Abs(Vector3.Dot(Vector3.Dot(this.Velocity, base.transform.right) * base.transform.right - Vector3.Dot(other.Velocity, other.Transform.right) * other.Transform.right, other.Transform.right)) > TagEffectsLibrary.HighFiveSpeedThreshold))
			{
				this.PlayHandEffects(TagEffectsLibrary.EffectType.HIGH_FIVE, other);
			}
		}

		// Token: 0x06007023 RID: 28707 RVA: 0x00242C54 File Offset: 0x00240E54
		private void PlayHandEffects(TagEffectsLibrary.EffectType effectType, IHandEffectsTrigger other)
		{
			if (this.rig.IsNull())
			{
				return;
			}
			bool flag = false;
			if (this.rig.isOfflineVRRig)
			{
				PlayerGameEvents.TriggerHandEffect(effectType.ToString());
			}
			if (this.OnTrigger != null || (other != null && other.OnTrigger != null))
			{
				switch (effectType)
				{
				case TagEffectsLibrary.EffectType.FIRST_PERSON:
				{
					Action<IHandEffectsTrigger.Mode> onTrigger = this.OnTrigger;
					if (onTrigger != null)
					{
						onTrigger(IHandEffectsTrigger.Mode.Tag1P);
					}
					if (other != null)
					{
						Action<IHandEffectsTrigger.Mode> onTrigger2 = other.OnTrigger;
						if (onTrigger2 != null)
						{
							onTrigger2(IHandEffectsTrigger.Mode.Tag1P);
						}
					}
					break;
				}
				case TagEffectsLibrary.EffectType.THIRD_PERSON:
				{
					Action<IHandEffectsTrigger.Mode> onTrigger3 = this.OnTrigger;
					if (onTrigger3 != null)
					{
						onTrigger3(IHandEffectsTrigger.Mode.Tag3P);
					}
					if (other != null)
					{
						Action<IHandEffectsTrigger.Mode> onTrigger4 = other.OnTrigger;
						if (onTrigger4 != null)
						{
							onTrigger4(IHandEffectsTrigger.Mode.Tag3P);
						}
					}
					break;
				}
				case TagEffectsLibrary.EffectType.HIGH_FIVE:
				{
					Action<IHandEffectsTrigger.Mode> onTrigger5 = this.OnTrigger;
					if (onTrigger5 != null)
					{
						onTrigger5(IHandEffectsTrigger.Mode.HighFive);
					}
					if (other != null)
					{
						Action<IHandEffectsTrigger.Mode> onTrigger6 = other.OnTrigger;
						if (onTrigger6 != null)
						{
							onTrigger6(IHandEffectsTrigger.Mode.HighFive);
						}
					}
					break;
				}
				case TagEffectsLibrary.EffectType.FIST_BUMP:
				{
					Action<IHandEffectsTrigger.Mode> onTrigger7 = this.OnTrigger;
					if (onTrigger7 != null)
					{
						onTrigger7(IHandEffectsTrigger.Mode.FistBump);
					}
					if (other != null)
					{
						Action<IHandEffectsTrigger.Mode> onTrigger8 = other.OnTrigger;
						if (onTrigger8 != null)
						{
							onTrigger8(IHandEffectsTrigger.Mode.FistBump);
						}
					}
					break;
				}
				}
			}
			HandEffectsOverrideCosmetic handEffectsOverrideCosmetic = null;
			HandEffectsOverrideCosmetic handEffectsOverrideCosmetic2 = null;
			foreach (HandEffectsOverrideCosmetic handEffectsOverrideCosmetic3 in (this.rightHand ? this.rig.CosmeticHandEffectsOverride_Right : this.rig.CosmeticHandEffectsOverride_Left))
			{
				if (handEffectsOverrideCosmetic3.handEffectType == this.MapEnum(effectType))
				{
					handEffectsOverrideCosmetic2 = handEffectsOverrideCosmetic3;
					break;
				}
			}
			if (this.rig.isOfflineVRRig && GorillaTagger.Instance != null)
			{
				if (other.Rig)
				{
					foreach (HandEffectsOverrideCosmetic handEffectsOverrideCosmetic4 in ((other.Rig.CosmeticHandEffectsOverride_Right != null) ? other.Rig.CosmeticHandEffectsOverride_Right : other.Rig.CosmeticHandEffectsOverride_Left))
					{
						if (handEffectsOverrideCosmetic4.handEffectType == this.MapEnum(effectType))
						{
							handEffectsOverrideCosmetic = handEffectsOverrideCosmetic4;
							break;
						}
					}
					if (handEffectsOverrideCosmetic && handEffectsOverrideCosmetic.handEffectType == this.MapEnum(effectType) && ((!handEffectsOverrideCosmetic.isLeftHand && other.RightHand) || (handEffectsOverrideCosmetic.isLeftHand && !other.RightHand)))
					{
						if (handEffectsOverrideCosmetic.thirdPerson.playHaptics)
						{
							GorillaTagger.Instance.StartVibration(!this.rightHand, handEffectsOverrideCosmetic.thirdPerson.hapticStrength, handEffectsOverrideCosmetic.thirdPerson.hapticDuration);
						}
						TagEffectsLibrary.placeEffects(handEffectsOverrideCosmetic.thirdPerson.effectVFX, base.transform, this.rig.scaleFactor, false, handEffectsOverrideCosmetic.thirdPerson.parentEffect, base.transform.rotation);
						flag = true;
					}
				}
				if (handEffectsOverrideCosmetic2 && handEffectsOverrideCosmetic2.handEffectType == this.MapEnum(effectType) && ((handEffectsOverrideCosmetic2.isLeftHand && !this.rightHand) || (!handEffectsOverrideCosmetic2.isLeftHand && this.rightHand)))
				{
					if (handEffectsOverrideCosmetic2.firstPerson.playHaptics)
					{
						GorillaTagger.Instance.StartVibration(!this.rightHand, handEffectsOverrideCosmetic2.firstPerson.hapticStrength, handEffectsOverrideCosmetic2.firstPerson.hapticDuration);
					}
					TagEffectsLibrary.placeEffects(handEffectsOverrideCosmetic2.firstPerson.effectVFX, other.Transform, this.rig.scaleFactor, false, handEffectsOverrideCosmetic2.firstPerson.parentEffect, other.Transform.rotation);
					flag = true;
				}
			}
			if (!flag)
			{
				if (this.rig.isOfflineVRRig)
				{
					GorillaTagger.Instance.StartVibration(!this.rightHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
				}
				TagEffectsLibrary.PlayEffect(base.transform, !this.rightHand, this.rig.scaleFactor, effectType, this.CosmeticEffectPack, other.CosmeticEffectPack, base.transform.rotation);
			}
		}

		// Token: 0x06007024 RID: 28708 RVA: 0x00243058 File Offset: 0x00241258
		public bool InTriggerZone(IHandEffectsTrigger t)
		{
			return (base.transform.position - t.Transform.position).IsShorterThan(this.triggerRadius * this.rig.scaleFactor);
		}

		// Token: 0x06007025 RID: 28709 RVA: 0x0024308C File Offset: 0x0024128C
		private HandEffectsOverrideCosmetic.HandEffectType MapEnum(TagEffectsLibrary.EffectType oldEnum)
		{
			return HandEffectsTrigger.mappingArray[(int)oldEnum];
		}

		// Token: 0x04008019 RID: 32793
		[SerializeField]
		private float triggerRadius = 0.07f;

		// Token: 0x0400801A RID: 32794
		[SerializeField]
		private bool rightHand;

		// Token: 0x0400801B RID: 32795
		[SerializeField]
		private bool isStatic;

		// Token: 0x0400801C RID: 32796
		private VRRig rig;

		// Token: 0x0400801D RID: 32797
		public GorillaVelocityEstimator velocityEstimator;

		// Token: 0x0400801E RID: 32798
		[SerializeField]
		private GameObject[] debugVisuals;

		// Token: 0x04008021 RID: 32801
		private static HandEffectsOverrideCosmetic.HandEffectType[] mappingArray = new HandEffectsOverrideCosmetic.HandEffectType[]
		{
			HandEffectsOverrideCosmetic.HandEffectType.None,
			HandEffectsOverrideCosmetic.HandEffectType.None,
			HandEffectsOverrideCosmetic.HandEffectType.HighFive,
			HandEffectsOverrideCosmetic.HandEffectType.FistBump
		};
	}
}
