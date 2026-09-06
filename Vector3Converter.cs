using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

// Token: 0x02000DAF RID: 3503
public class Vector3Converter : JsonConverter
{
	// Token: 0x060055F4 RID: 22004 RVA: 0x001C1DD8 File Offset: 0x001BFFD8
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		Vector3 vector = (Vector3)value;
		writer.WriteStartObject();
		writer.WritePropertyName("x");
		writer.WriteValue(vector.x);
		writer.WritePropertyName("y");
		writer.WriteValue(vector.y);
		writer.WritePropertyName("z");
		writer.WriteValue(vector.z);
		writer.WriteEndObject();
	}

	// Token: 0x060055F5 RID: 22005 RVA: 0x001C1E40 File Offset: 0x001C0040
	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		JObject jobject = JObject.Load(reader);
		return new Vector3((float)jobject["x"], (float)jobject["y"], (float)jobject["z"]);
	}

	// Token: 0x060055F6 RID: 22006 RVA: 0x001C1E91 File Offset: 0x001C0091
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Vector3);
	}
}
