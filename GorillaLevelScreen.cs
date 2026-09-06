using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000891 RID: 2193
public class GorillaLevelScreen : MonoBehaviour
{
	// Token: 0x0600393C RID: 14652 RVA: 0x001382B5 File Offset: 0x001364B5
	private void Awake()
	{
		if (this.myText != null)
		{
			this.startingText = this.myText.text;
		}
	}

	// Token: 0x0600393D RID: 14653 RVA: 0x001382D8 File Offset: 0x001364D8
	public void UpdateText(string newText, bool setToGoodMaterial)
	{
		if (this.myText != null)
		{
			this.myText.text = newText;
		}
		Material[] materials = base.GetComponent<MeshRenderer>().materials;
		materials[0] = (setToGoodMaterial ? this.goodMaterial : this.badMaterial);
		base.GetComponent<MeshRenderer>().materials = materials;
	}

	// Token: 0x0400494F RID: 18767
	public string startingText;

	// Token: 0x04004950 RID: 18768
	public Material goodMaterial;

	// Token: 0x04004951 RID: 18769
	public Material badMaterial;

	// Token: 0x04004952 RID: 18770
	public Text myText;
}
