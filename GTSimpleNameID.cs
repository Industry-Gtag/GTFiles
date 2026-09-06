using System;
using Unity.Mathematics;

// Token: 0x02000384 RID: 900
[Serializable]
public struct GTSimpleNameID
{
	// Token: 0x060015F4 RID: 5620 RVA: 0x00074660 File Offset: 0x00072860
	static GTSimpleNameID()
	{
		if ("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-".Length != 64 || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-"[0] != '0' || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-"[9] != '9' || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-"[10] != 'A' || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-"[36] != 'a' || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-"[62] != '_' || "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-"[63] != '-')
		{
			throw new Exception("GTSimpleNameID: The constant string `_k_possibleChars` does not match the expected format. Did you change something without updating the logic?");
		}
	}

	// Token: 0x060015F5 RID: 5621 RVA: 0x000746E8 File Offset: 0x000728E8
	public unsafe static GTSimpleNameID FromString(string input)
	{
		if (input == null)
		{
			input = string.Empty;
		}
		GTSimpleNameID gtsimpleNameID = default(GTSimpleNameID);
		int num = math.min(input.Length, 41);
		gtsimpleNameID.U0 = (ulong)((long)num & 63L);
		int num2 = 6;
		int i = 0;
		while (i < num)
		{
			char c = input[i];
			byte b;
			if (c >= 'A')
			{
				if (c >= 'a')
				{
					if (c > 'z')
					{
						goto IL_00A7;
					}
					b = (byte)(c - 'a' + '$');
				}
				else if (c > 'Z')
				{
					if (c != '_')
					{
						goto IL_00A7;
					}
					b = 62;
				}
				else
				{
					b = (byte)(c - 'A' + '\n');
				}
			}
			else if (c >= '0')
			{
				if (c > '9')
				{
					goto IL_00A7;
				}
				b = (byte)(c - '0');
			}
			else
			{
				if (c != '-')
				{
					goto IL_00A7;
				}
				b = 63;
			}
			ulong num3 = (ulong)b;
			int num4 = num2 + i * 6;
			ulong* ptr = &gtsimpleNameID.U0;
			int num5 = num4 / 64;
			int num6 = num4 % 64;
			ulong num7 = 63UL;
			ulong num8 = num3 & num7;
			ulong num9 = ~(num7 << num6);
			ptr[num5] &= num9;
			ptr[num5] |= num8 << num6;
			int num10 = 64 - num6;
			if (num10 < 6 && num5 < 3)
			{
				int num11 = 6 - num10;
				ulong num12 = (1UL << num11) - 1UL;
				ulong num13 = num8 >> num10;
				ptr[num5 + 1] &= ~num12;
				ptr[num5 + 1] |= num13;
			}
			i++;
			continue;
			IL_00A7:
			throw new ArgumentException(string.Format("Invalid character '{0}' in input string.", c), "input");
		}
		return gtsimpleNameID;
	}

	// Token: 0x060015F6 RID: 5622 RVA: 0x00074868 File Offset: 0x00072A68
	public override string ToString()
	{
		int num = math.min((int)(this.U0 & 63UL), 41);
		char[] array = new char[num];
		int num2 = 6;
		for (int i = 0; i < num; i++)
		{
			int num3 = num2 + i * 6;
			ulong num4 = GTSimpleNameID._Read6Bits(in this, num3);
			array[i] = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-"[(int)num4];
		}
		return new string(array);
	}

	// Token: 0x060015F7 RID: 5623 RVA: 0x000748C4 File Offset: 0x00072AC4
	private unsafe static ulong _Read6Bits(in GTSimpleNameID cv, int bitOffset)
	{
		fixed (ulong* ptr = &cv.U0)
		{
			ulong* ptr2 = ptr;
			int num = bitOffset / 64;
			int num2 = bitOffset % 64;
			ulong num3 = ptr2[num] >> num2;
			int num4 = 64 - num2;
			if (num4 < 6 && num < 3)
			{
				int num5 = 6 - num4;
				ulong num6 = (1UL << num5) - 1UL;
				ulong num7 = ptr2[num + 1] & num6;
				num7 <<= num4;
				num3 |= num7;
			}
			return num3 & 63UL;
		}
	}

	// Token: 0x04001AC9 RID: 6857
	public ulong U0;

	// Token: 0x04001ACA RID: 6858
	public ulong U1;

	// Token: 0x04001ACB RID: 6859
	public ulong U2;

	// Token: 0x04001ACC RID: 6860
	public ulong U3;

	// Token: 0x04001ACD RID: 6861
	private const string _k_possibleChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_-";

	// Token: 0x04001ACE RID: 6862
	private const int _k_maxLength = 41;

	// Token: 0x04001ACF RID: 6863
	private const ulong _k_bitmask6Bits = 63UL;

	// Token: 0x04001AD0 RID: 6864
	private const ushort _k_indexOf_A = 10;

	// Token: 0x04001AD1 RID: 6865
	private const ushort _k_indexOf_a = 36;

	// Token: 0x04001AD2 RID: 6866
	private const ushort _k_indexOf_underscore = 62;

	// Token: 0x04001AD3 RID: 6867
	private const ushort _k_indexOf_hyphen = 63;
}
