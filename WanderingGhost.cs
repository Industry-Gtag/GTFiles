using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000ADC RID: 2780
[NetworkBehaviourWeaved(1)]
public class WanderingGhost : NetworkComponent
{
	// Token: 0x06004753 RID: 18259 RVA: 0x00180B74 File Offset: 0x0017ED74
	protected override void Start()
	{
		base.Start();
		this.waypointRegions = this.waypointsContainer.GetComponentsInChildren<ZoneBasedObject>();
		this.idlePassedTime = 0f;
		ThrowableSetDressing[] array = this.allFlowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].anchor.position = this.flowerDisabledPosition;
		}
		base.Invoke("DelayedStart", 0.5f);
	}

	// Token: 0x06004754 RID: 18260 RVA: 0x00180BDB File Offset: 0x0017EDDB
	private void DelayedStart()
	{
		this.PickNextWaypoint();
		base.transform.position = this.currentWaypoint._transform.position;
		this.PickNextWaypoint();
		this.ChangeState(WanderingGhost.ghostState.patrol);
	}

	// Token: 0x06004755 RID: 18261 RVA: 0x00180C0C File Offset: 0x0017EE0C
	private void LateUpdate()
	{
		this.UpdateState();
		this.hoverVelocity -= this.mrenderer.transform.localPosition * this.hoverRectifyForce * Time.deltaTime;
		this.hoverVelocity += Random.insideUnitSphere * this.hoverRandomForce * Time.deltaTime;
		this.hoverVelocity = Vector3.MoveTowards(this.hoverVelocity, Vector3.zero, this.hoverDrag * Time.deltaTime);
		this.mrenderer.transform.localPosition += this.hoverVelocity * Time.deltaTime;
	}

	// Token: 0x06004756 RID: 18262 RVA: 0x00180CD0 File Offset: 0x0017EED0
	private void PickNextWaypoint()
	{
		if (this.waypoints.Count == 0 || this.lastWaypointRegion == null || !this.lastWaypointRegion.IsLocalPlayerInZone())
		{
			ZoneBasedObject zoneBasedObject = ZoneBasedObject.SelectRandomEligible(this.waypointRegions, this.debugForceWaypointRegion);
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
				this.waypoints.Add(new WanderingGhost.Waypoint(transform.name.Contains("_v_"), transform));
			}
		}
		int num = Random.Range(0, this.waypoints.Count);
		this.currentWaypoint = this.waypoints[num];
		this.waypoints.RemoveAt(num);
	}

	// Token: 0x06004757 RID: 18263 RVA: 0x00180DE0 File Offset: 0x0017EFE0
	private void Patrol()
	{
		this.idlePassedTime = 0f;
		this.mrenderer.sharedMaterial = this.scryableMaterial;
		Transform transform = this.currentWaypoint._transform;
		base.transform.position = Vector3.MoveTowards(base.transform.position, transform.position, this.patrolSpeed * Time.deltaTime);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.LookRotation(transform.position - base.transform.position), 360f * Time.deltaTime);
	}

	// Token: 0x06004758 RID: 18264 RVA: 0x00180E84 File Offset: 0x0017F084
	private bool MaybeHideGhost()
	{
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.sphereColliderRadius, this.hitColliders);
		for (int i = 0; i < num; i++)
		{
			if (this.hitColliders[i].gameObject.IsOnLayer(UnityLayer.GorillaHand) || this.hitColliders[i].gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider))
			{
				this.ChangeState(WanderingGhost.ghostState.patrol);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004759 RID: 18265 RVA: 0x00180EF0 File Offset: 0x0017F0F0
	private void ChangeState(WanderingGhost.ghostState newState)
	{
		this.currentState = newState;
		this.mrenderer.sharedMaterial = ((newState == WanderingGhost.ghostState.idle) ? this.visibleMaterial : this.scryableMaterial);
		if (newState == WanderingGhost.ghostState.patrol)
		{
			this.audioSource.GTStop();
			this.audioSource.volume = this.patrolVolume;
			this.audioSource.clip = this.patrolAudio;
			this.audioSource.GTPlay();
			return;
		}
		if (newState != WanderingGhost.ghostState.idle)
		{
			return;
		}
		this.audioSource.GTStop();
		this.audioSource.volume = this.idleVolume;
		this.audioSource.GTPlayOneShot(this.appearAudio.GetRandomItem<AudioClip>(), 1f);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.SpawnFlowerNearby();
		}
	}

	// Token: 0x0600475A RID: 18266 RVA: 0x00180FAC File Offset: 0x0017F1AC
	private void UpdateState()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		WanderingGhost.ghostState ghostState = this.currentState;
		if (ghostState != WanderingGhost.ghostState.patrol)
		{
			if (ghostState != WanderingGhost.ghostState.idle)
			{
				return;
			}
			this.idlePassedTime += Time.deltaTime;
			if (this.idlePassedTime >= this.idleStayDuration || this.MaybeHideGhost())
			{
				this.PickNextWaypoint();
				this.ChangeState(WanderingGhost.ghostState.patrol);
			}
		}
		else
		{
			if (this.currentWaypoint._transform == null)
			{
				this.PickNextWaypoint();
				return;
			}
			this.Patrol();
			if (Vector3.Distance(base.transform.position, this.currentWaypoint._transform.position) < 0.2f)
			{
				if (this.currentWaypoint._visible)
				{
					this.ChangeState(WanderingGhost.ghostState.idle);
					return;
				}
				this.PickNextWaypoint();
				return;
			}
		}
	}

	// Token: 0x0600475B RID: 18267 RVA: 0x00181070 File Offset: 0x0017F270
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

	// Token: 0x170006AB RID: 1707
	// (get) Token: 0x0600475C RID: 18268 RVA: 0x001810D1 File Offset: 0x0017F2D1
	// (set) Token: 0x0600475D RID: 18269 RVA: 0x001810FB File Offset: 0x0017F2FB
	[Networked]
	[NetworkedWeaved(0, 1)]
	private unsafe WanderingGhost.ghostState Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing WanderingGhost.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return (WanderingGhost.ghostState)this.Ptr[0];
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing WanderingGhost.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			this.Ptr[0] = (int)value;
		}
	}

	// Token: 0x0600475E RID: 18270 RVA: 0x00181126 File Offset: 0x0017F326
	public override void WriteDataFusion()
	{
		this.Data = this.currentState;
	}

	// Token: 0x0600475F RID: 18271 RVA: 0x00181134 File Offset: 0x0017F334
	public override void ReadDataFusion()
	{
		this.ReadDataShared(this.Data);
	}

	// Token: 0x06004760 RID: 18272 RVA: 0x00181142 File Offset: 0x0017F342
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		stream.SendNext(this.currentState);
	}

	// Token: 0x06004761 RID: 18273 RVA: 0x00181164 File Offset: 0x0017F364
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		WanderingGhost.ghostState ghostState = (WanderingGhost.ghostState)stream.ReceiveNext();
		this.ReadDataShared(ghostState);
	}

	// Token: 0x06004762 RID: 18274 RVA: 0x00181192 File Offset: 0x0017F392
	private void ReadDataShared(WanderingGhost.ghostState state)
	{
		WanderingGhost.ghostState ghostState = this.currentState;
		this.currentState = state;
		if (ghostState != this.currentState)
		{
			this.ChangeState(this.currentState);
		}
	}

	// Token: 0x06004763 RID: 18275 RVA: 0x001811B5 File Offset: 0x0017F3B5
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		base.OnOwnerChange(newOwner, previousOwner);
		if (newOwner == PhotonNetwork.LocalPlayer)
		{
			this.ChangeState(this.currentState);
		}
	}

	// Token: 0x06004764 RID: 18276 RVA: 0x001811D4 File Offset: 0x0017F3D4
	private void SpawnFlowerNearby()
	{
		Vector3 vector = base.transform.position + Vector3.down * 0.25f;
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(base.transform.position + Random.insideUnitCircle.x0y() * this.flowerSpawnRadius, Vector3.down), out raycastHit, 3f, this.flowerGroundMask))
		{
			vector = raycastHit.point;
		}
		ThrowableSetDressing throwableSetDressing = null;
		int num = 0;
		foreach (ThrowableSetDressing throwableSetDressing2 in this.allFlowers)
		{
			if (!throwableSetDressing2.InHand())
			{
				num++;
				if (Random.Range(0, num) == 0)
				{
					throwableSetDressing = throwableSetDressing2;
				}
			}
		}
		if (throwableSetDressing != null)
		{
			if (!throwableSetDressing.IsLocalOwnedWorldShareable)
			{
				throwableSetDressing.WorldShareableRequestOwnership();
			}
			throwableSetDressing.SetWillTeleport();
			throwableSetDressing.transform.position = vector;
			throwableSetDressing.StartRespawnTimer(this.flowerSpawnDuration);
		}
	}

	// Token: 0x06004766 RID: 18278 RVA: 0x00181314 File Offset: 0x0017F514
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06004767 RID: 18279 RVA: 0x0018132C File Offset: 0x0017F52C
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x040059D3 RID: 22995
	public float patrolSpeed = 3f;

	// Token: 0x040059D4 RID: 22996
	public float idleStayDuration = 5f;

	// Token: 0x040059D5 RID: 22997
	public float sphereColliderRadius = 2f;

	// Token: 0x040059D6 RID: 22998
	public ThrowableSetDressing[] allFlowers;

	// Token: 0x040059D7 RID: 22999
	public Vector3 flowerDisabledPosition;

	// Token: 0x040059D8 RID: 23000
	public float flowerSpawnRadius;

	// Token: 0x040059D9 RID: 23001
	public float flowerSpawnDuration;

	// Token: 0x040059DA RID: 23002
	public LayerMask flowerGroundMask;

	// Token: 0x040059DB RID: 23003
	public MeshRenderer mrenderer;

	// Token: 0x040059DC RID: 23004
	public Material visibleMaterial;

	// Token: 0x040059DD RID: 23005
	public Material scryableMaterial;

	// Token: 0x040059DE RID: 23006
	public GameObject waypointsContainer;

	// Token: 0x040059DF RID: 23007
	private ZoneBasedObject[] waypointRegions;

	// Token: 0x040059E0 RID: 23008
	private ZoneBasedObject lastWaypointRegion;

	// Token: 0x040059E1 RID: 23009
	private List<WanderingGhost.Waypoint> waypoints = new List<WanderingGhost.Waypoint>();

	// Token: 0x040059E2 RID: 23010
	private WanderingGhost.Waypoint currentWaypoint;

	// Token: 0x040059E3 RID: 23011
	public string debugForceWaypointRegion;

	// Token: 0x040059E4 RID: 23012
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040059E5 RID: 23013
	public AudioClip[] appearAudio;

	// Token: 0x040059E6 RID: 23014
	public float idleVolume;

	// Token: 0x040059E7 RID: 23015
	public AudioClip patrolAudio;

	// Token: 0x040059E8 RID: 23016
	public float patrolVolume;

	// Token: 0x040059E9 RID: 23017
	private WanderingGhost.ghostState currentState;

	// Token: 0x040059EA RID: 23018
	private float idlePassedTime;

	// Token: 0x040059EB RID: 23019
	public UnityAction<GameObject> TriggerHauntedObjects;

	// Token: 0x040059EC RID: 23020
	private Vector3 hoverVelocity;

	// Token: 0x040059ED RID: 23021
	public float hoverRectifyForce;

	// Token: 0x040059EE RID: 23022
	public float hoverRandomForce;

	// Token: 0x040059EF RID: 23023
	public float hoverDrag;

	// Token: 0x040059F0 RID: 23024
	private const int maxColliders = 10;

	// Token: 0x040059F1 RID: 23025
	private Collider[] hitColliders = new Collider[10];

	// Token: 0x040059F2 RID: 23026
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private WanderingGhost.ghostState _Data;

	// Token: 0x02000ADD RID: 2781
	[Serializable]
	public struct Waypoint
	{
		// Token: 0x06004768 RID: 18280 RVA: 0x00181340 File Offset: 0x0017F540
		public Waypoint(bool visible, Transform tr)
		{
			this._visible = visible;
			this._transform = tr;
		}

		// Token: 0x040059F3 RID: 23027
		[Tooltip("The ghost will be visible when its reached to this waypoint")]
		public bool _visible;

		// Token: 0x040059F4 RID: 23028
		public Transform _transform;
	}

	// Token: 0x02000ADE RID: 2782
	private enum ghostState
	{
		// Token: 0x040059F6 RID: 23030
		patrol,
		// Token: 0x040059F7 RID: 23031
		idle
	}
}
