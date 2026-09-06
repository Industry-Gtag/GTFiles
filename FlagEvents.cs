using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000590 RID: 1424
[Serializable]
public class FlagEvents<T> where T : Enum
{
	// Token: 0x06002414 RID: 9236 RVA: 0x000C2220 File Offset: 0x000C0420
	public void InvokeAll(T test, bool isLocal = false)
	{
		int num = Convert.ToInt32(test);
		for (int i = 0; i < this.list.Length; i++)
		{
			if ((num & this.list[i].flagsAsInt) != 0 && (!this.list[i].runOnlyLocally || isLocal))
			{
				UnityEvent anyFlagTrue = this.list[i].anyFlagTrue;
				if (anyFlagTrue != null)
				{
					anyFlagTrue.Invoke();
				}
			}
		}
	}

	// Token: 0x04002F54 RID: 12116
	[SerializeField]
	private FlagEvents<T>.FlagEvent[] list;

	// Token: 0x02000591 RID: 1425
	[Serializable]
	private class FlagEvent : ISerializationCallbackReceiver
	{
		// Token: 0x170003CC RID: 972
		// (get) Token: 0x06002416 RID: 9238 RVA: 0x000C2289 File Offset: 0x000C0489
		private string FlagsLabel
		{
			get
			{
				return typeof(T).Name;
			}
		}

		// Token: 0x06002417 RID: 9239 RVA: 0x000C229A File Offset: 0x000C049A
		public void OnBeforeSerialize()
		{
			this.flagsAsInt = Convert.ToInt32(this.flags);
		}

		// Token: 0x06002418 RID: 9240 RVA: 0x000C22B2 File Offset: 0x000C04B2
		public void OnAfterDeserialize()
		{
			this.flags = (T)((object)this.flagsAsInt);
		}

		// Token: 0x04002F55 RID: 12117
		public string debugName = "Any flag true";

		// Token: 0x04002F56 RID: 12118
		[Tooltip("Check this box if only the local player is supposed to run this event.")]
		public bool runOnlyLocally;

		// Token: 0x04002F57 RID: 12119
		private T flags;

		// Token: 0x04002F58 RID: 12120
		[HideInInspector]
		public int flagsAsInt;

		// Token: 0x04002F59 RID: 12121
		public UnityEvent anyFlagTrue;
	}
}
