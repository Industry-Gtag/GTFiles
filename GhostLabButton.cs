using System;
using UnityEngine;

// Token: 0x020001CA RID: 458
public class GhostLabButton : GorillaPressableButton, IBuildValidation
{
	// Token: 0x06000C21 RID: 3105 RVA: 0x000421A8 File Offset: 0x000403A8
	public bool BuildValidationCheck()
	{
		if (this.ghostLab == null)
		{
			Debug.LogError("ghostlab is missing", this);
			return false;
		}
		return true;
	}

	// Token: 0x06000C22 RID: 3106 RVA: 0x000421C6 File Offset: 0x000403C6
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.ghostLab.DoorButtonPress(this.buttonIndex, this.forSingleDoor);
	}

	// Token: 0x04000EC6 RID: 3782
	public GhostLab ghostLab;

	// Token: 0x04000EC7 RID: 3783
	public int buttonIndex;

	// Token: 0x04000EC8 RID: 3784
	public bool forSingleDoor;
}
