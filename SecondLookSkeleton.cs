using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001D0 RID: 464
public class SecondLookSkeleton : MonoBehaviour
{
	// Token: 0x06000C44 RID: 3140 RVA: 0x00042A9C File Offset: 0x00040C9C
	private void Start()
	{
		this.playersSeen = new List<NetPlayer>();
		this.synchValues = base.GetComponent<SecondLookSkeletonSynchValues>();
		this.playerTransform = Camera.main.transform;
		this.tapped = !this.requireTappingToActivate;
		this.localCaught = false;
		this.audioSource = base.GetComponentInChildren<AudioSource>();
		this.spookyGhost.SetActive(false);
		this.angerPointIndex = Random.Range(0, this.angerPoint.Length);
		this.angerPointChangedTime = Time.time;
		this.synchValues.angerPoint = this.angerPointIndex;
		this.spookyGhost.transform.position = this.angerPoint[this.synchValues.angerPoint].position;
		this.spookyGhost.transform.rotation = this.angerPoint[this.synchValues.angerPoint].rotation;
		this.ChangeState(SecondLookSkeleton.GhostState.Unactivated);
		this.rHits = new RaycastHit[20];
		this.lookedAway = false;
		this.firstLookActivated = false;
		this.animator.Play("ArmsOut");
	}

	// Token: 0x06000C45 RID: 3141 RVA: 0x00042BB1 File Offset: 0x00040DB1
	private void Update()
	{
		this.ProcessGhostState();
	}

	// Token: 0x06000C46 RID: 3142 RVA: 0x00042BBC File Offset: 0x00040DBC
	public void ChangeState(SecondLookSkeleton.GhostState newState)
	{
		if (newState == this.currentState)
		{
			return;
		}
		switch (newState)
		{
		case SecondLookSkeleton.GhostState.Unactivated:
			this.spookyGhost.gameObject.SetActive(false);
			this.audioSource.GTStop();
			this.audioSource.loop = false;
			if (this.IsMine())
			{
				this.synchValues.angerPoint = Random.Range(0, this.angerPoint.Length);
				this.angerPointIndex = this.synchValues.angerPoint;
				this.angerPointChangedTime = Time.time;
				this.spookyGhost.transform.position = this.angerPoint[this.angerPointIndex].position;
				this.spookyGhost.transform.rotation = this.angerPoint[this.angerPointIndex].rotation;
			}
			this.currentState = SecondLookSkeleton.GhostState.Unactivated;
			return;
		case SecondLookSkeleton.GhostState.Activated:
			this.currentState = SecondLookSkeleton.GhostState.Activated;
			if (this.tapped)
			{
				GTAudioSourceExtensions.GTPlayClipAtPoint(this.initialScream, this.audioSource.transform.position, 1f);
				if (this.spookyText != null)
				{
					this.spookyText.SetActive(true);
				}
				this.spookyGhost.SetActive(true);
			}
			this.animator.Play("ArmsOut");
			this.spookyGhost.transform.rotation = Quaternion.LookRotation(this.playerTransform.position - this.spookyGhost.transform.position, Vector3.up);
			if (this.IsMine())
			{
				this.timeFirstAppeared = Time.time;
				return;
			}
			break;
		case SecondLookSkeleton.GhostState.Patrolling:
			this.playersSeen.Clear();
			if (this.tapped)
			{
				this.spookyGhost.SetActive(true);
				this.animator.Play("CrawlPatrol");
				this.audioSource.loop = true;
				this.audioSource.clip = this.patrolLoop;
				this.audioSource.GTPlay();
			}
			if (this.IsMine())
			{
				this.currentNode = this.pathPoints[Random.Range(0, this.pathPoints.Length)];
				this.nextNode = this.currentNode.connectedNodes[Random.Range(0, this.currentNode.connectedNodes.Length)];
				this.SyncNodes();
				this.spookyGhost.transform.position = this.currentNode.transform.position;
			}
			this.currentState = SecondLookSkeleton.GhostState.Patrolling;
			return;
		case SecondLookSkeleton.GhostState.Chasing:
			this.currentState = SecondLookSkeleton.GhostState.Chasing;
			this.resetChaseHistory.Clear();
			this.animator.Play("CrawlChase");
			this.localThrown = false;
			this.localCaught = false;
			if (this.tapped)
			{
				this.audioSource.clip = this.chaseLoop;
				this.audioSource.loop = true;
				this.audioSource.GTPlay();
				return;
			}
			break;
		case SecondLookSkeleton.GhostState.CaughtPlayer:
			this.currentState = SecondLookSkeleton.GhostState.CaughtPlayer;
			this.heightOffset.localPosition = Vector3.zero;
			if (this.tapped)
			{
				this.audioSource.GTPlayOneShot(this.grabbedSound, 1f);
				this.audioSource.loop = true;
				this.audioSource.clip = this.carryingLoop;
				this.audioSource.GTPlay();
				this.animator.Play("ArmsOut");
			}
			if (!this.IsMine())
			{
				this.SetNodes();
				return;
			}
			break;
		case SecondLookSkeleton.GhostState.PlayerThrown:
			this.currentState = SecondLookSkeleton.GhostState.PlayerThrown;
			this.timeThrown = Time.time;
			this.localThrown = false;
			break;
		case SecondLookSkeleton.GhostState.Reset:
			break;
		default:
			return;
		}
	}

