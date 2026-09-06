using System;
using UnityEngine;

// Token: 0x020007F6 RID: 2038
[Serializable]
public class GRDoor
{
	// Token: 0x0600340D RID: 13325 RVA: 0x0011E27F File Offset: 0x0011C47F
	public void Setup()
	{
		this.doorState = GRDoor.DoorState.Closed;
	}

	// Token: 0x0600340E RID: 13326 RVA: 0x0011E288 File Offset: 0x0011C488
	public void SetDoorState(GRDoor.DoorState newState)
	{
		if (newState == this.doorState)
		{
			return;
		}
		this.doorState = newState;
		if (this.doorState == GRDoor.DoorState.Closed)
		{
			this.animation.clip = this.closeAnim;
			this.animation.Play();
			this.closeDoorSound.Play(null);
			return;
		}
		this.animation.clip = this.openAnim;
		this.animation.Play();
		this.openDoorSound.Play(null);
	}

	// Token: 0x040043D3 RID: 17363
	public GRDoor.DoorState doorState;

	// Token: 0x040043D4 RID: 17364
	public Animation animation;

	// Token: 0x040043D5 RID: 17365
	public AnimationClip openAnim;

	// Token: 0x040043D6 RID: 17366
	public AnimationClip closeAnim;

	// Token: 0x040043D7 RID: 17367
	public AbilitySound openDoorSound;

	// Token: 0x040043D8 RID: 17368
	public AbilitySound closeDoorSound;

	// Token: 0x020007F7 RID: 2039
	public enum DoorState
	{
		// Token: 0x040043DA RID: 17370
		Closed,
		// Token: 0x040043DB RID: 17371
		Open
	}
}
