using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000607 RID: 1543
public class MonkeBall : MonoBehaviourTick
{
	// Token: 0x06002670 RID: 9840 RVA: 0x000CB79C File Offset: 0x000C999C
	private void Start()
	{
		this.Refresh();
	}

	// Token: 0x06002671 RID: 9841 RVA: 0x000CB7A4 File Offset: 0x000C99A4
	public override void Tick()
	{
		this.UpdateVisualOffset();
		if (!PhotonNetwork.IsMasterClient)
		{
			if (this._resyncPosition)
			{
				this._resyncDelay -= Time.deltaTime;
				if (this._resyncDelay <= 0f)
				{
					this._resyncPosition = false;
					GameBallManager.Instance.RequestSetBallPosition(this.gameBall.id);
				}
			}
			if (this._positionFailsafe)
			{
				if (base.transform.position.y < -500f || (GameBallManager.Instance.transform.position - base.transform.position).sqrMagnitude > 6400f)
				{
					if (PhotonNetwork.IsConnected)
					{
						GameBallManager.Instance.RequestSetBallPosition(this.gameBall.id);
					}
					else
					{
						base.transform.position = GameBallManager.Instance.transform.position;
					}
					this._positionFailsafe = false;
					this._positionFailsafeTimer = 3f;
					return;
				}
			}
			else
			{
				this._positionFailsafeTimer -= Time.deltaTime;
				if (this._positionFailsafeTimer <= 0f)
				{
					this._positionFailsafe = true;
				}
			}
			return;
		}
		if (this.gameBall.onlyGrabTeamId != -1 && Time.timeAsDouble >= this.restrictTeamGrabEndTime)
		{
			MonkeBallGame.Instance.RequestRestrictBallToTeam(this.gameBall.id, -1);
		}
		if (this.AlreadyDropped())
		{
			this._droppedTimer += Time.deltaTime;
			if (this._droppedTimer >= 7.5f)
			{
				this._droppedTimer = 0f;
				GameBallManager.Instance.RequestTeleportBall(this.gameBall.id, base.transform.position, base.transform.rotation, this._rigidBody.linearVelocity, this._rigidBody.angularVelocity);
			}
		}
		if (this._justGrabbed)
		{
			this._justGrabbedTimer -= Time.deltaTime;
			if (this._justGrabbedTimer <= 0f)
			{
				this._justGrabbed = false;
			}
		}
		if (this._resyncPosition)
		{
			this._resyncDelay -= Time.deltaTime;
			if (this._resyncDelay <= 0f)
			{
				this._resyncPosition = false;
				GameBallManager.Instance.RequestTeleportBall(this.gameBall.id, base.transform.position, base.transform.rotation, this._rigidBody.linearVelocity, this._rigidBody.angularVelocity);
			}
		}
		if (this._positionFailsafe)
		{
			if (base.transform.position.y < -250f || (GameBallManager.Instance.transform.position - base.transform.position).sqrMagnitude > 6400f)
			{
				MonkeBallGame.Instance.LaunchBallNeutral(this.gameBall.id);
				this._positionFailsafe = false;
				this._positionFailsafeTimer = 3f;
				return;
			}
		}
		else
		{
			this._positionFailsafeTimer -= Time.deltaTime;
			if (this._positionFailsafeTimer <= 0f)
			{
				this._positionFailsafe = true;
			}
		}
	}

