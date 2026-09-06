using System;
using UnityEngine.Events;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	// Token: 0x0200129B RID: 4763
	[Serializable]
	public class ExclusionZoneStateEvent
	{
		// Token: 0x060077D1 RID: 30673 RVA: 0x0026CD07 File Offset: 0x0026AF07
		public void Invoke(VRRig vrRig)
		{
			if (CosmeticExclusionZoneRegistry.IsRestricted(vrRig))
			{
				UnityEvent onRestricted = this.OnRestricted;
				if (onRestricted == null)
				{
					return;
				}
				onRestricted.Invoke();
				return;
			}
			else
			{
				UnityEvent onNormal = this.OnNormal;
				if (onNormal == null)
				{
					return;
				}
				onNormal.Invoke();
				return;
			}
		}

		// Token: 0x04008808 RID: 34824
		public UnityEvent OnNormal;

		// Token: 0x04008809 RID: 34825
		public UnityEvent OnRestricted;
	}
}
