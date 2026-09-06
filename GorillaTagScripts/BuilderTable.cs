using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using BoingKit;
using CjLib;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using GorillaTagScripts.Builder;
using Ionic.Zlib;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using Unity.Collections;
using Unity.Jobs;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000F62 RID: 3938
	public class BuilderTable : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x1700095F RID: 2399
		// (get) Token: 0x060060DF RID: 24799 RVA: 0x001EBE01 File Offset: 0x001EA001
		// (set) Token: 0x060060E0 RID: 24800 RVA: 0x001EBE09 File Offset: 0x001EA009
		public bool TickRunning { get; set; }

		// Token: 0x17000960 RID: 2400
		// (get) Token: 0x060060E1 RID: 24801 RVA: 0x001EBE12 File Offset: 0x001EA012
		[HideInInspector]
		public float gridSize
		{
			get
			{
				return this.pieceScale / 2f;
			}
		}

		// Token: 0x060060E2 RID: 24802 RVA: 0x001EBE20 File Offset: 0x001EA020
		private void ExecuteAction(BuilderAction action)
		{
			if (!this.isTableMutable)
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(action.pieceId);
			BuilderPiece piece2 = this.GetPiece(action.parentPieceId);
			int playerActorNumber = action.playerActorNumber;
			bool flag = PhotonNetwork.LocalPlayer.ActorNumber == action.playerActorNumber;
			switch (action.type)
			{
			case BuilderActionType.AttachToPlayer:
			{
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				RigContainer rigContainer;
				if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(playerActorNumber), out rigContainer))
				{
					string.Format("Execute Builder Action {0} {1} {2} {3} {4}", new object[] { action.localCommandId, action.type, action.pieceId, action.playerActorNumber, action.isLeftHand });
					return;
				}
				BodyDockPositions myBodyDockPositions = rigContainer.Rig.myBodyDockPositions;
				Transform transform = (action.isLeftHand ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform);
				piece.SetParentHeld(transform, playerActorNumber, action.isLeftHand);
				piece.transform.SetLocalPositionAndRotation(action.localPosition, action.localRotation);
				BuilderPiece.State state = (flag ? BuilderPiece.State.GrabbedLocal : BuilderPiece.State.Grabbed);
				piece.SetState(state, false);
				if (!flag)
				{
					BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
				}
				if (flag)
				{
					BuilderPieceInteractor.instance.AddPieceToHeld(piece, action.isLeftHand, action.localPosition, action.localRotation);
					return;
				}
				break;
			}
			case BuilderActionType.DetachFromPlayer:
				if (flag)
				{
					BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
				}
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				return;
			case BuilderActionType.AttachToPiece:
			{
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				Quaternion identity = Quaternion.identity;
				Vector3 zero = Vector3.zero;
				Vector3 position = piece.transform.position;
				Quaternion rotation = piece.transform.rotation;
				if (piece2 != null)
				{
					piece.BumpTwistToPositionRotation(action.twist, action.bumpOffsetx, action.bumpOffsetz, action.attachIndex, piece2.gridPlanes[action.parentAttachIndex], out zero, out identity, out position, out rotation);
				}
				piece.transform.SetPositionAndRotation(position, rotation);
				BuilderPiece.State state2;
				if (piece2 == null)
				{
					state2 = BuilderPiece.State.AttachedAndPlaced;
				}
				else if (piece2.isArmShelf || piece2.state == BuilderPiece.State.AttachedToArm)
				{
					state2 = BuilderPiece.State.AttachedToArm;
				}
				else if (piece2.isBuiltIntoTable || piece2.state == BuilderPiece.State.AttachedAndPlaced)
				{
					state2 = BuilderPiece.State.AttachedAndPlaced;
				}
				else if (piece2.state == BuilderPiece.State.Grabbed)
				{
					state2 = BuilderPiece.State.Grabbed;
				}
				else if (piece2.state == BuilderPiece.State.GrabbedLocal)
				{
					state2 = BuilderPiece.State.GrabbedLocal;
				}
				else
				{
					state2 = BuilderPiece.State.AttachedToDropped;
				}
				BuilderPiece rootPiece = piece2.GetRootPiece();
				this.gridPlaneData.Clear();
				this.checkGridPlaneData.Clear();
				this.allPotentialPlacements.Clear();
				BuilderTable.tempPieceSet.Clear();
				QueryParameters queryParameters = new QueryParameters
				{
					layerMask = this.allPiecesMask
				};
				OverlapSphereCommand overlapSphereCommand = new OverlapSphereCommand(position, 1f, queryParameters);
				this.nearbyPiecesCommands[0] = overlapSphereCommand;
				OverlapSphereCommand.ScheduleBatch(this.nearbyPiecesCommands, this.nearbyPiecesResults, 1, 1024, default(JobHandle)).Complete();
				int num = 0;
				while (num < 1024 && this.nearbyPiecesResults[num].instanceID != 0)
				{
					BuilderPiece builderPiece = piece;
					BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(this.nearbyPiecesResults[num].collider);
					if (builderPieceFromCollider != null && !BuilderTable.tempPieceSet.Contains(builderPieceFromCollider))
					{
						BuilderTable.tempPieceSet.Add(builderPieceFromCollider);
						if (this.CanPiecesPotentiallyOverlap(builderPiece, rootPiece, state2, builderPieceFromCollider))
						{
							for (int i = 0; i < builderPieceFromCollider.gridPlanes.Count; i++)
							{
								BuilderGridPlaneData builderGridPlaneData = new BuilderGridPlaneData(builderPieceFromCollider.gridPlanes[i], -1);
								this.checkGridPlaneData.Add(in builderGridPlaneData);
							}
						}
					}
					num++;
				}
				BuilderTableJobs.BuildTestPieceListForJob(piece, this.gridPlaneData);
				BuilderPotentialPlacement builderPotentialPlacement = new BuilderPotentialPlacement
				{
					localPosition = zero,
					localRotation = identity,
					attachIndex = action.attachIndex,
					parentAttachIndex = action.parentAttachIndex,
					attachPiece = piece,
					parentPiece = piece2
				};
				this.CalcAllPotentialPlacements(this.gridPlaneData, this.checkGridPlaneData, builderPotentialPlacement, this.allPotentialPlacements);
				piece.SetParentPiece(action.attachIndex, piece2, action.parentAttachIndex);
				for (int j = 0; j < this.allPotentialPlacements.Count; j++)
				{
					BuilderPotentialPlacement builderPotentialPlacement2 = this.allPotentialPlacements[j];
					BuilderAttachGridPlane builderAttachGridPlane = builderPotentialPlacement2.attachPiece.gridPlanes[builderPotentialPlacement2.attachIndex];
					BuilderAttachGridPlane builderAttachGridPlane2 = builderPotentialPlacement2.parentPiece.gridPlanes[builderPotentialPlacement2.parentAttachIndex];
					BuilderAttachGridPlane movingParentGrid = builderAttachGridPlane.GetMovingParentGrid();
					bool flag2 = movingParentGrid != null;
					BuilderAttachGridPlane movingParentGrid2 = builderAttachGridPlane2.GetMovingParentGrid();
					bool flag3 = movingParentGrid2 != null;
					if (flag2 == flag3 && (!flag2 || !(movingParentGrid != movingParentGrid2)))
					{
						SnapOverlap snapOverlap = this.builderPool.CreateSnapOverlap(builderAttachGridPlane2, builderPotentialPlacement2.attachBounds);
						builderAttachGridPlane.AddSnapOverlap(snapOverlap);
						SnapOverlap snapOverlap2 = this.builderPool.CreateSnapOverlap(builderAttachGridPlane, builderPotentialPlacement2.parentAttachBounds);
						builderAttachGridPlane2.AddSnapOverlap(snapOverlap2);
					}
				}
				piece.transform.SetLocalPositionAndRotation(zero, identity);
				if (piece2 != null && piece2.state == BuilderPiece.State.GrabbedLocal)
				{
					BuilderPiece rootPiece2 = piece2.GetRootPiece();
					BuilderPieceInteractor.instance.OnCountChangedForRoot(rootPiece2);
				}
				if (piece2 == null)
				{
					piece.SetActivateTimeStamp(action.timeStamp);
					piece.SetState(BuilderPiece.State.AttachedAndPlaced, false);
					this.SetIsDirty(true);
					if (flag)
					{
						BuilderPieceInteractor.instance.DisableCollisionsWithHands();
						return;
					}
				}
				else
				{
					if (piece2.isArmShelf || piece2.state == BuilderPiece.State.AttachedToArm)
					{
						piece.SetState(BuilderPiece.State.AttachedToArm, false);
						return;
					}
					if (piece2.isBuiltIntoTable || piece2.state == BuilderPiece.State.AttachedAndPlaced)
					{
						piece.SetActivateTimeStamp(action.timeStamp);
						piece.SetState(BuilderPiece.State.AttachedAndPlaced, false);
						if (piece2 != null)
						{
							BuilderPiece attachedBuiltInPiece = piece2.GetAttachedBuiltInPiece();
							BuilderPiecePrivatePlot builderPiecePrivatePlot;
							if (attachedBuiltInPiece != null && attachedBuiltInPiece.TryGetPlotComponent(out builderPiecePrivatePlot))
							{
								builderPiecePrivatePlot.OnPieceAttachedToPlot(piece);
							}
						}
						this.SetIsDirty(true);
						if (flag)
						{
							BuilderPieceInteractor.instance.DisableCollisionsWithHands();
							return;
						}
					}
					else
					{
						if (piece2.state == BuilderPiece.State.Grabbed)
						{
							piece.SetState(BuilderPiece.State.Grabbed, false);
							return;
						}
						if (piece2.state == BuilderPiece.State.GrabbedLocal)
						{
							piece.SetState(BuilderPiece.State.GrabbedLocal, false);
							return;
						}
						piece.SetState(BuilderPiece.State.AttachedToDropped, false);
						return;
					}
				}
				break;
			}
			case BuilderActionType.DetachFromPiece:
			{
				BuilderPiece builderPiece2 = piece;
				bool flag4 = piece.state == BuilderPiece.State.GrabbedLocal;
				if (flag4)
				{
					builderPiece2 = piece.GetRootPiece();
				}
				if (piece.state == BuilderPiece.State.AttachedAndPlaced)
				{
					this.SetIsDirty(true);
					BuilderPiece attachedBuiltInPiece2 = piece.GetAttachedBuiltInPiece();
					BuilderPiecePrivatePlot builderPiecePrivatePlot2;
					if (attachedBuiltInPiece2 != null && attachedBuiltInPiece2.TryGetPlotComponent(out builderPiecePrivatePlot2))
					{
						builderPiecePrivatePlot2.OnPieceDetachedFromPlot(piece);
					}
				}
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				if (flag4)
				{
					BuilderPieceInteractor.instance.OnCountChangedForRoot(builderPiece2);
					return;
				}
				break;
			}
			case BuilderActionType.MakePieceRoot:
				BuilderPiece.MakePieceRoot(piece);
				return;
			case BuilderActionType.DropPiece:
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				piece.SetState(BuilderPiece.State.Dropped, false);
				piece.transform.SetLocalPositionAndRotation(action.localPosition, action.localRotation);
				if (piece.rigidBody != null)
				{
					piece.rigidBody.position = action.localPosition;
					piece.rigidBody.rotation = action.localRotation;
					piece.rigidBody.linearVelocity = action.velocity;
					piece.rigidBody.angularVelocity = action.angVelocity;
					return;
				}
				break;
			case BuilderActionType.AttachToShelf:
			{
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				int attachIndex = action.attachIndex;
				bool isLeftHand = action.isLeftHand;
				int parentAttachIndex = action.parentAttachIndex;
				float x = action.velocity.x;
				piece.transform.localScale = Vector3.one;
				piece.SetState(isLeftHand ? BuilderPiece.State.OnConveyor : BuilderPiece.State.OnShelf, false);
				if (isLeftHand)
				{
					if (attachIndex >= 0 && attachIndex < this.conveyors.Count)
					{
						BuilderConveyor builderConveyor = this.conveyors[attachIndex];
						float num2 = x / builderConveyor.GetFrameMovement();
						if (PhotonNetwork.ServerTimestamp >= parentAttachIndex)
						{
							uint num3 = (uint)(PhotonNetwork.ServerTimestamp - parentAttachIndex);
							num2 += num3 / 1000f;
						}
						piece.shelfOwner = attachIndex;
						builderConveyor.OnShelfPieceCreated(piece, num2);
						return;
					}
				}
				else
				{
					if (attachIndex >= 0 && attachIndex < this.dispenserShelves.Count)
					{
						BuilderDispenserShelf builderDispenserShelf = this.dispenserShelves[attachIndex];
						piece.shelfOwner = attachIndex;
						builderDispenserShelf.OnShelfPieceCreated(piece, false);
						return;
					}
					piece.transform.SetLocalPositionAndRotation(action.localPosition, action.localRotation);
				}
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x060060E3 RID: 24803 RVA: 0x001EC6D4 File Offset: 0x001EA8D4
		public static bool AreStatesCompatibleForOverlap(BuilderPiece.State stateA, BuilderPiece.State stateB, BuilderPiece rootA, BuilderPiece rootB)
		{
			switch (stateA)
			{
			case BuilderPiece.State.None:
				return false;
			case BuilderPiece.State.AttachedAndPlaced:
				return stateB == BuilderPiece.State.AttachedAndPlaced;
			case BuilderPiece.State.AttachedToDropped:
			case BuilderPiece.State.Dropped:
			case BuilderPiece.State.OnShelf:
			case BuilderPiece.State.OnConveyor:
				return (stateB == BuilderPiece.State.AttachedToDropped || stateB == BuilderPiece.State.Dropped || stateB == BuilderPiece.State.OnShelf || stateB == BuilderPiece.State.OnConveyor) && rootA.Equals(rootB);
			case BuilderPiece.State.Grabbed:
				return stateB == BuilderPiece.State.Grabbed && rootA.Equals(rootB);
			case BuilderPiece.State.Displayed:
				return false;
			case BuilderPiece.State.GrabbedLocal:
				return stateB == BuilderPiece.State.GrabbedLocal && rootA.heldInLeftHand == rootB.heldInLeftHand;
			case BuilderPiece.State.AttachedToArm:
			{
				if (stateB != BuilderPiece.State.AttachedToArm)
				{
					return false;
				}
				object obj = ((rootA.parentPiece != null) ? rootA.parentPiece : rootA);
				BuilderPiece builderPiece = ((rootB.parentPiece != null) ? rootB.parentPiece : rootB);
				return obj.Equals(builderPiece);
			}
			default:
				return false;
			}
		}

		// Token: 0x17000961 RID: 2401
		// (get) Token: 0x060060E4 RID: 24804 RVA: 0x001EC799 File Offset: 0x001EA999
		// (set) Token: 0x060060E5 RID: 24805 RVA: 0x001EC7A1 File Offset: 0x001EA9A1
		public int CurrentSaveSlot
		{
			get
			{
				return this.currentSaveSlot;
			}
			set
			{
				if (this.saveInProgress)
				{
					return;
				}
				if (!BuilderScanKiosk.IsSaveSlotValid(value))
				{
					this.currentSaveSlot = -1;
				}
				if (this.currentSaveSlot != value)
				{
					this.SetIsDirty(true);
				}
				this.currentSaveSlot = value;
			}
		}

		// Token: 0x060060E6 RID: 24806 RVA: 0x001EC7D4 File Offset: 0x001EA9D4
		private void Awake()
		{
			if (BuilderTable.zoneToInstance == null)
			{
				BuilderTable.zoneToInstance = new Dictionary<GTZone, BuilderTable>(2);
			}
			if (!BuilderTable.zoneToInstance.TryAdd(this.tableZone, this))
			{
				Object.Destroy(this);
			}
			this.acceptableSqrDistFromCenter = Mathf.Pow(217f * this.pieceScale, 2f);
			if (this.buttonSnapRotation != null)
			{
				this.buttonSnapRotation.Setup(new Action<BuilderOptionButton, bool>(this.OnButtonFreeRotation));
				this.buttonSnapRotation.SetPressed(this.useSnapRotation);
			}
			if (this.buttonSnapPosition != null)
			{
				this.buttonSnapPosition.Setup(new Action<BuilderOptionButton, bool>(this.OnButtonFreePosition));
				this.buttonSnapPosition.SetPressed(this.usePlacementStyle > BuilderPlacementStyle.Float);
			}
			if (this.buttonSaveLayout != null)
			{
				this.buttonSaveLayout.Setup(new Action<BuilderOptionButton, bool>(this.OnButtonSaveLayout));
			}
			if (this.buttonClearLayout != null)
			{
				this.buttonClearLayout.Setup(new Action<BuilderOptionButton, bool>(this.OnButtonClearLayout));
			}
			this.isSetup = false;
			this.nextPieceId = 10000;
			BuilderTable.placedLayer = LayerMask.NameToLayer("Gorilla Object");
			BuilderTable.heldLayerLocal = LayerMask.NameToLayer("Prop");
			BuilderTable.heldLayer = LayerMask.NameToLayer("BuilderProp");
			BuilderTable.droppedLayer = LayerMask.NameToLayer("BuilderProp");
			this.currSnapParams = this.pushAndEaseParams;
			this.tableState = BuilderTable.TableState.WaitingForZoneAndRoom;
			this.inRoom = false;
			this.inBuilderZone = false;
			this.builderNetworking.SetTable(this);
			this.plotOwners = new Dictionary<int, int>(10);
			this.doesLocalPlayerOwnPlot = false;
			this.queuedBuildCommands = new List<BuilderTable.BuilderCommand>(1028);
			if (this.isTableMutable)
			{
				this.playerToArmShelfLeft = new Dictionary<int, int>(10);
				this.playerToArmShelfRight = new Dictionary<int, int>(10);
				this.rollBackBufferedCommands = new List<BuilderTable.BuilderCommand>(1028);
				this.rollBackActions = new List<BuilderAction>(1028);
				this.rollForwardCommands = new List<BuilderTable.BuilderCommand>(1028);
				this.droppedPieces = new List<BuilderPiece>(BuilderTable.DROPPED_PIECE_LIMIT + 50);
				this.droppedPieceData = new List<BuilderTable.DroppedPieceData>(BuilderTable.DROPPED_PIECE_LIMIT + 50);
				this.SetupMonkeBlocksRoom();
				this.gridPlaneData = new NativeList<BuilderGridPlaneData>(1024, Allocator.Persistent);
				this.checkGridPlaneData = new NativeList<BuilderGridPlaneData>(1024, Allocator.Persistent);
				this.nearbyPiecesResults = new NativeArray<ColliderHit>(1024, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				this.nearbyPiecesCommands = new NativeArray<OverlapSphereCommand>(1, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				this.allPotentialPlacements = new List<BuilderPotentialPlacement>(1024);
			}
			else
			{
				this.rollBackBufferedCommands = new List<BuilderTable.BuilderCommand>(128);
				this.rollBackActions = new List<BuilderAction>(128);
				this.rollForwardCommands = new List<BuilderTable.BuilderCommand>(128);
			}
			this.SetupResources();
			if (!this.isTableMutable && this.linkedTerminal != null)
			{
				this.linkedTerminal.Init(this);
			}
		}

		// Token: 0x060060E7 RID: 24807 RVA: 0x0001A297 File Offset: 0x00018497
		private void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x060060E8 RID: 24808 RVA: 0x0001A29F File Offset: 0x0001849F
		private void OnDisable()
		{
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x060060E9 RID: 24809 RVA: 0x001ECAB7 File Offset: 0x001EACB7
		public static bool TryGetBuilderTableForZone(GTZone zone, out BuilderTable table)
		{
			if (BuilderTable.zoneToInstance == null)
			{
				table = null;
				return false;
			}
			return BuilderTable.zoneToInstance.TryGetValue(zone, out table);
		}

		// Token: 0x060060EA RID: 24810 RVA: 0x001ECAD4 File Offset: 0x001EACD4
		private void SetupMonkeBlocksRoom()
		{
			if (this.shelves == null)
			{
				this.shelves = new List<BuilderShelf>(64);
			}
			if (this.shelvesRoot != null)
			{
				this.shelvesRoot.GetComponentsInChildren<BuilderShelf>(this.shelves);
			}
			this.conveyors = new List<BuilderConveyor>(32);
			this.dispenserShelves = new List<BuilderDispenserShelf>(32);
			if (this.allShelvesRoot != null)
			{
				for (int i = 0; i < this.allShelvesRoot.Count; i++)
				{
					this.allShelvesRoot[i].GetComponentsInChildren<BuilderConveyor>(BuilderTable.tempConveyors);
					this.conveyors.AddRange(BuilderTable.tempConveyors);
					BuilderTable.tempConveyors.Clear();
					this.allShelvesRoot[i].GetComponentsInChildren<BuilderDispenserShelf>(BuilderTable.tempDispensers);
					this.dispenserShelves.AddRange(BuilderTable.tempDispensers);
					BuilderTable.tempDispensers.Clear();
				}
			}
			this.recyclers = new List<BuilderRecycler>(5);
			if (this.recyclerRoot != null)
			{
				for (int j = 0; j < this.recyclerRoot.Count; j++)
				{
					this.recyclerRoot[j].GetComponentsInChildren<BuilderRecycler>(BuilderTable.tempRecyclers);
					this.recyclers.AddRange(BuilderTable.tempRecyclers);
					BuilderTable.tempRecyclers.Clear();
				}
			}
			for (int k = 0; k < this.recyclers.Count; k++)
			{
				this.recyclers[k].recyclerID = k;
				this.recyclers[k].table = this;
			}
			this.dropZones = new List<BuilderDropZone>(6);
			this.dropZoneRoot.GetComponentsInChildren<BuilderDropZone>(this.dropZones);
			for (int l = 0; l < this.dropZones.Count; l++)
			{
				this.dropZones[l].dropZoneID = l;
				this.dropZones[l].table = this;
			}
			foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
			{
				builderResourceMeter.table = this;
			}
		}

		// Token: 0x060060EB RID: 24811 RVA: 0x001ECCE0 File Offset: 0x001EAEE0
		private void SetupResources()
		{
			this.maxResources = new int[3];
			if (this.totalResources != null && this.totalResources.quantities != null)
			{
				for (int i = 0; i < this.totalResources.quantities.Count; i++)
				{
					if (this.totalResources.quantities[i].type >= BuilderResourceType.Basic && this.totalResources.quantities[i].type < BuilderResourceType.Count)
					{
						this.maxResources[(int)this.totalResources.quantities[i].type] += this.totalResources.quantities[i].count;
					}
				}
			}
			this.usedResources = new int[3];
			this.reservedResources = new int[3];
			if (this.totalReservedResources != null && this.totalReservedResources.quantities != null)
			{
				for (int j = 0; j < this.totalReservedResources.quantities.Count; j++)
				{
					if (this.totalReservedResources.quantities[j].type >= BuilderResourceType.Basic && this.totalReservedResources.quantities[j].type < BuilderResourceType.Count)
					{
						this.reservedResources[(int)this.totalReservedResources.quantities[j].type] += this.totalReservedResources.quantities[j].count;
					}
				}
			}
			this.plotMaxResources = new int[3];
			if (this.resourcesPerPrivatePlot != null && this.resourcesPerPrivatePlot.quantities != null)
			{
				for (int k = 0; k < this.resourcesPerPrivatePlot.quantities.Count; k++)
				{
					if (this.resourcesPerPrivatePlot.quantities[k].type >= BuilderResourceType.Basic && this.resourcesPerPrivatePlot.quantities[k].type < BuilderResourceType.Count)
					{
						this.plotMaxResources[(int)this.resourcesPerPrivatePlot.quantities[k].type] += this.resourcesPerPrivatePlot.quantities[k].count;
					}
				}
			}
			this.OnAvailableResourcesChange();
		}

		// Token: 0x060060EC RID: 24812 RVA: 0x001ECF28 File Offset: 0x001EB128
		private void Start()
		{
			if (NetworkSystem.Instance != null && NetworkSystem.Instance.InRoom != this.inRoom)
			{
				this.SetInRoom(NetworkSystem.Instance.InRoom);
			}
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.HandleOnZoneChanged));
			this.HandleOnZoneChanged();
			this.RequestTableConfiguration();
			this.FetchSharedBlocksStartingMapConfig();
			PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(new UnityAction<string>(this.OnTitleDataUpdate));
		}

		// Token: 0x060060ED RID: 24813 RVA: 0x001ECFB7 File Offset: 0x001EB1B7
		private void OnApplicationQuit()
		{
			this.ClearTable();
			this.tableState = BuilderTable.TableState.WaitingForZoneAndRoom;
		}

		// Token: 0x060060EE RID: 24814 RVA: 0x001ECFC8 File Offset: 0x001EB1C8
		private void OnDestroy()
		{
			PlayFabTitleDataCache.Instance.OnTitleDataUpdate.RemoveListener(new UnityAction<string>(this.OnTitleDataUpdate));
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.HandleOnZoneChanged));
			if (this.isTableMutable)
			{
				if (this.gridPlaneData.IsCreated)
				{
					this.gridPlaneData.Dispose();
				}
				if (this.checkGridPlaneData.IsCreated)
				{
					this.checkGridPlaneData.Dispose();
				}
				if (this.nearbyPiecesResults.IsCreated)
				{
					this.nearbyPiecesResults.Dispose();
				}
				if (this.nearbyPiecesCommands.IsCreated)
				{
					this.nearbyPiecesCommands.Dispose();
				}
			}
			this.DestroyData();
		}

		// Token: 0x060060EF RID: 24815 RVA: 0x001ED084 File Offset: 0x001EB284
		private void HandleOnZoneChanged()
		{
			bool flag = ZoneManagement.instance.IsZoneActive(this.tableZone);
			this.SetInBuilderZone(flag);
		}

		// Token: 0x060060F0 RID: 24816 RVA: 0x001ED0AC File Offset: 0x001EB2AC
		public void InitIfNeeded()
		{
			if (!this.isSetup)
			{
				if (BuilderSetManager.instance == null)
				{
					return;
				}
				BuilderSetManager.instance.InitPieceDictionary();
				this.builderRenderer.BuildRenderer(BuilderSetManager.pieceList);
				this.baseGridPlanes.Clear();
				this.basePieces = new List<BuilderPiece>(1024);
				for (int i = 0; i < this.builtInPieceRoots.Count; i++)
				{
					this.builtInPieceRoots[i].SetActive(true);
					this.builtInPieceRoots[i].GetComponentsInChildren<BuilderPiece>(false, BuilderTable.tempPieces);
					this.basePieces.AddRange(BuilderTable.tempPieces);
				}
				this.allPrivatePlots = new List<BuilderPiecePrivatePlot>(20);
				this.CreateData();
				for (int j = 0; j < this.basePieces.Count; j++)
				{
					BuilderPiece builderPiece = this.basePieces[j];
					builderPiece.SetTable(this);
					builderPiece.pieceId = 5 + j;
					builderPiece.SetScale(this.pieceScale);
					builderPiece.SetupPiece(this.gridSize);
					builderPiece.OnCreate();
					builderPiece.SetState(BuilderPiece.State.OnShelf, true);
					this.baseGridPlanes.AddRange(builderPiece.gridPlanes);
					BuilderPiecePrivatePlot builderPiecePrivatePlot;
					if (builderPiece.IsPrivatePlot() && builderPiece.TryGetPlotComponent(out builderPiecePrivatePlot))
					{
						this.allPrivatePlots.Add(builderPiecePrivatePlot);
					}
					this.AddPieceData(builderPiece);
				}
				this.builderPool = BuilderPool.instance;
				this.builderPool.Setup();
				base.StartCoroutine(this.builderPool.BuildFromPieceSets());
				if (this.isTableMutable)
				{
					for (int k = 0; k < this.conveyors.Count; k++)
					{
						this.conveyors[k].table = this;
						this.conveyors[k].shelfID = k;
						this.conveyors[k].Setup();
					}
					for (int l = 0; l < this.dispenserShelves.Count; l++)
					{
						this.dispenserShelves[l].table = this;
						this.dispenserShelves[l].shelfID = l;
						this.dispenserShelves[l].Setup();
					}
					this.conveyorManager.Setup(this);
					this.repelledPieceRoots = new HashSet<int>[this.repelHistoryLength];
					for (int m = 0; m < this.repelHistoryLength; m++)
					{
						this.repelledPieceRoots[m] = new HashSet<int>(10);
					}
					this.sharedBuildAreas = this.sharedBuildArea.GetComponents<BoxCollider>();
					BoxCollider[] array = this.sharedBuildAreas;
					for (int n = 0; n < array.Length; n++)
					{
						array[n].enabled = false;
					}
					this.sharedBuildArea.SetActive(false);
				}
				BoxCollider[] components = this.noBlocksArea.GetComponents<BoxCollider>();
				this.noBlocksAreas = new List<BuilderTable.BoxCheckParams>(components.Length);
				foreach (BoxCollider boxCollider in components)
				{
					boxCollider.enabled = true;
					BuilderTable.BoxCheckParams boxCheckParams = new BuilderTable.BoxCheckParams
					{
						center = boxCollider.transform.TransformPoint(boxCollider.center),
						halfExtents = Vector3.Scale(boxCollider.transform.lossyScale, boxCollider.size) / 2f,
						rotation = boxCollider.transform.rotation
					};
					this.noBlocksAreas.Add(boxCheckParams);
					boxCollider.enabled = false;
				}
				this.noBlocksArea.SetActive(false);
				this.isSetup = true;
			}
		}

		// Token: 0x060060F1 RID: 24817 RVA: 0x001ED42E File Offset: 0x001EB62E
		private void SetIsDirty(bool dirty)
		{
			if (this.isDirty != dirty)
			{
				UnityEvent<bool> onSaveDirtyChanged = this.OnSaveDirtyChanged;
				if (onSaveDirtyChanged != null)
				{
					onSaveDirtyChanged.Invoke(dirty);
				}
			}
			this.isDirty = dirty;
		}

		// Token: 0x060060F2 RID: 24818 RVA: 0x001ED454 File Offset: 0x001EB654
		private void FixedUpdate()
		{
			if (this.tableState != BuilderTable.TableState.Ready && this.tableState != BuilderTable.TableState.WaitForMasterResync)
			{
				return;
			}
			foreach (IBuilderPieceFunctional builderPieceFunctional in this.funcComponentsToRegisterFixed)
			{
				if (builderPieceFunctional != null)
				{
					this.fixedUpdateFunctionalComponents.Add(builderPieceFunctional);
				}
			}
			foreach (IBuilderPieceFunctional builderPieceFunctional2 in this.funcComponentsToUnregisterFixed)
			{
				this.fixedUpdateFunctionalComponents.Remove(builderPieceFunctional2);
			}
			this.funcComponentsToRegisterFixed.Clear();
			this.funcComponentsToUnregisterFixed.Clear();
			foreach (IBuilderPieceFunctional builderPieceFunctional3 in this.fixedUpdateFunctionalComponents)
			{
				builderPieceFunctional3.FunctionalPieceFixedUpdate();
			}
		}

		// Token: 0x060060F3 RID: 24819 RVA: 0x001ED560 File Offset: 0x001EB760
		public void Tick()
		{
			this.RunUpdate();
		}

		// Token: 0x060060F4 RID: 24820 RVA: 0x001ED568 File Offset: 0x001EB768
		private void RunUpdate()
		{
			this.InitIfNeeded();
			this.UpdateTableState();
			if (this.isTableMutable)
			{
				this.UpdateDroppedPieces(Time.deltaTime);
				this.repelHistoryIndex = (this.repelHistoryIndex + 1) % this.repelHistoryLength;
				int num = (this.repelHistoryIndex + 1) % this.repelHistoryLength;
				this.repelledPieceRoots[num].Clear();
			}
		}

		// Token: 0x060060F5 RID: 24821 RVA: 0x001ED5C6 File Offset: 0x001EB7C6
		public void AddQueuedCommand(BuilderTable.BuilderCommand cmd)
		{
			this.queuedBuildCommands.Add(cmd);
		}

		// Token: 0x060060F6 RID: 24822 RVA: 0x001ED5D4 File Offset: 0x001EB7D4
		public void ClearQueuedCommands()
		{
			if (this.queuedBuildCommands != null)
			{
				this.queuedBuildCommands.Clear();
			}
			this.RemoveRollBackActions();
			if (this.rollBackBufferedCommands != null)
			{
				this.rollBackBufferedCommands.Clear();
			}
			this.RemoveRollForwardCommands();
		}

		// Token: 0x060060F7 RID: 24823 RVA: 0x001ED608 File Offset: 0x001EB808
		public int GetNumQueuedCommands()
		{
			if (this.queuedBuildCommands != null)
			{
				return this.queuedBuildCommands.Count;
			}
			return 0;
		}

		// Token: 0x060060F8 RID: 24824 RVA: 0x001ED61F File Offset: 0x001EB81F
		public void AddRollbackAction(BuilderAction action)
		{
			this.rollBackActions.Add(action);
		}

		// Token: 0x060060F9 RID: 24825 RVA: 0x001ED62D File Offset: 0x001EB82D
		public void RemoveRollBackActions()
		{
			this.rollBackActions.Clear();
		}

		// Token: 0x060060FA RID: 24826 RVA: 0x001ED63C File Offset: 0x001EB83C
		public void RemoveRollBackActions(int localCommandId)
		{
			for (int i = this.rollBackActions.Count - 1; i >= 0; i--)
			{
				if (localCommandId == -1 || this.rollBackActions[i].localCommandId == localCommandId)
				{
					this.rollBackActions.RemoveAt(i);
				}
			}
		}

		// Token: 0x060060FB RID: 24827 RVA: 0x001ED688 File Offset: 0x001EB888
		public bool HasRollBackActionsForCommand(int localCommandId)
		{
			for (int i = 0; i < this.rollBackActions.Count; i++)
			{
				if (this.rollBackActions[i].localCommandId == localCommandId)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060060FC RID: 24828 RVA: 0x001ED6C2 File Offset: 0x001EB8C2
		public void AddRollForwardCommand(BuilderTable.BuilderCommand command)
		{
			this.rollForwardCommands.Add(command);
		}

		// Token: 0x060060FD RID: 24829 RVA: 0x001ED6D0 File Offset: 0x001EB8D0
		public void RemoveRollForwardCommands()
		{
			this.rollForwardCommands.Clear();
		}

		// Token: 0x060060FE RID: 24830 RVA: 0x001ED6E0 File Offset: 0x001EB8E0
		public void RemoveRollForwardCommands(int localCommandId)
		{
			for (int i = this.rollForwardCommands.Count - 1; i >= 0; i--)
			{
				if (localCommandId == -1 || this.rollForwardCommands[i].localCommandId == localCommandId)
				{
					this.rollForwardCommands.RemoveAt(i);
				}
			}
		}

		// Token: 0x060060FF RID: 24831 RVA: 0x001ED72C File Offset: 0x001EB92C
		public bool HasRollForwardCommand(int localCommandId)
		{
			for (int i = 0; i < this.rollForwardCommands.Count; i++)
			{
				if (this.rollForwardCommands[i].localCommandId == localCommandId)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06006100 RID: 24832 RVA: 0x001ED768 File Offset: 0x001EB968
		public bool ShouldRollbackBufferCommand(BuilderTable.BuilderCommand cmd)
		{
			return cmd.type != BuilderTable.BuilderCommandType.Create && cmd.type != BuilderTable.BuilderCommandType.CreateArmShelf && this.rollBackActions.Count > 0 && (cmd.player == null || !cmd.player.IsLocal || !this.HasRollForwardCommand(cmd.localCommandId));
		}

		// Token: 0x06006101 RID: 24833 RVA: 0x001ED7BF File Offset: 0x001EB9BF
		public void AddRollbackBufferedCommand(BuilderTable.BuilderCommand bufferedCmd)
		{
			this.rollBackBufferedCommands.Add(bufferedCmd);
		}

		// Token: 0x06006102 RID: 24834 RVA: 0x001ED7D0 File Offset: 0x001EB9D0
		private void ExecuteRollBackActions()
		{
			for (int i = this.rollBackActions.Count - 1; i >= 0; i--)
			{
				this.ExecuteAction(this.rollBackActions[i]);
			}
			this.rollBackActions.Clear();
		}

		// Token: 0x06006103 RID: 24835 RVA: 0x001ED814 File Offset: 0x001EBA14
		private void ExecuteRollbackBufferedCommands()
		{
			for (int i = 0; i < this.rollBackBufferedCommands.Count; i++)
			{
				BuilderTable.BuilderCommand builderCommand = this.rollBackBufferedCommands[i];
				builderCommand.isQueued = false;
				builderCommand.canRollback = false;
				this.ExecuteBuildCommand(builderCommand);
			}
			this.rollBackBufferedCommands.Clear();
		}

		// Token: 0x06006104 RID: 24836 RVA: 0x001ED868 File Offset: 0x001EBA68
		private void ExecuteRollForwardCommands()
		{
			BuilderTable.tempRollForwardCommands.Clear();
			for (int i = 0; i < this.rollForwardCommands.Count; i++)
			{
				BuilderTable.tempRollForwardCommands.Add(this.rollForwardCommands[i]);
			}
			this.rollForwardCommands.Clear();
			for (int j = 0; j < BuilderTable.tempRollForwardCommands.Count; j++)
			{
				BuilderTable.BuilderCommand builderCommand = BuilderTable.tempRollForwardCommands[j];
				builderCommand.isQueued = true;
				builderCommand.canRollback = true;
				this.ExecuteBuildCommand(builderCommand);
			}
			BuilderTable.tempRollForwardCommands.Clear();
		}

		// Token: 0x06006105 RID: 24837 RVA: 0x001ED8F8 File Offset: 0x001EBAF8
		private void UpdateRollForwardCommandData()
		{
			for (int i = 0; i < this.rollForwardCommands.Count; i++)
			{
				BuilderTable.BuilderCommand builderCommand = this.rollForwardCommands[i];
				if (builderCommand.type == BuilderTable.BuilderCommandType.Drop)
				{
					BuilderPiece piece = this.GetPiece(builderCommand.pieceId);
					if (piece != null && piece.rigidBody != null)
					{
						builderCommand.localPosition = piece.rigidBody.position;
						builderCommand.localRotation = piece.rigidBody.rotation;
						builderCommand.velocity = piece.rigidBody.linearVelocity;
						builderCommand.angVelocity = piece.rigidBody.angularVelocity;
						this.rollForwardCommands[i] = builderCommand;
					}
				}
			}
		}

		// Token: 0x06006106 RID: 24838 RVA: 0x001ED9B0 File Offset: 0x001EBBB0
		public bool TryRollbackAndReExecute(int localCommandId)
		{
			if (this.HasRollBackActionsForCommand(localCommandId))
			{
				if (this.rollBackBufferedCommands.Count > 0)
				{
					this.UpdateRollForwardCommandData();
					this.ExecuteRollBackActions();
					this.ExecuteRollbackBufferedCommands();
					this.ExecuteRollForwardCommands();
					this.RemoveRollBackActions(localCommandId);
					this.RemoveRollForwardCommands(localCommandId);
				}
				else
				{
					this.RemoveRollBackActions(localCommandId);
					this.RemoveRollForwardCommands(localCommandId);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06006107 RID: 24839 RVA: 0x001EDA0F File Offset: 0x001EBC0F
		public void RollbackFailedCommand(int localCommandId)
		{
			if (this.HasRollBackActionsForCommand(localCommandId))
			{
				this.UpdateRollForwardCommandData();
				this.ExecuteRollBackActions();
				this.ExecuteRollbackBufferedCommands();
				this.RemoveRollForwardCommands(-1);
				this.ExecuteRollForwardCommands();
			}
		}

		// Token: 0x06006108 RID: 24840 RVA: 0x001EDA39 File Offset: 0x001EBC39
		public BuilderTable.TableState GetTableState()
		{
			return this.tableState;
		}

		// Token: 0x06006109 RID: 24841 RVA: 0x001EDA44 File Offset: 0x001EBC44
		public void SetTableState(BuilderTable.TableState newState)
		{
			this.InitIfNeeded();
			if (newState == this.tableState)
			{
				return;
			}
			BuilderTable.TableState tableState = this.tableState;
			this.tableState = newState;
			switch (this.tableState)
			{
			case BuilderTable.TableState.WaitingForInitalBuild:
				if (!this.isTableMutable && !NetworkSystem.Instance.IsMasterClient)
				{
					this.sharedBlocksMap = null;
					UnityEvent onMapCleared = this.OnMapCleared;
					if (onMapCleared == null)
					{
						return;
					}
					onMapCleared.Invoke();
					return;
				}
				break;
			case BuilderTable.TableState.ReceivingInitialBuild:
			case BuilderTable.TableState.ReceivingMasterResync:
			case BuilderTable.TableState.InitialBuild:
			case BuilderTable.TableState.ExecuteQueuedCommands:
				break;
			case BuilderTable.TableState.WaitForInitialBuildMaster:
				this.nextPieceId = 10000;
				if (this.isTableMutable)
				{
					this.BuildInitialTableForPlayer();
					return;
				}
				this.BuildSelectedSharedMap();
				return;
			case BuilderTable.TableState.WaitForMasterResync:
				this.ClearQueuedCommands();
				this.ResetConveyors();
				return;
			case BuilderTable.TableState.Ready:
				this.OnAvailableResourcesChange();
				if (!this.isTableMutable)
				{
					string text = ((this.sharedBlocksMap == null) ? "" : this.sharedBlocksMap.MapID);
					UnityEvent<string> onMapLoaded = this.OnMapLoaded;
					if (onMapLoaded != null)
					{
						onMapLoaded.Invoke(text);
					}
					this.SetPendingMap(null);
					return;
				}
				break;
			case BuilderTable.TableState.BadData:
				this.ClearTable();
				this.ClearQueuedCommands();
				break;
			case BuilderTable.TableState.WaitingForSharedMapLoad:
				this.ClearTable();
				this.ClearQueuedCommands();
				this.builderNetworking.ResetSerializedTableForAllPlayers();
				return;
			default:
				return;
			}
		}

		// Token: 0x0600610A RID: 24842 RVA: 0x001EDB70 File Offset: 0x001EBD70
		public void SetPendingMap(string mapID)
		{
			this.pendingMapID = mapID;
		}

		// Token: 0x0600610B RID: 24843 RVA: 0x001EDB79 File Offset: 0x001EBD79
		public string GetPendingMap()
		{
			return this.pendingMapID;
		}

		// Token: 0x0600610C RID: 24844 RVA: 0x001EDB81 File Offset: 0x001EBD81
		public string GetCurrentMapID()
		{
			SharedBlocksManager.SharedBlocksMap sharedBlocksMap = this.sharedBlocksMap;
			if (sharedBlocksMap == null)
			{
				return null;
			}
			return sharedBlocksMap.MapID;
		}

		// Token: 0x0600610D RID: 24845 RVA: 0x001EDB94 File Offset: 0x001EBD94
		public void LoadSharedMap(SharedBlocksManager.SharedBlocksMap map)
		{
			if (NetworkSystem.Instance.InRoom)
			{
				if (map.MapID.IsNullOrEmpty())
				{
					GTDev.LogWarning<string>("Invalid map to load", null);
					UnityEvent<string> onMapLoadFailed = this.OnMapLoadFailed;
					if (onMapLoadFailed == null)
					{
						return;
					}
					onMapLoadFailed.Invoke("Invalid Map ID");
					return;
				}
				else
				{
					if (this.tableState == BuilderTable.TableState.Ready || this.tableState == BuilderTable.TableState.BadData)
					{
						this.builderNetworking.RequestLoadSharedBlocksMap(map.MapID);
						return;
					}
					UnityEvent<string> onMapLoadFailed2 = this.OnMapLoadFailed;
					if (onMapLoadFailed2 == null)
					{
						return;
					}
					onMapLoadFailed2.Invoke("WAIT FOR LOAD IN PROGRESS");
					return;
				}
			}
			else
			{
				UnityEvent<string> onMapLoadFailed3 = this.OnMapLoadFailed;
				if (onMapLoadFailed3 == null)
				{
					return;
				}
				onMapLoadFailed3.Invoke("Not In Room");
				return;
			}
		}

		// Token: 0x0600610E RID: 24846 RVA: 0x001EDC2C File Offset: 0x001EBE2C
		public void SetInRoom(bool inRoom)
		{
			this.inRoom = inRoom;
			bool flag = inRoom && this.inBuilderZone;
			if (!inRoom)
			{
				this.pendingMapID = null;
				this.sharedBlocksMap = null;
				UnityEvent onMapCleared = this.OnMapCleared;
				if (onMapCleared != null)
				{
					onMapCleared.Invoke();
				}
			}
			if (flag && this.tableState == BuilderTable.TableState.WaitingForZoneAndRoom)
			{
				this.SetTableState(BuilderTable.TableState.WaitingForInitalBuild);
				this.builderNetworking.PlayerEnterBuilder();
				return;
			}
			if (!flag && this.tableState != BuilderTable.TableState.WaitingForZoneAndRoom && !this.builderNetworking.IsPrivateMasterClient())
			{
				this.SetTableState(BuilderTable.TableState.WaitingForZoneAndRoom);
				this.builderNetworking.PlayerExitBuilder();
				return;
			}
			if (flag && PhotonNetwork.IsMasterClient && this.isTableMutable)
			{
				this.builderNetworking.RequestCreateArmShelfForPlayer(PhotonNetwork.LocalPlayer);
				return;
			}
			if (!flag && this.builderNetworking.IsPrivateMasterClient() && this.isTableMutable)
			{
				this.RemoveArmShelfForPlayer(PhotonNetwork.LocalPlayer);
			}
		}

		// Token: 0x0600610F RID: 24847 RVA: 0x001EDD00 File Offset: 0x001EBF00
		public static bool IsLocalPlayerInBuilderZone()
		{
			GorillaTagger instance = GorillaTagger.Instance;
			ZoneEntityBSP zoneEntityBSP;
			if (instance == null)
			{
				zoneEntityBSP = null;
			}
			else
			{
				VRRig offlineVRRig = instance.offlineVRRig;
				zoneEntityBSP = ((offlineVRRig != null) ? offlineVRRig.zoneEntity : null);
			}
			ZoneEntityBSP zoneEntityBSP2 = zoneEntityBSP;
			BuilderTable builderTable;
			return !(zoneEntityBSP2 == null) && BuilderTable.TryGetBuilderTableForZone(zoneEntityBSP2.currentZone, out builderTable) && builderTable.IsInBuilderZone();
		}

		// Token: 0x06006110 RID: 24848 RVA: 0x001EDD4D File Offset: 0x001EBF4D
		public bool IsInBuilderZone()
		{
			return this.inBuilderZone;
		}

		// Token: 0x06006111 RID: 24849 RVA: 0x001EDD58 File Offset: 0x001EBF58
		public void SetInBuilderZone(bool inBuilderZone)
		{
			this.inBuilderZone = inBuilderZone;
			this.ShowPieces(inBuilderZone);
			bool flag = this.inRoom && inBuilderZone;
			if (flag && this.tableState == BuilderTable.TableState.WaitingForZoneAndRoom)
			{
				this.SetTableState(BuilderTable.TableState.WaitingForInitalBuild);
				this.builderNetworking.PlayerEnterBuilder();
				return;
			}
			if (!flag && this.tableState != BuilderTable.TableState.WaitingForZoneAndRoom && !this.builderNetworking.IsPrivateMasterClient())
			{
				this.SetTableState(BuilderTable.TableState.WaitingForZoneAndRoom);
				this.builderNetworking.PlayerExitBuilder();
				return;
			}
			if (flag && PhotonNetwork.IsMasterClient)
			{
				this.builderNetworking.RequestCreateArmShelfForPlayer(PhotonNetwork.LocalPlayer);
				return;
			}
			if (!flag && this.builderNetworking.IsPrivateMasterClient())
			{
				this.RemoveArmShelfForPlayer(PhotonNetwork.LocalPlayer);
			}
		}

		// Token: 0x06006112 RID: 24850 RVA: 0x001EDDFC File Offset: 0x001EBFFC
		private void ShowPieces(bool show)
		{
			if (this.builderRenderer != null)
			{
				this.builderRenderer.Show(show);
			}
			if (this.pieces == null || this.basePieces == null)
			{
				return;
			}
			for (int i = 0; i < this.pieces.Count; i++)
			{
				this.pieces[i].SetDirectRenderersVisible(show);
			}
			for (int j = 0; j < this.basePieces.Count; j++)
			{
				this.basePieces[j].SetDirectRenderersVisible(show);
			}
		}

		// Token: 0x06006113 RID: 24851 RVA: 0x001EDE84 File Offset: 0x001EC084
		private void UpdateTableState()
		{
			switch (this.tableState)
			{
			case BuilderTable.TableState.InitialBuild:
			{
				BuilderTableNetworking.PlayerTableInitState localTableInit = this.builderNetworking.GetLocalTableInit();
				try
				{
					this.ClearTable();
					this.ClearQueuedCommands();
					byte[] array = GZipStream.UncompressBuffer(localTableInit.serializedTableState);
					localTableInit.totalSerializedBytes = array.Length;
					Array.Copy(array, 0, localTableInit.serializedTableState, 0, localTableInit.totalSerializedBytes);
					this.DeserializeTableState(localTableInit.serializedTableState, localTableInit.numSerializedBytes);
					if (this.tableState == BuilderTable.TableState.BadData)
					{
						return;
					}
					this.SetTableState(BuilderTable.TableState.ExecuteQueuedCommands);
					this.SetIsDirty(true);
					return;
				}
				catch (Exception)
				{
					this.SetTableState(BuilderTable.TableState.BadData);
					return;
				}
				break;
			}
			case BuilderTable.TableState.ExecuteQueuedCommands:
				break;
			case BuilderTable.TableState.Ready:
			{
				JobHandle jobHandle = default(JobHandle);
				if (this.isTableMutable)
				{
					this.conveyorManager.UpdateManager();
					jobHandle = this.conveyorManager.ConstructJobHandle();
					JobHandle.ScheduleBatchedJobs();
					foreach (BuilderDispenserShelf builderDispenserShelf in this.dispenserShelves)
					{
						builderDispenserShelf.UpdateShelf();
					}
					foreach (BuilderPiecePrivatePlot builderPiecePrivatePlot in this.allPrivatePlots)
					{
						builderPiecePrivatePlot.UpdatePlot();
					}
					foreach (BuilderRecycler builderRecycler in this.recyclers)
					{
						builderRecycler.UpdateRecycler();
					}
					for (int i = this.shelfSliceUpdateIndex; i < this.dispenserShelves.Count; i += BuilderTable.SHELF_SLICE_BUCKETS)
					{
						this.dispenserShelves[i].UpdateShelfSliced();
					}
					this.shelfSliceUpdateIndex = (this.shelfSliceUpdateIndex + 1) % BuilderTable.SHELF_SLICE_BUCKETS;
				}
				foreach (IBuilderPieceFunctional builderPieceFunctional in this.funcComponentsToRegister)
				{
					if (builderPieceFunctional != null)
					{
						this.activeFunctionalComponents.Add(builderPieceFunctional);
					}
				}
				foreach (IBuilderPieceFunctional builderPieceFunctional2 in this.funcComponentsToUnregister)
				{
					this.activeFunctionalComponents.Remove(builderPieceFunctional2);
				}
				this.funcComponentsToRegister.Clear();
				this.funcComponentsToUnregister.Clear();
				foreach (IBuilderPieceFunctional builderPieceFunctional3 in this.activeFunctionalComponents)
				{
					if (builderPieceFunctional3 != null)
					{
						builderPieceFunctional3.FunctionalPieceUpdate();
					}
				}
				if (this.isTableMutable)
				{
					foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
					{
						builderResourceMeter.UpdateMeterFill();
					}
					this.CleanUpDroppedPiece();
					jobHandle.Complete();
					return;
				}
				return;
			}
			default:
				return;
			}
			for (int j = 0; j < this.queuedBuildCommands.Count; j++)
			{
				BuilderTable.BuilderCommand builderCommand = this.queuedBuildCommands[j];
				builderCommand.isQueued = true;
				this.ExecuteBuildCommand(builderCommand);
			}
			this.queuedBuildCommands.Clear();
			this.SetTableState(BuilderTable.TableState.Ready);
		}

		// Token: 0x06006114 RID: 24852 RVA: 0x001EE210 File Offset: 0x001EC410
		private void RouteNewCommand(BuilderTable.BuilderCommand cmd, bool force)
		{
			bool flag = this.ShouldExecuteCommand();
			if (force)
			{
				this.ExecuteBuildCommand(cmd);
				return;
			}
			if (flag && this.ShouldRollbackBufferCommand(cmd))
			{
				this.AddRollbackBufferedCommand(cmd);
				return;
			}
			if (flag)
			{
				this.ExecuteBuildCommand(cmd);
				return;
			}
			if (this.ShouldQueueCommand())
			{
				this.AddQueuedCommand(cmd);
				return;
			}
			this.ShouldDiscardCommand();
		}

		// Token: 0x06006115 RID: 24853 RVA: 0x001EE268 File Offset: 0x001EC468
		private void ExecuteBuildCommand(BuilderTable.BuilderCommand cmd)
		{
			if (!this.isTableMutable && cmd.type != BuilderTable.BuilderCommandType.FunctionalStateChange)
			{
				return;
			}
			switch (cmd.type)
			{
			case BuilderTable.BuilderCommandType.Create:
				this.ExecutePieceCreated(cmd);
				return;
			case BuilderTable.BuilderCommandType.Place:
				this.ExecutePiecePlacedWithActions(cmd);
				return;
			case BuilderTable.BuilderCommandType.Grab:
				this.ExecutePieceGrabbedWithActions(cmd);
				return;
			case BuilderTable.BuilderCommandType.Drop:
				this.ExecutePieceDroppedWithActions(cmd);
				return;
			case BuilderTable.BuilderCommandType.Remove:
				break;
			case BuilderTable.BuilderCommandType.Paint:
				this.ExecutePiecePainted(cmd);
				return;
			case BuilderTable.BuilderCommandType.Recycle:
				this.ExecutePieceRecycled(cmd);
				return;
			case BuilderTable.BuilderCommandType.ClaimPlot:
				this.ExecuteClaimPlot(cmd);
				return;
			case BuilderTable.BuilderCommandType.FreePlot:
				this.ExecuteFreePlot(cmd);
				return;
			case BuilderTable.BuilderCommandType.CreateArmShelf:
				this.ExecuteArmShelfCreated(cmd);
				return;
			case BuilderTable.BuilderCommandType.PlayerLeftRoom:
				this.ExecutePlayerLeftRoom(cmd);
				return;
			case BuilderTable.BuilderCommandType.FunctionalStateChange:
				this.ExecuteSetFunctionalPieceState(cmd);
				return;
			case BuilderTable.BuilderCommandType.SetSelection:
				this.ExecuteSetSelection(cmd);
				return;
			case BuilderTable.BuilderCommandType.Repel:
				this.ExecutePieceRepelled(cmd);
				break;
			default:
				return;
			}
		}

		// Token: 0x06006116 RID: 24854 RVA: 0x001EE335 File Offset: 0x001EC535
		public void ClearTable()
		{
			this.ClearTableInternal();
		}

		// Token: 0x06006117 RID: 24855 RVA: 0x001EE340 File Offset: 0x001EC540
		private void ClearTableInternal()
		{
			BuilderTable.tempDeletePieces.Clear();
			for (int i = 0; i < this.pieces.Count; i++)
			{
				BuilderTable.tempDeletePieces.Add(this.pieces[i]);
			}
			if (this.isTableMutable)
			{
				this.droppedPieces.Clear();
				this.droppedPieceData.Clear();
			}
			for (int j = 0; j < BuilderTable.tempDeletePieces.Count; j++)
			{
				BuilderTable.tempDeletePieces[j].ClearParentPiece(false);
				BuilderTable.tempDeletePieces[j].ClearParentHeld();
				BuilderTable.tempDeletePieces[j].SetState(BuilderPiece.State.None, false);
				this.RemovePiece(BuilderTable.tempDeletePieces[j]);
			}
			for (int k = 0; k < BuilderTable.tempDeletePieces.Count; k++)
			{
				this.builderPool.DestroyPiece(BuilderTable.tempDeletePieces[k]);
			}
			BuilderTable.tempDeletePieces.Clear();
			this.pieces.Clear();
			this.pieceIDToIndexCache.Clear();
			this.nextPieceId = 10000;
			if (this.isTableMutable)
			{
				this.conveyorManager.OnClearTable();
				foreach (BuilderDispenserShelf builderDispenserShelf in this.dispenserShelves)
				{
					builderDispenserShelf.OnClearTable();
				}
				for (int l = 0; l < this.repelHistoryLength; l++)
				{
					this.repelledPieceRoots[l].Clear();
				}
			}
			this.funcComponentsToRegister.Clear();
			this.funcComponentsToUnregister.Clear();
			this.activeFunctionalComponents.Clear();
			foreach (BuilderPiece builderPiece in this.basePieces)
			{
				foreach (BuilderAttachGridPlane builderAttachGridPlane in builderPiece.gridPlanes)
				{
					builderAttachGridPlane.OnReturnToPool(this.builderPool);
				}
			}
			if (this.isTableMutable)
			{
				this.ClearBuiltInPlots();
				this.playerToArmShelfLeft.Clear();
				this.playerToArmShelfRight.Clear();
				if (BuilderPieceInteractor.instance != null)
				{
					BuilderPieceInteractor.instance.RemovePiecesFromHands();
				}
			}
		}

		// Token: 0x06006118 RID: 24856 RVA: 0x001EE5AC File Offset: 0x001EC7AC
		private void ClearBuiltInPlots()
		{
			foreach (BuilderPiecePrivatePlot builderPiecePrivatePlot in this.allPrivatePlots)
			{
				builderPiecePrivatePlot.ClearPlot();
			}
			this.plotOwners.Clear();
			this.SetLocalPlayerOwnsPlot(false);
		}

		// Token: 0x06006119 RID: 24857 RVA: 0x001EE610 File Offset: 0x001EC810
		private void OnDeserializeUpdatePlots()
		{
			foreach (BuilderPiecePrivatePlot builderPiecePrivatePlot in this.allPrivatePlots)
			{
				builderPiecePrivatePlot.RecountPlotCost();
			}
		}

		// Token: 0x0600611A RID: 24858 RVA: 0x001EE660 File Offset: 0x001EC860
		public void BuildPiecesOnShelves()
		{
			if (!this.isTableMutable)
			{
				return;
			}
			if (this.shelves == null)
			{
				return;
			}
			for (int i = 0; i < this.shelves.Count; i++)
			{
				if (this.shelves[i] != null)
				{
					this.shelves[i].Init();
				}
			}
			bool flag = true;
			while (flag)
			{
				flag = false;
				for (int j = 0; j < this.shelves.Count; j++)
				{
					if (this.shelves[j].HasOpenSlot())
					{
						this.shelves[j].BuildNextPiece(this);
						if (this.shelves[j].HasOpenSlot())
						{
							flag = true;
						}
					}
				}
			}
		}

		// Token: 0x0600611B RID: 24859 RVA: 0x001EE713 File Offset: 0x001EC913
		private void OnFinishedInitialTableBuild()
		{
			this.BuildPiecesOnShelves();
			this.SetTableState(BuilderTable.TableState.Ready);
			this.CreateArmShelvesForPlayersInBuilder();
		}

		// Token: 0x0600611C RID: 24860 RVA: 0x001EE728 File Offset: 0x001EC928
		public int CreatePieceId()
		{
			int num = this.nextPieceId;
			if (this.nextPieceId == 2147483647)
			{
				this.nextPieceId = 20000;
			}
			this.nextPieceId++;
			return num;
		}

		// Token: 0x0600611D RID: 24861 RVA: 0x001EE758 File Offset: 0x001EC958
		public void ResetConveyors()
		{
			if (this.isTableMutable)
			{
				foreach (BuilderConveyor builderConveyor in this.conveyors)
				{
					builderConveyor.ResetConveyorState();
				}
			}
		}

		// Token: 0x0600611E RID: 24862 RVA: 0x001EE7B0 File Offset: 0x001EC9B0
		public void RequestCreateConveyorPiece(int newPieceType, int materialType, int shelfID)
		{
			if (shelfID < 0 || shelfID >= this.conveyors.Count)
			{
				return;
			}
			BuilderConveyor builderConveyor = this.conveyors[shelfID];
			if (builderConveyor == null)
			{
				return;
			}
			Transform spawnTransform = builderConveyor.GetSpawnTransform();
			this.builderNetworking.CreateShelfPiece(newPieceType, spawnTransform.position, spawnTransform.rotation, materialType, BuilderPiece.State.OnConveyor, shelfID);
		}

		// Token: 0x0600611F RID: 24863 RVA: 0x001EE809 File Offset: 0x001ECA09
		public void RequestCreateDispenserShelfPiece(int pieceType, Vector3 position, Quaternion rotation, int materialType, int shelfID)
		{
			if (shelfID < 0 || shelfID >= this.dispenserShelves.Count)
			{
				return;
			}
			if (this.dispenserShelves[shelfID] == null)
			{
				return;
			}
			this.builderNetworking.CreateShelfPiece(pieceType, position, rotation, materialType, BuilderPiece.State.OnShelf, shelfID);
		}

		// Token: 0x06006120 RID: 24864 RVA: 0x001EE84C File Offset: 0x001ECA4C
		public void CreateConveyorPiece(int pieceType, int pieceId, Vector3 position, Quaternion rotation, int materialType, int shelfID, int sendTimestamp)
		{
			if (shelfID < 0 || shelfID >= this.conveyors.Count)
			{
				return;
			}
			if (this.conveyors[shelfID] == null)
			{
				return;
			}
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Create,
				pieceType = pieceType,
				pieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				materialType = materialType,
				state = BuilderPiece.State.OnConveyor,
				parentPieceId = shelfID,
				parentAttachIndex = sendTimestamp,
				player = NetworkSystem.Instance.MasterClient
			};
			this.RouteNewCommand(builderCommand, false);
		}

		// Token: 0x06006121 RID: 24865 RVA: 0x001EE8F4 File Offset: 0x001ECAF4
		public void CreateDispenserShelfPiece(int pieceType, int pieceId, Vector3 position, Quaternion rotation, int materialType, int shelfID)
		{
			if (shelfID < 0 || shelfID >= this.dispenserShelves.Count)
			{
				return;
			}
			if (this.dispenserShelves[shelfID] == null)
			{
				return;
			}
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Create,
				pieceType = pieceType,
				pieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				materialType = materialType,
				state = BuilderPiece.State.OnShelf,
				parentPieceId = shelfID,
				isLeft = true,
				player = NetworkSystem.Instance.MasterClient
			};
			this.RouteNewCommand(builderCommand, false);
		}

		// Token: 0x06006122 RID: 24866 RVA: 0x001EE99A File Offset: 0x001ECB9A
		public void RequestShelfSelection(int shelfId, int groupID, bool isConveyor)
		{
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				return;
			}
			this.builderNetworking.RequestShelfSelection(shelfId, groupID, isConveyor);
		}

		// Token: 0x06006123 RID: 24867 RVA: 0x001EE9B4 File Offset: 0x001ECBB4
		public void VerifySetSelections()
		{
			if (!this.isTableMutable)
			{
				return;
			}
			foreach (BuilderConveyor builderConveyor in this.conveyors)
			{
				builderConveyor.VerifySetSelection();
			}
			foreach (BuilderDispenserShelf builderDispenserShelf in this.dispenserShelves)
			{
				builderDispenserShelf.VerifySetSelection();
			}
		}

		// Token: 0x06006124 RID: 24868 RVA: 0x001EEA4C File Offset: 0x001ECC4C
		public bool ValidateShelfSelectionParams(int shelfId, int displayGroupID, bool isConveyor, Player player)
		{
			bool flag = shelfId >= 0 && ((isConveyor && shelfId < this.conveyors.Count) || (!isConveyor && shelfId < this.dispenserShelves.Count)) && BuilderSetManager.instance.DoesPlayerOwnDisplayGroup(player, displayGroupID);
			if (PhotonNetwork.IsMasterClient)
			{
				if (isConveyor)
				{
					BuilderConveyor builderConveyor = this.conveyors[shelfId];
					bool flag2 = this.IsPlayerHandNearAction(NetPlayer.Get(player), builderConveyor.transform.position, false, true, 4f);
					flag = flag && flag2;
				}
				else
				{
					BuilderDispenserShelf builderDispenserShelf = this.dispenserShelves[shelfId];
					bool flag3 = this.IsPlayerHandNearAction(NetPlayer.Get(player), builderDispenserShelf.transform.position, false, true, 4f);
					flag = flag && flag3;
				}
			}
			return flag;
		}

		// Token: 0x06006125 RID: 24869 RVA: 0x001EEB08 File Offset: 0x001ECD08
		private void SetConveyorSelection(int conveyorId, int setId)
		{
			BuilderConveyor builderConveyor = this.conveyors[conveyorId];
			if (builderConveyor == null)
			{
				return;
			}
			builderConveyor.SetSelection(setId);
		}

		// Token: 0x06006126 RID: 24870 RVA: 0x001EEB34 File Offset: 0x001ECD34
		private void SetDispenserSelection(int conveyorId, int setId)
		{
			BuilderDispenserShelf builderDispenserShelf = this.dispenserShelves[conveyorId];
			if (builderDispenserShelf == null)
			{
				return;
			}
			builderDispenserShelf.SetSelection(setId);
		}

		// Token: 0x06006127 RID: 24871 RVA: 0x001EEB60 File Offset: 0x001ECD60
		public void ChangeSetSelection(int shelfID, int setID, bool isConveyor)
		{
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.SetSelection,
				parentPieceId = shelfID,
				pieceType = setID,
				isLeft = isConveyor,
				player = NetworkSystem.Instance.MasterClient
			};
			this.RouteNewCommand(builderCommand, false);
		}

		// Token: 0x06006128 RID: 24872 RVA: 0x001EEBB4 File Offset: 0x001ECDB4
		public void ExecuteSetSelection(BuilderTable.BuilderCommand cmd)
		{
			bool isLeft = cmd.isLeft;
			int parentPieceId = cmd.parentPieceId;
			int pieceType = cmd.pieceType;
			if (isLeft)
			{
				this.SetConveyorSelection(parentPieceId, pieceType);
				return;
			}
			this.SetDispenserSelection(parentPieceId, pieceType);
		}

		// Token: 0x06006129 RID: 24873 RVA: 0x001EEBE8 File Offset: 0x001ECDE8
		public bool ValidateFunctionalPieceState(int pieceID, byte state, NetPlayer player)
		{
			BuilderPiece piece = this.GetPiece(pieceID);
			return !(piece == null) && piece.functionalPieceComponent != null && (!NetworkSystem.Instance.IsMasterClient || player.IsMasterClient || this.IsPlayerHandNearAction(player, piece.transform.position, true, false, piece.functionalPieceComponent.GetInteractionDistace())) && piece.functionalPieceComponent.IsStateValid(state);
		}

		// Token: 0x0600612A RID: 24874 RVA: 0x001EEC58 File Offset: 0x001ECE58
		public void OnFunctionalStateRequest(int pieceID, byte state, NetPlayer player, int timeStamp)
		{
			BuilderPiece piece = this.GetPiece(pieceID);
			if (piece == null)
			{
				return;
			}
			if (piece.functionalPieceComponent == null)
			{
				return;
			}
			if (player == null)
			{
				return;
			}
			piece.functionalPieceComponent.OnStateRequest(state, player, timeStamp);
		}

		// Token: 0x0600612B RID: 24875 RVA: 0x001EEC94 File Offset: 0x001ECE94
		public void SetFunctionalPieceState(int pieceID, byte state, NetPlayer player, int timeStamp)
		{
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.FunctionalStateChange,
				pieceId = pieceID,
				twist = state,
				player = player,
				serverTimeStamp = timeStamp
			};
			this.RouteNewCommand(builderCommand, false);
		}

		// Token: 0x0600612C RID: 24876 RVA: 0x001EECE0 File Offset: 0x001ECEE0
		public void ExecuteSetFunctionalPieceState(BuilderTable.BuilderCommand cmd)
		{
			BuilderPiece piece = this.GetPiece(cmd.pieceId);
			if (piece == null)
			{
				return;
			}
			piece.SetFunctionalPieceState(cmd.twist, cmd.player, cmd.serverTimeStamp);
		}

		// Token: 0x0600612D RID: 24877 RVA: 0x001EED1C File Offset: 0x001ECF1C
		public void RegisterFunctionalPiece(IBuilderPieceFunctional component)
		{
			if (component != null)
			{
				this.funcComponentsToRegister.Add(component);
			}
		}

		// Token: 0x0600612E RID: 24878 RVA: 0x001EED2D File Offset: 0x001ECF2D
		public void UnregisterFunctionalPiece(IBuilderPieceFunctional component)
		{
			if (component != null)
			{
				this.funcComponentsToUnregister.Add(component);
			}
		}

		// Token: 0x0600612F RID: 24879 RVA: 0x001EED3E File Offset: 0x001ECF3E
		public void RegisterFunctionalPieceFixedUpdate(IBuilderPieceFunctional component)
		{
			if (component != null)
			{
				this.funcComponentsToRegisterFixed.Add(component);
			}
		}

		// Token: 0x06006130 RID: 24880 RVA: 0x001EED4F File Offset: 0x001ECF4F
		public void UnregisterFunctionalPieceFixedUpdate(IBuilderPieceFunctional component)
		{
			if (component != null)
			{
				this.funcComponentsToRegisterFixed.Remove(component);
			}
		}

		// Token: 0x06006131 RID: 24881 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void RequestCreatePiece(int newPieceType, Vector3 position, Quaternion rotation, int materialType)
		{
		}

		// Token: 0x06006132 RID: 24882 RVA: 0x001EED64 File Offset: 0x001ECF64
		public void CreatePiece(int pieceType, int pieceId, Vector3 position, Quaternion rotation, int materialType, BuilderPiece.State state, Player player)
		{
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Create,
				pieceType = pieceType,
				pieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				materialType = materialType,
				state = state,
				player = NetPlayer.Get(player)
			};
			this.RouteNewCommand(builderCommand, false);
		}

		// Token: 0x06006133 RID: 24883 RVA: 0x001EEDCC File Offset: 0x001ECFCC
		public void RequestRecyclePiece(BuilderPiece piece, bool playFX, int recyclerID)
		{
			this.builderNetworking.RequestRecyclePiece(piece.pieceId, piece.transform.position, piece.transform.rotation, playFX, recyclerID);
		}

		// Token: 0x06006134 RID: 24884 RVA: 0x001EEDF8 File Offset: 0x001ECFF8
		public void RecyclePiece(int pieceId, Vector3 position, Quaternion rotation, bool playFX, int recyclerID, Player player)
		{
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Recycle,
				pieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				player = NetPlayer.Get(player),
				isLeft = playFX,
				parentPieceId = recyclerID
			};
			this.RouteNewCommand(builderCommand, false);
		}

		// Token: 0x06006135 RID: 24885 RVA: 0x001EEE57 File Offset: 0x001ED057
		private bool ShouldExecuteCommand()
		{
			return this.tableState == BuilderTable.TableState.Ready || this.tableState == BuilderTable.TableState.WaitForInitialBuildMaster;
		}

		// Token: 0x06006136 RID: 24886 RVA: 0x001EEE6D File Offset: 0x001ED06D
		private bool ShouldQueueCommand()
		{
			return this.tableState == BuilderTable.TableState.ReceivingInitialBuild || this.tableState == BuilderTable.TableState.ReceivingMasterResync || this.tableState == BuilderTable.TableState.InitialBuild || this.tableState == BuilderTable.TableState.ExecuteQueuedCommands;
		}

		// Token: 0x06006137 RID: 24887 RVA: 0x001EEE95 File Offset: 0x001ED095
		private bool ShouldDiscardCommand()
		{
			return this.tableState == BuilderTable.TableState.WaitingForInitalBuild || this.tableState == BuilderTable.TableState.WaitForInitialBuildMaster || this.tableState == BuilderTable.TableState.WaitingForZoneAndRoom;
		}

		// Token: 0x06006138 RID: 24888 RVA: 0x001EEEB4 File Offset: 0x001ED0B4
		public bool DoesChainContainPiece(BuilderPiece targetPiece, BuilderPiece firstInChain, BuilderPiece nextInChain)
		{
			return !(targetPiece == null) && !(firstInChain == null) && (targetPiece.Equals(firstInChain) || (!(nextInChain == null) && (targetPiece.Equals(nextInChain) || (!(firstInChain == nextInChain) && this.DoesChainContainPiece(targetPiece, firstInChain, nextInChain.parentPiece)))));
		}

		// Token: 0x06006139 RID: 24889 RVA: 0x001EEF10 File Offset: 0x001ED110
		public bool DoesChainContainChain(BuilderPiece chainARoot, BuilderPiece chainBAttachPiece)
		{
			if (chainARoot == null || chainBAttachPiece == null)
			{
				return false;
			}
			if (this.DoesChainContainPiece(chainARoot, chainBAttachPiece, chainBAttachPiece.parentPiece))
			{
				return true;
			}
			BuilderPiece builderPiece = chainARoot.firstChildPiece;
			while (builderPiece != null)
			{
				if (this.DoesChainContainChain(builderPiece, chainBAttachPiece))
				{
					return true;
				}
				builderPiece = builderPiece.nextSiblingPiece;
			}
			return false;
		}

		// Token: 0x0600613A RID: 24890 RVA: 0x001EEF6C File Offset: 0x001ED16C
		private bool IsPlayerHandNearAction(NetPlayer player, Vector3 worldPosition, bool isLeftHand, bool checkBothHands, float acceptableRadius = 2.5f)
		{
			bool flag = true;
			RigContainer rigContainer;
			if (player != null && VRRigCache.Instance != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
			{
				if (isLeftHand || checkBothHands)
				{
					flag = (worldPosition - rigContainer.Rig.leftHandTransform.position).sqrMagnitude < acceptableRadius * acceptableRadius;
				}
				if (!isLeftHand || checkBothHands)
				{
					float sqrMagnitude = (worldPosition - rigContainer.Rig.rightHandTransform.position).sqrMagnitude;
					flag = flag && sqrMagnitude < acceptableRadius * acceptableRadius;
				}
			}
			return flag;
		}

		// Token: 0x0600613B RID: 24891 RVA: 0x001EF000 File Offset: 0x001ED200
		public bool ValidatePlacePieceParams(int pieceId, int attachPieceId, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int parentPieceId, int attachIndex, int parentAttachIndex, NetPlayer placedByPlayer)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			BuilderPiece piece2 = this.GetPiece(attachPieceId);
			if (piece2 == null)
			{
				return false;
			}
			if (piece.heldByPlayerActorNumber != placedByPlayer.ActorNumber)
			{
				return false;
			}
			if (piece.isBuiltIntoTable || piece2.isBuiltIntoTable)
			{
				return false;
			}
			if (twist > 3)
			{
				return false;
			}
			BuilderPiece piece3 = this.GetPiece(parentPieceId);
			if (!(piece3 != null))
			{
				return false;
			}
			if (!BuilderPiece.CanPlayerAttachPieceToPiece(placedByPlayer.ActorNumber, piece2, piece3))
			{
				return false;
			}
			if (this.DoesChainContainChain(piece2, piece3))
			{
				return false;
			}
			if (attachIndex < 0 || attachIndex >= piece2.gridPlanes.Count)
			{
				return false;
			}
			if (piece3 != null && (parentAttachIndex < 0 || parentAttachIndex >= piece3.gridPlanes.Count))
			{
				return false;
			}
			if (piece3 != null)
			{
				bool flag = (long)(twist % 2) == 1L;
				BuilderAttachGridPlane builderAttachGridPlane = piece2.gridPlanes[attachIndex];
				int num = (flag ? builderAttachGridPlane.length : builderAttachGridPlane.width);
				int num2 = (flag ? builderAttachGridPlane.width : builderAttachGridPlane.length);
				BuilderAttachGridPlane builderAttachGridPlane2 = piece3.gridPlanes[parentAttachIndex];
				int num3 = Mathf.FloorToInt((float)builderAttachGridPlane2.width / 2f);
				int num4 = num3 - (builderAttachGridPlane2.width - 1);
				if ((int)bumpOffsetX < num4 - num || (int)bumpOffsetX > num3 + num)
				{
					return false;
				}
				int num5 = Mathf.FloorToInt((float)builderAttachGridPlane2.length / 2f);
				int num6 = num5 - (builderAttachGridPlane2.length - 1);
				if ((int)bumpOffsetZ < num6 - num2 || (int)bumpOffsetZ > num5 + num2)
				{
					return false;
				}
			}
			if (placedByPlayer == null)
			{
				return false;
			}
			if (PhotonNetwork.IsMasterClient && piece3 != null)
			{
				Vector3 vector;
				Quaternion quaternion;
				Vector3 vector2;
				Quaternion quaternion2;
				piece2.BumpTwistToPositionRotation(twist, bumpOffsetX, bumpOffsetZ, attachIndex, piece3.gridPlanes[parentAttachIndex], out vector, out quaternion, out vector2, out quaternion2);
				Vector3 vector3 = piece2.transform.InverseTransformPoint(piece.transform.position);
				Vector3 vector4 = vector2 + quaternion2 * vector3;
				if (!this.IsPlayerHandNearAction(placedByPlayer, vector4, piece.heldInLeftHand, false, 2.5f))
				{
					return false;
				}
				if (!this.ValidatePieceWorldTransform(vector2, quaternion2))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600613C RID: 24892 RVA: 0x001EF214 File Offset: 0x001ED414
		public bool ValidatePlacePieceState(int pieceId, int attachPieceId, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int parentPieceId, int attachIndex, int parentAttachIndex, Player placedByPlayer)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			BuilderPiece piece2 = this.GetPiece(attachPieceId);
			return !(piece2 == null) && !(this.GetPiece(parentPieceId) == null) && placedByPlayer != null && !piece2.GetRootPiece() != piece;
		}

		// Token: 0x0600613D RID: 24893 RVA: 0x001EF278 File Offset: 0x001ED478
		public void ExecutePieceCreated(BuilderTable.BuilderCommand cmd)
		{
			if ((cmd.player == null || !cmd.player.IsLocal) && !this.ValidateCreatePieceParams(cmd.pieceType, cmd.pieceId, cmd.state, cmd.materialType))
			{
				return;
			}
			BuilderPiece builderPiece = this.CreatePieceInternal(cmd.pieceType, cmd.pieceId, cmd.localPosition, cmd.localRotation, cmd.state, cmd.materialType, 0, this);
			if (!(builderPiece != null) || cmd.state != BuilderPiece.State.OnConveyor)
			{
				if (builderPiece != null && cmd.isLeft && cmd.state == BuilderPiece.State.OnShelf)
				{
					if (cmd.parentPieceId < 0 || cmd.parentPieceId >= this.dispenserShelves.Count)
					{
						return;
					}
					builderPiece.shelfOwner = cmd.parentPieceId;
					this.dispenserShelves[builderPiece.shelfOwner].OnShelfPieceCreated(builderPiece, true);
				}
				return;
			}
			if (cmd.parentPieceId < 0 || cmd.parentPieceId >= this.conveyors.Count)
			{
				return;
			}
			builderPiece.shelfOwner = cmd.parentPieceId;
			BuilderConveyor builderConveyor = this.conveyors[builderPiece.shelfOwner];
			int parentAttachIndex = cmd.parentAttachIndex;
			float num = 0f;
			if (PhotonNetwork.ServerTimestamp > parentAttachIndex)
			{
				num = (PhotonNetwork.ServerTimestamp - parentAttachIndex) / 1000f;
			}
			builderConveyor.OnShelfPieceCreated(builderPiece, num);
		}

		// Token: 0x0600613E RID: 24894 RVA: 0x001EF3BF File Offset: 0x001ED5BF
		public void ExecutePieceRecycled(BuilderTable.BuilderCommand cmd)
		{
			this.RecyclePieceInternal(cmd.pieceId, false, cmd.isLeft, cmd.parentPieceId);
		}

		// Token: 0x0600613F RID: 24895 RVA: 0x001EF3DA File Offset: 0x001ED5DA
		private bool ValidateCreatePieceParams(int newPieceType, int newPieceId, BuilderPiece.State state, int materialType)
		{
			return !(this.GetPiecePrefab(newPieceType) == null) && !(this.GetPiece(newPieceId) != null);
		}

		// Token: 0x06006140 RID: 24896 RVA: 0x001EF400 File Offset: 0x001ED600
		private bool ValidateDeserializedRootPieceState(int pieceId, BuilderPiece.State state, int shelfOwner, int heldByActor, Vector3 localPosition, Quaternion localRotation)
		{
			switch (state)
			{
			case BuilderPiece.State.Grabbed:
			case BuilderPiece.State.GrabbedLocal:
				if (heldByActor == -1)
				{
					return false;
				}
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. held piece in immutable table {0}", pieceId), null);
					return false;
				}
				if (localPosition.sqrMagnitude > 6.25f)
				{
					return false;
				}
				break;
			case BuilderPiece.State.Dropped:
				if (!this.ValidatePieceWorldTransform(localPosition, localRotation))
				{
					return false;
				}
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. dropped piece in immutable table {0}", pieceId), null);
					return false;
				}
				break;
			case BuilderPiece.State.OnShelf:
			case BuilderPiece.State.Displayed:
				if (!this.isTableMutable || shelfOwner == -1)
				{
					if (!this.ValidatePieceWorldTransform(localPosition, localRotation))
					{
						return false;
					}
				}
				else if (shelfOwner < 0 || shelfOwner > this.dispenserShelves.Count - 1)
				{
					return false;
				}
				break;
			case BuilderPiece.State.OnConveyor:
				if (shelfOwner == -1)
				{
					return false;
				}
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. OnConveyor piece in immutable table {0}", pieceId), null);
					return false;
				}
				if (shelfOwner < 0 || shelfOwner > this.conveyors.Count - 1)
				{
					return false;
				}
				break;
			case BuilderPiece.State.AttachedToArm:
				if (heldByActor == -1)
				{
					return false;
				}
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. AttachedToArm piece in immutable table {0}", pieceId), null);
					return false;
				}
				if (localPosition.sqrMagnitude > 6.25f)
				{
					return false;
				}
				break;
			default:
				return false;
			}
			return true;
		}

		// Token: 0x06006141 RID: 24897 RVA: 0x001EF54C File Offset: 0x001ED74C
		private bool ValidateDeserializedChildPieceState(int pieceId, BuilderPiece.State state)
		{
			switch (state)
			{
			case BuilderPiece.State.AttachedAndPlaced:
			case BuilderPiece.State.OnShelf:
			case BuilderPiece.State.Displayed:
				return true;
			case BuilderPiece.State.AttachedToDropped:
			case BuilderPiece.State.Grabbed:
			case BuilderPiece.State.GrabbedLocal:
			case BuilderPiece.State.AttachedToArm:
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. Invalid state {0} of child piece {1} in Immutable table", state, pieceId), null);
					return false;
				}
				return true;
			}
			return false;
		}

		// Token: 0x06006142 RID: 24898 RVA: 0x001EF5B0 File Offset: 0x001ED7B0
		public bool ValidatePieceWorldTransform(Vector3 position, Quaternion rotation)
		{
			float num = 10000f;
			return (in position).IsValid(in num) && (in rotation).IsValid() && (this.roomCenter.position - position).sqrMagnitude <= this.acceptableSqrDistFromCenter && this.ValidatePositionInArea(position);
		}

		// Token: 0x06006143 RID: 24899 RVA: 0x001EF604 File Offset: 0x001ED804
		public bool ValidatePositionInArea(Vector3 position)
		{
			using (List<SimpleAABB>.Enumerator enumerator = this.m_areaBounds.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsInBounds(position))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06006144 RID: 24900 RVA: 0x001EF660 File Offset: 0x001ED860
		private BuilderPiece CreatePieceInternal(int newPieceType, int newPieceId, Vector3 position, Quaternion rotation, BuilderPiece.State state, int materialType, int activateTimeStamp, BuilderTable table)
		{
			if (this.GetPiecePrefab(newPieceType) == null)
			{
				return null;
			}
			if (!PhotonNetwork.IsMasterClient)
			{
				this.nextPieceId = newPieceId + 1;
			}
			BuilderPiece builderPiece = this.builderPool.CreatePiece(newPieceType, false);
			builderPiece.SetScale(table.pieceScale);
			builderPiece.transform.SetPositionAndRotation(position, rotation);
			builderPiece.pieceType = newPieceType;
			builderPiece.pieceId = newPieceId;
			builderPiece.SetTable(table);
			builderPiece.gameObject.SetActive(true);
			builderPiece.SetupPiece(this.gridSize);
			builderPiece.OnCreate();
			builderPiece.activatedTimeStamp = ((state == BuilderPiece.State.AttachedAndPlaced) ? activateTimeStamp : 0);
			builderPiece.SetMaterial(materialType, true);
			builderPiece.SetState(state, true);
			this.AddPiece(builderPiece);
			return builderPiece;
		}

		// Token: 0x06006145 RID: 24901 RVA: 0x001EF714 File Offset: 0x001ED914
		private void RecyclePieceInternal(int pieceId, bool ignoreHaptics, bool playFX, int recyclerId)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			if (playFX)
			{
				try
				{
					piece.PlayRecycleFx();
				}
				catch (Exception)
				{
				}
			}
			if (!ignoreHaptics)
			{
				BuilderPiece rootPiece = piece.GetRootPiece();
				if (rootPiece != null && rootPiece.IsHeldLocal())
				{
					GorillaTagger.Instance.StartVibration(piece.IsHeldInLeftHand(), GorillaTagger.Instance.tapHapticStrength, this.pushAndEaseParams.snapDelayTime * 2f);
				}
			}
			BuilderPiece builderPiece = piece.firstChildPiece;
			while (builderPiece != null)
			{
				int pieceId2 = builderPiece.pieceId;
				builderPiece = builderPiece.nextSiblingPiece;
				this.RecyclePieceInternal(pieceId2, true, playFX, recyclerId);
			}
			if (this.isTableMutable && recyclerId >= 0 && recyclerId < this.recyclers.Count)
			{
				this.recyclers[recyclerId].OnRecycleRequestedAtRecycler(piece);
			}
			if (piece.state == BuilderPiece.State.OnConveyor && piece.shelfOwner >= 0 && piece.shelfOwner < this.conveyors.Count)
			{
				this.conveyors[piece.shelfOwner].OnShelfPieceRecycled(piece);
			}
			else if ((piece.state == BuilderPiece.State.OnShelf || piece.state == BuilderPiece.State.Displayed) && piece.shelfOwner >= 0 && piece.shelfOwner < this.dispenserShelves.Count)
			{
				this.dispenserShelves[piece.shelfOwner].OnShelfPieceRecycled(piece);
			}
			if (piece.isArmShelf && this.isTableMutable)
			{
				if (piece.armShelf != null)
				{
					piece.armShelf.piece = null;
					piece.armShelf = null;
				}
				int num;
				if (piece.heldInLeftHand && this.playerToArmShelfLeft.TryGetValue(piece.heldByPlayerActorNumber, out num) && num == piece.pieceId)
				{
					this.playerToArmShelfLeft.Remove(piece.heldByPlayerActorNumber);
				}
				int num2;
				if (!piece.heldInLeftHand && this.playerToArmShelfRight.TryGetValue(piece.heldByPlayerActorNumber, out num2) && num2 == piece.pieceId)
				{
					this.playerToArmShelfRight.Remove(piece.heldByPlayerActorNumber);
				}
			}
			else if (PhotonNetwork.LocalPlayer.ActorNumber == piece.heldByPlayerActorNumber)
			{
				BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
			}
			int pieceId3 = piece.pieceId;
			piece.ClearParentPiece(false);
			piece.ClearParentHeld();
			piece.SetState(BuilderPiece.State.None, false);
			this.RemovePiece(piece);
			this.builderPool.DestroyPiece(piece);
		}

		// Token: 0x06006146 RID: 24902 RVA: 0x001EF970 File Offset: 0x001EDB70
		public BuilderPiece GetPiecePrefab(int pieceType)
		{
			return BuilderSetManager.instance.GetPiecePrefab(pieceType);
		}

		// Token: 0x06006147 RID: 24903 RVA: 0x001EF980 File Offset: 0x001EDB80
		private bool ValidateAttachPieceParams(int pieceId, int attachIndex, int parentId, int parentAttachIndex, int piecePlacement)
		{
			if (pieceId == parentId)
			{
				return false;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			BuilderPiece piece2 = this.GetPiece(parentId);
			if (piece2 == null)
			{
				return false;
			}
			if ((piecePlacement & 262143) != piecePlacement)
			{
				return false;
			}
			if (piece.isBuiltIntoTable)
			{
				return false;
			}
			if (this.DoesChainContainChain(piece, piece2))
			{
				return false;
			}
			if (attachIndex < 0 || attachIndex >= piece.gridPlanes.Count)
			{
				return false;
			}
			if (parentAttachIndex < 0 || parentAttachIndex >= piece2.gridPlanes.Count)
			{
				return false;
			}
			byte b;
			sbyte b2;
			sbyte b3;
			BuilderTable.UnpackPiecePlacement(piecePlacement, out b, out b2, out b3);
			bool flag = (long)(b % 2) == 1L;
			BuilderAttachGridPlane builderAttachGridPlane = piece.gridPlanes[attachIndex];
			int num = (flag ? builderAttachGridPlane.length : builderAttachGridPlane.width);
			int num2 = (flag ? builderAttachGridPlane.width : builderAttachGridPlane.length);
			BuilderAttachGridPlane builderAttachGridPlane2 = piece2.gridPlanes[parentAttachIndex];
			int num3 = Mathf.FloorToInt((float)builderAttachGridPlane2.width / 2f);
			int num4 = num3 - (builderAttachGridPlane2.width - 1);
			if ((int)b2 < num4 - num || (int)b2 > num3 + num)
			{
				return false;
			}
			int num5 = Mathf.FloorToInt((float)builderAttachGridPlane2.length / 2f);
			int num6 = num5 - (builderAttachGridPlane2.length - 1);
			return (int)b3 >= num6 - num2 && (int)b3 <= num5 + num2;
		}

		// Token: 0x06006148 RID: 24904 RVA: 0x001EFAD0 File Offset: 0x001EDCD0
		private void AttachPieceInternal(int pieceId, int attachIndex, int parentId, int parentAttachIndex, int placement)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			BuilderPiece piece2 = this.GetPiece(parentId);
			if (piece == null)
			{
				return;
			}
			byte b;
			sbyte b2;
			sbyte b3;
			BuilderTable.UnpackPiecePlacement(placement, out b, out b2, out b3);
			Vector3 zero = Vector3.zero;
			Quaternion quaternion;
			if (piece2 != null && parentAttachIndex >= 0 && parentAttachIndex < piece2.gridPlanes.Count)
			{
				Vector3 vector;
				Quaternion quaternion2;
				piece.BumpTwistToPositionRotation(b, b2, b3, attachIndex, piece2.gridPlanes[parentAttachIndex], out zero, out quaternion, out vector, out quaternion2);
			}
			else
			{
				quaternion = Quaternion.Euler(0f, (float)b * 90f, 0f);
			}
			piece.SetParentPiece(attachIndex, piece2, parentAttachIndex);
			piece.transform.SetLocalPositionAndRotation(zero, quaternion);
		}

		// Token: 0x06006149 RID: 24905 RVA: 0x001EFB7C File Offset: 0x001EDD7C
		private void AttachPieceToActorInternal(int pieceId, int actorNumber, bool isLeftHand)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
			{
				return;
			}
			VRRig rig = rigContainer.Rig;
			BodyDockPositions myBodyDockPositions = rig.myBodyDockPositions;
			Transform transform = (isLeftHand ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform);
			if (piece.isArmShelf)
			{
				if (!this.isTableMutable)
				{
					return;
				}
				transform = (isLeftHand ? rig.builderArmShelfLeft.pieceAnchor : rig.builderArmShelfRight.pieceAnchor);
				if (isLeftHand)
				{
					rig.builderArmShelfLeft.piece = piece;
					piece.armShelf = rig.builderArmShelfLeft;
					int num;
					if (this.playerToArmShelfLeft.TryGetValue(actorNumber, out num) && num != pieceId)
					{
						BuilderPiece piece2 = this.GetPiece(num);
						if (piece2 != null && piece2.isArmShelf)
						{
							piece2.ClearParentHeld();
							this.playerToArmShelfLeft.Remove(actorNumber);
							Vector3 vector;
							Quaternion quaternion;
							piece2.transform.GetPositionAndRotation(out vector, out quaternion);
							if (!this.ValidatePieceWorldTransform(vector, quaternion))
							{
								this.RecyclePieceInternal(piece2.pieceId, true, false, -1);
							}
						}
					}
					this.playerToArmShelfLeft.TryAdd(actorNumber, pieceId);
				}
				else
				{
					rig.builderArmShelfRight.piece = piece;
					piece.armShelf = rig.builderArmShelfRight;
					int num2;
					if (this.playerToArmShelfRight.TryGetValue(actorNumber, out num2) && num2 != pieceId)
					{
						BuilderPiece piece3 = this.GetPiece(num2);
						if (piece3 != null && piece3.isArmShelf)
						{
							piece3.ClearParentHeld();
							this.playerToArmShelfRight.Remove(actorNumber);
							Vector3 vector2;
							Quaternion quaternion2;
							piece3.transform.GetPositionAndRotation(out vector2, out quaternion2);
							if (!this.ValidatePieceWorldTransform(vector2, quaternion2))
							{
								this.RecyclePieceInternal(piece3.pieceId, true, false, -1);
							}
						}
					}
					this.playerToArmShelfRight.TryAdd(actorNumber, pieceId);
				}
			}
			Vector3 localPosition = piece.transform.localPosition;
			Quaternion localRotation = piece.transform.localRotation;
			piece.ClearParentHeld();
			piece.ClearParentPiece(false);
			piece.SetParentHeld(transform, actorNumber, isLeftHand);
			piece.transform.SetLocalPositionAndRotation(localPosition, localRotation);
			BuilderPiece.State state = (player.IsLocal ? BuilderPiece.State.GrabbedLocal : BuilderPiece.State.Grabbed);
			if (piece.isArmShelf)
			{
				state = BuilderPiece.State.AttachedToArm;
				piece.transform.localScale = Vector3.one;
			}
			piece.SetState(state, false);
			if (!player.IsLocal)
			{
				BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
			}
			if (player.IsLocal && !piece.isArmShelf)
			{
				BuilderPieceInteractor.instance.AddPieceToHeld(piece, isLeftHand, localPosition, localRotation);
			}
		}

		// Token: 0x0600614A RID: 24906 RVA: 0x001EFDF0 File Offset: 0x001EDFF0
		public void RequestPlacePiece(BuilderPiece piece, BuilderPiece attachPiece, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, BuilderPiece parentPiece, int attachIndex, int parentAttachIndex)
		{
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				return;
			}
			this.builderNetworking.RequestPlacePiece(piece, attachPiece, bumpOffsetX, bumpOffsetZ, twist, parentPiece, attachIndex, parentAttachIndex);
		}

		// Token: 0x0600614B RID: 24907 RVA: 0x001EFE20 File Offset: 0x001EE020
		public void PlacePiece(int localCommandId, int pieceId, int attachPieceId, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int parentPieceId, int attachIndex, int parentAttachIndex, NetPlayer placedByPlayer, int timeStamp, bool force)
		{
			this.PiecePlacedInternal(localCommandId, pieceId, attachPieceId, bumpOffsetX, bumpOffsetZ, twist, parentPieceId, attachIndex, parentAttachIndex, placedByPlayer, timeStamp, force);
		}

		// Token: 0x0600614C RID: 24908 RVA: 0x001EFE48 File Offset: 0x001EE048
		public void PiecePlacedInternal(int localCommandId, int pieceId, int attachPieceId, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int parentPieceId, int attachIndex, int parentAttachIndex, NetPlayer placedByPlayer, int timeStamp, bool force)
		{
			if (!force && placedByPlayer == NetworkSystem.Instance.LocalPlayer && this.HasRollForwardCommand(localCommandId) && this.TryRollbackAndReExecute(localCommandId))
			{
				return;
			}
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Place,
				pieceId = pieceId,
				bumpOffsetX = bumpOffsetX,
				bumpOffsetZ = bumpOffsetZ,
				twist = twist,
				attachPieceId = attachPieceId,
				parentPieceId = parentPieceId,
				attachIndex = attachIndex,
				parentAttachIndex = parentAttachIndex,
				player = placedByPlayer,
				canRollback = force,
				localCommandId = localCommandId,
				serverTimeStamp = timeStamp
			};
			this.RouteNewCommand(builderCommand, force);
		}

		// Token: 0x0600614D RID: 24909 RVA: 0x001EFF00 File Offset: 0x001EE100
		public void ExecutePiecePlacedWithActions(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			int attachPieceId = cmd.attachPieceId;
			int parentPieceId = cmd.parentPieceId;
			int parentAttachIndex = cmd.parentAttachIndex;
			int attachIndex = cmd.attachIndex;
			NetPlayer player = cmd.player;
			int localCommandId = cmd.localCommandId;
			int actorNumber = cmd.player.ActorNumber;
			byte twist = cmd.twist;
			sbyte bumpOffsetX = cmd.bumpOffsetX;
			sbyte bumpOffsetZ = cmd.bumpOffsetZ;
			if ((player == null || !player.IsLocal) && !this.ValidatePlacePieceParams(pieceId, attachPieceId, bumpOffsetX, bumpOffsetZ, twist, parentPieceId, attachIndex, parentAttachIndex, player))
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			BuilderPiece piece2 = this.GetPiece(attachPieceId);
			if (piece2 == null)
			{
				return;
			}
			BuilderAction builderAction = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, actorNumber);
			BuilderAction builderAction2 = BuilderActions.CreateMakeRoot(localCommandId, attachPieceId);
			BuilderAction builderAction3 = BuilderActions.CreateAttachToPiece(localCommandId, attachPieceId, cmd.parentPieceId, cmd.attachIndex, cmd.parentAttachIndex, bumpOffsetX, bumpOffsetZ, twist, actorNumber, cmd.serverTimeStamp);
			if (cmd.canRollback)
			{
				BuilderAction builderAction4 = BuilderActions.CreateDetachFromPiece(localCommandId, attachPieceId, actorNumber);
				BuilderAction builderAction5 = BuilderActions.CreateMakeRoot(localCommandId, pieceId);
				BuilderAction builderAction6 = BuilderActions.CreateAttachToPlayerRollback(localCommandId, piece);
				this.AddRollbackAction(builderAction6);
				this.AddRollbackAction(builderAction5);
				this.AddRollbackAction(builderAction4);
				this.AddRollForwardCommand(cmd);
			}
			this.ExecuteAction(builderAction);
			this.ExecuteAction(builderAction2);
			this.ExecuteAction(builderAction3);
			if (!cmd.isQueued)
			{
				piece2.PlayPlacementFx();
			}
		}

		// Token: 0x0600614E RID: 24910 RVA: 0x001F0064 File Offset: 0x001EE264
		public bool ValidateGrabPieceParams(int pieceId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer)
		{
			float num = 10000f;
			if (!(in localPosition).IsValid(in num) || !(in localRotation).IsValid())
			{
				return false;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			if (piece.isBuiltIntoTable)
			{
				return false;
			}
			if (grabbedByPlayer == null)
			{
				return false;
			}
			if (!piece.CanPlayerGrabPiece(grabbedByPlayer.ActorNumber, piece.transform.position))
			{
				return false;
			}
			if (localPosition.sqrMagnitude > 6400f)
			{
				return false;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				Vector3 position = piece.transform.position;
				if (!this.IsPlayerHandNearAction(grabbedByPlayer, position, isLeftHand, false, 2.5f))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600614F RID: 24911 RVA: 0x001F0104 File Offset: 0x001EE304
		public bool ValidateGrabPieceState(int pieceId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, Player grabbedByPlayer)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			return !(piece == null) && piece.state != BuilderPiece.State.Displayed && piece.state != BuilderPiece.State.None;
		}

		// Token: 0x06006150 RID: 24912 RVA: 0x001F013C File Offset: 0x001EE33C
		public bool IsLocationWithinSharedBuildArea(Vector3 worldPosition)
		{
			Vector3 vector = this.sharedBuildArea.transform.InverseTransformPoint(worldPosition);
			foreach (BoxCollider boxCollider in this.sharedBuildAreas)
			{
				Vector3 vector2 = boxCollider.center + boxCollider.size / 2f;
				Vector3 vector3 = boxCollider.center - boxCollider.size / 2f;
				if (vector.x >= vector3.x && vector.x <= vector2.x && vector.y >= vector3.y && vector.y <= vector2.y && vector.z >= vector3.z && vector.z <= vector2.z)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06006151 RID: 24913 RVA: 0x001F021C File Offset: 0x001EE41C
		private bool NoBlocksCheck()
		{
			foreach (BuilderTable.BoxCheckParams boxCheckParams in this.noBlocksAreas)
			{
				DebugUtil.DrawBox(boxCheckParams.center, boxCheckParams.rotation, boxCheckParams.halfExtents * 2f, Color.magenta, true, DebugUtil.Style.Wireframe);
				int num = 0;
				num |= 1 << BuilderTable.placedLayer;
				int num2 = Physics.OverlapBoxNonAlloc(boxCheckParams.center, boxCheckParams.halfExtents, this.noBlocksCheckResults, boxCheckParams.rotation, num);
				for (int i = 0; i < num2; i++)
				{
					BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(this.noBlocksCheckResults[i]);
					if (builderPieceFromCollider != null && builderPieceFromCollider.GetTable() == this && builderPieceFromCollider.state == BuilderPiece.State.AttachedAndPlaced && !builderPieceFromCollider.isBuiltIntoTable)
					{
						GTDev.LogError<string>(string.Format("Builder Table found piece {0} {1} in NO BLOCK AREA {2}", builderPieceFromCollider.pieceId, builderPieceFromCollider.displayName, builderPieceFromCollider.transform.position), null);
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06006152 RID: 24914 RVA: 0x001F034C File Offset: 0x001EE54C
		public void RequestGrabPiece(BuilderPiece piece, bool isLefHand, Vector3 localPosition, Quaternion localRotation)
		{
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				return;
			}
			this.builderNetworking.RequestGrabPiece(piece, isLefHand, localPosition, localRotation);
		}

		// Token: 0x06006153 RID: 24915 RVA: 0x001F0368 File Offset: 0x001EE568
		public void GrabPiece(int localCommandId, int pieceId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer, bool force)
		{
			this.PieceGrabbedInternal(localCommandId, pieceId, isLeftHand, localPosition, localRotation, grabbedByPlayer, force);
		}

		// Token: 0x06006154 RID: 24916 RVA: 0x001F037C File Offset: 0x001EE57C
		public void PieceGrabbedInternal(int localCommandId, int pieceId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer, bool force)
		{
			if (!force && grabbedByPlayer == NetworkSystem.Instance.LocalPlayer && this.HasRollForwardCommand(localCommandId) && this.TryRollbackAndReExecute(localCommandId))
			{
				return;
			}
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Grab,
				pieceId = pieceId,
				attachPieceId = -1,
				isLeft = isLeftHand,
				localPosition = localPosition,
				localRotation = localRotation,
				player = grabbedByPlayer,
				canRollback = force,
				localCommandId = localCommandId
			};
			this.RouteNewCommand(builderCommand, force);
		}

		// Token: 0x06006155 RID: 24917 RVA: 0x001F0410 File Offset: 0x001EE610
		public void ExecutePieceGrabbedWithActions(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			bool isLeft = cmd.isLeft;
			NetPlayer player = cmd.player;
			Vector3 localPosition = cmd.localPosition;
			Quaternion localRotation = cmd.localRotation;
			int localCommandId = cmd.localCommandId;
			int actorNumber = cmd.player.ActorNumber;
			if ((player == null || !player.Equals(NetworkSystem.Instance.LocalPlayer)) && !this.ValidateGrabPieceParams(pieceId, isLeft, localPosition, localRotation, player))
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			bool flag = PhotonNetwork.CurrentRoom.GetPlayer(piece.heldByPlayerActorNumber, false) != null;
			bool flag2 = BuilderPiece.IsDroppedState(piece.state);
			bool flag3 = piece.state == BuilderPiece.State.OnConveyor || piece.state == BuilderPiece.State.OnShelf || piece.state == BuilderPiece.State.Displayed;
			BuilderAction builderAction = BuilderActions.CreateAttachToPlayer(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, actorNumber, cmd.isLeft);
			BuilderAction builderAction2 = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, actorNumber);
			if (flag)
			{
				BuilderAction builderAction3 = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, piece.heldByPlayerActorNumber);
				if (cmd.canRollback)
				{
					BuilderAction builderAction4 = BuilderActions.CreateAttachToPlayerRollback(localCommandId, piece);
					this.AddRollbackAction(builderAction4);
					this.AddRollbackAction(builderAction2);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(builderAction3);
				this.ExecuteAction(builderAction);
				return;
			}
			if (flag3)
			{
				BuilderAction builderAction5;
				if (piece.state == BuilderPiece.State.OnConveyor)
				{
					int serverTimestamp = PhotonNetwork.ServerTimestamp;
					float splineProgressForPiece = this.conveyorManager.GetSplineProgressForPiece(piece);
					builderAction5 = BuilderActions.CreateAttachToShelfRollback(localCommandId, piece, piece.shelfOwner, true, serverTimestamp, splineProgressForPiece);
				}
				else
				{
					if (piece.state == BuilderPiece.State.Displayed)
					{
						int actorNumber2 = NetworkSystem.Instance.LocalPlayer.ActorNumber;
					}
					builderAction5 = BuilderActions.CreateAttachToShelfRollback(localCommandId, piece, piece.shelfOwner, false, 0, 0f);
				}
				BuilderAction builderAction6 = BuilderActions.CreateMakeRoot(localCommandId, pieceId);
				BuilderPiece rootPiece = piece.GetRootPiece();
				BuilderAction builderAction7 = BuilderActions.CreateMakeRoot(localCommandId, rootPiece.pieceId);
				if (cmd.canRollback)
				{
					this.AddRollbackAction(builderAction5);
					this.AddRollbackAction(builderAction7);
					this.AddRollbackAction(builderAction2);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(builderAction6);
				this.ExecuteAction(builderAction);
				return;
			}
			if (flag2)
			{
				BuilderAction builderAction8 = BuilderActions.CreateMakeRoot(localCommandId, pieceId);
				BuilderPiece rootPiece2 = piece.GetRootPiece();
				BuilderAction builderAction9 = BuilderActions.CreateDropPieceRollback(localCommandId, rootPiece2, actorNumber);
				BuilderAction builderAction10 = BuilderActions.CreateMakeRoot(localCommandId, rootPiece2.pieceId);
				if (cmd.canRollback)
				{
					this.AddRollbackAction(builderAction9);
					this.AddRollbackAction(builderAction10);
					this.AddRollbackAction(builderAction2);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(builderAction8);
				this.ExecuteAction(builderAction);
				return;
			}
			if (piece.parentPiece != null)
			{
				BuilderAction builderAction11 = BuilderActions.CreateDetachFromPiece(localCommandId, pieceId, actorNumber);
				BuilderAction builderAction12 = BuilderActions.CreateAttachToPieceRollback(localCommandId, piece, actorNumber);
				if (cmd.canRollback)
				{
					this.AddRollbackAction(builderAction12);
					this.AddRollbackAction(builderAction2);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(builderAction11);
				this.ExecuteAction(builderAction);
			}
		}

		// Token: 0x06006156 RID: 24918 RVA: 0x001F06E4 File Offset: 0x001EE8E4
		public bool ValidateDropPieceParams(int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer droppedByPlayer)
		{
			float num = 10000f;
			if ((in position).IsValid(in num) && (in rotation).IsValid())
			{
				float num2 = 10000f;
				if ((in velocity).IsValid(in num2))
				{
					float num3 = 10000f;
					if ((in angVelocity).IsValid(in num3))
					{
						BuilderPiece piece = this.GetPiece(pieceId);
						if (piece == null)
						{
							return false;
						}
						if (piece.isBuiltIntoTable)
						{
							return false;
						}
						if (droppedByPlayer == null)
						{
							return false;
						}
						if (velocity.sqrMagnitude > BuilderTable.MAX_DROP_VELOCITY * BuilderTable.MAX_DROP_VELOCITY)
						{
							return false;
						}
						if (angVelocity.sqrMagnitude > BuilderTable.MAX_DROP_ANG_VELOCITY * BuilderTable.MAX_DROP_ANG_VELOCITY)
						{
							return false;
						}
						if ((this.roomCenter.position - position).sqrMagnitude > this.acceptableSqrDistFromCenter || !this.ValidatePositionInArea(position))
						{
							return false;
						}
						if (piece.state == BuilderPiece.State.AttachedToArm)
						{
							if (piece.parentPiece == null)
							{
								return false;
							}
							if (piece.parentPiece.heldByPlayerActorNumber != droppedByPlayer.ActorNumber)
							{
								return false;
							}
						}
						else if (piece.heldByPlayerActorNumber != droppedByPlayer.ActorNumber)
						{
							return false;
						}
						return !PhotonNetwork.IsMasterClient || this.IsPlayerHandNearAction(droppedByPlayer, position, piece.heldInLeftHand, false, 2.5f);
					}
				}
			}
			return false;
		}

		// Token: 0x06006157 RID: 24919 RVA: 0x001F080C File Offset: 0x001EEA0C
		public bool ValidateDropPieceState(int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, Player droppedByPlayer)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			bool flag = piece.state == BuilderPiece.State.AttachedToArm;
			return (flag || piece.heldByPlayerActorNumber == droppedByPlayer.ActorNumber) && (!flag || piece.parentPiece.heldByPlayerActorNumber == droppedByPlayer.ActorNumber);
		}

		// Token: 0x06006158 RID: 24920 RVA: 0x001F0862 File Offset: 0x001EEA62
		public void RequestDropPiece(BuilderPiece piece, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity)
		{
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				return;
			}
			this.builderNetworking.RequestDropPiece(piece, position, rotation, velocity, angVelocity);
		}

		// Token: 0x06006159 RID: 24921 RVA: 0x001F0880 File Offset: 0x001EEA80
		public void DropPiece(int localCommandId, int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer droppedByPlayer, bool force)
		{
			this.PieceDroppedInternal(localCommandId, pieceId, position, rotation, velocity, angVelocity, droppedByPlayer, force);
		}

		// Token: 0x0600615A RID: 24922 RVA: 0x001F08A0 File Offset: 0x001EEAA0
		public void PieceDroppedInternal(int localCommandId, int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer droppedByPlayer, bool force)
		{
			if (!force && droppedByPlayer == NetworkSystem.Instance.LocalPlayer && this.HasRollForwardCommand(localCommandId) && this.TryRollbackAndReExecute(localCommandId))
			{
				return;
			}
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Drop,
				pieceId = pieceId,
				parentPieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				velocity = velocity,
				angVelocity = angVelocity,
				player = droppedByPlayer,
				canRollback = force,
				localCommandId = localCommandId
			};
			this.RouteNewCommand(builderCommand, force);
		}

		// Token: 0x0600615B RID: 24923 RVA: 0x001F093C File Offset: 0x001EEB3C
		public void ExecutePieceDroppedWithActions(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			int localCommandId = cmd.localCommandId;
			int actorNumber = cmd.player.ActorNumber;
			if (!this.ValidateDropPieceParams(pieceId, cmd.localPosition, cmd.localRotation, cmd.velocity, cmd.angVelocity, cmd.player))
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			if (piece.state == BuilderPiece.State.AttachedToArm)
			{
				BuilderPiece parentPiece = piece.parentPiece;
				BuilderAction builderAction = BuilderActions.CreateDetachFromPiece(localCommandId, pieceId, actorNumber);
				BuilderAction builderAction2 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, cmd.velocity, cmd.angVelocity, actorNumber);
				if (cmd.canRollback)
				{
					BuilderAction builderAction3 = BuilderActions.CreateAttachToPieceRollback(localCommandId, piece, actorNumber);
					this.AddRollbackAction(builderAction3);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(builderAction);
				this.ExecuteAction(builderAction2);
				return;
			}
			BuilderAction builderAction4 = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, actorNumber);
			BuilderAction builderAction5 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, cmd.velocity, cmd.angVelocity, actorNumber);
			if (cmd.canRollback)
			{
				BuilderAction builderAction6 = BuilderActions.CreateAttachToPlayerRollback(localCommandId, piece);
				this.AddRollbackAction(builderAction6);
				this.AddRollForwardCommand(cmd);
			}
			this.ExecuteAction(builderAction4);
			this.ExecuteAction(builderAction5);
		}

		// Token: 0x0600615C RID: 24924 RVA: 0x001F0A68 File Offset: 0x001EEC68
		public void ExecutePieceRepelled(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			int localCommandId = cmd.localCommandId;
			int actorNumber = cmd.player.ActorNumber;
			int attachPieceId = cmd.attachPieceId;
			BuilderPiece piece = this.GetPiece(pieceId);
			Vector3 vector = cmd.velocity;
			if (piece == null)
			{
				return;
			}
			if (piece.isBuiltIntoTable || piece.isArmShelf)
			{
				return;
			}
			if (piece.state != BuilderPiece.State.Grabbed && piece.state != BuilderPiece.State.GrabbedLocal && piece.state != BuilderPiece.State.Dropped && piece.state != BuilderPiece.State.AttachedToDropped && piece.state != BuilderPiece.State.AttachedToArm)
			{
				return;
			}
			if (attachPieceId >= 0 && attachPieceId < this.dropZones.Count)
			{
				BuilderDropZone builderDropZone = this.dropZones[attachPieceId];
				builderDropZone.PlayEffect();
				if (builderDropZone.overrideDirection)
				{
					vector = builderDropZone.GetRepelDirectionWorld() * BuilderTable.DROP_ZONE_REPEL;
				}
			}
			if (piece.heldByPlayerActorNumber >= 0)
			{
				BuilderAction builderAction = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, piece.heldByPlayerActorNumber);
				BuilderAction builderAction2 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, vector, cmd.angVelocity, actorNumber);
				this.ExecuteAction(builderAction);
				this.ExecuteAction(builderAction2);
				return;
			}
			if (piece.state == BuilderPiece.State.AttachedToArm && piece.parentPiece != null)
			{
				BuilderAction builderAction3 = BuilderActions.CreateDetachFromPiece(localCommandId, pieceId, piece.heldByPlayerActorNumber);
				BuilderAction builderAction4 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, vector, cmd.angVelocity, actorNumber);
				this.ExecuteAction(builderAction3);
				this.ExecuteAction(builderAction4);
				return;
			}
			BuilderAction builderAction5 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, vector, cmd.angVelocity, actorNumber);
			this.ExecuteAction(builderAction5);
		}

		// Token: 0x0600615D RID: 24925 RVA: 0x001F0C08 File Offset: 0x001EEE08
		private void CleanUpDroppedPiece()
		{
			if (!PhotonNetwork.IsMasterClient || this.droppedPieces.Count <= BuilderTable.DROPPED_PIECE_LIMIT)
			{
				return;
			}
			BuilderPiece builderPiece = this.FindFirstSleepingPiece();
			if (builderPiece != null && builderPiece.state == BuilderPiece.State.Dropped)
			{
				this.RequestRecyclePiece(builderPiece, false, -1);
				return;
			}
			Debug.LogErrorFormat("Piece {0} in Dropped List is {1}", new object[] { builderPiece.pieceId, builderPiece.state });
		}

		// Token: 0x0600615E RID: 24926 RVA: 0x001F0C80 File Offset: 0x001EEE80
		public void FreezeDroppedPiece(BuilderPiece piece)
		{
			int num = this.droppedPieces.IndexOf(piece);
			if (num >= 0)
			{
				BuilderTable.DroppedPieceData droppedPieceData = this.droppedPieceData[num];
				droppedPieceData.droppedState = BuilderTable.DroppedPieceState.Frozen;
				droppedPieceData.speedThreshCrossedTime = 0f;
				this.droppedPieceData[num] = droppedPieceData;
				if (piece.rigidBody != null)
				{
					piece.SetKinematic(true, false);
				}
				piece.forcedFrozen = true;
			}
		}

		// Token: 0x0600615F RID: 24927 RVA: 0x001F0CEC File Offset: 0x001EEEEC
		public void AddPieceToDropList(BuilderPiece piece)
		{
			this.droppedPieces.Add(piece);
			this.droppedPieceData.Add(new BuilderTable.DroppedPieceData
			{
				speedThreshCrossedTime = 0f,
				droppedState = BuilderTable.DroppedPieceState.Light,
				filteredSpeed = 0f
			});
		}

		// Token: 0x06006160 RID: 24928 RVA: 0x001F0D3C File Offset: 0x001EEF3C
		private BuilderPiece FindFirstSleepingPiece()
		{
			if (this.droppedPieces.Count < 1)
			{
				return null;
			}
			BuilderPiece builderPiece = this.droppedPieces[0];
			for (int i = 0; i < this.droppedPieces.Count; i++)
			{
				if (this.droppedPieces[i].rigidBody != null && this.droppedPieces[i].rigidBody.IsSleeping())
				{
					BuilderPiece builderPiece2 = this.droppedPieces[i];
					this.droppedPieces.RemoveAt(i);
					this.droppedPieceData.RemoveAt(i);
					return builderPiece2;
				}
			}
			BuilderPiece builderPiece3 = this.droppedPieces[0];
			this.droppedPieces.RemoveAt(0);
			this.droppedPieceData.RemoveAt(0);
			return builderPiece3;
		}

		// Token: 0x06006161 RID: 24929 RVA: 0x001F0DF6 File Offset: 0x001EEFF6
		public void RemovePieceFromDropList(BuilderPiece piece)
		{
			if (piece.state == BuilderPiece.State.Dropped)
			{
				this.droppedPieces.Remove(piece);
			}
		}

		// Token: 0x06006162 RID: 24930 RVA: 0x001F0E10 File Offset: 0x001EF010
		private void UpdateDroppedPieces(float dt)
		{
			for (int i = 0; i < this.droppedPieces.Count; i++)
			{
				if (this.droppedPieceData[i].droppedState == BuilderTable.DroppedPieceState.Frozen && this.droppedPieces[i].state == BuilderPiece.State.Dropped)
				{
					BuilderTable.DroppedPieceData droppedPieceData = this.droppedPieceData[i];
					droppedPieceData.speedThreshCrossedTime += dt;
					if (droppedPieceData.speedThreshCrossedTime > 60f)
					{
						this.droppedPieces[i].forcedFrozen = false;
						this.droppedPieces[i].ClearCollisionHistory();
						this.droppedPieces[i].SetKinematic(false, true);
						droppedPieceData.droppedState = BuilderTable.DroppedPieceState.Light;
						droppedPieceData.speedThreshCrossedTime = 0f;
					}
					this.droppedPieceData[i] = droppedPieceData;
				}
				else
				{
					Rigidbody rigidBody = this.droppedPieces[i].rigidBody;
					if (rigidBody != null)
					{
						BuilderTable.DroppedPieceData droppedPieceData2 = this.droppedPieceData[i];
						float magnitude = rigidBody.linearVelocity.magnitude;
						droppedPieceData2.filteredSpeed = droppedPieceData2.filteredSpeed * 0.95f + magnitude * 0.05f;
						switch (droppedPieceData2.droppedState)
						{
						case BuilderTable.DroppedPieceState.Light:
							droppedPieceData2.speedThreshCrossedTime = ((droppedPieceData2.filteredSpeed < 0.05f) ? (droppedPieceData2.speedThreshCrossedTime + dt) : 0f);
							if (droppedPieceData2.speedThreshCrossedTime > 0f)
							{
								rigidBody.mass = 10000f;
								droppedPieceData2.droppedState = BuilderTable.DroppedPieceState.Heavy;
								droppedPieceData2.speedThreshCrossedTime = 0f;
							}
							break;
						case BuilderTable.DroppedPieceState.Heavy:
							droppedPieceData2.speedThreshCrossedTime += dt;
							droppedPieceData2.speedThreshCrossedTime = ((droppedPieceData2.filteredSpeed > 0.075f) ? (droppedPieceData2.speedThreshCrossedTime + dt) : 0f);
							if (droppedPieceData2.speedThreshCrossedTime > 0.5f)
							{
								rigidBody.mass = 1f;
								droppedPieceData2.droppedState = BuilderTable.DroppedPieceState.Light;
								droppedPieceData2.speedThreshCrossedTime = 0f;
							}
							break;
						}
						this.droppedPieceData[i] = droppedPieceData2;
					}
				}
			}
		}

		// Token: 0x06006163 RID: 24931 RVA: 0x001F1029 File Offset: 0x001EF229
		private void SetLocalPlayerOwnsPlot(bool ownsPlot)
		{
			this.doesLocalPlayerOwnPlot = ownsPlot;
			UnityEvent<bool> onLocalPlayerClaimedPlot = this.OnLocalPlayerClaimedPlot;
			if (onLocalPlayerClaimedPlot == null)
			{
				return;
			}
			onLocalPlayerClaimedPlot.Invoke(this.doesLocalPlayerOwnPlot);
		}

		// Token: 0x06006164 RID: 24932 RVA: 0x001F1048 File Offset: 0x001EF248
		public void PlotClaimed(int plotPieceId, Player claimingPlayer)
		{
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.ClaimPlot,
				pieceId = plotPieceId,
				player = NetPlayer.Get(claimingPlayer)
			};
			this.RouteNewCommand(builderCommand, false);
		}

		// Token: 0x06006165 RID: 24933 RVA: 0x001F1084 File Offset: 0x001EF284
		public void ExecuteClaimPlot(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			NetPlayer player = cmd.player;
			if (pieceId == -1)
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null || !piece.IsPrivatePlot())
			{
				return;
			}
			if (player == null)
			{
				return;
			}
			BuilderPiecePrivatePlot builderPiecePrivatePlot;
			if (this.plotOwners.TryAdd(player.ActorNumber, pieceId) && piece.TryGetPlotComponent(out builderPiecePrivatePlot))
			{
				builderPiecePrivatePlot.ClaimPlotForPlayerNumber(player.ActorNumber);
				if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					this.SetLocalPlayerOwnsPlot(true);
				}
			}
		}

		// Token: 0x06006166 RID: 24934 RVA: 0x001F1108 File Offset: 0x001EF308
		public void PlayerLeftRoom(int playerActorNumber)
		{
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.PlayerLeftRoom,
				pieceId = playerActorNumber,
				player = null
			};
			bool flag = this.tableState == BuilderTable.TableState.WaitForMasterResync;
			this.RouteNewCommand(builderCommand, flag);
		}

		// Token: 0x06006167 RID: 24935 RVA: 0x001F114C File Offset: 0x001EF34C
		public void ExecutePlayerLeftRoom(BuilderTable.BuilderCommand cmd)
		{
			NetPlayer player = cmd.player;
			int num = ((player != null) ? player.ActorNumber : cmd.pieceId);
			this.FreePlotInternal(-1, num);
			int num2;
			if (this.playerToArmShelfLeft.TryGetValue(num, out num2))
			{
				this.RecyclePieceInternal(num2, true, false, -1);
			}
			this.playerToArmShelfLeft.Remove(num);
			int num3;
			if (this.playerToArmShelfRight.TryGetValue(num, out num3))
			{
				this.RecyclePieceInternal(num3, true, false, -1);
			}
			this.playerToArmShelfRight.Remove(num);
		}

		// Token: 0x06006168 RID: 24936 RVA: 0x001F11C8 File Offset: 0x001EF3C8
		public void PlotFreed(int plotPieceId, Player claimingPlayer)
		{
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.FreePlot,
				pieceId = plotPieceId,
				player = NetPlayer.Get(claimingPlayer)
			};
			this.RouteNewCommand(builderCommand, false);
		}

		// Token: 0x06006169 RID: 24937 RVA: 0x001F1204 File Offset: 0x001EF404
		public void ExecuteFreePlot(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			NetPlayer player = cmd.player;
			if (player == null)
			{
				return;
			}
			this.FreePlotInternal(pieceId, player.ActorNumber);
		}

		// Token: 0x0600616A RID: 24938 RVA: 0x001F1230 File Offset: 0x001EF430
		private void FreePlotInternal(int plotPieceId, int requestingPlayer)
		{
			if (plotPieceId == -1 && !this.plotOwners.TryGetValue(requestingPlayer, out plotPieceId))
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(plotPieceId);
			if (piece == null || !piece.IsPrivatePlot())
			{
				return;
			}
			BuilderPiecePrivatePlot builderPiecePrivatePlot;
			if (piece.TryGetPlotComponent(out builderPiecePrivatePlot))
			{
				int ownerActorNumber = builderPiecePrivatePlot.GetOwnerActorNumber();
				this.plotOwners.Remove(ownerActorNumber);
				builderPiecePrivatePlot.FreePlot();
				if (ownerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					this.SetLocalPlayerOwnsPlot(false);
				}
			}
		}

		// Token: 0x0600616B RID: 24939 RVA: 0x001F12A4 File Offset: 0x001EF4A4
		public bool DoesPlayerOwnPlot(int actorNum)
		{
			return this.plotOwners.ContainsKey(actorNum);
		}

		// Token: 0x0600616C RID: 24940 RVA: 0x001F12B2 File Offset: 0x001EF4B2
		public void RequestPaintPiece(int pieceId, int materialType)
		{
			this.builderNetworking.RequestPaintPiece(pieceId, materialType);
		}

		// Token: 0x0600616D RID: 24941 RVA: 0x001F12C1 File Offset: 0x001EF4C1
		public void PaintPiece(int pieceId, int materialType, Player paintingPlayer, bool force)
		{
			this.PaintPieceInternal(pieceId, materialType, paintingPlayer, force);
		}

		// Token: 0x0600616E RID: 24942 RVA: 0x001F12D0 File Offset: 0x001EF4D0
		private void PaintPieceInternal(int pieceId, int materialType, Player paintingPlayer, bool force)
		{
			if (!force && paintingPlayer == PhotonNetwork.LocalPlayer)
			{
				return;
			}
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Paint,
				pieceId = pieceId,
				materialType = materialType,
				player = NetPlayer.Get(paintingPlayer)
			};
			this.RouteNewCommand(builderCommand, force);
		}

		// Token: 0x0600616F RID: 24943 RVA: 0x001F1324 File Offset: 0x001EF524
		public void ExecutePiecePainted(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			int materialType = cmd.materialType;
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece != null && !piece.isBuiltIntoTable)
			{
				piece.SetMaterial(materialType, false);
			}
		}

		// Token: 0x06006170 RID: 24944 RVA: 0x001F1360 File Offset: 0x001EF560
		public void CreateArmShelvesForPlayersInBuilder()
		{
			if (!this.isTableMutable)
			{
				return;
			}
			if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
			{
				foreach (Player player in this.builderNetworking.armShelfRequests)
				{
					if (player != null)
					{
						this.builderNetworking.RequestCreateArmShelfForPlayer(player);
					}
				}
				this.builderNetworking.armShelfRequests.Clear();
			}
		}

		// Token: 0x06006171 RID: 24945 RVA: 0x001F13E8 File Offset: 0x001EF5E8
		public void RemoveArmShelfForPlayer(Player player)
		{
			if (!this.isTableMutable)
			{
				return;
			}
			if (player == null)
			{
				return;
			}
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				this.builderNetworking.armShelfRequests.Remove(player);
				return;
			}
			int num;
			if (this.playerToArmShelfLeft.TryGetValue(player.ActorNumber, out num))
			{
				BuilderPiece piece = this.GetPiece(num);
				this.playerToArmShelfLeft.Remove(player.ActorNumber);
				if (piece.armShelf != null)
				{
					piece.armShelf.piece = null;
					piece.armShelf = null;
				}
				if (PhotonNetwork.IsMasterClient)
				{
					this.builderNetworking.RequestRecyclePiece(num, piece.transform.position, piece.transform.rotation, false, -1);
				}
				else
				{
					this.DropPieceForPlayerLeavingInternal(piece, player.ActorNumber);
				}
			}
			int num2;
			if (this.playerToArmShelfRight.TryGetValue(player.ActorNumber, out num2))
			{
				BuilderPiece piece2 = this.GetPiece(num2);
				this.playerToArmShelfRight.Remove(player.ActorNumber);
				if (piece2.armShelf != null)
				{
					piece2.armShelf.piece = null;
					piece2.armShelf = null;
				}
				if (PhotonNetwork.IsMasterClient)
				{
					this.builderNetworking.RequestRecyclePiece(num2, piece2.transform.position, piece2.transform.rotation, false, -1);
					return;
				}
				this.DropPieceForPlayerLeavingInternal(piece2, player.ActorNumber);
			}
		}

		// Token: 0x06006172 RID: 24946 RVA: 0x001F1534 File Offset: 0x001EF734
		public void DropAllPiecesForPlayerLeaving(int playerActorNumber)
		{
			List<BuilderPiece> list = this.pieces;
			if (list == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				BuilderPiece builderPiece = list[i];
				if (builderPiece != null && builderPiece.heldByPlayerActorNumber == playerActorNumber && (builderPiece.state == BuilderPiece.State.Grabbed || builderPiece.state == BuilderPiece.State.GrabbedLocal))
				{
					this.DropPieceForPlayerLeavingInternal(builderPiece, playerActorNumber);
				}
			}
		}

		// Token: 0x06006173 RID: 24947 RVA: 0x001F1594 File Offset: 0x001EF794
		public void RecycleAllPiecesForPlayerLeaving(int playerActorNumber)
		{
			List<BuilderPiece> list = this.pieces;
			if (list == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				BuilderPiece builderPiece = list[i];
				if (builderPiece != null && builderPiece.heldByPlayerActorNumber == playerActorNumber && (builderPiece.state == BuilderPiece.State.Grabbed || builderPiece.state == BuilderPiece.State.GrabbedLocal))
				{
					this.RecyclePieceForPlayerLeavingInternal(builderPiece, playerActorNumber);
				}
			}
		}

		// Token: 0x06006174 RID: 24948 RVA: 0x001F15F4 File Offset: 0x001EF7F4
		private void DropPieceForPlayerLeavingInternal(BuilderPiece piece, int playerActorNumber)
		{
			BuilderAction builderAction = BuilderActions.CreateDetachFromPlayer(-1, piece.pieceId, playerActorNumber);
			BuilderAction builderAction2 = BuilderActions.CreateDropPiece(-1, piece.pieceId, piece.transform.position, piece.transform.rotation, Vector3.zero, Vector3.zero, playerActorNumber);
			this.ExecuteAction(builderAction);
			this.ExecuteAction(builderAction2);
		}

		// Token: 0x06006175 RID: 24949 RVA: 0x001F164B File Offset: 0x001EF84B
		private void RecyclePieceForPlayerLeavingInternal(BuilderPiece piece, int playerActorNumber)
		{
			this.builderNetworking.RequestRecyclePiece(piece.pieceId, piece.transform.position, piece.transform.rotation, false, -1);
		}

		// Token: 0x06006176 RID: 24950 RVA: 0x001F1678 File Offset: 0x001EF878
		private void DetachPieceForPlayerLeavingInternal(BuilderPiece piece, int playerActorNumber)
		{
			BuilderAction builderAction = BuilderActions.CreateDetachFromPiece(-1, piece.pieceId, playerActorNumber);
			BuilderAction builderAction2 = BuilderActions.CreateDropPiece(-1, piece.pieceId, piece.transform.position, piece.transform.rotation, Vector3.zero, Vector3.zero, playerActorNumber);
			this.ExecuteAction(builderAction);
			this.ExecuteAction(builderAction2);
		}

		// Token: 0x06006177 RID: 24951 RVA: 0x001F16D0 File Offset: 0x001EF8D0
		public void CreateArmShelf(int pieceIdLeft, int pieceIdRight, int pieceType, Player player)
		{
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.CreateArmShelf,
				pieceId = pieceIdLeft,
				pieceType = pieceType,
				player = NetPlayer.Get(player),
				isLeft = true
			};
			this.RouteNewCommand(builderCommand, false);
			BuilderTable.BuilderCommand builderCommand2 = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.CreateArmShelf,
				pieceId = pieceIdRight,
				pieceType = pieceType,
				player = NetPlayer.Get(player),
				isLeft = false
			};
			this.RouteNewCommand(builderCommand2, false);
		}

		// Token: 0x06006178 RID: 24952 RVA: 0x001F1760 File Offset: 0x001EF960
		public void ExecuteArmShelfCreated(BuilderTable.BuilderCommand cmd)
		{
			NetPlayer player = cmd.player;
			if (player == null)
			{
				return;
			}
			bool isLeft = cmd.isLeft;
			if (this.GetPiece(cmd.pieceId) != null)
			{
				return;
			}
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
			{
				BuilderArmShelf builderArmShelf = (isLeft ? rigContainer.Rig.builderArmShelfLeft : rigContainer.Rig.builderArmShelfRight);
				if (builderArmShelf != null)
				{
					if (builderArmShelf.piece != null)
					{
						if (builderArmShelf.piece.isArmShelf && builderArmShelf.piece.isActiveAndEnabled)
						{
							builderArmShelf.piece.armShelf = null;
							this.RecyclePiece(builderArmShelf.piece.pieceId, builderArmShelf.piece.transform.position, builderArmShelf.piece.transform.rotation, false, -1, PhotonNetwork.LocalPlayer);
						}
						else
						{
							builderArmShelf.piece = null;
						}
						BuilderPiece builderPiece = this.CreatePieceInternal(cmd.pieceType, cmd.pieceId, builderArmShelf.pieceAnchor.position, builderArmShelf.pieceAnchor.rotation, BuilderPiece.State.AttachedToArm, -1, 0, this);
						builderArmShelf.piece = builderPiece;
						builderPiece.armShelf = builderArmShelf;
						builderPiece.SetParentHeld(builderArmShelf.pieceAnchor, cmd.player.ActorNumber, isLeft);
						builderPiece.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
						builderPiece.transform.localScale = Vector3.one;
						if (isLeft)
						{
							this.playerToArmShelfLeft.AddOrUpdate(player.ActorNumber, cmd.pieceId);
							return;
						}
						this.playerToArmShelfRight.AddOrUpdate(player.ActorNumber, cmd.pieceId);
						return;
					}
					else
					{
						BuilderPiece builderPiece2 = this.CreatePieceInternal(cmd.pieceType, cmd.pieceId, builderArmShelf.pieceAnchor.position, builderArmShelf.pieceAnchor.rotation, BuilderPiece.State.AttachedToArm, -1, 0, this);
						builderArmShelf.piece = builderPiece2;
						builderPiece2.armShelf = builderArmShelf;
						builderPiece2.SetParentHeld(builderArmShelf.pieceAnchor, cmd.player.ActorNumber, isLeft);
						builderPiece2.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
						builderPiece2.transform.localScale = Vector3.one;
						if (isLeft)
						{
							this.playerToArmShelfLeft.TryAdd(player.ActorNumber, cmd.pieceId);
							return;
						}
						this.playerToArmShelfRight.TryAdd(player.ActorNumber, cmd.pieceId);
					}
				}
			}
		}

		// Token: 0x06006179 RID: 24953 RVA: 0x001F19AC File Offset: 0x001EFBAC
		public void ClearLocalArmShelf()
		{
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			if (offlineVRRig != null)
			{
				BuilderArmShelf builderArmShelf = offlineVRRig.builderArmShelfLeft;
				if (builderArmShelf != null)
				{
					BuilderPiece piece = builderArmShelf.piece;
					builderArmShelf.piece = null;
					if (piece != null)
					{
						piece.transform.SetParent(null);
					}
				}
				builderArmShelf = offlineVRRig.builderArmShelfRight;
				if (builderArmShelf != null)
				{
					BuilderPiece piece2 = builderArmShelf.piece;
					builderArmShelf.piece = null;
					if (piece2 != null)
					{
						piece2.transform.SetParent(null);
					}
				}
			}
		}

		// Token: 0x0600617A RID: 24954 RVA: 0x001F1A34 File Offset: 0x001EFC34
		public void PieceEnteredDropZone(int pieceId, Vector3 worldPos, Quaternion worldRot, int dropZoneId)
		{
			Vector3 vector = (this.roomCenter.position - worldPos).normalized * BuilderTable.DROP_ZONE_REPEL;
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Repel,
				pieceId = pieceId,
				parentPieceId = pieceId,
				attachPieceId = dropZoneId,
				localPosition = worldPos,
				localRotation = worldRot,
				velocity = vector,
				angVelocity = Vector3.zero,
				player = NetworkSystem.Instance.MasterClient,
				canRollback = false
			};
			this.RouteNewCommand(builderCommand, false);
		}

		// Token: 0x0600617B RID: 24955 RVA: 0x001F1AD8 File Offset: 0x001EFCD8
		public bool ValidateRepelPiece(BuilderPiece piece)
		{
			if (!this.isSetup)
			{
				return false;
			}
			if (piece.isBuiltIntoTable || piece.isArmShelf)
			{
				return false;
			}
			if (piece.state == BuilderPiece.State.Grabbed || piece.state == BuilderPiece.State.GrabbedLocal || piece.state == BuilderPiece.State.Dropped || piece.state == BuilderPiece.State.AttachedToDropped || piece.state == BuilderPiece.State.AttachedToArm)
			{
				bool flag = false;
				for (int i = 0; i < this.repelHistoryLength; i++)
				{
					flag = flag || this.repelledPieceRoots[i].Contains(piece.pieceId);
					if (flag)
					{
						return false;
					}
				}
				this.repelledPieceRoots[this.repelHistoryIndex].Add(piece.pieceId);
				return true;
			}
			return false;
		}

		// Token: 0x0600617C RID: 24956 RVA: 0x001F1B7C File Offset: 0x001EFD7C
		public void RepelPieceTowardTable(int pieceID)
		{
			BuilderPiece piece = this.GetPiece(pieceID);
			if (piece == null)
			{
				return;
			}
			Vector3 position = piece.transform.position;
			Quaternion rotation = piece.transform.rotation;
			if (position.y < this.tableCenter.position.y)
			{
				position.y = this.tableCenter.position.y;
			}
			Vector3 vector = (this.tableCenter.position - position).normalized * BuilderTable.DROP_ZONE_REPEL;
			if (piece.IsHeldLocal())
			{
				BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
			}
			piece.ClearParentHeld();
			piece.ClearParentPiece(false);
			piece.transform.localScale = Vector3.one;
			piece.SetState(BuilderPiece.State.Dropped, false);
			piece.transform.SetLocalPositionAndRotation(position, rotation);
			if (piece.rigidBody != null)
			{
				piece.rigidBody.position = position;
				piece.rigidBody.rotation = rotation;
				piece.rigidBody.linearVelocity = vector;
				piece.rigidBody.AddForce(Vector3.up * (BuilderTable.DROP_ZONE_REPEL / 2f) * piece.rigidBody.mass, ForceMode.Impulse);
				piece.rigidBody.angularVelocity = Vector3.zero;
			}
		}

		// Token: 0x0600617D RID: 24957 RVA: 0x001F1CC4 File Offset: 0x001EFEC4
		public BuilderPiece GetPiece(int pieceId)
		{
			int num;
			if (this.pieceIDToIndexCache.TryGetValue(pieceId, out num))
			{
				if (num >= 0 && num < this.pieces.Count)
				{
					return this.pieces[num];
				}
				this.pieceIDToIndexCache.Remove(pieceId);
			}
			for (int i = 0; i < this.pieces.Count; i++)
			{
				if (this.pieces[i].pieceId == pieceId)
				{
					this.pieceIDToIndexCache.Add(pieceId, i);
					return this.pieces[i];
				}
			}
			for (int j = 0; j < this.basePieces.Count; j++)
			{
				if (this.basePieces[j].pieceId == pieceId)
				{
					return this.basePieces[j];
				}
			}
			return null;
		}

		// Token: 0x0600617E RID: 24958 RVA: 0x001F1D89 File Offset: 0x001EFF89
		public void AddPiece(BuilderPiece piece)
		{
			this.pieces.Add(piece);
			this.UseResources(piece);
			this.AddPieceData(piece);
		}

		// Token: 0x0600617F RID: 24959 RVA: 0x001F1DA6 File Offset: 0x001EFFA6
		public void RemovePiece(BuilderPiece piece)
		{
			this.pieces.Remove(piece);
			this.AddResources(piece);
			this.RemovePieceData(piece);
			this.pieceIDToIndexCache.Clear();
		}

		// Token: 0x06006180 RID: 24960 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void CreateData()
		{
		}

		// Token: 0x06006181 RID: 24961 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void DestroyData()
		{
		}

		// Token: 0x06006182 RID: 24962 RVA: 0x00116369 File Offset: 0x00114569
		private int AddPieceData(BuilderPiece piece)
		{
			return -1;
		}

		// Token: 0x06006183 RID: 24963 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void UpdatePieceData(BuilderPiece piece)
		{
		}

		// Token: 0x06006184 RID: 24964 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void RemovePieceData(BuilderPiece piece)
		{
		}

		// Token: 0x06006185 RID: 24965 RVA: 0x00116369 File Offset: 0x00114569
		private int AddGridPlaneData(BuilderAttachGridPlane gridPlane)
		{
			return -1;
		}

		// Token: 0x06006186 RID: 24966 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void RemoveGridPlaneData(BuilderAttachGridPlane gridPlane)
		{
		}

		// Token: 0x06006187 RID: 24967 RVA: 0x00116369 File Offset: 0x00114569
		private int AddPrivatePlotData(BuilderPiecePrivatePlot plot)
		{
			return -1;
		}

		// Token: 0x06006188 RID: 24968 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void RemovePrivatePlotData(BuilderPiecePrivatePlot plot)
		{
		}

		// Token: 0x06006189 RID: 24969 RVA: 0x001F1DCE File Offset: 0x001EFFCE
		public void OnButtonFreeRotation(BuilderOptionButton button, bool isLeftHand)
		{
			this.useSnapRotation = !this.useSnapRotation;
			button.SetPressed(this.useSnapRotation);
		}

		// Token: 0x0600618A RID: 24970 RVA: 0x001F1DEB File Offset: 0x001EFFEB
		public void OnButtonFreePosition(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.usePlacementStyle == BuilderPlacementStyle.Float)
			{
				this.usePlacementStyle = BuilderPlacementStyle.SnapDown;
			}
			else if (this.usePlacementStyle == BuilderPlacementStyle.SnapDown)
			{
				this.usePlacementStyle = BuilderPlacementStyle.Float;
			}
			button.SetPressed(this.usePlacementStyle > BuilderPlacementStyle.Float);
		}

		// Token: 0x0600618B RID: 24971 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnButtonSaveLayout(BuilderOptionButton button, bool isLeftHand)
		{
		}

		// Token: 0x0600618C RID: 24972 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnButtonClearLayout(BuilderOptionButton button, bool isLeftHand)
		{
		}

		// Token: 0x0600618D RID: 24973 RVA: 0x001F1E20 File Offset: 0x001F0020
		public bool TryPlaceGridPlane(BuilderPiece piece, BuilderAttachGridPlane gridPlane, List<BuilderAttachGridPlane> checkGridPlanes, out BuilderPotentialPlacement potentialPlacement)
		{
			potentialPlacement = default(BuilderPotentialPlacement);
			potentialPlacement.Reset();
			Vector3 position = gridPlane.transform.position;
			Quaternion rotation = gridPlane.transform.rotation;
			if (this.gridSize <= 0f)
			{
				return false;
			}
			bool flag = false;
			for (int i = 0; i < checkGridPlanes.Count; i++)
			{
				BuilderAttachGridPlane builderAttachGridPlane = checkGridPlanes[i];
				this.TryPlaceGridPlaneOnGridPlane(piece, gridPlane, position, rotation, builderAttachGridPlane, ref potentialPlacement, ref flag);
			}
			return flag;
		}

		// Token: 0x0600618E RID: 24974 RVA: 0x001F1E94 File Offset: 0x001F0094
		public bool TryPlaceGridPlaneOnGridPlane(BuilderPiece piece, BuilderAttachGridPlane gridPlane, Vector3 gridPlanePos, Quaternion gridPlaneRot, BuilderAttachGridPlane checkGridPlane, ref BuilderPotentialPlacement potentialPlacement, ref bool success)
		{
			if (checkGridPlane.male == gridPlane.male)
			{
				return false;
			}
			if (checkGridPlane.piece == gridPlane.piece)
			{
				return false;
			}
			Transform center = checkGridPlane.center;
			Vector3 position = center.position;
			float sqrMagnitude = (position - gridPlanePos).sqrMagnitude;
			float num = checkGridPlane.boundingRadius + gridPlane.boundingRadius;
			if (sqrMagnitude > num * num)
			{
				return false;
			}
			Quaternion rotation = center.rotation;
			Quaternion quaternion = Quaternion.Inverse(rotation);
			Quaternion quaternion2 = quaternion * gridPlaneRot;
			if (Vector3.Dot(Vector3.up, quaternion2 * Vector3.up) < this.currSnapParams.maxUpDotProduct)
			{
				return false;
			}
			Vector3 vector = quaternion * (gridPlanePos - position);
			float y = vector.y;
			float num2 = -Mathf.Abs(y);
			if (success && num2 < potentialPlacement.score)
			{
				return false;
			}
			if (Mathf.Abs(y) > 1f)
			{
				return false;
			}
			if ((gridPlane.male && y > this.currSnapParams.minOffsetY) || (!gridPlane.male && y < -this.currSnapParams.minOffsetY))
			{
				return false;
			}
			if (Mathf.Abs(y) > this.currSnapParams.maxOffsetY)
			{
				return false;
			}
			Quaternion quaternion3;
			Quaternion quaternion4;
			global::BoingKit.QuaternionUtil.DecomposeSwingTwist(quaternion2, Vector3.up, out quaternion3, out quaternion4);
			float maxTwistDotProduct = this.currSnapParams.maxTwistDotProduct;
			Vector3 vector2 = quaternion4 * Vector3.forward;
			float num3 = Vector3.Dot(vector2, Vector3.forward);
			float num4 = Vector3.Dot(vector2, Vector3.right);
			bool flag = Mathf.Abs(num3) > maxTwistDotProduct;
			bool flag2 = Mathf.Abs(num4) > maxTwistDotProduct;
			if (!flag && !flag2)
			{
				return false;
			}
			float num5;
			uint num6;
			if (flag)
			{
				num5 = ((num3 > 0f) ? 0f : 180f);
				num6 = ((num3 > 0f) ? 0U : 2U);
			}
			else
			{
				num5 = ((num4 > 0f) ? 90f : 270f);
				num6 = ((num4 > 0f) ? 1U : 3U);
			}
			int num7 = (flag2 ? gridPlane.width : gridPlane.length);
			int num8 = (flag2 ? gridPlane.length : gridPlane.width);
			float num9 = ((num8 % 2 == 0) ? (this.gridSize / 2f) : 0f);
			float num10 = ((num7 % 2 == 0) ? (this.gridSize / 2f) : 0f);
			float num11 = ((checkGridPlane.width % 2 == 0) ? (this.gridSize / 2f) : 0f);
			float num12 = ((checkGridPlane.length % 2 == 0) ? (this.gridSize / 2f) : 0f);
			float num13 = num9 - num11;
			float num14 = num10 - num12;
			int num15 = Mathf.RoundToInt((vector.x - num13) / this.gridSize);
			int num16 = Mathf.RoundToInt((vector.z - num14) / this.gridSize);
			int num17 = num15 + Mathf.FloorToInt((float)num8 / 2f);
			int num18 = num16 + Mathf.FloorToInt((float)num7 / 2f);
			int num19 = num17 - (num8 - 1);
			int num20 = num18 - (num7 - 1);
			int num21 = Mathf.FloorToInt((float)checkGridPlane.width / 2f);
			int num22 = Mathf.FloorToInt((float)checkGridPlane.length / 2f);
			int num23 = num21 - (checkGridPlane.width - 1);
			int num24 = num22 - (checkGridPlane.length - 1);
			if (num19 > num21 || num17 < num23 || num20 > num22 || num18 < num24)
			{
				return false;
			}
			BuilderPiece rootPiece = checkGridPlane.piece.GetRootPiece();
			if (BuilderTable.ShareSameRoot(gridPlane.piece, rootPiece))
			{
				return false;
			}
			if (!BuilderPiece.CanPlayerAttachPieceToPiece(PhotonNetwork.LocalPlayer.ActorNumber, gridPlane.piece, rootPiece))
			{
				return false;
			}
			BuilderPiece piece2 = checkGridPlane.piece;
			if (piece2 != null)
			{
				if (piece2.preventSnapUntilMoved > 0)
				{
					return false;
				}
				if (piece2.requestedParentPiece != null && BuilderTable.ShareSameRoot(piece, piece2.requestedParentPiece))
				{
					return false;
				}
			}
			Quaternion quaternion5 = Quaternion.Euler(0f, num5, 0f);
			Quaternion quaternion6 = rotation * quaternion5;
			float num25 = (float)num15 * this.gridSize + num13;
			float num26 = (float)num16 * this.gridSize + num14;
			Vector3 vector3 = new Vector3(num25, 0f, num26);
			Vector3 vector4 = position + rotation * vector3;
			Transform center2 = gridPlane.center;
			Quaternion quaternion7 = quaternion6 * Quaternion.Inverse(center2.localRotation);
			Vector3 vector5 = piece.transform.InverseTransformPoint(center2.position);
			Vector3 vector6 = vector4 - quaternion7 * vector5;
			potentialPlacement.localPosition = vector6;
			potentialPlacement.localRotation = quaternion7;
			potentialPlacement.score = num2;
			success = true;
			potentialPlacement.parentPiece = piece2;
			potentialPlacement.parentAttachIndex = checkGridPlane.attachIndex;
			potentialPlacement.attachDistance = Mathf.Abs(y);
			potentialPlacement.attachPlaneNormal = Vector3.up;
			if (!checkGridPlane.male)
			{
				potentialPlacement.attachPlaneNormal *= -1f;
			}
			if (potentialPlacement.parentPiece != null)
			{
				BuilderAttachGridPlane builderAttachGridPlane = potentialPlacement.parentPiece.gridPlanes[potentialPlacement.parentAttachIndex];
				potentialPlacement.localPosition = builderAttachGridPlane.transform.InverseTransformPoint(potentialPlacement.localPosition);
				potentialPlacement.localRotation = Quaternion.Inverse(builderAttachGridPlane.transform.rotation) * potentialPlacement.localRotation;
			}
			potentialPlacement.parentAttachBounds.min.x = Mathf.Max(num23, num19);
			potentialPlacement.parentAttachBounds.min.y = Mathf.Max(num24, num20);
			potentialPlacement.parentAttachBounds.max.x = Mathf.Min(num21, num17);
			potentialPlacement.parentAttachBounds.max.y = Mathf.Min(num22, num18);
			Vector2Int vector2Int = Vector2Int.zero;
			Vector2Int vector2Int2 = Vector2Int.zero;
			vector2Int.x = potentialPlacement.parentAttachBounds.min.x - num15;
			vector2Int2.x = potentialPlacement.parentAttachBounds.max.x - num15;
			vector2Int.y = potentialPlacement.parentAttachBounds.min.y - num16;
			vector2Int2.y = potentialPlacement.parentAttachBounds.max.y - num16;
			potentialPlacement.twist = (byte)num6;
			potentialPlacement.bumpOffsetX = (sbyte)num15;
			potentialPlacement.bumpOffsetZ = (sbyte)num16;
			int num27 = ((num8 % 2 == 0) ? 1 : 0);
			int num28 = ((num7 % 2 == 0) ? 1 : 0);
			if (flag && num3 < 0f)
			{
				vector2Int = this.Rotate180(vector2Int, num27, num28);
				vector2Int2 = this.Rotate180(vector2Int2, num27, num28);
			}
			else if (flag2 && num4 < 0f)
			{
				vector2Int = this.Rotate270(vector2Int, num27, num28);
				vector2Int2 = this.Rotate270(vector2Int2, num27, num28);
			}
			else if (flag2 && num4 > 0f)
			{
				vector2Int = this.Rotate90(vector2Int, num27, num28);
				vector2Int2 = this.Rotate90(vector2Int2, num27, num28);
			}
			potentialPlacement.attachBounds.min.x = Mathf.Min(vector2Int.x, vector2Int2.x);
			potentialPlacement.attachBounds.min.y = Mathf.Min(vector2Int.y, vector2Int2.y);
			potentialPlacement.attachBounds.max.x = Mathf.Max(vector2Int.x, vector2Int2.x);
			potentialPlacement.attachBounds.max.y = Mathf.Max(vector2Int.y, vector2Int2.y);
			return true;
		}

		// Token: 0x0600618F RID: 24975 RVA: 0x001F260E File Offset: 0x001F080E
		private Vector2Int Rotate90(Vector2Int v, int offsetX, int offsetY)
		{
			return new Vector2Int(v.y * -1 + offsetY, v.x);
		}

		// Token: 0x06006190 RID: 24976 RVA: 0x001F2627 File Offset: 0x001F0827
		private Vector2Int Rotate270(Vector2Int v, int offsetX, int offsetY)
		{
			return new Vector2Int(v.y, v.x * -1 + offsetX);
		}

		// Token: 0x06006191 RID: 24977 RVA: 0x001F2640 File Offset: 0x001F0840
		private Vector2Int Rotate180(Vector2Int v, int offsetX, int offsetY)
		{
			return new Vector2Int(v.x * -1 + offsetX, v.y * -1 + offsetY);
		}

		// Token: 0x06006192 RID: 24978 RVA: 0x001F265D File Offset: 0x001F085D
		public bool ShareSameRoot(BuilderAttachGridPlane plane, BuilderAttachGridPlane otherPlane)
		{
			return !(plane == null) && !(otherPlane == null) && !(otherPlane.piece == null) && BuilderTable.ShareSameRoot(plane.piece, otherPlane.piece);
		}

		// Token: 0x06006193 RID: 24979 RVA: 0x001F2694 File Offset: 0x001F0894
		public static bool ShareSameRoot(BuilderPiece piece, BuilderPiece otherPiece)
		{
			if (otherPiece == null || piece == null)
			{
				return false;
			}
			if (piece == otherPiece)
			{
				return true;
			}
			BuilderPiece builderPiece = piece;
			int num = 2048;
			while (builderPiece.parentPiece != null && !builderPiece.parentPiece.isBuiltIntoTable)
			{
				builderPiece = builderPiece.parentPiece;
				num--;
				if (num <= 0)
				{
					return true;
				}
			}
			num = 2048;
			BuilderPiece builderPiece2 = otherPiece;
			while (builderPiece2.parentPiece != null && !builderPiece2.parentPiece.isBuiltIntoTable)
			{
				builderPiece2 = builderPiece2.parentPiece;
				num--;
				if (num <= 0)
				{
					return true;
				}
			}
			return builderPiece == builderPiece2;
		}

		// Token: 0x06006194 RID: 24980 RVA: 0x001F2734 File Offset: 0x001F0934
		public bool TryPlacePieceOnTableNoDrop(bool leftHand, BuilderPiece testPiece, List<BuilderAttachGridPlane> checkGridPlanesMale, List<BuilderAttachGridPlane> checkGridPlanesFemale, out BuilderPotentialPlacement potentialPlacement)
		{
			potentialPlacement = default(BuilderPotentialPlacement);
			potentialPlacement.Reset();
			if (this == null)
			{
				return false;
			}
			if (testPiece == null)
			{
				return false;
			}
			this.currSnapParams = this.pushAndEaseParams;
			return this.TryPlacePieceGridPlanesOnTableInternal(testPiece, this.maxPlacementChildDepth, checkGridPlanesMale, checkGridPlanesFemale, out potentialPlacement);
		}

		// Token: 0x06006195 RID: 24981 RVA: 0x001F2784 File Offset: 0x001F0984
		public bool TryPlacePieceOnTableNoDropJobs(NativeList<BuilderGridPlaneData> gridPlaneData, NativeList<BuilderPieceData> pieceData, NativeList<BuilderGridPlaneData> checkGridPlaneData, NativeList<BuilderPieceData> checkPieceData, out BuilderPotentialPlacement potentialPlacement, List<BuilderPotentialPlacement> allPlacements)
		{
			potentialPlacement = default(BuilderPotentialPlacement);
			potentialPlacement.Reset();
			if (this == null)
			{
				return false;
			}
			this.currSnapParams = this.pushAndEaseParams;
			NativeQueue<BuilderPotentialPlacementData> nativeQueue = new NativeQueue<BuilderPotentialPlacementData>(Allocator.TempJob);
			new BuilderFindPotentialSnaps
			{
				gridSize = this.gridSize,
				currSnapParams = this.currSnapParams,
				gridPlanes = gridPlaneData,
				checkGridPlanes = checkGridPlaneData,
				worldToLocalPos = Vector3.zero,
				worldToLocalRot = Quaternion.identity,
				localToWorldPos = Vector3.zero,
				localToWorldRot = Quaternion.identity,
				potentialPlacements = nativeQueue.AsParallelWriter()
			}.Schedule(gridPlaneData.Length, 32, default(JobHandle)).Complete();
			BuilderPotentialPlacementData builderPotentialPlacementData = default(BuilderPotentialPlacementData);
			bool flag = false;
			while (!nativeQueue.IsEmpty())
			{
				BuilderPotentialPlacementData builderPotentialPlacementData2 = nativeQueue.Dequeue();
				if (!flag || builderPotentialPlacementData2.score > builderPotentialPlacementData.score)
				{
					builderPotentialPlacementData = builderPotentialPlacementData2;
					flag = true;
				}
			}
			if (flag)
			{
				potentialPlacement = builderPotentialPlacementData.ToPotentialPlacement(this);
			}
			if (flag)
			{
				nativeQueue.Clear();
				this.currSnapParams = this.overlapParams;
				Vector3 vector = -potentialPlacement.attachPiece.transform.position;
				Quaternion quaternion = Quaternion.Inverse(potentialPlacement.attachPiece.transform.rotation);
				BuilderAttachGridPlane builderAttachGridPlane = potentialPlacement.parentPiece.gridPlanes[potentialPlacement.parentAttachIndex];
				Quaternion quaternion2 = builderAttachGridPlane.transform.rotation * potentialPlacement.localRotation;
				Vector3 vector2 = builderAttachGridPlane.transform.TransformPoint(potentialPlacement.localPosition);
				new BuilderFindPotentialSnaps
				{
					gridSize = this.gridSize,
					currSnapParams = this.currSnapParams,
					gridPlanes = gridPlaneData,
					checkGridPlanes = checkGridPlaneData,
					worldToLocalPos = vector,
					worldToLocalRot = quaternion,
					localToWorldPos = vector2,
					localToWorldRot = quaternion2,
					potentialPlacements = nativeQueue.AsParallelWriter()
				}.Schedule(gridPlaneData.Length, 32, default(JobHandle)).Complete();
				while (!nativeQueue.IsEmpty())
				{
					BuilderPotentialPlacementData builderPotentialPlacementData3 = nativeQueue.Dequeue();
					if (builderPotentialPlacementData3.attachDistance < this.currSnapParams.maxBlockSnapDist)
					{
						allPlacements.Add(builderPotentialPlacementData3.ToPotentialPlacement(this));
					}
				}
			}
			nativeQueue.Dispose();
			return flag;
		}

		// Token: 0x06006196 RID: 24982 RVA: 0x001F29F0 File Offset: 0x001F0BF0
		public bool CalcAllPotentialPlacements(NativeList<BuilderGridPlaneData> gridPlaneData, NativeList<BuilderGridPlaneData> checkGridPlaneData, BuilderPotentialPlacement potentialPlacement, List<BuilderPotentialPlacement> allPlacements)
		{
			if (this == null)
			{
				return false;
			}
			bool flag = false;
			this.currSnapParams = this.overlapParams;
			NativeQueue<BuilderPotentialPlacementData> nativeQueue = new NativeQueue<BuilderPotentialPlacementData>(Allocator.TempJob);
			nativeQueue.Clear();
			Vector3 vector = -potentialPlacement.attachPiece.transform.position;
			Quaternion quaternion = Quaternion.Inverse(potentialPlacement.attachPiece.transform.rotation);
			BuilderAttachGridPlane builderAttachGridPlane = potentialPlacement.parentPiece.gridPlanes[potentialPlacement.parentAttachIndex];
			Quaternion quaternion2 = builderAttachGridPlane.transform.rotation * potentialPlacement.localRotation;
			Vector3 vector2 = builderAttachGridPlane.transform.TransformPoint(potentialPlacement.localPosition);
			new BuilderFindPotentialSnaps
			{
				gridSize = this.gridSize,
				currSnapParams = this.currSnapParams,
				gridPlanes = gridPlaneData,
				checkGridPlanes = checkGridPlaneData,
				worldToLocalPos = vector,
				worldToLocalRot = quaternion,
				localToWorldPos = vector2,
				localToWorldRot = quaternion2,
				potentialPlacements = nativeQueue.AsParallelWriter()
			}.Schedule(gridPlaneData.Length, 32, default(JobHandle)).Complete();
			while (!nativeQueue.IsEmpty())
			{
				BuilderPotentialPlacementData builderPotentialPlacementData = nativeQueue.Dequeue();
				if (builderPotentialPlacementData.attachDistance < this.currSnapParams.maxBlockSnapDist)
				{
					allPlacements.Add(builderPotentialPlacementData.ToPotentialPlacement(this));
				}
			}
			nativeQueue.Dispose();
			return flag;
		}

		// Token: 0x06006197 RID: 24983 RVA: 0x001F2B5C File Offset: 0x001F0D5C
		public bool CanPiecesPotentiallySnap(BuilderPiece pieceInHand, BuilderPiece piece)
		{
			BuilderPiece rootPiece = piece.GetRootPiece();
			return !(rootPiece == pieceInHand) && BuilderPiece.CanPlayerAttachPieceToPiece(PhotonNetwork.LocalPlayer.ActorNumber, pieceInHand, rootPiece) && (!(piece.requestedParentPiece != null) || !BuilderTable.ShareSameRoot(pieceInHand, piece.requestedParentPiece)) && piece.preventSnapUntilMoved <= 0;
		}

		// Token: 0x06006198 RID: 24984 RVA: 0x001F2BC0 File Offset: 0x001F0DC0
		public bool CanPiecesPotentiallyOverlap(BuilderPiece pieceInHand, BuilderPiece rootWhenPlaced, BuilderPiece.State stateWhenPlaced, BuilderPiece otherPiece)
		{
			BuilderPiece rootPiece = otherPiece.GetRootPiece();
			if (rootPiece == pieceInHand)
			{
				return false;
			}
			if (!BuilderPiece.CanPlayerAttachPieceToPiece(PhotonNetwork.LocalPlayer.ActorNumber, pieceInHand, rootPiece))
			{
				return false;
			}
			if (otherPiece.requestedParentPiece != null && BuilderTable.ShareSameRoot(pieceInHand, otherPiece.requestedParentPiece))
			{
				return false;
			}
			if (otherPiece.preventSnapUntilMoved > 0)
			{
				return false;
			}
			BuilderPiece.State state = otherPiece.state;
			if (otherPiece.isBuiltIntoTable && !otherPiece.isArmShelf)
			{
				state = BuilderPiece.State.AttachedAndPlaced;
			}
			return BuilderTable.AreStatesCompatibleForOverlap(stateWhenPlaced, state, rootWhenPlaced, rootPiece);
		}

		// Token: 0x06006199 RID: 24985 RVA: 0x001F2C49 File Offset: 0x001F0E49
		public void TryDropPiece(bool leftHand, BuilderPiece testPiece, Vector3 velocity, Vector3 angVelocity)
		{
			if (this == null)
			{
				return;
			}
			if (testPiece == null)
			{
				return;
			}
			this.RequestDropPiece(testPiece, testPiece.transform.position, testPiece.transform.rotation, velocity, angVelocity);
		}

		// Token: 0x0600619A RID: 24986 RVA: 0x001F2C80 File Offset: 0x001F0E80
		public bool TryPlacePieceGridPlanesOnTableInternal(BuilderPiece testPiece, int recurse, List<BuilderAttachGridPlane> checkGridPlanesMale, List<BuilderAttachGridPlane> checkGridPlanesFemale, out BuilderPotentialPlacement potentialPlacement)
		{
			potentialPlacement = default(BuilderPotentialPlacement);
			potentialPlacement.Reset();
			bool flag = false;
			bool flag2 = false;
			if (testPiece != null && testPiece.gridPlanes != null && testPiece.gridPlanes.Count > 0 && testPiece.gridPlanes != null)
			{
				for (int i = 0; i < testPiece.gridPlanes.Count; i++)
				{
					List<BuilderAttachGridPlane> list = (testPiece.gridPlanes[i].male ? checkGridPlanesFemale : checkGridPlanesMale);
					BuilderPotentialPlacement builderPotentialPlacement;
					if (this.TryPlaceGridPlane(testPiece, testPiece.gridPlanes[i], list, out builderPotentialPlacement))
					{
						if (builderPotentialPlacement.attachDistance < this.currSnapParams.snapAttachDistance * 1.1f)
						{
							flag2 = true;
						}
						if (builderPotentialPlacement.score > potentialPlacement.score && testPiece.preventSnapUntilMoved <= 0)
						{
							potentialPlacement = builderPotentialPlacement;
							potentialPlacement.attachIndex = i;
							potentialPlacement.attachPiece = testPiece;
							flag = true;
						}
					}
				}
			}
			if (recurse > 0)
			{
				BuilderPiece builderPiece = testPiece.firstChildPiece;
				while (builderPiece != null)
				{
					BuilderPotentialPlacement builderPotentialPlacement2;
					if (this.TryPlacePieceGridPlanesOnTableInternal(builderPiece, recurse - 1, checkGridPlanesMale, checkGridPlanesFemale, out builderPotentialPlacement2))
					{
						if (builderPotentialPlacement2.attachDistance < this.currSnapParams.snapAttachDistance * 1.1f)
						{
							flag2 = true;
						}
						if (builderPotentialPlacement2.score > potentialPlacement.score && testPiece.preventSnapUntilMoved <= 0)
						{
							potentialPlacement = builderPotentialPlacement2;
							flag = true;
						}
					}
					builderPiece = builderPiece.nextSiblingPiece;
				}
			}
			if (testPiece.preventSnapUntilMoved > 0 && !flag2)
			{
				testPiece.preventSnapUntilMoved--;
				this.UpdatePieceData(testPiece);
			}
			return flag;
		}

		// Token: 0x0600619B RID: 24987 RVA: 0x001F2E04 File Offset: 0x001F1004
		public void TryPlaceRandomlyOnTable(BuilderPiece piece)
		{
			BuilderAttachGridPlane builderAttachGridPlane = piece.gridPlanes[Random.Range(0, piece.gridPlanes.Count)];
			List<BuilderAttachGridPlane> list = this.baseGridPlanes;
			int num = Random.Range(0, list.Count);
			int i = 0;
			while (i < list.Count)
			{
				int num2 = (i + num) % list.Count;
				BuilderAttachGridPlane builderAttachGridPlane2 = list[num2];
				if (builderAttachGridPlane2.male != builderAttachGridPlane.male && !(builderAttachGridPlane2.piece == builderAttachGridPlane.piece) && !this.ShareSameRoot(builderAttachGridPlane, builderAttachGridPlane2))
				{
					Vector3 zero = Vector3.zero;
					Quaternion identity = Quaternion.identity;
					BuilderPiece piece2 = builderAttachGridPlane2.piece;
					int attachIndex = builderAttachGridPlane2.attachIndex;
					Transform center = builderAttachGridPlane.center;
					Quaternion quaternion = builderAttachGridPlane2.transform.rotation * Quaternion.Inverse(center.localRotation);
					Vector3 vector = piece.transform.InverseTransformPoint(center.position);
					Vector3 vector2 = builderAttachGridPlane2.transform.position - quaternion * vector;
					if (piece2 != null)
					{
						BuilderAttachGridPlane builderAttachGridPlane3 = piece2.gridPlanes[attachIndex];
						Vector3 lossyScale = builderAttachGridPlane3.transform.lossyScale;
						Vector3 vector3 = new Vector3(1f / lossyScale.x, 1f / lossyScale.y, 1f / lossyScale.z);
						Quaternion.Inverse(builderAttachGridPlane3.transform.rotation) * Vector3.Scale(vector2 - builderAttachGridPlane3.transform.position, vector3);
						Quaternion.Inverse(builderAttachGridPlane3.transform.rotation) * quaternion;
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}

		// Token: 0x0600619C RID: 24988 RVA: 0x001F2FC0 File Offset: 0x001F11C0
		public void UseResources(BuilderPiece piece)
		{
			BuilderResources cost = piece.cost;
			if (cost == null)
			{
				return;
			}
			for (int i = 0; i < cost.quantities.Count; i++)
			{
				this.UseResource(cost.quantities[i]);
			}
		}

		// Token: 0x0600619D RID: 24989 RVA: 0x001F3006 File Offset: 0x001F1206
		private void UseResource(BuilderResourceQuantity quantity)
		{
			if (quantity.type < BuilderResourceType.Basic || quantity.type >= BuilderResourceType.Count)
			{
				return;
			}
			this.usedResources[(int)quantity.type] += quantity.count;
			if (this.tableState == BuilderTable.TableState.Ready)
			{
				this.OnAvailableResourcesChange();
			}
		}

		// Token: 0x0600619E RID: 24990 RVA: 0x001F3048 File Offset: 0x001F1248
		public void AddResources(BuilderPiece piece)
		{
			BuilderResources cost = piece.cost;
			if (cost == null)
			{
				return;
			}
			for (int i = 0; i < cost.quantities.Count; i++)
			{
				this.AddResource(cost.quantities[i]);
			}
		}

		// Token: 0x0600619F RID: 24991 RVA: 0x001F308E File Offset: 0x001F128E
		private void AddResource(BuilderResourceQuantity quantity)
		{
			if (quantity.type < BuilderResourceType.Basic || quantity.type >= BuilderResourceType.Count)
			{
				return;
			}
			this.usedResources[(int)quantity.type] -= quantity.count;
			if (this.tableState == BuilderTable.TableState.Ready)
			{
				this.OnAvailableResourcesChange();
			}
		}

		// Token: 0x060061A0 RID: 24992 RVA: 0x001F30D0 File Offset: 0x001F12D0
		public bool HasEnoughUnreservedResources(BuilderResources resources)
		{
			if (resources == null)
			{
				return false;
			}
			for (int i = 0; i < resources.quantities.Count; i++)
			{
				if (!this.HasEnoughUnreservedResource(resources.quantities[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060061A1 RID: 24993 RVA: 0x001F3118 File Offset: 0x001F1318
		public bool HasEnoughUnreservedResource(BuilderResourceQuantity quantity)
		{
			return quantity.type >= BuilderResourceType.Basic && quantity.type < BuilderResourceType.Count && this.usedResources[(int)quantity.type] + this.reservedResources[(int)quantity.type] + quantity.count <= this.maxResources[(int)quantity.type];
		}

		// Token: 0x060061A2 RID: 24994 RVA: 0x001F3170 File Offset: 0x001F1370
		public bool HasEnoughResources(BuilderPiece piece)
		{
			BuilderResources cost = piece.cost;
			if (cost == null)
			{
				return false;
			}
			for (int i = 0; i < cost.quantities.Count; i++)
			{
				if (!this.HasEnoughResource(cost.quantities[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060061A3 RID: 24995 RVA: 0x001F31BC File Offset: 0x001F13BC
		public bool HasEnoughResource(BuilderResourceQuantity quantity)
		{
			return quantity.type >= BuilderResourceType.Basic && quantity.type < BuilderResourceType.Count && this.usedResources[(int)quantity.type] + quantity.count <= this.maxResources[(int)quantity.type];
		}

		// Token: 0x060061A4 RID: 24996 RVA: 0x001F31F8 File Offset: 0x001F13F8
		public int GetAvailableResources(BuilderResourceType type)
		{
			if (type < BuilderResourceType.Basic || type >= BuilderResourceType.Count)
			{
				return 0;
			}
			return this.maxResources[(int)type] - this.usedResources[(int)type];
		}

		// Token: 0x060061A5 RID: 24997 RVA: 0x001F3218 File Offset: 0x001F1418
		private void OnAvailableResourcesChange()
		{
			if (this.isSetup && this.isTableMutable)
			{
				for (int i = 0; i < this.conveyors.Count; i++)
				{
					this.conveyors[i].OnAvailableResourcesChange();
				}
				foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
				{
					builderResourceMeter.OnAvailableResourcesChange();
				}
			}
		}

		// Token: 0x060061A6 RID: 24998 RVA: 0x001F32A0 File Offset: 0x001F14A0
		public int GetPrivateResourceLimitForType(int type)
		{
			if (this.plotMaxResources == null)
			{
				return 0;
			}
			return this.plotMaxResources[type];
		}

		// Token: 0x060061A7 RID: 24999 RVA: 0x001F32B4 File Offset: 0x001F14B4
		private void WriteVector3(BinaryWriter writer, Vector3 data)
		{
			writer.Write(data.x);
			writer.Write(data.y);
			writer.Write(data.z);
		}

		// Token: 0x060061A8 RID: 25000 RVA: 0x001F32DA File Offset: 0x001F14DA
		private void WriteQuaternion(BinaryWriter writer, Quaternion data)
		{
			writer.Write(data.x);
			writer.Write(data.y);
			writer.Write(data.z);
			writer.Write(data.w);
		}

		// Token: 0x060061A9 RID: 25001 RVA: 0x001F330C File Offset: 0x001F150C
		private Vector3 ReadVector3(BinaryReader reader)
		{
			Vector3 vector;
			vector.x = reader.ReadSingle();
			vector.y = reader.ReadSingle();
			vector.z = reader.ReadSingle();
			return vector;
		}

		// Token: 0x060061AA RID: 25002 RVA: 0x001F3344 File Offset: 0x001F1544
		private Quaternion ReadQuaternion(BinaryReader reader)
		{
			Quaternion quaternion;
			quaternion.x = reader.ReadSingle();
			quaternion.y = reader.ReadSingle();
			quaternion.z = reader.ReadSingle();
			quaternion.w = reader.ReadSingle();
			return quaternion;
		}

		// Token: 0x060061AB RID: 25003 RVA: 0x001F3388 File Offset: 0x001F1588
		public static int PackPiecePlacement(byte twist, sbyte xOffset, sbyte zOffset)
		{
			int num = (int)(twist & 3);
			int num2 = (int)xOffset + 128;
			int num3 = (int)zOffset + 128;
			return num2 + (num3 << 8) + (num << 16);
		}

		// Token: 0x060061AC RID: 25004 RVA: 0x001F33B4 File Offset: 0x001F15B4
		public static void UnpackPiecePlacement(int packed, out byte twist, out sbyte xOffset, out sbyte zOffset)
		{
			int num = packed & 255;
			int num2 = (packed >> 8) & 255;
			int num3 = (packed >> 16) & 3;
			twist = (byte)num3;
			xOffset = (sbyte)(num - 128);
			zOffset = (sbyte)(num2 - 128);
		}

		// Token: 0x060061AD RID: 25005 RVA: 0x001F33F4 File Offset: 0x001F15F4
		private long PackSnapInfo(int attachGridIndex, int otherAttachGridIndex, Vector2Int min, Vector2Int max)
		{
			long num = (long)Mathf.Clamp(attachGridIndex, 0, 31);
			long num2 = (long)Mathf.Clamp(otherAttachGridIndex, 0, 31);
			long num3 = (long)Mathf.Clamp(min.x + 1024, 0, 2047);
			long num4 = (long)Mathf.Clamp(min.y + 1024, 0, 2047);
			long num5 = (long)Mathf.Clamp(max.x + 1024, 0, 2047);
			long num6 = (long)Mathf.Clamp(max.y + 1024, 0, 2047);
			return num + (num2 << 5) + (num3 << 10) + (num4 << 21) + (num5 << 32) + (num6 << 43);
		}

		// Token: 0x060061AE RID: 25006 RVA: 0x001F3498 File Offset: 0x001F1698
		private void UnpackSnapInfo(long packed, out int attachGridIndex, out int otherAttachGridIndex, out Vector2Int min, out Vector2Int max)
		{
			long num = packed & 31L;
			attachGridIndex = (int)num;
			num = (packed >> 5) & 31L;
			otherAttachGridIndex = (int)num;
			int num2 = (int)((packed >> 10) & 2047L) - 1024;
			int num3 = (int)((packed >> 21) & 2047L) - 1024;
			min = new Vector2Int(num2, num3);
			int num4 = (int)((packed >> 32) & 2047L) - 1024;
			int num5 = (int)((packed >> 43) & 2047L) - 1024;
			max = new Vector2Int(num4, num5);
		}

		// Token: 0x060061AF RID: 25007 RVA: 0x001F3525 File Offset: 0x001F1725
		private void OnTitleDataUpdate(string key)
		{
			if (key.Equals(this.SharedMapConfigTitleDataKey))
			{
				this.FetchSharedBlocksStartingMapConfig();
			}
		}

		// Token: 0x060061B0 RID: 25008 RVA: 0x001F353B File Offset: 0x001F173B
		private void FetchSharedBlocksStartingMapConfig()
		{
			if (!this.isTableMutable)
			{
				PlayFabTitleDataCache.Instance.GetTitleData(this.SharedMapConfigTitleDataKey, new Action<string>(this.OnGetStartingMapConfigSuccess), new Action<PlayFabError>(this.OnGetStartingMapConfigFail), false);
			}
		}

		// Token: 0x060061B1 RID: 25009 RVA: 0x001F3570 File Offset: 0x001F1770
		private void OnGetStartingMapConfigSuccess(string result)
		{
			this.ResetStartingMapConfig();
			if (result.IsNullOrEmpty())
			{
				return;
			}
			try
			{
				SharedBlocksManager.StartingMapConfig startingMapConfig = JsonUtility.FromJson<SharedBlocksManager.StartingMapConfig>(result);
				if (startingMapConfig.useMapID)
				{
					if (SharedBlocksManager.IsMapIDValid(startingMapConfig.mapID))
					{
						this.startingMapConfig.useMapID = true;
						this.startingMapConfig.mapID = startingMapConfig.mapID;
					}
					else
					{
						GTDev.LogError<string>(string.Format("BuilderTable {0} OnGetStartingMapConfigSuccess Title Data Default Map Config has Invalid Map ID", this.tableZone), null);
					}
				}
				else
				{
					this.startingMapConfig.pageNumber = Mathf.Max(startingMapConfig.pageNumber, 0);
					this.startingMapConfig.pageSize = Mathf.Max(startingMapConfig.pageSize, 1);
					if (!startingMapConfig.sortMethod.IsNullOrEmpty() && (startingMapConfig.sortMethod.Equals(SharedBlocksManager.MapSortMethod.Top.ToString()) || startingMapConfig.sortMethod.Equals(SharedBlocksManager.MapSortMethod.NewlyCreated.ToString()) || startingMapConfig.sortMethod.Equals(SharedBlocksManager.MapSortMethod.RecentlyUpdated.ToString())))
					{
						this.startingMapConfig.sortMethod = startingMapConfig.sortMethod;
					}
					else
					{
						GTDev.LogError<string>("BuilderTable " + this.tableZone.ToString() + " OnGetStartingMapConfigSuccess Unknown sort method " + startingMapConfig.sortMethod, null);
					}
					SharedBlocksManager.instance.RefreshPopularMapsForRandom();
				}
			}
			catch (Exception ex)
			{
				GTDev.LogError<string>("BuilderTable " + this.tableZone.ToString() + " OnGetStartingMapConfigSuccess Exception Deserializing " + ex.Message, null);
			}
		}

		// Token: 0x060061B2 RID: 25010 RVA: 0x001F3710 File Offset: 0x001F1910
		private void OnGetStartingMapConfigFail(PlayFabError error)
		{
			GTDev.LogWarning<string>("BuilderTable " + this.tableZone.ToString() + " OnGetStartingMapConfigFail " + error.Error.ToString(), null);
			this.ResetStartingMapConfig();
		}

		// Token: 0x060061B3 RID: 25011 RVA: 0x001F3750 File Offset: 0x001F1950
		private void ResetStartingMapConfig()
		{
			this.startingMapConfig = new SharedBlocksManager.StartingMapConfig
			{
				pageNumber = 0,
				pageSize = 50,
				sortMethod = SharedBlocksManager.MapSortMethod.Top.ToString(),
				useMapID = false,
				mapID = null
			};
		}

		// Token: 0x060061B4 RID: 25012 RVA: 0x001F37A3 File Offset: 0x001F19A3
		private void RequestTableConfiguration()
		{
			SharedBlocksManager.instance.OnGetTableConfiguration += this.OnGetTableConfiguration;
			SharedBlocksManager.instance.RequestTableConfiguration();
		}

		// Token: 0x060061B5 RID: 25013 RVA: 0x001F37C5 File Offset: 0x001F19C5
		private void OnGetTableConfiguration(string configString)
		{
			SharedBlocksManager.instance.OnGetTableConfiguration -= this.OnGetTableConfiguration;
			if (!configString.IsNullOrEmpty())
			{
				this.ParseTableConfiguration(configString);
			}
		}

		// Token: 0x060061B6 RID: 25014 RVA: 0x001F37EC File Offset: 0x001F19EC
		private void ParseTableConfiguration(string dataRecord)
		{
			if (string.IsNullOrEmpty(dataRecord))
			{
				return;
			}
			BuilderTableConfiguration builderTableConfiguration = JsonUtility.FromJson<BuilderTableConfiguration>(dataRecord);
			if (builderTableConfiguration != null)
			{
				if (builderTableConfiguration.TableResourceLimits != null)
				{
					for (int i = 0; i < builderTableConfiguration.TableResourceLimits.Length; i++)
					{
						int num = builderTableConfiguration.TableResourceLimits[i];
						if (num >= 0)
						{
							this.maxResources[i] = num;
						}
					}
				}
				if (builderTableConfiguration.PlotResourceLimits != null)
				{
					for (int j = 0; j < builderTableConfiguration.PlotResourceLimits.Length; j++)
					{
						int num2 = builderTableConfiguration.PlotResourceLimits[j];
						if (num2 >= 0)
						{
							this.plotMaxResources[j] = num2;
						}
					}
				}
				int droppedPieceLimit = builderTableConfiguration.DroppedPieceLimit;
				if (droppedPieceLimit >= 0)
				{
					BuilderTable.DROPPED_PIECE_LIMIT = droppedPieceLimit;
				}
				if (builderTableConfiguration.updateCountdownDate != null && !string.IsNullOrEmpty(builderTableConfiguration.updateCountdownDate))
				{
					try
					{
						DateTime.Parse(builderTableConfiguration.updateCountdownDate, CultureInfo.InvariantCulture);
						BuilderTable.nextUpdateOverride = builderTableConfiguration.updateCountdownDate;
						goto IL_00DC;
					}
					catch
					{
						BuilderTable.nextUpdateOverride = string.Empty;
						goto IL_00DC;
					}
				}
				BuilderTable.nextUpdateOverride = string.Empty;
				IL_00DC:
				this.OnAvailableResourcesChange();
				UnityEvent onTableConfigurationUpdated = this.OnTableConfigurationUpdated;
				if (onTableConfigurationUpdated == null)
				{
					return;
				}
				onTableConfigurationUpdated.Invoke();
			}
		}

		// Token: 0x060061B7 RID: 25015 RVA: 0x001F38FC File Offset: 0x001F1AFC
		private void DumpTableConfig()
		{
			BuilderTableConfiguration builderTableConfiguration = new BuilderTableConfiguration();
			Array.Clear(builderTableConfiguration.TableResourceLimits, 0, builderTableConfiguration.TableResourceLimits.Length);
			Array.Clear(builderTableConfiguration.PlotResourceLimits, 0, builderTableConfiguration.PlotResourceLimits.Length);
			foreach (BuilderResourceQuantity builderResourceQuantity in this.totalResources.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < (BuilderResourceType)builderTableConfiguration.TableResourceLimits.Length)
				{
					builderTableConfiguration.TableResourceLimits[(int)builderResourceQuantity.type] = builderResourceQuantity.count;
				}
			}
			foreach (BuilderResourceQuantity builderResourceQuantity2 in this.resourcesPerPrivatePlot.quantities)
			{
				if (builderResourceQuantity2.type >= BuilderResourceType.Basic && builderResourceQuantity2.type < (BuilderResourceType)builderTableConfiguration.PlotResourceLimits.Length)
				{
					builderTableConfiguration.PlotResourceLimits[(int)builderResourceQuantity2.type] = builderResourceQuantity2.count;
				}
			}
			builderTableConfiguration.DroppedPieceLimit = BuilderTable.DROPPED_PIECE_LIMIT;
			builderTableConfiguration.updateCountdownDate = "1/10/2025 16:00:00";
			string text = JsonUtility.ToJson(builderTableConfiguration);
			Debug.Log("Configuration Dump \n" + text);
		}

		// Token: 0x060061B8 RID: 25016 RVA: 0x001F3A48 File Offset: 0x001F1C48
		private string GetSaveDataTimeKey(int slot)
		{
			return BuilderTable.personalBuildKey + slot.ToString("D2") + "Time";
		}

		// Token: 0x060061B9 RID: 25017 RVA: 0x001F3A65 File Offset: 0x001F1C65
		private string GetSaveDataKey(int slot)
		{
			return BuilderTable.personalBuildKey + slot.ToString("D2");
		}

		// Token: 0x060061BA RID: 25018 RVA: 0x001F3A7D File Offset: 0x001F1C7D
		public void FindAndLoadSharedBlocksMap(string mapID)
		{
			SharedBlocksManager.instance.RequestMapDataFromID(mapID, new SharedBlocksManager.BlocksMapRequestCallback(this.FoundSharedBlocksMap));
		}

		// Token: 0x060061BB RID: 25019 RVA: 0x001F3A96 File Offset: 0x001F1C96
		public string GetSharedBlocksMapID()
		{
			if (this.sharedBlocksMap != null)
			{
				return this.sharedBlocksMap.MapID;
			}
			return string.Empty;
		}

		// Token: 0x060061BC RID: 25020 RVA: 0x001F3AB4 File Offset: 0x001F1CB4
		private void FoundSharedBlocksMap(SharedBlocksManager.SharedBlocksMap map)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (map == null || map.MapData.IsNullOrEmpty())
			{
				this.builderNetworking.LoadSharedBlocksFailedMaster((map == null) ? string.Empty : map.MapID);
				this.sharedBlocksMap = null;
				this.tableData = new BuilderTableData();
				this.ClearTable();
				this.ClearQueuedCommands();
				this.SetTableState(BuilderTable.TableState.Ready);
				return;
			}
			this.sharedBlocksMap = map;
			this.SetTableState(BuilderTable.TableState.WaitForInitialBuildMaster);
		}

		// Token: 0x060061BD RID: 25021 RVA: 0x001F3B30 File Offset: 0x001F1D30
		private void BuildInitialTableForPlayer()
		{
			if (NetworkSystem.Instance.IsNull() || !NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.SessionIsPrivate || NetworkSystem.Instance.GetLocalPlayer() == null || !NetworkSystem.Instance.IsMasterClient)
			{
				this.TryBuildingFromTitleData();
				return;
			}
			if (!BuilderScanKiosk.IsSaveSlotValid(this.currentSaveSlot))
			{
				this.TryBuildingFromTitleData();
				return;
			}
			SharedBlocksManager.instance.OnFetchPrivateScanComplete += this.OnFetchPrivateScanComplete;
			SharedBlocksManager.instance.RequestFetchPrivateScan(this.currentSaveSlot);
		}

		// Token: 0x060061BE RID: 25022 RVA: 0x001F3BBC File Offset: 0x001F1DBC
		private void OnFetchPrivateScanComplete(int slot, bool success)
		{
			SharedBlocksManager.instance.OnFetchPrivateScanComplete -= this.OnFetchPrivateScanComplete;
			if (this.tableState != BuilderTable.TableState.WaitForInitialBuildMaster)
			{
				return;
			}
			string text;
			if (!success || !SharedBlocksManager.instance.TryGetPrivateScanResponse(slot, out text))
			{
				this.TryBuildingFromTitleData();
				return;
			}
			if (!this.BuildTableFromJson(text, false))
			{
				this.TryBuildingFromTitleData();
				return;
			}
			this.SetIsDirty(false);
			this.OnFinishedInitialTableBuild();
		}

		// Token: 0x060061BF RID: 25023 RVA: 0x001F3C20 File Offset: 0x001F1E20
		private void BuildSelectedSharedMap()
		{
			if (!NetworkSystem.Instance.IsNull() && NetworkSystem.Instance.InRoom && NetworkSystem.Instance.IsMasterClient)
			{
				if (this.sharedBlocksMap != null && !this.sharedBlocksMap.MapData.IsNullOrEmpty())
				{
					this.TryBuildingSharedBlocksMap(this.sharedBlocksMap.MapData);
					return;
				}
				if (SharedBlocksManager.IsMapIDValid(this.pendingMapID))
				{
					SharedBlocksManager.SharedBlocksMap sharedBlocksMap = new SharedBlocksManager.SharedBlocksMap
					{
						MapID = this.pendingMapID
					};
					this.LoadSharedMap(sharedBlocksMap);
					return;
				}
				this.FindStartingMap();
			}
		}

		// Token: 0x060061C0 RID: 25024 RVA: 0x001F3CAC File Offset: 0x001F1EAC
		private void FindStartingMap()
		{
			if (this.hasStartingMap && Time.timeAsDouble < this.startingMapCacheTime + 60.0)
			{
				this.FoundDefaultSharedBlocksMap(true, this.startingMap);
				return;
			}
			if (this.getStartingMapInProgress)
			{
				return;
			}
			this.hasStartingMap = false;
			this.getStartingMapInProgress = true;
			if (this.startingMapConfig.useMapID && SharedBlocksManager.IsMapIDValid(this.startingMapConfig.mapID))
			{
				this.startingMap = new SharedBlocksManager.SharedBlocksMap
				{
					MapID = this.startingMapConfig.mapID
				};
				SharedBlocksManager.instance.RequestMapDataFromID(this.startingMapConfig.mapID, new SharedBlocksManager.BlocksMapRequestCallback(this.FoundTopMapData));
				return;
			}
			if (this.hasCachedTopMaps && Time.timeAsDouble <= this.lastGetTopMapsTime + 60.0)
			{
				this.ChooseMapFromList();
				return;
			}
			SharedBlocksManager.instance.OnGetPopularMapsComplete += this.FoundStartingMapList;
			if (!SharedBlocksManager.instance.RequestGetTopMaps(this.startingMapConfig.pageNumber, this.startingMapConfig.pageSize, this.startingMapConfig.sortMethod.ToString()))
			{
				this.FoundStartingMapList(false);
			}
		}

		// Token: 0x060061C1 RID: 25025 RVA: 0x001F3DD0 File Offset: 0x001F1FD0
		private void FoundStartingMapList(bool success)
		{
			SharedBlocksManager.instance.OnGetPopularMapsComplete -= this.FoundStartingMapList;
			if (success && SharedBlocksManager.instance.LatestPopularMaps.Count > 0)
			{
				this.startingMapList.Clear();
				this.startingMapList.AddRange(SharedBlocksManager.instance.LatestPopularMaps);
				this.hasCachedTopMaps = this.startingMapList.Count > 0;
				this.lastGetTopMapsTime = (double)Time.time;
				this.ChooseMapFromList();
				return;
			}
			this.FoundDefaultSharedBlocksMap(false, null);
		}

		// Token: 0x060061C2 RID: 25026 RVA: 0x001F3E58 File Offset: 0x001F2058
		private void ChooseMapFromList()
		{
			int num = Random.Range(0, this.startingMapList.Count);
			this.startingMap = this.startingMapList[num];
			if (this.startingMap == null || !SharedBlocksManager.IsMapIDValid(this.startingMap.MapID))
			{
				this.FoundDefaultSharedBlocksMap(false, null);
				return;
			}
			SharedBlocksManager.instance.RequestMapDataFromID(this.startingMap.MapID, new SharedBlocksManager.BlocksMapRequestCallback(this.FoundTopMapData));
		}

		// Token: 0x060061C3 RID: 25027 RVA: 0x001F3ED0 File Offset: 0x001F20D0
		private void FoundTopMapData(SharedBlocksManager.SharedBlocksMap map)
		{
			if (map == null || !SharedBlocksManager.IsMapIDValid(map.MapID) || map.MapID != this.startingMap.MapID)
			{
				this.FoundDefaultSharedBlocksMap(false, null);
				return;
			}
			this.hasStartingMap = true;
			this.startingMapCacheTime = Time.timeAsDouble;
			this.startingMap.MapData = map.MapData;
			this.FoundDefaultSharedBlocksMap(true, this.startingMap);
		}

		// Token: 0x060061C4 RID: 25028 RVA: 0x001F3F40 File Offset: 0x001F2140
		private void FoundDefaultSharedBlocksMap(bool success, SharedBlocksManager.SharedBlocksMap map)
		{
			this.getStartingMapInProgress = false;
			if (success && !map.MapData.IsNullOrEmpty())
			{
				this.startingMapCacheTime = Time.timeAsDouble;
				this.startingMap = map;
				this.hasStartingMap = true;
				this.sharedBlocksMap = map;
				this.TryBuildingSharedBlocksMap(this.sharedBlocksMap.MapData);
				return;
			}
			this.TryBuildingFromTitleData();
		}

		// Token: 0x060061C5 RID: 25029 RVA: 0x001F3F9C File Offset: 0x001F219C
		private void TryBuildingSharedBlocksMap(string mapData)
		{
			if (this.tableState != BuilderTable.TableState.WaitForInitialBuildMaster)
			{
				return;
			}
			if (!this.BuildTableFromJson(mapData, true))
			{
				GTDev.LogWarning<string>("Unable to build shared blocks map", null);
				this.builderNetworking.LoadSharedBlocksFailedMaster(this.sharedBlocksMap.MapID);
				this.sharedBlocksMap = null;
				this.tableData = new BuilderTableData();
				this.ClearTable();
				this.ClearQueuedCommands();
				this.SetTableState(BuilderTable.TableState.Ready);
				return;
			}
			base.StartCoroutine(this.CheckForNoBlocks());
		}

		// Token: 0x060061C6 RID: 25030 RVA: 0x001F4011 File Offset: 0x001F2211
		private IEnumerator CheckForNoBlocks()
		{
			yield return null;
			if (!this.NoBlocksCheck())
			{
				GTDev.LogError<string>("Failed No Blocks Check", null);
				this.builderNetworking.SharedBlocksOutOfBoundsMaster(this.sharedBlocksMap.MapID);
				this.sharedBlocksMap = null;
				this.tableData = new BuilderTableData();
				this.ClearTable();
				this.ClearQueuedCommands();
				this.SetTableState(BuilderTable.TableState.Ready);
				yield break;
			}
			this.OnFinishedInitialTableBuild();
			yield break;
		}

		// Token: 0x060061C7 RID: 25031 RVA: 0x001F4020 File Offset: 0x001F2220
		private void TryBuildingFromTitleData()
		{
			SharedBlocksManager.instance.OnGetTitleDataBuildComplete += this.OnGetTitleDataBuildComplete;
			SharedBlocksManager.instance.FetchTitleDataBuild();
		}

		// Token: 0x060061C8 RID: 25032 RVA: 0x001F4044 File Offset: 0x001F2244
		private void OnGetTitleDataBuildComplete(string titleDataBuild)
		{
			SharedBlocksManager.instance.OnGetTitleDataBuildComplete -= this.OnGetTitleDataBuildComplete;
			if (this.tableState != BuilderTable.TableState.WaitForInitialBuildMaster)
			{
				return;
			}
			if (!titleDataBuild.IsNullOrEmpty())
			{
				if (!this.BuildTableFromJson(titleDataBuild, true))
				{
					this.tableData = new BuilderTableData();
				}
			}
			else
			{
				this.tableData = new BuilderTableData();
			}
			this.OnFinishedInitialTableBuild();
		}

		// Token: 0x060061C9 RID: 25033 RVA: 0x001F40A4 File Offset: 0x001F22A4
		public void SaveTableForPlayer(string busyStr, string blocksErrStr)
		{
			if (SharedBlocksManager.instance.IsWaitingOnRequest())
			{
				this.SetIsDirty(true);
				UnityEvent<string> onSaveFailure = this.OnSaveFailure;
				if (onSaveFailure == null)
				{
					return;
				}
				onSaveFailure.Invoke(busyStr);
				return;
			}
			else
			{
				this.saveInProgress = true;
				if (!BuilderScanKiosk.IsSaveSlotValid(this.currentSaveSlot))
				{
					this.saveInProgress = false;
					return;
				}
				if (!this.isDirty)
				{
					this.saveInProgress = false;
					UnityEvent onSaveTimeUpdated = this.OnSaveTimeUpdated;
					if (onSaveTimeUpdated == null)
					{
						return;
					}
					onSaveTimeUpdated.Invoke();
					return;
				}
				else
				{
					if (this.NoBlocksCheck())
					{
						if (this.tableData == null)
						{
							this.tableData = new BuilderTableData();
						}
						this.SetIsDirty(false);
						this.tableData.numEdits++;
						string text = this.WriteTableToJson();
						text = Convert.ToBase64String(GZipStream.CompressString(text));
						SharedBlocksManager.instance.OnSavePrivateScanSuccess += this.OnSaveScanSuccess;
						SharedBlocksManager.instance.OnSavePrivateScanFailed += this.OnSaveScanFailure;
						SharedBlocksManager.instance.RequestSavePrivateScan(this.currentSaveSlot, text);
						return;
					}
					this.saveInProgress = false;
					this.SetIsDirty(true);
					UnityEvent<string> onSaveFailure2 = this.OnSaveFailure;
					if (onSaveFailure2 == null)
					{
						return;
					}
					onSaveFailure2.Invoke(blocksErrStr);
					return;
				}
			}
		}

		// Token: 0x060061CA RID: 25034 RVA: 0x001F41B8 File Offset: 0x001F23B8
		private void OnSaveScanSuccess(int scan)
		{
			SharedBlocksManager.instance.OnSavePrivateScanSuccess -= this.OnSaveScanSuccess;
			SharedBlocksManager.instance.OnSavePrivateScanFailed -= this.OnSaveScanFailure;
			this.saveInProgress = false;
			UnityEvent onSaveSuccess = this.OnSaveSuccess;
			if (onSaveSuccess == null)
			{
				return;
			}
			onSaveSuccess.Invoke();
		}

		// Token: 0x060061CB RID: 25035 RVA: 0x001F4208 File Offset: 0x001F2408
		private void OnSaveScanFailure(int scan, string message)
		{
			SharedBlocksManager.instance.OnSavePrivateScanSuccess -= this.OnSaveScanSuccess;
			SharedBlocksManager.instance.OnSavePrivateScanFailed -= this.OnSaveScanFailure;
			this.saveInProgress = false;
			this.SetIsDirty(true);
			UnityEvent<string> onSaveFailure = this.OnSaveFailure;
			if (onSaveFailure == null)
			{
				return;
			}
			onSaveFailure.Invoke(message);
		}

		// Token: 0x060061CC RID: 25036 RVA: 0x001F4260 File Offset: 0x001F2460
		private string WriteTableToJson()
		{
			this.tableData.Clear();
			BuilderTable.tempDuplicateOverlaps.Clear();
			for (int i = 0; i < this.pieces.Count; i++)
			{
				if (this.pieces[i].state == BuilderPiece.State.AttachedAndPlaced)
				{
					this.tableData.pieceType.Add(this.pieces[i].overrideSavedPiece ? this.pieces[i].savedPieceType : this.pieces[i].pieceType);
					this.tableData.pieceId.Add(this.pieces[i].pieceId);
					this.tableData.parentId.Add((this.pieces[i].parentPiece == null) ? (-1) : this.pieces[i].parentPiece.pieceId);
					this.tableData.attachIndex.Add(this.pieces[i].attachIndex);
					this.tableData.parentAttachIndex.Add((this.pieces[i].parentPiece == null) ? (-1) : this.pieces[i].parentAttachIndex);
					this.tableData.placement.Add(this.pieces[i].GetPiecePlacement());
					this.tableData.materialType.Add(this.pieces[i].overrideSavedPiece ? this.pieces[i].savedMaterialType : this.pieces[i].materialType);
					BuilderMovingSnapPiece component = this.pieces[i].GetComponent<BuilderMovingSnapPiece>();
					int num = ((component == null) ? 0 : component.GetTimeOffset());
					this.tableData.timeOffset.Add(num);
					for (int j = 0; j < this.pieces[i].gridPlanes.Count; j++)
					{
						if (!(this.pieces[i].gridPlanes[j] == null))
						{
							for (SnapOverlap snapOverlap = this.pieces[i].gridPlanes[j].firstOverlap; snapOverlap != null; snapOverlap = snapOverlap.nextOverlap)
							{
								if (snapOverlap.otherPlane.piece.state == BuilderPiece.State.AttachedAndPlaced || snapOverlap.otherPlane.piece.isBuiltIntoTable)
								{
									BuilderTable.SnapOverlapKey snapOverlapKey = BuilderTable.BuildOverlapKey(this.pieces[i].pieceId, snapOverlap.otherPlane.piece.pieceId, j, snapOverlap.otherPlane.attachIndex);
									if (!BuilderTable.tempDuplicateOverlaps.Contains(snapOverlapKey))
									{
										BuilderTable.tempDuplicateOverlaps.Add(snapOverlapKey);
										long num2 = this.PackSnapInfo(j, snapOverlap.otherPlane.attachIndex, snapOverlap.bounds.min, snapOverlap.bounds.max);
										this.tableData.overlapingPieces.Add(this.pieces[i].pieceId);
										this.tableData.overlappedPieces.Add(snapOverlap.otherPlane.piece.pieceId);
										this.tableData.overlapInfo.Add(num2);
									}
								}
							}
						}
					}
				}
			}
			foreach (BuilderPiece builderPiece in this.basePieces)
			{
				if (!(builderPiece == null))
				{
					for (int k = 0; k < builderPiece.gridPlanes.Count; k++)
					{
						if (!(builderPiece.gridPlanes[k] == null))
						{
							for (SnapOverlap snapOverlap2 = builderPiece.gridPlanes[k].firstOverlap; snapOverlap2 != null; snapOverlap2 = snapOverlap2.nextOverlap)
							{
								if (snapOverlap2.otherPlane.piece.state == BuilderPiece.State.AttachedAndPlaced || snapOverlap2.otherPlane.piece.isBuiltIntoTable)
								{
									BuilderTable.SnapOverlapKey snapOverlapKey2 = BuilderTable.BuildOverlapKey(builderPiece.pieceId, snapOverlap2.otherPlane.piece.pieceId, k, snapOverlap2.otherPlane.attachIndex);
									if (!BuilderTable.tempDuplicateOverlaps.Contains(snapOverlapKey2))
									{
										BuilderTable.tempDuplicateOverlaps.Add(snapOverlapKey2);
										long num3 = this.PackSnapInfo(k, snapOverlap2.otherPlane.attachIndex, snapOverlap2.bounds.min, snapOverlap2.bounds.max);
										this.tableData.overlapingPieces.Add(builderPiece.pieceId);
										this.tableData.overlappedPieces.Add(snapOverlap2.otherPlane.piece.pieceId);
										this.tableData.overlapInfo.Add(num3);
									}
								}
							}
						}
					}
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
			this.tableData.numPieces = this.tableData.pieceType.Count;
			return JsonUtility.ToJson(this.tableData);
		}

		// Token: 0x060061CD RID: 25037 RVA: 0x001F47B4 File Offset: 0x001F29B4
		private static BuilderTable.SnapOverlapKey BuildOverlapKey(int pieceId, int otherPieceId, int attachGridIndex, int otherAttachGridIndex)
		{
			BuilderTable.SnapOverlapKey snapOverlapKey = default(BuilderTable.SnapOverlapKey);
			snapOverlapKey.piece = (long)pieceId;
			snapOverlapKey.piece <<= 32;
			snapOverlapKey.piece |= (long)attachGridIndex;
			snapOverlapKey.otherPiece = (long)otherPieceId;
			snapOverlapKey.otherPiece <<= 32;
			snapOverlapKey.otherPiece |= (long)otherAttachGridIndex;
			return snapOverlapKey;
		}

		// Token: 0x060061CE RID: 25038 RVA: 0x001F4810 File Offset: 0x001F2A10
		private bool BuildTableFromJson(string tableJson, bool fromTitleData)
		{
			if (string.IsNullOrEmpty(tableJson))
			{
				return false;
			}
			this.tableData = null;
			try
			{
				this.tableData = JsonUtility.FromJson<BuilderTableData>(tableJson);
			}
			catch
			{
			}
			try
			{
				if (this.tableData == null)
				{
					tableJson = GZipStream.UncompressString(Convert.FromBase64String(tableJson));
					this.tableData = JsonUtility.FromJson<BuilderTableData>(tableJson);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
				return false;
			}
			if (this.tableData == null)
			{
				return false;
			}
			if (this.tableData.version < 4)
			{
				return false;
			}
			int num = ((this.tableData.pieceType == null) ? 0 : this.tableData.pieceType.Count);
			if (num == 0)
			{
				this.OnDeserializeUpdatePlots();
				return true;
			}
			if (this.tableData.pieceId == null || this.tableData.pieceId.Count != num || this.tableData.placement == null || this.tableData.placement.Count != num)
			{
				GTDev.LogError<string>("BuildTableFromJson Piece Count Mismatch", null);
				return false;
			}
			if (num >= this.maxResources[0])
			{
				GTDev.LogError<string>(string.Format("BuildTableFromJson Failed sanity piece count check {0}", num), null);
				return false;
			}
			Dictionary<int, int> dictionary = new Dictionary<int, int>(num);
			bool flag = this.tableData.timeOffset != null && this.tableData.timeOffset.Count > 0;
			if (flag && this.tableData.timeOffset.Count != num)
			{
				GTDev.LogError<string>("BuildTableFromJson Piece Count Mismatch (Time Offsets)", null);
				return false;
			}
			int i = 0;
			while (i < this.tableData.pieceType.Count)
			{
				int num2 = this.CreatePieceId();
				if (!dictionary.TryAdd(this.tableData.pieceId[i], num2))
				{
					GTDev.LogError<string>("BuildTableFromJson Piece id duplicate in save", null);
					this.ClearTable();
					return false;
				}
				int num3 = ((this.tableData.materialType != null && this.tableData.materialType.Count > i) ? this.tableData.materialType[i] : (-1));
				int num4 = this.tableData.pieceType[i];
				int num5 = num3;
				bool flag2 = true;
				BuilderPiece piecePrefab = this.GetPiecePrefab(this.tableData.pieceType[i]);
				if (piecePrefab == null)
				{
					this.ClearTable();
					return false;
				}
				if (fromTitleData)
				{
					goto IL_02B2;
				}
				if (num5 == -1 && piecePrefab.materialOptions != null)
				{
					int num6;
					Material material;
					int num7;
					piecePrefab.materialOptions.GetDefaultMaterial(out num6, out material, out num7);
					num5 = num6;
				}
				flag2 = BuilderSetManager.instance.IsPieceOwnedLocally(this.tableData.pieceType[i], num5);
				if (!fromTitleData && !flag2)
				{
					if (!piecePrefab.fallbackInfo.materialSwapThisPrefab)
					{
						if (piecePrefab.fallbackInfo.prefab == null)
						{
							goto IL_03E0;
						}
						num4 = piecePrefab.fallbackInfo.prefab.name.GetStaticHash();
					}
					num5 = -1;
				}
				goto IL_02B2;
				IL_03E0:
				i++;
				continue;
				IL_02B2:
				if (piecePrefab.cost != null && piecePrefab.cost.quantities != null)
				{
					for (int j = 0; j < piecePrefab.cost.quantities.Count; j++)
					{
						BuilderResourceQuantity builderResourceQuantity = piecePrefab.cost.quantities[j];
						if (!this.HasEnoughResource(builderResourceQuantity))
						{
							if (builderResourceQuantity.type == BuilderResourceType.Basic)
							{
								this.ClearTable();
								GTDev.LogError<string>("BuildTableFromJson saved table uses too many basic resource", null);
								return false;
							}
							GTDev.LogWarning<string>("BuildTableFromJson saved table uses too many functional or decorative resource", null);
						}
					}
				}
				int num8 = (flag ? this.tableData.timeOffset[i] : 0);
				BuilderPiece builderPiece = this.CreatePieceInternal(num4, num2, Vector3.zero, Quaternion.identity, BuilderPiece.State.AttachedAndPlaced, num5, NetworkSystem.Instance.ServerTimestamp - num8, this);
				if (builderPiece == null)
				{
					this.ClearTable();
					GTDev.LogError<string>(string.Format("Piece Type {0} is not defined", this.tableData.pieceType[i]), null);
					return false;
				}
				if (!fromTitleData && !flag2)
				{
					builderPiece.overrideSavedPiece = true;
					builderPiece.savedPieceType = this.tableData.pieceType[i];
					builderPiece.savedMaterialType = num3;
				}
				goto IL_03E0;
			}
			for (int k = 0; k < this.tableData.pieceType.Count; k++)
			{
				int num9 = ((this.tableData.parentAttachIndex == null || this.tableData.parentAttachIndex.Count <= k) ? (-1) : this.tableData.parentAttachIndex[k]);
				int num10 = ((this.tableData.attachIndex == null || this.tableData.attachIndex.Count <= k) ? (-1) : this.tableData.attachIndex[k]);
				int valueOrDefault = dictionary.GetValueOrDefault(this.tableData.pieceId[k], -1);
				int num11 = -1;
				int num12;
				if (dictionary.TryGetValue(this.tableData.parentId[k], out num12))
				{
					num11 = num12;
				}
				else if (this.tableData.parentId[k] < 10000 && this.tableData.parentId[k] >= 5)
				{
					num11 = this.tableData.parentId[k];
				}
				this.AttachPieceInternal(valueOrDefault, num10, num11, num9, this.tableData.placement[k]);
			}
			foreach (BuilderPiece builderPiece2 in this.pieces)
			{
				if (builderPiece2.state == BuilderPiece.State.AttachedAndPlaced)
				{
					builderPiece2.OnPlacementDeserialized();
				}
			}
			this.OnDeserializeUpdatePlots();
			BuilderTable.tempDuplicateOverlaps.Clear();
			if (this.tableData.overlapingPieces != null)
			{
				int num13 = 0;
				while (num13 < this.tableData.overlapingPieces.Count && num13 < this.tableData.overlappedPieces.Count && num13 < this.tableData.overlapInfo.Count)
				{
					int num14 = -1;
					int num15;
					if (dictionary.TryGetValue(this.tableData.overlapingPieces[num13], out num15))
					{
						num14 = num15;
					}
					else if (this.tableData.overlapingPieces[num13] < 10000 && this.tableData.overlapingPieces[num13] >= 5)
					{
						num14 = this.tableData.overlapingPieces[num13];
					}
					int num16 = -1;
					int num17;
					if (dictionary.TryGetValue(this.tableData.overlappedPieces[num13], out num17))
					{
						num16 = num17;
					}
					else if (this.tableData.overlappedPieces[num13] < 10000 && this.tableData.overlappedPieces[num13] >= 5)
					{
						num16 = this.tableData.overlappedPieces[num13];
					}
					if (num14 != -1 && num16 != -1)
					{
						long num18 = this.tableData.overlapInfo[num13];
						BuilderPiece piece = this.GetPiece(num14);
						if (!(piece == null))
						{
							BuilderPiece piece2 = this.GetPiece(num16);
							if (!(piece2 == null))
							{
								int num19;
								int num20;
								Vector2Int vector2Int;
								Vector2Int vector2Int2;
								this.UnpackSnapInfo(num18, out num19, out num20, out vector2Int, out vector2Int2);
								if (num19 >= 0 && num19 < piece.gridPlanes.Count && num20 >= 0 && num20 < piece2.gridPlanes.Count)
								{
									BuilderTable.SnapOverlapKey snapOverlapKey = BuilderTable.BuildOverlapKey(num14, num16, num19, num20);
									if (!BuilderTable.tempDuplicateOverlaps.Contains(snapOverlapKey))
									{
										BuilderTable.tempDuplicateOverlaps.Add(snapOverlapKey);
										piece.gridPlanes[num19].AddSnapOverlap(this.builderPool.CreateSnapOverlap(piece2.gridPlanes[num20], new SnapBounds(vector2Int, vector2Int2)));
									}
								}
							}
						}
					}
					num13++;
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
			return true;
		}

		// Token: 0x060061CF RID: 25039 RVA: 0x001F4FF8 File Offset: 0x001F31F8
		public int SerializeTableState(byte[] bytes, int maxBytes)
		{
			MemoryStream memoryStream = new MemoryStream(bytes);
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			if (this.conveyors == null)
			{
				binaryWriter.Write(0);
			}
			else
			{
				binaryWriter.Write(this.conveyors.Count);
				foreach (BuilderConveyor builderConveyor in this.conveyors)
				{
					int selectedDisplayGroupID = builderConveyor.GetSelectedDisplayGroupID();
					binaryWriter.Write(selectedDisplayGroupID);
				}
			}
			if (this.dispenserShelves == null)
			{
				binaryWriter.Write(0);
			}
			else
			{
				binaryWriter.Write(this.dispenserShelves.Count);
				foreach (BuilderDispenserShelf builderDispenserShelf in this.dispenserShelves)
				{
					int selectedDisplayGroupID2 = builderDispenserShelf.GetSelectedDisplayGroupID();
					binaryWriter.Write(selectedDisplayGroupID2);
				}
			}
			BuilderTable.childPieces.Clear();
			BuilderTable.rootPieces.Clear();
			BuilderTable.childPieces.EnsureCapacity(this.pieces.Count);
			BuilderTable.rootPieces.EnsureCapacity(this.pieces.Count);
			foreach (BuilderPiece builderPiece in this.pieces)
			{
				if (builderPiece.parentPiece == null)
				{
					BuilderTable.rootPieces.Add(builderPiece);
				}
				else
				{
					BuilderTable.childPieces.Add(builderPiece);
				}
			}
			binaryWriter.Write(BuilderTable.rootPieces.Count);
			for (int i = 0; i < BuilderTable.rootPieces.Count; i++)
			{
				BuilderPiece builderPiece2 = BuilderTable.rootPieces[i];
				binaryWriter.Write(builderPiece2.pieceType);
				binaryWriter.Write(builderPiece2.pieceId);
				binaryWriter.Write((byte)builderPiece2.state);
				if (builderPiece2.state == BuilderPiece.State.OnConveyor || builderPiece2.state == BuilderPiece.State.OnShelf || builderPiece2.state == BuilderPiece.State.Displayed)
				{
					binaryWriter.Write(builderPiece2.shelfOwner);
				}
				else
				{
					binaryWriter.Write(builderPiece2.heldByPlayerActorNumber);
				}
				binaryWriter.Write(builderPiece2.heldInLeftHand ? 1 : 0);
				binaryWriter.Write(builderPiece2.materialType);
				long num = BitPackUtils.PackWorldPosForNetwork(builderPiece2.transform.localPosition);
				int num2 = BitPackUtils.PackQuaternionForNetwork(builderPiece2.transform.localRotation);
				binaryWriter.Write(num);
				binaryWriter.Write(num2);
				if (builderPiece2.state == BuilderPiece.State.AttachedAndPlaced)
				{
					binaryWriter.Write(builderPiece2.functionalPieceState);
					binaryWriter.Write(builderPiece2.activatedTimeStamp);
				}
				if (builderPiece2.state == BuilderPiece.State.OnConveyor)
				{
					binaryWriter.Write((this.conveyorManager == null) ? 0 : this.conveyorManager.GetPieceCreateTimestamp(builderPiece2));
				}
			}
			binaryWriter.Write(BuilderTable.childPieces.Count);
			for (int j = 0; j < BuilderTable.childPieces.Count; j++)
			{
				BuilderPiece builderPiece3 = BuilderTable.childPieces[j];
				binaryWriter.Write(builderPiece3.pieceType);
				binaryWriter.Write(builderPiece3.pieceId);
				int num3 = ((builderPiece3.parentPiece == null) ? (-1) : builderPiece3.parentPiece.pieceId);
				binaryWriter.Write(num3);
				binaryWriter.Write(builderPiece3.attachIndex);
				binaryWriter.Write(builderPiece3.parentAttachIndex);
				binaryWriter.Write((byte)builderPiece3.state);
				if (builderPiece3.state == BuilderPiece.State.OnConveyor || builderPiece3.state == BuilderPiece.State.OnShelf || builderPiece3.state == BuilderPiece.State.Displayed)
				{
					binaryWriter.Write(builderPiece3.shelfOwner);
				}
				else
				{
					binaryWriter.Write(builderPiece3.heldByPlayerActorNumber);
				}
				binaryWriter.Write(builderPiece3.heldInLeftHand ? 1 : 0);
				binaryWriter.Write(builderPiece3.materialType);
				int piecePlacement = builderPiece3.GetPiecePlacement();
				binaryWriter.Write(piecePlacement);
				if (builderPiece3.state == BuilderPiece.State.AttachedAndPlaced)
				{
					binaryWriter.Write(builderPiece3.functionalPieceState);
					binaryWriter.Write(builderPiece3.activatedTimeStamp);
				}
				if (builderPiece3.state == BuilderPiece.State.OnConveyor)
				{
					binaryWriter.Write((this.conveyorManager == null) ? 0 : this.conveyorManager.GetPieceCreateTimestamp(builderPiece3));
				}
			}
			if (this.isTableMutable)
			{
				binaryWriter.Write(this.plotOwners.Count);
				using (Dictionary<int, int>.Enumerator enumerator4 = this.plotOwners.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						KeyValuePair<int, int> keyValuePair = enumerator4.Current;
						binaryWriter.Write(keyValuePair.Key);
						binaryWriter.Write(keyValuePair.Value);
					}
					goto IL_04F9;
				}
			}
			if (this.sharedBlocksMap == null || this.sharedBlocksMap.MapID == null || !SharedBlocksManager.IsMapIDValid(this.sharedBlocksMap.MapID))
			{
				for (int k = 0; k < BuilderTable.mapIDBuffer.Length; k++)
				{
					BuilderTable.mapIDBuffer[k] = 'a';
				}
			}
			else
			{
				for (int l = 0; l < BuilderTable.mapIDBuffer.Length; l++)
				{
					BuilderTable.mapIDBuffer[l] = this.sharedBlocksMap.MapID[l];
				}
			}
			binaryWriter.Write(BuilderTable.mapIDBuffer);
			IL_04F9:
			long position = memoryStream.Position;
			BuilderTable.overlapPieces.Clear();
			BuilderTable.overlapOtherPieces.Clear();
			BuilderTable.overlapPacked.Clear();
			BuilderTable.tempDuplicateOverlaps.Clear();
			foreach (BuilderPiece builderPiece4 in this.pieces)
			{
				if (!(builderPiece4 == null))
				{
					for (int m = 0; m < builderPiece4.gridPlanes.Count; m++)
					{
						if (!(builderPiece4.gridPlanes[m] == null))
						{
							for (SnapOverlap snapOverlap = builderPiece4.gridPlanes[m].firstOverlap; snapOverlap != null; snapOverlap = snapOverlap.nextOverlap)
							{
								BuilderTable.SnapOverlapKey snapOverlapKey = BuilderTable.BuildOverlapKey(builderPiece4.pieceId, snapOverlap.otherPlane.piece.pieceId, m, snapOverlap.otherPlane.attachIndex);
								if (!BuilderTable.tempDuplicateOverlaps.Contains(snapOverlapKey))
								{
									BuilderTable.tempDuplicateOverlaps.Add(snapOverlapKey);
									long num4 = this.PackSnapInfo(m, snapOverlap.otherPlane.attachIndex, snapOverlap.bounds.min, snapOverlap.bounds.max);
									BuilderTable.overlapPieces.Add(builderPiece4.pieceId);
									BuilderTable.overlapOtherPieces.Add(snapOverlap.otherPlane.piece.pieceId);
									BuilderTable.overlapPacked.Add(num4);
								}
							}
						}
					}
				}
			}
			foreach (BuilderPiece builderPiece5 in this.basePieces)
			{
				if (!(builderPiece5 == null))
				{
					for (int n = 0; n < builderPiece5.gridPlanes.Count; n++)
					{
						if (!(builderPiece5.gridPlanes[n] == null))
						{
							for (SnapOverlap snapOverlap2 = builderPiece5.gridPlanes[n].firstOverlap; snapOverlap2 != null; snapOverlap2 = snapOverlap2.nextOverlap)
							{
								BuilderTable.SnapOverlapKey snapOverlapKey2 = BuilderTable.BuildOverlapKey(builderPiece5.pieceId, snapOverlap2.otherPlane.piece.pieceId, n, snapOverlap2.otherPlane.attachIndex);
								if (!BuilderTable.tempDuplicateOverlaps.Contains(snapOverlapKey2))
								{
									BuilderTable.tempDuplicateOverlaps.Add(snapOverlapKey2);
									long num5 = this.PackSnapInfo(n, snapOverlap2.otherPlane.attachIndex, snapOverlap2.bounds.min, snapOverlap2.bounds.max);
									BuilderTable.overlapPieces.Add(builderPiece5.pieceId);
									BuilderTable.overlapOtherPieces.Add(snapOverlap2.otherPlane.piece.pieceId);
									BuilderTable.overlapPacked.Add(num5);
								}
							}
						}
					}
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
			binaryWriter.Write(BuilderTable.overlapPieces.Count);
			for (int num6 = 0; num6 < BuilderTable.overlapPieces.Count; num6++)
			{
				binaryWriter.Write(BuilderTable.overlapPieces[num6]);
				binaryWriter.Write(BuilderTable.overlapOtherPieces[num6]);
				binaryWriter.Write(BuilderTable.overlapPacked[num6]);
			}
			return (int)memoryStream.Position;
		}

		// Token: 0x060061D0 RID: 25040 RVA: 0x001F58E0 File Offset: 0x001F3AE0
		public void DeserializeTableState(byte[] bytes, int numBytes)
		{
			if (numBytes <= 0)
			{
				return;
			}
			Vector3 vector;
			Quaternion quaternion;
			VRRigCache.Instance.localRig.SpeakerHead.transform.GetPositionAndRotation(out vector, out quaternion);
			bool flag = this.ValidatePieceWorldTransform(vector, quaternion);
			int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			BinaryReader binaryReader = new BinaryReader(new MemoryStream(bytes));
			BuilderTable.tempPeiceIds.Clear();
			BuilderTable.tempParentPeiceIds.Clear();
			BuilderTable.tempAttachIndexes.Clear();
			BuilderTable.tempParentAttachIndexes.Clear();
			BuilderTable.tempParentActorNumbers.Clear();
			BuilderTable.tempInLeftHand.Clear();
			BuilderTable.tempPiecePlacement.Clear();
			int num = binaryReader.ReadInt32();
			bool flag2 = this.conveyors != null;
			for (int i = 0; i < num; i++)
			{
				int num2 = binaryReader.ReadInt32();
				if (flag2 && i < this.conveyors.Count)
				{
					this.conveyors[i].SetSelection(num2);
				}
			}
			int num3 = binaryReader.ReadInt32();
			bool flag3 = this.dispenserShelves != null;
			for (int j = 0; j < num3; j++)
			{
				int num4 = binaryReader.ReadInt32();
				if (flag3 && j < this.dispenserShelves.Count)
				{
					this.dispenserShelves[j].SetSelection(num4);
				}
			}
			int num5 = binaryReader.ReadInt32();
			for (int k = 0; k < num5; k++)
			{
				int num6 = binaryReader.ReadInt32();
				int num7 = binaryReader.ReadInt32();
				BuilderPiece.State state = (BuilderPiece.State)binaryReader.ReadByte();
				int num8 = binaryReader.ReadInt32();
				bool flag4 = binaryReader.ReadByte() > 0;
				int num9 = binaryReader.ReadInt32();
				long num10 = binaryReader.ReadInt64();
				int num11 = binaryReader.ReadInt32();
				Vector3 vector2 = BitPackUtils.UnpackWorldPosFromNetwork(num10);
				Quaternion quaternion2 = BitPackUtils.UnpackQuaternionFromNetwork(num11);
				byte b = ((state == BuilderPiece.State.AttachedAndPlaced) ? binaryReader.ReadByte() : 0);
				int num12 = ((state == BuilderPiece.State.AttachedAndPlaced) ? binaryReader.ReadInt32() : 0);
				int num13 = ((state == BuilderPiece.State.OnConveyor) ? binaryReader.ReadInt32() : 0);
				float num14 = 10000f;
				if (!(in vector2).IsValid(in num14) || !(in quaternion2).IsValid() || !this.ValidateCreatePieceParams(num6, num7, state, num9))
				{
					this.SetTableState(BuilderTable.TableState.BadData);
					return;
				}
				int num15 = -1;
				if (state == BuilderPiece.State.OnConveyor || state == BuilderPiece.State.OnShelf || state == BuilderPiece.State.Displayed)
				{
					num15 = num8;
					num8 = -1;
				}
				if ((num8 != actorNumber || flag) && this.ValidateDeserializedRootPieceState(num7, state, num15, num8, vector2, quaternion2))
				{
					BuilderPiece builderPiece = this.CreatePieceInternal(num6, num7, vector2, quaternion2, state, num9, num12, this);
					BuilderTable.tempPeiceIds.Add(num7);
					BuilderTable.tempParentActorNumbers.Add(num8);
					BuilderTable.tempInLeftHand.Add(flag4);
					builderPiece.SetFunctionalPieceState(b, NetPlayer.Get(PhotonNetwork.MasterClient), PhotonNetwork.ServerTimestamp);
					if (num15 >= 0 && this.isTableMutable)
					{
						builderPiece.shelfOwner = num15;
						if (state == BuilderPiece.State.OnConveyor)
						{
							BuilderConveyor builderConveyor = this.conveyors[num15];
							float num16 = 0f;
							if (PhotonNetwork.ServerTimestamp > num13)
							{
								num16 = (PhotonNetwork.ServerTimestamp - num13) / 1000f;
							}
							builderConveyor.OnShelfPieceCreated(builderPiece, num16);
						}
						else if (state == BuilderPiece.State.OnShelf || state == BuilderPiece.State.Displayed)
						{
							this.dispenserShelves[num15].OnShelfPieceCreated(builderPiece, false);
						}
					}
				}
			}
			for (int l = 0; l < BuilderTable.tempPeiceIds.Count; l++)
			{
				if (BuilderTable.tempParentActorNumbers[l] >= 0)
				{
					this.AttachPieceToActorInternal(BuilderTable.tempPeiceIds[l], BuilderTable.tempParentActorNumbers[l], BuilderTable.tempInLeftHand[l]);
				}
			}
			BuilderTable.tempPeiceIds.Clear();
			BuilderTable.tempParentActorNumbers.Clear();
			BuilderTable.tempInLeftHand.Clear();
			int num17 = binaryReader.ReadInt32();
			for (int m = 0; m < num17; m++)
			{
				int num18 = binaryReader.ReadInt32();
				int num19 = binaryReader.ReadInt32();
				int num20 = binaryReader.ReadInt32();
				int num21 = binaryReader.ReadInt32();
				int num22 = binaryReader.ReadInt32();
				BuilderPiece.State state2 = (BuilderPiece.State)binaryReader.ReadByte();
				int num23 = binaryReader.ReadInt32();
				bool flag5 = binaryReader.ReadByte() > 0;
				int num24 = binaryReader.ReadInt32();
				int num25 = binaryReader.ReadInt32();
				byte b2 = ((state2 == BuilderPiece.State.AttachedAndPlaced) ? binaryReader.ReadByte() : 0);
				int num26 = ((state2 == BuilderPiece.State.AttachedAndPlaced) ? binaryReader.ReadInt32() : 0);
				int num27 = ((state2 == BuilderPiece.State.OnConveyor) ? binaryReader.ReadInt32() : 0);
				if (!this.ValidateCreatePieceParams(num18, num19, state2, num24))
				{
					this.SetTableState(BuilderTable.TableState.BadData);
					return;
				}
				int num28 = -1;
				if (state2 == BuilderPiece.State.OnConveyor || state2 == BuilderPiece.State.OnShelf || state2 == BuilderPiece.State.Displayed)
				{
					num28 = num23;
					num23 = -1;
				}
				if ((num23 != actorNumber || flag) && this.ValidateDeserializedChildPieceState(num19, state2))
				{
					BuilderPiece builderPiece2 = this.CreatePieceInternal(num18, num19, this.roomCenter.position, Quaternion.identity, state2, num24, num26, this);
					builderPiece2.SetFunctionalPieceState(b2, NetPlayer.Get(PhotonNetwork.MasterClient), PhotonNetwork.ServerTimestamp);
					BuilderTable.tempPeiceIds.Add(num19);
					BuilderTable.tempParentPeiceIds.Add(num20);
					BuilderTable.tempAttachIndexes.Add(num21);
					BuilderTable.tempParentAttachIndexes.Add(num22);
					BuilderTable.tempParentActorNumbers.Add(num23);
					BuilderTable.tempInLeftHand.Add(flag5);
					BuilderTable.tempPiecePlacement.Add(num25);
					if (num28 >= 0 && this.isTableMutable)
					{
						builderPiece2.shelfOwner = num28;
						if (state2 == BuilderPiece.State.OnConveyor)
						{
							BuilderConveyor builderConveyor2 = this.conveyors[num28];
							float num29 = 0f;
							if (PhotonNetwork.ServerTimestamp > num27)
							{
								num29 = (PhotonNetwork.ServerTimestamp - num27) / 1000f;
							}
							builderConveyor2.OnShelfPieceCreated(builderPiece2, num29);
						}
						else if (state2 == BuilderPiece.State.OnShelf || state2 == BuilderPiece.State.Displayed)
						{
							this.dispenserShelves[num28].OnShelfPieceCreated(builderPiece2, false);
						}
					}
				}
			}
			for (int n = 0; n < BuilderTable.tempPeiceIds.Count; n++)
			{
				if (!this.ValidateAttachPieceParams(BuilderTable.tempPeiceIds[n], BuilderTable.tempAttachIndexes[n], BuilderTable.tempParentPeiceIds[n], BuilderTable.tempParentAttachIndexes[n], BuilderTable.tempPiecePlacement[n]))
				{
					this.RecyclePieceInternal(BuilderTable.tempPeiceIds[n], true, false, -1);
				}
				else
				{
					this.AttachPieceInternal(BuilderTable.tempPeiceIds[n], BuilderTable.tempAttachIndexes[n], BuilderTable.tempParentPeiceIds[n], BuilderTable.tempParentAttachIndexes[n], BuilderTable.tempPiecePlacement[n]);
				}
			}
			for (int num30 = 0; num30 < BuilderTable.tempPeiceIds.Count; num30++)
			{
				if (BuilderTable.tempParentActorNumbers[num30] >= 0)
				{
					this.AttachPieceToActorInternal(BuilderTable.tempPeiceIds[num30], BuilderTable.tempParentActorNumbers[num30], BuilderTable.tempInLeftHand[num30]);
				}
			}
			foreach (BuilderPiece builderPiece3 in this.pieces)
			{
				if (builderPiece3.state == BuilderPiece.State.AttachedAndPlaced)
				{
					builderPiece3.OnPlacementDeserialized();
				}
			}
			if (this.isTableMutable)
			{
				this.plotOwners.Clear();
				this.doesLocalPlayerOwnPlot = false;
				int num31 = binaryReader.ReadInt32();
				for (int num32 = 0; num32 < num31; num32++)
				{
					int num33 = binaryReader.ReadInt32();
					int num34 = binaryReader.ReadInt32();
					BuilderPiecePrivatePlot builderPiecePrivatePlot;
					if (this.plotOwners.TryAdd(num33, num34) && this.GetPiece(num34).TryGetPlotComponent(out builderPiecePrivatePlot))
					{
						builderPiecePrivatePlot.ClaimPlotForPlayerNumber(num33);
						if (num33 == PhotonNetwork.LocalPlayer.ActorNumber)
						{
							this.doesLocalPlayerOwnPlot = true;
						}
					}
				}
				UnityEvent<bool> onLocalPlayerClaimedPlot = this.OnLocalPlayerClaimedPlot;
				if (onLocalPlayerClaimedPlot != null)
				{
					onLocalPlayerClaimedPlot.Invoke(this.doesLocalPlayerOwnPlot);
				}
				this.OnDeserializeUpdatePlots();
			}
			else
			{
				BuilderTable.mapIDBuffer = binaryReader.ReadChars(BuilderTable.mapIDBuffer.Length);
				string text = new string(BuilderTable.mapIDBuffer);
				if (SharedBlocksManager.IsMapIDValid(text))
				{
					this.sharedBlocksMap = new SharedBlocksManager.SharedBlocksMap
					{
						MapID = text
					};
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
			int num35 = binaryReader.ReadInt32();
			for (int num36 = 0; num36 < num35; num36++)
			{
				int num37 = binaryReader.ReadInt32();
				int num38 = binaryReader.ReadInt32();
				long num39 = binaryReader.ReadInt64();
				BuilderPiece piece = this.GetPiece(num37);
				if (!(piece == null))
				{
					BuilderPiece piece2 = this.GetPiece(num38);
					if (!(piece2 == null))
					{
						int num40;
						int num41;
						Vector2Int vector2Int;
						Vector2Int vector2Int2;
						this.UnpackSnapInfo(num39, out num40, out num41, out vector2Int, out vector2Int2);
						if (num40 >= 0 && num40 < piece.gridPlanes.Count && num41 >= 0 && num41 < piece2.gridPlanes.Count)
						{
							BuilderTable.SnapOverlapKey snapOverlapKey = BuilderTable.BuildOverlapKey(num37, num38, num40, num41);
							if (!BuilderTable.tempDuplicateOverlaps.Contains(snapOverlapKey))
							{
								BuilderTable.tempDuplicateOverlaps.Add(snapOverlapKey);
								piece.gridPlanes[num40].AddSnapOverlap(this.builderPool.CreateSnapOverlap(piece2.gridPlanes[num41], new SnapBounds(vector2Int, vector2Int2)));
							}
						}
					}
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
		}

		// Token: 0x04006F94 RID: 28564
		public const GTZone BUILDER_ZONE = GTZone.monkeBlocks;

		// Token: 0x04006F95 RID: 28565
		private const int INITIAL_BUILTIN_PIECE_ID = 5;

		// Token: 0x04006F96 RID: 28566
		private const int INITIAL_CREATED_PIECE_ID = 10000;

		// Token: 0x04006F97 RID: 28567
		public static float MAX_DROP_VELOCITY = 20f;

		// Token: 0x04006F98 RID: 28568
		public static float MAX_DROP_ANG_VELOCITY = 50f;

		// Token: 0x04006F99 RID: 28569
		private const float MAX_DISTANCE_FROM_CENTER = 217f;

		// Token: 0x04006F9A RID: 28570
		private const float MAX_LOCAL_MAGNITUDE = 80f;

		// Token: 0x04006F9B RID: 28571
		public const float MAX_DISTANCE_FROM_HAND = 2.5f;

		// Token: 0x04006F9C RID: 28572
		public static float DROP_ZONE_REPEL = 2.25f;

		// Token: 0x04006F9D RID: 28573
		public static int placedLayer;

		// Token: 0x04006F9E RID: 28574
		public static int heldLayer;

		// Token: 0x04006F9F RID: 28575
		public static int heldLayerLocal;

		// Token: 0x04006FA0 RID: 28576
		public static int droppedLayer;

		// Token: 0x04006FA1 RID: 28577
		private float acceptableSqrDistFromCenter = 47089f;

		// Token: 0x04006FA2 RID: 28578
		public float pieceScale = 0.04f;

		// Token: 0x04006FA3 RID: 28579
		public GTZone tableZone = GTZone.monkeBlocks;

		// Token: 0x04006FA4 RID: 28580
		[SerializeField]
		private string SharedMapConfigTitleDataKey = "SharedBlocksStartingMapConfig";

		// Token: 0x04006FA5 RID: 28581
		public BuilderTableNetworking builderNetworking;

		// Token: 0x04006FA6 RID: 28582
		public BuilderRenderer builderRenderer;

		// Token: 0x04006FA7 RID: 28583
		[HideInInspector]
		public BuilderPool builderPool;

		// Token: 0x04006FA8 RID: 28584
		public Transform tableCenter;

		// Token: 0x04006FA9 RID: 28585
		public Transform roomCenter;

		// Token: 0x04006FAA RID: 28586
		public Transform worldCenter;

		// Token: 0x04006FAB RID: 28587
		public GameObject noBlocksArea;

		// Token: 0x04006FAC RID: 28588
		public List<GameObject> builtInPieceRoots;

		// Token: 0x04006FAD RID: 28589
		[Tooltip("Optional terminal to control loaded blocks")]
		public SharedBlocksTerminal linkedTerminal;

		// Token: 0x04006FAE RID: 28590
		[Tooltip("Can Blocks Be Placed and Grabbed")]
		public bool isTableMutable;

		// Token: 0x04006FAF RID: 28591
		public GameObject shelvesRoot;

		// Token: 0x04006FB0 RID: 28592
		public GameObject dropZoneRoot;

		// Token: 0x04006FB1 RID: 28593
		public List<GameObject> recyclerRoot;

		// Token: 0x04006FB2 RID: 28594
		public List<GameObject> allShelvesRoot;

		// Token: 0x04006FB3 RID: 28595
		[NonSerialized]
		public List<BuilderConveyor> conveyors = new List<BuilderConveyor>();

		// Token: 0x04006FB4 RID: 28596
		[NonSerialized]
		public List<BuilderDispenserShelf> dispenserShelves = new List<BuilderDispenserShelf>();

		// Token: 0x04006FB5 RID: 28597
		public BuilderConveyorManager conveyorManager;

		// Token: 0x04006FB6 RID: 28598
		public List<BuilderResourceMeter> resourceMeters;

		// Token: 0x04006FB7 RID: 28599
		public GameObject sharedBuildArea;

		// Token: 0x04006FB8 RID: 28600
		private BoxCollider[] sharedBuildAreas;

		// Token: 0x04006FB9 RID: 28601
		public BuilderPiece armShelfPieceType;

		// Token: 0x04006FBA RID: 28602
		[NonSerialized]
		public List<BuilderRecycler> recyclers;

		// Token: 0x04006FBB RID: 28603
		[NonSerialized]
		public List<BuilderDropZone> dropZones;

		// Token: 0x04006FBC RID: 28604
		private int shelfSliceUpdateIndex;

		// Token: 0x04006FBD RID: 28605
		public static int SHELF_SLICE_BUCKETS = 6;

		// Token: 0x04006FBE RID: 28606
		public float defaultTint = 1f;

		// Token: 0x04006FBF RID: 28607
		public float droppedTint = 0.75f;

		// Token: 0x04006FC0 RID: 28608
		public float grabbedTint = 0.75f;

		// Token: 0x04006FC1 RID: 28609
		public float shelfTint = 1f;

		// Token: 0x04006FC2 RID: 28610
		public float potentialGrabTint = 0.75f;

		// Token: 0x04006FC3 RID: 28611
		public float paintingTint = 0.6f;

		// Token: 0x04006FC5 RID: 28613
		private List<BuilderTable.BoxCheckParams> noBlocksAreas;

		// Token: 0x04006FC6 RID: 28614
		private Collider[] noBlocksCheckResults = new Collider[64];

		// Token: 0x04006FC7 RID: 28615
		public LayerMask allPiecesMask;

		// Token: 0x04006FC8 RID: 28616
		public bool useSnapRotation;

		// Token: 0x04006FC9 RID: 28617
		public BuilderPlacementStyle usePlacementStyle;

		// Token: 0x04006FCA RID: 28618
		public BuilderOptionButton buttonSnapRotation;

		// Token: 0x04006FCB RID: 28619
		public BuilderOptionButton buttonSnapPosition;

		// Token: 0x04006FCC RID: 28620
		public BuilderOptionButton buttonSaveLayout;

		// Token: 0x04006FCD RID: 28621
		public BuilderOptionButton buttonClearLayout;

		// Token: 0x04006FCE RID: 28622
		[HideInInspector]
		public List<BuilderAttachGridPlane> baseGridPlanes;

		// Token: 0x04006FCF RID: 28623
		private List<BuilderPiece> basePieces;

		// Token: 0x04006FD0 RID: 28624
		[HideInInspector]
		public List<BuilderPiecePrivatePlot> allPrivatePlots;

		// Token: 0x04006FD1 RID: 28625
		private int nextPieceId;

		// Token: 0x04006FD2 RID: 28626
		[HideInInspector]
		public List<BuilderTable.BuildPieceSpawn> buildPieceSpawns;

		// Token: 0x04006FD3 RID: 28627
		[HideInInspector]
		public List<BuilderShelf> shelves;

		// Token: 0x04006FD4 RID: 28628
		[NonSerialized]
		public List<BuilderPiece> pieces = new List<BuilderPiece>(1024);

		// Token: 0x04006FD5 RID: 28629
		private Dictionary<int, int> pieceIDToIndexCache = new Dictionary<int, int>(1024);

		// Token: 0x04006FD6 RID: 28630
		[HideInInspector]
		public Dictionary<int, int> plotOwners;

		// Token: 0x04006FD7 RID: 28631
		private bool doesLocalPlayerOwnPlot;

		// Token: 0x04006FD8 RID: 28632
		public Dictionary<int, int> playerToArmShelfLeft;

		// Token: 0x04006FD9 RID: 28633
		public Dictionary<int, int> playerToArmShelfRight;

		// Token: 0x04006FDA RID: 28634
		private HashSet<int> builderPiecesVisited = new HashSet<int>(128);

		// Token: 0x04006FDB RID: 28635
		public BuilderResources totalResources;

		// Token: 0x04006FDC RID: 28636
		[Tooltip("Resources reserved for conveyors and dispensers")]
		public BuilderResources totalReservedResources;

		// Token: 0x04006FDD RID: 28637
		public BuilderResources resourcesPerPrivatePlot;

		// Token: 0x04006FDE RID: 28638
		[NonSerialized]
		public int[] maxResources;

		// Token: 0x04006FDF RID: 28639
		private int[] plotMaxResources;

		// Token: 0x04006FE0 RID: 28640
		[NonSerialized]
		public int[] usedResources;

		// Token: 0x04006FE1 RID: 28641
		[NonSerialized]
		public int[] reservedResources;

		// Token: 0x04006FE2 RID: 28642
		private List<int> playersInBuilder;

		// Token: 0x04006FE3 RID: 28643
		private List<IBuilderPieceFunctional> activeFunctionalComponents = new List<IBuilderPieceFunctional>(128);

		// Token: 0x04006FE4 RID: 28644
		private List<IBuilderPieceFunctional> funcComponentsToRegister = new List<IBuilderPieceFunctional>(10);

		// Token: 0x04006FE5 RID: 28645
		private List<IBuilderPieceFunctional> funcComponentsToUnregister = new List<IBuilderPieceFunctional>(10);

		// Token: 0x04006FE6 RID: 28646
		private List<IBuilderPieceFunctional> fixedUpdateFunctionalComponents = new List<IBuilderPieceFunctional>(128);

		// Token: 0x04006FE7 RID: 28647
		private List<IBuilderPieceFunctional> funcComponentsToRegisterFixed = new List<IBuilderPieceFunctional>(10);

		// Token: 0x04006FE8 RID: 28648
		private List<IBuilderPieceFunctional> funcComponentsToUnregisterFixed = new List<IBuilderPieceFunctional>(10);

		// Token: 0x04006FE9 RID: 28649
		private const int MAX_SPHERE_CHECK_RESULTS = 1024;

		// Token: 0x04006FEA RID: 28650
		private NativeList<BuilderGridPlaneData> gridPlaneData;

		// Token: 0x04006FEB RID: 28651
		private NativeList<BuilderGridPlaneData> checkGridPlaneData;

		// Token: 0x04006FEC RID: 28652
		private NativeArray<ColliderHit> nearbyPiecesResults;

		// Token: 0x04006FED RID: 28653
		private NativeArray<OverlapSphereCommand> nearbyPiecesCommands;

		// Token: 0x04006FEE RID: 28654
		private List<BuilderPotentialPlacement> allPotentialPlacements;

		// Token: 0x04006FEF RID: 28655
		private static HashSet<BuilderPiece> tempPieceSet = new HashSet<BuilderPiece>(512);

		// Token: 0x04006FF0 RID: 28656
		private BuilderTable.TableState tableState;

		// Token: 0x04006FF1 RID: 28657
		private bool inRoom;

		// Token: 0x04006FF2 RID: 28658
		private bool inBuilderZone;

		// Token: 0x04006FF3 RID: 28659
		private static int DROPPED_PIECE_LIMIT = 100;

		// Token: 0x04006FF4 RID: 28660
		public static string nextUpdateOverride = string.Empty;

		// Token: 0x04006FF5 RID: 28661
		private List<BuilderPiece> droppedPieces;

		// Token: 0x04006FF6 RID: 28662
		private List<BuilderTable.DroppedPieceData> droppedPieceData;

		// Token: 0x04006FF7 RID: 28663
		private HashSet<int>[] repelledPieceRoots;

		// Token: 0x04006FF8 RID: 28664
		private int repelHistoryLength = 3;

		// Token: 0x04006FF9 RID: 28665
		private int repelHistoryIndex;

		// Token: 0x04006FFA RID: 28666
		private bool hasRequestedConfig;

		// Token: 0x04006FFB RID: 28667
		private bool isDirty;

		// Token: 0x04006FFC RID: 28668
		private bool saveInProgress;

		// Token: 0x04006FFD RID: 28669
		private int currentSaveSlot = -1;

		// Token: 0x04006FFE RID: 28670
		[HideInInspector]
		public UnityEvent OnSaveTimeUpdated;

		// Token: 0x04006FFF RID: 28671
		[HideInInspector]
		public UnityEvent<bool> OnSaveDirtyChanged;

		// Token: 0x04007000 RID: 28672
		[HideInInspector]
		public UnityEvent OnSaveSuccess;

		// Token: 0x04007001 RID: 28673
		[HideInInspector]
		public UnityEvent<string> OnSaveFailure;

		// Token: 0x04007002 RID: 28674
		[HideInInspector]
		public UnityEvent OnTableConfigurationUpdated;

		// Token: 0x04007003 RID: 28675
		[HideInInspector]
		public UnityEvent<bool> OnLocalPlayerClaimedPlot;

		// Token: 0x04007004 RID: 28676
		[HideInInspector]
		public UnityEvent OnMapCleared;

		// Token: 0x04007005 RID: 28677
		[HideInInspector]
		public UnityEvent<string> OnMapLoaded;

		// Token: 0x04007006 RID: 28678
		[HideInInspector]
		public UnityEvent<string> OnMapLoadFailed;

		// Token: 0x04007007 RID: 28679
		private List<BuilderTable.BuilderCommand> queuedBuildCommands;

		// Token: 0x04007008 RID: 28680
		private List<BuilderAction> rollBackActions;

		// Token: 0x04007009 RID: 28681
		private List<BuilderTable.BuilderCommand> rollBackBufferedCommands;

		// Token: 0x0400700A RID: 28682
		private List<BuilderTable.BuilderCommand> rollForwardCommands;

		// Token: 0x0400700B RID: 28683
		[OnEnterPlay_Clear]
		private static Dictionary<GTZone, BuilderTable> zoneToInstance;

		// Token: 0x0400700C RID: 28684
		private bool isSetup;

		// Token: 0x0400700D RID: 28685
		public BuilderTable.SnapParams pushAndEaseParams;

		// Token: 0x0400700E RID: 28686
		public BuilderTable.SnapParams overlapParams;

		// Token: 0x0400700F RID: 28687
		private BuilderTable.SnapParams currSnapParams;

		// Token: 0x04007010 RID: 28688
		public int maxPlacementChildDepth = 5;

		// Token: 0x04007011 RID: 28689
		public List<SimpleAABB> m_areaBounds = new List<SimpleAABB>();

		// Token: 0x04007012 RID: 28690
		private static List<BuilderPiece> tempPieces = new List<BuilderPiece>(256);

		// Token: 0x04007013 RID: 28691
		private static List<BuilderConveyor> tempConveyors = new List<BuilderConveyor>(256);

		// Token: 0x04007014 RID: 28692
		private static List<BuilderDispenserShelf> tempDispensers = new List<BuilderDispenserShelf>(256);

		// Token: 0x04007015 RID: 28693
		private static List<BuilderRecycler> tempRecyclers = new List<BuilderRecycler>(5);

		// Token: 0x04007016 RID: 28694
		private static List<BuilderTable.BuilderCommand> tempRollForwardCommands = new List<BuilderTable.BuilderCommand>(128);

		// Token: 0x04007017 RID: 28695
		private static List<BuilderPiece> tempDeletePieces = new List<BuilderPiece>(1024);

		// Token: 0x04007018 RID: 28696
		public const int MAX_PIECE_DATA = 2560;

		// Token: 0x04007019 RID: 28697
		public const int MAX_GRID_PLANE_DATA = 10240;

		// Token: 0x0400701A RID: 28698
		public const int MAX_PRIVATE_PLOT_DATA = 64;

		// Token: 0x0400701B RID: 28699
		public const int MAX_PLAYER_DATA = 64;

		// Token: 0x0400701C RID: 28700
		private BuilderTableData tableData;

		// Token: 0x0400701D RID: 28701
		private int fetchConfigurationAttempts;

		// Token: 0x0400701E RID: 28702
		private int maxRetries = 3;

		// Token: 0x0400701F RID: 28703
		private SharedBlocksManager.SharedBlocksMap sharedBlocksMap;

		// Token: 0x04007020 RID: 28704
		private string pendingMapID;

		// Token: 0x04007021 RID: 28705
		private SharedBlocksManager.StartingMapConfig startingMapConfig = new SharedBlocksManager.StartingMapConfig
		{
			pageNumber = 0,
			pageSize = 50,
			sortMethod = SharedBlocksManager.MapSortMethod.Top.ToString(),
			useMapID = false,
			mapID = null
		};

		// Token: 0x04007022 RID: 28706
		private List<SharedBlocksManager.SharedBlocksMap> startingMapList = new List<SharedBlocksManager.SharedBlocksMap>();

		// Token: 0x04007023 RID: 28707
		private SharedBlocksManager.SharedBlocksMap startingMap;

		// Token: 0x04007024 RID: 28708
		private bool hasStartingMap;

		// Token: 0x04007025 RID: 28709
		private double startingMapCacheTime = double.MinValue;

		// Token: 0x04007026 RID: 28710
		private bool getStartingMapInProgress;

		// Token: 0x04007027 RID: 28711
		private bool hasCachedTopMaps;

		// Token: 0x04007028 RID: 28712
		private double lastGetTopMapsTime = double.MinValue;

		// Token: 0x04007029 RID: 28713
		private static string personalBuildKey = "MyBuild";

		// Token: 0x0400702A RID: 28714
		private static HashSet<BuilderTable.SnapOverlapKey> tempDuplicateOverlaps = new HashSet<BuilderTable.SnapOverlapKey>(16384);

		// Token: 0x0400702B RID: 28715
		private static List<BuilderPiece> childPieces = new List<BuilderPiece>(4096);

		// Token: 0x0400702C RID: 28716
		private static List<BuilderPiece> rootPieces = new List<BuilderPiece>(4096);

		// Token: 0x0400702D RID: 28717
		private static List<int> overlapPieces = new List<int>(4096);

		// Token: 0x0400702E RID: 28718
		private static List<int> overlapOtherPieces = new List<int>(4096);

		// Token: 0x0400702F RID: 28719
		private static List<long> overlapPacked = new List<long>(4096);

		// Token: 0x04007030 RID: 28720
		private static char[] mapIDBuffer = new char[8];

		// Token: 0x04007031 RID: 28721
		private static Dictionary<long, int> snapOverlapSanity = new Dictionary<long, int>(16384);

		// Token: 0x04007032 RID: 28722
		private static List<int> tempPeiceIds = new List<int>(4096);

		// Token: 0x04007033 RID: 28723
		private static List<int> tempParentPeiceIds = new List<int>(4096);

		// Token: 0x04007034 RID: 28724
		private static List<int> tempAttachIndexes = new List<int>(4096);

		// Token: 0x04007035 RID: 28725
		private static List<int> tempParentAttachIndexes = new List<int>(4096);

		// Token: 0x04007036 RID: 28726
		private static List<int> tempParentActorNumbers = new List<int>(4096);

		// Token: 0x04007037 RID: 28727
		private static List<bool> tempInLeftHand = new List<bool>(4096);

		// Token: 0x04007038 RID: 28728
		private static List<int> tempPiecePlacement = new List<int>(4096);

		// Token: 0x02000F63 RID: 3939
		private struct BoxCheckParams
		{
			// Token: 0x04007039 RID: 28729
			public Vector3 center;

			// Token: 0x0400703A RID: 28730
			public Vector3 halfExtents;

			// Token: 0x0400703B RID: 28731
			public Quaternion rotation;
		}

		// Token: 0x02000F64 RID: 3940
		[Serializable]
		public class BuildPieceSpawn
		{
			// Token: 0x0400703C RID: 28732
			public GameObject buildPiecePrefab;

			// Token: 0x0400703D RID: 28733
			public int count = 1;
		}

		// Token: 0x02000F65 RID: 3941
		public enum BuilderCommandType
		{
			// Token: 0x0400703F RID: 28735
			Create,
			// Token: 0x04007040 RID: 28736
			Place,
			// Token: 0x04007041 RID: 28737
			Grab,
			// Token: 0x04007042 RID: 28738
			Drop,
			// Token: 0x04007043 RID: 28739
			Remove,
			// Token: 0x04007044 RID: 28740
			Paint,
			// Token: 0x04007045 RID: 28741
			Recycle,
			// Token: 0x04007046 RID: 28742
			ClaimPlot,
			// Token: 0x04007047 RID: 28743
			FreePlot,
			// Token: 0x04007048 RID: 28744
			CreateArmShelf,
			// Token: 0x04007049 RID: 28745
			PlayerLeftRoom,
			// Token: 0x0400704A RID: 28746
			FunctionalStateChange,
			// Token: 0x0400704B RID: 28747
			SetSelection,
			// Token: 0x0400704C RID: 28748
			Repel
		}

		// Token: 0x02000F66 RID: 3942
		public enum TableState
		{
			// Token: 0x0400704E RID: 28750
			WaitingForZoneAndRoom,
			// Token: 0x0400704F RID: 28751
			WaitingForInitalBuild,
			// Token: 0x04007050 RID: 28752
			ReceivingInitialBuild,
			// Token: 0x04007051 RID: 28753
			WaitForInitialBuildMaster,
			// Token: 0x04007052 RID: 28754
			WaitForMasterResync,
			// Token: 0x04007053 RID: 28755
			ReceivingMasterResync,
			// Token: 0x04007054 RID: 28756
			InitialBuild,
			// Token: 0x04007055 RID: 28757
			ExecuteQueuedCommands,
			// Token: 0x04007056 RID: 28758
			Ready,
			// Token: 0x04007057 RID: 28759
			BadData,
			// Token: 0x04007058 RID: 28760
			WaitingForSharedMapLoad
		}

		// Token: 0x02000F67 RID: 3943
		public enum DroppedPieceState
		{
			// Token: 0x0400705A RID: 28762
			None = -1,
			// Token: 0x0400705B RID: 28763
			Light,
			// Token: 0x0400705C RID: 28764
			Heavy,
			// Token: 0x0400705D RID: 28765
			Frozen
		}

		// Token: 0x02000F68 RID: 3944
		private struct DroppedPieceData
		{
			// Token: 0x0400705E RID: 28766
			public BuilderTable.DroppedPieceState droppedState;

			// Token: 0x0400705F RID: 28767
			public float speedThreshCrossedTime;

			// Token: 0x04007060 RID: 28768
			public float filteredSpeed;
		}

		// Token: 0x02000F69 RID: 3945
		public struct BuilderCommand
		{
			// Token: 0x04007061 RID: 28769
			public BuilderTable.BuilderCommandType type;

			// Token: 0x04007062 RID: 28770
			public int pieceType;

			// Token: 0x04007063 RID: 28771
			public int pieceId;

			// Token: 0x04007064 RID: 28772
			public int attachPieceId;

			// Token: 0x04007065 RID: 28773
			public int parentPieceId;

			// Token: 0x04007066 RID: 28774
			public int parentAttachIndex;

			// Token: 0x04007067 RID: 28775
			public int attachIndex;

			// Token: 0x04007068 RID: 28776
			public Vector3 localPosition;

			// Token: 0x04007069 RID: 28777
			public Quaternion localRotation;

			// Token: 0x0400706A RID: 28778
			public byte twist;

			// Token: 0x0400706B RID: 28779
			public sbyte bumpOffsetX;

			// Token: 0x0400706C RID: 28780
			public sbyte bumpOffsetZ;

			// Token: 0x0400706D RID: 28781
			public Vector3 velocity;

			// Token: 0x0400706E RID: 28782
			public Vector3 angVelocity;

			// Token: 0x0400706F RID: 28783
			public bool isLeft;

			// Token: 0x04007070 RID: 28784
			public int materialType;

			// Token: 0x04007071 RID: 28785
			public NetPlayer player;

			// Token: 0x04007072 RID: 28786
			public BuilderPiece.State state;

			// Token: 0x04007073 RID: 28787
			public bool isQueued;

			// Token: 0x04007074 RID: 28788
			public bool canRollback;

			// Token: 0x04007075 RID: 28789
			public int localCommandId;

			// Token: 0x04007076 RID: 28790
			public int serverTimeStamp;
		}

		// Token: 0x02000F6A RID: 3946
		[Serializable]
		public struct SnapParams
		{
			// Token: 0x04007077 RID: 28791
			public float minOffsetY;

			// Token: 0x04007078 RID: 28792
			public float maxOffsetY;

			// Token: 0x04007079 RID: 28793
			public float maxUpDotProduct;

			// Token: 0x0400707A RID: 28794
			public float maxTwistDotProduct;

			// Token: 0x0400707B RID: 28795
			public float snapAttachDistance;

			// Token: 0x0400707C RID: 28796
			public float snapDelayTime;

			// Token: 0x0400707D RID: 28797
			public float snapDelayOffsetDist;

			// Token: 0x0400707E RID: 28798
			public float unSnapDelayTime;

			// Token: 0x0400707F RID: 28799
			public float unSnapDelayDist;

			// Token: 0x04007080 RID: 28800
			public float maxBlockSnapDist;
		}

		// Token: 0x02000F6B RID: 3947
		private struct SnapOverlapKey
		{
			// Token: 0x060061D4 RID: 25044 RVA: 0x001F6529 File Offset: 0x001F4729
			public override int GetHashCode()
			{
				return HashCode.Combine<int, int>(this.piece.GetHashCode(), this.otherPiece.GetHashCode());
			}

			// Token: 0x060061D5 RID: 25045 RVA: 0x001F6546 File Offset: 0x001F4746
			public bool Equals(BuilderTable.SnapOverlapKey other)
			{
				return this.piece == other.piece && this.otherPiece == other.otherPiece;
			}

			// Token: 0x060061D6 RID: 25046 RVA: 0x001F6566 File Offset: 0x001F4766
			public override bool Equals(object o)
			{
				return o is BuilderTable.SnapOverlapKey && this.Equals((BuilderTable.SnapOverlapKey)o);
			}

			// Token: 0x04007081 RID: 28801
			public long piece;

			// Token: 0x04007082 RID: 28802
			public long otherPiece;
		}
	}
}
