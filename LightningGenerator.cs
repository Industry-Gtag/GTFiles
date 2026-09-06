using System;
using UnityEngine;

// Token: 0x02000E3E RID: 3646
public class LightningGenerator : MonoBehaviour
{
	// Token: 0x06005916 RID: 22806 RVA: 0x001CF2DC File Offset: 0x001CD4DC
	private void Awake()
	{
		this.strikes = new LightningStrike[this.maxConcurrentStrikes];
		for (int i = 0; i < this.strikes.Length; i++)
		{
			if (i == 0)
			{
				this.strikes[i] = this.prototype;
			}
			else
			{
				this.strikes[i] = Object.Instantiate<LightningStrike>(this.prototype, base.transform);
			}
			this.strikes[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x06005917 RID: 22807 RVA: 0x001CF34C File Offset: 0x001CD54C
	private void OnEnable()
	{
		LightningDispatcher.RequestLightningStrike += this.LightningDispatcher_RequestLightningStrike;
	}

	// Token: 0x06005918 RID: 22808 RVA: 0x001CF35F File Offset: 0x001CD55F
	private void OnDisable()
	{
		LightningDispatcher.RequestLightningStrike -= this.LightningDispatcher_RequestLightningStrike;
	}

	// Token: 0x06005919 RID: 22809 RVA: 0x001CF372 File Offset: 0x001CD572
	private LightningStrike LightningDispatcher_RequestLightningStrike(Vector3 t1, Vector3 t2)
	{
		this.index = (this.index + 1) % this.strikes.Length;
		return this.strikes[this.index];
	}

	// Token: 0x04006943 RID: 26947
	[SerializeField]
	private uint maxConcurrentStrikes = 10U;

	// Token: 0x04006944 RID: 26948
	[SerializeField]
	private LightningStrike prototype;

	// Token: 0x04006945 RID: 26949
	private LightningStrike[] strikes;

	// Token: 0x04006946 RID: 26950
	private int index;
}