	// Token: 0x06000C47 RID: 3143 RVA: 0x00042F24 File Offset: 0x00041124
	private void ProcessGhostState()
	{
		if (this.IsMine())
		{
			switch (this.currentState)
			{
			case SecondLookSkeleton.GhostState.Unactivated:
				if (this.changeAngerPointOnTimeInterval && Time.time - this.angerPointChangedTime > this.changeAngerPointTimeMinutes * 60f)
				{
					this.synchValues.angerPoint = Random.Range(0, this.angerPoint.Length);
					this.angerPointIndex = this.synchValues.angerPoint;
					this.angerPointChangedTime = Time.time;
				}
				this.spookyGhost.transform.position = this.angerPoint[this.angerPointIndex].position;
				this.spookyGhost.transform.rotation = this.angerPoint[this.angerPointIndex].rotation;
				this.CheckActivateGhost();
				return;
			case SecondLookSkeleton.GhostState.Activated:
				if (Time.time > this.timeFirstAppeared + this.timeToFirstDisappear)
				{
					this.ChangeState(SecondLookSkeleton.GhostState.Patrolling);
					return;
				}
				break;
			case SecondLookSkeleton.GhostState.Patrolling:
				if (!this.CheckPlayerSeen() && this.playersSeen.Count == 0)
				{
					this.PatrolMove();
					return;
				}
				this.StartChasing();
				return;
			case SecondLookSkeleton.GhostState.Chasing:
				if (!this.CheckPlayerSeen() || !this.CanGrab())
				{
					this.ChaseMove();
					return;
				}
				this.GrabPlayer();
				return;
			case SecondLookSkeleton.GhostState.CaughtPlayer:
				this.CaughtPlayerUpdate();
				return;
			case SecondLookSkeleton.GhostState.PlayerThrown:
				if (Time.time > this.timeThrown + this.timeThrownCooldown)
				{
					this.ChangeState(SecondLookSkeleton.GhostState.Unactivated);
				}
				break;
			case SecondLookSkeleton.GhostState.Reset:
				break;
			default:
				return;
			}
			return;
		}
		this.SetTappedState();
		switch (this.currentState)
		{
		case SecondLookSkeleton.GhostState.Unactivated:
			this.SetNodes();
			this.spookyGhost.transform.position = this.angerPoint[this.angerPointIndex].position;
			this.spookyGhost.transform.rotation = this.angerPoint[this.angerPointIndex].rotation;
			this.CheckActivateGhost();
			return;
		case SecondLookSkeleton.GhostState.Activated:
			this.FollowPosition();
			return;
		case SecondLookSkeleton.GhostState.Patrolling:
			this.FollowPosition();
			this.CheckPlayerSeen();
			return;
		case SecondLookSkeleton.GhostState.Chasing:
			if (this.CheckPlayerSeen() && this.CanGrab())
			{
				this.GrabPlayer();
			}
			this.FollowPosition();
			return;
		case SecondLookSkeleton.GhostState.CaughtPlayer:
		case SecondLookSkeleton.GhostState.PlayerThrown:
			this.CaughtPlayerUpdate();
			break;
		case SecondLookSkeleton.GhostState.Reset:
			break;
		default:
			return;
		}
	}

