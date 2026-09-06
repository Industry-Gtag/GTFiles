using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x0200114C RID: 4428
	public class StandImport
	{
		// Token: 0x06006F3A RID: 28474 RVA: 0x0023DDEC File Offset: 0x0023BFEC
		public void DecomposeFromTitleDataString(string data)
		{
			string[] array = data.Split("\\n", StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				this.DecomposeStandDataTitleData(array[i]);
			}
		}

		// Token: 0x06006F3B RID: 28475 RVA: 0x0023DE20 File Offset: 0x0023C020
		public void DecomposeStandDataTitleData(string dataString)
		{
			string[] array = dataString.Split("\\t", StringSplitOptions.None);
			if (array.Length == 5)
			{
				this.standData.Add(new StandTypeData(array));
				return;
			}
			if (array.Length == 4)
			{
				this.standData.Add(new StandTypeData(array));
				return;
			}
			string text = "";
			foreach (string text2 in array)
			{
				text = text + text2 + "|";
			}
			Debug.LogError("Store Importer Data String is not valid : " + text);
		}

		// Token: 0x06006F3C RID: 28476 RVA: 0x0023DEA3 File Offset: 0x0023C0A3
		public void DeserializeFromJSON(string JSONString)
		{
			this.standData = JsonConvert.DeserializeObject<List<StandTypeData>>(JSONString);
		}

		// Token: 0x06006F3D RID: 28477 RVA: 0x0023DEB4 File Offset: 0x0023C0B4
		public void DecomposeStandData(string dataString)
		{
			string[] array = dataString.Split('\t', StringSplitOptions.None);
			if (array.Length == 5)
			{
				this.standData.Add(new StandTypeData(array));
				return;
			}
			if (array.Length == 4)
			{
				this.standData.Add(new StandTypeData(array));
				return;
			}
			string text = "";
			foreach (string text2 in array)
			{
				text = text + text2 + "|";
			}
			Debug.LogError("Store Importer Data String is not valid : " + text);
		}

		// Token: 0x04007F35 RID: 32565
		public List<StandTypeData> standData = new List<StandTypeData>();

		// Token: 0x04007F36 RID: 32566
		public Dictionary<string, StandTypeData> standKeyToDataDict = new Dictionary<string, StandTypeData>();
	}
}
