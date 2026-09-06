using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace LitJson
{
	// Token: 0x02000EEA RID: 3818
	public class JsonData : IJsonWrapper, IList, ICollection, IEnumerable, IOrderedDictionary, IDictionary, IEquatable<JsonData>
	{
		// Token: 0x170008EC RID: 2284
		// (get) Token: 0x06005D01 RID: 23809 RVA: 0x001DCB7A File Offset: 0x001DAD7A
		public int Count
		{
			get
			{
				return this.EnsureCollection().Count;
			}
		}

		// Token: 0x170008ED RID: 2285
		// (get) Token: 0x06005D02 RID: 23810 RVA: 0x001DCB87 File Offset: 0x001DAD87
		public bool IsArray
		{
			get
			{
				return this.type == JsonType.Array;
			}
		}

		// Token: 0x170008EE RID: 2286
		// (get) Token: 0x06005D03 RID: 23811 RVA: 0x001DCB92 File Offset: 0x001DAD92
		public bool IsBoolean
		{
			get
			{
				return this.type == JsonType.Boolean;
			}
		}

		// Token: 0x170008EF RID: 2287
		// (get) Token: 0x06005D04 RID: 23812 RVA: 0x001DCB9D File Offset: 0x001DAD9D
		public bool IsDouble
		{
			get
			{
				return this.type == JsonType.Double;
			}
		}

		// Token: 0x170008F0 RID: 2288
		// (get) Token: 0x06005D05 RID: 23813 RVA: 0x001DCBA8 File Offset: 0x001DADA8
		public bool IsInt
		{
			get
			{
				return this.type == JsonType.Int;
			}
		}

		// Token: 0x170008F1 RID: 2289
		// (get) Token: 0x06005D06 RID: 23814 RVA: 0x001DCBB3 File Offset: 0x001DADB3
		public bool IsLong
		{
			get
			{
				return this.type == JsonType.Long;
			}
		}

		// Token: 0x170008F2 RID: 2290
		// (get) Token: 0x06005D07 RID: 23815 RVA: 0x001DCBBE File Offset: 0x001DADBE
		public bool IsObject
		{
			get
			{
				return this.type == JsonType.Object;
			}
		}

		// Token: 0x170008F3 RID: 2291
		// (get) Token: 0x06005D08 RID: 23816 RVA: 0x001DCBC9 File Offset: 0x001DADC9
		public bool IsString
		{
			get
			{
				return this.type == JsonType.String;
			}
		}

		// Token: 0x170008F4 RID: 2292
		// (get) Token: 0x06005D09 RID: 23817 RVA: 0x001DCBD4 File Offset: 0x001DADD4
		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		// Token: 0x170008F5 RID: 2293
		// (get) Token: 0x06005D0A RID: 23818 RVA: 0x001DCBDC File Offset: 0x001DADDC
		bool ICollection.IsSynchronized
		{
			get
			{
				return this.EnsureCollection().IsSynchronized;
			}
		}

		// Token: 0x170008F6 RID: 2294
		// (get) Token: 0x06005D0B RID: 23819 RVA: 0x001DCBE9 File Offset: 0x001DADE9
		object ICollection.SyncRoot
		{
			get
			{
				return this.EnsureCollection().SyncRoot;
			}
		}

		// Token: 0x170008F7 RID: 2295
		// (get) Token: 0x06005D0C RID: 23820 RVA: 0x001DCBF6 File Offset: 0x001DADF6
		bool IDictionary.IsFixedSize
		{
			get
			{
				return this.EnsureDictionary().IsFixedSize;
			}
		}

		// Token: 0x170008F8 RID: 2296
		// (get) Token: 0x06005D0D RID: 23821 RVA: 0x001DCC03 File Offset: 0x001DAE03
		bool IDictionary.IsReadOnly
		{
			get
			{
				return this.EnsureDictionary().IsReadOnly;
			}
		}

		// Token: 0x170008F9 RID: 2297
		// (get) Token: 0x06005D0E RID: 23822 RVA: 0x001DCC10 File Offset: 0x001DAE10
		ICollection IDictionary.Keys
		{
			get
			{
				this.EnsureDictionary();
				IList<string> list = new List<string>();
				foreach (KeyValuePair<string, JsonData> keyValuePair in this.object_list)
				{
					list.Add(keyValuePair.Key);
				}
				return (ICollection)list;
			}
		}

		// Token: 0x170008FA RID: 2298
		// (get) Token: 0x06005D0F RID: 23823 RVA: 0x001DCC78 File Offset: 0x001DAE78
		ICollection IDictionary.Values
		{
			get
			{
				this.EnsureDictionary();
				IList<JsonData> list = new List<JsonData>();
				foreach (KeyValuePair<string, JsonData> keyValuePair in this.object_list)
				{
					list.Add(keyValuePair.Value);
				}
				return (ICollection)list;
			}
		}

		// Token: 0x170008FB RID: 2299
		// (get) Token: 0x06005D10 RID: 23824 RVA: 0x001DCCE0 File Offset: 0x001DAEE0
		bool IJsonWrapper.IsArray
		{
			get
			{
				return this.IsArray;
			}
		}

		// Token: 0x170008FC RID: 2300
		// (get) Token: 0x06005D11 RID: 23825 RVA: 0x001DCCE8 File Offset: 0x001DAEE8
		bool IJsonWrapper.IsBoolean
		{
			get
			{
				return this.IsBoolean;
			}
		}

		// Token: 0x170008FD RID: 2301
		// (get) Token: 0x06005D12 RID: 23826 RVA: 0x001DCCF0 File Offset: 0x001DAEF0
		bool IJsonWrapper.IsDouble
		{
			get
			{
				return this.IsDouble;
			}
		}

		// Token: 0x170008FE RID: 2302
		// (get) Token: 0x06005D13 RID: 23827 RVA: 0x001DCCF8 File Offset: 0x001DAEF8
		bool IJsonWrapper.IsInt
		{
			get
			{
				return this.IsInt;
			}
		}

		// Token: 0x170008FF RID: 2303
		// (get) Token: 0x06005D14 RID: 23828 RVA: 0x001DCD00 File Offset: 0x001DAF00
		bool IJsonWrapper.IsLong
		{
			get
			{
				return this.IsLong;
			}
		}

		// Token: 0x17000900 RID: 2304
		// (get) Token: 0x06005D15 RID: 23829 RVA: 0x001DCD08 File Offset: 0x001DAF08
		bool IJsonWrapper.IsObject
		{
			get
			{
				return this.IsObject;
			}
		}

		// Token: 0x17000901 RID: 2305
		// (get) Token: 0x06005D16 RID: 23830 RVA: 0x001DCD10 File Offset: 0x001DAF10
		bool IJsonWrapper.IsString
		{
			get
			{
				return this.IsString;
			}
		}

		// Token: 0x17000902 RID: 2306
		// (get) Token: 0x06005D17 RID: 23831 RVA: 0x001DCD18 File Offset: 0x001DAF18
		bool IList.IsFixedSize
		{
			get
			{
				return this.EnsureList().IsFixedSize;
			}
		}

		// Token: 0x17000903 RID: 2307
		// (get) Token: 0x06005D18 RID: 23832 RVA: 0x001DCD25 File Offset: 0x001DAF25
		bool IList.IsReadOnly
		{
			get
			{
				return this.EnsureList().IsReadOnly;
			}
		}

		// Token: 0x17000904 RID: 2308
		object IDictionary.this[object key]
		{
			get
			{
				return this.EnsureDictionary()[key];
			}
			set
			{
				if (!(key is string))
				{
					throw new ArgumentException("The key has to be a string");
				}
				JsonData jsonData = this.ToJsonData(value);
				this[(string)key] = jsonData;
			}
		}

		// Token: 0x17000905 RID: 2309
		object IOrderedDictionary.this[int idx]
		{
			get
			{
				this.EnsureDictionary();
				return this.object_list[idx].Value;
			}
			set
			{
				this.EnsureDictionary();
				JsonData jsonData = this.ToJsonData(value);
				KeyValuePair<string, JsonData> keyValuePair = this.object_list[idx];
				this.inst_object[keyValuePair.Key] = jsonData;
				KeyValuePair<string, JsonData> keyValuePair2 = new KeyValuePair<string, JsonData>(keyValuePair.Key, jsonData);
				this.object_list[idx] = keyValuePair2;
			}
		}

		// Token: 0x17000906 RID: 2310
		object IList.this[int index]
		{
			get
			{
				return this.EnsureList()[index];
			}
			set
			{
				this.EnsureList();
				JsonData jsonData = this.ToJsonData(value);
				this[index] = jsonData;
			}
		}

		// Token: 0x17000907 RID: 2311
		public JsonData this[string prop_name]
		{
			get
			{
				this.EnsureDictionary();
				return this.inst_object[prop_name];
			}
			set
			{
				this.EnsureDictionary();
				KeyValuePair<string, JsonData> keyValuePair = new KeyValuePair<string, JsonData>(prop_name, value);
				if (this.inst_object.ContainsKey(prop_name))
				{
					for (int i = 0; i < this.object_list.Count; i++)
					{
						if (this.object_list[i].Key == prop_name)
						{
							this.object_list[i] = keyValuePair;
							break;
						}
					}
				}
				else
				{
					this.object_list.Add(keyValuePair);
				}
				this.inst_object[prop_name] = value;
				this.json = null;
			}
		}

		// Token: 0x17000908 RID: 2312
		public JsonData this[int index]
		{
			get
			{
				this.EnsureCollection();
				if (this.type == JsonType.Array)
				{
					return this.inst_array[index];
				}
				return this.object_list[index].Value;
			}
			set
			{
				this.EnsureCollection();
				if (this.type == JsonType.Array)
				{
					this.inst_array[index] = value;
				}
				else
				{
					KeyValuePair<string, JsonData> keyValuePair = this.object_list[index];
					KeyValuePair<string, JsonData> keyValuePair2 = new KeyValuePair<string, JsonData>(keyValuePair.Key, value);
					this.object_list[index] = keyValuePair2;
					this.inst_object[keyValuePair.Key] = value;
				}
				this.json = null;
			}
		}

		// Token: 0x06005D23 RID: 23843 RVA: 0x00002050 File Offset: 0x00000250
		public JsonData()
		{
		}

		// Token: 0x06005D24 RID: 23844 RVA: 0x001DCF83 File Offset: 0x001DB183
		public JsonData(bool boolean)
		{
			this.type = JsonType.Boolean;
			this.inst_boolean = boolean;
		}

		// Token: 0x06005D25 RID: 23845 RVA: 0x001DCF99 File Offset: 0x001DB199
		public JsonData(double number)
		{
			this.type = JsonType.Double;
			this.inst_double = number;
		}

		// Token: 0x06005D26 RID: 23846 RVA: 0x001DCFAF File Offset: 0x001DB1AF
		public JsonData(int number)
		{
			this.type = JsonType.Int;
			this.inst_int = number;
		}

		// Token: 0x06005D27 RID: 23847 RVA: 0x001DCFC5 File Offset: 0x001DB1C5
		public JsonData(long number)
		{
			this.type = JsonType.Long;
			this.inst_long = number;
		}

		// Token: 0x06005D28 RID: 23848 RVA: 0x001DCFDC File Offset: 0x001DB1DC
		public JsonData(object obj)
		{
			if (obj is bool)
			{
				this.type = JsonType.Boolean;
				this.inst_boolean = (bool)obj;
				return;
			}
			if (obj is double)
			{
				this.type = JsonType.Double;
				this.inst_double = (double)obj;
				return;
			}
			if (obj is int)
			{
				this.type = JsonType.Int;
				this.inst_int = (int)obj;
				return;
			}
			if (obj is long)
			{
				this.type = JsonType.Long;
				this.inst_long = (long)obj;
				return;
			}
			if (obj is string)
			{
				this.type = JsonType.String;
				this.inst_string = (string)obj;
				return;
			}
			throw new ArgumentException("Unable to wrap the given object with JsonData");
		}

		// Token: 0x06005D29 RID: 23849 RVA: 0x001DD085 File Offset: 0x001DB285
		public JsonData(string str)
		{
			this.type = JsonType.String;
			this.inst_string = str;
		}

		// Token: 0x06005D2A RID: 23850 RVA: 0x001DD09B File Offset: 0x001DB29B
		public static implicit operator JsonData(bool data)
		{
			return new JsonData(data);
		}

		// Token: 0x06005D2B RID: 23851 RVA: 0x001DD0A3 File Offset: 0x001DB2A3
		public static implicit operator JsonData(double data)
		{
			return new JsonData(data);
		}

		// Token: 0x06005D2C RID: 23852 RVA: 0x001DD0AB File Offset: 0x001DB2AB
		public static implicit operator JsonData(int data)
		{
			return new JsonData(data);
		}

		// Token: 0x06005D2D RID: 23853 RVA: 0x001DD0B3 File Offset: 0x001DB2B3
		public static implicit operator JsonData(long data)
		{
			return new JsonData(data);
		}

		// Token: 0x06005D2E RID: 23854 RVA: 0x001DD0BB File Offset: 0x001DB2BB
		public static implicit operator JsonData(string data)
		{
			return new JsonData(data);
		}

		// Token: 0x06005D2F RID: 23855 RVA: 0x001DD0C3 File Offset: 0x001DB2C3
		public static explicit operator bool(JsonData data)
		{
			if (data.type != JsonType.Boolean)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a double");
			}
			return data.inst_boolean;
		}

		// Token: 0x06005D30 RID: 23856 RVA: 0x001DD0DF File Offset: 0x001DB2DF
		public static explicit operator double(JsonData data)
		{
			if (data.type != JsonType.Double)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a double");
			}
			return data.inst_double;
		}

		// Token: 0x06005D31 RID: 23857 RVA: 0x001DD0FB File Offset: 0x001DB2FB
		public static explicit operator int(JsonData data)
		{
			if (data.type != JsonType.Int)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold an int");
			}
			return data.inst_int;
		}

		// Token: 0x06005D32 RID: 23858 RVA: 0x001DD117 File Offset: 0x001DB317
		public static explicit operator long(JsonData data)
		{
			if (data.type != JsonType.Long)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold an int");
			}
			return data.inst_long;
		}

		// Token: 0x06005D33 RID: 23859 RVA: 0x001DD133 File Offset: 0x001DB333
		public static explicit operator string(JsonData data)
		{
			if (data.type != JsonType.String)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a string");
			}
			return data.inst_string;
		}

		// Token: 0x06005D34 RID: 23860 RVA: 0x001DD14F File Offset: 0x001DB34F
		void ICollection.CopyTo(Array array, int index)
		{
			this.EnsureCollection().CopyTo(array, index);
		}

		// Token: 0x06005D35 RID: 23861 RVA: 0x001DD160 File Offset: 0x001DB360
		void IDictionary.Add(object key, object value)
		{
			JsonData jsonData = this.ToJsonData(value);
			this.EnsureDictionary().Add(key, jsonData);
			KeyValuePair<string, JsonData> keyValuePair = new KeyValuePair<string, JsonData>((string)key, jsonData);
			this.object_list.Add(keyValuePair);
			this.json = null;
		}

		// Token: 0x06005D36 RID: 23862 RVA: 0x001DD1A3 File Offset: 0x001DB3A3
		void IDictionary.Clear()
		{
			this.EnsureDictionary().Clear();
			this.object_list.Clear();
			this.json = null;
		}

		// Token: 0x06005D37 RID: 23863 RVA: 0x001DD1C2 File Offset: 0x001DB3C2
		bool IDictionary.Contains(object key)
		{
			return this.EnsureDictionary().Contains(key);
		}

		// Token: 0x06005D38 RID: 23864 RVA: 0x001DD1D0 File Offset: 0x001DB3D0
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return ((IOrderedDictionary)this).GetEnumerator();
		}

		// Token: 0x06005D39 RID: 23865 RVA: 0x001DD1D8 File Offset: 0x001DB3D8
		void IDictionary.Remove(object key)
		{
			this.EnsureDictionary().Remove(key);
			for (int i = 0; i < this.object_list.Count; i++)
			{
				if (this.object_list[i].Key == (string)key)
				{
					this.object_list.RemoveAt(i);
					break;
				}
			}
			this.json = null;
		}

		// Token: 0x06005D3A RID: 23866 RVA: 0x001DD23D File Offset: 0x001DB43D
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.EnsureCollection().GetEnumerator();
		}

		// Token: 0x06005D3B RID: 23867 RVA: 0x001DD24A File Offset: 0x001DB44A
		bool IJsonWrapper.GetBoolean()
		{
			if (this.type != JsonType.Boolean)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a boolean");
			}
			return this.inst_boolean;
		}

		// Token: 0x06005D3C RID: 23868 RVA: 0x001DD266 File Offset: 0x001DB466
		double IJsonWrapper.GetDouble()
		{
			if (this.type != JsonType.Double)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a double");
			}
			return this.inst_double;
		}

		// Token: 0x06005D3D RID: 23869 RVA: 0x001DD282 File Offset: 0x001DB482
		int IJsonWrapper.GetInt()
		{
			if (this.type != JsonType.Int)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold an int");
			}
			return this.inst_int;
		}

		// Token: 0x06005D3E RID: 23870 RVA: 0x001DD29E File Offset: 0x001DB49E
		long IJsonWrapper.GetLong()
		{
			if (this.type != JsonType.Long)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a long");
			}
			return this.inst_long;
		}

		// Token: 0x06005D3F RID: 23871 RVA: 0x001DD2BA File Offset: 0x001DB4BA
		string IJsonWrapper.GetString()
		{
			if (this.type != JsonType.String)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a string");
			}
			return this.inst_string;
		}

		// Token: 0x06005D40 RID: 23872 RVA: 0x001DD2D6 File Offset: 0x001DB4D6
		void IJsonWrapper.SetBoolean(bool val)
		{
			this.type = JsonType.Boolean;
			this.inst_boolean = val;
			this.json = null;
		}

		// Token: 0x06005D41 RID: 23873 RVA: 0x001DD2ED File Offset: 0x001DB4ED
		void IJsonWrapper.SetDouble(double val)
		{
			this.type = JsonType.Double;
			this.inst_double = val;
			this.json = null;
		}

		// Token: 0x06005D42 RID: 23874 RVA: 0x001DD304 File Offset: 0x001DB504
		void IJsonWrapper.SetInt(int val)
		{
			this.type = JsonType.Int;
			this.inst_int = val;
			this.json = null;
		}

		// Token: 0x06005D43 RID: 23875 RVA: 0x001DD31B File Offset: 0x001DB51B
		void IJsonWrapper.SetLong(long val)
		{
			this.type = JsonType.Long;
			this.inst_long = val;
			this.json = null;
		}

		// Token: 0x06005D44 RID: 23876 RVA: 0x001DD332 File Offset: 0x001DB532
		void IJsonWrapper.SetString(string val)
		{
			this.type = JsonType.String;
			this.inst_string = val;
			this.json = null;
		}

		// Token: 0x06005D45 RID: 23877 RVA: 0x001DD349 File Offset: 0x001DB549
		string IJsonWrapper.ToJson()
		{
			return this.ToJson();
		}

		// Token: 0x06005D46 RID: 23878 RVA: 0x001DD351 File Offset: 0x001DB551
		void IJsonWrapper.ToJson(JsonWriter writer)
		{
			this.ToJson(writer);
		}

		// Token: 0x06005D47 RID: 23879 RVA: 0x001DD35A File Offset: 0x001DB55A
		int IList.Add(object value)
		{
			return this.Add(value);
		}

		// Token: 0x06005D48 RID: 23880 RVA: 0x001DD363 File Offset: 0x001DB563
		void IList.Clear()
		{
			this.EnsureList().Clear();
			this.json = null;
		}

		// Token: 0x06005D49 RID: 23881 RVA: 0x001DD377 File Offset: 0x001DB577
		bool IList.Contains(object value)
		{
			return this.EnsureList().Contains(value);
		}

		// Token: 0x06005D4A RID: 23882 RVA: 0x001DD385 File Offset: 0x001DB585
		int IList.IndexOf(object value)
		{
			return this.EnsureList().IndexOf(value);
		}

		// Token: 0x06005D4B RID: 23883 RVA: 0x001DD393 File Offset: 0x001DB593
		void IList.Insert(int index, object value)
		{
			this.EnsureList().Insert(index, value);
			this.json = null;
		}

		// Token: 0x06005D4C RID: 23884 RVA: 0x001DD3A9 File Offset: 0x001DB5A9
		void IList.Remove(object value)
		{
			this.EnsureList().Remove(value);
			this.json = null;
		}

		// Token: 0x06005D4D RID: 23885 RVA: 0x001DD3BE File Offset: 0x001DB5BE
		void IList.RemoveAt(int index)
		{
			this.EnsureList().RemoveAt(index);
			this.json = null;
		}

		// Token: 0x06005D4E RID: 23886 RVA: 0x001DD3D3 File Offset: 0x001DB5D3
		IDictionaryEnumerator IOrderedDictionary.GetEnumerator()
		{
			this.EnsureDictionary();
			return new OrderedDictionaryEnumerator(this.object_list.GetEnumerator());
		}

		// Token: 0x06005D4F RID: 23887 RVA: 0x001DD3EC File Offset: 0x001DB5EC
		void IOrderedDictionary.Insert(int idx, object key, object value)
		{
			string text = (string)key;
			JsonData jsonData = this.ToJsonData(value);
			this[text] = jsonData;
			KeyValuePair<string, JsonData> keyValuePair = new KeyValuePair<string, JsonData>(text, jsonData);
			this.object_list.Insert(idx, keyValuePair);
		}

		// Token: 0x06005D50 RID: 23888 RVA: 0x001DD428 File Offset: 0x001DB628
		void IOrderedDictionary.RemoveAt(int idx)
		{
			this.EnsureDictionary();
			this.inst_object.Remove(this.object_list[idx].Key);
			this.object_list.RemoveAt(idx);
		}

		// Token: 0x06005D51 RID: 23889 RVA: 0x001DD468 File Offset: 0x001DB668
		private ICollection EnsureCollection()
		{
			if (this.type == JsonType.Array)
			{
				return (ICollection)this.inst_array;
			}
			if (this.type == JsonType.Object)
			{
				return (ICollection)this.inst_object;
			}
			throw new InvalidOperationException("The JsonData instance has to be initialized first");
		}

		// Token: 0x06005D52 RID: 23890 RVA: 0x001DD4A0 File Offset: 0x001DB6A0
		private IDictionary EnsureDictionary()
		{
			if (this.type == JsonType.Object)
			{
				return (IDictionary)this.inst_object;
			}
			if (this.type != JsonType.None)
			{
				throw new InvalidOperationException("Instance of JsonData is not a dictionary");
			}
			this.type = JsonType.Object;
			this.inst_object = new Dictionary<string, JsonData>();
			this.object_list = new List<KeyValuePair<string, JsonData>>();
			return (IDictionary)this.inst_object;
		}

		// Token: 0x06005D53 RID: 23891 RVA: 0x001DD500 File Offset: 0x001DB700
		private IList EnsureList()
		{
			if (this.type == JsonType.Array)
			{
				return (IList)this.inst_array;
			}
			if (this.type != JsonType.None)
			{
				throw new InvalidOperationException("Instance of JsonData is not a list");
			}
			this.type = JsonType.Array;
			this.inst_array = new List<JsonData>();
			return (IList)this.inst_array;
		}

		// Token: 0x06005D54 RID: 23892 RVA: 0x001DD552 File Offset: 0x001DB752
		private JsonData ToJsonData(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (obj is JsonData)
			{
				return (JsonData)obj;
			}
			return new JsonData(obj);
		}

		// Token: 0x06005D55 RID: 23893 RVA: 0x001DD570 File Offset: 0x001DB770
		private static void WriteJson(IJsonWrapper obj, JsonWriter writer)
		{
			if (obj.IsString)
			{
				writer.Write(obj.GetString());
				return;
			}
			if (obj.IsBoolean)
			{
				writer.Write(obj.GetBoolean());
				return;
			}
			if (obj.IsDouble)
			{
				writer.Write(obj.GetDouble());
				return;
			}
			if (obj.IsInt)
			{
				writer.Write(obj.GetInt());
				return;
			}
			if (obj.IsLong)
			{
				writer.Write(obj.GetLong());
				return;
			}
			if (obj.IsArray)
			{
				writer.WriteArrayStart();
				foreach (object obj2 in obj)
				{
					JsonData.WriteJson((JsonData)obj2, writer);
				}
				writer.WriteArrayEnd();
				return;
			}
			if (obj.IsObject)
			{
				writer.WriteObjectStart();
				foreach (object obj3 in obj)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj3;
					writer.WritePropertyName((string)dictionaryEntry.Key);
					JsonData.WriteJson((JsonData)dictionaryEntry.Value, writer);
				}
				writer.WriteObjectEnd();
				return;
			}
		}

		// Token: 0x06005D56 RID: 23894 RVA: 0x001DD6B8 File Offset: 0x001DB8B8
		public int Add(object value)
		{
			JsonData jsonData = this.ToJsonData(value);
			this.json = null;
			return this.EnsureList().Add(jsonData);
		}

		// Token: 0x06005D57 RID: 23895 RVA: 0x001DD6E0 File Offset: 0x001DB8E0
		public void Clear()
		{
			if (this.IsObject)
			{
				((IDictionary)this).Clear();
				return;
			}
			if (this.IsArray)
			{
				((IList)this).Clear();
				return;
			}
		}

		// Token: 0x06005D58 RID: 23896 RVA: 0x001DD700 File Offset: 0x001DB900
		public bool Equals(JsonData x)
		{
			if (x == null)
			{
				return false;
			}
			if (x.type != this.type)
			{
				return false;
			}
			switch (this.type)
			{
			case JsonType.None:
				return true;
			case JsonType.Object:
				return this.inst_object.Equals(x.inst_object);
			case JsonType.Array:
				return this.inst_array.Equals(x.inst_array);
			case JsonType.String:
				return this.inst_string.Equals(x.inst_string);
			case JsonType.Int:
				return this.inst_int.Equals(x.inst_int);
			case JsonType.Long:
				return this.inst_long.Equals(x.inst_long);
			case JsonType.Double:
				return this.inst_double.Equals(x.inst_double);
			case JsonType.Boolean:
				return this.inst_boolean.Equals(x.inst_boolean);
			default:
				return false;
			}
		}

		// Token: 0x06005D59 RID: 23897 RVA: 0x001DD7D5 File Offset: 0x001DB9D5
		public JsonType GetJsonType()
		{
			return this.type;
		}

		// Token: 0x06005D5A RID: 23898 RVA: 0x001DD7E0 File Offset: 0x001DB9E0
		public void SetJsonType(JsonType type)
		{
			if (this.type == type)
			{
				return;
			}
			switch (type)
			{
			case JsonType.Object:
				this.inst_object = new Dictionary<string, JsonData>();
				this.object_list = new List<KeyValuePair<string, JsonData>>();
				break;
			case JsonType.Array:
				this.inst_array = new List<JsonData>();
				break;
			case JsonType.String:
				this.inst_string = null;
				break;
			case JsonType.Int:
				this.inst_int = 0;
				break;
			case JsonType.Long:
				this.inst_long = 0L;
				break;
			case JsonType.Double:
				this.inst_double = 0.0;
				break;
			case JsonType.Boolean:
				this.inst_boolean = false;
				break;
			}
			this.type = type;
		}

		// Token: 0x06005D5B RID: 23899 RVA: 0x001DD880 File Offset: 0x001DBA80
		public string ToJson()
		{
			if (this.json != null)
			{
				return this.json;
			}
			StringWriter stringWriter = new StringWriter();
			JsonData.WriteJson(this, new JsonWriter(stringWriter)
			{
				Validate = false
			});
			this.json = stringWriter.ToString();
			return this.json;
		}

		// Token: 0x06005D5C RID: 23900 RVA: 0x001DD8CC File Offset: 0x001DBACC
		public void ToJson(JsonWriter writer)
		{
			bool validate = writer.Validate;
			writer.Validate = false;
			JsonData.WriteJson(this, writer);
			writer.Validate = validate;
		}

		// Token: 0x06005D5D RID: 23901 RVA: 0x001DD8F8 File Offset: 0x001DBAF8
		public override string ToString()
		{
			switch (this.type)
			{
			case JsonType.Object:
				return "JsonData object";
			case JsonType.Array:
				return "JsonData array";
			case JsonType.String:
				return this.inst_string;
			case JsonType.Int:
				return this.inst_int.ToString();
			case JsonType.Long:
				return this.inst_long.ToString();
			case JsonType.Double:
				return this.inst_double.ToString();
			case JsonType.Boolean:
				return this.inst_boolean.ToString();
			default:
				return "Uninitialized JsonData";
			}
		}

		// Token: 0x04006CB8 RID: 27832
		private IList<JsonData> inst_array;

		// Token: 0x04006CB9 RID: 27833
		private bool inst_boolean;

		// Token: 0x04006CBA RID: 27834
		private double inst_double;

		// Token: 0x04006CBB RID: 27835
		private int inst_int;

		// Token: 0x04006CBC RID: 27836
		private long inst_long;

		// Token: 0x04006CBD RID: 27837
		private IDictionary<string, JsonData> inst_object;

		// Token: 0x04006CBE RID: 27838
		private string inst_string;

		// Token: 0x04006CBF RID: 27839
		private string json;

		// Token: 0x04006CC0 RID: 27840
		private JsonType type;

		// Token: 0x04006CC1 RID: 27841
		private IList<KeyValuePair<string, JsonData>> object_list;
	}
}
