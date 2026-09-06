using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000A0B RID: 2571
public class TextWatcher : MonoBehaviour
{
	// Token: 0x060041FB RID: 16891 RVA: 0x0015F829 File Offset: 0x0015DA29
	private void Start()
	{
		this.myText = base.GetComponent<Text>();
		this.textToCopy.AddCallback(new Action<string>(this.OnTextChanged), true);
	}

	// Token: 0x060041FC RID: 16892 RVA: 0x0015F84F File Offset: 0x0015DA4F
	private void OnDestroy()
	{
		this.textToCopy.RemoveCallback(new Action<string>(this.OnTextChanged));
	}

	// Token: 0x060041FD RID: 16893 RVA: 0x0015F868 File Offset: 0x0015DA68
	private void OnTextChanged(string newText)
	{
		this.myText.text = newText;
	}

	// Token: 0x040052BE RID: 21182
	public WatchableStringSO textToCopy;

	// Token: 0x040052BF RID: 21183
	private Text myText;
}
