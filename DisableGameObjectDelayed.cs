using System;
using UnityEngine;

// Token: 0x02000DA4 RID: 3492
public class DisableGameObjectDelayed : MonoBehaviour
{
	// Token: 0x060055DF RID: 21983 RVA: 0x001C1B41 File Offset: 0x001BFD41
	private void OnEnable()
	{
		this.enabledTime = Time.time;
	}

	// Token: 0x060055E0 RID: 21984 RVA: 0x001C1B4E File Offset: 0x001BFD4E
	private void Update()
	{
		if (Time.time > this.enabledTime + this.delayTime)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x060055E1 RID: 21985 RVA: 0x001C1B70 File Offset: 0x001BFD70
	public void EnableAndResetTimer()
	{
		base.gameObject.SetActive(true);
		this.OnEnable();
	}

	// Token: 0x04006743 RID: 26435
	public float delayTime = 1f;

	// Token: 0x04006744 RID: 26436
	public float enabledTime;
}
