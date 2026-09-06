using System;
using UnityEngine;

// Token: 0x020001C6 RID: 454
public class DoorSlidingOpenAudio : MonoBehaviour, IBuildValidation, ITickSystemTick
{
	// Token: 0x17000126 RID: 294
	// (get) Token: 0x06000C0E RID: 3086 RVA: 0x00041CE2 File Offset: 0x0003FEE2
	// (set) Token: 0x06000C0F RID: 3087 RVA: 0x00041CEA File Offset: 0x0003FEEA
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x06000C10 RID: 3088 RVA: 0x00041CF3 File Offset: 0x0003FEF3
	private void OnEnable()
	{
		TickSystem<object>.AddCallbackTarget(this);
	}

	// Token: 0x06000C11 RID: 3089 RVA: 0x00041CFB File Offset: 0x0003FEFB
	private void OnDisable()
	{
		TickSystem<object>.RemoveCallbackTarget(this);
	}

	// Token: 0x06000C12 RID: 3090 RVA: 0x00041D04 File Offset: 0x0003FF04
	public bool BuildValidationCheck()
	{
		if (this.button == null)
		{
			Debug.LogError("reference button missing for doorslidingopenaudio", base.gameObject);
			return false;
		}
		if (this.audioSource == null)
		{
			Debug.LogError("missing audio source on doorslidingopenaudio", base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x06000C13 RID: 3091 RVA: 0x00041D54 File Offset: 0x0003FF54
	void ITickSystemTick.Tick()
	{
		if (this.button.ghostLab.IsDoorMoving(this.button.forSingleDoor, this.button.buttonIndex))
		{
			if (!this.audioSource.isPlaying)
			{
				this.audioSource.time = 0f;
				this.audioSource.GTPlay();
				return;
			}
		}
		else if (this.audioSource.isPlaying)
		{
			this.audioSource.time = 0f;
			this.audioSource.GTStop();
		}
	}

	// Token: 0x04000EB5 RID: 3765
	public GhostLabButton button;

	// Token: 0x04000EB6 RID: 3766
	public AudioSource audioSource;
}
