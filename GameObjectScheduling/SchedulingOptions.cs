using System;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x020013FE RID: 5118
	[CreateAssetMenu(fileName = "New Options", menuName = "Game Object Scheduling/Options", order = 0)]
	public class SchedulingOptions : ScriptableObject
	{
		// Token: 0x17000C6A RID: 3178
		// (get) Token: 0x06008104 RID: 33028 RVA: 0x0029EB46 File Offset: 0x0029CD46
		public DateTime DtDebugServerTime
		{
			get
			{
				return this.dtDebugServerTime.AddSeconds((double)(Time.time * this.timescale));
			}
		}

		// Token: 0x04009201 RID: 37377
		[SerializeField]
		private string debugServerTime;

		// Token: 0x04009202 RID: 37378
		[SerializeField]
		private DateTime dtDebugServerTime;

		// Token: 0x04009203 RID: 37379
		[SerializeField]
		[Range(-60f, 3660f)]
		private float timescale = 1f;
	}
}
