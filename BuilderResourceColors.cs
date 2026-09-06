using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000622 RID: 1570
[CreateAssetMenu(fileName = "BuilderMaterialResourceColors", menuName = "Gorilla Tag/Builder/ResourceColors", order = 0)]
public class BuilderResourceColors : ScriptableObject
{
	// Token: 0x06002722 RID: 10018 RVA: 0x000CF050 File Offset: 0x000CD250
	public Color GetColorForType(BuilderResourceType type)
	{
		foreach (BuilderResourceColor builderResourceColor in this.colors)
		{
			if (builderResourceColor.type == type)
			{
				return builderResourceColor.color;
			}
		}
		return Color.black;
	}

	// Token: 0x040032A9 RID: 12969
	public List<BuilderResourceColor> colors;
}
