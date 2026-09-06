using System;
using UnityEngine;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x020011B7 RID: 4535
	public class HandHoldXSceneRef : MonoBehaviour
	{
		// Token: 0x17000B34 RID: 2868
		// (get) Token: 0x0600726D RID: 29293 RVA: 0x00254B40 File Offset: 0x00252D40
		public HandHold target
		{
			get
			{
				HandHold handHold;
				if (this.reference.TryResolve<HandHold>(out handHold))
				{
					return handHold;
				}
				return null;
			}
		}

		// Token: 0x17000B35 RID: 2869
		// (get) Token: 0x0600726E RID: 29294 RVA: 0x00254B60 File Offset: 0x00252D60
		public GameObject targetObject
		{
			get
			{
				GameObject gameObject;
				if (this.reference.TryResolve(out gameObject))
				{
					return gameObject;
				}
				return null;
			}
		}

		// Token: 0x04008341 RID: 33601
		[SerializeField]
		public XSceneRef reference;
	}
}
