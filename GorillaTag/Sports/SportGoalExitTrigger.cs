using System;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x02001247 RID: 4679
	public class SportGoalExitTrigger : MonoBehaviour
	{
		// Token: 0x0600769B RID: 30363 RVA: 0x00267404 File Offset: 0x00265604
		private void OnTriggerExit(Collider other)
		{
			SportBall componentInParent = other.GetComponentInParent<SportBall>();
			if (componentInParent != null && this.goalTrigger != null)
			{
				this.goalTrigger.BallExitedGoalTrigger(componentInParent);
			}
		}

		// Token: 0x04008615 RID: 34325
		[SerializeField]
		private SportGoalTrigger goalTrigger;
	}
}
