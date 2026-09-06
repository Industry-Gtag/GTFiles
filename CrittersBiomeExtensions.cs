using System;
using System.Collections.Generic;

// Token: 0x02000059 RID: 89
public static class CrittersBiomeExtensions
{
	// Token: 0x060001B5 RID: 437 RVA: 0x0000A8F8 File Offset: 0x00008AF8
	static CrittersBiomeExtensions()
	{
		foreach (object obj in Enum.GetValues(typeof(CrittersBiome)))
		{
			CrittersBiome crittersBiome = (CrittersBiome)obj;
			if (crittersBiome != CrittersBiome.Any && crittersBiome != CrittersBiome.IntroArea)
			{
				CrittersBiomeExtensions._allScannableBiomes.Add(crittersBiome);
			}
		}
	}

	// Token: 0x060001B6 RID: 438 RVA: 0x0000A980 File Offset: 0x00008B80
	public static string GetHabitatDescription(this CrittersBiome biome)
	{
		string text;
		if (!CrittersBiomeExtensions._habitatLookup.TryGetValue(biome, out text))
		{
			if (biome == CrittersBiome.Any)
			{
				text = "Any";
			}
			else
			{
				if (CrittersBiomeExtensions._habitatBiomes == null)
				{
					CrittersBiomeExtensions._habitatBiomes = new List<CrittersBiome>();
				}
				CrittersBiomeExtensions._habitatBiomes.Clear();
				for (int i = 0; i < CrittersBiomeExtensions._allScannableBiomes.Count; i++)
				{
					if (biome.HasFlag(CrittersBiomeExtensions._allScannableBiomes[i]))
					{
						CrittersBiomeExtensions._habitatBiomes.Add(CrittersBiomeExtensions._allScannableBiomes[i]);
					}
				}
			}
			text = ((CrittersBiomeExtensions._habitatBiomes.Count > 3) ? "Various" : string.Join<CrittersBiome>(", ", CrittersBiomeExtensions._habitatBiomes));
			CrittersBiomeExtensions._habitatLookup[biome] = text;
		}
		return text;
	}

	// Token: 0x040001E9 RID: 489
	private static List<CrittersBiome> _allScannableBiomes = new List<CrittersBiome>();

	// Token: 0x040001EA RID: 490
	private static Dictionary<CrittersBiome, string> _habitatLookup = new Dictionary<CrittersBiome, string>();

	// Token: 0x040001EB RID: 491
	private static List<CrittersBiome> _habitatBiomes;
}
