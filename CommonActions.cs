using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200003D RID: 61
public class CommonActions : MonoBehaviour
{
	// Token: 0x060000FD RID: 253 RVA: 0x00006345 File Offset: 0x00004545
	public void LoadSavedOutfit(int index)
	{
		if (CosmeticsController.instance)
		{
			CosmeticsController.instance.LoadSavedOutfit(index);
		}
	}

	// Token: 0x060000FE RID: 254 RVA: 0x00006362 File Offset: 0x00004562
	public void LoadPrevOutfit()
	{
		if (CosmeticsController.instance)
		{
			CosmeticsController.instance.PressWardrobeScrollOutfit(false);
		}
	}

	// Token: 0x060000FF RID: 255 RVA: 0x0000637F File Offset: 0x0000457F
	public void LoadNextOutfit()
	{
		if (CosmeticsController.instance)
		{
			CosmeticsController.instance.PressWardrobeScrollOutfit(true);
		}
	}
}
