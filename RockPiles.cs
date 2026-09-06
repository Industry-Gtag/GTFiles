using System;
using UnityEngine;

// Token: 0x0200024A RID: 586
public class RockPiles : MonoBehaviour
{
	// Token: 0x06000FB5 RID: 4021 RVA: 0x0005568C File Offset: 0x0005388C
	public void Show(int visiblePercentage)
	{
		if (visiblePercentage <= 0)
		{
			this.ShowRock(-1);
			return;
		}
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < this._rocks.Length; i++)
		{
			RockPiles.RockPile rockPile = this._rocks[i];
			if (visiblePercentage >= rockPile.threshold && num2 < rockPile.threshold)
			{
				num = i;
				num2 = rockPile.threshold;
			}
		}
		this.ShowRock(num);
	}

	// Token: 0x06000FB6 RID: 4022 RVA: 0x000556EC File Offset: 0x000538EC
	private void ShowRock(int rockToShow)
	{
		for (int i = 0; i < this._rocks.Length; i++)
		{
			this._rocks[i].visual.SetActive(i == rockToShow);
		}
	}

	// Token: 0x040012ED RID: 4845
	[SerializeField]
	private RockPiles.RockPile[] _rocks;

	// Token: 0x0200024B RID: 587
	[Serializable]
	public struct RockPile
	{
		// Token: 0x040012EE RID: 4846
		public GameObject visual;

		// Token: 0x040012EF RID: 4847
		public int threshold;
	}
}
