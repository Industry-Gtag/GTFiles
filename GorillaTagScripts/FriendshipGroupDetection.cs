using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTag;
using Photon.Pun;
using Unity.Profiling;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F88 RID: 3976
	public class FriendshipGroupDetection : NetworkSceneObject, ITickSystemTick
	{
		// Token: 0x1700096E RID: 2414
		// (get) Token: 0x060062A9 RID: 25257 RVA: 0x001FBDB8 File Offset: 0x001F9FB8
		// (set) Token: 0x060062AA RID: 25258 RVA: 0x001FBDBF File Offset: 0x001F9FBF
		public static FriendshipGroupDetection Instance { get; private set; }

		// Token: 0x1700096F RID: 2415
		// (get) Token: 0x060062AB RID: 25259 RVA: 0x001FBDC7 File Offset: 0x001F9FC7
		// (set) Token: 0x060062AC RID: 25260 RVA: 0x001FBDCF File Offset: 0x001F9FCF
		public List<Color> myBeadColors { get; private set; } = new List<Color>();

		// Token: 0x17000970 RID: 2416
		// (get) Token: 0x060062AD RID: 25261 RVA: 0x001FBDD8 File Offset: 0x001F9FD8
		// (set) Token: 0x060062AE RID: 25262 RVA: 0x001FBDE0 File Offset: 0x001F9FE0
		public Color myBraceletColor { get; private set; }

		// Token: 0x17000971 RID: 2417
		// (get) Token: 0x060062AF RID: 25263 RVA: 0x001FBDE9 File Offset: 0x001F9FE9
		// (set) Token: 0x060062B0 RID: 25264 RVA: 0x001FBDF1 File Offset: 0x001F9FF1
		public int MyBraceletSelfIndex { get; private set; }

		// Token: 0x17000972 RID: 2418
		// (get) Token: 0x060062B1 RID: 25265 RVA: 0x001FBDFA File Offset: 0x001F9FFA
		public List<string> PartyMemberIDs
		{
			get
			{
				return this.myPartyMemberIDs;
			}
		}

		// Token: 0x17000973 RID: 2419
		// (get) Token: 0x060062B2 RID: 25266 RVA: 0x001FBE02 File Offset: 0x001FA002
		public bool IsInParty
		{
			get
			{
				return this.myPartyMemberIDs != null;
			}
		}

		// Token: 0x17000974 RID: 2420
		// (get) Token: 0x060062B3 RID: 25267 RVA: 0x001FBE0D File Offset: 0x001FA00D
		// (set) Token: 0x060062B4 RID: 25268 RVA: 0x001FBE15 File Offset: 0x001FA015
		public GroupJoinZoneAB partyZone { get; private set; }

		// Token: 0x17000975 RID: 2421
		// (get) Token: 0x060062B5 RID: 25269 RVA: 0x001FBE1E File Offset: 0x001FA01E
		// (set) Token: 0x060062B6 RID: 25270 RVA: 0x001FBE26 File Offset: 0x001FA026
		public bool TickRunning { get; set; }

		// Token: 0x060062B7 RID: 25271 RVA: 0x001FBE30 File Offset: 0x001FA030
		private void Awake()
		{
			FriendshipGroupDetection.Instance = this;
			if (this.friendshipBubble)
			{
				this.particleSystem = this.friendshipBubble.GetComponent<ParticleSystem>();
				this.audioSource = this.friendshipBubble.GetComponent<AudioSource>();
			}
			NetworkSystem.Instance.OnPlayerJoined += this.OnPlayerJoinedRoom;
		}

		// Token: 0x060062B8 RID: 25272 RVA: 0x000E130F File Offset: 0x000DF50F
		private new void OnEnable()
		{
			NetworkBehaviourUtils.InternalOnEnable(this);
			base.OnEnable();
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x060062B9 RID: 25273 RVA: 0x000E1323 File Offset: 0x000DF523
		private new void OnDisable()
		{
			NetworkBehaviourUtils.InternalOnDisable(this);
			base.OnDisable();
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x060062BA RID: 25274 RVA: 0x001FBE94 File Offset: 0x001FA094
		private void OnPlayerJoinedRoom(NetPlayer joiningPlayer)
		{
			if (!this.IsInParty || NetworkSystem.Instance == null || NetworkSystem.Instance.CurrentRoom == null)
			{
				return;
			}
			bool flag = (int)NetworkSystem.Instance.CurrentRoom.MaxPlayers == NetworkSystem.Instance.RoomPlayerCount;
			Debug.Log(string.Concat(new string[]
			{
				"[FriendshipGroupDetection::OnPlayerJoinedRoom] JoiningPlayer: ",
				joiningPlayer.NickName,
				", ",
				joiningPlayer.UserId,
				" ",
				string.Format("| IsLocal: {0} | Room Full: {1}", joiningPlayer.IsLocal, flag)
			}));
			if (joiningPlayer.IsLocal)
			{
				this.lastJoinedRoomTime = (double)Time.time;
				if (!flag)
				{
					Debug.Log("[FriendshipGroupDetection::OnPlayerJoinedRoom] Delaying PartyRefresh...");
					this.wantsPartyRefreshPostJoin = true;
					return;
				}
			}
			if (flag)
			{
				this.RefreshPartyMembers();
			}
		}

		// Token: 0x060062BB RID: 25275 RVA: 0x001FBF69 File Offset: 0x001FA169
		public void AddGroupZoneCallback(Action<GroupJoinZoneAB> callback)
		{
			this.groupZoneCallbacks.Add(callback);
		}

		// Token: 0x060062BC RID: 25276 RVA: 0x001FBF77 File Offset: 0x001FA177
		public void RemoveGroupZoneCallback(Action<GroupJoinZoneAB> callback)
		{
			this.groupZoneCallbacks.Remove(callback);
		}

		// Token: 0x060062BD RID: 25277 RVA: 0x001FBF86 File Offset: 0x001FA186
		public bool IsInMyGroup(string userID)
		{
			return this.myPartyMemberIDs != null && this.myPartyMemberIDs.Contains(userID);
		}

		// Token: 0x060062BE RID: 25278 RVA: 0x001FBFA0 File Offset: 0x001FA1A0
		public bool AnyPartyMembersOutsideFriendCollider()
		{
			if (!this.IsInParty)
			{
				return false;
			}
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				if (rigContainer.Rig.IsLocalPartyMember && !GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(rigContainer.Creator.UserId))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x17000976 RID: 2422
		// (get) Token: 0x060062BF RID: 25279 RVA: 0x001FC028 File Offset: 0x001FA228
		// (set) Token: 0x060062C0 RID: 25280 RVA: 0x001FC030 File Offset: 0x001FA230
		public bool DidJoinLeftHanded { get; private set; }

		// Token: 0x060062C1 RID: 25281 RVA: 0x001FC03C File Offset: 0x001FA23C
		public void Tick()
		{
			using (FriendshipGroupDetection.profiler_Tick.Auto())
			{
				if (this.wantsPartyRefreshPostJoin && this.lastJoinedRoomTime + this.joinedRoomRefreshPartyDelay < (double)Time.time)
				{
					this.RefreshPartyMembers();
				}
				else if (this.wantsPartyRefreshPostFollowFailed && this.lastFailedToFollowPartyTime + this.failedToFollowRefreshPartyDelay < (double)Time.time)
				{
					this.RefreshPartyMembers();
				}
				List<int> list = this.playersInProvisionalGroup;
				List<int> list2 = this.playersInProvisionalGroup;
				List<int> list3 = this.tempIntList;
				this.tempIntList = list2;
				this.playersInProvisionalGroup = list3;
				Vector3 vector;
				this.UpdateProvisionalGroup(out vector);
				if (this.playersInProvisionalGroup.Count > 0)
				{
					this.friendshipBubble.transform.position = vector;
				}
				bool flag = false;
				if (list.Count == this.playersInProvisionalGroup.Count)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i] != this.playersInProvisionalGroup[i])
						{
							flag = true;
							break;
						}
					}
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					this.groupCreateAfterTimestamp = Time.time + this.groupTime;
					this.amFirstProvisionalPlayer = this.playersInProvisionalGroup.Count > 0 && this.playersInProvisionalGroup[0] == NetworkSystem.Instance.LocalPlayer.ActorNumber;
					if (this.playersInProvisionalGroup.Count > 0 && !this.amFirstProvisionalPlayer)
					{
						List<int> list4 = this.tempIntList;
						list4.Clear();
						NetPlayer netPlayer = null;
						foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
						{
							VRRig rig = rigContainer.Rig;
							if (rig.creator.ActorNumber == this.playersInProvisionalGroup[0])
							{
								netPlayer = rig.creator;
								if (rig.IsLocalPartyMember)
								{
									list4.Clear();
									break;
								}
							}
							else if (rig.IsLocalPartyMember)
							{
								list4.Add(rig.creator.ActorNumber);
							}
						}
						if (list4.Count > 0)
						{
							this.photonView.RPC("NotifyPartyMerging", netPlayer.GetPlayerRef(), new object[] { list4.ToArray() });
						}
						else
						{
							this.photonView.RPC("NotifyNoPartyToMerge", netPlayer.GetPlayerRef(), Array.Empty<object>());
						}
					}
					if (this.playersInProvisionalGroup.Count == 0)
					{
						if (Time.time > this.suppressPartyCreationUntilTimestamp && this.playEffectsAfterTimestamp == 0f)
						{
							this.audioSource.GTStop();
							this.audioSource.GTPlayOneShot(this.fistBumpInterruptedAudio, 1f);
						}
						this.particleSystem.Stop();
						this.playEffectsAfterTimestamp = 0f;
					}
					else
					{
						this.playEffectsAfterTimestamp = Time.time + this.playEffectsDelay;
					}
				}
				else if (this.playEffectsAfterTimestamp > 0f && Time.time > this.playEffectsAfterTimestamp)
				{
					this.audioSource.time = 0f;
					this.audioSource.GTPlay();
					this.particleSystem.Play();
					this.playEffectsAfterTimestamp = 0f;
				}
				else if (this.playersInProvisionalGroup.Count > 0 && Time.time > this.groupCreateAfterTimestamp && this.amFirstProvisionalPlayer)
				{
					List<int> list5 = this.tempIntList;
					list5.Clear();
					list5.AddRange(this.playersInProvisionalGroup);
					int num = 0;
					if (this.IsInParty)
					{
						foreach (RigContainer rigContainer2 in VRRigCache.ActiveRigContainers)
						{
							VRRig rig2 = rigContainer2.Rig;
							if (rig2.IsLocalPartyMember)
							{
								list5.Add(rig2.creator.ActorNumber);
								num++;
							}
						}
					}
					int num2 = 0;
					foreach (int num3 in this.playersInProvisionalGroup)
					{
						int[] array;
						if (this.partyMergeIDs.TryGetValue(num3, out array))
						{
							list5.AddRange(array);
							num2++;
						}
					}
					list5.Sort();
					int[] array2 = list5.Distinct<int>().ToArray<int>();
					this.myBraceletColor = GTColor.RandomHSV(this.braceletRandomColorHSVRanges);
					this.SendPartyFormedRPC(FriendshipGroupDetection.PackColor(this.myBraceletColor), array2, false);
					this.groupCreateAfterTimestamp = Time.time + this.cooldownAfterCreatingGroup;
				}
				if (this.myPartyMemberIDs != null)
				{
					this.UpdateWarningSigns();
				}
			}
		}

		// Token: 0x060062C2 RID: 25282 RVA: 0x001FC518 File Offset: 0x001FA718
		private void UpdateProvisionalGroup(out Vector3 midpoint)
		{
			using (FriendshipGroupDetection.profiler_updateProvisionalGroup.Auto())
			{
				this.playersInProvisionalGroup.Clear();
				bool flag;
				VRMap makingFist = VRRig.LocalRig.GetMakingFist(this.debug, out flag);
				if (makingFist == null || !NetworkSystem.Instance.InRoom || VRRig.LocalRig.leftHandLink.IsLinkActive() || VRRig.LocalRig.rightHandLink.IsLinkActive() || VRRigCache.ActiveRigs.Count == 0 || Time.time < this.suppressPartyCreationUntilTimestamp || (global::GorillaGameModes.GameMode.ActiveGameMode != null && !global::GorillaGameModes.GameMode.ActiveGameMode.CanJoinFrienship(NetworkSystem.Instance.LocalPlayer)))
				{
					midpoint = Vector3.zero;
				}
				else
				{
					this.WillJoinLeftHanded = flag;
					this.playersToPropagateFrom.Clear();
					this.provisionalGroupUsingLeftHands.Clear();
					this.playersMakingFists.Clear();
					int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
					int num = -1;
					foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
					{
						VRRig rig = rigContainer.Rig;
						bool flag2;
						VRMap makingFist2 = rig.GetMakingFist(this.debug, out flag2);
						if (makingFist2 != null && !rig.leftHandLink.IsLinkActive() && !rig.rightHandLink.IsLinkActive() && (!global::GorillaGameModes.GameMode.ActiveGameMode.IsNotNull() || global::GorillaGameModes.GameMode.ActiveGameMode.CanJoinFrienship(rig.OwningNetPlayer)))
						{
							FriendshipGroupDetection.PlayerFist playerFist = new FriendshipGroupDetection.PlayerFist
							{
								actorNumber = rig.creator.ActorNumber,
								position = makingFist2.rigTarget.position,
								isLeftHand = flag2
							};
							if (rig.isOfflineVRRig)
							{
								num = this.playersMakingFists.Count;
							}
							this.playersMakingFists.Add(playerFist);
						}
					}
					if (this.playersMakingFists.Count <= 1 || num == -1)
					{
						midpoint = Vector3.zero;
					}
					else
					{
						this.playersToPropagateFrom.Enqueue(this.playersMakingFists[num]);
						this.playersInProvisionalGroup.Add(actorNumber);
						midpoint = makingFist.rigTarget.position;
						int num2 = 1 << num;
						FriendshipGroupDetection.PlayerFist playerFist2;
						while (this.playersToPropagateFrom.TryDequeue(out playerFist2))
						{
							for (int i = 0; i < this.playersMakingFists.Count; i++)
							{
								if ((num2 & (1 << i)) == 0)
								{
									FriendshipGroupDetection.PlayerFist playerFist3 = this.playersMakingFists[i];
									if ((playerFist2.position - playerFist3.position).IsShorterThan(this.detectionRadius))
									{
										int num3 = ~this.playersInProvisionalGroup.BinarySearch(playerFist3.actorNumber);
										num2 |= 1 << i;
										this.playersInProvisionalGroup.Insert(num3, playerFist3.actorNumber);
										if (playerFist3.isLeftHand)
										{
											this.provisionalGroupUsingLeftHands.Add(playerFist3.actorNumber);
										}
										this.playersToPropagateFrom.Enqueue(playerFist3);
										midpoint += playerFist3.position;
									}
								}
							}
						}
						if (this.playersInProvisionalGroup.Count == 1)
						{
							this.playersInProvisionalGroup.Clear();
						}
						if (this.playersInProvisionalGroup.Count > 0)
						{
							midpoint /= (float)this.playersInProvisionalGroup.Count;
						}
					}
				}
			}
		}

		// Token: 0x060062C3 RID: 25283 RVA: 0x001FC8CC File Offset: 0x001FAACC
		private void UpdateWarningSigns()
		{
			GroupJoinZoneAB groupJoinZoneAB = 0;
			if (this.myPartyMemberIDs != null)
			{
				foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
				{
					VRRig rig = rigContainer.Rig;
					if (rig.IsLocalPartyMember && !rig.isOfflineVRRig)
					{
						groupJoinZoneAB |= rig.zoneEntity.GroupZone;
					}
				}
			}
			if (groupJoinZoneAB != this.partyZone)
			{
				this.debugStr.Clear();
				foreach (RigContainer rigContainer2 in VRRigCache.ActiveRigContainers)
				{
					VRRig rig2 = rigContainer2.Rig;
					if (rig2.IsLocalPartyMember && !rig2.isOfflineVRRig)
					{
						this.debugStr.Append(string.Format("{0} in {1};", rig2.playerNameVisible, rig2.zoneEntity.GroupZone));
					}
				}
				this.partyZone = groupJoinZoneAB;
				this.CheckPartyZoneCallbacks();
			}
		}

		// Token: 0x060062C4 RID: 25284 RVA: 0x001FC9E8 File Offset: 0x001FABE8
		public void CheckPartyZoneCallbacks()
		{
			foreach (Action<GroupJoinZoneAB> action in this.groupZoneCallbacks)
			{
				action(this.partyZone);
			}
		}

		// Token: 0x060062C5 RID: 25285 RVA: 0x001FCA40 File Offset: 0x001FAC40
		[PunRPC]
		private void NotifyNoPartyToMerge(PhotonMessageInfo info)
		{
			MonkeAgent.IncrementRPCCall(info, "NotifyNoPartyToMerge");
			if (info.Sender == null || this.partyMergeIDs == null)
			{
				return;
			}
			this.partyMergeIDs.Remove(info.Sender.ActorNumber);
		}

		// Token: 0x060062C6 RID: 25286 RVA: 0x001FCA75 File Offset: 0x001FAC75
		[PunRPC]
		private void NotifyPartyMerging(int[] memberIDs, PhotonMessageInfo info)
		{
			MonkeAgent.IncrementRPCCall(info, "NotifyPartyMerging");
			if (memberIDs == null)
			{
				return;
			}
			if (memberIDs.Length > 10)
			{
				return;
			}
			this.partyMergeIDs[info.Sender.ActorNumber] = memberIDs;
		}

		// Token: 0x060062C7 RID: 25287 RVA: 0x001FCAA8 File Offset: 0x001FACA8
		public void SendAboutToGroupJoin()
		{
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				VRRig rig = rigContainer.Rig;
				Debug.Log(string.Concat(new string[]
				{
					"Sending group join to ",
					VRRigCache.ActiveRigContainers.Count.ToString(),
					" players. Party member:",
					rig.OwningNetPlayer.NickName,
					"Is offline rig",
					rig.isOfflineVRRig.ToString()
				}));
				if (rig.IsLocalPartyMember && !rig.isOfflineVRRig)
				{
					this.photonView.RPC("PartyMemberIsAboutToGroupJoin", rig.Creator.GetPlayerRef(), Array.Empty<object>());
				}
			}
		}

		// Token: 0x060062C8 RID: 25288 RVA: 0x001FCB84 File Offset: 0x001FAD84
		[PunRPC]
		private void PartyMemberIsAboutToGroupJoin(PhotonMessageInfo info)
		{
			MonkeAgent.IncrementRPCCall(info, "PartyMemberIsAboutToGroupJoin");
			this.PartMemberIsAboutToGroupJoinWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x060062C9 RID: 25289 RVA: 0x001FCBA0 File Offset: 0x001FADA0
		private void PartMemberIsAboutToGroupJoinWrapped(PhotonMessageInfoWrapped wrappedInfo)
		{
			float time = Time.time;
			float num = this.aboutToGroupJoin_CooldownUntilTimestamp;
			if (wrappedInfo.senderID < NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.aboutToGroupJoin_CooldownUntilTimestamp = Time.time + 5f;
				if (this.myPartyMembersHash.Contains(wrappedInfo.Sender.UserId))
				{
					PhotonNetworkController.Instance.DeferJoining(2f);
				}
			}
		}

		// Token: 0x060062CA RID: 25290 RVA: 0x001FCC0C File Offset: 0x001FAE0C
		private void SendPartyFormedRPC(short braceletColor, int[] memberIDs, bool forceDebug)
		{
			string text = Enum.Parse<GameModeType>(GorillaComputer.instance.currentGameMode.Value, true).ToString();
			foreach (VRRig vrrig in VRRigCache.ActiveRigs)
			{
				if (this.playersInProvisionalGroup.BinarySearch(vrrig.creator.ActorNumber) >= 0)
				{
					this.photonView.RPC("PartyFormedSuccessfully", vrrig.Creator.GetPlayerRef(), new object[] { text, braceletColor, memberIDs, forceDebug });
				}
			}
		}

		// Token: 0x060062CB RID: 25291 RVA: 0x001FCCCC File Offset: 0x001FAECC
		[PunRPC]
		private void PartyFormedSuccessfully(string partyGameMode, short braceletColor, int[] memberIDs, bool forceDebug, PhotonMessageInfo info)
		{
			MonkeAgent.IncrementRPCCall(info, "PartyFormedSuccessfully");
			this.PartyFormedSuccesfullyWrapped(partyGameMode, braceletColor, memberIDs, forceDebug, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x060062CC RID: 25292 RVA: 0x001FCCEC File Offset: 0x001FAEEC
		private void PartyFormedSuccesfullyWrapped(string partyGameMode, short braceletColor, int[] memberIDs, bool forceDebug, PhotonMessageInfoWrapped info)
		{
			if (memberIDs == null || memberIDs.Length > 10 || !memberIDs.Contains(info.Sender.ActorNumber) || this.playersInProvisionalGroup.IndexOf(info.Sender.ActorNumber) != 0 || Mathf.Abs(this.groupCreateAfterTimestamp - Time.time) > this.m_maxGroupJoinTimeDifference || !global::GorillaGameModes.GameMode.IsValidGameMode(partyGameMode))
			{
				return;
			}
			if (this.IsInParty)
			{
				string text = Enum.Parse<GameModeType>(GorillaComputer.instance.currentGameMode.Value, true).ToString();
				foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
				{
					VRRig rig = rigContainer.Rig;
					if (rig.IsLocalPartyMember && !rig.isOfflineVRRig)
					{
						this.photonView.RPC("AddPartyMembers", rig.Creator.GetPlayerRef(), new object[] { text, braceletColor, memberIDs });
					}
				}
			}
			this.suppressPartyCreationUntilTimestamp = Time.time + this.cooldownAfterCreatingGroup;
			this.DidJoinLeftHanded = this.WillJoinLeftHanded;
			this.SetNewParty(partyGameMode, braceletColor, memberIDs);
		}

		// Token: 0x060062CD RID: 25293 RVA: 0x001FCE2C File Offset: 0x001FB02C
		[PunRPC]
		private void AddPartyMembers(string partyGameMode, short braceletColor, int[] memberIDs, PhotonMessageInfo info)
		{
			this.AddPartyMembersWrapped(partyGameMode, braceletColor, memberIDs, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x060062CE RID: 25294 RVA: 0x001FCE40 File Offset: 0x001FB040
		private void AddPartyMembersWrapped(string partyGameMode, short braceletColor, int[] memberIDs, PhotonMessageInfoWrapped infoWrapped)
		{
			MonkeAgent.IncrementRPCCall(infoWrapped, "AddPartyMembersWrapped");
			if (!this.IsInParty || memberIDs == null || memberIDs.Length > 10 || !this.myPartyMembersHash.Contains(NetworkSystem.Instance.GetUserID(infoWrapped.senderID)) || !global::GorillaGameModes.GameMode.IsValidGameMode(partyGameMode))
			{
				return;
			}
			this.SetNewParty(partyGameMode, braceletColor, memberIDs);
		}

		// Token: 0x060062CF RID: 25295 RVA: 0x001FCE9C File Offset: 0x001FB09C
		private void SetNewParty(string partyGameMode, short braceletColor, int[] memberIDs)
		{
			GorillaComputer.instance.SetGameModeWithoutButton(partyGameMode);
			this.myPartyMemberIDs = new List<string>();
			FriendshipGroupDetection.userIdLookup.Clear();
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				FriendshipGroupDetection.userIdLookup.Add(rigContainer.Creator.ActorNumber, rigContainer.Creator.UserId);
			}
			foreach (int num in memberIDs)
			{
				string text;
				if (FriendshipGroupDetection.userIdLookup.TryGetValue(num, out text))
				{
					this.myPartyMemberIDs.Add(text);
				}
			}
			this.myBraceletColor = FriendshipGroupDetection.UnpackColor(braceletColor);
			GorillaTagger.Instance.StartVibration(this.DidJoinLeftHanded, this.hapticStrength, this.hapticDuration);
			this.OnPartyMembershipChanged();
			PlayerGameEvents.MiscEvent("FriendshipGroupJoined", 1);
		}

		// Token: 0x060062D0 RID: 25296 RVA: 0x001FCF90 File Offset: 0x001FB190
		public void LeaveParty()
		{
			if (this.myPartyMemberIDs == null)
			{
				return;
			}
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				VRRig rig = rigContainer.Rig;
				if (rig.IsLocalPartyMember && !rig.isOfflineVRRig)
				{
					this.photonView.RPC("PlayerLeftParty", rig.Creator.GetPlayerRef(), Array.Empty<object>());
				}
			}
			this.myPartyMemberIDs = null;
			this.OnPartyMembershipChanged();
			PhotonNetworkController.Instance.ClearDeferredJoin();
			GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x060062D1 RID: 25297 RVA: 0x001FD044 File Offset: 0x001FB244
		public void OnFailedToFollowParty()
		{
			if (!this.IsInParty)
			{
				return;
			}
			this.lastFailedToFollowPartyTime = (double)Time.time;
			this.wantsPartyRefreshPostFollowFailed = true;
		}

		// Token: 0x060062D2 RID: 25298 RVA: 0x001FD064 File Offset: 0x001FB264
		public void RefreshPartyMembers()
		{
			if (this.myPartyMemberIDs.IsNullOrEmpty<string>())
			{
				return;
			}
			Debug.Log("[FriendshipGroupDetection::RefreshPartyMembers] refreshing...");
			List<string> list = new List<string>(this.myPartyMemberIDs);
			Debug.Log("[FriendshipGroupDetection::RefreshPartyMembers] found " + string.Format("{0} current players in Room...", NetworkSystem.Instance.AllNetPlayers.Length));
			for (int i = 0; i < NetworkSystem.Instance.AllNetPlayers.Length; i++)
			{
				if (NetworkSystem.Instance.AllNetPlayers[i] != null)
				{
					list.Remove(NetworkSystem.Instance.AllNetPlayers[i].UserId);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				Debug.Log("[FriendshipGroupDetection::RefreshPartyMembers] removing missing player " + list[j] + " from party...");
				this.PlayerIDLeftParty(list[j]);
			}
			this.wantsPartyRefreshPostJoin = false;
			this.wantsPartyRefreshPostFollowFailed = false;
		}

		// Token: 0x060062D3 RID: 25299 RVA: 0x001FD143 File Offset: 0x001FB343
		[PunRPC]
		private void PlayerLeftParty(PhotonMessageInfo info)
		{
			MonkeAgent.IncrementRPCCall(info, "PlayerLeftParty");
			this.PlayerLeftPartyWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x060062D4 RID: 25300 RVA: 0x001FD15C File Offset: 0x001FB35C
		private void PlayerLeftPartyWrapped(PhotonMessageInfoWrapped infoWrapped)
		{
			if (this.myPartyMemberIDs == null)
			{
				return;
			}
			if (!this.myPartyMemberIDs.Remove(infoWrapped.Sender.UserId))
			{
				return;
			}
			if (this.myPartyMemberIDs.Count <= 1)
			{
				this.myPartyMemberIDs = null;
			}
			this.OnPartyMembershipChanged();
			GorillaTagger.Instance.StartVibration(this.DidJoinLeftHanded, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x060062D5 RID: 25301 RVA: 0x001FD1C4 File Offset: 0x001FB3C4
		private void PlayerIDLeftParty(string userID)
		{
			if (this.myPartyMemberIDs == null)
			{
				return;
			}
			if (!this.myPartyMemberIDs.Remove(userID))
			{
				return;
			}
			if (this.myPartyMemberIDs.Count <= 1)
			{
				this.myPartyMemberIDs = null;
			}
			this.OnPartyMembershipChanged();
			GorillaTagger.Instance.StartVibration(this.DidJoinLeftHanded, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x060062D6 RID: 25302 RVA: 0x001FD220 File Offset: 0x001FB420
		public void SendVerifyPartyMember(NetPlayer player)
		{
			this.photonView.RPC("VerifyPartyMember", player.GetPlayerRef(), Array.Empty<object>());
		}

		// Token: 0x060062D7 RID: 25303 RVA: 0x001FD23D File Offset: 0x001FB43D
		[PunRPC]
		private void VerifyPartyMember(PhotonMessageInfo info)
		{
			this.VerifyPartyMemberWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x060062D8 RID: 25304 RVA: 0x001FD24C File Offset: 0x001FB44C
		private void VerifyPartyMemberWrapped(PhotonMessageInfoWrapped infoWrapped)
		{
			MonkeAgent.IncrementRPCCall(infoWrapped, "VerifyPartyMemberWrapped");
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(infoWrapped.Sender, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 15, infoWrapped.SentServerTime))
			{
				return;
			}
			if (this.myPartyMemberIDs == null || !this.myPartyMemberIDs.Contains(NetworkSystem.Instance.GetUserID(infoWrapped.senderID)))
			{
				this.photonView.RPC("PlayerLeftParty", infoWrapped.Sender.GetPlayerRef(), Array.Empty<object>());
			}
		}

		// Token: 0x060062D9 RID: 25305 RVA: 0x001FD2DC File Offset: 0x001FB4DC
		public void SendRequestPartyGameMode(string gameMode)
		{
			int num = int.MaxValue;
			NetPlayer netPlayer = null;
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				VRRig rig = rigContainer.Rig;
				if (rig.IsLocalPartyMember && rig.creator.ActorNumber < num)
				{
					netPlayer = rig.creator;
					num = rig.creator.ActorNumber;
				}
			}
			if (netPlayer != null)
			{
				this.photonView.RPC("RequestPartyGameMode", netPlayer.GetPlayerRef(), new object[] { gameMode });
			}
		}

		// Token: 0x060062DA RID: 25306 RVA: 0x001FD37C File Offset: 0x001FB57C
		[PunRPC]
		private void RequestPartyGameMode(string gameMode, PhotonMessageInfo info)
		{
			this.RequestPartyGameModeWrapped(gameMode, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x060062DB RID: 25307 RVA: 0x001FD38C File Offset: 0x001FB58C
		private void RequestPartyGameModeWrapped(string gameMode, PhotonMessageInfoWrapped info)
		{
			MonkeAgent.IncrementRPCCall(info, "RequestPartyGameModeWrapped");
			if (!this.IsInParty || !this.IsInMyGroup(info.Sender.UserId) || !global::GorillaGameModes.GameMode.IsValidGameMode(gameMode))
			{
				return;
			}
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				VRRig rig = rigContainer.Rig;
				if (rig.IsLocalPartyMember)
				{
					this.photonView.RPC("NotifyPartyGameModeChanged", rig.creator.GetPlayerRef(), new object[] { gameMode });
				}
			}
		}

		// Token: 0x060062DC RID: 25308 RVA: 0x001FD434 File Offset: 0x001FB634
		[PunRPC]
		private void NotifyPartyGameModeChanged(string gameMode, PhotonMessageInfo info)
		{
			this.NotifyPartyGameModeChangedWrapped(gameMode, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x060062DD RID: 25309 RVA: 0x001FD443 File Offset: 0x001FB643
		private void NotifyPartyGameModeChangedWrapped(string gameMode, PhotonMessageInfoWrapped info)
		{
			MonkeAgent.IncrementRPCCall(info, "NotifyPartyGameModeChangedWrapped");
			if (!this.IsInParty || !this.IsInMyGroup(info.Sender.UserId) || !global::GorillaGameModes.GameMode.IsValidGameMode(gameMode))
			{
				return;
			}
			GorillaComputer.instance.SetGameModeWithoutButton(gameMode);
		}

		// Token: 0x060062DE RID: 25310 RVA: 0x001FD484 File Offset: 0x001FB684
		private void OnPartyMembershipChanged()
		{
			this.myPartyMembersHash.Clear();
			if (this.myPartyMemberIDs != null)
			{
				foreach (string text in this.myPartyMemberIDs)
				{
					this.myPartyMembersHash.Add(text);
				}
			}
			this.myBeadColors.Clear();
			FriendshipGroupDetection.tempColorLookup.Clear();
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				VRRig rig = rigContainer.Rig;
				rig.ClearPartyMemberStatus();
				if (rig.IsLocalPartyMember)
				{
					FriendshipGroupDetection.tempColorLookup.Add(rig.Creator.UserId, rig.playerColor);
				}
			}
			this.MyBraceletSelfIndex = 0;
			if (this.myPartyMemberIDs != null)
			{
				using (List<string>.Enumerator enumerator = this.myPartyMemberIDs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text2 = enumerator.Current;
						Color color;
						if (FriendshipGroupDetection.tempColorLookup.TryGetValue(text2, out color))
						{
							if (text2 == PhotonNetwork.LocalPlayer.UserId)
							{
								this.MyBraceletSelfIndex = this.myBeadColors.Count;
							}
							this.myBeadColors.Add(color);
						}
					}
					goto IL_0160;
				}
			}
			GorillaComputer.instance.SetGameModeWithoutButton(GorillaComputer.instance.lastPressedGameMode);
			this.wantsPartyRefreshPostJoin = false;
			this.wantsPartyRefreshPostFollowFailed = false;
			IL_0160:
			this.myBeadColors.Add(this.myBraceletColor);
			GorillaTagger.Instance.offlineVRRig.UpdateFriendshipBracelet();
			this.UpdateWarningSigns();
		}

		// Token: 0x060062DF RID: 25311 RVA: 0x001FD640 File Offset: 0x001FB840
		public bool IsPartyWithinCollider(GorillaFriendCollider friendCollider, bool checkLocal = false)
		{
			if (checkLocal && !friendCollider.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.LocalPlayer.UserId))
			{
				return false;
			}
			for (int i = 0; i < VRRigCache.ActiveRigContainers.Count; i++)
			{
				VRRig rig = VRRigCache.ActiveRigContainers[i].Rig;
				if (!rig.isOfflineVRRig && rig.IsLocalPartyMember && !friendCollider.playerIDsCurrentlyTouching.Contains(rig.Creator.UserId))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060062E0 RID: 25312 RVA: 0x001FD6BF File Offset: 0x001FB8BF
		public static short PackColor(Color col)
		{
			return (short)(Mathf.RoundToInt(col.r * 9f) + Mathf.RoundToInt(col.g * 9f) * 10 + Mathf.RoundToInt(col.b * 9f) * 100);
		}

		// Token: 0x060062E1 RID: 25313 RVA: 0x001FD700 File Offset: 0x001FB900
		public static Color UnpackColor(short data)
		{
			return new Color
			{
				r = (float)(data % 10) / 9f,
				g = (float)(data / 10 % 10) / 9f,
				b = (float)(data / 100 % 10) / 9f
			};
		}

		// Token: 0x04007166 RID: 29030
		[SerializeField]
		private float detectionRadius = 0.5f;

		// Token: 0x04007167 RID: 29031
		[SerializeField]
		private float groupTime = 5f;

		// Token: 0x04007168 RID: 29032
		[SerializeField]
		private float cooldownAfterCreatingGroup = 5f;

		// Token: 0x04007169 RID: 29033
		[SerializeField]
		private float hapticStrength = 1.5f;

		// Token: 0x0400716A RID: 29034
		[SerializeField]
		private float hapticDuration = 2f;

		// Token: 0x0400716B RID: 29035
		[SerializeField]
		private double joinedRoomRefreshPartyDelay = 30.0;

		// Token: 0x0400716C RID: 29036
		[SerializeField]
		private double failedToFollowRefreshPartyDelay = 30.0;

		// Token: 0x0400716D RID: 29037
		public bool debug;

		// Token: 0x0400716E RID: 29038
		public double offset = 0.5;

		// Token: 0x0400716F RID: 29039
		[SerializeField]
		private float m_maxGroupJoinTimeDifference = 1f;

		// Token: 0x04007170 RID: 29040
		private List<string> myPartyMemberIDs;

		// Token: 0x04007171 RID: 29041
		private HashSet<string> myPartyMembersHash = new HashSet<string>();

		// Token: 0x04007176 RID: 29046
		private List<Action<GroupJoinZoneAB>> groupZoneCallbacks = new List<Action<GroupJoinZoneAB>>();

		// Token: 0x04007177 RID: 29047
		[SerializeField]
		private GTColor.HSVRanges braceletRandomColorHSVRanges;

		// Token: 0x04007178 RID: 29048
		public GameObject friendshipBubble;

		// Token: 0x04007179 RID: 29049
		public AudioClip fistBumpInterruptedAudio;

		// Token: 0x0400717A RID: 29050
		private ParticleSystem particleSystem;

		// Token: 0x0400717B RID: 29051
		private AudioSource audioSource;

		// Token: 0x0400717C RID: 29052
		private double lastJoinedRoomTime;

		// Token: 0x0400717D RID: 29053
		private bool wantsPartyRefreshPostJoin;

		// Token: 0x0400717E RID: 29054
		private double lastFailedToFollowPartyTime;

		// Token: 0x0400717F RID: 29055
		private bool wantsPartyRefreshPostFollowFailed;

		// Token: 0x04007181 RID: 29057
		private Queue<FriendshipGroupDetection.PlayerFist> playersToPropagateFrom = new Queue<FriendshipGroupDetection.PlayerFist>();

		// Token: 0x04007182 RID: 29058
		private List<int> playersInProvisionalGroup = new List<int>();

		// Token: 0x04007183 RID: 29059
		private List<int> provisionalGroupUsingLeftHands = new List<int>();

		// Token: 0x04007184 RID: 29060
		private List<int> tempIntList = new List<int>();

		// Token: 0x04007185 RID: 29061
		private bool amFirstProvisionalPlayer;

		// Token: 0x04007186 RID: 29062
		private Dictionary<int, int[]> partyMergeIDs = new Dictionary<int, int[]>();

		// Token: 0x04007187 RID: 29063
		private float groupCreateAfterTimestamp;

		// Token: 0x04007188 RID: 29064
		private float playEffectsAfterTimestamp;

		// Token: 0x04007189 RID: 29065
		[SerializeField]
		private float playEffectsDelay;

		// Token: 0x0400718A RID: 29066
		private float suppressPartyCreationUntilTimestamp;

		// Token: 0x0400718C RID: 29068
		private bool WillJoinLeftHanded;

		// Token: 0x0400718D RID: 29069
		private static readonly ProfilerMarker profiler_Tick = new ProfilerMarker("GT/FriendshipGroupDetection.Tick");

		// Token: 0x0400718E RID: 29070
		private List<FriendshipGroupDetection.PlayerFist> playersMakingFists = new List<FriendshipGroupDetection.PlayerFist>();

		// Token: 0x0400718F RID: 29071
		private static readonly ProfilerMarker profiler_updateProvisionalGroup = new ProfilerMarker("GT/FriendshipGroupDetection.UpdateProvisionalGroup");

		// Token: 0x04007190 RID: 29072
		private StringBuilder debugStr = new StringBuilder();

		// Token: 0x04007191 RID: 29073
		private float aboutToGroupJoin_CooldownUntilTimestamp;

		// Token: 0x04007192 RID: 29074
		private static Dictionary<int, string> userIdLookup = new Dictionary<int, string>();

		// Token: 0x04007193 RID: 29075
		private static Dictionary<string, Color> tempColorLookup = new Dictionary<string, Color>();

		// Token: 0x02000F89 RID: 3977
		private struct PlayerFist
		{
			// Token: 0x04007194 RID: 29076
			public int actorNumber;

			// Token: 0x04007195 RID: 29077
			public Vector3 position;

			// Token: 0x04007196 RID: 29078
			public bool isLeftHand;
		}
	}
}
