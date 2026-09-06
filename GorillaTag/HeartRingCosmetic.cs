using System;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x020011DF RID: 4575
	[DefaultExecutionOrder(1250)]
	public class HeartRingCosmetic : MonoBehaviour
	{
		// Token: 0x0600744E RID: 29774 RVA: 0x0025D421 File Offset: 0x0025B621
		protected void Awake()
		{
			Application.quitting += delegate
			{
				base.enabled = false;
			};
		}

		// Token: 0x0600744F RID: 29775 RVA: 0x0025D434 File Offset: 0x0025B634
		protected void OnEnable()
		{
			this.particleSystem = this.effects.GetComponentInChildren<ParticleSystem>(true);
			this.audioSource = this.effects.GetComponentInChildren<AudioSource>(true);
			this.ownerRig = base.GetComponentInParent<VRRig>();
			bool flag = this.ownerRig != null && this.ownerRig.head != null && this.ownerRig.head.rigTarget != null;
			base.enabled = flag;
			this.effects.SetActive(flag);
			if (!flag)
			{
				Debug.LogError("Disabling HeartRingCosmetic. Could not find owner head. Scene path: " + base.transform.GetPath(), this);
				return;
			}
			this.ownerHead = ((this.ownerRig != null) ? this.ownerRig.head.rigTarget.transform : base.transform);
			this.maxEmissionRate = this.particleSystem.emission.rateOverTime.constant;
			this.maxVolume = this.audioSource.volume;
		}

		// Token: 0x06007450 RID: 29776 RVA: 0x0025D53C File Offset: 0x0025B73C
		protected void LateUpdate()
		{
			Transform transform = base.transform;
			Vector3 position = transform.position;
			float x = transform.lossyScale.x;
			float num = this.effectActivationRadius * this.effectActivationRadius * x * x;
			bool flag = (this.ownerHead.TransformPoint(this.headToMouthOffset) - position).sqrMagnitude < num;
			ParticleSystem.EmissionModule emission = this.particleSystem.emission;
			emission.rateOverTime = Mathf.Lerp(emission.rateOverTime.constant, flag ? this.maxEmissionRate : 0f, Time.deltaTime / 0.1f);
			this.audioSource.volume = Mathf.Lerp(this.audioSource.volume, flag ? this.maxVolume : 0f, Time.deltaTime / 2f);
			this.ownerRig.UsingHauntedRing = this.isHauntedVoiceChanger && flag;
			if (this.ownerRig.UsingHauntedRing)
			{
				this.ownerRig.HauntedRingVoicePitch = this.hauntedVoicePitch;
			}
		}

		// Token: 0x0400841A RID: 33818
		public GameObject effects;

		// Token: 0x0400841B RID: 33819
		[SerializeField]
		private bool isHauntedVoiceChanger;

		// Token: 0x0400841C RID: 33820
		[SerializeField]
		private float hauntedVoicePitch = 0.75f;

		// Token: 0x0400841D RID: 33821
		[AssignInCorePrefab]
		public float effectActivationRadius = 0.15f;

		// Token: 0x0400841E RID: 33822
		private readonly Vector3 headToMouthOffset = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x0400841F RID: 33823
		private VRRig ownerRig;

		// Token: 0x04008420 RID: 33824
		private Transform ownerHead;

		// Token: 0x04008421 RID: 33825
		private ParticleSystem particleSystem;

		// Token: 0x04008422 RID: 33826
		private AudioSource audioSource;

		// Token: 0x04008423 RID: 33827
		private float maxEmissionRate;

		// Token: 0x04008424 RID: 33828
		private float maxVolume;

		// Token: 0x04008425 RID: 33829
		private const float emissionFadeTime = 0.1f;

		// Token: 0x04008426 RID: 33830
		private const float volumeFadeTime = 2f;
	}
}
