using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000D60 RID: 3424
public class TeleportStationManager : MonoBehaviour
{
	// Token: 0x17000818 RID: 2072
	// (get) Token: 0x060054B9 RID: 21689 RVA: 0x001BC9E7 File Offset: 0x001BABE7
	public static TeleportStationManager Instance
	{
		get
		{
			return TeleportStationManager.__instance;
		}
	}

	// Token: 0x060054BA RID: 21690 RVA: 0x001BC9EE File Offset: 0x001BABEE
	private void Awake()
	{
		if (TeleportStationManager.__instance != null)
		{
			return;
		}
		TeleportStationManager.__instance = this;
	}

	// Token: 0x060054BB RID: 21691 RVA: 0x001BCA04 File Offset: 0x001BAC04
	public static void Initialize(GameObject fPersonEffect, GameObject thirdPersonEffectStart, GameObject thirdPersonEffectEnd)
	{
		if (TeleportStationManager.__instance.ready)
		{
			return;
		}
		TeleportStationManager.__instance.firstPersonEffect = Object.Instantiate<GameObject>(fPersonEffect, TeleportStationManager.__instance.transform);
		TeleportStationManager.__instance.thirdPersonEffectStarts = new GameObject[20];
		TeleportStationManager.__instance.thirdPersonEffectEnds = new GameObject[20];
		for (int i = 0; i < 20; i++)
		{
			TeleportStationManager.__instance.thirdPersonEffectStarts[i] = Object.Instantiate<GameObject>(thirdPersonEffectStart, TeleportStationManager.__instance.transform);
			TeleportStationManager.__instance.thirdPersonEffectEnds[i] = Object.Instantiate<GameObject>(thirdPersonEffectEnd, TeleportStationManager.__instance.transform);
		}
		TeleportStationManager.__instance.effectsIndex = 0;
		TeleportStationManager.__instance.ready = true;
	}

	// Token: 0x060054BC RID: 21692 RVA: 0x001BCAB8 File Offset: 0x001BACB8
	private void Update()
	{
		if (this.firstPersonTPort != null)
		{
			this.firstPersonTPort.Tick(Time.deltaTime, base.gameObject);
			if (this.firstPersonTPort.Done)
			{
				this.firstPersonTPort = null;
			}
		}
		for (int i = this.thirdPersonTPort.Count - 1; i >= 0; i--)
		{
			this.thirdPersonTPort[i].Tick(Time.deltaTime);
			if (this.thirdPersonTPort[i].Done)
			{
				this.thirdPersonTPort.RemoveAt(i);
			}
		}
	}

	// Token: 0x060054BD RID: 21693 RVA: 0x001BCB44 File Offset: 0x001BAD44
	public void ThirdPersonTeleport(VRRig rig, int effectTime)
	{
		this.thirdPersonTPort.Add(new TeleportStationManager.TPTPort(rig, effectTime, this.thirdPersonEffectStarts[this.effectsIndex], this.thirdPersonEffectEnds[this.effectsIndex]));
		this.effectsIndex = (this.effectsIndex + 1) % 20;
	}

	// Token: 0x060054BE RID: 21694 RVA: 0x001BCB84 File Offset: 0x001BAD84
	public void FirstPersonTeleport(Vector3 targetPos, float targetRot, Vector3 targetSlop, GTZone teleportToZone, GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider destinationFriendCollider, GorillaNetworkJoinTrigger destinationJoinTrigger, int effectTime)
	{
		if (this.firstPersonTPort != null)
		{
			return;
		}
		this.firstPersonTPort = new TeleportStationManager.FPTPort(targetPos, targetRot, targetSlop, teleportToZone, sourceFriendCollider, destinationFriendCollider, destinationJoinTrigger, effectTime, this.firstPersonEffect);
	}

	// Token: 0x04006602 RID: 26114
	private static TeleportStationManager __instance;

	// Token: 0x04006603 RID: 26115
	private const int _3RD_PERSON_EFFECTS_CACHE_SIZE = 20;

	// Token: 0x04006604 RID: 26116
	private GameObject[] thirdPersonEffectStarts = new GameObject[20];

	// Token: 0x04006605 RID: 26117
	private GameObject[] thirdPersonEffectEnds = new GameObject[20];

	// Token: 0x04006606 RID: 26118
	private GameObject firstPersonEffect;

	// Token: 0x04006607 RID: 26119
	private int effectsIndex;

	// Token: 0x04006608 RID: 26120
	private TeleportStationManager.FPTPort firstPersonTPort;

	// Token: 0x04006609 RID: 26121
	private List<TeleportStationManager.TPTPort> thirdPersonTPort = new List<TeleportStationManager.TPTPort>();

	// Token: 0x0400660A RID: 26122
	private bool ready;

