using System;
using UnityEngine.Events;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	// Token: 0x0200129E RID: 4766
	[Serializable]
	public class ExclusionZoneStateEvent<T0, T1> : ZoneStateEventBase
	{
		// Token: 0x060077D6 RID: 30678 RVA: 0x0026CD70 File Offset: 0x0026AF70
		public void Invoke(VRRig vrRig, T0 arg0, T1 arg1)
		{
			if (base.IsRestricted(vrRig))
			{
				ExclusionZoneStateEvent<T0, T1>.TypedEvent typedEvent = this.onRestricted;
				if (typedEvent == null)
				{
					return;
				}
				typedEvent.Invoke(arg0, arg1);
				return;
			}
			else
			{
				ExclusionZoneStateEvent<T0, T1>.TypedEvent typedEvent2 = this.onNormal;
				if (typedEvent2 == null)
				{
					return;
				}
				typedEvent2.Invoke(arg0, arg1);
				return;
			}
		}

		// Token: 0x0400880C RID: 34828
		public ExclusionZoneStateEvent<T0, T1>.TypedEvent onNormal;

		// Token: 0x0400880D RID: 34829
		public ExclusionZoneStateEvent<T0, T1>.TypedEvent onRestricted;

		// Token: 0x0200129F RID: 4767
		[Serializable]
		public class TypedEvent : UnityEvent<T0, T1>
		{
		}
	}
}
