using System;
using GorillaExtensions;
using GorillaNetworking;
using TMPro;
using UnityEngine;

namespace CosmeticRoom
{
	// Token: 0x02001086 RID: 4230
	public class CurrencyBoard : MonoBehaviour
	{
		// Token: 0x0600699B RID: 27035 RVA: 0x0021FFE0 File Offset: 0x0021E1E0
		public void OnEnable()
		{
			CosmeticsController.instance.AddCurrencyBoard(this);
		}

		// Token: 0x0600699C RID: 27036 RVA: 0x0021FFEF File Offset: 0x0021E1EF
		public void OnDisable()
		{
			CosmeticsController.instance.RemoveCurrencyBoard(this);
		}

		// Token: 0x0600699D RID: 27037 RVA: 0x00220000 File Offset: 0x0021E200
		public void UpdateCurrencyBoard(bool checkedDaily, bool gotDaily, int currencyBalance, int secTilTomorrow)
		{
			if (this.dailyRocksTextTMP.IsNotNull())
			{
				this.dailyRocksTextTMP.text = (checkedDaily ? (gotDaily ? "SUCCESSFULLY GOT DAILY ROCKS!" : "WAITING TO GET DAILY ROCKS...") : "CHECKING DAILY ROCKS...");
			}
			if (this.currencyBoardTextTMP.IsNotNull())
			{
				this.currencyBoardTextTMP.text = string.Concat(new string[]
				{
					currencyBalance.ToString(),
					"\n\n",
					(secTilTomorrow / 3600).ToString(),
					" HR, ",
					(secTilTomorrow % 3600 / 60).ToString(),
					"MIN"
				});
			}
		}

		// Token: 0x0400795D RID: 31069
		public TMP_Text dailyRocksTextTMP;

		// Token: 0x0400795E RID: 31070
		public TMP_Text currencyBoardTextTMP;
	}
}
