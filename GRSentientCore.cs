using System;
using CjLib;
using GorillaLocomotion;
using Photon.Pun;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x020007F0 RID: 2032
public class GRSentientCore : MonoBehaviour, IGRSleepableEntity
{
	// Token: 0x170004B6 RID: 1206
	// (get) Token: 0x060033E8 RID: 13288 RVA: 0x000AB694 File Offset: 0x000A9894
	public Vector3 Position
	{
		get
		{
			return base.transform.position;
		}
	}

	// Token: 0x170004B7 RID: 1207
	// (get) Token: 0x060033E9 RID: 13289 RVA: 0x0011D00E File Offset: 0x0011B20E
	public float WakeUpRadius
	{
		get
		{
			return this.wakeupRadius;
		}
	}

	// Token: 0x060033EA RID: 13290 RVA: 0x0011D018 File Offset: 0x0011B218
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		GhostReactor.instance.sleepableEntities.Add(this);
		this.gameEntity.OnStateChanged += this.OnStateChanged;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.OnReleased));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnSnapped = (Action)Delegate.Combine(gameEntity3.OnSnapped, new Action(this.OnSnapped));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnDetached = (Action)Delegate.Combine(gameEntity4.OnDetached, new Action(this.OnDetached));
		this.Sleep();
	}

	// Token: 0x060033EB RID: 13291 RVA: 0x0011D0FC File Offset: 0x0011B2FC
	private void OnDestroy()
	{
		if (GhostReactor.instance != null)
		{
			GhostReactor.instance.sleepableEntities.Remove(this);
		}
		if (this.gameEntity != null)
		{
			this.gameEntity.OnStateChanged -= this.OnStateChanged;
			GameEntity gameEntity = this.gameEntity;
			gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
			GameEntity gameEntity2 = this.gameEntity;
			gameEntity2.OnReleased = (Action)Delegate.Remove(gameEntity2.OnReleased, new Action(this.OnReleased));
			GameEntity gameEntity3 = this.gameEntity;
			gameEntity3.OnSnapped = (Action)Delegate.Remove(gameEntity3.OnSnapped, new Action(this.OnSnapped));
			GameEntity gameEntity4 = this.gameEntity;
			gameEntity4.OnDetached = (Action)Delegate.Remove(gameEntity4.OnDetached, new Action(this.OnDetached));
		}
	}

	// Token: 0x060033EC RID: 13292 RVA: 0x0011D1EB File Offset: 0x0011B3EB
	public bool IsSleeping()
	{
		return this.gameEntity.GetState() == 0L;
	}

	// Token: 0x060033ED RID: 13293 RVA: 0x0011D1FC File Offset: 0x0011B3FC
	public void WakeUp()
	{
		if (this.gameEntity.IsAuthority() && this.IsSleeping())
		{
			this.gameEntity.RequestState(this.gameEntity.id, 1L);
		}
		if (this.localState == GRSentientCore.SentientCoreState.Asleep)
		{
			this.localState = GRSentientCore.SentientCoreState.Awake;
			this.localStateStartTime = Time.time;
		}
		this.sleepRequested = false;
		base.enabled = true;
	}

	// Token: 0x060033EE RID: 13294 RVA: 0x0011D25E File Offset: 0x0011B45E
	public void Sleep()
	{
		this.sleepRequested = true;
	}

	// Token: 0x060033EF RID: 13295 RVA: 0x0011D267 File Offset: 0x0011B467
	private void OnStateChanged(long prevState, long nextState)
	{
		if ((int)nextState == 0)
		{
			this.sleepRequested = false;
		}
		else if (!base.enabled)
		{
			this.WakeUp();
		}
		this.SetState((GRSentientCore.SentientCoreState)nextState);
	}

	// Token: 0x060033F0 RID: 13296 RVA: 0x0011D28C File Offset: 0x0011B48C
	private void OnGrabbed()
	{
		this.WakeUp();
		this.SetState(GRSentientCore.SentientCoreState.Held);
		this.timeUntilNextAlert = Mathf.Min(this.timeUntilFirstAlert, this.timeUntilNextAlert);
	}

	// Token: 0x060033F1 RID: 13297 RVA: 0x0011D2B2 File Offset: 0x0011B4B2
	private void OnReleased()
	{
		this.SetState(GRSentientCore.SentientCoreState.Dropped);
	}

	// Token: 0x060033F2 RID: 13298 RVA: 0x0011D2BB File Offset: 0x0011B4BB
	private void OnSnapped()
	{
		this.SetState(GRSentientCore.SentientCoreState.AttachedToPlayer);
	}

	// Token: 0x060033F3 RID: 13299 RVA: 0x0011D2B2 File Offset: 0x0011B4B2
	private void OnDetached()
	{
		this.SetState(GRSentientCore.SentientCoreState.Dropped);
	}

	// Token: 0x060033F4 RID: 13300 RVA: 0x0011D2C4 File Offset: 0x0011B4C4
	private void Update()
	{
		if (this.debugDraw)
		{
			DebugUtil.DrawSphere(base.transform.position, 0.15f, 12, 12, Color.cyan, true, DebugUtil.Style.Wireframe);
		}
		if (this.gameEntity.IsAuthority())
		{
			this.AuthorityUpdate();
		}
		this.SharedUpdate();
	}

	// Token: 0x060033F5 RID: 13301 RVA: 0x0011D314 File Offset: 0x0011B514
	private void AuthorityUpdate()
	{
		if (this.trailFX != null)
		{
			if (this.gameEntity.snappedByActorNumber != -1 || this.gameEntity.heldByActorNumber != -1)
			{
				if (this.trailFX.isPlaying)
				{
					this.trailFX.Stop();
				}
			}
			else if (!this.trailFX.isPlaying)
			{
				this.trailFX.Play();
			}
		}
		switch (this.localState)
		{
		case GRSentientCore.SentientCoreState.Asleep:
		case GRSentientCore.SentientCoreState.JumpAnticipation:
		case GRSentientCore.SentientCoreState.Jumping:
		case GRSentientCore.SentientCoreState.HeldAlert:
		case GRSentientCore.SentientCoreState.Dropped:
			break;
		case GRSentientCore.SentientCoreState.Awake:
			if (this.sleepRequested)
			{
				this.sleepRequested = false;
				this.SetState(GRSentientCore.SentientCoreState.Asleep);
			}
			if (this.gameEntity.heldByActorNumber != -1)
			{
				this.SetState(GRSentientCore.SentientCoreState.Held);
				return;
			}
			if (!this.sleepRequested && Time.time > this.localStateStartTime + this.jumpCooldownTime)
			{
				this.AuthorityInitiateJump();
				return;
			}
			break;
		case GRSentientCore.SentientCoreState.JumpInitiated:
			if (this.sleepRequested)
			{
				this.sleepRequested = false;
				this.SetState(GRSentientCore.SentientCoreState.Asleep);
				return;
			}
			break;
		case GRSentientCore.SentientCoreState.Held:
			this.timeUntilNextAlert -= Time.deltaTime;
			if (this.timeUntilNextAlert < 0f)
			{
				this.timeUntilNextAlert = Random.Range(this.timeRangeBetweenAlerts.x, this.timeRangeBetweenAlerts.y);
				this.SetState(GRSentientCore.SentientCoreState.HeldAlert);
				return;
			}
			break;
		case GRSentientCore.SentientCoreState.AttachedToPlayer:
			this.timeUntilNextAlert -= Time.deltaTime;
			if (this.timeUntilNextAlert < 0f)
			{
				this.timeUntilNextAlert = Random.Range(this.timeRangeBetweenAlerts.x, this.timeRangeBetweenAlerts.y);
				this.alertEnemiesSound.Play(null);
				GRNoiseEventManager.instance.AddNoiseEvent(base.transform.position, this.alertNoiseEventMagnitude, this.enemyAlertDuration);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060033F6 RID: 13302 RVA: 0x0011D4D8 File Offset: 0x0011B6D8
	private void SharedUpdate()
	{
		switch (this.localState)
		{
		default:
			base.enabled = false;
			return;
		case GRSentientCore.SentientCoreState.Awake:
			if (this.visualCore != null && this.visualCore.transform.localScale != Vector3.one)
			{
				this.visualCore.transform.localScale = Vector3.one;
				this.visualCore.transform.localPosition = Vector3.zero;
				this.visualCore.transform.localRotation = Quaternion.identity;
				return;
			}
			break;
		case GRSentientCore.SentientCoreState.JumpAnticipation:
			if (this.debugDraw)
			{
				this.DrawJumpPath(Color.yellow);
			}
			if (Time.time <= this.jumpStartTime)
			{
				Vector3 normalized = (this.surfaceNormal + this.jumpDirection).normalized;
				float num = (this.jumpStartTime - Time.time) / this.jumpAnticipationTime * 0.25f + 0.75f;
				float num2 = Mathf.Sqrt(1f / num);
				this.visualCore.transform.localScale = new Vector3(num2, num, num2);
				this.visualCore.transform.position = this.visualCore.parent.position - normalized * (1f - num) * this.radius;
				this.visualCore.transform.rotation = Quaternion.FromToRotation(Vector3.up, normalized);
				return;
			}
			this.SetState(GRSentientCore.SentientCoreState.Jumping);
			this.jumpSound.Play(null);
			if (this.visualCore != null)
			{
				this.visualCore.transform.localScale = Vector3.one;
				this.visualCore.transform.localPosition = Vector3.zero;
				this.visualCore.transform.localRotation = Quaternion.identity;
				return;
			}
			break;
		case GRSentientCore.SentientCoreState.Jumping:
		{
			if (this.debugDraw)
			{
				this.DrawJumpPath(Color.yellow);
			}
			float deltaTime = Time.deltaTime;
			Vector3 vector = base.transform.position + this.jumpVelocity * deltaTime;
			Vector3 vector2 = (this.useSurfaceNormalForGravityDirection ? (-this.surfaceNormal) : Vector3.down);
			this.jumpVelocity += vector2 * (this.jumpGravityAccel * deltaTime);
			float magnitude = this.jumpVelocity.magnitude;
			if (magnitude > this.maxSpeed && this.maxSpeed > 0f)
			{
				this.jumpVelocity *= this.maxSpeed / magnitude;
			}
			float magnitude2 = (vector - base.transform.position).magnitude;
			Vector3 vector3 = ((magnitude2 > 0.001f) ? ((vector - base.transform.position) / magnitude2) : Vector3.zero);
			RaycastHit raycastHit;
			if (Physics.SphereCast(new Ray(base.transform.position, vector3), this.radius, out raycastHit, magnitude2, GTPlayer.Instance.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore))
			{
				vector = base.transform.position + vector3 * raycastHit.distance;
				this.surfaceNormal = raycastHit.normal;
				this.SetState(GRSentientCore.SentientCoreState.Awake);
				this.landSound.Play(null);
			}
			base.transform.position = vector;
			return;
		}
		case GRSentientCore.SentientCoreState.Held:
		{
			GRPlayer grplayer = GRPlayer.Get(this.gameEntity.heldByActorNumber);
			if (grplayer != null)
			{
				grplayer.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.TimeChaosExposure, Time.deltaTime);
			}
			this.isPlayingAlert = false;
			return;
		}
		case GRSentientCore.SentientCoreState.HeldAlert:
			if (!this.isPlayingAlert)
			{
				this.isPlayingAlert = true;
				this.alertEnemiesSound.Play(null);
				GRNoiseEventManager.instance.AddNoiseEvent(base.transform.position, this.alertNoiseEventMagnitude, this.enemyAlertDuration);
			}
			if (Time.time - this.localStateStartTime > this.enemyAlertDuration)
			{
				this.SetState(GRSentientCore.SentientCoreState.Held);
				return;
			}
			break;
		case GRSentientCore.SentientCoreState.AttachedToPlayer:
		{
			GRPlayer grplayer2 = GRPlayer.Get(this.gameEntity.snappedByActorNumber);
			if (grplayer2 != null)
			{
				grplayer2.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.TimeChaosExposure, Time.deltaTime);
				return;
			}
			break;
		}
		case GRSentientCore.SentientCoreState.Dropped:
		{
			float deltaTime2 = Time.deltaTime;
			Vector3 vector4 = base.transform.position + this.rb.linearVelocity * deltaTime2;
			float magnitude3 = (vector4 - base.transform.position).magnitude;
			Vector3 vector5 = ((magnitude3 > 0.001f) ? ((vector4 - base.transform.position) / magnitude3) : Vector3.zero);
			RaycastHit raycastHit2;
			if (Physics.SphereCast(new Ray(base.transform.position, vector5), this.radius, out raycastHit2, magnitude3, GTPlayer.Instance.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore))
			{
				vector4 = base.transform.position + vector5 * raycastHit2.distance;
				this.surfaceNormal = raycastHit2.normal;
				base.transform.position = vector4;
				this.rb.isKinematic = true;
				this.SetState(GRSentientCore.SentientCoreState.Awake);
			}
			break;
		}
		}
	}

	// Token: 0x060033F7 RID: 13303 RVA: 0x0011D9F4 File Offset: 0x0011BBF4
	private void SetState(GRSentientCore.SentientCoreState nextState)
	{
		if (this.localState != nextState)
		{
			this.localState = nextState;
			this.localStateStartTime = Time.time;
			if (this.gameEntity.IsAuthority())
			{
				this.gameEntity.RequestState(this.gameEntity.id, (long)nextState);
			}
		}
	}

	// Token: 0x060033F8 RID: 13304 RVA: 0x0011DA44 File Offset: 0x0011BC44
	public void PerformJump(Vector3 startPos, Vector3 normal, Vector3 direction, double jumpNetworkTime)
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (!base.enabled || this.IsSleeping())
		{
			this.WakeUp();
		}
		base.transform.position = startPos;
		float num = Mathf.Clamp((float)(jumpNetworkTime - PhotonNetwork.Time), 0f, this.jumpAnticipationTime);
		this.jumpStartTime = Time.time + num;
		this.jumpDirection = direction;
		this.jumpDirection.Normalize();
		this.jumpStartPosition = startPos;
		this.surfaceNormal = normal;
		this.jumpVelocity = this.jumpDirection * this.jumpSpeed;
		this.SetState(GRSentientCore.SentientCoreState.JumpAnticipation);
	}

	// Token: 0x060033F9 RID: 13305 RVA: 0x0011DAE0 File Offset: 0x0011BCE0
	private void DrawJumpPath(Color pathColor)
	{
		DebugUtil.DrawLine(this.jumpStartPosition, this.jumpStartPosition + this.surfaceNormal * 0.15f, Color.cyan, true);
		float num = 0.016666f;
		int num2 = 100;
		Vector3 vector = this.jumpStartPosition;
		Vector3 vector2 = this.jumpDirection * this.jumpSpeed;
		for (int i = 0; i < num2; i++)
		{
			Vector3 vector3 = vector + vector2 * num;
			vector2 += -this.surfaceNormal * (this.jumpGravityAccel * num);
			float magnitude = (vector3 - vector).magnitude;
			Vector3 vector4 = ((magnitude > 0.001f) ? ((vector3 - vector) / magnitude) : Vector3.zero);
			RaycastHit raycastHit;
			if (Physics.SphereCast(new Ray(vector, vector4), this.radius, out raycastHit, magnitude, GTPlayer.Instance.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore))
			{
				vector3 = raycastHit.point;
				DebugUtil.DrawLine(vector, vector3, pathColor, true);
				DebugUtil.DrawLine(vector3, vector3 + raycastHit.normal * 0.15f, Color.cyan, true);
				DebugUtil.DrawSphere(raycastHit.point, 0.1f, 12, 12, pathColor, true, DebugUtil.Style.Wireframe);
				return;
			}
			DebugUtil.DrawLine(vector, vector3, pathColor, true);
			vector = vector3;
		}
	}

	// Token: 0x060033FA RID: 13306 RVA: 0x0011DC3C File Offset: 0x0011BE3C
	public void AuthorityInitiateJump()
	{
		if (!this.gameEntity.IsAuthority())
		{
			return;
		}
		Vector3 insideUnitSphere = Random.insideUnitSphere;
		if (Vector3.Dot(insideUnitSphere, this.surfaceNormal) > 0.99f)
		{
			insideUnitSphere = new Vector3(this.surfaceNormal.y, this.surfaceNormal.z, this.surfaceNormal.x);
		}
		float num = Random.Range(this.jumpAngleMinMax.x, this.jumpAngleMinMax.y);
		Vector3 vector = Quaternion.AngleAxis(90f - num, Vector3.Cross(this.surfaceNormal, insideUnitSphere)) * this.surfaceNormal;
		vector.Normalize();
		this.SetState(GRSentientCore.SentientCoreState.JumpInitiated);
		this.gameEntity.manager.ghostReactorManager.RequestSentientCorePerformJump(this.gameEntity, base.transform.position, this.surfaceNormal, vector, this.jumpAnticipationTime);
	}

	// Token: 0x04004395 RID: 17301
	public GameEntity gameEntity;

	// Token: 0x04004396 RID: 17302
	public Vector2 jumpAngleMinMax = new Vector2(30f, 60f);

	// Token: 0x04004397 RID: 17303
	public float jumpSpeed = 3f;

	// Token: 0x04004398 RID: 17304
	public float jumpGravityAccel = 10f;

	// Token: 0x04004399 RID: 17305
	public float maxSpeed = 5f;

	// Token: 0x0400439A RID: 17306
	public float radius = 0.14f;

	// Token: 0x0400439B RID: 17307
	public float jumpAnticipationTime = 1f;

	// Token: 0x0400439C RID: 17308
	public float jumpCooldownTime = 2f;

	// Token: 0x0400439D RID: 17309
	public bool useSurfaceNormalForGravityDirection = true;

	// Token: 0x0400439E RID: 17310
	public Vector2 timeRangeBetweenAlerts = new Vector2(7f, 12f);

	// Token: 0x0400439F RID: 17311
	public float timeUntilFirstAlert = 0.5f;

	// Token: 0x040043A0 RID: 17312
	public float alertNoiseEventMagnitude = 1f;

	// Token: 0x040043A1 RID: 17313
	public AbilitySound jumpSound;

	// Token: 0x040043A2 RID: 17314
	public AbilitySound landSound;

	// Token: 0x040043A3 RID: 17315
	public AbilitySound alertEnemiesSound;

	// Token: 0x040043A4 RID: 17316
	public float wakeupRadius = 3f;

	// Token: 0x040043A5 RID: 17317
	public bool debugDraw;

	// Token: 0x040043A6 RID: 17318
	public Transform visualCore;

	// Token: 0x040043A7 RID: 17319
	public ParticleSystem trailFX;

	// Token: 0x040043A8 RID: 17320
	private Vector3 surfaceNormal = Vector3.up;

	// Token: 0x040043A9 RID: 17321
	private Vector3 jumpDirection = Vector3.up;

	// Token: 0x040043AA RID: 17322
	private Vector3 jumpStartPosition;

	// Token: 0x040043AB RID: 17323
	private Vector3 jumpVelocity;

	// Token: 0x040043AC RID: 17324
	private float jumpStartTime;

	// Token: 0x040043AD RID: 17325
	private Rigidbody rb;

	// Token: 0x040043AE RID: 17326
	private float timeUntilNextAlert = 7f;

	// Token: 0x040043AF RID: 17327
	private float enemyAlertDuration = 1f;

	// Token: 0x040043B0 RID: 17328
	private bool isPlayingAlert;

	// Token: 0x040043B1 RID: 17329
	private bool sleepRequested;

	// Token: 0x040043B2 RID: 17330
	[ReadOnly]
	public GRSentientCore.SentientCoreState localState = GRSentientCore.SentientCoreState.Awake;

	// Token: 0x040043B3 RID: 17331
	private float localStateStartTime;

	// Token: 0x020007F1 RID: 2033
	public enum SentientCoreState
	{
		// Token: 0x040043B5 RID: 17333
		Asleep,
		// Token: 0x040043B6 RID: 17334
		Awake,
		// Token: 0x040043B7 RID: 17335
		JumpInitiated,
		// Token: 0x040043B8 RID: 17336
		JumpAnticipation,
		// Token: 0x040043B9 RID: 17337
		Jumping,
		// Token: 0x040043BA RID: 17338
		Held,
		// Token: 0x040043BB RID: 17339
		HeldAlert,
		// Token: 0x040043BC RID: 17340
		AttachedToPlayer,
		// Token: 0x040043BD RID: 17341
		Dropped
	}
}
