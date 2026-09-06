using System;

// Token: 0x02000B50 RID: 2896
public static class KIDFeaturesExtensions
{
	// Token: 0x060049B3 RID: 18867 RVA: 0x00188DFC File Offset: 0x00186FFC
	public static string ToStandardisedString(this EKIDFeatures feature)
	{
		switch (feature)
		{
		case EKIDFeatures.Multiplayer:
			return "multiplayer";
		case EKIDFeatures.Custom_Nametags:
			return "custom-username";
		case EKIDFeatures.Voice_Chat:
			return "voice-chat";
		case EKIDFeatures.Mods:
			return "mods";
		case EKIDFeatures.Groups:
			return "join-groups";
		default:
			return feature.ToString();
		}
	}

	// Token: 0x060049B4 RID: 18868 RVA: 0x00188E50 File Offset: 0x00187050
	public static EKIDFeatures? FromString(string name)
	{
		string text = name.ToLower();
		if (text == "voice-chat")
		{
			return new EKIDFeatures?(EKIDFeatures.Voice_Chat);
		}
		if (text == "custom-username")
		{
			return new EKIDFeatures?(EKIDFeatures.Custom_Nametags);
		}
		if (text == "multiplayer")
		{
			return new EKIDFeatures?(EKIDFeatures.Multiplayer);
		}
		if (text == "mods")
		{
			return new EKIDFeatures?(EKIDFeatures.Mods);
		}
		if (!(text == "join-groups"))
		{
			return null;
		}
		return new EKIDFeatures?(EKIDFeatures.Groups);
	}

	// Token: 0x060049B5 RID: 18869 RVA: 0x00188ED4 File Offset: 0x001870D4
	public static bool TryGetFromString(string name, out EKIDFeatures result)
	{
		EKIDFeatures? ekidfeatures = KIDFeaturesExtensions.FromString(name);
		if (ekidfeatures != null)
		{
			result = ekidfeatures.Value;
			return true;
		}
		result = EKIDFeatures.Voice_Chat;
		return false;
	}
}
