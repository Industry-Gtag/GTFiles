using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200028B RID: 651
public class GrabbyTentacleController : MonoBehaviour
{
	// Token: 0x06001182 RID: 4482 RVA: 0x0005E284 File Offset: 0x0005C484
	private void OnEnable()
	{
		if (this.tentacles != null)
		{
			int num = this.tentacles.Length;
		}
		this.nextAttemptTimestamp = 0f;
		this.grabbedBefore.Clear();
		if (GrabbyTentacleNetworking.Instance != null)
		{
			GrabbyTentacleNetworking.Instance.Register(this);
			return;
		}
		Debug.LogError("[GrabbyTentacleController] No GrabbyTentacleNetworking.Instance at OnEnable — is the main scene loaded?");
	}

	// Token: 0x06001183 RID: 4483 RVA: 0x0005E2DB File Offset: 0x0005C4DB
	private void OnDisable()
	{
		if (GrabbyTentacleNetworking.Instance != null)
		{
			GrabbyTentacleNetworking.Instance.Unregister(this);
		}
	}

	// Token: 0x06001184 RID: 4484 RVA: 0x0005E2F8 File Offset: 0x0005C4F8
	private void Update()
	{
		if (PhotonNetwork.InRoom && (!PhotonNetwork.IsMasterClient || GrabbyTentacleNetworking.Instance == null))
		{
			return;
		}
		if (this.tentacles == null || this.tentacles.Length == 0 || this.grabRegion == null)
		{
			return;
		}
		int num = 0;
		while (num < this.tentacles.Length && this.nextAttemptTimestamp <= Time.time)
		{
			TentacleTracker tentacleTracker = this.tentacles[num];
			if (!(tentacleTracker == null) && !tentacleTracker.gameObject.activeSelf)
			{
				this.nextAttemptTimestamp = Time.time + Random.Range(this.minRetryDelay, this.maxRetryDelay);
				Player player = this.PickTarget();
				if (player != null)
				{
					this.grabbedBefore.Add(player.ActorNumber);
					if (PhotonNetwork.InRoom)
					{
						GrabbyTentacleNetworking.Instance.SendGrab(num, player);
					}
					else
					{
						this.OnGrabReceived(num, VRRig.LocalRig, true);
					}
				}
			}
			num++;
		}
	}

	// Token: 0x06001185 RID: 4485 RVA: 0x0005E3E4 File Offset: 0x0005C5E4
	private Player PickTarget()
	{
		this.candidateBuffer.Clear();
		this.freshCandidates.Clear();
		IReadOnlyList<VRRig> activeRigs = VRRigCache.ActiveRigs;
		for (int i = 0; i < activeRigs.Count; i++)
		{
			VRRig vrrig = activeRigs[i];
			if (!(vrrig == null) && vrrig.Creator != null && !vrrig.Creator.IsNull && !this.IsRigCurrentlyGrabbed(vrrig))
			{
				Vector3 vector = ((vrrig.head != null && vrrig.head.rigTarget != null) ? vrrig.head.rigTarget.position : vrrig.transform.position);
				if (!(this.grabRegion.ClosestPoint(vector) != vector))
				{
					this.candidateBuffer.Add(vrrig);
					if (!this.grabbedBefore.Contains(vrrig.Creator.ActorNumber))
					{
						this.freshCandidates.Add(vrrig);
					}
				}
			}
		}
		List<VRRig> list = ((this.freshCandidates.Count > 0) ? this.freshCandidates : this.candidateBuffer);
		if (list.Count == 0)
		{
			return null;
		}
		VRRig vrrig2 = list[Random.Range(0, list.Count)];
		Room currentRoom = PhotonNetwork.CurrentRoom;
		if (currentRoom == null)
		{
			return null;
		}
		return currentRoom.GetPlayer(vrrig2.Creator.ActorNumber, false);
	}

	// Token: 0x06001186 RID: 4486 RVA: 0x0005E544 File Offset: 0x0005C744
	private bool IsRigCurrentlyGrabbed(VRRig rig)
	{
		for (int i = 0; i < this.tentacles.Length; i++)
		{
			TentacleTracker tentacleTracker = this.tentacles[i];
			if (tentacleTracker != null && tentacleTracker.gameObject.activeSelf && tentacleTracker.currentTargetRig == rig)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001187 RID: 4487 RVA: 0x0005E594 File Offset: 0x0005C794
	public void OnGrabReceived(int tentacleIndex, VRRig targetRig, bool isLocalPlayer)
	{
		if (this.tentacles == null || tentacleIndex < 0 || tentacleIndex >= this.tentacles.Length)
		{
			return;
		}
		TentacleTracker tentacleTracker = this.tentacles[tentacleIndex];
		if (tentacleTracker == null)
		{
			return;
		}
		tentacleTracker.BeginGrab(targetRig, isLocalPlayer);
	}

	// Token: 0x040014E5 RID: 5349
	[SerializeField]
	private TentacleTracker[] tentacles;

	// Token: 0x040014E6 RID: 5350
	[SerializeField]
	private BoxCollider grabRegion;

	// Token: 0x040014E7 RID: 5351
	[SerializeField]
	private float minRetryDelay = 1f;

	// Token: 0x040014E8 RID: 5352
	[SerializeField]
	private float maxRetryDelay = 2f;

	// Token: 0x040014E9 RID: 5353
	private float nextAttemptTimestamp;

	// Token: 0x040014EA RID: 5354
	private readonly HashSet<int> grabbedBefore = new HashSet<int>();

	// Token: 0x040014EB RID: 5355
	private readonly List<VRRig> candidateBuffer = new List<VRRig>(16);

	// Token: 0x040014EC RID: 5356
	private readonly List<VRRig> freshCandidates = new List<VRRig>(16);
}
