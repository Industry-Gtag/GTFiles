using System;
using UnityEngine;

// Token: 0x02000D48 RID: 3400
public class RoomMuteEnforcer : MonoBehaviour
{
	// Token: 0x06005443 RID: 21571 RVA: 0x001BBFF4 File Offset: 0x001BA1F4
	private void OnEnable()
	{
		RoomControls.OnPlayerMuteChanged += new Action<string, bool>(this.SetRoomMute);
		VRRigCache.OnRigActivated += this.SyncRoomMute;
		RoomControls.OnRoomStateLoaded += new Action(this.SyncAllRoomMutes);
	}

	// Token: 0x06005444 RID: 21572 RVA: 0x001BC048 File Offset: 0x001BA248
	private void OnDisable()
	{
		RoomControls.OnPlayerMuteChanged -= new Action<string, bool>(this.SetRoomMute);
		VRRigCache.OnRigActivated -= this.SyncRoomMute;
		RoomControls.OnRoomStateLoaded -= new Action(this.SyncAllRoomMutes);
	}

	// Token: 0x06005445 RID: 21573 RVA: 0x001BC09C File Offset: 0x001BA29C
	private void SetRoomMute(string userId, bool muted)
	{
		foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
		{
			NetPlayer creator = rigContainer.Creator;
			if (((creator != null) ? creator.UserId : null) == userId)
			{
				rigContainer.SetMuted(RigContainer.MuteReason.Room, muted);
				break;
			}
		}
	}

	// Token: 0x06005446 RID: 21574 RVA: 0x001BC108 File Offset: 0x001BA308
	private void SyncRoomMute(RigContainer rig)
	{
		if (rig.Creator != null)
		{
			rig.SetMuted(RigContainer.MuteReason.Room, RoomControls.MutedPlayers.ContainsKey(rig.Creator.UserId));
		}
	}

	// Token: 0x06005447 RID: 21575 RVA: 0x001BC130 File Offset: 0x001BA330
	private void SyncAllRoomMutes()
	{
		foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
		{
			this.SyncRoomMute(rigContainer);
		}
	}
}