	// Token: 0x06000C48 RID: 3144 RVA: 0x00043144 File Offset: 0x00041344
	private void CaughtPlayerUpdate()
	{
		if (this.localThrown)
		{
			return;
		}
		if (this.GhostAtExit())
		{
			if (this.localCaught)
			{
				this.ChuckPlayer();
			}
			if (this.IsMine())
			{
				this.DeactivateGhost();
			}
			return;
		}
		this.CaughtMove();
		if (this.localCaught)
		{
			this.FloatPlayer();
			return;
		}
		if (this.CheckPlayerSeen() && this.CanGrab())
		{
			this.localCaught = true;
		}
	}

	// Token: 0x06000C49 RID: 3145 RVA: 0x000431AC File Offset: 0x000413AC
	private void SetTappedState()
	{
		if (!this.tapped)
		{
			return;
		}
		if (this.spookyText != null && !this.spookyText.activeSelf)
		{
			this.spookyText.SetActive(true);
		}
		if (this.spookyGhost.activeSelf && this.currentState != SecondLookSkeleton.GhostState.Unactivated)
		{
			return;
		}
		this.spookyGhost.SetActive(true);
		switch (this.currentState)
		{
		case SecondLookSkeleton.GhostState.Unactivated:
			this.spookyGhost.SetActive(false);
			return;
		case SecondLookSkeleton.GhostState.Activated:
			this.animator.Play("ArmsOut");
			return;
		case SecondLookSkeleton.GhostState.Patrolling:
			this.animator.Play("CrawlPatrol");
			this.audioSource.loop = true;
			this.audioSource.clip = this.patrolLoop;
			this.audioSource.GTPlay();
			return;
		case SecondLookSkeleton.GhostState.Chasing:
			this.audioSource.clip = this.chaseLoop;
			this.audioSource.loop = true;
			this.audioSource.GTPlay();
			this.animator.Play("CrawlChase");
			this.spookyGhost.SetActive(true);
			return;
		case SecondLookSkeleton.GhostState.CaughtPlayer:
			this.audioSource.GTPlayOneShot(this.grabbedSound, 1f);
			this.audioSource.loop = true;
			this.audioSource.clip = this.carryingLoop;
			this.audioSource.GTPlay();
			this.animator.Play("ArmsOut");
			break;
		case SecondLookSkeleton.GhostState.PlayerThrown:
			this.animator.Play("ArmsOut");
			return;
		case SecondLookSkeleton.GhostState.Reset:
			break;
		default:
			return;
		}
	}

	// Token: 0x06000C4A RID: 3146 RVA: 0x00043330 File Offset: 0x00041530
	private void FollowPosition()
	{
		this.spookyGhost.transform.position = Vector3.Lerp(this.spookyGhost.transform.position, this.synchValues.position, 0.66f);
		this.spookyGhost.transform.rotation = Quaternion.Lerp(this.spookyGhost.transform.rotation, this.synchValues.rotation, 0.66f);
		if (this.currentState == SecondLookSkeleton.GhostState.Patrolling || this.currentState == SecondLookSkeleton.GhostState.Chasing)
		{
			this.SetHeightOffset();
			return;
		}
		this.heightOffset.localPosition = Vector3.zero;
	}

	// Token: 0x06000C4B RID: 3147 RVA: 0x000433D0 File Offset: 0x000415D0
	private void CheckActivateGhost()
	{
		if (!this.tapped || this.currentState != SecondLookSkeleton.GhostState.Unactivated || this.playerTransform == null)
		{
			return;
		}
		this.currentlyLooking = this.IsCurrentlyLooking();
		if (this.requireSecondLookToActivate)
		{
			if (!this.firstLookActivated && this.currentlyLooking)
			{
				this.firstLookActivated = this.currentlyLooking;
				return;
			}
			if (this.firstLookActivated && !this.currentlyLooking)
			{
				this.lookedAway = true;
				return;
			}
			if (this.firstLookActivated && this.lookedAway && this.currentlyLooking)
			{
				this.firstLookActivated = false;
				this.lookedAway = false;
				this.ActivateGhost();
				return;
			}
		}
		else if (this.currentlyLooking)
		{
			this.ActivateGhost();
		}
	}

