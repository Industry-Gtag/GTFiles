using System;
using UnityEngine;

namespace GorillaNetworking.ScheduledEvents
{
	// Token: 0x0200112A RID: 4394
	public class ScheduledEventControlledObject : MonoBehaviour
	{
		// Token: 0x06006E25 RID: 28197 RVA: 0x0023874C File Offset: 0x0023694C
		private void Start()
		{
			ScheduledEventManager instance = ScheduledEventManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.Register(this);
		}

		// Token: 0x06006E26 RID: 28198 RVA: 0x0023875E File Offset: 0x0023695E
		private void OnDestroy()
		{
			if (ScheduledEventManager.Instance != null)
			{
				ScheduledEventManager.Instance.Unregister(this);
			}
		}

		// Token: 0x06006E27 RID: 28199 RVA: 0x00238778 File Offset: 0x00236978
		public bool MatchesPhase(ScheduledEventPhase phase)
		{
			switch (phase)
			{
			case ScheduledEventPhase.Before:
				return this.enableBefore;
			case ScheduledEventPhase.During:
				return this.enableDuring;
			case ScheduledEventPhase.After:
				return this.enableAfter;
			case ScheduledEventPhase.NoEvent:
				return this.enableIfNoEvent;
			default:
				return false;
			}
		}

		// Token: 0x04007E6D RID: 32365
		[Tooltip("Active while waiting for the event to start (also the default state during initial scene load / sync).")]
		public bool enableBefore;

		// Token: 0x04007E6E RID: 32366
		[Tooltip("Active while the event is playing in this room.")]
		public bool enableDuring;

		// Token: 0x04007E6F RID: 32367
		[Tooltip("Active after the event has finished in this room, or in post-event rooms where the player missed it.")]
		public bool enableAfter;

		// Token: 0x04007E70 RID: 32368
		[Tooltip("Active when no scheduled event is configured at all (manager has no titleDataKey). Use for the ordinary stage look.")]
		public bool enableIfNoEvent;
	}
}
