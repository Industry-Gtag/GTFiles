using System;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x0200100E RID: 4110
	public class ObstacleCourseZoneTrigger : MonoBehaviour
	{
		// Token: 0x140000A9 RID: 169
		// (add) Token: 0x06006671 RID: 26225 RVA: 0x0020EDD8 File Offset: 0x0020CFD8
		// (remove) Token: 0x06006672 RID: 26226 RVA: 0x0020EE10 File Offset: 0x0020D010
		public event ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerEnter;

		// Token: 0x140000AA RID: 170
		// (add) Token: 0x06006673 RID: 26227 RVA: 0x0020EE48 File Offset: 0x0020D048
		// (remove) Token: 0x06006674 RID: 26228 RVA: 0x0020EE80 File Offset: 0x0020D080
		public event ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerExit;

		// Token: 0x06006675 RID: 26229 RVA: 0x0020EEB5 File Offset: 0x0020D0B5
		private void OnTriggerEnter(Collider other)
		{
			if (!other.GetComponent<SphereCollider>())
			{
				return;
			}
			if (other.attachedRigidbody.gameObject.CompareTag("GorillaPlayer"))
			{
				ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent onPlayerTriggerEnter = this.OnPlayerTriggerEnter;
				if (onPlayerTriggerEnter == null)
				{
					return;
				}
				onPlayerTriggerEnter(other);
			}
		}

		// Token: 0x06006676 RID: 26230 RVA: 0x0020EEED File Offset: 0x0020D0ED
		private void OnTriggerExit(Collider other)
		{
			if (!other.GetComponent<SphereCollider>())
			{
				return;
			}
			if (other.attachedRigidbody.gameObject.CompareTag("GorillaPlayer"))
			{
				ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent onPlayerTriggerExit = this.OnPlayerTriggerExit;
				if (onPlayerTriggerExit == null)
				{
					return;
				}
				onPlayerTriggerExit(other);
			}
		}

		// Token: 0x04007559 RID: 30041
		public LayerMask bodyLayer;

		// Token: 0x0200100F RID: 4111
		// (Invoke) Token: 0x06006679 RID: 26233
		public delegate void ObstacleCourseTriggerEvent(Collider collider);
	}
}