	// Token: 0x06000C4C RID: 3148 RVA: 0x00043480 File Offset: 0x00041680
	private bool CanSeePlayer()
	{
		return this.CanSeePlayerWithResults(out this.closest);
	}

	// Token: 0x06000C4D RID: 3149 RVA: 0x00043490 File Offset: 0x00041690
	private bool CanSeePlayerWithResults(out RaycastHit closest)
	{
		Vector3 vector = this.playerTransform.position - this.lookSource.position;
		int num = Physics.RaycastNonAlloc(this.lookSource.position, vector.normalized, this.rHits, this.maxSeeDistance, this.mask, QueryTriggerInteraction.Ignore);
		closest = this.rHits[0];
		if (num == 0)
		{
			return false;
		}
		for (int i = 0; i < num; i++)
		{
			if (closest.distance > this.rHits[i].distance)
			{
				closest = this.rHits[i];
			}
		}
		return (this.playerMask & (1 << closest.collider.gameObject.layer)) != 0;
	}

	// Token: 0x06000C4E RID: 3150 RVA: 0x0004355B File Offset: 0x0004175B
	private void ActivateGhost()
	{
		if (this.IsMine())
		{
			this.ChangeState(SecondLookSkeleton.GhostState.Activated);
			return;
		}
		this.synchValues.SendRPC("RemoteActivateGhost", RpcTarget.MasterClient, Array.Empty<object>());
	}

	// Token: 0x06000C4F RID: 3151 RVA: 0x00043583 File Offset: 0x00041783
	private void StartChasing()
	{
		if (!this.IsMine())
		{
			return;
		}
		this.ChangeState(SecondLookSkeleton.GhostState.Chasing);
	}

	// Token: 0x06000C50 RID: 3152 RVA: 0x00043598 File Offset: 0x00041798
	private bool CheckPlayerSeen()
	{
		if (!this.tapped)
		{
			return false;
		}
		if (this.playersSeen.Contains(NetworkSystem.Instance.LocalPlayer))
		{
			return true;
		}
		if (!this.CanSeePlayer())
		{
			return false;
		}
		if (NetworkSystem.Instance.InRoom)
		{
			this.synchValues.SendRPC("RemotePlayerSeen", RpcTarget.Others, Array.Empty<object>());
		}
		this.playersSeen.Add(NetworkSystem.Instance.LocalPlayer);
		return true;
	}

	// Token: 0x06000C51 RID: 3153 RVA: 0x0004360A File Offset: 0x0004180A
	public void RemoteActivateGhost()
	{
		if (this.IsMine() && this.currentState == SecondLookSkeleton.GhostState.Unactivated)
		{
			this.ActivateGhost();
		}
	}

	// Token: 0x06000C52 RID: 3154 RVA: 0x00043622 File Offset: 0x00041822
	public void RemotePlayerSeen(NetPlayer player)
	{
		if (this.IsMine() && !this.playersSeen.Contains(player))
		{
			this.playersSeen.Add(player);
		}
	}

	// Token: 0x06000C53 RID: 3155 RVA: 0x00043648 File Offset: 0x00041848
	public void RemotePlayerCaught(NetPlayer player)
	{
		if (this.IsMine() && this.currentState == SecondLookSkeleton.GhostState.Chasing)
		{
			RigContainer rigContainer;
			VRRigCache.Instance.TryGetVrrig(player, out rigContainer);
			if (rigContainer != null && this.playersSeen.Contains(player))
			{
				this.ChangeState(SecondLookSkeleton.GhostState.CaughtPlayer);
			}
		}
	}