	// Token: 0x06002672 RID: 9842 RVA: 0x000CBAB0 File Offset: 0x000C9CB0
	public void OnCollisionEnter(Collision collision)
	{
		if (this.AlreadyDropped() || this._justGrabbed)
		{
			return;
		}
		if (MonkeBall.IsGamePlayer(collision.collider))
		{
			return;
		}
		this.alreadyDropped = true;
		this._droppedTimer = 0f;
		this.gameBall.PlayBounceFX();
		if (!PhotonNetwork.IsMasterClient)
		{
			if (this._rigidBody.linearVelocity.sqrMagnitude > 1f)
			{
				this._resyncPosition = true;
				this._resyncDelay = 1.5f;
			}
			int lastHeldByActorNumber = this.gameBall.lastHeldByActorNumber;
			int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
			return;
		}
		if (this._rigidBody.linearVelocity.sqrMagnitude > 1f)
		{
			this._resyncPosition = true;
			this._resyncDelay = 0.5f;
		}
		if (this._launchAfterScore)
		{
			this._launchAfterScore = false;
			MonkeBallGame.Instance.RequestRestrictBallToTeamOnScore(this.gameBall.id, MonkeBallGame.Instance.GetOtherTeam(this.gameBall.lastHeldByTeamId));
			return;
		}
		MonkeBallGame.Instance.RequestRestrictBallToTeam(this.gameBall.id, MonkeBallGame.Instance.GetOtherTeam(this.gameBall.lastHeldByTeamId));
	}

	// Token: 0x06002673 RID: 9843 RVA: 0x000CBBD7 File Offset: 0x000C9DD7
	public void TriggerDelayedResync()
	{
		this._resyncPosition = true;
		if (PhotonNetwork.IsMasterClient)
		{
			this._resyncDelay = 0.5f;
			return;
		}
		this._resyncDelay = 1.5f;
	}

	// Token: 0x06002674 RID: 9844 RVA: 0x000CBBFE File Offset: 0x000C9DFE
	public void SetRigidbodyDiscrete()
	{
		this._rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
	}

	// Token: 0x06002675 RID: 9845 RVA: 0x000CBC0C File Offset: 0x000C9E0C
	public void SetRigidbodyContinuous()
	{
		this._rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
	}

	// Token: 0x06002676 RID: 9846 RVA: 0x000CBC1A File Offset: 0x000C9E1A
	public static MonkeBall Get(GameBall ball)
	{
		if (ball == null)
		{
			return null;
		}
		return ball.GetComponent<MonkeBall>();
	}

	// Token: 0x06002677 RID: 9847 RVA: 0x000CBC2D File Offset: 0x000C9E2D
	public bool AlreadyDropped()
	{
		return this.alreadyDropped;
	}

	// Token: 0x06002678 RID: 9848 RVA: 0x000CBC35 File Offset: 0x000C9E35
	public void OnGrabbed()
	{
		this.alreadyDropped = false;
		this._justGrabbed = true;
		this._justGrabbedTimer = 0.1f;
		this._resyncPosition = false;
	}

