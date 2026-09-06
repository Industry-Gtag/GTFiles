using System;
using UnityEngine;

// Token: 0x02000D7C RID: 3452
[Serializable]
public struct AnimHashId
{
	// Token: 0x17000828 RID: 2088
	// (get) Token: 0x06005518 RID: 21784 RVA: 0x001BE345 File Offset: 0x001BC545
	public string text
	{
		get
		{
			return this._text;
		}
	}

	// Token: 0x17000829 RID: 2089
	// (get) Token: 0x06005519 RID: 21785 RVA: 0x001BE34D File Offset: 0x001BC54D
	public int hash
	{
		get
		{
			return this._hash;
		}
	}

	// Token: 0x0600551A RID: 21786 RVA: 0x001BE355 File Offset: 0x001BC555
	public AnimHashId(string text)
	{
		this._text = text;
		this._hash = Animator.StringToHash(text);
	}

	// Token: 0x0600551B RID: 21787 RVA: 0x001BE345 File Offset: 0x001BC545
	public override string ToString()
	{
		return this._text;
	}

	// Token: 0x0600551C RID: 21788 RVA: 0x001BE34D File Offset: 0x001BC54D
	public override int GetHashCode()
	{
		return this._hash;
	}

	// Token: 0x0600551D RID: 21789 RVA: 0x001BE34D File Offset: 0x001BC54D
	public static implicit operator int(AnimHashId h)
	{
		return h._hash;
	}

	// Token: 0x0600551E RID: 21790 RVA: 0x001BE36A File Offset: 0x001BC56A
	public static implicit operator AnimHashId(string s)
	{
		return new AnimHashId(s);
	}

	// Token: 0x040066C2 RID: 26306
	[SerializeField]
	private string _text;

	// Token: 0x040066C3 RID: 26307
	[NonSerialized]
	private int _hash;
}