	// Token: 0x02000D61 RID: 3425
	public class FPTPort
	{
		// Token: 0x060054C0 RID: 21696 RVA: 0x001BCBE8 File Offset: 0x001BADE8
		public FPTPort(Vector3 targetPos, float targetRot, Vector3 targetSlop, GTZone teleportToZone, GorillaFriendCollider sourceFriendCollider, GorillaFriendCollider destinationFriendCollider, GorillaNetworkJoinTrigger destinationJoinTrigger, int effectTime, GameObject firstPersonEffect)
		{
			this.targetPos = targetPos;
			this.targetRot = targetRot;
			this.targetSlop = targetSlop;
			this.teleportToZone = teleportToZone;
			this.sourceFriendCollider = sourceFriendCollider;
			this.destinationFriendCollider = destinationFriendCollider;
			this.destinationJoinTrigger = destinationJoinTrigger;
			this.effectTime = (float)effectTime;
			this.effectTimeRemains = (float)effectTime;
			this.firstPersonEffect = firstPersonEffect;
		}

		// Token: 0x17000819 RID: 2073
		// (get) Token: 0x060054C1 RID: 21697 RVA: 0x001BCC4A File Offset: 0x001BAE4A
		public int Phase
		{
			get
			{
				return this.phase;
			}
		}

		// Token: 0x1700081A RID: 2074
		// (get) Token: 0x060054C2 RID: 21698 RVA: 0x001BCC52 File Offset: 0x001BAE52
		public float EffectTimeRemains
		{
			get
			{
				return this.effectTimeRemains;
			}
		}

		// Token: 0x1700081B RID: 2075
		// (get) Token: 0x060054C3 RID: 21699 RVA: 0x001BCC5A File Offset: 0x001BAE5A
		public bool Done
		{
			get
			{
				return this.phase > 3;
			}
		}

