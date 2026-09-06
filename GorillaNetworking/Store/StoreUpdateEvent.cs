using System;
using System.Collections.Generic;
using LitJson;
using Newtonsoft.Json;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02001155 RID: 4437
	public class StoreUpdateEvent
	{
		// Token: 0x06006F65 RID: 28517 RVA: 0x00002050 File Offset: 0x00000250
		public StoreUpdateEvent()
		{
		}

		// Token: 0x06006F66 RID: 28518 RVA: 0x0023E714 File Offset: 0x0023C914
		public StoreUpdateEvent(string pedestalID, string itemName, DateTime startTimeUTC, DateTime endTimeUTC)
		{
			this.PedestalID = pedestalID;
			this.ItemName = itemName;
			this.StartTimeUTC = startTimeUTC;
			this.EndTimeUTC = endTimeUTC;
		}

		// Token: 0x06006F67 RID: 28519 RVA: 0x0023E739 File Offset: 0x0023C939
		public static string SerializeAsJSon(StoreUpdateEvent storeEvent)
		{
			return JsonUtility.ToJson(storeEvent);
		}

		// Token: 0x06006F68 RID: 28520 RVA: 0x0023E741 File Offset: 0x0023C941
		public static string SerializeArrayAsJSon(StoreUpdateEvent[] storeEvents)
		{
			return JsonConvert.SerializeObject(storeEvents);
		}

		// Token: 0x06006F69 RID: 28521 RVA: 0x0023E749 File Offset: 0x0023C949
		public static StoreUpdateEvent DeserializeFromJSon(string json)
		{
			return JsonUtility.FromJson<StoreUpdateEvent>(json);
		}

		// Token: 0x06006F6A RID: 28522 RVA: 0x0023E751 File Offset: 0x0023C951
		public static StoreUpdateEvent[] DeserializeFromJSonArray(string json)
		{
			List<StoreUpdateEvent> list = JsonMapper.ToObject<List<StoreUpdateEvent>>(json);
			list.Sort((StoreUpdateEvent x, StoreUpdateEvent y) => x.StartTimeUTC.CompareTo(y.StartTimeUTC));
			return list.ToArray();
		}

		// Token: 0x06006F6B RID: 28523 RVA: 0x0023E783 File Offset: 0x0023C983
		public static List<StoreUpdateEvent> DeserializeFromJSonList(string json)
		{
			List<StoreUpdateEvent> list = JsonMapper.ToObject<List<StoreUpdateEvent>>(json);
			list.Sort((StoreUpdateEvent x, StoreUpdateEvent y) => x.StartTimeUTC.CompareTo(y.StartTimeUTC));
			return list;
		}

		// Token: 0x04007F65 RID: 32613
		public string PedestalID;

		// Token: 0x04007F66 RID: 32614
		public string ItemName;

		// Token: 0x04007F67 RID: 32615
		public DateTime StartTimeUTC;

		// Token: 0x04007F68 RID: 32616
		public DateTime EndTimeUTC;
	}
}
