using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Reactions
{
	// Token: 0x0200124E RID: 4686
	public class FireInstance : MonoBehaviour
	{
		// Token: 0x060076BB RID: 30395 RVA: 0x00267B44 File Offset: 0x00265D44
		protected void Awake()
		{
			FireManager.Register(this);
		}

		// Token: 0x060076BC RID: 30396 RVA: 0x00267B4C File Offset: 0x00265D4C
		protected void OnDestroy()
		{
			FireManager.Unregister(this);
		}

		// Token: 0x060076BD RID: 30397 RVA: 0x00267B54 File Offset: 0x00265D54
		protected void OnEnable()
		{
			FireManager.OnEnable(this);
		}

		// Token: 0x060076BE RID: 30398 RVA: 0x00267B5C File Offset: 0x00265D5C
		protected void OnDisable()
		{
			FireManager.OnDisable(this);
		}

		// Token: 0x060076BF RID: 30399 RVA: 0x00267B64 File Offset: 0x00265D64
		protected void OnTriggerEnter(Collider other)
		{
			FireManager.OnTriggerEnter(this, other);
		}

		// Token: 0x0400862B RID: 34347
		[Header("Scene References")]
		[Tooltip("If not assigned it will try to auto assign to a component on the same GameObject.")]
		[SerializeField]
		internal Collider _collider;

		// Token: 0x0400862C RID: 34348
		[Tooltip("If not assigned it will try to auto assign to a component on the same GameObject.")]
		[FormerlySerializedAs("_thermalSourceVolume")]
		[SerializeField]
		internal ThermalSourceVolume _thermalVolume;

		// Token: 0x0400862D RID: 34349
		[SerializeField]
		internal ParticleSystem _particleSystem;

		// Token: 0x0400862E RID: 34350
		[FormerlySerializedAs("_audioSource")]
		[SerializeField]
		internal AudioSource _loopingAudioSource;

		// Token: 0x0400862F RID: 34351
		[Tooltip("The emissive color will be darkened on the materials of these renderers as the fire is extinguished.")]
		[SerializeField]
		internal Renderer[] _emissiveRenderers;

		// Token: 0x04008630 RID: 34352
		[Header("Asset References")]
		[SerializeField]
		internal GTDirectAssetRef<AudioClip> _extinguishSound;

		// Token: 0x04008631 RID: 34353
		[SerializeField]
		internal float _extinguishSoundVolume = 1f;

		// Token: 0x04008632 RID: 34354
		[SerializeField]
		internal GTDirectAssetRef<AudioClip> _igniteSound;

		// Token: 0x04008633 RID: 34355
		[SerializeField]
		internal float _igniteSoundVolume = 1f;

		// Token: 0x04008634 RID: 34356
		[Header("Values")]
		[SerializeField]
		internal bool _despawnOnExtinguish = true;

		// Token: 0x04008635 RID: 34357
		[SerializeField]
		internal float _maxLifetime = 10f;

		// Token: 0x04008636 RID: 34358
		[Tooltip("How long it should take to reheat to it's default temperature.")]
		[SerializeField]
		internal float _reheatSpeed = 1f;

		// Token: 0x04008637 RID: 34359
		[Tooltip("If you completely extinguish the object, how long should it stay extinguished?")]
		[SerializeField]
		internal float _stayExtinguishedDuration = 1f;

		// Token: 0x04008638 RID: 34360
		internal float _defaultTemperature;

		// Token: 0x04008639 RID: 34361
		internal float _timeSinceExtinguished;

		// Token: 0x0400863A RID: 34362
		internal float _timeSinceDyingStart;

		// Token: 0x0400863B RID: 34363
		internal float _timeAlive;

		// Token: 0x0400863C RID: 34364
		internal float _psDefaultEmissionRate;

		// Token: 0x0400863D RID: 34365
		internal ParticleSystem.EmissionModule _psEmissionModule;

		// Token: 0x0400863E RID: 34366
		internal Vector3Int _spatialGridPosition;

		// Token: 0x0400863F RID: 34367
		internal bool _isDespawning;

		// Token: 0x04008640 RID: 34368
		internal float _deathStateDuration;

		// Token: 0x04008641 RID: 34369
		internal MaterialPropertyBlock[] _emiRenderers_matPropBlocks;

		// Token: 0x04008642 RID: 34370
		internal Color[] _emiRenderers_defaultColors;
	}
}
