using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000DD5 RID: 3541
[Serializable]
public struct ShaderHashId : IEquatable<ShaderHashId>
{
	// Token: 0x17000848 RID: 2120
	// (get) Token: 0x060056B8 RID: 22200 RVA: 0x001C47BB File Offset: 0x001C29BB
	public string text
	{
		get
		{
			return this._text;
		}
	}

	// Token: 0x17000849 RID: 2121
	// (get) Token: 0x060056B9 RID: 22201 RVA: 0x001C47C3 File Offset: 0x001C29C3
	public int hash
	{
		get
		{
			return this._hash;
		}
	}

	// Token: 0x060056BA RID: 22202 RVA: 0x001C47CB File Offset: 0x001C29CB
	public ShaderHashId(string text)
	{
		this._text = text;
		this._hash = Shader.PropertyToID(text);
	}

	// Token: 0x060056BB RID: 22203 RVA: 0x001C47BB File Offset: 0x001C29BB
	public override string ToString()
	{
		return this._text;
	}

	// Token: 0x060056BC RID: 22204 RVA: 0x001C47C3 File Offset: 0x001C29C3
	public override int GetHashCode()
	{
		return this._hash;
	}

	// Token: 0x060056BD RID: 22205 RVA: 0x001C47C3 File Offset: 0x001C29C3
	public static implicit operator int(ShaderHashId h)
	{
		return h._hash;
	}

	// Token: 0x060056BE RID: 22206 RVA: 0x001C47E0 File Offset: 0x001C29E0
	public static implicit operator ShaderHashId(string s)
	{
		return new ShaderHashId(s);
	}

	// Token: 0x060056BF RID: 22207 RVA: 0x001C47E8 File Offset: 0x001C29E8
	public bool Equals(ShaderHashId other)
	{
		return this._hash == other._hash;
	}

	// Token: 0x060056C0 RID: 22208 RVA: 0x001C47F8 File Offset: 0x001C29F8
	public override bool Equals(object obj)
	{
		if (obj is ShaderHashId)
		{
			ShaderHashId shaderHashId = (ShaderHashId)obj;
			return this.Equals(shaderHashId);
		}
		return false;
	}

	// Token: 0x060056C1 RID: 22209 RVA: 0x001C481D File Offset: 0x001C2A1D
	public static bool operator ==(ShaderHashId x, ShaderHashId y)
	{
		return x.Equals(y);
	}

	// Token: 0x060056C2 RID: 22210 RVA: 0x001C4827 File Offset: 0x001C2A27
	public static bool operator !=(ShaderHashId x, ShaderHashId y)
	{
		return !x.Equals(y);
	}

	// Token: 0x0400679E RID: 26526
	[FormerlySerializedAs("_hashText")]
	[SerializeField]
	private string _text;

	// Token: 0x0400679F RID: 26527
	[NonSerialized]
	private int _hash;
}
