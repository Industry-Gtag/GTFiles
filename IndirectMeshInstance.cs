using System;
using UnityEngine;

// Token: 0x0200037B RID: 891
[DisallowMultipleComponent]
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public sealed class IndirectMeshInstance : MonoBehaviour
{
	// Token: 0x060015D8 RID: 5592 RVA: 0x000736E0 File Offset: 0x000718E0
	private void Awake()
	{
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshFilter = base.GetComponent<MeshFilter>();
	}

	// Token: 0x060015D9 RID: 5593 RVA: 0x000736FC File Offset: 0x000718FC
	private void OnEnable()
	{
		if (this._registered)
		{
			return;
		}
		this._registered = true;
		IndirectMeshGroup componentInParent = base.GetComponentInParent<IndirectMeshGroup>();
		IndirectMeshRenderer.Register(this, (componentInParent != null) ? componentInParent.GetInstanceID() : 0);
		if (this.dynamic)
		{
			this.meshRenderer.enabled = false;
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04001AA1 RID: 6817
	[Tooltip("When true, the transform is tracked and updated each frame instead of baked at registration time.")]
	[SerializeField]
	internal bool dynamic;

	// Token: 0x04001AA2 RID: 6818
	internal MeshRenderer meshRenderer;

	// Token: 0x04001AA3 RID: 6819
	internal MeshFilter meshFilter;

	// Token: 0x04001AA4 RID: 6820
	private bool _registered;
}
