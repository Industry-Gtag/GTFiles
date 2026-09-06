using System;
using System.Collections;
using System.Collections.Specialized;

namespace LitJson
{
	// Token: 0x02000EE9 RID: 3817
	public interface IJsonWrapper : IList, ICollection, IEnumerable, IOrderedDictionary, IDictionary
	{
		// Token: 0x170008E5 RID: 2277
		// (get) Token: 0x06005CEC RID: 23788
		bool IsArray { get; }

		// Token: 0x170008E6 RID: 2278
		// (get) Token: 0x06005CED RID: 23789
		bool IsBoolean { get; }

		// Token: 0x170008E7 RID: 2279
		// (get) Token: 0x06005CEE RID: 23790
		bool IsDouble { get; }

		// Token: 0x170008E8 RID: 2280
		// (get) Token: 0x06005CEF RID: 23791
		bool IsInt { get; }

		// Token: 0x170008E9 RID: 2281
		// (get) Token: 0x06005CF0 RID: 23792
		bool IsLong { get; }

		// Token: 0x170008EA RID: 2282
		// (get) Token: 0x06005CF1 RID: 23793
		bool IsObject { get; }

		// Token: 0x170008EB RID: 2283
		// (get) Token: 0x06005CF2 RID: 23794
		bool IsString { get; }

		// Token: 0x06005CF3 RID: 23795
		bool GetBoolean();

		// Token: 0x06005CF4 RID: 23796
		double GetDouble();

		// Token: 0x06005CF5 RID: 23797
		int GetInt();

		// Token: 0x06005CF6 RID: 23798
		JsonType GetJsonType();

		// Token: 0x06005CF7 RID: 23799
		long GetLong();

		// Token: 0x06005CF8 RID: 23800
		string GetString();

		// Token: 0x06005CF9 RID: 23801
		void SetBoolean(bool val);

		// Token: 0x06005CFA RID: 23802
		void SetDouble(double val);

		// Token: 0x06005CFB RID: 23803
		void SetInt(int val);

		// Token: 0x06005CFC RID: 23804
		void SetJsonType(JsonType type);

		// Token: 0x06005CFD RID: 23805
		void SetLong(long val);

		// Token: 0x06005CFE RID: 23806
		void SetString(string val);

		// Token: 0x06005CFF RID: 23807
		string ToJson();

		// Token: 0x06005D00 RID: 23808
		void ToJson(JsonWriter writer);
	}
}