	// Token: 0x06000C54 RID: 3156 RVA: 0x00043694 File Offset: 0x00041894
	private bool IsCurrentlyLooking()
	{
		return Vector3.Dot(this.playerTransform.forward, -this.spookyGhost.transform.forward) > 0f && (this.spookyGhost.transform.position - this.playerTransform.position).magnitude < this.ghostActivationDistance && this.CanSeePlayer();
	}

	// Token: 0x06000C55 RID: 3157 RVA: 0x00043705 File Offset: 0x00041905
	private void PatrolMove()
	{
		this.GhostMove(this.nextNode.transform, this.patrolSpeed);
		this.SetHeightOffset();
		this.CheckReachedNextNode(false, false);
	}

	// Token: 0x06000C56 RID: 3158 RVA: 0x0004372C File Offset: 0x0004192C
	private void CheckReachedNextNode(bool forChuck, bool forChase)
	{
		if ((this.nextNode.transform.position - this.spookyGhost.transform.position).magnitude < this.reachNodeDist)
		{
			if (this.nextNode.connectedNodes.Length == 1)
			{
				this.currentNode = this.nextNode;
				this.nextNode = this.nextNode.connectedNodes[0];
				this.SyncNodes();
				return;
			}
			if (forChuck)
			{
				float num = this.nextNode.distanceToExitNode;
				SkeletonPathingNode skeletonPathingNode = this.nextNode.connectedNodes[0];
				for (int i = 0; i < this.nextNode.connectedNodes.Length; i++)
				{
					if (this.nextNode.connectedNodes[i].distanceToExitNode <= num)
					{
						skeletonPathingNode = this.nextNode.connectedNodes[i];
						num = skeletonPathingNode.distanceToExitNode;
					}
				}
				this.currentNode = this.nextNode;
				this.nextNode = skeletonPathingNode;
				this.SyncNodes();
				return;
			}
			if (forChase)
			{
				float num2 = float.MaxValue;
				float num3 = num2;
				RigContainer rigContainer = GorillaTagger.Instance.offlineVRRig.rigContainer;
				RigContainer rigContainer2 = rigContainer;
				for (int j = 0; j < this.playersSeen.Count; j++)
				{
					VRRigCache.Instance.TryGetVrrig(this.playersSeen[j], out rigContainer);
					if (!(rigContainer == null))
					{
						num2 = (rigContainer.transform.position - this.nextNode.transform.position).sqrMagnitude;
						if (num2 < num3)
						{
							rigContainer2 = rigContainer;
							num3 = num2;
						}
					}
				}
				Vector3 vector = rigContainer2.transform.position - this.nextNode.transform.position;
				SkeletonPathingNode skeletonPathingNode2 = this.nextNode.connectedNodes[0];
				num3 = 0f;
				for (int k = 0; k < this.nextNode.connectedNodes.Length; k++)
				{
					Vector3 vector2 = this.nextNode.connectedNodes[k].transform.position - this.nextNode.transform.position;
					num2 = Mathf.Sign(Vector3.Dot(vector, vector2)) * Vector3.Project(vector, vector2).sqrMagnitude;
					if (num2 >= num3)
					{
						skeletonPathingNode2 = this.nextNode.connectedNodes[k];
						num3 = num2;
					}
				}
				this.currentNode = this.nextNode;
				this.nextNode = skeletonPathingNode2;
				this.SyncNodes();
				this.resetChaseHistory.Add(this.nextNode);
				if (this.resetChaseHistory.Count > 8)
				{
					this.resetChaseHistory.RemoveAt(0);
				}
				if (this.resetChaseHistory.Count >= 8 && this.resetChaseHistory[0] == this.resetChaseHistory[2] == this.resetChaseHistory[4] == this.resetChaseHistory[6] && this.resetChaseHistory[1] == this.resetChaseHistory[3] == this.resetChaseHistory[5] == this.resetChaseHistory[7])
				{
					this.resetChaseHistory.Clear();
					this.ChangeState(SecondLookSkeleton.GhostState.Patrolling);
				}
				return;
			}
			SkeletonPathingNode skeletonPathingNode3 = this.nextNode.connectedNodes[Random.Range(0, this.nextNode.connectedNodes.Length)];
			for (int l = 0; l < 10; l++)
			{
				skeletonPathingNode3 = this.nextNode.connectedNodes[Random.Range(0, this.nextNode.connectedNodes.Length)];
				if (!skeletonPathingNode3.ejectionPoint && skeletonPathingNode3 != this.currentNode)
				{
					break;
				}
			}
			this.currentNode = this.nextNode;
			this.nextNode = skeletonPathingNode3;
			this.SyncNodes();
		}
	}

