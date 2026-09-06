using System;
using UnityEngine;

// Token: 0x020007B2 RID: 1970
public class GREntityDestroyTrigger : MonoBehaviour
{
	// Token: 0x06003274 RID: 12916 RVA: 0x00114864 File Offset: 0x00112A64
	private void OnTriggerEnter(Collider other)
	{
		GameEntity component = other.attachedRigidbody.GetComponent<GameEntity>();
		if (component != null && component.IsAuthority())
		{
			component.manager.RequestDestroyItem(component.id);
		}
	}
}
