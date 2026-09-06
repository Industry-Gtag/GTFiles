using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001205 RID: 4613
	[CreateAssetMenu(fileName = "WatchableIntSO", menuName = "ScriptableObjects/WatchableIntSO")]
	public class WatchableIntSO : WatchableGenericSO<int>
	{
		// Token: 0x17000B55 RID: 2901
		// (get) Token: 0x060074F8 RID: 29944 RVA: 0x0025FBEA File Offset: 0x0025DDEA
		private int currentValue
		{
			get
			{
				return base.Value;
			}
		}
	}
}
