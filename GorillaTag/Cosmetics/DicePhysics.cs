using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012D0 RID: 4816
	public class DicePhysics : MonoBehaviour
	{
		// Token: 0x060078AC RID: 30892 RVA: 0x00272A24 File Offset: 0x00270C24
		public int GetRandomSide()
		{
			DicePhysics.DiceType diceType = this.diceType;
			if (diceType != DicePhysics.DiceType.D6)
			{
				if (this.forceLandingSide)
				{
					return Mathf.Clamp(this.forcedLandingSide, 1, 20);
				}
				int num;
				if (this.CheckCosmeticRollOverride(out num))
				{
					return Mathf.Clamp(num, 1, 20);
				}
				return Random.Range(1, 21);
			}
			else
			{
				if (this.forceLandingSide)
				{
					return Mathf.Clamp(this.forcedLandingSide, 1, 6);
				}
				int num2;
				if (this.CheckCosmeticRollOverride(out num2))
				{
					return Mathf.Clamp(num2, 1, 6);
				}
				return Random.Range(1, 7);
			}
		}

		// Token: 0x060078AD RID: 30893 RVA: 0x00272AA4 File Offset: 0x00270CA4
		public Vector3 GetSideDirection(int side)
		{
			DicePhysics.DiceType diceType = this.diceType;
			if (diceType != DicePhysics.DiceType.D6)
			{
				int num = Mathf.Clamp(side - 1, 0, 19);
				return this.d20SideDirections[num];
			}
			int num2 = Mathf.Clamp(side - 1, 0, 5);
			return this.d6SideDirections[num2];
		}

		// Token: 0x060078AE RID: 30894 RVA: 0x00272AF0 File Offset: 0x00270CF0
		public void StartThrow(DiceHoldable holdable, Vector3 startPosition, Vector3 velocity, float playerScale, int side, double startTime)
		{
			this.holdableParent = holdable;
			base.transform.parent = null;
			base.transform.position = startPosition;
			base.transform.localScale = Vector3.one * playerScale;
			this.rb.isKinematic = false;
			this.rb.useGravity = true;
			this.rb.linearVelocity = velocity;
			if (!this.allowPickupFromGround && this.interactionPoint != null)
			{
				this.interactionPoint.enabled = false;
			}
			this.throwStartTime = ((startTime > 0.0) ? startTime : ((double)Time.time));
			this.throwSettledTime = -1.0;
			this.scale = playerScale;
			this.landingSide = Mathf.Clamp(side, 1, 20);
			this.prevVelocity = Vector3.zero;
			velocity = Vector3.zero;
			base.enabled = true;
		}

		// Token: 0x060078AF RID: 30895 RVA: 0x00272BD8 File Offset: 0x00270DD8
		public void EndThrow()
		{
			this.rb.isKinematic = true;
			this.rb.linearVelocity = Vector3.zero;
			if (this.holdableParent != null)
			{
				base.transform.parent = this.holdableParent.transform;
			}
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			base.transform.localScale = Vector3.one;
			this.scale = 1f;
			this.throwStartTime = -1.0;
			if (this.interactionPoint != null)
			{
				this.interactionPoint.enabled = true;
			}
			this.onRollFinished.Invoke();
			base.enabled = false;
		}

		// Token: 0x060078B0 RID: 30896 RVA: 0x00272C9C File Offset: 0x00270E9C
		private void FixedUpdate()
		{
			double num = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			float num2 = (float)(num - this.throwStartTime);
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 0.1f * this.scale, this.surfaceLayers.value, QueryTriggerInteraction.Ignore))
			{
				Vector3 normal = raycastHit.normal;
				Vector3 sideDirection = this.GetSideDirection(this.landingSide);
				Vector3 vector = base.transform.rotation * sideDirection;
				Vector3 normalized = Vector3.Cross(vector, normal).normalized;
				float num3 = Vector3.SignedAngle(vector, normal, normalized);
				float num4 = Mathf.Sign(num3) * this.angleDeltaVsStrengthCurve.Evaluate(Mathf.Clamp01(Mathf.Abs(num3) / 180f));
				float num5 = this.landingTimeVsStrengthCurve.Evaluate(Mathf.Clamp01(num2 / this.landingTime));
				float magnitude = this.rb.linearVelocity.magnitude;
				float num6 = Mathf.Clamp01(1f - Mathf.Min(magnitude, 1f));
				float num7 = Mathf.Max(num5, num6);
				Vector3 vector2 = this.strength * (num7 * num4 * normalized) - this.damping * this.rb.angularVelocity;
				this.rb.AddTorque(vector2, ForceMode.Acceleration);
				if (!this.rb.isKinematic && magnitude < 0.01f && num3 < 2f)
				{
					this.rb.isKinematic = true;
					this.throwSettledTime = num;
					this.InvokeLandingEffects(this.landingSide);
				}
				else if (!this.rb.isKinematic && num2 > this.landingTime)
				{
					this.rb.isKinematic = true;
					this.throwSettledTime = num;
					base.transform.rotation = Quaternion.FromToRotation(vector, normal) * base.transform.rotation;
					this.InvokeLandingEffects(this.landingSide);
				}
			}
			if (num2 > this.landingTime + this.postLandingTime || (this.rb.isKinematic && (float)(num - this.throwSettledTime) > this.postLandingTime))
			{
				this.EndThrow();
			}
			this.prevVelocity = this.velocity;
			this.velocity = this.rb.linearVelocity;
		}

		// Token: 0x060078B1 RID: 30897 RVA: 0x00272EE8 File Offset: 0x002710E8
		private void OnCollisionEnter(Collision collision)
		{
			float magnitude = collision.impulse.magnitude;
			if (magnitude > 0.001f)
			{
				Vector3 vector = Vector3.Reflect(this.prevVelocity, collision.impulse / magnitude);
				this.rb.linearVelocity = vector * this.bounceAmplification;
			}
		}

		// Token: 0x060078B2 RID: 30898 RVA: 0x00272F3C File Offset: 0x0027113C
		private void InvokeLandingEffects(int side)
		{
			DicePhysics.DiceType diceType = this.diceType;
			if (diceType != DicePhysics.DiceType.D6)
			{
				if (side == 20)
				{
					this.onBestRoll.Invoke();
					return;
				}
				if (side == 1)
				{
					this.onWorstRoll.Invoke();
					return;
				}
			}
			else
			{
				if (side == 6)
				{
					this.onBestRoll.Invoke();
					return;
				}
				if (side == 1)
				{
					this.onWorstRoll.Invoke();
				}
			}
		}

		// Token: 0x060078B3 RID: 30899 RVA: 0x00272F98 File Offset: 0x00271198
		private bool CheckCosmeticRollOverride(out int rollSide)
		{
			if (this.cosmeticRollOverrides.Length != 0)
			{
				if (this.cachedLocalRig == null)
				{
					RigContainer rigContainer;
					if (PhotonNetwork.InRoom && VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer) && rigContainer.Rig != null)
					{
						this.cachedLocalRig = rigContainer.Rig;
					}
					else
					{
						this.cachedLocalRig = GorillaTagger.Instance.offlineVRRig;
					}
				}
				if (this.cachedLocalRig != null)
				{
					int num = -1;
					for (int i = 0; i < this.cosmeticRollOverrides.Length; i++)
					{
						if (this.cosmeticRollOverrides[i].cosmeticName != null && this.cachedLocalRig.cosmeticSet != null && this.cachedLocalRig.cosmeticSet.HasItem(this.cosmeticRollOverrides[i].cosmeticName) && (!this.cosmeticRollOverrides[i].requireHolding || (EquipmentInteractor.instance.leftHandHeldEquipment != null && EquipmentInteractor.instance.leftHandHeldEquipment.name == this.cosmeticRollOverrides[i].cosmeticName) || (EquipmentInteractor.instance.rightHandHeldEquipment != null && EquipmentInteractor.instance.rightHandHeldEquipment.name == this.cosmeticRollOverrides[i].cosmeticName)) && this.cosmeticRollOverrides[i].landingSide > num)
						{
							num = this.cosmeticRollOverrides[i].landingSide;
						}
					}
					if (num > 0)
					{
						rollSide = num;
						return true;
					}
				}
			}
			rollSide = 0;
			return false;
		}

		// Token: 0x0400890E RID: 35086
		[SerializeField]
		private DicePhysics.DiceType diceType = DicePhysics.DiceType.D20;

		// Token: 0x0400890F RID: 35087
		[SerializeField]
		private float landingTime = 5f;

		// Token: 0x04008910 RID: 35088
		[SerializeField]
		private float postLandingTime = 2f;

		// Token: 0x04008911 RID: 35089
		[SerializeField]
		private LayerMask surfaceLayers;

		// Token: 0x04008912 RID: 35090
		[SerializeField]
		private AnimationCurve angleDeltaVsStrengthCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04008913 RID: 35091
		[SerializeField]
		private AnimationCurve landingTimeVsStrengthCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04008914 RID: 35092
		[SerializeField]
		private float strength = 1f;

		// Token: 0x04008915 RID: 35093
		[SerializeField]
		private float damping = 0.5f;

		// Token: 0x04008916 RID: 35094
		[SerializeField]
		private bool forceLandingSide;

		// Token: 0x04008917 RID: 35095
		[SerializeField]
		private int forcedLandingSide = 20;

		// Token: 0x04008918 RID: 35096
		[SerializeField]
		private bool allowPickupFromGround = true;

		// Token: 0x04008919 RID: 35097
		[SerializeField]
		private float bounceAmplification = 1f;

		// Token: 0x0400891A RID: 35098
		[SerializeField]
		private DicePhysics.CosmeticRollOverride[] cosmeticRollOverrides;

		// Token: 0x0400891B RID: 35099
		[SerializeField]
		private UnityEvent onBestRoll;

		// Token: 0x0400891C RID: 35100
		[SerializeField]
		private UnityEvent onWorstRoll;

		// Token: 0x0400891D RID: 35101
		[SerializeField]
		private UnityEvent onRollFinished;

		// Token: 0x0400891E RID: 35102
		[SerializeField]
		private Rigidbody rb;

		// Token: 0x0400891F RID: 35103
		[SerializeField]
		private InteractionPoint interactionPoint;

		// Token: 0x04008920 RID: 35104
		private VRRig cachedLocalRig;

		// Token: 0x04008921 RID: 35105
		private DiceHoldable holdableParent;

		// Token: 0x04008922 RID: 35106
		private double throwStartTime = -1.0;

		// Token: 0x04008923 RID: 35107
		private double throwSettledTime = -1.0;

		// Token: 0x04008924 RID: 35108
		private int landingSide;

		// Token: 0x04008925 RID: 35109
		private float scale;

		// Token: 0x04008926 RID: 35110
		private Vector3 prevVelocity = Vector3.zero;

		// Token: 0x04008927 RID: 35111
		private Vector3 velocity = Vector3.zero;

		// Token: 0x04008928 RID: 35112
		private const float a = 38.833332f;

		// Token: 0x04008929 RID: 35113
		private const float b = 77.66666f;

		// Token: 0x0400892A RID: 35114
		private Vector3[] d20SideDirections = new Vector3[]
		{
			Quaternion.AngleAxis(144f, Vector3.up) * Quaternion.AngleAxis(38.833332f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(324f, -Vector3.up) * Quaternion.AngleAxis(38.833332f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(288f, Vector3.up) * Quaternion.AngleAxis(38.833332f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(180f, -Vector3.up) * Quaternion.AngleAxis(38.833332f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(252f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(108f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(72f, Vector3.up) * Quaternion.AngleAxis(38.833332f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(36f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(216f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(0f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(180f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(324f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(144f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(108f, -Vector3.up) * Quaternion.AngleAxis(38.833332f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(72f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(288f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(0f, Vector3.up) * Quaternion.AngleAxis(38.833332f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(252f, -Vector3.up) * Quaternion.AngleAxis(38.833332f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(216f, Vector3.up) * Quaternion.AngleAxis(38.833332f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(36f, -Vector3.up) * Quaternion.AngleAxis(38.833332f, -Vector3.forward) * Vector3.up
		};

		// Token: 0x0400892B RID: 35115
		private Vector3[] d6SideDirections = new Vector3[]
		{
			new Vector3(0f, -1f, 0f),
			new Vector3(-1f, 0f, 0f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(1f, 0f, 0f),
			new Vector3(0f, 1f, 0f)
		};

		// Token: 0x020012D1 RID: 4817
		private enum DiceType
		{
			// Token: 0x0400892D RID: 35117
			D6,
			// Token: 0x0400892E RID: 35118
			D20
		}

		// Token: 0x020012D2 RID: 4818
		[Serializable]
		private struct CosmeticRollOverride
		{
			// Token: 0x0400892F RID: 35119
			public string cosmeticName;

			// Token: 0x04008930 RID: 35120
			public int landingSide;

			// Token: 0x04008931 RID: 35121
			public bool requireHolding;
		}
	}
}
