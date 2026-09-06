using System;
using System.Collections.Generic;
using GorillaNetworking;
using GTMathUtil;
using Unity.Profiling;
using UnityEngine;

// Token: 0x02000CC9 RID: 3273
public class GorillaFriendCollider : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06005127 RID: 20775 RVA: 0x001AE932 File Offset: 0x001ACB32
	private void Awake()
	{
		this.thisCapsule = base.GetComponent<CapsuleCollider>();
		this.thisBox = base.GetComponent<BoxCollider>();
		if (!GorillaFriendCollider.updateAdded)
		{
			GorillaFriendCollider.updateAdded = true;
			VRRigCache.OnActiveRigsChanged += GorillaFriendCollider.UpdateActiveRigs;
			GorillaFriendCollider.UpdateActiveRigs();
		}
	}

	// Token: 0x06005128 RID: 20776 RVA: 0x001AE96F File Offset: 0x001ACB6F
	private static void UpdateActiveRigs()
	{
		VRRigCache.Instance.GetActiveRigs(GorillaFriendCollider.playerRigs);
	}

	// Token: 0x06005129 RID: 20777 RVA: 0x00019260 File Offset: 0x00017460
	private void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600512A RID: 20778 RVA: 0x00019269 File Offset: 0x00017469
	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600512B RID: 20779 RVA: 0x001AE980 File Offset: 0x001ACB80
	public void RegisterUI(JoinTriggerUI joinUI)
	{
		this.ui = joinUI;
	}

	// Token: 0x0600512C RID: 20780 RVA: 0x001AE989 File Offset: 0x001ACB89
	public void UnregisterUI()
	{
		this.ui = null;
	}

	// Token: 0x0600512D RID: 20781 RVA: 0x001AE992 File Offset: 0x001ACB92
	private void AddUserID(in string userID)
	{
		if (this.playerIDsCurrentlyTouching.Contains(userID))
		{
			return;
		}
		this.playerIDsCurrentlyTouching.Add(userID);
	}

	// Token: 0x0600512E RID: 20782 RVA: 0x001AE9B4 File Offset: 0x001ACBB4
	public void SliceUpdate()
	{
		using (GorillaFriendCollider.profiler_SliceUpdate.Auto())
		{
			if (NetworkSystem.Instance.InRoom || this.runCheckWhileNotInRoom)
			{
				this.RefreshPlayersWithinBounds();
			}
		}
	}

	// Token: 0x0600512F RID: 20783 RVA: 0x001AEA0C File Offset: 0x001ACC0C
	public void RefreshPlayersWithinBounds()
	{
		int count = this.playerIDsCurrentlyTouching.Count;
		this.playerIDsCurrentlyTouching.Clear();
		NetPlayer localPlayer = NetworkSystem.Instance.LocalPlayer;
		if (localPlayer == null)
		{
			return;
		}
		bool flag = this.thisBox != null;
		bool flag2 = this.thisCapsule != null;
		for (int i = 0; i < GorillaFriendCollider.playerRigs.Count; i++)
		{
			float y = GorillaFriendCollider.playerRigs[i].bodyTransform.transform.position.y;
			if ((!this.applyCapsuleYLimits || (y >= this.capsuleColliderYLimits.x && y <= this.capsuleColliderYLimits.y)) && ((flag && WithinBounds.PointWithinBoxColliderBounds(GorillaFriendCollider.playerRigs[i].rigContainer.SpeakerHead.position, this.thisBox)) || (!flag && flag2 && WithinBounds.PointWithinCapsuleColliderBounds(GorillaFriendCollider.playerRigs[i].rigContainer.SpeakerHead.position, this.thisCapsule))))
			{
				this.playerIDsCurrentlyTouching.Add(GorillaFriendCollider.playerRigs[i].isLocal ? localPlayer.UserId : GorillaFriendCollider.playerRigs[i].creator.UserId);
			}
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if (this.playerIDsCurrentlyTouching.Contains(localPlayer.UserId) && GorillaComputer.instance.friendJoinCollider != this)
		{
			GorillaComputer.instance.allowedMapsToJoin = this.myAllowedMapsToJoin;
			GorillaComputer.instance.friendJoinCollider = this;
			GorillaComputer.instance.UpdateScreen();
		}
		if (this.updatePartyZoneCallbacks && count != this.playerIDsCurrentlyTouching.Count && this.ui != null)
		{
			this.ui.TriggerUpdateUI();
		}
	}

	// Token: 0x04006325 RID: 25381
	public List<string> playerIDsCurrentlyTouching = new List<string>();

	// Token: 0x04006326 RID: 25382
	private CapsuleCollider thisCapsule;

	// Token: 0x04006327 RID: 25383
	private BoxCollider thisBox;

	// Token: 0x04006328 RID: 25384
	[Tooltip("If using a capsule collider, the player position can be checked against these minimum and maximum Y limits (world position) to make it behave more like a cylinder check")]
	public bool applyCapsuleYLimits;

	// Token: 0x04006329 RID: 25385
	[Tooltip("If the player's Y world position is lower than Limits.x or higher than Limits.y, they will not be considered \"Inside\" the friend collider")]
	public Vector2 capsuleColliderYLimits = Vector2.zero;

	// Token: 0x0400632A RID: 25386
	public bool runCheckWhileNotInRoom;

	// Token: 0x0400632B RID: 25387
	public string[] myAllowedMapsToJoin;

	// Token: 0x0400632C RID: 25388
	private readonly Collider[] overlapColliders = new Collider[20];

	// Token: 0x0400632D RID: 25389
	public bool manualRefreshOnly;

	// Token: 0x0400632E RID: 25390
	[Tooltip("If true, then when the number of players in the collider changes call the zone callbacks.")]
	public bool updatePartyZoneCallbacks;

	// Token: 0x0400632F RID: 25391
	private JoinTriggerUI ui;

	// Token: 0x04006330 RID: 25392
	private float _nextUpdateTime = -1f;

	// Token: 0x04006331 RID: 25393
	private static List<VRRig> playerRigs = new List<VRRig>();

	// Token: 0x04006332 RID: 25394
	private static bool updateAdded = false;

	// Token: 0x04006333 RID: 25395
	private static readonly ProfilerMarker profiler_SliceUpdate = new ProfilerMarker("GT/FriendCollider.SliceUpdate");
}
