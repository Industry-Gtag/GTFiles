using System;
using ExitGames.Client.Photon;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000A42 RID: 2626
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class GorillaEnemyAI : MonoBehaviourPun, IPunObservable, IInRoomCallbacks
{
	// Token: 0x06004370 RID: 17264 RVA: 0x00166E04 File Offset: 0x00165004
	private void Start()
	{
		this.agent = base.GetComponent<NavMeshAgent>();
		this.r = base.GetComponent<Rigidbody>();
		this.r.useGravity = true;
		if (!base.photonView.IsMine)
		{
			this.agent.enabled = false;
			this.r.isKinematic = true;
		}
	}

	// Token: 0x06004371 RID: 17265 RVA: 0x00166E5C File Offset: 0x0016505C
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.eulerAngles);
			return;
		}
		Vector3 vector = (Vector3)stream.ReceiveNext();
		(ref this.targetPosition).SetValueSafe(in vector);
		vector = (Vector3)stream.ReceiveNext();
		(ref this.targetRotation).SetValueSafe(in vector);
	}

	// Token: 0x06004372 RID: 17266 RVA: 0x00166ED0 File Offset: 0x001650D0
	private void Update()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.FindClosestPlayer();
			if (this.playerTransform != null)
			{
				this.agent.destination = this.playerTransform.position;
			}
			base.transform.LookAt(new Vector3(this.playerTransform.transform.position.x, base.transform.position.y, this.playerTransform.position.z));
			this.r.linearVelocity *= 0.99f;
			return;
		}
		base.transform.position = Vector3.Lerp(base.transform.position, this.targetPosition, this.lerpValue);
		base.transform.eulerAngles = Vector3.Lerp(base.transform.eulerAngles, this.targetRotation, this.lerpValue);
	}

	// Token: 0x06004373 RID: 17267 RVA: 0x00166FC0 File Offset: 0x001651C0
	private void FindClosestPlayer()
	{
		VRRig[] array = Object.FindObjectsByType<VRRig>(FindObjectsSortMode.None);
		VRRig vrrig = null;
		float num = 100000f;
		foreach (VRRig vrrig2 in array)
		{
			Vector3 vector = vrrig2.transform.position - base.transform.position;
			if (vector.magnitude < num)
			{
				vrrig = vrrig2;
				num = vector.magnitude;
			}
		}
		this.playerTransform = vrrig.transform;
	}

	// Token: 0x06004374 RID: 17268 RVA: 0x00167032 File Offset: 0x00165232
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == 19)
		{
			PhotonNetwork.Destroy(base.photonView);
		}
	}

	// Token: 0x06004375 RID: 17269 RVA: 0x0016704E File Offset: 0x0016524E
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.agent.enabled = true;
			this.r.isKinematic = false;
		}
	}

	// Token: 0x06004376 RID: 17270 RVA: 0x00002C2D File Offset: 0x00000E2D
	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x06004377 RID: 17271 RVA: 0x00002C2D File Offset: 0x00000E2D
	void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
	{
	}

	// Token: 0x06004378 RID: 17272 RVA: 0x00002C2D File Offset: 0x00000E2D
	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06004379 RID: 17273 RVA: 0x00002C2D File Offset: 0x00000E2D
	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x04005550 RID: 21840
	public Transform playerTransform;

	// Token: 0x04005551 RID: 21841
	private NavMeshAgent agent;

	// Token: 0x04005552 RID: 21842
	private Rigidbody r;

	// Token: 0x04005553 RID: 21843
	private Vector3 targetPosition;

	// Token: 0x04005554 RID: 21844
	private Vector3 targetRotation;

	// Token: 0x04005555 RID: 21845
	public float lerpValue;
}
