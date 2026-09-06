using System;
using UnityEngine;

// Token: 0x02000208 RID: 520
public class SnowballKnockbackEnabler : MonoBehaviour
{
	// Token: 0x06000DAC RID: 3500 RVA: 0x0004AFE9 File Offset: 0x000491E9
	private void OnEnable()
	{
		GrowingSnowballThrowable.NotifyEnableKnockbackIntent(this);
	}

	// Token: 0x06000DAD RID: 3501 RVA: 0x0004AFF1 File Offset: 0x000491F1
	private void OnDisable()
	{
		GrowingSnowballThrowable.NotifyDisableKnockbackIntent(this);
	}
}
