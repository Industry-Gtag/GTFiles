using System;
using UnityEngine;

// Token: 0x02000229 RID: 553
public class RandomizeWavePhaseOffset : MonoBehaviour
{
	// Token: 0x06000EAC RID: 3756 RVA: 0x00050048 File Offset: 0x0004E248
	private void Start()
	{
		Material material = base.GetComponent<MeshRenderer>().material;
		UberShader.VertexWavePhaseOffset.SetValue<float>(material, Random.Range(this.minPhaseOffset, this.maxPhaseOffset));
	}

	// Token: 0x04001196 RID: 4502
	[SerializeField]
	private float minPhaseOffset;

	// Token: 0x04001197 RID: 4503
	[SerializeField]
	private float maxPhaseOffset;
}
