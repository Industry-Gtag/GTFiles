using System;
using UnityEngine;

// Token: 0x0200039B RID: 923
public class RoomStateVisibility : MonoBehaviour
{
	// Token: 0x06001678 RID: 5752 RVA: 0x00082847 File Offset: 0x00080A47
	private void Start()
	{
		this.OnRoomChanged();
		RoomSystem.JoinedRoomEvent += new Action(this.OnRoomChanged);
		RoomSystem.LeftRoomEvent += new Action(this.OnRoomChanged);
	}

	// Token: 0x06001679 RID: 5753 RVA: 0x00082885 File Offset: 0x00080A85
	private void OnDestroy()
	{
		RoomSystem.JoinedRoomEvent -= new Action(this.OnRoomChanged);
		RoomSystem.LeftRoomEvent -= new Action(this.OnRoomChanged);
	}

	// Token: 0x0600167A RID: 5754 RVA: 0x000828C0 File Offset: 0x00080AC0
	private void OnRoomChanged()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			base.gameObject.SetActive(this.enableOutOfRoom);
			return;
		}
		if (NetworkSystem.Instance.SessionIsPrivate)
		{
			base.gameObject.SetActive(this.enableInPrivateRoom);
			return;
		}
		base.gameObject.SetActive(this.enableInRoom);
	}

	// Token: 0x04002097 RID: 8343
	[SerializeField]
	private bool enableOutOfRoom;

	// Token: 0x04002098 RID: 8344
	[SerializeField]
	private bool enableInRoom = true;

	// Token: 0x04002099 RID: 8345
	[SerializeField]
	private bool enableInPrivateRoom = true;
}
