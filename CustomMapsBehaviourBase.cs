using System;
using UnityEngine;

// Token: 0x02000A73 RID: 2675
public abstract class CustomMapsBehaviourBase
{
	// Token: 0x060044DA RID: 17626
	public abstract bool CanExecute();

	// Token: 0x060044DB RID: 17627
	public abstract void Execute();

	// Token: 0x060044DC RID: 17628
	public abstract void NetExecute();

	// Token: 0x060044DD RID: 17629
	public abstract void ResetBehavior();

	// Token: 0x060044DE RID: 17630
	public abstract bool CanContinueExecuting();

	// Token: 0x060044DF RID: 17631
	public abstract void OnTriggerEnter(Collider otherCollider);
}
