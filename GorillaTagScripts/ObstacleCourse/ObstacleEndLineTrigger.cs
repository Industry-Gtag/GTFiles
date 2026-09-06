using System;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02001010 RID: 4112
	public class ObstacleEndLineTrigger : MonoBehaviour
	{
		// Token: 0x140000AB RID: 171
		// (add) Token: 0x0600667C RID: 26236 RVA: 0x0020EF28 File Offset: 0x0020D128
		// (remove) Token: 0x0600667D RID: 26237 RVA: 0x0020EF60 File Offset: 0x0020D160
		public event ObstacleEndLineTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerEnter;

		// Token: 0x0600667E RID: 26238 RVA: 0x0020EF98 File Offset: 0x0020D198
		private void OnTriggerEnter(Collider other)
		{
			VRRig vrrig;
			if (other.attachedRigidbody.gameObject.TryGetComponent<VRRig>(out vrrig))
			{
				ObstacleEndLineTrigger.ObstacleCourseTriggerEvent onPlayerTriggerEnter = this.OnPlayerTriggerEnter;
				if (onPlayerTriggerEnter == null)
				{
					return;
				}
				onPlayerTriggerEnter(vrrig);
			}
		}

		// Token: 0x02001011 RID: 4113
		// (Invoke) Token: 0x06006681 RID: 26241
		public delegate void ObstacleCourseTriggerEvent(VRRig vrrig);
	}
}
