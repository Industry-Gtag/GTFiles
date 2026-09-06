using System;
using UnityEngine.Events;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	// Token: 0x0200129C RID: 4764
	[Serializable]
	public class ExclusionZoneStateEvent<T> : ZoneStateEventBase
	{
		// Token: 0x060077D3 RID: 30675 RVA: 0x0026CD32 File Offset: 0x0026AF32
		public void Invoke(VRRig vrRig, T arg)
		{
			if (base.IsRestricted(vrRig))
			{
				ExclusionZoneStateEvent<T>.TypedEvent typedEvent = this.onRestricted;
				if (typedEvent == null)
				{
					return;
				}
				typedEvent.Invoke(arg);
				return;
			}
			else
			{
				ExclusionZoneStateEvent<T>.TypedEvent typedEvent2 = this.onNormal;
				if (typedEvent2 == null)
				{
					return;
				}
				typedEvent2.Invoke(arg);
				return;
			}
		}

		// Token: 0x0400880A RID: 34826
		public ExclusionZoneStateEvent<T>.TypedEvent onNormal;

		// Token: 0x0400880B RID: 34827
		public ExclusionZoneStateEvent<T>.TypedEvent onRestricted;

		// Token: 0x0200129D RID: 4765
		[Serializable]
		public class TypedEvent : UnityEvent<T>
		{
		}
	}
}
