using System;
using TMPro;
using UnityEngine;

// Token: 0x020005A0 RID: 1440
public class RandomizeLabel : MonoBehaviour
{
	// Token: 0x0600247E RID: 9342 RVA: 0x000C3EDA File Offset: 0x000C20DA
	public void Randomize()
	{
		this.strings.distinct = this.distinct;
		this.label.text = this.strings.NextItem();
	}

	// Token: 0x04002FDD RID: 12253
	public TMP_Text label;

	// Token: 0x04002FDE RID: 12254
	public RandomStrings strings;

	// Token: 0x04002FDF RID: 12255
	public bool distinct;
}
