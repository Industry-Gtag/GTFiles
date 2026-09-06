using System;
using UnityEngine;

// Token: 0x0200001D RID: 29
public class PlatformerCollectiblesMain : MonoBehaviour
{
	// Token: 0x06000071 RID: 113 RVA: 0x00003C84 File Offset: 0x00001E84
	public void Start()
	{
		int num = 0;
		while ((float)num < this.CoinGridCount)
		{
			float num2 = -0.5f * this.CoinGridSize + this.CoinGridSize * (float)num / (this.CoinGridCount - 1f);
			int num3 = 0;
			while ((float)num3 < this.CoinGridCount)
			{
				float num4 = -0.5f * this.CoinGridSize + this.CoinGridSize * (float)num3 / (this.CoinGridCount - 1f);
				Object.Instantiate<GameObject>(this.Coin).transform.position = new Vector3(num2, 0.2f, num4);
				num3++;
			}
			num++;
		}
	}

	// Token: 0x04000065 RID: 101
	public GameObject Coin;

	// Token: 0x04000066 RID: 102
	public float CoinGridCount = 5f;

	// Token: 0x04000067 RID: 103
	public float CoinGridSize = 7f;
}