	// Token: 0x06002679 RID: 9849 RVA: 0x000CBC57 File Offset: 0x000C9E57
	public void OnSwitchHeldByTeam(int teamId)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			MonkeBallGame.Instance.RequestRestrictBallToTeam(this.gameBall.id, teamId);
		}
	}

	// Token: 0x0600267A RID: 9850 RVA: 0x000CBC76 File Offset: 0x000C9E76
	public void ClearCannotGrabTeamId()
	{
		this.gameBall.onlyGrabTeamId = -1;
		this.restrictTeamGrabEndTime = -1.0;
		this.Refresh();
	}

	// Token: 0x0600267B RID: 9851 RVA: 0x000CBC9C File Offset: 0x000C9E9C
	public bool RestrictBallToTeam(int teamId, float duration)
	{
		if (teamId == this.gameBall.onlyGrabTeamId && Time.timeAsDouble + (double)duration < this.restrictTeamGrabEndTime)
		{
			return false;
		}
		this.gameBall.onlyGrabTeamId = teamId;
		this.restrictTeamGrabEndTime = Time.timeAsDouble + (double)duration;
		this.Refresh();
		return true;
	}

	// Token: 0x0600267C RID: 9852 RVA: 0x000CBCEA File Offset: 0x000C9EEA
	private void Refresh()
	{
		if (this.gameBall.onlyGrabTeamId == -1)
		{
			this.mainRenderer.material = this.defaultMaterial;
			return;
		}
		this.mainRenderer.material = this.teamMaterial[this.gameBall.onlyGrabTeamId];
	}

	// Token: 0x0600267D RID: 9853 RVA: 0x000CBD29 File Offset: 0x000C9F29
	private static bool IsGamePlayer(Collider collider)
	{
		return GameBallPlayer.GetGamePlayer(collider, false) != null;
	}

	// Token: 0x0600267E RID: 9854 RVA: 0x000CBD38 File Offset: 0x000C9F38
	public void SetVisualOffset(bool detach)
	{
		if (detach)
		{
			this.lastVisiblePosition = this.mainRenderer.transform.position;
			this._visualOffset = true;
			this._timeOffset = Time.time;
			this.mainRenderer.transform.SetParent(null, true);
			return;
		}
		this.ReattachVisuals();
	}

	// Token: 0x0600267F RID: 9855 RVA: 0x000CBD8C File Offset: 0x000C9F8C
	private void ReattachVisuals()
	{
		if (!this._visualOffset)
		{
			return;
		}
		this.mainRenderer.transform.SetParent(base.transform);
		this.mainRenderer.transform.localPosition = Vector3.zero;
		this.mainRenderer.transform.localRotation = Quaternion.identity;
		this._visualOffset = false;
	}

	// Token: 0x06002680 RID: 9856 RVA: 0x000CBDEC File Offset: 0x000C9FEC
	private void UpdateVisualOffset()
	{
		if (this._visualOffset)
		{
			this.mainRenderer.transform.position = Vector3.Lerp(this.mainRenderer.transform.position, this._rigidBody.position, Mathf.Clamp((Time.time - this._timeOffset) / this.maxLerpTime, this.offsetLerp, 1f));
			if ((this.mainRenderer.transform.position - this._rigidBody.position).sqrMagnitude < this._offsetThreshold)
			{
				this.ReattachVisuals();
			}
		}
	}

	// Token: 0x040031EC RID: 12780
	public GameBall gameBall;

	// Token: 0x040031ED RID: 12781
	public MeshRenderer mainRenderer;

	// Token: 0x040031EE RID: 12782
	public Material defaultMaterial;

	// Token: 0x040031EF RID: 12783
	public Material[] teamMaterial;

	// Token: 0x040031F0 RID: 12784
	public double restrictTeamGrabEndTime;

	// Token: 0x040031F1 RID: 12785
	public bool alreadyDropped;

	// Token: 0x040031F2 RID: 12786
	private bool _justGrabbed;

	// Token: 0x040031F3 RID: 12787
	private float _justGrabbedTimer;

	// Token: 0x040031F4 RID: 12788
	private bool _launchAfterScore;

	// Token: 0x040031F5 RID: 12789
	private float _droppedTimer;

	// Token: 0x040031F6 RID: 12790
	private bool _resyncPosition;

	// Token: 0x040031F7 RID: 12791
	private float _resyncDelay;

	// Token: 0x040031F8 RID: 12792
	private bool _visualOffset;

	// Token: 0x040031F9 RID: 12793
	private float _offsetThreshold = 0.05f;

	// Token: 0x040031FA RID: 12794
	private float _timeOffset;

	// Token: 0x040031FB RID: 12795
	public float maxLerpTime = 0.5f;

	// Token: 0x040031FC RID: 12796
	public float offsetLerp = 0.2f;

	// Token: 0x040031FD RID: 12797
	private bool _positionFailsafe = true;

	// Token: 0x040031FE RID: 12798
	private float _positionFailsafeTimer;

	// Token: 0x040031FF RID: 12799
	public Vector3 lastVisiblePosition;

	// Token: 0x04003200 RID: 12800
	[SerializeField]
	private Rigidbody _rigidBody;
}
