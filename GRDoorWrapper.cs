using System;
using UnityEngine;

// Token: 0x020003D5 RID: 981
public class GRDoorWrapper : MonoBehaviour
{
	// Token: 0x06001765 RID: 5989 RVA: 0x00087224 File Offset: 0x00085424
	public void ToggleDoor(bool value)
	{
		this.grDoor.SetDoorState(value ? GRDoor.DoorState.Open : GRDoor.DoorState.Closed);
	}

	// Token: 0x040022A6 RID: 8870
	[SerializeField]
	private GRDoor grDoor;
}