		// Token: 0x060054C4 RID: 21700 RVA: 0x001BCC68 File Offset: 0x001BAE68
		public void Tick(float deltaTime, GameObject go)
		{
			GTPlayer instance = GTPlayer.Instance;
			if (instance == null)
			{
				Debug.LogError("[TeleportStation] GTPlayer.Instance is null.");
				return;
			}
			switch (this.phase)
			{
			case 0:
				instance.disableMovement = true;
				instance.SetGravityOverride(go, null);
				this.firstPersonEffect.transform.parent = Camera.main.transform;
				this.firstPersonEffect.transform.localPosition = Vector3.zero;
				this.firstPersonEffect.SetActive(true);
				this.phase++;
				break;
			case 1:
				if (this.effectTimeRemains < this.effectTime / 2f)
				{
					Physics.SyncTransforms();
					this.phase++;
				}
				break;
			case 2:
				this.targetPos.x = this.targetPos.x + Random.Range(-this.targetSlop.x, this.targetSlop.x);
				this.targetPos.z = this.targetPos.z + Random.Range(-this.targetSlop.z, this.targetSlop.z);
				this.phase++;
				instance.TeleportTo(this.targetPos, Quaternion.Euler(0f, this.targetRot + Random.Range(-this.targetSlop.y, this.targetSlop.y), 0f), false, true);
				if (this.teleportToZone != GTZone.none)
				{
					ZoneManagement.SetActiveZone(this.teleportToZone);
				}
				GTPlayerTransform.Instance.ClearAllGravityZones();
				if (NetworkSystem.Instance.InRoom)
				{
					int num = this.LowestActorNumberInFriendCollider();
					if (!NetworkSystem.Instance.SessionIsPrivate && num == NetworkSystem.Instance.LocalPlayer.ActorNumber)
					{
						this.SetupFriendGroup(this.sourceFriendCollider, this.destinationFriendCollider, true);
						RoomSystem.SendElevatorFollowCommand(PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr, this.sourceFriendCollider, this.destinationFriendCollider);
						PhotonNetwork.SendAllOutgoingCommands();
						if (FriendshipGroupDetection.Instance.IsInParty)
						{
							PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this.destinationJoinTrigger, JoinType.ForceJoinWithParty, null, false);
						}
						else
						{
							PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this.destinationJoinTrigger, JoinType.JoinWithNearby, null, false);
						}
					}
				}
				else
				{
					this.SetupFriendGroup(this.sourceFriendCollider, this.destinationFriendCollider, false);
					PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this.destinationJoinTrigger, JoinType.JoinWithNearby, null, false);
				}
				break;
			case 3:
				if (this.effectTimeRemains <= 0f)
				{
					this.firstPersonEffect.SetActive(false);
					this.firstPersonEffect.transform.parent = go.transform;
					this.firstPersonEffect.transform.localPosition = Vector3.zero;
					instance.disableMovement = false;
					instance.UnsetGravityOverride(go);
					this.phase++;
				}
				break;
			}
			this.effectTimeRemains -= deltaTime;
		}

		// Token: 0x060054C5 RID: 21701 RVA: 0x001BCF54 File Offset: 0x001BB154
		private int LowestActorNumberInFriendCollider()
		{
			this.sourceFriendCollider.RefreshPlayersWithinBounds();
			this.destinationFriendCollider.RefreshPlayersWithinBounds();
			int num = int.MaxValue;
			NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
			for (int i = 0; i < allNetPlayers.Length; i++)
			{
				if (num > allNetPlayers[i].ActorNumber && (this.sourceFriendCollider.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId) || this.destinationFriendCollider.playerIDsCurrentlyTouching.Contains(allNetPlayers[i].UserId)))
				{
					num = allNetPlayers[i].ActorNumber;
				}
			}
			return num;
		}

		// Token: 0x060054C6 RID: 21702 RVA: 0x001BCFE0 File Offset: 0x001BB1E0
		private void SetupFriendGroup(GorillaFriendCollider source, GorillaFriendCollider destination, bool refreshFriendList = false)
		{
			if (refreshFriendList)
			{
				PhotonNetworkController.Instance.FriendIDList = new List<string>(source.playerIDsCurrentlyTouching);
				PhotonNetworkController.Instance.FriendIDList.AddRange(destination.playerIDsCurrentlyTouching);
			}
			PhotonNetworkController.Instance.shuffler = Random.Range(0, 99).ToString().PadLeft(2, '0') + Random.Range(0, 99999999).ToString().PadLeft(8, '0');
			PhotonNetworkController.Instance.keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
		}

		// Token: 0x0400660B RID: 26123
		private Vector3 targetPos;

		// Token: 0x0400660C RID: 26124
		private float targetRot;

		// Token: 0x0400660D RID: 26125
		private Vector3 targetSlop;

		// Token: 0x0400660E RID: 26126
		private GTZone teleportToZone;

		// Token: 0x0400660F RID: 26127
		private GorillaFriendCollider sourceFriendCollider;

		// Token: 0x04006610 RID: 26128
		private GorillaFriendCollider destinationFriendCollider;

		// Token: 0x04006611 RID: 26129
		private GorillaNetworkJoinTrigger destinationJoinTrigger;

		// Token: 0x04006612 RID: 26130
		private float effectTime;

		// Token: 0x04006613 RID: 26131
		private float effectTimeRemains;

		// Token: 0x04006614 RID: 26132
		private GameObject firstPersonEffect;

		// Token: 0x04006615 RID: 26133
		private int phase;
	}

	// Token: 0x02000D62 RID: 3426
	public class TPTPort
	{
		// Token: 0x060054C7 RID: 21703 RVA: 0x001BD089 File Offset: 0x001BB289
		public TPTPort(VRRig rig, int effectTime, GameObject startEffect, GameObject endEffect)
		{
			this.rig = rig;
			this.effectTimeRemains = (float)effectTime;
			this.startEffect = startEffect;
			this.endEffect = endEffect;
		}

		// Token: 0x1700081C RID: 2076
		// (get) Token: 0x060054C8 RID: 21704 RVA: 0x001BD0AF File Offset: 0x001BB2AF
		public float EffectTimeRemains
		{
			get
			{
				return this.effectTimeRemains;
			}
		}

		// Token: 0x1700081D RID: 2077
		// (get) Token: 0x060054C9 RID: 21705 RVA: 0x001BD0B7 File Offset: 0x001BB2B7
		public bool Done
		{
			get
			{
				return this.phase > 1;
			}
		}

		// Token: 0x060054CA RID: 21706 RVA: 0x001BD0C4 File Offset: 0x001BB2C4
		public void Tick(float deltaTime)
		{
			int num = this.phase;
			if (num != 0)
			{
				if (num == 1)
				{
					if (this.effectTimeRemains <= 0f)
					{
						this.rig.ReactivateAllRenderers();
						this.endEffect.transform.position = this.rig.transform.position;
						this.endEffect.SetActive(true);
						this.phase++;
					}
				}
			}
			else
			{
				this.rig.DeactivateAllRenderers();
				this.startEffect.transform.position = this.rig.transform.position;
				this.startEffect.SetActive(true);
				this.phase++;
			}
			this.effectTimeRemains -= deltaTime;
		}

		// Token: 0x04006616 RID: 26134
		private VRRig rig;

		// Token: 0x04006617 RID: 26135
		private float effectTimeRemains;

		// Token: 0x04006618 RID: 26136
		private GameObject startEffect;

		// Token: 0x04006619 RID: 26137
		private GameObject endEffect;

		// Token: 0x0400661A RID: 26138
		private int phase;
	}
}
