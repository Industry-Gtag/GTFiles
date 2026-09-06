using System;
using Unity.Burst;

// Token: 0x02000C8E RID: 3214
public static class LuaHashing
{
	// Token: 0x06004F3D RID: 20285 RVA: 0x001A69C0 File Offset: 0x001A4BC0
	[BurstCompile]
	public unsafe static int ByteHash(byte* bytes, int len)
	{
		int num = 352654597;
		int num2 = num;
		for (int i = 0; i < len; i += 2)
		{
			num = ((num << 5) + num) ^ (int)bytes[i];
			if (i == len - 1)
			{
				break;
			}
			num2 = ((num2 << 5) + num2) ^ (int)bytes[i + 1];
		}
		return num + num2 * 1648465312;
	}

	// Token: 0x06004F3E RID: 20286 RVA: 0x001A6A08 File Offset: 0x001A4C08
	[BurstCompile]
	public unsafe static int ByteHash(byte* bytes)
	{
		int num = 352654597;
		int num2 = num;
		int num3 = 0;
		while (bytes[num3] != 0)
		{
			num = ((num << 5) + num) ^ (int)bytes[num3];
			num3++;
			if (bytes[num3] == 0)
			{
				break;
			}
			num2 = ((num2 << 5) + num2) ^ (int)bytes[num3];
			num3++;
		}
		return num + num2 * 1648465312;
	}

	// Token: 0x06004F3F RID: 20287 RVA: 0x001A6A54 File Offset: 0x001A4C54
	public static int ByteHash(string bytes)
	{
		int length = bytes.Length;
		int num = 352654597;
		int num2 = num;
		for (int i = 0; i < length; i += 2)
		{
			num = ((num << 5) + num) ^ (int)bytes[i];
			if (i == length - 1)
			{
				break;
			}
			num2 = ((num2 << 5) + num2) ^ (int)bytes[i + 1];
		}
		return num + num2 * 1648465312;
	}

	// Token: 0x06004F40 RID: 20288 RVA: 0x001A6AAC File Offset: 0x001A4CAC
	[BurstCompile]
	public static int ByteHash(byte[] bytes)
	{
		int num = bytes.Length;
		int num2 = 352654597;
		int num3 = num2;
		for (int i = 0; i < num; i += 2)
		{
			num2 = ((num2 << 5) + num2) ^ (int)bytes[i];
			if (i == num - 1)
			{
				break;
			}
			num3 = ((num3 << 5) + num3) ^ (int)bytes[i + 1];
		}
		return num2 + num3 * 1648465312;
	}

	// Token: 0x040061B1 RID: 25009
	private const int k_enhancer = 1648465312;

	// Token: 0x040061B2 RID: 25010
	private const int k_Seed = 352654597;
}
