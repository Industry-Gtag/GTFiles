using System;
using System.Collections.Generic;

// Token: 0x02000157 RID: 343
public static class SIResourceHelper
{
	// Token: 0x0600090C RID: 2316 RVA: 0x00030F60 File Offset: 0x0002F160
	public static bool IsInOrder(this IList<SIResource.ResourceCost> cost)
	{
		if (cost == null)
		{
			return true;
		}
		SIResource.ResourceType resourceType = (SIResource.ResourceType)(-1);
		foreach (SIResource.ResourceCost resourceCost in cost)
		{
			if (resourceCost.type <= resourceType)
			{
				return false;
			}
			resourceType = resourceCost.type;
		}
		return true;
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x00030FC0 File Offset: 0x0002F1C0
	public static bool IsValid(this IList<SIResource.ResourceCost> cost)
	{
		if (cost == null || cost.Count == 0)
		{
			return false;
		}
		int num = 0;
		foreach (SIResource.ResourceCost resourceCost in cost)
		{
			int num2 = 1 << (int)resourceCost.type;
			if ((num & num2) != 0)
			{
				return false;
			}
			if (resourceCost.amount <= 0)
			{
				return false;
			}
			num |= num2;
		}
		return true;
	}

	// Token: 0x0600090E RID: 2318 RVA: 0x0003103C File Offset: 0x0002F23C
	public static bool IsValid_AllowZero(this IList<SIResource.ResourceCost> cost)
	{
		if (cost == null || cost.Count == 0)
		{
			return false;
		}
		int num = 0;
		foreach (SIResource.ResourceCost resourceCost in cost)
		{
			int num2 = 1 << (int)resourceCost.type;
			if ((num & num2) != 0)
			{
				return false;
			}
			if (resourceCost.amount < 0)
			{
				return false;
			}
			num |= num2;
		}
		return true;
	}

	// Token: 0x0600090F RID: 2319 RVA: 0x000310B8 File Offset: 0x0002F2B8
	public static SIResource.ResourceCategoryCost GetCategoryCosts(this IList<SIResource.ResourceCost> costs)
	{
		int num = 0;
		int num2 = 0;
		if (costs != null)
		{
			foreach (SIResource.ResourceCost resourceCost in costs)
			{
				if (resourceCost.type == SIResource.ResourceType.TechPoint)
				{
					num += resourceCost.amount;
				}
				else
				{
					num2 += resourceCost.amount;
				}
			}
		}
		return new SIResource.ResourceCategoryCost(num, num2);
	}

	// Token: 0x06000910 RID: 2320 RVA: 0x00031124 File Offset: 0x0002F324
	public static List<SIResource.ResourceCost> GetTotalResourceCost(this IList<SIResource.ResourceCost> baseCost, IList<SIResource.ResourceCost> additiveCosts)
	{
		List<SIResource.ResourceCost> list = new List<SIResource.ResourceCost>(baseCost);
		foreach (SIResource.ResourceCost resourceCost in additiveCosts)
		{
			list.Add(resourceCost);
		}
		return list;
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x00031174 File Offset: 0x0002F374
	public static List<SIResource.ResourceCost> GetMax(this IList<SIResource.ResourceCost> baseCost, IList<SIResource.ResourceCost> additiveCosts)
	{
		List<SIResource.ResourceCost> list = new List<SIResource.ResourceCost>(baseCost);
		foreach (SIResource.ResourceCost resourceCost in additiveCosts)
		{
			list.Add(resourceCost);
		}
		list.Sort();
		return list;
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x000311CC File Offset: 0x0002F3CC
	public static int GetAmount(this IList<SIResource.ResourceCost> costs, SIResource.ResourceType resourceType)
	{
		foreach (SIResource.ResourceCost resourceCost in costs)
		{
			if (resourceCost.type == resourceType)
			{
				return resourceCost.amount;
			}
		}
		return 0;
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x00031224 File Offset: 0x0002F424
	public static void SetAmount(this List<SIResource.ResourceCost> costs, SIResource.ResourceType resourceType, int amount)
	{
		for (int i = 0; i < costs.Count; i++)
		{
			SIResource.ResourceCost resourceCost = costs[i];
			if (resourceCost.type == resourceType)
			{
				resourceCost.amount = amount;
				costs[i] = resourceCost;
				return;
			}
		}
		costs.Add(new SIResource.ResourceCost(resourceType, amount));
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x00031274 File Offset: 0x0002F474
	public static void AddResourceCost(this List<SIResource.ResourceCost> baseCost, SIResource.ResourceCost additiveCost)
	{
		for (int i = 0; i < baseCost.Count; i++)
		{
			SIResource.ResourceCost resourceCost = baseCost[i];
			if (resourceCost.type == additiveCost.type)
			{
				resourceCost.amount += additiveCost.amount;
				baseCost[i] = resourceCost;
				return;
			}
		}
		baseCost.Add(additiveCost);
	}

	// Token: 0x06000915 RID: 2325 RVA: 0x000312CC File Offset: 0x0002F4CC
	public static void AddResourceCost(this List<SIResource.ResourceCost> baseCost, IList<SIResource.ResourceCost> additiveCost)
	{
		foreach (SIResource.ResourceCost resourceCost in additiveCost)
		{
			baseCost.AddResourceCost(resourceCost);
		}
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x00031314 File Offset: 0x0002F514
	public static int GetTechPointCost(this IList<SIResource.ResourceCost> costs)
	{
		int num = 0;
		foreach (SIResource.ResourceCost resourceCost in costs)
		{
			if (resourceCost.type == SIResource.ResourceType.TechPoint)
			{
				num += resourceCost.amount;
			}
		}
		return num;
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x0003136C File Offset: 0x0002F56C
	public static int GetMiscCost(this IList<SIResource.ResourceCost> costs)
	{
		int num = 0;
		foreach (SIResource.ResourceCost resourceCost in costs)
		{
			if (resourceCost.type != SIResource.ResourceType.TechPoint)
			{
				num += resourceCost.amount;
			}
		}
		return num;
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x000313C4 File Offset: 0x0002F5C4
	public static void SetResourceCost(this IList<SIResource.ResourceCost> costs, SIResource.ResourceCategoryCost desiredCosts)
	{
		costs.SetTechPointCost(desiredCosts.techPoints);
		costs.SetMiscCost(desiredCosts.misc);
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x000313DE File Offset: 0x0002F5DE
	public static void AddResourceCost(this IList<SIResource.ResourceCost> baseCost, SIResource.ResourceCategoryCost additiveCost)
	{
		baseCost.SetTechPointCost(baseCost.GetTechPointCost() + additiveCost.techPoints);
		baseCost.SetMiscCost(baseCost.GetMiscCost() + additiveCost.misc);
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x00031408 File Offset: 0x0002F608
	public static void SetTechPointCost(this IList<SIResource.ResourceCost> baseCost, int desiredCost)
	{
		for (int i = 0; i < baseCost.Count; i++)
		{
			SIResource.ResourceCost resourceCost = baseCost[i];
			if (resourceCost.type == SIResource.ResourceType.TechPoint)
			{
				resourceCost.amount = desiredCost;
				baseCost[i] = resourceCost;
				return;
			}
		}
		baseCost.Add(new SIResource.ResourceCost(SIResource.ResourceType.TechPoint, desiredCost));
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x00031454 File Offset: 0x0002F654
	public static void SetMiscCost(this IList<SIResource.ResourceCost> baseCost, int desiredCost)
	{
		int num = baseCost.GetMiscCost();
		if (num == desiredCost)
		{
			return;
		}
		for (int i = 0; i < baseCost.Count; i++)
		{
			SIResource.ResourceCost resourceCost = baseCost[i];
			if (resourceCost.type != SIResource.ResourceType.TechPoint)
			{
				resourceCost.amount += desiredCost - num;
				if (resourceCost.amount >= 1)
				{
					baseCost[i] = resourceCost;
					return;
				}
				baseCost.RemoveAt(i--);
				num = baseCost.GetMiscCost();
				if (num == desiredCost)
				{
					return;
				}
			}
		}
		if (desiredCost == num)
		{
			return;
		}
		baseCost.Add(new SIResource.ResourceCost(SIResource.ResourceType.StrangeWood, desiredCost - num));
	}
}