	// Token: 0x06000C57 RID: 3159 RVA: 0x00043AF6 File Offset: 0x00041CF6
	private void ChaseMove()
	{
		this.GhostMove(this.nextNode.transform, this.chaseSpeed);
		this.SetHeightOffset();
		this.CheckReachedNextNode(false, true);
	}

	// Token: 0x06000C58 RID: 3160 RVA: 0x00043B1D File Offset: 0x00041D1D
	private void CaughtMove()
	{
		this.GhostMove(this.nextNode.transform, this.caughtSpeed);
		this.CheckReachedNextNode(true, false);
		this.SyncNodes();
	}

	// Token: 0x06000C59 RID: 3161 RVA: 0x00043B44 File Offset: 0x00041D44
	private void SyncNodes()
	{
		this.synchValues.currentNode = this.pathPoints.IndexOfRef(this.currentNode);
		this.synchValues.nextNode = this.pathPoints.IndexOfRef(this.nextNode);
		this.synchValues.angerPoint = this.angerPointIndex;
	}

	// Token: 0x06000C5A RID: 3162 RVA: 0x00043B9C File Offset: 0x00041D9C
	public void SetNodes()
	{
		if (this.synchValues.currentNode > this.pathPoints.Length || this.synchValues.currentNode < 0)
		{
			return;
		}
		this.currentNode = this.pathPoints[this.synchValues.currentNode];
		this.nextNode = this.pathPoints[this.synchValues.nextNode];
		this.angerPointIndex = this.synchValues.angerPoint;
	}

	// Token: 0x06000C5B RID: 3163 RVA: 0x00043C10 File Offset: 0x00041E10
	private bool GhostAtExit()
	{
		return this.currentNode.distanceToExitNode == 0f && (this.spookyGhost.transform.position - this.currentNode.transform.position).magnitude < this.reachNodeDist;
	}

	// Token: 0x06000C5C RID: 3164 RVA: 0x00043C68 File Offset: 0x00041E68
	private void GhostMove(Transform target, float speed)
	{
		this.spookyGhost.transform.rotation = Quaternion.RotateTowards(this.spookyGhost.transform.rotation, Quaternion.LookRotation(target.position - this.spookyGhost.transform.position, Vector3.up), this.maxRotSpeed * Time.deltaTime);
		this.spookyGhost.transform.position += (target.position - this.spookyGhost.transform.position).normalized * speed * Time.deltaTime;
	}

	// Token: 0x06000C5D RID: 3165 RVA: 0x00043D19 File Offset: 0x00041F19
	private void DeactivateGhost()
	{
		this.ChangeState(SecondLookSkeleton.GhostState.PlayerThrown);
	}

	// Token: 0x06000C5E RID: 3166 RVA: 0x00043D24 File Offset: 0x00041F24
	private bool CanGrab()
	{
		return (this.spookyGhost.transform.position - this.playerTransform.position).magnitude < this.catchDistance;
	}

	// Token: 0x06000C5F RID: 3167 RVA: 0x00043D61 File Offset: 0x00041F61
	private void GrabPlayer()
	{
		if (this.IsMine())
		{
			if (this.currentState == SecondLookSkeleton.GhostState.Chasing)
			{
				this.ChangeState(SecondLookSkeleton.GhostState.CaughtPlayer);
			}
			this.localCaught = true;
		}
		this.synchValues.SendRPC("RemotePlayerCaught", RpcTarget.MasterClient, Array.Empty<object>());
	}

