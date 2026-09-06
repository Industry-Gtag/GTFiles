using System;
using UnityEngine;

// Token: 0x02000248 RID: 584
public class MonkeVoteProximityTrigger : GorillaTriggerBox
{
	// Token: 0x14000021 RID: 33
	// (add) Token: 0x06000FA7 RID: 4007 RVA: 0x0005540C File Offset: 0x0005360C
	// (remove) Token: 0x06000FA8 RID: 4008 RVA: 0x00055444 File Offset: 0x00053644
	public event Action OnEnter;

	// Token: 0x1700018C RID: 396
	// (get) Token: 0x06000FA9 RID: 4009 RVA: 0x00055479 File Offset: 0x00053679
	// (set) Token: 0x06000FAA RID: 4010 RVA: 0x00055481 File Offset: 0x00053681
	public bool isPlayerNearby { get; private set; }

	// Token: 0x06000FAB RID: 4011 RVA: 0x0005548A File Offset: 0x0005368A
	public override void OnBoxTriggered()
	{
		this.isPlayerNearby = true;
		if (this.triggerTime + this.retriggerDelay < Time.unscaledTime)
		{
			this.triggerTime = Time.unscaledTime;
			Action onEnter = this.OnEnter;
			if (onEnter == null)
			{
				return;
			}
			onEnter();
		}
	}

	// Token: 0x06000FAC RID: 4012 RVA: 0x000554C2 File Offset: 0x000536C2
	public override void OnBoxExited()
	{
		this.isPlayerNearby = false;
	}

	// Token: 0x040012DD RID: 4829
	private float triggerTime = float.MinValue;

	// Token: 0x040012DE RID: 4830
	private float retriggerDelay = 0.25f;
}
