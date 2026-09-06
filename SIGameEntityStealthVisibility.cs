using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200011C RID: 284
public class SIGameEntityStealthVisibility : MonoBehaviour
{
	// Token: 0x0600071D RID: 1821 RVA: 0x00028BE2 File Offset: 0x00026DE2
	private void OnEnable()
	{
		this.revealRange = Mathf.Min(this.revealRange, this.hideRange);
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x00028BFB File Offset: 0x00026DFB
	private void OnDisable()
	{
		this.SetVisibility(true);
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x00028C04 File Offset: 0x00026E04
	private void LateUpdate()
	{
		Vector3 position = GTPlayer.Instance.transform.position;
		float num = Vector3.SqrMagnitude(base.transform.position - position);
		if (this.isStealthed && num < this.revealRange * this.revealRange)
		{
			this.SetVisibility(true);
			return;
		}
		if (!this.isStealthed && num > this.hideRange * this.hideRange)
		{
			this.SetVisibility(false);
		}
	}

	// Token: 0x06000720 RID: 1824 RVA: 0x00028C78 File Offset: 0x00026E78
	private void SetVisibility(bool visible)
	{
		this.isStealthed = !visible;
		for (int i = 0; i < this.stealthedComponents.Length; i++)
		{
			this.stealthedComponents[i].enabled = visible;
		}
	}

	// Token: 0x040008FE RID: 2302
	[SerializeField]
	private Renderer[] stealthedComponents;

	// Token: 0x040008FF RID: 2303
	[SerializeField]
	private float revealRange = 5f;

	// Token: 0x04000900 RID: 2304
	[SerializeField]
	private float hideRange = 8f;

	// Token: 0x04000901 RID: 2305
	private bool isStealthed;
}
