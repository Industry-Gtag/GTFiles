using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000381 RID: 897
[Serializable]
public class GTSerializableDict<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver where TKey : IComparable<TKey>
{
	// Token: 0x060015ED RID: 5613 RVA: 0x000744C8 File Offset: 0x000726C8
	public void OnBeforeSerialize()
	{
		this._m_serializedEntries.Clear();
		foreach (KeyValuePair<TKey, TValue> keyValuePair in this)
		{
			this._m_serializedEntries.Add(new GTSerializableKeyValue<TKey, TValue>(keyValuePair.Key, keyValuePair.Value));
		}
		this._m_serializedEntries.Sort((GTSerializableKeyValue<TKey, TValue> entry1, GTSerializableKeyValue<TKey, TValue> entry2) => entry1.k.CompareTo(entry2.k));
	}

	// Token: 0x060015EE RID: 5614 RVA: 0x00074564 File Offset: 0x00072764
	public void OnAfterDeserialize()
	{
		base.Clear();
		foreach (GTSerializableKeyValue<TKey, TValue> gtserializableKeyValue in this._m_serializedEntries)
		{
			try
			{
				base.Add(gtserializableKeyValue.k, gtserializableKeyValue.v);
			}
			catch (ArgumentException ex)
			{
				Debug.LogError("ERROR!!! GTSerializableDict: " + string.Format("Duplicate key found during deserialization: '{0}'. Ignoring duplicate. ", gtserializableKeyValue.k) + "Exception: " + ex.Message);
			}
		}
		this._m_serializedEntries.Clear();
	}

	// Token: 0x04001AC4 RID: 6852
	[SerializeField]
	[HideInInspector]
	private List<GTSerializableKeyValue<TKey, TValue>> _m_serializedEntries = new List<GTSerializableKeyValue<TKey, TValue>>();
}
