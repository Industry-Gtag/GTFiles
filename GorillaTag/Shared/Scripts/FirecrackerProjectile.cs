using System;
using System.Collections;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Shared.Scripts
{
	// Token: 0x02001291 RID: 4753
	public class FirecrackerProjectile : MonoBehaviour, ITickSystemTick, IProjectile
	{
		// Token: 0x17000BA9 RID: 2985
		// (get) Token: 0x060077A3 RID: 30627 RVA: 0x0026C4DB File Offset: 0x0026A6DB
		// (set) Token: 0x060077A4 RID: 30628 RVA: 0x0026C4E3 File Offset: 0x0026A6E3
		public bool TickRunning { get; set; }

		// Token: 0x060077A5 RID: 30629 RVA: 0x0026C4EC File Offset: 0x0026A6EC
		public void Tick()
		{
			if (Time.time - this.timeCreated > this.forceBackToPoolAfterSec || Time.time - this.timeExploded > this.explosionTime)
			{
				UnityEvent<FirecrackerProjectile> onDetonationComplete = this.OnDetonationComplete;
				if (onDetonationComplete == null)
				{
					return;
				}
				onDetonationComplete.Invoke(this);
			}
		}

		// Token: 0x060077A6 RID: 30630 RVA: 0x0026C528 File Offset: 0x0026A728
		private void OnEnable()
		{
			TickSystem<object>.AddCallbackTarget(this);
			this.m_timer.Start();
			this.timeExploded = float.PositiveInfinity;
			this.timeCreated = float.PositiveInfinity;
			this.collisionEntered = false;
			if (this.disableWhenHit)
			{
				this.disableWhenHit.SetActive(true);
			}
			UnityEvent onEnableObject = this.OnEnableObject;
			if (onEnableObject == null)
			{
				return;
			}
			onEnableObject.Invoke();
		}

		// Token: 0x060077A7 RID: 30631 RVA: 0x0026C58C File Offset: 0x0026A78C
		private void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
			this.m_timer.Stop();
			if (this.useTransferrableObjectState)
			{
				UnityEvent onResetProjectileState = this.OnResetProjectileState;
				if (onResetProjectileState == null)
				{
					return;
				}
				onResetProjectileState.Invoke();
			}
		}

		// Token: 0x060077A8 RID: 30632 RVA: 0x0026C5B7 File Offset: 0x0026A7B7
		private void Awake()
		{
			this.rb = base.GetComponent<Rigidbody>();
			this.audioSource = base.GetComponent<AudioSource>();
			this.m_timer.callback = new Action(this.Detonate);
		}

		// Token: 0x060077A9 RID: 30633 RVA: 0x0026C5E8 File Offset: 0x0026A7E8
		private void Detonate()
		{
			this.m_timer.Stop();
			this.timeExploded = Time.time;
			if (this.disableWhenHit)
			{
				this.disableWhenHit.SetActive(false);
			}
			this.collisionEntered = false;
		}

		// Token: 0x060077AA RID: 30634 RVA: 0x0026C620 File Offset: 0x0026A820
		internal void SetTransferrableState(TransferrableObject.SyncOptions syncType, int state)
		{
			if (!this.useTransferrableObjectState)
			{
				return;
			}
			if (syncType != TransferrableObject.SyncOptions.Bool)
			{
				if (syncType != TransferrableObject.SyncOptions.Int)
				{
					return;
				}
				UnityEvent<int> onItemStateIntChanged = this.OnItemStateIntChanged;
				if (onItemStateIntChanged == null)
				{
					return;
				}
				onItemStateIntChanged.Invoke(state);
				return;
			}
			else
			{
				bool flag = (state & 1) != 0;
				bool flag2 = (state & 2) != 0;
				bool flag3 = (state & 4) != 0;
				bool flag4 = (state & 8) != 0;
				if (flag)
				{
					UnityEvent onItemStateBoolATrue = this.OnItemStateBoolATrue;
					if (onItemStateBoolATrue != null)
					{
						onItemStateBoolATrue.Invoke();
					}
				}
				else
				{
					UnityEvent onItemStateBoolAFalse = this.OnItemStateBoolAFalse;
					if (onItemStateBoolAFalse != null)
					{
						onItemStateBoolAFalse.Invoke();
					}
				}
				if (flag2)
				{
					UnityEvent onItemStateBoolBTrue = this.OnItemStateBoolBTrue;
					if (onItemStateBoolBTrue != null)
					{
						onItemStateBoolBTrue.Invoke();
					}
				}
				else
				{
					UnityEvent onItemStateBoolBFalse = this.OnItemStateBoolBFalse;
					if (onItemStateBoolBFalse != null)
					{
						onItemStateBoolBFalse.Invoke();
					}
				}
				if (flag3)
				{
					UnityEvent onItemStateBoolCTrue = this.OnItemStateBoolCTrue;
					if (onItemStateBoolCTrue != null)
					{
						onItemStateBoolCTrue.Invoke();
					}
				}
				else
				{
					UnityEvent onItemStateBoolCFalse = this.OnItemStateBoolCFalse;
					if (onItemStateBoolCFalse != null)
					{
						onItemStateBoolCFalse.Invoke();
					}
				}
				if (flag4)
				{
					UnityEvent onItemStateBoolDTrue = this.OnItemStateBoolDTrue;
					if (onItemStateBoolDTrue == null)
					{
						return;
					}
					onItemStateBoolDTrue.Invoke();
					return;
				}
				else
				{
					UnityEvent onItemStateBoolDFalse = this.OnItemStateBoolDFalse;
					if (onItemStateBoolDFalse == null)
					{
						return;
					}
					onItemStateBoolDFalse.Invoke();
					return;
				}
			}
		}

		// Token: 0x060077AB RID: 30635 RVA: 0x0026C708 File Offset: 0x0026A908
		public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progress)
		{
			base.transform.position = startPosition;
			base.transform.rotation = startRotation;
			base.transform.localScale = Vector3.one * ownerRig.scaleFactor;
			this.rb.linearVelocity = velocity;
		}

		// Token: 0x060077AC RID: 30636 RVA: 0x0026C758 File Offset: 0x0026A958
		private void OnCollisionEnter(Collision other)
		{
			if (this.collisionEntered)
			{
				return;
			}
			Vector3 point = other.contacts[0].point;
			Vector3 normal = other.contacts[0].normal;
			UnityEvent<FirecrackerProjectile, Vector3> onCollisionEntered = this.OnCollisionEntered;
			if (onCollisionEntered != null)
			{
				onCollisionEntered.Invoke(this, normal);
			}
			if (this.sizzleDuration > 0f)
			{
				base.StartCoroutine(this.Sizzle(point, normal));
			}
			else
			{
				UnityEvent<FirecrackerProjectile, Vector3> onDetonationStart = this.OnDetonationStart;
				if (onDetonationStart != null)
				{
					onDetonationStart.Invoke(this, point);
				}
				this.Detonate(point, normal);
			}
			this.collisionEntered = true;
		}

		// Token: 0x060077AD RID: 30637 RVA: 0x0026C7E5 File Offset: 0x0026A9E5
		private IEnumerator Sizzle(Vector3 contactPoint, Vector3 normal)
		{
			if (this.audioSource && this.sizzleAudioClip != null)
			{
				this.audioSource.GTPlayOneShot(this.sizzleAudioClip, 1f);
			}
			yield return new WaitForSeconds(this.sizzleDuration);
			UnityEvent<FirecrackerProjectile, Vector3> onDetonationStart = this.OnDetonationStart;
			if (onDetonationStart != null)
			{
				onDetonationStart.Invoke(this, contactPoint);
			}
			this.Detonate(contactPoint, normal);
			yield break;
		}

		// Token: 0x060077AE RID: 30638 RVA: 0x0026C804 File Offset: 0x0026AA04
		private void Detonate(Vector3 contactPoint, Vector3 normal)
		{
			this.timeExploded = Time.time;
			GameObject gameObject = ObjectPools.instance.Instantiate(this.explosionEffect, contactPoint, true);
			gameObject.transform.up = normal;
			gameObject.transform.position = base.transform.position;
			SoundBankPlayer soundBankPlayer;
			if (gameObject.TryGetComponent<SoundBankPlayer>(out soundBankPlayer) && soundBankPlayer.soundBank)
			{
				soundBankPlayer.Play();
			}
			if (this.disableWhenHit)
			{
				this.disableWhenHit.SetActive(false);
			}
			this.collisionEntered = false;
		}

		// Token: 0x040087DA RID: 34778
		[SerializeField]
		private GameObject explosionEffect;

		// Token: 0x040087DB RID: 34779
		[SerializeField]
		private float forceBackToPoolAfterSec = 20f;

		// Token: 0x040087DC RID: 34780
		[SerializeField]
		private float explosionTime = 5f;

		// Token: 0x040087DD RID: 34781
		[SerializeField]
		private GameObject disableWhenHit;

		// Token: 0x040087DE RID: 34782
		[SerializeField]
		private float sizzleDuration;

		// Token: 0x040087DF RID: 34783
		[SerializeField]
		private AudioClip sizzleAudioClip;

		// Token: 0x040087E0 RID: 34784
		[Space]
		public UnityEvent OnEnableObject;

		// Token: 0x040087E1 RID: 34785
		public UnityEvent<FirecrackerProjectile, Vector3> OnCollisionEntered;

		// Token: 0x040087E2 RID: 34786
		public UnityEvent<FirecrackerProjectile, Vector3> OnDetonationStart;

		// Token: 0x040087E3 RID: 34787
		public UnityEvent<FirecrackerProjectile> OnDetonationComplete;

		// Token: 0x040087E4 RID: 34788
		private Rigidbody rb;

		// Token: 0x040087E5 RID: 34789
		private float timeCreated = float.PositiveInfinity;

		// Token: 0x040087E6 RID: 34790
		private float timeExploded = float.PositiveInfinity;

		// Token: 0x040087E7 RID: 34791
		private AudioSource audioSource;

		// Token: 0x040087E8 RID: 34792
		private TickSystemTimer m_timer = new TickSystemTimer(40f);

		// Token: 0x040087E9 RID: 34793
		private bool collisionEntered;

		// Token: 0x040087EA RID: 34794
		[SerializeField]
		private bool useTransferrableObjectState;

		// Token: 0x040087EB RID: 34795
		[SerializeField]
		protected UnityEvent OnResetProjectileState;

		// Token: 0x040087EC RID: 34796
		[SerializeField]
		protected string boolADebugName;

		// Token: 0x040087ED RID: 34797
		[SerializeField]
		protected UnityEvent OnItemStateBoolATrue;

		// Token: 0x040087EE RID: 34798
		[SerializeField]
		protected UnityEvent OnItemStateBoolAFalse;

		// Token: 0x040087EF RID: 34799
		[SerializeField]
		protected string boolBDebugName;

		// Token: 0x040087F0 RID: 34800
		[SerializeField]
		protected UnityEvent OnItemStateBoolBTrue;

		// Token: 0x040087F1 RID: 34801
		[SerializeField]
		protected UnityEvent OnItemStateBoolBFalse;

		// Token: 0x040087F2 RID: 34802
		[SerializeField]
		protected string boolCDebugName;

		// Token: 0x040087F3 RID: 34803
		[SerializeField]
		protected UnityEvent OnItemStateBoolCTrue;

		// Token: 0x040087F4 RID: 34804
		[SerializeField]
		protected UnityEvent OnItemStateBoolCFalse;

		// Token: 0x040087F5 RID: 34805
		[SerializeField]
		protected string boolDDebugName;

		// Token: 0x040087F6 RID: 34806
		[SerializeField]
		protected UnityEvent OnItemStateBoolDTrue;

		// Token: 0x040087F7 RID: 34807
		[SerializeField]
		protected UnityEvent OnItemStateBoolDFalse;

		// Token: 0x040087F8 RID: 34808
		[SerializeField]
		protected UnityEvent<int> OnItemStateIntChanged;
	}
}
