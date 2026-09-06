using System;
using UnityEngine;

// Token: 0x020005F3 RID: 1523
public class AnimatorReset : MonoBehaviour
{
	// Token: 0x060025ED RID: 9709 RVA: 0x000C90B3 File Offset: 0x000C72B3
	public void Reset()
	{
		if (!this.target)
		{
			return;
		}
		this.target.Rebind();
		this.target.Update(0f);
	}

	// Token: 0x060025EE RID: 9710 RVA: 0x000C90DE File Offset: 0x000C72DE
	private void OnEnable()
	{
		if (this.onEnable)
		{
			this.Reset();
		}
	}

	// Token: 0x060025EF RID: 9711 RVA: 0x000C90EE File Offset: 0x000C72EE
	private void OnDisable()
	{
		if (this.onDisable)
		{
			this.Reset();
		}
	}

	// Token: 0x04003183 RID: 12675
	public Animator target;

	// Token: 0x04003184 RID: 12676
	public bool onEnable;

	// Token: 0x04003185 RID: 12677
	public bool onDisable = true;
}
