using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x020011FA RID: 4602
	public class DeactivateOnAwake : MonoBehaviour
	{
		// Token: 0x060074C7 RID: 29895 RVA: 0x0025EC4D File Offset: 0x0025CE4D
		private void Awake()
		{
			base.gameObject.SetActive(false);
			Object.Destroy(this);
		}
	}
}
