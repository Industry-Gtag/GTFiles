using System;
using System.Collections;
using System.Collections.Generic;

namespace LitJson
{
	// Token: 0x02000EEB RID: 3819
	internal class OrderedDictionaryEnumerator : IDictionaryEnumerator, IEnumerator
	{
		// Token: 0x17000909 RID: 2313
		// (get) Token: 0x06005D5E RID: 23902 RVA: 0x001DD97A File Offset: 0x001DBB7A
		public object Current
		{
			get
			{
				return this.Entry;
			}
		}

		// Token: 0x1700090A RID: 2314
		// (get) Token: 0x06005D5F RID: 23903 RVA: 0x001DD988 File Offset: 0x001DBB88
		public DictionaryEntry Entry
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return new DictionaryEntry(keyValuePair.Key, keyValuePair.Value);
			}
		}

		// Token: 0x1700090B RID: 2315
		// (get) Token: 0x06005D60 RID: 23904 RVA: 0x001DD9B4 File Offset: 0x001DBBB4
		public object Key
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return keyValuePair.Key;
			}
		}

		// Token: 0x1700090C RID: 2316
		// (get) Token: 0x06005D61 RID: 23905 RVA: 0x001DD9D4 File Offset: 0x001DBBD4
		public object Value
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return keyValuePair.Value;
			}
		}

		// Token: 0x06005D62 RID: 23906 RVA: 0x001DD9F4 File Offset: 0x001DBBF4
		public OrderedDictionaryEnumerator(IEnumerator<KeyValuePair<string, JsonData>> enumerator)
		{
			this.list_enumerator = enumerator;
		}

		// Token: 0x06005D63 RID: 23907 RVA: 0x001DDA03 File Offset: 0x001DBC03
		public bool MoveNext()
		{
			return this.list_enumerator.MoveNext();
		}

		// Token: 0x06005D64 RID: 23908 RVA: 0x001DDA10 File Offset: 0x001DBC10
		public void Reset()
		{
			this.list_enumerator.Reset();
		}

		// Token: 0x04006CC2 RID: 27842
		private IEnumerator<KeyValuePair<string, JsonData>> list_enumerator;
	}
}
