using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000082 RID: 130
public class CritterVisuals : MonoBehaviour
{
	// Token: 0x1700003B RID: 59
	// (get) Token: 0x0600032E RID: 814 RVA: 0x000134DF File Offset: 0x000116DF
	public CritterAppearance Appearance
	{
		get
		{
			return this._appearance;
		}
	}

	// Token: 0x0600032F RID: 815 RVA: 0x000134E8 File Offset: 0x000116E8
	public void SetAppearance(CritterAppearance appearance)
	{
		this._appearance = appearance;
		float num = this._appearance.size.ClampSafe(0.25f, 1.5f);
		this.bodyRoot.localScale = new Vector3(num, num, num);
		if (!string.IsNullOrEmpty(appearance.hatName))
		{
			foreach (GameObject gameObject in this.hats)
			{
				gameObject.SetActive(gameObject.name == this._appearance.hatName);
			}
			this.hatRoot.gameObject.SetActive(true);
			return;
		}
		this.hatRoot.gameObject.SetActive(false);
	}

	// Token: 0x06000330 RID: 816 RVA: 0x0001358D File Offset: 0x0001178D
	public void ApplyMesh(Mesh newMesh)
	{
		this.myMeshFilter.sharedMesh = newMesh;
	}

	// Token: 0x06000331 RID: 817 RVA: 0x0001359B File Offset: 0x0001179B
	public void ApplyMaterial(Material mat)
	{
		this.myRenderer.sharedMaterial = mat;
	}

	// Token: 0x040003C4 RID: 964
	public int critterType;

	// Token: 0x040003C5 RID: 965
	[Header("Visuals")]
	public Transform bodyRoot;

	// Token: 0x040003C6 RID: 966
	public MeshRenderer myRenderer;

	// Token: 0x040003C7 RID: 967
	public MeshFilter myMeshFilter;

	// Token: 0x040003C8 RID: 968
	public Transform hatRoot;

	// Token: 0x040003C9 RID: 969
	public GameObject[] hats;

	// Token: 0x040003CA RID: 970
	private CritterAppearance _appearance;
}
