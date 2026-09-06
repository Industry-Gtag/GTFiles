using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002A2 RID: 674
[Serializable]
public class GTEnumValueMap<T> : ISerializationCallbackReceiver
{
	// Token: 0x060011C1 RID: 4545 RVA: 0x0005F61D File Offset: 0x0005D81D
	public bool TryGet(long i, out T o)
	{
		return this._enumValue_to_unityObject.TryGetValue(i, out o);
	}

	// Token: 0x170001BD RID: 445
	// (get) Token: 0x060011C2 RID: 4546 RVA: 0x0005F62C File Offset: 0x0005D82C
	public IEnumerable<T> Values
	{
		get
		{
			return this._enumValue_to_unityObject.Values;
		}
	}

	// Token: 0x060011C3 RID: 4547 RVA: 0x00002C2D File Offset: 0x00000E2D
	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
	}

	// Token: 0x060011C4 RID: 4548 RVA: 0x0005F639 File Offset: 0x0005D839
	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		this.Init();
	}

	// Token: 0x060011C5 RID: 4549 RVA: 0x0005F644 File Offset: 0x0005D844
	public void Init()
	{
		if (this.m_enumValueAndUnityObjectPairs == null)
		{
			return;
		}
		if (this._enumValue_to_unityObject == null)
		{
			this._enumValue_to_unityObject = new Dictionary<long, T>();
		}
		this._enumValue_to_unityObject.Clear();
		foreach (GTEnumValueMap<T>.EnumValueToUnityObject enumValueToUnityObject in this.m_enumValueAndUnityObjectPairs)
		{
			if (enumValueToUnityObject.enabled && enumValueToUnityObject.value != null)
			{
				this._enumValue_to_unityObject[enumValueToUnityObject.enumKey] = enumValueToUnityObject.value;
			}
		}
		if (!Application.isEditor)
		{
			this.m_enumScriptGuid = null;
			this.m_enumValueAndUnityObjectPairs = null;
		}
	}

	// Token: 0x04001546 RID: 5446
	private Dictionary<long, T> _enumValue_to_unityObject = new Dictionary<long, T>();

	// Token: 0x04001547 RID: 5447
	[Tooltip("The GUID to the Enum script asset which is what is serialized in editor (not used at runtime). This is exposed and editable as a precaution but shouldn't be necessary to have to use.")]
	[SerializeField]
	private string m_enumScriptGuid;

	// Token: 0x04001548 RID: 5448
	[SerializeField]
	private List<GTEnumValueMap<T>.EnumValueToUnityObject> m_enumValueAndUnityObjectPairs = new List<GTEnumValueMap<T>.EnumValueToUnityObject>();

	// Token: 0x020002A3 RID: 675
	[Serializable]
	private struct EnumValueToUnityObject
	{
		// Token: 0x04001549 RID: 5449
		public bool enabled;

		// Token: 0x0400154A RID: 5450
		public long enumKey;

		// Token: 0x0400154B RID: 5451
		public string enumName;

		// Token: 0x0400154C RID: 5452
		public T value;
	}
}
