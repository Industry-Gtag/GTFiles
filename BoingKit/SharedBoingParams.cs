using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02001454 RID: 5204
	[CreateAssetMenu(fileName = "BoingParams", menuName = "Boing Kit/Shared Boing Params", order = 550)]
	public class SharedBoingParams : ScriptableObject
	{
		// Token: 0x0600832A RID: 33578 RVA: 0x002AF88C File Offset: 0x002ADA8C
		public SharedBoingParams()
		{
			this.Params.Init();
		}

		// Token: 0x0400944F RID: 37967
		public BoingWork.Params Params;
	}
}
