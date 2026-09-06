using System;
using TMPro;
using UnityEngine;

// Token: 0x02000A0C RID: 2572
public class TextWatcherTMPro : MonoBehaviour
{
	// Token: 0x060041FF RID: 16895 RVA: 0x0015F876 File Offset: 0x0015DA76
	private void Start()
	{
		this.myText = base.GetComponent<TextMeshPro>();
		this.textToCopy.AddCallback(new Action<string>(this.OnTextChanged), true);
	}

	// Token: 0x06004200 RID: 16896 RVA: 0x0015F89C File Offset: 0x0015DA9C
	private void OnDestroy()
	{
		this.textToCopy.RemoveCallback(new Action<string>(this.OnTextChanged));
	}

	// Token: 0x06004201 RID: 16897 RVA: 0x0015F8B5 File Offset: 0x0015DAB5
	private void OnTextChanged(string newText)
	{
		this.myText.text = newText;
	}

	// Token: 0x040052C0 RID: 21184
	public WatchableStringSO textToCopy;

	// Token: 0x040052C1 RID: 21185
	private TextMeshPro myText;
}
