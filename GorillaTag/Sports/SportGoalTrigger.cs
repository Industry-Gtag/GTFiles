using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x02001248 RID: 4680
	public class SportGoalTrigger : MonoBehaviour
	{
		// Token: 0x0600769D RID: 30365 RVA: 0x0026743B File Offset: 0x0026563B
		public void BallExitedGoalTrigger(SportBall ball)
		{
			if (this.ballsPendingTriggerExit.Contains(ball))
			{
				this.ballsPendingTriggerExit.Remove(ball);
			}
		}

		// Token: 0x0600769E RID: 30366 RVA: 0x00267458 File Offset: 0x00265658
		private void PruneBallsPendingTriggerExitByDistance()
		{
			foreach (SportBall sportBall in this.ballsPendingTriggerExit)
			{
				if ((sportBall.transform.position - base.transform.position).sqrMagnitude > this.ballTriggerExitDistanceFallback * this.ballTriggerExitDistanceFallback)
				{
					this.ballsPendingTriggerExit.Remove(sportBall);
				}
			}
		}

		// Token: 0x0600769F RID: 30367 RVA: 0x002674E4 File Offset: 0x002656E4
		private void OnTriggerEnter(Collider other)
		{
			SportBall componentInParent = other.GetComponentInParent<SportBall>();
			if (componentInParent != null && this.scoreboard != null)
			{
				this.PruneBallsPendingTriggerExitByDistance();
				if (!this.ballsPendingTriggerExit.Contains(componentInParent))
				{
					this.scoreboard.TeamScored(this.teamScoringOnThisGoal);
					this.ballsPendingTriggerExit.Add(componentInParent);
				}
			}
		}

		// Token: 0x04008616 RID: 34326
		[SerializeField]
		private SportScoreboard scoreboard;

		// Token: 0x04008617 RID: 34327
		[SerializeField]
		private int teamScoringOnThisGoal = 1;

		// Token: 0x04008618 RID: 34328
		[SerializeField]
		private float ballTriggerExitDistanceFallback = 3f;

		// Token: 0x04008619 RID: 34329
		private HashSet<SportBall> ballsPendingTriggerExit = new HashSet<SportBall>();
	}
}
