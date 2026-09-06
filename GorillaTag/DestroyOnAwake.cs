using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x020011E9 RID: 4585
	public class DestroyOnAwake : MonoBehaviour
	{
		// Token: 0x0600748F RID: 29839 RVA: 0x0025E40C File Offset: 0x0025C60C
		protected void Awake()
		{
			try
			{
				Object.Destroy(base.gameObject);
			}
			catch
			{
			}
		}

		// Token: 0x06007490 RID: 29840 RVA: 0x0025E43C File Offset: 0x0025C63C
		protected void OnEnable()
		{
			try
			{
				Object.Destroy(base.gameObject);
			}
			catch
			{
			}
		}

		// Token: 0x06007491 RID: 29841 RVA: 0x0025E46C File Offset: 0x0025C66C
		protected void Update()
		{
			try
			{
				Object.Destroy(base.gameObject);
			}
			catch
			{
			}
		}
	}
}
