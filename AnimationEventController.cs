using System;
using UnityEngine;

// Token: 0x020001AC RID: 428
public class AnimationEventController : MonoBehaviour
{
	// Token: 0x06000B9F RID: 2975 RVA: 0x0003E988 File Offset: 0x0003CB88
	public void TriggerAttackVFX()
	{
		this.fxAttack.SetActive(false);
		this.fxAttack.SetActive(true);
	}

	// Token: 0x04000E07 RID: 3591
	public GameObject fxAttack;
}
