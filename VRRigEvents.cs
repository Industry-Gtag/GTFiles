using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020004FD RID: 1277
[RequireComponent(typeof(RigContainer))]
public class VRRigEvents : MonoBehaviour, IPreDisable
{
	// Token: 0x06002006 RID: 8198 RVA: 0x000AC87A File Offset: 0x000AAA7A
	public void PreDisable()
	{
		DelegateListProcessor<RigContainer> delegateListProcessor = this.disableEvent;
		if (delegateListProcessor == null)
		{
			return;
		}
		delegateListProcessor.InvokeSafe(in this.rigRef);
	}

	// Token: 0x06002007 RID: 8199 RVA: 0x000AC892 File Offset: 0x000AAA92
	public void SendPostEnableEvent()
	{
		DelegateListProcessor<RigContainer> delegateListProcessor = this.enableEvent;
		if (delegateListProcessor == null)
		{
			return;
		}
		delegateListProcessor.InvokeSafe(in this.rigRef);
	}

	// Token: 0x04002ACD RID: 10957
	[SerializeField]
	private RigContainer rigRef;

	// Token: 0x04002ACE RID: 10958
	public DelegateListProcessor<RigContainer> disableEvent = new DelegateListProcessor<RigContainer>(5);

	// Token: 0x04002ACF RID: 10959
	public DelegateListProcessor<RigContainer> enableEvent = new DelegateListProcessor<RigContainer>(5);
}
