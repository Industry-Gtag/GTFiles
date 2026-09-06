using System;
using Newtonsoft.Json;

// Token: 0x02000DAE RID: 3502
public static class JsonUtils
{
	// Token: 0x060055F0 RID: 22000 RVA: 0x001C1D43 File Offset: 0x001BFF43
	public static string ToJson<T>(this T obj, bool indent = true)
	{
		return JsonConvert.SerializeObject(obj, indent ? Formatting.Indented : Formatting.None);
	}

	// Token: 0x060055F1 RID: 22001 RVA: 0x001C1D57 File Offset: 0x001BFF57
	public static T FromJson<T>(this string s)
	{
		return JsonConvert.DeserializeObject<T>(s);
	}

	// Token: 0x060055F2 RID: 22002 RVA: 0x001C1D60 File Offset: 0x001BFF60
	public static string JsonSerializeEventData<T>(this T obj)
	{
		JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.All,
			CheckAdditionalContent = true,
			Formatting = Formatting.None
		};
		jsonSerializerSettings.Converters.Add(new Vector3Converter());
		return JsonConvert.SerializeObject(obj, jsonSerializerSettings);
	}

	// Token: 0x060055F3 RID: 22003 RVA: 0x001C1DA4 File Offset: 0x001BFFA4
	public static T JsonDeserializeEventData<T>(this string s)
	{
		JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.All
		};
		jsonSerializerSettings.Converters.Add(new Vector3Converter());
		return JsonConvert.DeserializeObject<T>(s, jsonSerializerSettings);
	}
}
