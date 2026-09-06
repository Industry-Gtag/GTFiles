using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012E3 RID: 4835
	public class CosmeticEffectsOnPlayers : MonoBehaviour, ISpawnable
	{
		// Token: 0x06007915 RID: 30997 RVA: 0x0027776C File Offset: 0x0027596C
		private bool ShouldAffectRig(VRRig rig, CosmeticEffectsOnPlayers.TargetType target)
		{
			bool flag = rig == this.myRig;
			bool flag2;
			switch (target)
			{
			case CosmeticEffectsOnPlayers.TargetType.Owner:
				flag2 = flag;
				break;
			case CosmeticEffectsOnPlayers.TargetType.Others:
				flag2 = !flag;
				break;
			case CosmeticEffectsOnPlayers.TargetType.All:
				flag2 = true;
				break;
			default:
				flag2 = false;
				break;
			}
			return flag2;
		}

		// Token: 0x06007916 RID: 30998 RVA: 0x002777AC File Offset: 0x002759AC
		private void Awake()
		{
			foreach (CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect in this.allEffects)
			{
				this.allEffectsDict.TryAdd(cosmeticEffect.effectType, cosmeticEffect);
			}
		}

		// Token: 0x06007917 RID: 30999 RVA: 0x002777E8 File Offset: 0x002759E8
		public void SetKnockbackStrengthMultiplier(float value)
		{
			foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> keyValuePair in this.allEffectsDict)
			{
				keyValuePair.Value.knockbackStrengthMultiplier = value;
			}
		}

		// Token: 0x06007918 RID: 31000 RVA: 0x00277844 File Offset: 0x00275A44
		public void ApplyAllEffects()
		{
			this.ApplyAllEffectsByDistance(base.transform.position);
		}

		// Token: 0x06007919 RID: 31001 RVA: 0x00277857 File Offset: 0x00275A57
		public void ApplyAllEffectsByDistance(Transform _transform)
		{
			this.ApplyAllEffectsByDistance(_transform.position);
		}

		// Token: 0x0600791A RID: 31002 RVA: 0x00277868 File Offset: 0x00275A68
		public void ApplyAllEffectsByDistance(Vector3 position)
		{
			foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> keyValuePair in this.allEffectsDict)
			{
				switch (keyValuePair.Key)
				{
				case CosmeticEffectsOnPlayers.EFFECTTYPE.Skin:
					this.ApplySkinByDistance(keyValuePair, position);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback:
					this.ApplyTagWithKnockbackByDistance(keyValuePair, position);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback:
					this.ApplyInstantKnockbackByDistance(keyValuePair, position);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.SFX:
					this.PlaySfxByDistance(keyValuePair, position);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.VFX:
					this.PlayVFXByDistance(keyValuePair, position);
					break;
				}
			}
		}

		// Token: 0x0600791B RID: 31003 RVA: 0x00277910 File Offset: 0x00275B10
		public void ApplyAllEffectsForRig(VRRig rig)
		{
			foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> keyValuePair in this.allEffectsDict)
			{
				switch (keyValuePair.Key)
				{
				case CosmeticEffectsOnPlayers.EFFECTTYPE.Skin:
					this.ApplySkinForRig(keyValuePair, rig);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback:
					this.ApplyTagWithKnockbackForRig(keyValuePair, rig);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback:
					this.ApplyInstantKnockbackForRig(keyValuePair, rig);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.VoiceOverride:
					this.ApplyVOForRig(keyValuePair, rig);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.SFX:
					this.PlaySfxForRig(keyValuePair, rig);
					break;
				case CosmeticEffectsOnPlayers.EFFECTTYPE.VFX:
					this.PlayVFXForRig(keyValuePair, rig);
					break;
				}
			}
		}

		// Token: 0x0600791C RID: 31004 RVA: 0x002779C4 File Offset: 0x00275BC4
		private void ApplySkinByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!effect.Value.IsGameModeAllowed())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			IEnumerable<VRRig> enumerable;
			if (!PhotonNetwork.InRoom)
			{
				IReadOnlyList<VRRig> readOnlyList = new VRRig[] { GorillaTagger.Instance.offlineVRRig };
				enumerable = readOnlyList;
			}
			else
			{
				enumerable = VRRigCache.ActiveRigs;
			}
			foreach (VRRig vrrig in enumerable)
			{
				if (this.ShouldAffectRig(vrrig, effect.Value.target) && (vrrig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
				{
					if (vrrig == this.myRig)
					{
						effect.Value.EffectDuration = effect.Value.effectDurationOwner;
					}
					vrrig.SpawnSkinEffects(effect);
				}
			}
		}

		// Token: 0x0600791D RID: 31005 RVA: 0x00277AB0 File Offset: 0x00275CB0
		private void ApplySkinForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig vrRig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (vrRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			vrRig.SpawnSkinEffects(effect);
		}

		// Token: 0x0600791E RID: 31006 RVA: 0x00277B20 File Offset: 0x00275D20
		private void ApplyTagWithKnockbackForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig vrRig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (vrRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			vrRig.EnableHitWithKnockBack(effect);
		}

		// Token: 0x0600791F RID: 31007 RVA: 0x00277B90 File Offset: 0x00275D90
		private void ApplyTagWithKnockbackByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!effect.Value.IsGameModeAllowed())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			IEnumerable<VRRig> enumerable;
			if (!PhotonNetwork.InRoom)
			{
				IReadOnlyList<VRRig> readOnlyList = new VRRig[] { GorillaTagger.Instance.offlineVRRig };
				enumerable = readOnlyList;
			}
			else
			{
				enumerable = VRRigCache.ActiveRigs;
			}
			foreach (VRRig vrrig in enumerable)
			{
				if (this.ShouldAffectRig(vrrig, effect.Value.target) && (vrrig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
				{
					if (vrrig == this.myRig)
					{
						effect.Value.EffectDuration = effect.Value.effectDurationOwner;
					}
					vrrig.EnableHitWithKnockBack(effect);
				}
			}
		}

		// Token: 0x06007920 RID: 31008 RVA: 0x00277C7C File Offset: 0x00275E7C
		private void ApplyInstantKnockbackForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig vrRig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (vrRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			Vector3 vector = vrRig.transform.position - base.transform.position;
			float num = (1f / vector.magnitude * effect.Value.knockbackStrength * effect.Value.knockbackStrengthMultiplier).ClampSafe(effect.Value.minKnockbackStrength, effect.Value.maxKnockbackStrength);
			if (effect.Value.applyScaleToKnockbackStrength)
			{
				num *= vrRig.scaleFactor;
			}
			RoomSystem.HitPlayer(vrRig.creator, vector.normalized, num);
			vrRig.ApplyInstanceKnockBack(effect);
		}

		// Token: 0x06007921 RID: 31009 RVA: 0x00277D78 File Offset: 0x00275F78
		private void ApplyInstantKnockbackByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(GorillaTagger.Instance.offlineVRRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (GorillaTagger.Instance.offlineVRRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			Vector3 vector = GorillaTagger.Instance.offlineVRRig.transform.position - position;
			if (vector.IsShorterThan(effect.Value.effectDistanceRadius))
			{
				float magnitude = vector.magnitude;
				GTPlayer instance = GTPlayer.Instance;
				if (effect.Value.specialVerticalForce && (instance.IsHandTouching(true) || instance.IsHandTouching(false) || instance.BodyOnGround))
				{
					Vector3 vector2 = -Physics.gravity.normalized;
					Vector3 vector3 = Vector3.ProjectOnPlane(vector, vector2);
					vector = ((Vector3.Dot(vector / magnitude, vector2) > 0f) ? vector : vector3) + vector3.magnitude * vector2;
				}
				float num = (effect.Value.knockbackStrength * effect.Value.knockbackStrengthMultiplier / magnitude).ClampSafe(effect.Value.minKnockbackStrength, effect.Value.maxKnockbackStrength);
				if (effect.Value.applyScaleToKnockbackStrength)
				{
					num *= instance.scale;
				}
				instance.ApplyKnockback(vector.normalized, num, effect.Value.forceOffTheGround);
				GorillaTagger.Instance.offlineVRRig.ApplyInstanceKnockBack(effect);
			}
		}

		// Token: 0x06007922 RID: 31010 RVA: 0x00277F24 File Offset: 0x00276124
		private void ApplyVOForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig rig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(rig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (rig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			rig.ActivateVOEffect(effect);
		}

		// Token: 0x06007923 RID: 31011 RVA: 0x00277F94 File Offset: 0x00276194
		private void PlaySfxForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig vrRig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (vrRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			vrRig.PlayCosmeticEffectSFX(effect);
		}

		// Token: 0x06007924 RID: 31012 RVA: 0x00278004 File Offset: 0x00276204
		private void PlaySfxByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!effect.Value.IsGameModeAllowed())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			IEnumerable<VRRig> enumerable;
			if (!PhotonNetwork.InRoom)
			{
				IReadOnlyList<VRRig> readOnlyList = new VRRig[] { GorillaTagger.Instance.offlineVRRig };
				enumerable = readOnlyList;
			}
			else
			{
				enumerable = VRRigCache.ActiveRigs;
			}
			foreach (VRRig vrrig in enumerable)
			{
				if (this.ShouldAffectRig(vrrig, effect.Value.target) && (vrrig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
				{
					if (vrrig == this.myRig)
					{
						effect.Value.EffectDuration = effect.Value.effectDurationOwner;
					}
					vrrig.PlayCosmeticEffectSFX(effect);
				}
			}
		}

		// Token: 0x06007925 RID: 31013 RVA: 0x002780F0 File Offset: 0x002762F0
		private void PlayVFXForRig(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, VRRig vrRig)
		{
			if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			if (vrRig == this.myRig)
			{
				effect.Value.EffectDuration = effect.Value.effectDurationOwner;
			}
			vrRig.SpawnVFXEffect(effect);
		}

		// Token: 0x06007926 RID: 31014 RVA: 0x00278160 File Offset: 0x00276360
		private void PlayVFXByDistance(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect, Vector3 position)
		{
			if (!effect.Value.IsGameModeAllowed())
			{
				return;
			}
			effect.Value.EffectStartedTime = Time.time;
			IEnumerable<VRRig> enumerable;
			if (!PhotonNetwork.InRoom)
			{
				IReadOnlyList<VRRig> readOnlyList = new VRRig[] { GorillaTagger.Instance.offlineVRRig };
				enumerable = readOnlyList;
			}
			else
			{
				enumerable = VRRigCache.ActiveRigs;
			}
			foreach (VRRig vrrig in enumerable)
			{
				if (this.ShouldAffectRig(vrrig, effect.Value.target) && (vrrig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
				{
					if (vrrig == this.myRig)
					{
						effect.Value.EffectDuration = effect.Value.effectDurationOwner;
					}
					vrrig.SpawnVFXEffect(effect);
				}
			}
		}

		// Token: 0x17000BC2 RID: 3010
		// (get) Token: 0x06007927 RID: 31015 RVA: 0x0027824C File Offset: 0x0027644C
		// (set) Token: 0x06007928 RID: 31016 RVA: 0x00278254 File Offset: 0x00276454
		public bool IsSpawned { get; set; }

		// Token: 0x17000BC3 RID: 3011
		// (get) Token: 0x06007929 RID: 31017 RVA: 0x0027825D File Offset: 0x0027645D
		// (set) Token: 0x0600792A RID: 31018 RVA: 0x00278265 File Offset: 0x00276465
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x0600792B RID: 31019 RVA: 0x0027826E File Offset: 0x0027646E
		public void OnSpawn(VRRig rig)
		{
			this.myRig = rig;
		}

		// Token: 0x0600792C RID: 31020 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnDespawn()
		{
		}

		// Token: 0x04008A21 RID: 35361
		public CosmeticEffectsOnPlayers.CosmeticEffect[] allEffects = new CosmeticEffectsOnPlayers.CosmeticEffect[0];

		// Token: 0x04008A22 RID: 35362
		private VRRig myRig;

		// Token: 0x04008A23 RID: 35363
		private Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> allEffectsDict = new Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect>();

		// Token: 0x020012E4 RID: 4836
		[Serializable]
		public enum TargetType
		{
			// Token: 0x04008A27 RID: 35367
			Owner,
			// Token: 0x04008A28 RID: 35368
			Others,
			// Token: 0x04008A29 RID: 35369
			All
		}

		// Token: 0x020012E5 RID: 4837
		[Serializable]
		public class CosmeticEffect
		{
			// Token: 0x17000BC4 RID: 3012
			// (get) Token: 0x0600792E RID: 31022 RVA: 0x00278296 File Offset: 0x00276496
			// (set) Token: 0x0600792F RID: 31023 RVA: 0x0027829E File Offset: 0x0027649E
			public float knockbackStrengthMultiplier { get; set; }

			// Token: 0x06007930 RID: 31024 RVA: 0x002782A8 File Offset: 0x002764A8
			public bool IsGameModeAllowed()
			{
				GameModeType gameModeType = ((GameMode.ActiveGameMode != null) ? GameMode.ActiveGameMode.GameType() : GameModeType.Casual);
				return !this.excludeForGameModes.Contains(gameModeType);
			}

			// Token: 0x17000BC5 RID: 3013
			// (get) Token: 0x06007931 RID: 31025 RVA: 0x002782E1 File Offset: 0x002764E1
			// (set) Token: 0x06007932 RID: 31026 RVA: 0x002782E9 File Offset: 0x002764E9
			public float EffectDuration
			{
				get
				{
					return this.effectDurationOthers;
				}
				set
				{
					this.effectDurationOthers = value;
				}
			}

			// Token: 0x17000BC6 RID: 3014
			// (get) Token: 0x06007933 RID: 31027 RVA: 0x002782F2 File Offset: 0x002764F2
			// (set) Token: 0x06007934 RID: 31028 RVA: 0x002782FA File Offset: 0x002764FA
			public float EffectStartedTime { get; set; }

			// Token: 0x06007935 RID: 31029 RVA: 0x00278303 File Offset: 0x00276503
			private bool IsSkin()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.Skin;
			}

			// Token: 0x06007936 RID: 31030 RVA: 0x0027830E File Offset: 0x0027650E
			private bool IsTagKnockback()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback;
			}

			// Token: 0x06007937 RID: 31031 RVA: 0x00278319 File Offset: 0x00276519
			private bool IsInstantKnockback()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback;
			}

			// Token: 0x06007938 RID: 31032 RVA: 0x00278324 File Offset: 0x00276524
			private bool HasKnockback()
			{
				CosmeticEffectsOnPlayers.EFFECTTYPE effecttype = this.effectType;
				return effecttype == CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback || effecttype == CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback;
			}

			// Token: 0x06007939 RID: 31033 RVA: 0x00278349 File Offset: 0x00276549
			private bool IsVO()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.VoiceOverride;
			}

			// Token: 0x0600793A RID: 31034 RVA: 0x00278354 File Offset: 0x00276554
			private bool IsSFX()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.SFX;
			}

			// Token: 0x0600793B RID: 31035 RVA: 0x0027835F File Offset: 0x0027655F
			private bool IsVFX()
			{
				return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.VFX;
			}

			// Token: 0x17000BC7 RID: 3015
			// (get) Token: 0x0600793C RID: 31036 RVA: 0x0027836A File Offset: 0x0027656A
			private HashSet<GameModeType> Modes
			{
				get
				{
					if (this.modesHash == null)
					{
						this.modesHash = new HashSet<GameModeType>(this.excludeForGameModes);
					}
					return this.modesHash;
				}
			}

			// Token: 0x04008A2A RID: 35370
			public GameModeType[] excludeForGameModes;

			// Token: 0x04008A2B RID: 35371
			public CosmeticEffectsOnPlayers.EFFECTTYPE effectType;

			// Token: 0x04008A2C RID: 35372
			public float effectDistanceRadius;

			// Token: 0x04008A2D RID: 35373
			public CosmeticEffectsOnPlayers.TargetType target = CosmeticEffectsOnPlayers.TargetType.All;

			// Token: 0x04008A2E RID: 35374
			public float effectDurationOthers;

			// Token: 0x04008A2F RID: 35375
			public float effectDurationOwner;

			// Token: 0x04008A30 RID: 35376
			public GorillaSkin newSkin;

			// Token: 0x04008A31 RID: 35377
			[Tooltip("Use object pools")]
			public GameObject knockbackVFX;

			// Token: 0x04008A32 RID: 35378
			[FormerlySerializedAs("knockbackStrengthMultiplier")]
			public float knockbackStrength;

			// Token: 0x04008A33 RID: 35379
			public bool applyScaleToKnockbackStrength;

			// Token: 0x04008A34 RID: 35380
			[Tooltip("force pushing players with hands on the ground")]
			public bool forceOffTheGround;

			// Token: 0x04008A35 RID: 35381
			[Tooltip("Take the horizontal magnitude of the knockback, and add it opposite gravity. For example, being hit sideways will also impart a large upwards force. Breaks conservation of energy, but feels better to the player.")]
			public bool specialVerticalForce;

			// Token: 0x04008A36 RID: 35382
			[FormerlySerializedAs("minStrengthClamp")]
			public float minKnockbackStrength = 0.5f;

			// Token: 0x04008A37 RID: 35383
			[FormerlySerializedAs("maxStrengthClamp")]
			public float maxKnockbackStrength = 6f;

			// Token: 0x04008A39 RID: 35385
			public AudioClip[] voiceOverrideNormalClips;

			// Token: 0x04008A3A RID: 35386
			public AudioClip[] voiceOverrideLoudClips;

			// Token: 0x04008A3B RID: 35387
			public float voiceOverrideNormalVolume = 0.5f;

			// Token: 0x04008A3C RID: 35388
			public float voiceOverrideLoudVolume = 0.8f;

			// Token: 0x04008A3D RID: 35389
			public float voiceOverrideLoudThreshold = 0.175f;

			// Token: 0x04008A3E RID: 35390
			[Tooltip("plays sfx on player")]
			public List<AudioClip> sfxAudioClip;

			// Token: 0x04008A3F RID: 35391
			[Tooltip("plays vfx on player, must be in the global object pool and have a tag.")]
			public GameObject VFXGameObject;

			// Token: 0x04008A40 RID: 35392
			private HashSet<GameModeType> modesHash;
		}

		// Token: 0x020012E6 RID: 4838
		public enum EFFECTTYPE
		{
			// Token: 0x04008A43 RID: 35395
			Skin,
			// Token: 0x04008A44 RID: 35396
			[Obsolete("FPV has been removed, do not use, use Stick Object To Player instead")]
			TagWithKnockback = 2,
			// Token: 0x04008A45 RID: 35397
			InstantKnockback,
			// Token: 0x04008A46 RID: 35398
			VoiceOverride,
			// Token: 0x04008A47 RID: 35399
			SFX,
			// Token: 0x04008A48 RID: 35400
			VFX
		}
	}
}
