using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaUtil
{
	// Token: 0x02000F02 RID: 3842
	[CreateAssetMenu(fileName = "StringTable", menuName = "Scriptable Objects/StringTable")]
	public class StringTable : ScriptableObject
	{
		// Token: 0x17000922 RID: 2338
		// (get) Token: 0x06005E2E RID: 24110 RVA: 0x001E14DB File Offset: 0x001DF6DB
		public int Count
		{
			get
			{
				return this.entries.Length;
			}
		}

		// Token: 0x17000923 RID: 2339
		// (get) Token: 0x06005E2F RID: 24111 RVA: 0x001E14E5 File Offset: 0x001DF6E5
		public string KeyList
		{
			get
			{
				if (this.keyList.IsNullOrEmpty() && this.entries.Length != 0)
				{
					return this.buildKeyList();
				}
				return this.keyList;
			}
		}

		// Token: 0x06005E30 RID: 24112 RVA: 0x001E150C File Offset: 0x001DF70C
		private string buildKeyList()
		{
			this.keyList = string.Empty;
			if (this.entries.Length != 0)
			{
				for (int i = 0; i < this.entries.Length - 1; i++)
				{
					this.keyList = this.keyList + this.entries[i].Key + ", ";
				}
				this.keyList += this.entries[this.entries.Length - 1].Key;
			}
			return this.keyList;
		}

		// Token: 0x06005E31 RID: 24113 RVA: 0x001E159C File Offset: 0x001DF79C
		public bool ContainsKey(string key)
		{
			if (this.dict == null)
			{
				this.dict = new Dictionary<string, string>();
				for (int i = 0; i < this.entries.Length; i++)
				{
					this.dict.Add(this.entries[i].Key, this.entries[i].Value);
				}
			}
			return this.dict.ContainsKey(key);
		}

		// Token: 0x06005E32 RID: 24114 RVA: 0x001E1608 File Offset: 0x001DF808
		public string FetchValue(string key)
		{
			if (this.ContainsKey(key))
			{
				return this.dict[key];
			}
			return null;
		}

		// Token: 0x04006D50 RID: 27984
		[SerializeField]
		private StringTable.StringPair[] entries;

		// Token: 0x04006D51 RID: 27985
		private Dictionary<string, string> dict;

		// Token: 0x04006D52 RID: 27986
		private string keyList;

		// Token: 0x02000F03 RID: 3843
		[Serializable]
		private struct StringPair
		{
			// Token: 0x04006D53 RID: 27987
			public string Key;

			// Token: 0x04006D54 RID: 27988
			public string Value;
		}
	}
}
