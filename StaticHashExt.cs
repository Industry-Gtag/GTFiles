using System;

// Token: 0x02000DE9 RID: 3561
public static class StaticHashExt
{
	// Token: 0x0600574F RID: 22351 RVA: 0x001C85C9 File Offset: 0x001C67C9
	public static int GetStaticHash(this int i)
	{
		return StaticHash.Compute(i);
	}

	// Token: 0x06005750 RID: 22352 RVA: 0x001C85D1 File Offset: 0x001C67D1
	public static int GetStaticHash(this uint u)
	{
		return StaticHash.Compute(u);
	}

	// Token: 0x06005751 RID: 22353 RVA: 0x001C85D9 File Offset: 0x001C67D9
	public static int GetStaticHash(this float f)
	{
		return StaticHash.Compute(f);
	}

	// Token: 0x06005752 RID: 22354 RVA: 0x001C85E1 File Offset: 0x001C67E1
	public static int GetStaticHash(this long l)
	{
		return StaticHash.Compute(l);
	}

	// Token: 0x06005753 RID: 22355 RVA: 0x001C85E9 File Offset: 0x001C67E9
	public static int GetStaticHash(this double d)
	{
		return StaticHash.Compute(d);
	}

	// Token: 0x06005754 RID: 22356 RVA: 0x001C85F1 File Offset: 0x001C67F1
	public static int GetStaticHash(this bool b)
	{
		return StaticHash.Compute(b);
	}

	// Token: 0x06005755 RID: 22357 RVA: 0x001C85F9 File Offset: 0x001C67F9
	public static int GetStaticHash(this DateTime dt)
	{
		return StaticHash.Compute(dt);
	}

	// Token: 0x06005756 RID: 22358 RVA: 0x001C8601 File Offset: 0x001C6801
	public static int GetStaticHash(this string s)
	{
		return StaticHash.Compute(s);
	}

	// Token: 0x06005757 RID: 22359 RVA: 0x001C8609 File Offset: 0x001C6809
	public static int GetStaticHash(this byte[] bytes)
	{
		return StaticHash.Compute(bytes);
	}
}
