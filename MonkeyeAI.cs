using System;
using System.Collections.Generic;
using GorillaLocomotion;
using JetBrains.Annotations;
using Pathfinding;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020001A7 RID: 423
[RequireComponent(typeof(NetworkView))]
public class MonkeyeAI : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06000B59 RID: 2905 RVA: 0x0003C974 File Offset: 0x0003AB74
	private string UserIdFromRig(VRRig rig)
	{
		if (rig == null)
		{
			return "";
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			if (rig == GorillaTagger.Instance.offlineVRRig)
			{
				return "-1";
			}
			Debug.Log("Not in a room but not targeting offline rig");
			return null;
		}
		else
		{
			if (rig == GorillaTagger.Instance.offlineVRRig)
			{
				return NetworkSystem.Instance.LocalPlayer.UserId;
			}
			if (rig.creator == null)
			{
				return "";
			}
			return rig.creator.UserId;
		}
	}

	// Token: 0x06000B5A RID: 2906 RVA: 0x0003C9FC File Offset: 0x0003ABFC
	private VRRig GetRig(string userId)
	{
		if (userId == "")
		{
			return null;
		}
		if (NetworkSystem.Instance.InRoom || !(userId != "-1"))
		{
			foreach (VRRig vrrig in this.GetValidChoosableRigs())
			{
				if (!(vrrig == null))
				{
					NetPlayer creator = vrrig.creator;
					if (creator != null && userId == creator.UserId)
					{
						return vrrig;
					}
				}
			}
			return null;
		}
		if (userId == "-1 " && GorillaTagger.Instance != null)
		{
			return GorillaTagger.Instance.offlineVRRig;
		}
		return null;
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x0003CAC0 File Offset: 0x0003ACC0
	private float Distance2D(Vector3 a, Vector3 b)
	{
		Vector2 vector = new Vector2(a.x, a.z);
		Vector2 vector2 = new Vector2(b.x, b.z);
		return Vector2.Distance(vector, vector2);
	}

	// Token: 0x06000B5C RID: 2908 RVA: 0x0003CAF8 File Offset: 0x0003ACF8
	private Transform PickRandomPatrolPoint()
	{
		int num;
		do
		{
			num = Random.Range(0, this.patrolPts.Count);
		}
		while (num == this.patrolIdx);
		this.patrolIdx = num;
		return this.patrolPts[num];
	}

	// Token: 0x06000B5D RID: 2909 RVA: 0x0003CB38 File Offset: 0x0003AD38
	private void PickNewPath(bool pathFinished = false)
	{
		if (this.calculatingPath)
		{
			return;
		}
		this.currentWaypoint = 0;
		switch (this.replState.state)
		{
		case MonkeyeAI_ReplState.EStates.Patrolling:
			if (this.patrolCount == this.maxPatrols)
			{
				this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
				this.targetPosition = this.PickRandomPatrolPoint().position;
				this.patrolCount = 0;
			}
			else
			{
				this.targetPosition = this.PickRandomPatrolPoint().position;
				this.patrolCount++;
			}
			break;
		case MonkeyeAI_ReplState.EStates.Chasing:
			if (!this.lockedOn)
			{
				Vector3 position = base.transform.position;
				VRRig vrrig;
				if (this.ClosestPlayer(in position, out vrrig) && vrrig != this.targetRig)
				{
					this.SetTargetPlayer(vrrig);
				}
			}
			if (this.targetRig == null)
			{
				this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
				this.targetPosition = this.sleepPt.position;
			}
			else
			{
				this.targetPosition = this.targetRig.transform.position;
			}
			break;
		case MonkeyeAI_ReplState.EStates.ReturnToSleepPt:
			this.targetPosition = this.sleepPt.position;
			break;
		}
		this.calculatingPath = true;
		this.seeker.StartPath(base.transform.position, this.targetPosition, new OnPathDelegate(this.OnPathComplete));
	}

	// Token: 0x06000B5E RID: 2910 RVA: 0x0003CC88 File Offset: 0x0003AE88
	private void Awake()
	{
		this.lazerFx = base.GetComponent<Monkeye_LazerFX>();
		this.animController = base.GetComponent<Animator>();
		this.layerBase = this.animController.GetLayerIndex("Base_Layer");
		this.layerForward = this.animController.GetLayerIndex("MoveFwdAddPose");
		this.layerLeft = this.animController.GetLayerIndex("TurnLAddPose");
		this.layerRight = this.animController.GetLayerIndex("TurnRAddPose");
		this.seeker = base.GetComponent<Seeker>();
		this.renderer = this.portalFx.GetComponent<Renderer>();
		this.portalMatPropBlock = new MaterialPropertyBlock();
		this.monkEyeMatPropBlock = new MaterialPropertyBlock();
		this.layerMask = UnityLayer.Default.ToLayerMask() | UnityLayer.GorillaObject.ToLayerMask();
		this.SetDefaultAttackState();
		this.SetState(MonkeyeAI_ReplState.EStates.Sleeping);
		this.replStateRequestableOwnershipGaurd = this.replState.GetComponent<RequestableOwnershipGuard>();
		this.myRequestableOwnershipGaurd = base.GetComponent<RequestableOwnershipGuard>();
		if (this.monkEyeColor.a != 0f || this.monkEyeEyeColorNormal.a != 0f)
		{
			if (this.monkEyeColor.a != 0f)
			{
				this.monkEyeMatPropBlock.SetVector(MonkeyeAI.ColorShaderProp, this.monkEyeColor);
			}
			if (this.monkEyeEyeColorNormal.a != 0f)
			{
				this.monkEyeMatPropBlock.SetVector(MonkeyeAI.EyeColorShaderProp, this.monkEyeEyeColorNormal);
			}
			this.skinnedMeshRenderer.SetPropertyBlock(this.monkEyeMatPropBlock);
		}
		base.InvokeRepeating("AntiOverlapAssurance", 0.2f, 0.5f);
	}

	// Token: 0x06000B5F RID: 2911 RVA: 0x0003CE1F File Offset: 0x0003B01F
	private void Start()
	{
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
	}

	// Token: 0x06000B60 RID: 2912 RVA: 0x0003CE34 File Offset: 0x0003B034
	private void OnPathComplete(Path path_)
	{
		this.path = path_;
		this.currentWaypoint = 0;
		if (this.path.vectorPath.Count < 1)
		{
			base.transform.position = this.sleepPt.position;
			base.transform.rotation = this.sleepPt.rotation;
			this.path = null;
		}
		this.calculatingPath = false;
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x0003CE9C File Offset: 0x0003B09C
	private void FollowPath()
	{
		if (this.path == null || this.currentWaypoint >= this.path.vectorPath.Count || this.currentWaypoint < 0)
		{
			this.PickNewPath(false);
			if (this.path == null)
			{
				return;
			}
		}
		if (this.Distance2D(base.transform.position, this.path.vectorPath[this.currentWaypoint]) < 0.01f)
		{
			if (this.currentWaypoint + 1 == this.path.vectorPath.Count)
			{
				this.PickNewPath(true);
				return;
			}
			this.currentWaypoint++;
		}
		Vector3 normalized = (this.path.vectorPath[this.currentWaypoint] - base.transform.position).normalized;
		normalized.y = 0f;
		if (this.animController.GetCurrentAnimatorStateInfo(0).IsName("Move"))
		{
			Vector3 vector = normalized * this.speed;
			base.transform.position += vector * this.deltaTime;
		}
		Mathf.Clamp01(Vector3.Dot(base.transform.forward, normalized) / 1.5707964f);
		if (Mathf.Sign(Vector3.Cross(base.transform.forward, normalized).y) > 0f)
		{
			this.animController.SetLayerWeight(this.layerRight, 0f);
		}
		else
		{
			this.animController.SetLayerWeight(this.layerLeft, 0f);
		}
		this.animController.SetLayerWeight(this.layerForward, 0f);
		Vector3 vector2 = Vector3.RotateTowards(base.transform.forward, normalized, this.rotationSpeed * this.deltaTime, 0f);
		base.transform.rotation = Quaternion.LookRotation(vector2);
	}

	// Token: 0x06000B62 RID: 2914 RVA: 0x0003D080 File Offset: 0x0003B280
	private bool PlayerNear(VRRig rig, float dist, out float playerDist)
	{
		if (rig == null)
		{
			playerDist = float.PositiveInfinity;
			return false;
		}
		playerDist = this.Distance2D(rig.transform.position, base.transform.position);
		return playerDist < dist && Physics.RaycastNonAlloc(new Ray(base.transform.position, rig.transform.position - base.transform.position), this.rayResults, playerDist, this.layerMask) <= 0;
	}

	// Token: 0x06000B63 RID: 2915 RVA: 0x0003D110 File Offset: 0x0003B310
	private void Sleeping()
	{
		this.audioSource.volume = Mathf.Min(this.sleepLoopVolume, this.audioSource.volume + this.deltaTime / this.sleepDuration);
		if (this.audioSource.volume == this.sleepLoopVolume)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
			this.PickNewPath(false);
		}
	}

	// Token: 0x06000B64 RID: 2916 RVA: 0x0003D170 File Offset: 0x0003B370
	private bool ClosestPlayer(in Vector3 myPos, out VRRig outRig)
	{
		float num = float.MaxValue;
		outRig = null;
		foreach (VRRig vrrig in this.GetValidChoosableRigs())
		{
			float num2 = 0f;
			if (this.PlayerNear(vrrig, this.chaseDistance, out num2) && num2 < num)
			{
				num = num2;
				outRig = vrrig;
			}
		}
		return num != float.MaxValue;
	}

	// Token: 0x06000B65 RID: 2917 RVA: 0x0003D1F0 File Offset: 0x0003B3F0
	private bool CheckForChase()
	{
		foreach (VRRig vrrig in this.GetValidChoosableRigs())
		{
			float num = 0f;
			if (this.PlayerNear(vrrig, this.wakeDistance, out num))
			{
				this.SetTargetPlayer(vrrig);
				this.SetState(MonkeyeAI_ReplState.EStates.Chasing);
				this.PickNewPath(false);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x0003D270 File Offset: 0x0003B470
	public void SetChasePlayer(VRRig rig)
	{
		if (!this.GetValidChoosableRigs().Contains(rig))
		{
			return;
		}
		this.SetTargetPlayer(rig);
		this.lockedOn = true;
		this.SetState(MonkeyeAI_ReplState.EStates.Chasing);
		this.PickNewPath(false);
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x0003D29D File Offset: 0x0003B49D
	public void SetSleep()
	{
		if (this.replState.state == MonkeyeAI_ReplState.EStates.Patrolling || this.replState.state == MonkeyeAI_ReplState.EStates.Chasing)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Sleeping);
		}
	}

	// Token: 0x06000B68 RID: 2920 RVA: 0x0003D2C4 File Offset: 0x0003B4C4
	private void Patrolling()
	{
		this.audioSource.volume = Mathf.Min(this.patrolLoopVolume, this.audioSource.volume + this.deltaTime / this.patrolLoopFadeInTime);
		if (this.path == null)
		{
			this.PickNewPath(false);
		}
		if (this.audioSource.volume == this.patrolLoopVolume)
		{
			this.CheckForChase();
		}
	}

	// Token: 0x06000B69 RID: 2921 RVA: 0x0003D32C File Offset: 0x0003B52C
	private void Chasing()
	{
		this.audioSource.volume = Mathf.Min(this.chaseLoopVolume, this.audioSource.volume + this.deltaTime / this.chaseLoopFadeInTime);
		this.PickNewPath(false);
		if (this.targetRig == null)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
			return;
		}
		if (this.Distance2D(base.transform.position, this.targetRig.transform.position) < this.attackDistance)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.BeginAttack);
			return;
		}
	}

	// Token: 0x06000B6A RID: 2922 RVA: 0x0003D3B8 File Offset: 0x0003B5B8
	private void ReturnToSleepPt()
	{
		if (this.path == null)
		{
			this.PickNewPath(false);
		}
		if (this.CheckForChase())
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Chasing);
			return;
		}
		if (this.Distance2D(base.transform.position, this.sleepPt.position) < 0.01f)
		{
			this.SetState(MonkeyeAI_ReplState.EStates.Sleeping);
		}
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x0003D410 File Offset: 0x0003B610
	private void UpdateClientState()
	{
		if (this.wasConnectedToRoom && !NetworkSystem.Instance.InRoom)
		{
			this.SetDefaultState();
			return;
		}
		if (ColliderEnabledManager.instance != null && !this.replState.floorEnabled)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				if (this.replState.userId == "-1")
				{
					ColliderEnabledManager.instance.DisableFloorForFrame();
				}
			}
			else if (this.replState.userId == NetworkSystem.Instance.LocalPlayer.UserId)
			{
				ColliderEnabledManager.instance.DisableFloorForFrame();
			}
		}
		if (this.portalFx.activeSelf != this.replState.portalEnabled)
		{
			this.portalFx.SetActive(this.replState.portalEnabled);
		}
		this.portalFx.transform.position = new Vector3(this.replState.attackPos.x, this.portalFx.transform.position.y, this.replState.attackPos.z);
		this.replState.timer -= this.deltaTime;
		if (this.replState.timer < 0f)
		{
			this.replState.timer = 0f;
		}
		VRRig rig = this.GetRig(this.replState.userId);
		if (this.replState.state >= MonkeyeAI_ReplState.EStates.BeginAttack)
		{
			if (rig == null)
			{
				this.lazerFx.DisableLazer();
			}
			else if (this.replState.state < MonkeyeAI_ReplState.EStates.DropPlayer)
			{
				this.lazerFx.EnableLazer(this.eyeBones, rig, 10000f);
			}
			else
			{
				this.lazerFx.DisableLazer();
			}
		}
		else
		{
			this.lazerFx.DisableLazer();
		}
		if (this.replState.portalEnabled)
		{
			this.portalColor.a = this.replState.alpha;
			this.portalMatPropBlock.SetVector(MonkeyeAI.tintColorShaderProp, this.portalColor);
			this.renderer.SetPropertyBlock(this.portalMatPropBlock);
		}
		if (GorillaTagger.Instance.offlineVRRig == rig && this.replState.freezePlayer)
		{
			GTPlayer.Instance.SetMaximumSlipThisFrame();
			Rigidbody rigidbody = GorillaTagger.Instance.rigidbody;
			Vector3 linearVelocity = rigidbody.linearVelocity;
			rigidbody.linearVelocity = new Vector3(linearVelocity.x * this.deltaTime * 4f, Mathf.Min(linearVelocity.y, 0f), linearVelocity.x * this.deltaTime * 4f);
		}
		if (!this.replState.IsMine)
		{
			this.SetClientState(this.replState.state);
		}
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x0003D6BA File Offset: 0x0003B8BA
	private void SetDefaultState()
	{
		this.SetState(MonkeyeAI_ReplState.EStates.Sleeping);
		this.SetDefaultAttackState();
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x0003D6CC File Offset: 0x0003B8CC
	private void SetDefaultAttackState()
	{
		this.replState.floorEnabled = true;
		this.replState.timer = 0f;
		this.replState.userId = "";
		this.replState.attackPos = base.transform.position;
		this.replState.portalEnabled = false;
		this.replState.freezePlayer = false;
		this.replState.alpha = 0f;
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x0003D743 File Offset: 0x0003B943
	private void ExitAttackState()
	{
		this.SetDefaultAttackState();
		this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
	}

	// Token: 0x06000B6F RID: 2927 RVA: 0x0003D754 File Offset: 0x0003B954
	private void BeginAttack()
	{
		this.path = null;
		this.replState.freezePlayer = true;
		if (this.replState.timer <= 0f)
		{
			if (this.audioSource.isActiveAndEnabled)
			{
				this.audioSource.GTPlayOneShot(this.attackSound, this.attackVolume);
			}
			this.replState.timer = this.openFloorTime;
			this.replState.portalEnabled = true;
			this.SetState(MonkeyeAI_ReplState.EStates.OpenFloor);
		}
	}

	// Token: 0x06000B70 RID: 2928 RVA: 0x0003D7D0 File Offset: 0x0003B9D0
	private void OpenFloor()
	{
		this.replState.alpha = Mathf.Lerp(0f, 1f, 1f - Mathf.Clamp01(this.replState.timer / this.openFloorTime));
		if (this.replState.timer <= 0f)
		{
			this.replState.timer = this.dropPlayerTime;
			this.replState.floorEnabled = false;
			this.SetState(MonkeyeAI_ReplState.EStates.DropPlayer);
		}
	}

	// Token: 0x06000B71 RID: 2929 RVA: 0x0003D84A File Offset: 0x0003BA4A
	private void DropPlayer()
	{
		if (this.replState.timer <= 0f)
		{
			this.replState.timer = this.dropPlayerTime;
			this.replState.floorEnabled = true;
			this.SetState(MonkeyeAI_ReplState.EStates.CloseFloor);
		}
	}

	// Token: 0x06000B72 RID: 2930 RVA: 0x0003D882 File Offset: 0x0003BA82
	private void CloseFloor()
	{
		if (this.replState.timer <= 0f)
		{
			this.ExitAttackState();
		}
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x0003D89C File Offset: 0x0003BA9C
	private void ValidateChasingRig()
	{
		if (this.targetRig == null)
		{
			this.SetTargetPlayer(null);
			return;
		}
		bool flag = false;
		foreach (VRRig vrrig in this.GetValidChoosableRigs())
		{
			if (vrrig == this.targetRig)
			{
				flag = true;
				this.SetTargetPlayer(vrrig);
				break;
			}
		}
		if (!flag)
		{
			this.SetTargetPlayer(null);
		}
	}

	// Token: 0x06000B74 RID: 2932 RVA: 0x0003D924 File Offset: 0x0003BB24
	public void SetState(MonkeyeAI_ReplState.EStates state_)
	{
		if (this.replState.IsMine)
		{
			this.replState.state = state_;
		}
		this.animController.SetInteger(MonkeyeAI.animStateID, (int)this.replState.state);
		switch (this.replState.state)
		{
		case MonkeyeAI_ReplState.EStates.Sleeping:
			this.setEyeColor(this.monkEyeEyeColorNormal);
			this.lockedOn = false;
			this.audioSource.clip = this.sleepLoopSound;
			this.audioSource.volume = 0f;
			if (this.audioSource.isActiveAndEnabled)
			{
				this.audioSource.GTPlay();
				return;
			}
			break;
		case MonkeyeAI_ReplState.EStates.Patrolling:
			this.setEyeColor(this.monkEyeEyeColorNormal);
			this.lockedOn = false;
			this.audioSource.clip = this.patrolLoopSound;
			this.audioSource.loop = true;
			this.audioSource.volume = 0f;
			if (this.audioSource.isActiveAndEnabled)
			{
				this.audioSource.GTPlay();
			}
			this.patrolCount = 0;
			return;
		case MonkeyeAI_ReplState.EStates.Chasing:
			this.setEyeColor(this.monkEyeEyeColorNormal);
			this.audioSource.loop = true;
			this.audioSource.volume = 0f;
			this.audioSource.clip = this.chaseLoopSound;
			if (this.audioSource.isActiveAndEnabled)
			{
				this.audioSource.GTPlay();
				return;
			}
			break;
		case MonkeyeAI_ReplState.EStates.ReturnToSleepPt:
		case MonkeyeAI_ReplState.EStates.GoToSleep:
			break;
		case MonkeyeAI_ReplState.EStates.BeginAttack:
			this.setEyeColor(this.monkEyeEyeColorAttacking);
			if (this.replState.IsMine)
			{
				this.replState.attackPos = ((this.targetRig != null) ? this.targetRig.transform.position : base.transform.position);
				this.replState.timer = this.beginAttackTime;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06000B75 RID: 2933 RVA: 0x0003DAF4 File Offset: 0x0003BCF4
	public void SetClientState(MonkeyeAI_ReplState.EStates state_)
	{
		this.animController.SetInteger(MonkeyeAI.animStateID, (int)this.replState.state);
		if (this.previousState != this.replState.state)
		{
			this.previousState = this.replState.state;
			switch (this.replState.state)
			{
			case MonkeyeAI_ReplState.EStates.Sleeping:
				this.setEyeColor(this.monkEyeEyeColorNormal);
				this.lockedOn = false;
				this.audioSource.clip = this.sleepLoopSound;
				this.audioSource.volume = Mathf.Min(this.sleepLoopVolume, this.audioSource.volume + this.deltaTime / this.sleepDuration);
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
				}
				break;
			case MonkeyeAI_ReplState.EStates.Patrolling:
				this.setEyeColor(this.monkEyeEyeColorNormal);
				this.lockedOn = false;
				this.audioSource.clip = this.patrolLoopSound;
				this.audioSource.loop = true;
				this.audioSource.volume = Mathf.Min(this.patrolLoopVolume, this.audioSource.volume + this.deltaTime / this.patrolLoopFadeInTime);
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
				}
				this.patrolCount = 0;
				break;
			case MonkeyeAI_ReplState.EStates.Chasing:
				this.setEyeColor(this.monkEyeEyeColorNormal);
				this.audioSource.loop = true;
				this.audioSource.volume = Mathf.Min(this.chaseLoopVolume, this.audioSource.volume + this.deltaTime / this.chaseLoopFadeInTime);
				this.audioSource.clip = this.chaseLoopSound;
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
				}
				break;
			case MonkeyeAI_ReplState.EStates.BeginAttack:
				this.setEyeColor(this.monkEyeEyeColorAttacking);
				break;
			}
		}
		switch (this.replState.state)
		{
		case MonkeyeAI_ReplState.EStates.Sleeping:
			this.audioSource.volume = Mathf.Min(this.sleepLoopVolume, this.audioSource.volume + this.deltaTime / this.sleepDuration);
			return;
		case MonkeyeAI_ReplState.EStates.Patrolling:
			this.audioSource.volume = Mathf.Min(this.patrolLoopVolume, this.audioSource.volume + this.deltaTime / this.patrolLoopFadeInTime);
			return;
		case MonkeyeAI_ReplState.EStates.Chasing:
			this.audioSource.volume = Mathf.Min(this.chaseLoopVolume, this.audioSource.volume + this.deltaTime / this.chaseLoopFadeInTime);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000B76 RID: 2934 RVA: 0x0003DD91 File Offset: 0x0003BF91
	private void setEyeColor(Color c)
	{
		if (c.a != 0f)
		{
			this.monkEyeMatPropBlock.SetVector(MonkeyeAI.EyeColorShaderProp, c);
			this.skinnedMeshRenderer.SetPropertyBlock(this.monkEyeMatPropBlock);
		}
	}

	// Token: 0x06000B77 RID: 2935 RVA: 0x0003DDC8 File Offset: 0x0003BFC8
	public List<VRRig> GetValidChoosableRigs()
	{
		this.validRigs.Clear();
		foreach (VRRig vrrig in this.playerCollection.containedRigs)
		{
			if ((NetworkSystem.Instance.InRoom || vrrig.isOfflineVRRig) && !(vrrig == null))
			{
				this.validRigs.Add(vrrig);
			}
		}
		return this.validRigs;
	}

	// Token: 0x06000B78 RID: 2936 RVA: 0x0003DE54 File Offset: 0x0003C054
	public void SliceUpdate()
	{
		this.wasConnectedToRoom = NetworkSystem.Instance.InRoom;
		this.deltaTime = Time.time - this.lastTime;
		this.lastTime = Time.time;
		this.UpdateClientState();
		if (NetworkSystem.Instance.InRoom && !this.replState.IsMine)
		{
			this.path = null;
			return;
		}
		if (!this.playerCollection.gameObject.activeInHierarchy)
		{
			NetPlayer netPlayer = null;
			float num = float.PositiveInfinity;
			foreach (VRRig vrrig in this.playersInRoomCollection.containedRigs)
			{
				if (!(vrrig == null))
				{
					float num2 = Vector3.Distance(base.transform.position, vrrig.transform.position);
					if (num2 < num)
					{
						netPlayer = vrrig.creator;
						num = num2;
					}
				}
			}
			if (num > 6f)
			{
				return;
			}
			this.path = null;
			if (netPlayer == null)
			{
				return;
			}
			this.replStateRequestableOwnershipGaurd.TransferOwnership(netPlayer, "");
			this.myRequestableOwnershipGaurd.TransferOwnership(netPlayer, "");
			return;
		}
		else
		{
			this.ValidateChasingRig();
			switch (this.replState.state)
			{
			case MonkeyeAI_ReplState.EStates.Sleeping:
				this.Sleeping();
				break;
			case MonkeyeAI_ReplState.EStates.Patrolling:
				this.Patrolling();
				break;
			case MonkeyeAI_ReplState.EStates.Chasing:
				this.Chasing();
				break;
			case MonkeyeAI_ReplState.EStates.ReturnToSleepPt:
				this.ReturnToSleepPt();
				break;
			case MonkeyeAI_ReplState.EStates.BeginAttack:
				this.BeginAttack();
				break;
			case MonkeyeAI_ReplState.EStates.OpenFloor:
				this.OpenFloor();
				break;
			case MonkeyeAI_ReplState.EStates.DropPlayer:
				this.DropPlayer();
				break;
			case MonkeyeAI_ReplState.EStates.CloseFloor:
				this.CloseFloor();
				break;
			}
			if (this.path == null)
			{
				return;
			}
			this.FollowPath();
			this.velocity = base.transform.position - this.prevPosition;
			this.prevPosition = base.transform.position;
			return;
		}
	}

	// Token: 0x06000B79 RID: 2937 RVA: 0x00019260 File Offset: 0x00017460
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000B7A RID: 2938 RVA: 0x00019269 File Offset: 0x00017469
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000B7B RID: 2939 RVA: 0x0003E040 File Offset: 0x0003C240
	private void AntiOverlapAssurance()
	{
		try
		{
			if ((!NetworkSystem.Instance.InRoom || this.replState.IsMine) && this.playerCollection.gameObject.activeInHierarchy)
			{
				foreach (MonkeyeAI monkeyeAI in this.playerCollection.monkeyeAis)
				{
					if (!(monkeyeAI == this) && Vector3.Distance(base.transform.position, monkeyeAI.transform.position) < this.overlapRadius && (double)Vector3.Dot(base.transform.forward, monkeyeAI.transform.forward) > 0.2)
					{
						MonkeyeAI_ReplState.EStates state = this.replState.state;
						if (state != MonkeyeAI_ReplState.EStates.Patrolling)
						{
							if (state == MonkeyeAI_ReplState.EStates.Chasing)
							{
								if (monkeyeAI.replState.state == MonkeyeAI_ReplState.EStates.Chasing)
								{
									this.SetState(MonkeyeAI_ReplState.EStates.Patrolling);
								}
							}
						}
						else
						{
							this.PickNewPath(false);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex, this);
		}
	}

	// Token: 0x06000B7C RID: 2940 RVA: 0x0003E168 File Offset: 0x0003C368
	private void SetTargetPlayer([CanBeNull] VRRig rig)
	{
		if (rig == null)
		{
			this.replState.userId = "";
			this.replState.freezePlayer = false;
			this.replState.floorEnabled = true;
			this.replState.portalEnabled = false;
			this.targetRig = null;
			return;
		}
		this.replState.userId = this.UserIdFromRig(rig);
		this.targetRig = rig;
	}

	// Token: 0x04000D9E RID: 3486
	public List<Transform> patrolPts;

	// Token: 0x04000D9F RID: 3487
	public Transform sleepPt;

	// Token: 0x04000DA0 RID: 3488
	private int patrolIdx = -1;

	// Token: 0x04000DA1 RID: 3489
	private int patrolCount;

	// Token: 0x04000DA2 RID: 3490
	private Vector3 targetPosition;

	// Token: 0x04000DA3 RID: 3491
	private MaterialPropertyBlock portalMatPropBlock;

	// Token: 0x04000DA4 RID: 3492
	private MaterialPropertyBlock monkEyeMatPropBlock;

	// Token: 0x04000DA5 RID: 3493
	private Renderer renderer;

	// Token: 0x04000DA6 RID: 3494
	private AIDestinationSetter aiDest;

	// Token: 0x04000DA7 RID: 3495
	private AIPath aiPath;

	// Token: 0x04000DA8 RID: 3496
	private AILerp aiLerp;

	// Token: 0x04000DA9 RID: 3497
	private Seeker seeker;

	// Token: 0x04000DAA RID: 3498
	private Path path;

	// Token: 0x04000DAB RID: 3499
	private int currentWaypoint;

	// Token: 0x04000DAC RID: 3500
	private bool calculatingPath;

	// Token: 0x04000DAD RID: 3501
	private Monkeye_LazerFX lazerFx;

	// Token: 0x04000DAE RID: 3502
	private Animator animController;

	// Token: 0x04000DAF RID: 3503
	private RaycastHit[] rayResults = new RaycastHit[1];

	// Token: 0x04000DB0 RID: 3504
	private LayerMask layerMask;

	// Token: 0x04000DB1 RID: 3505
	private bool wasConnectedToRoom;

	// Token: 0x04000DB2 RID: 3506
	public SkinnedMeshRenderer skinnedMeshRenderer;

	// Token: 0x04000DB3 RID: 3507
	public MazePlayerCollection playerCollection;

	// Token: 0x04000DB4 RID: 3508
	public PlayerCollection playersInRoomCollection;

	// Token: 0x04000DB5 RID: 3509
	private List<VRRig> validRigs = new List<VRRig>();

	// Token: 0x04000DB6 RID: 3510
	public GameObject portalFx;

	// Token: 0x04000DB7 RID: 3511
	public Transform[] eyeBones;

	// Token: 0x04000DB8 RID: 3512
	public float speed = 0.1f;

	// Token: 0x04000DB9 RID: 3513
	public float rotationSpeed = 1f;

	// Token: 0x04000DBA RID: 3514
	public float wakeDistance = 1f;

	// Token: 0x04000DBB RID: 3515
	public float chaseDistance = 3f;

	// Token: 0x04000DBC RID: 3516
	public float sleepDuration = 3f;

	// Token: 0x04000DBD RID: 3517
	public float attackDistance = 0.1f;

	// Token: 0x04000DBE RID: 3518
	public float beginAttackTime = 1f;

	// Token: 0x04000DBF RID: 3519
	public float openFloorTime = 3f;

	// Token: 0x04000DC0 RID: 3520
	public float dropPlayerTime = 1f;

	// Token: 0x04000DC1 RID: 3521
	public float closeFloorTime = 1f;

	// Token: 0x04000DC2 RID: 3522
	public Color portalColor;

	// Token: 0x04000DC3 RID: 3523
	public Color gorillaPortalColor;

	// Token: 0x04000DC4 RID: 3524
	public Color monkEyeColor;

	// Token: 0x04000DC5 RID: 3525
	public Color monkEyeEyeColorNormal;

	// Token: 0x04000DC6 RID: 3526
	public Color monkEyeEyeColorAttacking;

	// Token: 0x04000DC7 RID: 3527
	public int maxPatrols = 4;

	// Token: 0x04000DC8 RID: 3528
	private VRRig targetRig;

	// Token: 0x04000DC9 RID: 3529
	private float deltaTime;

	// Token: 0x04000DCA RID: 3530
	private float lastTime;

	// Token: 0x04000DCB RID: 3531
	public MonkeyeAI_ReplState replState;

	// Token: 0x04000DCC RID: 3532
	private MonkeyeAI_ReplState.EStates previousState;

	// Token: 0x04000DCD RID: 3533
	private RequestableOwnershipGuard replStateRequestableOwnershipGaurd;

	// Token: 0x04000DCE RID: 3534
	private RequestableOwnershipGuard myRequestableOwnershipGaurd;

	// Token: 0x04000DCF RID: 3535
	private int layerBase;

	// Token: 0x04000DD0 RID: 3536
	private int layerForward = 1;

	// Token: 0x04000DD1 RID: 3537
	private int layerLeft = 2;

	// Token: 0x04000DD2 RID: 3538
	private int layerRight = 3;

	// Token: 0x04000DD3 RID: 3539
	private static readonly int EmissionColorShaderProp = ShaderProps._EmissionColor;

	// Token: 0x04000DD4 RID: 3540
	private static readonly int ColorShaderProp = ShaderProps._BaseColor;

	// Token: 0x04000DD5 RID: 3541
	private static readonly int EyeColorShaderProp = ShaderProps._GChannelColor;

	// Token: 0x04000DD6 RID: 3542
	private static readonly int tintColorShaderProp = ShaderProps._TintColor;

	// Token: 0x04000DD7 RID: 3543
	private static readonly int animStateID = Animator.StringToHash("state");

	// Token: 0x04000DD8 RID: 3544
	private Vector3 prevPosition;

	// Token: 0x04000DD9 RID: 3545
	private Vector3 velocity;

	// Token: 0x04000DDA RID: 3546
	public AudioSource audioSource;

	// Token: 0x04000DDB RID: 3547
	public AudioClip sleepLoopSound;

	// Token: 0x04000DDC RID: 3548
	public float sleepLoopVolume = 0.5f;

	// Token: 0x04000DDD RID: 3549
	[FormerlySerializedAs("moveLoopSound")]
	public AudioClip patrolLoopSound;

	// Token: 0x04000DDE RID: 3550
	public float patrolLoopVolume = 0.5f;

	// Token: 0x04000DDF RID: 3551
	public float patrolLoopFadeInTime = 1f;

	// Token: 0x04000DE0 RID: 3552
	public AudioClip chaseLoopSound;

	// Token: 0x04000DE1 RID: 3553
	public float chaseLoopVolume = 0.5f;

	// Token: 0x04000DE2 RID: 3554
	public float chaseLoopFadeInTime = 0.05f;

	// Token: 0x04000DE3 RID: 3555
	public AudioClip attackSound;

	// Token: 0x04000DE4 RID: 3556
	public float attackVolume = 0.5f;

	// Token: 0x04000DE5 RID: 3557
	public float overlapRadius;

	// Token: 0x04000DE6 RID: 3558
	private bool lockedOn;
}
