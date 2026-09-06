using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000D79 RID: 3449
[RequireComponent(typeof(TMP_Text))]
public class TypingTarget : GenericObservable
{
	// Token: 0x17000827 RID: 2087
	// (get) Token: 0x06005508 RID: 21768 RVA: 0x001BDFA5 File Offset: 0x001BC1A5
	public string Text
	{
		get
		{
			return this.value;
		}
	}

	// Token: 0x06005509 RID: 21769 RVA: 0x001BDFAD File Offset: 0x001BC1AD
	private void Awake()
	{
		this.tmp = base.GetComponent<TMP_Text>();
		this.tmp.text = " ";
		this.cursor.gameObject.SetActive(false);
	}

	// Token: 0x0600550A RID: 21770 RVA: 0x001BDFDC File Offset: 0x001BC1DC
	public void Append(string s)
	{
		if (this.value.Length == this.charLength)
		{
			return;
		}
		this.index++;
		List<char> list = new List<char>(this.value.ToCharArray());
		list.Insert(this.index, s[0]);
		this.value = new string(list.ToArray());
	}

	// Token: 0x0600550B RID: 21771 RVA: 0x001BE040 File Offset: 0x001BC240
	public void Delete()
	{
		if (this.value.Length == 0)
		{
			return;
		}
		List<char> list = new List<char>(this.value.ToCharArray());
		list.RemoveAt(this.index);
		this.value = new string(list.ToArray());
		this.index--;
	}

	// Token: 0x0600550C RID: 21772 RVA: 0x001BE097 File Offset: 0x001BC297
	public void Clear()
	{
		this.index = -1;
		this.value = string.Empty;
		this.cursor.gameObject.SetActive(false);
	}

	// Token: 0x0600550D RID: 21773 RVA: 0x001BE0BC File Offset: 0x001BC2BC
	public void MoveCursor(int i)
	{
		this.index = Mathf.Clamp(this.index + i, -1, this.value.Length - 1);
	}

	// Token: 0x0600550E RID: 21774 RVA: 0x001BE0DF File Offset: 0x001BC2DF
	protected override void OnLostObservable()
	{
		base.OnLostObservable();
		if (this.backupId.IsNullOrEmpty())
		{
			return;
		}
		PlayerPrefs.SetString("TypingTarget" + this.backupId, this.value);
		PlayerPrefs.Save();
		this.Clear();
	}

	// Token: 0x0600550F RID: 21775 RVA: 0x001BE11C File Offset: 0x001BC31C
	protected override void OnBecameObservable()
	{
		base.OnBecameObservable();
		if (this.backupId.IsNullOrEmpty())
		{
			return;
		}
		this.value = PlayerPrefs.GetString("TypingTarget" + this.backupId, string.Empty);
		this.index = this.value.Length - 1;
	}

	// Token: 0x06005510 RID: 21776 RVA: 0x001BE170 File Offset: 0x001BC370
	protected override void ObservableSliceUpdate()
	{
		if (this.tmp.text != this.value + " ")
		{
			this.tmp.text = this.value + " ";
		}
		if (this.index != this.pindex && this.index >= -1 && this.index + 1 < this.tmp.textInfo.characterCount)
		{
			this.cursor.gameObject.SetActive(true);
			this.cursor.localPosition = this.tmp.textInfo.characterInfo[this.index + 1].bottomLeft;
			this.pindex = this.index;
		}
	}

	// Token: 0x040066B5 RID: 26293
	private int index = -1;

	// Token: 0x040066B6 RID: 26294
	private int pindex = -1;

	// Token: 0x040066B7 RID: 26295
	private TMP_Text tmp;

	// Token: 0x040066B8 RID: 26296
	private string value;

	// Token: 0x040066B9 RID: 26297
	[SerializeField]
	private int charLength = 500;

	// Token: 0x040066BA RID: 26298
	[SerializeField]
	private Transform cursor;

	// Token: 0x040066BB RID: 26299
	[SerializeField]
	private string backupId;
}
