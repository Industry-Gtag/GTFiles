using System;
using System.ComponentModel;
using UnityEngine;

// Token: 0x020003AF RID: 943
public static class UnityTagsExt
{
	// Token: 0x060016C9 RID: 5833 RVA: 0x0008411C File Offset: 0x0008231C
	public static UnityTag ToTag(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return UnityTag.Invalid;
		}
		UnityTag unityTag;
		if (!UnityTags.StringToTag.TryGetValue(s, out unityTag))
		{
			return UnityTag.Invalid;
		}
		return unityTag;
	}

	// Token: 0x060016CA RID: 5834 RVA: 0x00084145 File Offset: 0x00082345
	public static void SetTag(this Component c, UnityTag tag)
	{
		if (c == null)
		{
			return;
		}
		if (tag == UnityTag.Invalid)
		{
			throw new InvalidEnumArgumentException("tag");
		}
		c.tag = UnityTags.StringValues[(int)tag];
	}

	// Token: 0x060016CB RID: 5835 RVA: 0x0008416D File Offset: 0x0008236D
	public static void SetTag(this GameObject g, UnityTag tag)
	{
		if (g == null)
		{
			return;
		}
		if (tag == UnityTag.Invalid)
		{
			throw new InvalidEnumArgumentException("tag");
		}
		g.tag = UnityTags.StringValues[(int)tag];
	}

	// Token: 0x060016CC RID: 5836 RVA: 0x00084195 File Offset: 0x00082395
	public static bool TryGetTag(this GameObject g, out UnityTag tag)
	{
		tag = UnityTag.Invalid;
		return !(g == null) && UnityTags.StringToTag.TryGetValue(g.tag, out tag);
	}

	// Token: 0x060016CD RID: 5837 RVA: 0x000841B6 File Offset: 0x000823B6
	public static bool TryGetTag(this Component c, out UnityTag tag)
	{
		tag = UnityTag.Invalid;
		return !(c == null) && UnityTags.StringToTag.TryGetValue(c.tag, out tag);
	}

	// Token: 0x060016CE RID: 5838 RVA: 0x000841D7 File Offset: 0x000823D7
	public static bool CompareTag(this GameObject g, UnityTag tag)
	{
		if (g == null)
		{
			return false;
		}
		if (tag == UnityTag.Invalid)
		{
			throw new InvalidEnumArgumentException("tag");
		}
		return g.CompareTag(UnityTags.StringValues[(int)tag]);
	}

	// Token: 0x060016CF RID: 5839 RVA: 0x00084200 File Offset: 0x00082400
	public static bool CompareTag(this Component c, UnityTag tag)
	{
		if (c == null)
		{
			return false;
		}
		if (tag == UnityTag.Invalid)
		{
			throw new InvalidEnumArgumentException("tag");
		}
		return c.CompareTag(UnityTags.StringValues[(int)tag]);
	}
}
