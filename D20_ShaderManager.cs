using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200022A RID: 554
public class D20_ShaderManager : MonoBehaviour
{
	// Token: 0x06000EAE RID: 3758 RVA: 0x00050080 File Offset: 0x0004E280
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.lastPosition = base.transform.position;
		Renderer component = base.GetComponent<Renderer>();
		this.material = component.material;
		this.material.SetVector("_Velocity", this.velocity);
		base.StartCoroutine(this.UpdateVelocityCoroutine());
	}

	// Token: 0x06000EAF RID: 3759 RVA: 0x000500E5 File Offset: 0x0004E2E5
	private IEnumerator UpdateVelocityCoroutine()
	{
		for (;;)
		{
			Vector3 position = base.transform.position;
			this.velocity = (position - this.lastPosition) / this.updateInterval;
			this.lastPosition = position;
			this.material.SetVector("_Velocity", this.velocity);
			yield return new WaitForSeconds(this.updateInterval);
		}
		yield break;
	}

	// Token: 0x04001198 RID: 4504
	private Rigidbody rb;

	// Token: 0x04001199 RID: 4505
	private Vector3 lastPosition;

	// Token: 0x0400119A RID: 4506
	public float updateInterval = 0.1f;

	// Token: 0x0400119B RID: 4507
	public Vector3 velocity;

	// Token: 0x0400119C RID: 4508
	private Material material;
}
