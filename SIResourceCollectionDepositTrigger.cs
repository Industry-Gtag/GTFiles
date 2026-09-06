using System;
using UnityEngine;

// Token: 0x0200015D RID: 349
public class SIResourceCollectionDepositTrigger : MonoBehaviour
{
	// Token: 0x0600093F RID: 2367 RVA: 0x000320CE File Offset: 0x000302CE
	private void Awake()
	{
		this.resourceDeposit = this.parentCollection.GetComponent<ISIResourceDeposit>();
	}

	// Token: 0x06000940 RID: 2368 RVA: 0x000320E4 File Offset: 0x000302E4
	private void OnTriggerEnter(Collider other)
	{
		SIResource componentInParent = other.GetComponentInParent<SIResource>();
		if (componentInParent == null)
		{
			return;
		}
		if (componentInParent.CanDeposit())
		{
			this.resourceDeposit.ResourceDeposited(componentInParent);
		}
	}

	// Token: 0x04000B55 RID: 2901
	public GameObject parentCollection;

	// Token: 0x04000B56 RID: 2902
	private ISIResourceDeposit resourceDeposit;
}
