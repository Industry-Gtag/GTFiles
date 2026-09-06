using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012E8 RID: 4840
	public class MedusaEyeLantern : MonoBehaviour
	{
		// Token: 0x06007947 RID: 31047 RVA: 0x00278884 File Offset: 0x00276A84
		private void Awake()
		{
			foreach (MedusaEyeLantern.EyeState eyeState in this.allStates)
			{
				this.allStatesDict.Add(eyeState.eyeState, eyeState);
			}
		}

		// Token: 0x06007948 RID: 31048 RVA: 0x002788BC File Offset: 0x00276ABC
		private void OnDestroy()
		{
			this.allStatesDict.Clear();
		}

		// Token: 0x06007949 RID: 31049 RVA: 0x002788C9 File Offset: 0x00276AC9
		private void Start()
		{
			if (this.rotatingObjectTransform == null)
			{
				this.rotatingObjectTransform = base.transform;
			}
			this.initialRotation = this.rotatingObjectTransform.localRotation;
			this.SwitchState(MedusaEyeLantern.State.DORMANT);
		}

		// Token: 0x0600794A RID: 31050 RVA: 0x00278900 File Offset: 0x00276B00
		private void Update()
		{
			if (!this.transferableParent.InHand() && this.currentState != MedusaEyeLantern.State.DORMANT)
			{
				this.SwitchState(MedusaEyeLantern.State.DORMANT);
			}
			if (!this.transferableParent.InHand())
			{
				return;
			}
			this.UpdateState();
			if (this.velocityTracker == null || this.rotatingObjectTransform == null)
			{
				return;
			}
			Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true, 0.15f, false);
			Vector3 vector = new Vector3(averageVelocity.x, 0f, averageVelocity.z);
			float magnitude = vector.magnitude;
			Vector3 normalized = vector.normalized;
			float num = Mathf.Clamp(-normalized.z, -1f, 1f) * this.maxRotationAngle * (magnitude * this.rotationSpeedMultiplier);
			float num2 = Mathf.Clamp(normalized.x, -1f, 1f) * this.maxRotationAngle * (magnitude * this.rotationSpeedMultiplier);
			this.targetRotation = this.initialRotation * Quaternion.Euler(num, 0f, num2);
			if (magnitude > this.sloshVelocityThreshold)
			{
				this.SwitchState(MedusaEyeLantern.State.SLOSHING);
			}
			if ((double)magnitude < 0.01)
			{
				this.targetRotation = this.initialRotation;
			}
			if (!this.EyeIsLockedOn())
			{
				this.rotatingObjectTransform.localRotation = Quaternion.Slerp(this.rotatingObjectTransform.localRotation, this.targetRotation, Time.deltaTime * this.rotationSmoothing);
			}
		}

		// Token: 0x0600794B RID: 31051 RVA: 0x00278A5E File Offset: 0x00276C5E
		public void HandleOnNoOneInRange()
		{
			this.SwitchState(MedusaEyeLantern.State.RESET);
			this.resetTargetTime = Time.time;
			this.rotatingObjectTransform.localRotation = this.initialRotation;
		}

		// Token: 0x0600794C RID: 31052 RVA: 0x00278A83 File Offset: 0x00276C83
		public void HandleOnNewPlayerDetected(VRRig target, float distance)
		{
			this.targetRig = target;
			if (this.currentState != MedusaEyeLantern.State.SLOSHING)
			{
				this.SwitchState(MedusaEyeLantern.State.TRACKING);
			}
		}

		// Token: 0x0600794D RID: 31053 RVA: 0x00278A9C File Offset: 0x00276C9C
		private void Sloshing()
		{
			Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true, 0.15f, false);
			Vector3 vector = new Vector3(averageVelocity.x, 0f, averageVelocity.z);
			if ((double)vector.magnitude < 0.01)
			{
				this.SwitchState(MedusaEyeLantern.State.DORMANT);
			}
		}

		// Token: 0x0600794E RID: 31054 RVA: 0x00278AF0 File Offset: 0x00276CF0
		private void FaceTarget()
		{
			if (this.targetRig == null || this.rotatingObjectTransform == null)
			{
				return;
			}
			Vector3 normalized = (this.targetRig.tagSound.transform.position - this.rotatingObjectTransform.position).normalized;
			Vector3 normalized2 = new Vector3(normalized.x, 0f, normalized.z).normalized;
			Debug.DrawRay(this.rotatingObjectTransform.position, this.rotatingObjectTransform.forward * 0.3f, Color.blue);
			Debug.DrawRay(this.rotatingObjectTransform.position, normalized2 * 0.3f, Color.green);
			if (normalized2.sqrMagnitude > 0.001f)
			{
				float num = Mathf.Acos(Mathf.Clamp(Vector3.Dot(this.rotatingObjectTransform.forward.normalized, normalized2), -1f, 1f)) * 57.29578f;
				if (180f - num < this.targetHeadAngleThreshold && this.currentState == MedusaEyeLantern.State.TRACKING)
				{
					this.SwitchState(MedusaEyeLantern.State.WARMUP);
					return;
				}
				Quaternion quaternion = Quaternion.LookRotation(-normalized2, Vector3.up);
				this.rotatingObjectTransform.rotation = Quaternion.RotateTowards(this.rotatingObjectTransform.rotation, quaternion, this.lookAtTargetSpeed * Time.deltaTime);
			}
		}

		// Token: 0x0600794F RID: 31055 RVA: 0x00278C54 File Offset: 0x00276E54
		private bool IsTargetLookingAtEye()
		{
			if (this.targetRig == null || this.rotatingObjectTransform == null)
			{
				return false;
			}
			Transform transform = this.targetRig.tagSound.transform;
			Vector3 normalized = (this.rotatingObjectTransform.position - this.rotatingObjectTransform.forward * this.faceDistanceOffset - transform.position).normalized;
			float num = Mathf.Acos(Mathf.Clamp(Vector3.Dot(transform.up.normalized, normalized), -1f, 1f)) * 57.29578f;
			Debug.DrawRay(transform.position, transform.up * 0.3f, Color.magenta);
			Debug.DrawRay(transform.position, normalized * 0.3f, Color.yellow);
			return num < this.lookAtEyeAngleThreshold;
		}

		// Token: 0x06007950 RID: 31056 RVA: 0x00278D3C File Offset: 0x00276F3C
		private void UpdateState()
		{
			switch (this.currentState)
			{
			case MedusaEyeLantern.State.SLOSHING:
				this.Sloshing();
				break;
			case MedusaEyeLantern.State.DORMANT:
				this.warmupCounter = 0f;
				this.petrificationStarted = float.PositiveInfinity;
				if (this.targetRig != null && (this.targetRig.transform.position - base.transform.position).IsShorterThan(this.distanceChecker.distanceThreshold))
				{
					this.SwitchState(MedusaEyeLantern.State.TRACKING);
				}
				break;
			case MedusaEyeLantern.State.TRACKING:
				this.FaceTarget();
				break;
			case MedusaEyeLantern.State.WARMUP:
				this.warmupCounter += Time.deltaTime;
				this.FaceTarget();
				if (this.warmupCounter > this.warmUpProgressTime)
				{
					this.SwitchState(MedusaEyeLantern.State.PRIMING);
					this.warmupCounter = 0f;
				}
				break;
			case MedusaEyeLantern.State.PRIMING:
				this.FaceTarget();
				if (this.IsTargetLookingAtEye())
				{
					UnityEvent<VRRig> onPetrification = this.OnPetrification;
					if (onPetrification != null)
					{
						onPetrification.Invoke(this.targetRig);
					}
					this.SwitchState(MedusaEyeLantern.State.PETRIFICATION);
					this.petrificationStarted = Time.time;
				}
				break;
			case MedusaEyeLantern.State.PETRIFICATION:
				if (Time.time - this.petrificationStarted > this.petrificationDuration)
				{
					this.SwitchState(MedusaEyeLantern.State.COOLDOWN);
				}
				break;
			case MedusaEyeLantern.State.COOLDOWN:
				if (Time.time - this.petrificationStarted > this.resetCooldown)
				{
					this.SwitchState(MedusaEyeLantern.State.DORMANT);
					this.petrificationStarted = float.PositiveInfinity;
				}
				break;
			case MedusaEyeLantern.State.RESET:
				if (Time.time - this.resetTargetTime > this.resetTargetTimer)
				{
					this.resetTargetTime = float.PositiveInfinity;
					this.SwitchState(MedusaEyeLantern.State.DORMANT);
				}
				break;
			}
			this.PlayHaptic(this.currentState);
		}

		// Token: 0x06007951 RID: 31057 RVA: 0x00278EEC File Offset: 0x002770EC
		private void SwitchState(MedusaEyeLantern.State newState)
		{
			this.lastState = this.currentState;
			this.currentState = newState;
			MedusaEyeLantern.EyeState eyeState;
			if (this.lastState != this.currentState && this.allStatesDict.TryGetValue(newState, out eyeState))
			{
				UnityEvent onEnterState = eyeState.onEnterState;
				if (onEnterState != null)
				{
					onEnterState.Invoke();
				}
			}
			MedusaEyeLantern.EyeState eyeState2;
			if (this.lastState != this.currentState && this.allStatesDict.TryGetValue(this.lastState, out eyeState2))
			{
				UnityEvent onExitState = eyeState2.onExitState;
				if (onExitState == null)
				{
					return;
				}
				onExitState.Invoke();
			}
		}

		// Token: 0x06007952 RID: 31058 RVA: 0x00278F70 File Offset: 0x00277170
		private void PlayHaptic(MedusaEyeLantern.State state)
		{
			if (!this.transferableParent.IsMyItem())
			{
				return;
			}
			MedusaEyeLantern.EyeState eyeState;
			this.allStatesDict.TryGetValue(state, out eyeState);
			if (this.currentState == MedusaEyeLantern.State.WARMUP)
			{
				float num = Mathf.Clamp01(this.warmupCounter / this.warmUpProgressTime);
				if (eyeState != null && eyeState.hapticStrength != null)
				{
					float num2 = eyeState.hapticStrength.Evaluate(num);
					bool flag = this.transferableParent.InLeftHand();
					GorillaTagger.Instance.StartVibration(flag, num2, Time.deltaTime);
					return;
				}
			}
			else if (eyeState != null && eyeState.hapticStrength != null)
			{
				float num3 = eyeState.hapticStrength.Evaluate(0.5f);
				bool flag2 = this.transferableParent.InLeftHand();
				GorillaTagger.Instance.StartVibration(flag2, num3, Time.deltaTime);
			}
		}

		// Token: 0x06007953 RID: 31059 RVA: 0x00279029 File Offset: 0x00277229
		private bool EyeIsLockedOn()
		{
			return this.currentState == MedusaEyeLantern.State.TRACKING || this.currentState == MedusaEyeLantern.State.WARMUP || this.currentState == MedusaEyeLantern.State.PRIMING;
		}

		// Token: 0x04008A4B RID: 35403
		[SerializeField]
		private DistanceCheckerCosmetic distanceChecker;

		// Token: 0x04008A4C RID: 35404
		[SerializeField]
		private TransferrableObject transferableParent;

		// Token: 0x04008A4D RID: 35405
		[SerializeField]
		private GorillaVelocityTracker velocityTracker;

		// Token: 0x04008A4E RID: 35406
		[SerializeField]
		private Transform rotatingObjectTransform;

		// Token: 0x04008A4F RID: 35407
		[Space]
		[Header("Rotation Settings")]
		[SerializeField]
		private float maxRotationAngle = 50f;

		// Token: 0x04008A50 RID: 35408
		[SerializeField]
		private float sloshVelocityThreshold = 1f;

		// Token: 0x04008A51 RID: 35409
		[SerializeField]
		private float rotationSmoothing = 10f;

		// Token: 0x04008A52 RID: 35410
		[SerializeField]
		private float rotationSpeedMultiplier = 5f;

		// Token: 0x04008A53 RID: 35411
		[Space]
		[Header("Target Tracking Settings")]
		[SerializeField]
		private float lookAtEyeAngleThreshold = 90f;

		// Token: 0x04008A54 RID: 35412
		[SerializeField]
		private float targetHeadAngleThreshold = 5f;

		// Token: 0x04008A55 RID: 35413
		[SerializeField]
		private float lookAtTargetSpeed = 5f;

		// Token: 0x04008A56 RID: 35414
		[SerializeField]
		private float warmUpProgressTime = 3f;

		// Token: 0x04008A57 RID: 35415
		[SerializeField]
		private float resetCooldown = 5f;

		// Token: 0x04008A58 RID: 35416
		[SerializeField]
		private float faceDistanceOffset = 0.2f;

		// Token: 0x04008A59 RID: 35417
		[SerializeField]
		private float petrificationDuration = 0.2f;

		// Token: 0x04008A5A RID: 35418
		[Space]
		[Header("Eye State Settings")]
		public MedusaEyeLantern.EyeState[] allStates = new MedusaEyeLantern.EyeState[0];

		// Token: 0x04008A5B RID: 35419
		public UnityEvent<VRRig> OnPetrification;

		// Token: 0x04008A5C RID: 35420
		private Quaternion initialRotation;

		// Token: 0x04008A5D RID: 35421
		private Quaternion targetRotation;

		// Token: 0x04008A5E RID: 35422
		private MedusaEyeLantern.State currentState;

		// Token: 0x04008A5F RID: 35423
		private MedusaEyeLantern.State lastState;

		// Token: 0x04008A60 RID: 35424
		private float petrificationStarted = float.PositiveInfinity;

		// Token: 0x04008A61 RID: 35425
		private float warmupCounter;

		// Token: 0x04008A62 RID: 35426
		private Dictionary<MedusaEyeLantern.State, MedusaEyeLantern.EyeState> allStatesDict = new Dictionary<MedusaEyeLantern.State, MedusaEyeLantern.EyeState>();

		// Token: 0x04008A63 RID: 35427
		private VRRig targetRig;

		// Token: 0x04008A64 RID: 35428
		private float resetTargetTimer = 1f;

		// Token: 0x04008A65 RID: 35429
		private float resetTargetTime = float.PositiveInfinity;

		// Token: 0x020012E9 RID: 4841
		[Serializable]
		public class EyeState
		{
			// Token: 0x04008A66 RID: 35430
			public MedusaEyeLantern.State eyeState;

			// Token: 0x04008A67 RID: 35431
			public AnimationCurve hapticStrength;

			// Token: 0x04008A68 RID: 35432
			public UnityEvent onEnterState;

			// Token: 0x04008A69 RID: 35433
			public UnityEvent onExitState;
		}

		// Token: 0x020012EA RID: 4842
		public enum State
		{
			// Token: 0x04008A6B RID: 35435
			SLOSHING,
			// Token: 0x04008A6C RID: 35436
			DORMANT,
			// Token: 0x04008A6D RID: 35437
			TRACKING,
			// Token: 0x04008A6E RID: 35438
			WARMUP,
			// Token: 0x04008A6F RID: 35439
			PRIMING,
			// Token: 0x04008A70 RID: 35440
			PETRIFICATION,
			// Token: 0x04008A71 RID: 35441
			COOLDOWN,
			// Token: 0x04008A72 RID: 35442
			RESET
		}
	}
}
