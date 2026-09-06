using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000906 RID: 2310
public class HoverboardAreaTrigger : MonoBehaviour
{
	// Token: 0x06003C96 RID: 15510 RVA: 0x0014AB87 File Offset: 0x00148D87
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			GTPlayer.Instance.AddHoverArea(this);
		}
	}

	// Token: 0x06003C97 RID: 15511 RVA: 0x0014ABA6 File Offset: 0x00148DA6
	private void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			GTPlayer.Instance.RemoveHoverArea(this);
		}
	}

	// Token: 0x06003C98 RID: 15512 RVA: 0x0014ABC5 File Offset: 0x00148DC5
	private void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		GTPlayer.Instance.RemoveHoverArea(this);
	}
}
