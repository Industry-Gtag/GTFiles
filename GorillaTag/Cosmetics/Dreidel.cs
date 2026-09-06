using System;
using CjLib;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001325 RID: 4901
	public class Dreidel : MonoBehaviour
	{
		// Token: 0x06007B0A RID: 31498 RVA: 0x00282004 File Offset: 0x00280204
		public bool TrySetIdle()
		{
			if (this.state == Dreidel.State.Idle || this.state == Dreidel.State.FindingSurface || this.state == Dreidel.State.Fallen)
			{
				this.StartIdle();
				return true;
			}
			return false;
		}

		// Token: 0x06007B0B RID: 31499 RVA: 0x00282029 File Offset: 0x00280229
		public bool TryCheckForSurfaces()
		{
			if (this.state == Dreidel.State.Idle || this.state == Dreidel.State.FindingSurface)
			{
				this.StartFindingSurfaces();
				return true;
			}
			return false;
		}

		// Token: 0x06007B0C RID: 31500 RVA: 0x00282045 File Offset: 0x00280245
		public void Spin()
		{
			this.StartSpin();
		}

		// Token: 0x06007B0D RID: 31501 RVA: 0x00282050 File Offset: 0x00280250
		public bool TryGetSpinStartData(out Vector3 surfacePoint, out Vector3 surfaceNormal, out float randomDuration, out Dreidel.Side randomSide, out Dreidel.Variation randomVariation, out double startTime)
		{
			if (this.canStartSpin)
			{
				surfacePoint = this.surfacePlanePoint;
				surfaceNormal = this.surfacePlaneNormal;
				randomDuration = Random.Range(this.spinTimeRange.x, this.spinTimeRange.y);
				randomSide = (Dreidel.Side)Random.Range(0, 4);
				randomVariation = (Dreidel.Variation)Random.Range(0, 5);
				startTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : (-1.0));
				return true;
			}
			surfacePoint = Vector3.zero;
			surfaceNormal = Vector3.zero;
			randomDuration = 0f;
			randomSide = Dreidel.Side.Shin;
			randomVariation = Dreidel.Variation.Tumble;
			startTime = -1.0;
			return false;
		}

		// Token: 0x06007B0E RID: 31502 RVA: 0x002820FC File Offset: 0x002802FC
		public void SetSpinStartData(Vector3 surfacePoint, Vector3 surfaceNormal, float duration, bool counterClockwise, Dreidel.Side side, Dreidel.Variation variation, double startTime)
		{
			this.surfacePlanePoint = surfacePoint;
			this.surfacePlaneNormal = surfaceNormal;
			this.spinTime = duration;
			this.spinStartTime = startTime;
			this.spinCounterClockwise = counterClockwise;
			this.landingSide = side;
			this.landingVariation = variation;
		}

		// Token: 0x06007B0F RID: 31503 RVA: 0x00282134 File Offset: 0x00280334
		private void LateUpdate()
		{
			float deltaTime = Time.deltaTime;
			double num = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			this.canStartSpin = false;
			switch (this.state)
			{
			default:
				base.transform.localPosition = Vector3.zero;
				base.transform.localRotation = Quaternion.identity;
				this.spinTransform.localRotation = Quaternion.identity;
				this.spinTransform.localPosition = Vector3.zero;
				return;
			case Dreidel.State.FindingSurface:
			{
				float num2 = ((GTPlayer.Instance != null) ? GTPlayer.Instance.scale : 1f);
				Vector3 down = Vector3.down;
				Vector3 vector = base.transform.parent.position - down * 2f * this.surfaceCheckDistance * num2;
				float num3 = (3f * this.surfaceCheckDistance + -this.bottomPointOffset.y) * num2;
				RaycastHit raycastHit;
				if (Physics.Raycast(vector, down, out raycastHit, num3, this.surfaceLayers.value, QueryTriggerInteraction.Ignore) && Vector3.Dot(raycastHit.normal, Vector3.up) > this.surfaceUprightThreshold && Vector3.Dot(raycastHit.normal, this.spinTransform.up) > this.surfaceDreidelAngleThreshold)
				{
					this.canStartSpin = true;
					this.surfacePlanePoint = raycastHit.point;
					this.surfacePlaneNormal = raycastHit.normal;
					this.AlignToSurfacePlane();
					this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
					this.UpdateSpinTransform();
					return;
				}
				this.canStartSpin = false;
				base.transform.localPosition = Vector3.zero;
				base.transform.localRotation = Quaternion.identity;
				this.spinTransform.localRotation = Quaternion.identity;
				this.spinTransform.localPosition = Vector3.zero;
				return;
			}
			case Dreidel.State.Spinning:
			{
				float num4 = Mathf.Clamp01((float)(num - this.stateStartTime) / this.spinTime);
				this.spinSpeed = Mathf.Lerp(this.spinSpeedStart, this.spinSpeedEnd, num4);
				float num5 = (this.spinCounterClockwise ? (-1f) : 1f);
				this.spinAngle += num5 * this.spinSpeed * 360f * deltaTime;
				float num6 = this.tiltWobble;
				float num7 = Mathf.Sin(this.spinWobbleFrequency * 2f * 3.1415927f * (float)(num - this.stateStartTime));
				float num8 = 0.5f * num7 + 0.5f;
				this.tiltWobble = Mathf.Lerp(this.spinWobbleAmplitudeEndMin * num4, this.spinWobbleAmplitude * num4, num8);
				if (this.landingTiltTarget.y == 0f)
				{
					if (this.landingVariation == Dreidel.Variation.Tumble || this.landingVariation == Dreidel.Variation.Smooth)
					{
						this.tiltFrontBack = Mathf.Sign(this.landingTiltTarget.x) * this.tiltWobble;
					}
					else
					{
						this.tiltFrontBack = Mathf.Sign(this.landingTiltLeadingTarget.x) * this.tiltWobble;
					}
				}
				else if (this.landingVariation == Dreidel.Variation.Tumble || this.landingVariation == Dreidel.Variation.Smooth)
				{
					this.tiltLeftRight = Mathf.Sign(this.landingTiltTarget.y) * this.tiltWobble;
				}
				else
				{
					this.tiltLeftRight = Mathf.Sign(this.landingTiltLeadingTarget.y) * this.tiltWobble;
				}
				float num9 = Mathf.Lerp(this.pathStartTurnRate, this.pathEndTurnRate, num4) + num7 * this.pathTurnRateSinOffset;
				if (this.spinCounterClockwise)
				{
					this.pathDir = Vector3.ProjectOnPlane(Quaternion.AngleAxis(-num9 * deltaTime, Vector3.up) * this.pathDir, Vector3.up);
					this.pathDir.Normalize();
				}
				else
				{
					this.pathDir = Vector3.ProjectOnPlane(Quaternion.AngleAxis(-num9 * deltaTime, Vector3.up) * this.pathDir, Vector3.up);
					this.pathDir.Normalize();
				}
				this.pathOffset += this.pathDir * this.pathMoveSpeed * deltaTime;
				this.AlignToSurfacePlane();
				this.UpdateSpinTransform();
				if (num4 - Mathf.Epsilon >= 1f && this.tiltWobble > 0.9f * this.spinWobbleAmplitude && num6 < this.tiltWobble)
				{
					this.StartFall();
					return;
				}
				break;
			}
			case Dreidel.State.Falling:
			{
				float num10 = this.fallTimeTumble;
				Dreidel.Variation variation = this.landingVariation;
				if (variation <= Dreidel.Variation.Smooth || variation - Dreidel.Variation.Bounce > 2)
				{
					this.spinSpeed = Mathf.MoveTowards(this.spinSpeed, 0f, this.spinSpeedStopRate * deltaTime);
					float num11 = (this.spinCounterClockwise ? (-1f) : 1f);
					this.spinAngle += num11 * this.spinSpeed * 360f * deltaTime;
					float num12 = ((this.landingVariation == Dreidel.Variation.Smooth) ? this.smoothFallFrequency : this.tumbleFallFrontBackFrequency);
					float num13 = ((this.landingVariation == Dreidel.Variation.Smooth) ? this.smoothFallDampingRatio : this.tumbleFallFrontBackDampingRatio);
					float num14 = ((this.landingVariation == Dreidel.Variation.Smooth) ? this.smoothFallFrequency : this.tumbleFallFrequency);
					float num15 = ((this.landingVariation == Dreidel.Variation.Smooth) ? this.smoothFallDampingRatio : this.tumbleFallDampingRatio);
					this.tiltFrontBack = this.tiltFrontBackSpring.TrackDampingRatio(this.landingTiltTarget.x, num12, num13, deltaTime);
					this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, num14, num15, deltaTime);
				}
				else
				{
					bool flag = this.landingVariation != Dreidel.Variation.Bounce;
					bool flag2 = this.landingVariation == Dreidel.Variation.FalseSlowTurn;
					float num16 = (flag ? this.slowTurnSwitchTime : this.bounceFallSwitchTime);
					if (flag)
					{
						num10 = this.fallTimeSlowTurn;
					}
					if (num - this.stateStartTime < (double)num16)
					{
						this.tiltFrontBack = this.tiltFrontBackSpring.TrackDampingRatio(this.landingTiltLeadingTarget.x, this.tumbleFallFrontBackFrequency, this.tumbleFallFrontBackDampingRatio, deltaTime);
						this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltLeadingTarget.y, this.tumbleFallFrequency, this.tumbleFallDampingRatio, deltaTime);
					}
					else
					{
						this.tiltFrontBack = this.tiltFrontBackSpring.TrackDampingRatio(this.landingTiltTarget.x, this.tumbleFallFrontBackFrequency, this.tumbleFallFrontBackDampingRatio, deltaTime);
						if (flag2)
						{
							if (!this.falseTargetReached && Mathf.Abs(this.landingTiltTarget.y - this.tiltLeftRight) > 0.49f)
							{
								this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, this.slowTurnFrequency, this.slowTurnDampingRatio, deltaTime);
							}
							else
							{
								this.falseTargetReached = true;
								this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltLeadingTarget.y, this.tumbleFallFrequency, this.tumbleFallDampingRatio, deltaTime);
							}
						}
						else if (flag && Mathf.Abs(this.landingTiltTarget.y - this.tiltLeftRight) > 0.45f)
						{
							this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, this.slowTurnFrequency, this.slowTurnDampingRatio, deltaTime);
						}
						else
						{
							this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, this.tumbleFallFrequency, this.tumbleFallDampingRatio, deltaTime);
						}
					}
					this.spinSpeed = Mathf.MoveTowards(this.spinSpeed, 0f, this.spinSpeedStopRate * deltaTime);
					float num17 = (this.spinCounterClockwise ? (-1f) : 1f);
					this.spinAngle += num17 * this.spinSpeed * 360f * deltaTime;
				}
				this.AlignToSurfacePlane();
				this.UpdateSpinTransform();
				float num18 = (float)(num - this.stateStartTime);
				if (num18 > num10)
				{
					if (!this.hasLanded)
					{
						this.hasLanded = true;
						if (this.landingSide == Dreidel.Side.Gimel)
						{
							this.gimelConfetti.transform.position = this.spinTransform.position + Vector3.up * this.confettiHeight;
							this.gimelConfetti.gameObject.SetActive(true);
							this.audioSource.GTPlayOneShot(this.gimelConfettiSound, 1f);
						}
					}
					if (num18 > num10 + this.respawnTimeAfterLanding)
					{
						this.StartIdle();
					}
				}
				break;
			}
			case Dreidel.State.Fallen:
				break;
			}
		}

		// Token: 0x06007B10 RID: 31504 RVA: 0x00282960 File Offset: 0x00280B60
		private void StartIdle()
		{
			this.state = Dreidel.State.Idle;
			this.stateStartTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			this.canStartSpin = false;
			this.spinAngle = 0f;
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			this.spinTransform.localRotation = Quaternion.identity;
			this.spinTransform.localPosition = Vector3.zero;
			this.tiltFrontBack = 0f;
			this.tiltLeftRight = 0f;
			this.pathOffset = Vector3.zero;
			this.pathDir = Vector3.forward;
			this.gimelConfetti.gameObject.SetActive(false);
			this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
			this.UpdateSpinTransform();
		}

		// Token: 0x06007B11 RID: 31505 RVA: 0x00282A3C File Offset: 0x00280C3C
		private void StartFindingSurfaces()
		{
			this.state = Dreidel.State.FindingSurface;
			this.stateStartTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			this.canStartSpin = false;
			this.spinAngle = 0f;
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			this.spinTransform.localRotation = Quaternion.identity;
			this.spinTransform.localPosition = Vector3.zero;
			this.tiltFrontBack = 0f;
			this.tiltLeftRight = 0f;
			this.pathOffset = Vector3.zero;
			this.pathDir = Vector3.forward;
			this.gimelConfetti.gameObject.SetActive(false);
			this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
			this.UpdateSpinTransform();
		}

		// Token: 0x06007B12 RID: 31506 RVA: 0x00282B18 File Offset: 0x00280D18
		private void StartSpin()
		{
			this.state = Dreidel.State.Spinning;
			this.stateStartTime = ((this.spinStartTime > 0.0) ? this.spinStartTime : ((double)Time.time));
			this.canStartSpin = false;
			this.spinSpeed = this.spinSpeedStart;
			this.tiltWobble = 0f;
			this.audioSource.loop = true;
			this.audioSource.clip = this.spinLoopAudio;
			this.audioSource.GTPlay();
			this.gimelConfetti.gameObject.SetActive(false);
			this.AlignToSurfacePlane();
			this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
			this.UpdateSpinTransform();
			this.pathOffset = Vector3.zero;
			this.pathDir = Vector3.forward;
		}

		// Token: 0x06007B13 RID: 31507 RVA: 0x00282BE0 File Offset: 0x00280DE0
		private void StartFall()
		{
			this.state = Dreidel.State.Falling;
			this.stateStartTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			this.canStartSpin = false;
			this.falseTargetReached = false;
			this.hasLanded = false;
			if (this.landingVariation == Dreidel.Variation.FalseSlowTurn)
			{
				if (this.spinCounterClockwise)
				{
					this.GetTiltVectorsForSideWithPrev(this.landingSide, out this.landingTiltLeadingTarget, out this.landingTiltTarget);
				}
				else
				{
					this.GetTiltVectorsForSideWithNext(this.landingSide, out this.landingTiltLeadingTarget, out this.landingTiltTarget);
				}
			}
			else if (this.spinCounterClockwise)
			{
				this.GetTiltVectorsForSideWithNext(this.landingSide, out this.landingTiltTarget, out this.landingTiltLeadingTarget);
			}
			else
			{
				this.GetTiltVectorsForSideWithPrev(this.landingSide, out this.landingTiltTarget, out this.landingTiltLeadingTarget);
			}
			this.spinSpeedSpring.Reset(this.spinSpeed, 0f);
			this.tiltFrontBackSpring.Reset(this.tiltFrontBack, 0f);
			this.tiltLeftRightSpring.Reset(this.tiltLeftRight, 0f);
			this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
			this.audioSource.loop = false;
			this.audioSource.GTPlayOneShot(this.fallSound, 1f);
			this.gimelConfetti.gameObject.SetActive(false);
		}

		// Token: 0x06007B14 RID: 31508 RVA: 0x00282D30 File Offset: 0x00280F30
		private Vector3 GetGroundContactPoint()
		{
			Vector3 position = this.spinTransform.position;
			this.dreidelCollider.enabled = true;
			Vector3 vector = this.dreidelCollider.ClosestPoint(position - base.transform.up);
			this.dreidelCollider.enabled = false;
			float num = Vector3.Dot(vector - position, this.spinTransform.up);
			if (num > 0f)
			{
				vector -= num * this.spinTransform.up;
			}
			return this.spinTransform.InverseTransformPoint(vector);
		}

		// Token: 0x06007B15 RID: 31509 RVA: 0x00282DC4 File Offset: 0x00280FC4
		private void GetTiltVectorsForSideWithPrev(Dreidel.Side side, out Vector2 sideTilt, out Vector2 prevSideTilt)
		{
			int num = ((side <= Dreidel.Side.Shin) ? 3 : (side - Dreidel.Side.Hey));
			if (side == Dreidel.Side.Hey || side == Dreidel.Side.Nun)
			{
				sideTilt = this.landingTiltValues[(int)side];
				prevSideTilt = this.landingTiltValues[num];
				prevSideTilt.x = sideTilt.x;
				return;
			}
			prevSideTilt = this.landingTiltValues[num];
			sideTilt = this.landingTiltValues[(int)side];
			sideTilt.x = prevSideTilt.x;
		}

		// Token: 0x06007B16 RID: 31510 RVA: 0x00282E48 File Offset: 0x00281048
		private void GetTiltVectorsForSideWithNext(Dreidel.Side side, out Vector2 sideTilt, out Vector2 nextSideTilt)
		{
			int num = (int)((side + 1) % Dreidel.Side.Count);
			if (side == Dreidel.Side.Hey || side == Dreidel.Side.Nun)
			{
				sideTilt = this.landingTiltValues[(int)side];
				nextSideTilt = this.landingTiltValues[num];
				nextSideTilt.x = sideTilt.x;
				return;
			}
			nextSideTilt = this.landingTiltValues[num];
			sideTilt = this.landingTiltValues[(int)side];
			sideTilt.x = nextSideTilt.x;
		}

		// Token: 0x06007B17 RID: 31511 RVA: 0x00282EC4 File Offset: 0x002810C4
		private void AlignToSurfacePlane()
		{
			Vector3 vector = Vector3.forward;
			if (Vector3.Dot(Vector3.up, this.surfacePlaneNormal) < 0.9999f)
			{
				Vector3 vector2 = Vector3.Cross(this.surfacePlaneNormal, Vector3.up);
				vector = Quaternion.AngleAxis(90f, vector2) * this.surfacePlaneNormal;
			}
			Quaternion quaternion = Quaternion.LookRotation(vector, this.surfacePlaneNormal);
			base.transform.position = this.surfacePlanePoint;
			base.transform.rotation = quaternion;
		}

		// Token: 0x06007B18 RID: 31512 RVA: 0x00282F40 File Offset: 0x00281140
		private void UpdateSpinTransform()
		{
			Vector3 position = this.spinTransform.position;
			Vector3 groundContactPoint = this.GetGroundContactPoint();
			Vector3 vector = this.groundPointSpring.TrackDampingRatio(groundContactPoint, this.groundTrackingFrequency, this.groundTrackingDampingRatio, Time.deltaTime);
			Vector3 vector2 = this.spinTransform.TransformPoint(vector);
			Quaternion quaternion = Quaternion.AngleAxis(90f * this.tiltLeftRight, Vector3.forward) * Quaternion.AngleAxis(90f * this.tiltFrontBack, Vector3.right);
			this.spinAxis = base.transform.InverseTransformDirection(base.transform.up);
			Quaternion quaternion2 = Quaternion.AngleAxis(this.spinAngle, this.spinAxis);
			this.spinTransform.localRotation = quaternion2 * quaternion;
			Vector3 vector3 = base.transform.InverseTransformVector(Vector3.Dot(position - vector2, base.transform.up) * base.transform.up);
			this.spinTransform.localPosition = vector3 + this.pathOffset;
			this.spinTransform.TransformPoint(this.bottomPointOffset);
		}

		// Token: 0x04008CA1 RID: 36001
		[Header("References")]
		[SerializeField]
		private Transform spinTransform;

		// Token: 0x04008CA2 RID: 36002
		[SerializeField]
		private MeshCollider dreidelCollider;

		// Token: 0x04008CA3 RID: 36003
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04008CA4 RID: 36004
		[SerializeField]
		private AudioClip spinLoopAudio;

		// Token: 0x04008CA5 RID: 36005
		[SerializeField]
		private AudioClip fallSound;

		// Token: 0x04008CA6 RID: 36006
		[SerializeField]
		private AudioClip gimelConfettiSound;

		// Token: 0x04008CA7 RID: 36007
		[SerializeField]
		private ParticleSystem gimelConfetti;

		// Token: 0x04008CA8 RID: 36008
		[Header("Offsets")]
		[SerializeField]
		private Vector3 centerOfMassOffset = Vector3.zero;

		// Token: 0x04008CA9 RID: 36009
		[SerializeField]
		private Vector3 bottomPointOffset = Vector3.zero;

		// Token: 0x04008CAA RID: 36010
		[SerializeField]
		private Vector2 bodyRect = Vector2.one;

		// Token: 0x04008CAB RID: 36011
		[SerializeField]
		private float confettiHeight = 0.125f;

		// Token: 0x04008CAC RID: 36012
		[Header("Surface Detection")]
		[SerializeField]
		private float surfaceCheckDistance = 0.15f;

		// Token: 0x04008CAD RID: 36013
		[SerializeField]
		private float surfaceUprightThreshold = 0.5f;

		// Token: 0x04008CAE RID: 36014
		[SerializeField]
		private float surfaceDreidelAngleThreshold = 0.9f;

		// Token: 0x04008CAF RID: 36015
		[SerializeField]
		private LayerMask surfaceLayers;

		// Token: 0x04008CB0 RID: 36016
		[Header("Spin Paramss")]
		[SerializeField]
		private float spinSpeedStart = 2f;

		// Token: 0x04008CB1 RID: 36017
		[SerializeField]
		private float spinSpeedEnd = 1f;

		// Token: 0x04008CB2 RID: 36018
		[SerializeField]
		private float spinTime = 10f;

		// Token: 0x04008CB3 RID: 36019
		[SerializeField]
		private Vector2 spinTimeRange = new Vector2(7f, 12f);

		// Token: 0x04008CB4 RID: 36020
		[SerializeField]
		private float spinWobbleFrequency = 0.1f;

		// Token: 0x04008CB5 RID: 36021
		[SerializeField]
		private float spinWobbleAmplitude = 0.01f;

		// Token: 0x04008CB6 RID: 36022
		[SerializeField]
		private float spinWobbleAmplitudeEndMin = 0.01f;

		// Token: 0x04008CB7 RID: 36023
		[SerializeField]
		private float tiltFrontBack;

		// Token: 0x04008CB8 RID: 36024
		[SerializeField]
		private float tiltLeftRight;

		// Token: 0x04008CB9 RID: 36025
		[SerializeField]
		private float groundTrackingDampingRatio = 0.9f;

		// Token: 0x04008CBA RID: 36026
		[SerializeField]
		private float groundTrackingFrequency = 1f;

		// Token: 0x04008CBB RID: 36027
		[Header("Motion Path")]
		[SerializeField]
		private float pathMoveSpeed = 0.1f;

		// Token: 0x04008CBC RID: 36028
		[SerializeField]
		private float pathStartTurnRate = 360f;

		// Token: 0x04008CBD RID: 36029
		[SerializeField]
		private float pathEndTurnRate = 90f;

		// Token: 0x04008CBE RID: 36030
		[SerializeField]
		private float pathTurnRateSinOffset = 180f;

		// Token: 0x04008CBF RID: 36031
		[Header("Falling Params")]
		[SerializeField]
		private float spinSpeedStopRate = 1f;

		// Token: 0x04008CC0 RID: 36032
		[SerializeField]
		private float tumbleFallDampingRatio = 0.4f;

		// Token: 0x04008CC1 RID: 36033
		[SerializeField]
		private float tumbleFallFrequency = 6f;

		// Token: 0x04008CC2 RID: 36034
		[SerializeField]
		private float tumbleFallFrontBackDampingRatio = 0.4f;

		// Token: 0x04008CC3 RID: 36035
		[SerializeField]
		private float tumbleFallFrontBackFrequency = 6f;

		// Token: 0x04008CC4 RID: 36036
		[SerializeField]
		private float smoothFallDampingRatio = 0.9f;

		// Token: 0x04008CC5 RID: 36037
		[SerializeField]
		private float smoothFallFrequency = 2f;

		// Token: 0x04008CC6 RID: 36038
		[SerializeField]
		private float slowTurnDampingRatio = 0.9f;

		// Token: 0x04008CC7 RID: 36039
		[SerializeField]
		private float slowTurnFrequency = 2f;

		// Token: 0x04008CC8 RID: 36040
		[SerializeField]
		private float bounceFallSwitchTime = 0.5f;

		// Token: 0x04008CC9 RID: 36041
		[SerializeField]
		private float slowTurnSwitchTime = 0.5f;

		// Token: 0x04008CCA RID: 36042
		[SerializeField]
		private float respawnTimeAfterLanding = 3f;

		// Token: 0x04008CCB RID: 36043
		[SerializeField]
		private float fallTimeTumble = 3f;

		// Token: 0x04008CCC RID: 36044
		[SerializeField]
		private float fallTimeSlowTurn = 5f;

		// Token: 0x04008CCD RID: 36045
		private Dreidel.State state;

		// Token: 0x04008CCE RID: 36046
		private double stateStartTime;

		// Token: 0x04008CCF RID: 36047
		private float spinSpeed;

		// Token: 0x04008CD0 RID: 36048
		private float spinAngle;

		// Token: 0x04008CD1 RID: 36049
		private Vector3 spinAxis = Vector3.up;

		// Token: 0x04008CD2 RID: 36050
		private bool canStartSpin;

		// Token: 0x04008CD3 RID: 36051
		private double spinStartTime = -1.0;

		// Token: 0x04008CD4 RID: 36052
		private float tiltWobble;

		// Token: 0x04008CD5 RID: 36053
		private bool falseTargetReached;

		// Token: 0x04008CD6 RID: 36054
		private bool hasLanded;

		// Token: 0x04008CD7 RID: 36055
		private Vector3 pathOffset = Vector3.zero;

		// Token: 0x04008CD8 RID: 36056
		private Vector3 pathDir = Vector3.forward;

		// Token: 0x04008CD9 RID: 36057
		private Vector3 surfacePlanePoint;

		// Token: 0x04008CDA RID: 36058
		private Vector3 surfacePlaneNormal;

		// Token: 0x04008CDB RID: 36059
		private FloatSpring tiltFrontBackSpring;

		// Token: 0x04008CDC RID: 36060
		private FloatSpring tiltLeftRightSpring;

		// Token: 0x04008CDD RID: 36061
		private FloatSpring spinSpeedSpring;

		// Token: 0x04008CDE RID: 36062
		private Vector3Spring groundPointSpring;

		// Token: 0x04008CDF RID: 36063
		private Vector2[] landingTiltValues = new Vector2[]
		{
			new Vector2(1f, -1f),
			new Vector2(1f, 0f),
			new Vector2(-1f, 1f),
			new Vector2(-1f, 0f)
		};

		// Token: 0x04008CE0 RID: 36064
		private Vector2 landingTiltLeadingTarget = Vector2.zero;

		// Token: 0x04008CE1 RID: 36065
		private Vector2 landingTiltTarget = Vector2.zero;

		// Token: 0x04008CE2 RID: 36066
		[Header("Debug Params")]
		[SerializeField]
		private Dreidel.Side landingSide;

		// Token: 0x04008CE3 RID: 36067
		[SerializeField]
		private Dreidel.Variation landingVariation;

		// Token: 0x04008CE4 RID: 36068
		[SerializeField]
		private bool spinCounterClockwise;

		// Token: 0x04008CE5 RID: 36069
		[SerializeField]
		private bool debugDraw;

		// Token: 0x02001326 RID: 4902
		private enum State
		{
			// Token: 0x04008CE7 RID: 36071
			Idle,
			// Token: 0x04008CE8 RID: 36072
			FindingSurface,
			// Token: 0x04008CE9 RID: 36073
			Spinning,
			// Token: 0x04008CEA RID: 36074
			Falling,
			// Token: 0x04008CEB RID: 36075
			Fallen
		}

		// Token: 0x02001327 RID: 4903
		public enum Side
		{
			// Token: 0x04008CED RID: 36077
			Shin,
			// Token: 0x04008CEE RID: 36078
			Hey,
			// Token: 0x04008CEF RID: 36079
			Gimel,
			// Token: 0x04008CF0 RID: 36080
			Nun,
			// Token: 0x04008CF1 RID: 36081
			Count
		}

		// Token: 0x02001328 RID: 4904
		public enum Variation
		{
			// Token: 0x04008CF3 RID: 36083
			Tumble,
			// Token: 0x04008CF4 RID: 36084
			Smooth,
			// Token: 0x04008CF5 RID: 36085
			Bounce,
			// Token: 0x04008CF6 RID: 36086
			SlowTurn,
			// Token: 0x04008CF7 RID: 36087
			FalseSlowTurn,
			// Token: 0x04008CF8 RID: 36088
			Count
		}
	}
}
