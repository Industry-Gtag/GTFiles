using System;
using UnityEngine;

// Token: 0x0200061A RID: 1562
public class BuilderPaintBucket : MonoBehaviour
{
	// Token: 0x0600270B RID: 9995 RVA: 0x000CED10 File Offset: 0x000CCF10
	private void Awake()
	{
		if (string.IsNullOrEmpty(this.materialId))
		{
			return;
		}
		this.materialType = this.materialId.GetHashCode();
		if (this.bucketMaterialOptions != null && this.paintBucketRenderer != null)
		{
			Material material;
			int num;
			this.bucketMaterialOptions.GetMaterialFromType(this.materialType, out material, out num);
			if (material != null)
			{
				this.paintBucketRenderer.material = material;
			}
		}
	}

	// Token: 0x0600270C RID: 9996 RVA: 0x000CED84 File Offset: 0x000CCF84
	private void OnTriggerEnter(Collider other)
	{
		if (this.materialType == -1)
		{
			return;
		}
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			BuilderPaintBrush component = attachedRigidbody.GetComponent<BuilderPaintBrush>();
			if (component != null)
			{
				component.SetBrushMaterial(this.materialType);
			}
		}
	}

	// Token: 0x04003293 RID: 12947
	[SerializeField]
	private BuilderMaterialOptions bucketMaterialOptions;

	// Token: 0x04003294 RID: 12948
	[SerializeField]
	private MeshRenderer paintBucketRenderer;

	// Token: 0x04003295 RID: 12949
	[SerializeField]
	private string materialId;

	// Token: 0x04003296 RID: 12950
	private int materialType = -1;
}
