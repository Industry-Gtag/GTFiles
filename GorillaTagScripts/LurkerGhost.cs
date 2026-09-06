using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000F9C RID: 3996
	[NetworkBehaviourWeaved(6)]
	public class LurkerGhost : NetworkComponent
	{
		// Token: 0x0600636A RID: 25450 RVA: 0x001FF5EE File Offset: 0x001FD7EE
		protected override void Awake()
		{
			base.Awake();
			this.possibleTargets = new List<NetPlayer>();
			this.targetPlayer = null;
			this.targetTransform = null;
			this.targetVRRig = null;
		}

		// Token: 0x0600636B RID: 25451 RVA: 0x001FF616 File Offset: 0x001FD816
		protected override void Start()
		{
			base.Start();
			this.waypointRegions = this.waypointsContainer.GetComponentsInChildren<ZoneBasedObject>();
			this.PickNextWaypoint();
			this.ChangeState(LurkerGhost.ghostState.patrol);
		}

		// Token: 0x0600636C RID: 25452 RVA: 0x001FF63C File Offset: 0x001FD83C
		private void LateUpdate()
		{
			this.UpdateState();
			this.UpdateGhostVisibility();
		}

		// Token: 0x0600636D RID: 25453 RVA: 0x001FF64C File Offset: 0x001FD84C
		private void PickNextWaypoint()
		{
			if (this.waypoints.Count == 0 || this.lastWaypointRegion == null || !this.lastWaypointRegion.IsLocalPlayerInZone())
			{
				ZoneBasedObject zoneBasedObject = ZoneBasedObject.SelectRandomEligible(this.waypointRegions, "");
				if (zoneBasedObject == null)
				{
					zoneBasedObject = this.lastWaypointRegion;
				}
				if (zoneBasedObject == null)
				{
					return;
				}
				this.lastWaypointRegion = zoneBasedObject;
				this.waypoints.Clear();
				foreach (object obj in zoneBasedObject.transform)
				{
					Transform transform = (Transform)obj;
					this.waypoints.Add(transform);
				}
			}
			int num = Random.Range(0, this.waypoints.Count);
			this.currentWaypoint = this.waypoints[num];
			this.targetRotation = Quaternion.LookRotation(this.currentWaypoint.position - base.transform.position);
			this.waypoints.RemoveAt(num);
		}

		// Token: 0x0600636E RID: 25454 RVA: 0x001FF76C File Offset: 0x001FD96C
		private void Patrol()
		{
			Transform transform = this.currentWaypoint;
			if (transform != null)
			{
				base.transform.position = Vector3.MoveTowards(base.transform.position, transform.position, this.patrolSpeed * Time.deltaTime);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, 360f * Time.deltaTime);
			}
		}

		// Token: 0x0600636F RID: 25455 RVA: 0x001FF7E4 File Offset: 0x001FD9E4
		private void PlaySound(AudioClip clip, bool loop)
		{
			if (this.audioSource && this.audioSource.isPlaying)
			{
				this.audioSource.GTStop();
			}
			if (this.audioSource && clip != null)
			{
				this.audioSource.clip = clip;
				this.audioSource.loop = loop;
				this.audioSource.GTPlay();
			}
		}

		// Token: 0x06006370 RID: 25456 RVA: 0x001FF850 File Offset: 0x001FDA50
		private bool PickPlayer(float maxDistance)
		{
			if (base.IsMine)
			{
				this.possibleTargets.Clear();
				for (int i = 0; i < VRRigCache.ActiveRigContainers.Count; i++)
				{
					if ((VRRigCache.ActiveRigContainers[i].transform.position - base.transform.position).magnitude < maxDistance && VRRigCache.ActiveRigContainers[i].Creator != this.targetPlayer)
					{
						this.possibleTargets.Add(VRRigCache.ActiveRigContainers[i].Creator);
					}
				}
				this.targetPlayer = null;
				this.targetTransform = null;
				this.targetVRRig = null;
				if (this.possibleTargets.Count > 0)
				{
					int num = Random.Range(0, this.possibleTargets.Count);
					this.PickPlayer(this.possibleTargets[num]);
				}
			}
			else
			{
				this.targetPlayer = null;
				this.targetTransform = null;
				this.targetVRRig = null;
			}
			return this.targetPlayer != null && this.targetTransform != null;
		}

		// Token: 0x06006371 RID: 25457 RVA: 0x001FF960 File Offset: 0x001FDB60
		private void PickPlayer(NetPlayer player)
		{
			int num = VRRigCache.ActiveRigContainers.FindIndex((RigContainer x) => x.Creator != null && x.Creator == player);
			if (num > -1 && num < VRRigCache.ActiveRigContainers.Count)
			{
				VRRig rig = VRRigCache.ActiveRigContainers[num].Rig;
				this.targetPlayer = rig.creator;
				this.targetTransform = rig.head.rigTarget;
				this.targetVRRig = rig;
			}
		}

		// Token: 0x06006372 RID: 25458 RVA: 0x001FF9D8 File Offset: 0x001FDBD8
		private void SeekPlayer()
		{
			if (this.targetTransform.IsNull())
			{
				this.ChangeState(LurkerGhost.ghostState.patrol);
				return;
			}
			this.targetPosition = this.targetTransform.position + this.targetTransform.forward.x0z() * this.seekAheadDistance;
			this.targetRotation = Quaternion.LookRotation(this.targetTransform.position - base.transform.position);
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPosition, this.seekSpeed * Time.deltaTime);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, 720f * Time.deltaTime);
		}

		// Token: 0x06006373 RID: 25459 RVA: 0x001FFAAC File Offset: 0x001FDCAC
		private void ChargeAtPlayer()
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPosition, this.chargeSpeed * Time.deltaTime);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, 720f * Time.deltaTime);
		}

		// Token: 0x06006374 RID: 25460 RVA: 0x001FFB14 File Offset: 0x001FDD14
		private void UpdateGhostVisibility()
		{
			switch (this.currentState)
			{
			case LurkerGhost.ghostState.patrol:
				this.meshRenderer.sharedMaterial = this.scryableMaterial;
				this.bonesMeshRenderer.sharedMaterial = this.scryableMaterialBones;
				return;
			case LurkerGhost.ghostState.seek:
			case LurkerGhost.ghostState.charge:
				if (this.targetPlayer == NetworkSystem.Instance.LocalPlayer || this.passingPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.meshRenderer.sharedMaterial = this.visibleMaterial;
					this.bonesMeshRenderer.sharedMaterial = this.visibleMaterialBones;
					return;
				}
				this.meshRenderer.sharedMaterial = this.scryableMaterial;
				this.bonesMeshRenderer.sharedMaterial = this.scryableMaterialBones;
				return;
			case LurkerGhost.ghostState.possess:
				if (this.targetPlayer == NetworkSystem.Instance.LocalPlayer || this.passingPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.meshRenderer.sharedMaterial = this.visibleMaterial;
					this.bonesMeshRenderer.sharedMaterial = this.visibleMaterialBones;
					return;
				}
				this.meshRenderer.sharedMaterial = this.scryableMaterial;
				this.bonesMeshRenderer.sharedMaterial = this.scryableMaterialBones;
				return;
			default:
				return;
			}
		}

		// Token: 0x06006375 RID: 25461 RVA: 0x001FFC38 File Offset: 0x001FDE38
		private void HauntObjects()
		{
			Collider[] array = new Collider[20];
			int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.sphereColliderRadius, array);
			for (int i = 0; i < num; i++)
			{
				if (array[i].CompareTag("HauntedObject"))
				{
					UnityAction<GameObject> triggerHauntedObjects = this.TriggerHauntedObjects;
					if (triggerHauntedObjects != null)
					{
						triggerHauntedObjects(array[i].gameObject);
					}
				}
			}
		}

		// Token: 0x06006376 RID: 25462 RVA: 0x001FFC9C File Offset: 0x001FDE9C
		private void ChangeState(LurkerGhost.ghostState newState)
		{
			this.currentState = newState;
			VRRig vrrig = null;
			switch (this.currentState)
			{
			case LurkerGhost.ghostState.patrol:
				this.PlaySound(this.patrolAudio, true);
				this.passingPlayer = null;
				this.cooldownTimeRemaining = Random.Range(this.cooldownDuration, this.maxCooldownDuration);
				this.currentRepeatHuntTimes = 0;
				break;
			case LurkerGhost.ghostState.charge:
				this.PlaySound(this.huntAudio, false);
				this.targetPosition = this.targetTransform.position;
				this.targetRotation = Quaternion.LookRotation(this.targetTransform.position - base.transform.position);
				break;
			case LurkerGhost.ghostState.possess:
				if (this.targetPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.PlaySound(this.possessedAudio, true);
					GorillaTagger.Instance.StartVibration(true, this.hapticStrength, this.hapticDuration);
					GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
				}
				vrrig = GorillaGameManager.StaticFindRigForPlayer(this.targetPlayer);
				break;
			}
			Shader.SetGlobalFloat(this._BlackAndWhite, (float)((newState == LurkerGhost.ghostState.possess && this.targetPlayer == NetworkSystem.Instance.LocalPlayer) ? 1 : 0));
			if (vrrig != this.lastHauntedVRRig && this.lastHauntedVRRig != null)
			{
				this.lastHauntedVRRig.IsHaunted = false;
			}
			if (vrrig != null)
			{
				vrrig.IsHaunted = true;
			}
			this.lastHauntedVRRig = vrrig;
			this.UpdateGhostVisibility();
		}

		// Token: 0x06006377 RID: 25463 RVA: 0x001FFE1A File Offset: 0x001FE01A
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			Shader.SetGlobalFloat(this._BlackAndWhite, 0f);
		}

		// Token: 0x06006378 RID: 25464 RVA: 0x001FFE38 File Offset: 0x001FE038
		private void UpdateState()
		{
			switch (this.currentState)
			{
			case LurkerGhost.ghostState.patrol:
				this.Patrol();
				if (base.IsMine)
				{
					if (this.currentWaypoint == null || Vector3.Distance(base.transform.position, this.currentWaypoint.position) < 0.2f)
					{
						this.PickNextWaypoint();
					}
					this.cooldownTimeRemaining -= Time.deltaTime;
					if (this.cooldownTimeRemaining <= 0f)
					{
						this.cooldownTimeRemaining = 0f;
						if (this.PickPlayer(this.maxHuntDistance))
						{
							this.ChangeState(LurkerGhost.ghostState.seek);
							return;
						}
					}
				}
				break;
			case LurkerGhost.ghostState.seek:
				this.SeekPlayer();
				if (base.IsMine && (this.targetPosition - base.transform.position).sqrMagnitude < this.seekCloseEnoughDistance * this.seekCloseEnoughDistance)
				{
					this.ChangeState(LurkerGhost.ghostState.charge);
					return;
				}
				break;
			case LurkerGhost.ghostState.charge:
				this.ChargeAtPlayer();
				if (base.IsMine && (this.targetPosition - base.transform.position).sqrMagnitude < 0.25f)
				{
					if ((this.targetTransform.position - this.targetPosition).magnitude < this.minCatchDistance)
					{
						this.ChangeState(LurkerGhost.ghostState.possess);
						return;
					}
					this.huntedPassedTime = 0f;
					this.ChangeState(LurkerGhost.ghostState.patrol);
					return;
				}
				break;
			case LurkerGhost.ghostState.possess:
				if (this.targetTransform != null)
				{
					float num = this.SpookyMagicNumbers.x + MathF.Abs(MathF.Sin(Time.time * this.SpookyMagicNumbers.y));
					float num2 = this.HauntedMagicNumbers.x * MathF.Sin(Time.time * this.HauntedMagicNumbers.y) + this.HauntedMagicNumbers.z * MathF.Sin(Time.time * this.HauntedMagicNumbers.w);
					float num3 = 0.5f + 0.5f * MathF.Sin(Time.time * this.SpookyMagicNumbers.z);
					Vector3 vector = this.targetTransform.position + new Vector3(num * (float)Math.Sin((double)num2), num3, num * (float)Math.Cos((double)num2));
					base.transform.position = Vector3.MoveTowards(base.transform.position, vector, this.chargeSpeed);
					base.transform.rotation = Quaternion.LookRotation(base.transform.position - this.targetTransform.position);
				}
				if (base.IsMine)
				{
					this.huntedPassedTime += Time.deltaTime;
					if (this.huntedPassedTime >= this.PossessionDuration)
					{
						this.huntedPassedTime = 0f;
						if (this.hauntNeighbors && this.currentRepeatHuntTimes < this.maxRepeatHuntTimes && this.PickPlayer(this.maxRepeatHuntDistance))
						{
							this.currentRepeatHuntTimes++;
							this.ChangeState(LurkerGhost.ghostState.seek);
							return;
						}
						this.ChangeState(LurkerGhost.ghostState.patrol);
					}
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x17000980 RID: 2432
		// (get) Token: 0x06006379 RID: 25465 RVA: 0x00200145 File Offset: 0x001FE345
		// (set) Token: 0x0600637A RID: 25466 RVA: 0x0020016F File Offset: 0x001FE36F
		[Networked]
		[NetworkedWeaved(0, 6)]
		private unsafe LurkerGhost.LurkerGhostData Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing LurkerGhost.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(LurkerGhost.LurkerGhostData*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing LurkerGhost.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(LurkerGhost.LurkerGhostData*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x0600637B RID: 25467 RVA: 0x0020019A File Offset: 0x001FE39A
		public override void WriteDataFusion()
		{
			this.Data = new LurkerGhost.LurkerGhostData(this.currentState, this.currentIndex, this.targetPlayer.ActorNumber, this.targetPosition);
		}

		// Token: 0x0600637C RID: 25468 RVA: 0x002001C4 File Offset: 0x001FE3C4
		public override void ReadDataFusion()
		{
			this.ReadDataShared(this.Data.CurrentState, this.Data.CurrentIndex, this.Data.TargetActor, this.Data.TargetPos);
		}

		// Token: 0x0600637D RID: 25469 RVA: 0x00200210 File Offset: 0x001FE410
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			stream.SendNext(this.currentState);
			stream.SendNext(this.currentIndex);
			if (this.targetPlayer != null)
			{
				stream.SendNext(this.targetPlayer.ActorNumber);
			}
			else
			{
				stream.SendNext(-1);
			}
			stream.SendNext(this.targetPosition);
		}

		// Token: 0x0600637E RID: 25470 RVA: 0x0020028C File Offset: 0x001FE48C
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			LurkerGhost.ghostState ghostState = (LurkerGhost.ghostState)stream.ReceiveNext();
			int num = (int)stream.ReceiveNext();
			int num2 = (int)stream.ReceiveNext();
			Vector3 vector = (Vector3)stream.ReceiveNext();
			this.ReadDataShared(ghostState, num, num2, vector);
		}

		// Token: 0x0600637F RID: 25471 RVA: 0x002002E4 File Offset: 0x001FE4E4
		private void ReadDataShared(LurkerGhost.ghostState state, int index, int targetActorNumber, Vector3 targetPos)
		{
			LurkerGhost.ghostState ghostState = this.currentState;
			this.currentState = state;
			this.currentIndex = index;
			NetPlayer netPlayer = this.targetPlayer;
			this.targetPlayer = NetworkSystem.Instance.GetPlayer(targetActorNumber);
			this.targetPosition = targetPos;
			float num = 10000f;
			if (!(in this.targetPosition).IsValid(in num))
			{
				RigContainer rigContainer;
				if (VRRigCache.Instance.TryGetVrrig(this.targetPlayer, out rigContainer))
				{
					this.targetPosition = (this.targetPlayer.IsLocal ? rigContainer.Rig.transform.position : rigContainer.Rig.syncPos);
				}
				else
				{
					this.targetPosition = base.transform.position;
				}
			}
			if (this.targetPlayer != netPlayer)
			{
				this.PickPlayer(this.targetPlayer);
			}
			if (ghostState != this.currentState || this.targetPlayer != netPlayer)
			{
				this.ChangeState(this.currentState);
			}
		}

		// Token: 0x06006380 RID: 25472 RVA: 0x002003C5 File Offset: 0x001FE5C5
		public override void OnOwnerChange(Player newOwner, Player previousOwner)
		{
			base.OnOwnerChange(newOwner, previousOwner);
			if (newOwner == PhotonNetwork.LocalPlayer)
			{
				this.ChangeState(this.currentState);
			}
		}

		// Token: 0x06006382 RID: 25474 RVA: 0x002004E8 File Offset: 0x001FE6E8
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x06006383 RID: 25475 RVA: 0x00200500 File Offset: 0x001FE700
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x04007204 RID: 29188
		public float patrolSpeed = 3f;

		// Token: 0x04007205 RID: 29189
		public float seekSpeed = 6f;

		// Token: 0x04007206 RID: 29190
		public float chargeSpeed = 6f;

		// Token: 0x04007207 RID: 29191
		[Tooltip("Cooldown until the next time the ghost needs to hunt a new player")]
		public float cooldownDuration = 10f;

		// Token: 0x04007208 RID: 29192
		[Tooltip("Max Cooldown (randomized)")]
		public float maxCooldownDuration = 10f;

		// Token: 0x04007209 RID: 29193
		[Tooltip("How long the possession effects should last")]
		public float PossessionDuration = 15f;

		// Token: 0x0400720A RID: 29194
		[Tooltip("Hunted objects within this radius will get triggered ")]
		public float sphereColliderRadius = 2f;

		// Token: 0x0400720B RID: 29195
		[Tooltip("Maximum distance to the possible player to get hunted")]
		public float maxHuntDistance = 20f;

		// Token: 0x0400720C RID: 29196
		[Tooltip("Minimum distance from the player to start the possession effects")]
		public float minCatchDistance = 2f;

		// Token: 0x0400720D RID: 29197
		[Tooltip("Maximum distance to the possible player to get repeat hunted")]
		public float maxRepeatHuntDistance = 5f;

		// Token: 0x0400720E RID: 29198
		[Tooltip("Maximum times the lurker can haunt a nearby player before going back on cooldown")]
		public int maxRepeatHuntTimes = 3;

		// Token: 0x0400720F RID: 29199
		[Tooltip("Time in seconds before a haunted player can pass the lurker to another player by tagging")]
		public float tagCoolDown = 2f;

		// Token: 0x04007210 RID: 29200
		[Tooltip("UP & DOWN, IN & OUT")]
		public Vector3 SpookyMagicNumbers = new Vector3(1f, 1f, 1f);

		// Token: 0x04007211 RID: 29201
		[Tooltip("SPIN, SPIN, SPIN, SPIN")]
		public Vector4 HauntedMagicNumbers = new Vector4(1f, 2f, 3f, 1f);

		// Token: 0x04007212 RID: 29202
		[Tooltip("Haptic vibration when haunted by the ghost")]
		public float hapticStrength = 1f;

		// Token: 0x04007213 RID: 29203
		public float hapticDuration = 1.5f;

		// Token: 0x04007214 RID: 29204
		public GameObject waypointsContainer;

		// Token: 0x04007215 RID: 29205
		private ZoneBasedObject[] waypointRegions;

		// Token: 0x04007216 RID: 29206
		private ZoneBasedObject lastWaypointRegion;

		// Token: 0x04007217 RID: 29207
		private List<Transform> waypoints = new List<Transform>();

		// Token: 0x04007218 RID: 29208
		private Transform currentWaypoint;

		// Token: 0x04007219 RID: 29209
		public Material visibleMaterial;

		// Token: 0x0400721A RID: 29210
		public Material scryableMaterial;

		// Token: 0x0400721B RID: 29211
		public Material visibleMaterialBones;

		// Token: 0x0400721C RID: 29212
		public Material scryableMaterialBones;

		// Token: 0x0400721D RID: 29213
		public MeshRenderer meshRenderer;

		// Token: 0x0400721E RID: 29214
		public MeshRenderer bonesMeshRenderer;

		// Token: 0x0400721F RID: 29215
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04007220 RID: 29216
		public AudioClip patrolAudio;

		// Token: 0x04007221 RID: 29217
		public AudioClip huntAudio;

		// Token: 0x04007222 RID: 29218
		public AudioClip possessedAudio;

		// Token: 0x04007223 RID: 29219
		public ThrowableSetDressing scryingGlass;

		// Token: 0x04007224 RID: 29220
		public float scryingAngerAngle;

		// Token: 0x04007225 RID: 29221
		public float scryingAngerDelay;

		// Token: 0x04007226 RID: 29222
		public float seekAheadDistance;

		// Token: 0x04007227 RID: 29223
		public float seekCloseEnoughDistance;

		// Token: 0x04007228 RID: 29224
		private float scryingAngerAfterTimestamp;

		// Token: 0x04007229 RID: 29225
		private int currentRepeatHuntTimes;

		// Token: 0x0400722A RID: 29226
		public UnityAction<GameObject> TriggerHauntedObjects;

		// Token: 0x0400722B RID: 29227
		private int currentIndex;

		// Token: 0x0400722C RID: 29228
		private LurkerGhost.ghostState currentState;

		// Token: 0x0400722D RID: 29229
		private float cooldownTimeRemaining;

		// Token: 0x0400722E RID: 29230
		private List<NetPlayer> possibleTargets;

		// Token: 0x0400722F RID: 29231
		private NetPlayer targetPlayer;

		// Token: 0x04007230 RID: 29232
		private Transform targetTransform;

		// Token: 0x04007231 RID: 29233
		private float huntedPassedTime;

		// Token: 0x04007232 RID: 29234
		private Vector3 targetPosition;

		// Token: 0x04007233 RID: 29235
		private Quaternion targetRotation;

		// Token: 0x04007234 RID: 29236
		private VRRig targetVRRig;

		// Token: 0x04007235 RID: 29237
		private ShaderHashId _BlackAndWhite = "_BlackAndWhite";

		// Token: 0x04007236 RID: 29238
		private VRRig lastHauntedVRRig;

		// Token: 0x04007237 RID: 29239
		private float nextTagTime;

		// Token: 0x04007238 RID: 29240
		private NetPlayer passingPlayer;

		// Token: 0x04007239 RID: 29241
		[SerializeField]
		private bool hauntNeighbors = true;

		// Token: 0x0400723A RID: 29242
		[WeaverGenerated]
		[DefaultForProperty("Data", 0, 6)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private LurkerGhost.LurkerGhostData _Data;

		// Token: 0x02000F9D RID: 3997
		private enum ghostState
		{
			// Token: 0x0400723C RID: 29244
			patrol,
			// Token: 0x0400723D RID: 29245
			seek,
			// Token: 0x0400723E RID: 29246
			charge,
			// Token: 0x0400723F RID: 29247
			possess
		}

		// Token: 0x02000F9E RID: 3998
		[NetworkStructWeaved(6)]
		[StructLayout(LayoutKind.Explicit, Size = 24)]
		private struct LurkerGhostData : INetworkStruct
		{
			// Token: 0x17000981 RID: 2433
			// (get) Token: 0x06006384 RID: 25476 RVA: 0x00200514 File Offset: 0x001FE714
			// (set) Token: 0x06006385 RID: 25477 RVA: 0x0020051C File Offset: 0x001FE71C
			public LurkerGhost.ghostState CurrentState { readonly get; set; }

			// Token: 0x17000982 RID: 2434
			// (get) Token: 0x06006386 RID: 25478 RVA: 0x00200525 File Offset: 0x001FE725
			// (set) Token: 0x06006387 RID: 25479 RVA: 0x0020052D File Offset: 0x001FE72D
			public int CurrentIndex { readonly get; set; }

			// Token: 0x17000983 RID: 2435
			// (get) Token: 0x06006388 RID: 25480 RVA: 0x00200536 File Offset: 0x001FE736
			// (set) Token: 0x06006389 RID: 25481 RVA: 0x0020053E File Offset: 0x001FE73E
			public int TargetActor { readonly get; set; }

			// Token: 0x17000984 RID: 2436
			// (get) Token: 0x0600638A RID: 25482 RVA: 0x00200547 File Offset: 0x001FE747
			// (set) Token: 0x0600638B RID: 25483 RVA: 0x00200559 File Offset: 0x001FE759
			[Networked]
			[NetworkedWeaved(3, 3)]
			public unsafe Vector3 TargetPos
			{
				readonly get
				{
					return *(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._TargetPos);
				}
				set
				{
					*(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._TargetPos) = value;
				}
			}

			// Token: 0x0600638C RID: 25484 RVA: 0x0020056C File Offset: 0x001FE76C
			public LurkerGhostData(LurkerGhost.ghostState state, int index, int actor, Vector3 pos)
			{
				this.CurrentState = state;
				this.CurrentIndex = index;
				this.TargetActor = actor;
				this.TargetPos = pos;
			}

			// Token: 0x04007243 RID: 29251
			[FixedBufferProperty(typeof(Vector3), typeof(UnityValueSurrogate@ElementReaderWriterVector3), 0, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(12)]
			private FixedStorage@3 _TargetPos;
		}
	}
}
