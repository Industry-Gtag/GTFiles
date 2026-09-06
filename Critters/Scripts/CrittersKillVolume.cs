using System;
using GorillaExtensions;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x020013ED RID: 5101
	public class CrittersKillVolume : MonoBehaviour
	{
		// Token: 0x060080B8 RID: 32952 RVA: 0x0029DA50 File Offset: 0x0029BC50
		private void OnTriggerEnter(Collider other)
		{
			if (other.attachedRigidbody)
			{
				CrittersActor component = other.attachedRigidbody.GetComponent<CrittersActor>();
				if (component.IsNotNull())
				{
					component.gameObject.SetActive(false);
				}
			}
		}
	}
}