	// Token: 0x06000C60 RID: 3168 RVA: 0x00043D98 File Offset: 0x00041F98
	private void FloatPlayer()
	{
		RaycastHit raycastHit;
		if (this.CanSeePlayerWithResults(out raycastHit))
		{
			GorillaTagger.Instance.rigidbody.MovePosition(Vector3.MoveTowards(GorillaTagger.Instance.rigidbody.position, this.spookyGhost.transform.position + this.spookyGhost.transform.rotation * this.offsetGrabPosition, this.caughtSpeed * 10f * Time.deltaTime));
		}
		else
		{
			Vector3 vector = raycastHit.point - this.playerTransform.position;
			vector += GTPlayer.Instance.headCollider.radius * 1.05f * vector.normalized;
			GorillaTagger.Instance.transform.parent.position += vector;
			GTPlayer.Instance.InitializeValues();
		}
		GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
		EquipmentInteractor.instance.ForceStopClimbing();
		GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, 0.25f);
		GorillaTagger.Instance.StartVibration(true, this.hapticStrength / 4f, Time.deltaTime);
		GorillaTagger.Instance.StartVibration(false, this.hapticStrength / 4f, Time.deltaTime);
	}

	// Token: 0x06000C61 RID: 3169 RVA: 0x00043EE8 File Offset: 0x000420E8
	private void ChuckPlayer()
	{
		this.localCaught = false;
		this.localThrown = true;
		Vector3 vector = this.currentNode.transform.position - this.currentNode.connectedNodes[0].transform.position;
		GorillaTagger instance = GorillaTagger.Instance;
		Rigidbody rigidbody = ((instance != null) ? instance.rigidbody : null);
		GTAudioSourceExtensions.GTPlayClipAtPoint(this.throwSound, this.audioSource.transform.position, 0.25f);
		this.audioSource.GTStop();
		this.audioSource.loop = false;
		if (rigidbody != null)
		{
			rigidbody.linearVelocity = vector.normalized * this.throwForce;
		}
	}

	// Token: 0x06000C62 RID: 3170 RVA: 0x00043F9C File Offset: 0x0004219C
	private void SetHeightOffset()
	{
		int num = Physics.RaycastNonAlloc(this.spookyGhost.transform.position + Vector3.up * this.bodyHeightOffset, Vector3.down, this.rHits, this.maxSeeDistance, this.mask, QueryTriggerInteraction.Ignore);
		if (num == 0)
		{
			this.heightOffset.localPosition = Vector3.zero;
			return;
		}
		RaycastHit raycastHit = this.rHits[0];
		for (int i = 0; i < num; i++)
		{
			if (raycastHit.distance < this.rHits[i].distance)
			{
				raycastHit = this.rHits[i];
			}
		}
		this.heightOffset.localPosition = new Vector3(0f, -raycastHit.distance, 0f);
	}

	// Token: 0x06000C63 RID: 3171 RVA: 0x00044067 File Offset: 0x00042267
	private bool IsMine()
	{
		return !NetworkSystem.Instance.InRoom || this.synchValues.IsMine;
	}

	// Token: 0x04000ED7 RID: 3799
	public Transform[] angerPoint;

	// Token: 0x04000ED8 RID: 3800
	public int angerPointIndex;

	// Token: 0x04000ED9 RID: 3801
	public SkeletonPathingNode[] pathPoints;

	// Token: 0x04000EDA RID: 3802
	public SkeletonPathingNode[] exitPoints;

	// Token: 0x04000EDB RID: 3803
	public Transform heightOffset;

	// Token: 0x04000EDC RID: 3804
	public bool requireSecondLookToActivate;

	// Token: 0x04000EDD RID: 3805
	public bool requireTappingToActivate;

	// Token: 0x04000EDE RID: 3806
	public bool changeAngerPointOnTimeInterval;

	// Token: 0x04000EDF RID: 3807
	public float changeAngerPointTimeMinutes = 3f;

	// Token: 0x04000EE0 RID: 3808
	private bool firstLookActivated;

	// Token: 0x04000EE1 RID: 3809
	private bool lookedAway;

	// Token: 0x04000EE2 RID: 3810
	private bool currentlyLooking;

	// Token: 0x04000EE3 RID: 3811
	public float ghostActivationDistance;

	// Token: 0x04000EE4 RID: 3812
	public GameObject spookyGhost;

	// Token: 0x04000EE5 RID: 3813
	public float timeFirstAppeared;

	// Token: 0x04000EE6 RID: 3814
	public float timeToFirstDisappear;

	// Token: 0x04000EE7 RID: 3815
	public SecondLookSkeleton.GhostState currentState;

	// Token: 0x04000EE8 RID: 3816
	public GameObject spookyText;

	// Token: 0x04000EE9 RID: 3817
	public float patrolSpeed;

	// Token: 0x04000EEA RID: 3818
	public float chaseSpeed;

	// Token: 0x04000EEB RID: 3819
	public float caughtSpeed;

	// Token: 0x04000EEC RID: 3820
	public SkeletonPathingNode firstNode;

	// Token: 0x04000EED RID: 3821
	public SkeletonPathingNode currentNode;

	// Token: 0x04000EEE RID: 3822
	public SkeletonPathingNode nextNode;

	// Token: 0x04000EEF RID: 3823
	public Transform lookSource;

	// Token: 0x04000EF0 RID: 3824
	private Transform playerTransform;

	// Token: 0x04000EF1 RID: 3825
	public float reachNodeDist;

	// Token: 0x04000EF2 RID: 3826
	public float maxRotSpeed;

	// Token: 0x04000EF3 RID: 3827
	public float hapticStrength;

	// Token: 0x04000EF4 RID: 3828
	public float hapticDuration;

	// Token: 0x04000EF5 RID: 3829
	public Vector3 offsetGrabPosition;

	// Token: 0x04000EF6 RID: 3830
	public float throwForce;

	// Token: 0x04000EF7 RID: 3831
	public Animator animator;

	// Token: 0x04000EF8 RID: 3832
	public float bodyHeightOffset;

	// Token: 0x04000EF9 RID: 3833
	private float timeThrown;

	// Token: 0x04000EFA RID: 3834
	public float timeThrownCooldown = 1f;

	// Token: 0x04000EFB RID: 3835
	public float catchDistance;

	// Token: 0x04000EFC RID: 3836
	public float maxSeeDistance;

	// Token: 0x04000EFD RID: 3837
	private RaycastHit[] rHits;

	// Token: 0x04000EFE RID: 3838
	public LayerMask mask;

	// Token: 0x04000EFF RID: 3839
	public LayerMask playerMask;

	// Token: 0x04000F00 RID: 3840
	public AudioSource audioSource;

	// Token: 0x04000F01 RID: 3841
	public AudioClip initialScream;

	// Token: 0x04000F02 RID: 3842
	public AudioClip patrolLoop;

	// Token: 0x04000F03 RID: 3843
	public AudioClip chaseLoop;

	// Token: 0x04000F04 RID: 3844
	public AudioClip grabbedSound;

	// Token: 0x04000F05 RID: 3845
	public AudioClip carryingLoop;

	// Token: 0x04000F06 RID: 3846
	public AudioClip throwSound;

	// Token: 0x04000F07 RID: 3847
	public List<SkeletonPathingNode> resetChaseHistory = new List<SkeletonPathingNode>();

	// Token: 0x04000F08 RID: 3848
	private SecondLookSkeletonSynchValues synchValues;

	// Token: 0x04000F09 RID: 3849
	private bool localCaught;

	// Token: 0x04000F0A RID: 3850
	private bool localThrown;

	// Token: 0x04000F0B RID: 3851
	public List<NetPlayer> playersSeen;

	// Token: 0x04000F0C RID: 3852
	public bool tapped;

	// Token: 0x04000F0D RID: 3853
	private RaycastHit closest;

	// Token: 0x04000F0E RID: 3854
	private float angerPointChangedTime;

	// Token: 0x020001D1 RID: 465
	public enum GhostState
	{
		// Token: 0x04000F10 RID: 3856
		Unactivated,
		// Token: 0x04000F11 RID: 3857
		Activated,
		// Token: 0x04000F12 RID: 3858
		Patrolling,
		// Token: 0x04000F13 RID: 3859
		Chasing,
		// Token: 0x04000F14 RID: 3860
		CaughtPlayer,
		// Token: 0x04000F15 RID: 3861
		PlayerThrown,
		// Token: 0x04000F16 RID: 3862
		Reset
	}
}
