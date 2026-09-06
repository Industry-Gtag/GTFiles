using System;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x020003A4 RID: 932
public static class GTPosRotScaleToString
{
	// Token: 0x06001698 RID: 5784 RVA: 0x00083220 File Offset: 0x00081420
	public static string ToString(Vector3 pos, Vector3 rot, Vector3 scale, bool isWorldSpace, string parentPath = null)
	{
		string text = (isWorldSpace ? "WorldPRS" : "LocalPRS");
		string text2 = string.Concat(new string[]
		{
			text,
			" { p=",
			GTPosRotScaleToString.ValToStr(pos),
			", r=",
			GTPosRotScaleToString.ValToStr(rot),
			", s=",
			GTPosRotScaleToString.ValToStr(scale)
		});
		if (!string.IsNullOrEmpty(parentPath))
		{
			text2 = text2 + " parent=\"" + parentPath + "\"";
		}
		return text2 + " }";
	}

	// Token: 0x06001699 RID: 5785 RVA: 0x000832A9 File Offset: 0x000814A9
	private static string ValToStr(Vector3 v)
	{
		return string.Format("({0:R}, {1:R}, {2:R})", v.x, v.y, v.z);
	}

	// Token: 0x0600169A RID: 5786 RVA: 0x000832D6 File Offset: 0x000814D6
	public static bool ParseIsWorldSpace(string input)
	{
		return input.Contains("WorldPRS");
	}

	// Token: 0x0600169B RID: 5787 RVA: 0x000832E4 File Offset: 0x000814E4
	public static string ParseParentPath(string input)
	{
		MatchCollection matchCollection = Regex.Matches(input, "parent\\s*=\\s*\"(?<parent>.*?)\"");
		if (matchCollection.Count <= 0)
		{
			return null;
		}
		return matchCollection[0].Groups["parent"].Value;
	}

	// Token: 0x0600169C RID: 5788 RVA: 0x00083323 File Offset: 0x00081523
	public static bool TryParsePos(string input, out Vector3 v)
	{
		return GTPosRotScaleToString.TryParseVec3_internal(GTRegex.k_Pos, input, out v);
	}

	// Token: 0x0600169D RID: 5789 RVA: 0x00083331 File Offset: 0x00081531
	public static bool TryParseRot(string input, out Vector3 v)
	{
		return GTPosRotScaleToString.TryParseVec3_internal(GTRegex.k_Rot, input, out v);
	}

	// Token: 0x0600169E RID: 5790 RVA: 0x0008333F File Offset: 0x0008153F
	public static bool TryParseScale(string input, out Vector3 v)
	{
		return GTPosRotScaleToString.TryParseVec3_internal(GTRegex.k_Scale, input, out v) || GTPosRotScaleToString.TryParseVec3_internal(GTRegex.k_Vec3, input, out v);
	}

	// Token: 0x0600169F RID: 5791 RVA: 0x0008335D File Offset: 0x0008155D
	public static bool TryParseVec3(string input, out Vector3 v)
	{
		return GTPosRotScaleToString.TryParseVec3_internal(GTRegex.k_Vec3, input, out v);
	}

	// Token: 0x060016A0 RID: 5792 RVA: 0x0008336C File Offset: 0x0008156C
	private static bool TryParseVec3_internal(Regex regex, string input, out Vector3 v)
	{
		v = Vector3.zero;
		MatchCollection matchCollection = regex.Matches(input);
		if (matchCollection.Count <= 0)
		{
			return false;
		}
		v = GTPosRotScaleToString.StringToVector3(matchCollection[0]);
		return true;
	}

	// Token: 0x060016A1 RID: 5793 RVA: 0x000833AC File Offset: 0x000815AC
	private static Vector3 StringToVector3(Match match)
	{
		float num = float.Parse(match.Groups["x"].Value);
		float num2 = float.Parse(match.Groups["y"].Value);
		float num3 = float.Parse(match.Groups["z"].Value);
		return new Vector3(num, num2, num3);
	}

	// Token: 0x040020B2 RID: 8370
	public const string k_LocalPRSLabel = "LocalPRS";

	// Token: 0x040020B3 RID: 8371
	public const string k_WorldPRSLabel = "WorldPRS";
}
