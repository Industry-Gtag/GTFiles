using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020009DA RID: 2522
public class RadialMaterialFade : MonoBehaviour
{
	// Token: 0x060040B5 RID: 16565 RVA: 0x00158AD0 File Offset: 0x00156CD0
	private void Update()
	{
		if (this.material == null || this.target == null)
		{
			return;
		}
		Camera mainCamera = GTPlayer.Instance.mainCamera;
		if (mainCamera == null)
		{
			return;
		}
		float num = Vector3.Distance(mainCamera.transform.position, this.target.position);
		float num2 = Mathf.InverseLerp(this.minDistance, this.maxDistance, num);
		float num3 = Mathf.Lerp(this.alphaAtMinDistance, this.alphaAtMaxDistance, num2);
		Color color = this.material.GetColor(RadialMaterialFade.colorID);
		color.a = num3;
		this.material.SetColor(RadialMaterialFade.colorID, color);
	}

	// Token: 0x0400513C RID: 20796
	[SerializeField]
	private Material material;

	// Token: 0x0400513D RID: 20797
	[SerializeField]
	private Transform target;

	// Token: 0x0400513E RID: 20798
	[Header("Distance")]
	[SerializeField]
	private float minDistance = 1f;

	// Token: 0x0400513F RID: 20799
	[SerializeField]
	private float maxDistance = 10f;

	// Token: 0x04005140 RID: 20800
	[Header("Alpha")]
	[SerializeField]
	[Range(0f, 1f)]
	private float alphaAtMinDistance;

	// Token: 0x04005141 RID: 20801
	[SerializeField]
	[Range(0f, 1f)]
	private float alphaAtMaxDistance = 1f;

	// Token: 0x04005142 RID: 20802
	private static readonly int colorID = Shader.PropertyToID("_Color");
}
