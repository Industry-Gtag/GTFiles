using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02001014 RID: 4116
	public class WinnerScoreboard : MonoBehaviour
	{
		// Token: 0x0600668C RID: 26252 RVA: 0x0020F098 File Offset: 0x0020D298
		public void UpdateBoard(string winner, ObstacleCourse.RaceState _currentState)
		{
			if (this.output == null)
			{
				return;
			}
			switch (_currentState)
			{
			case ObstacleCourse.RaceState.Started:
				Debug.Log(this.raceStarted);
				this.output.text = this.raceStarted;
				return;
			case ObstacleCourse.RaceState.Waiting:
				Debug.Log(this.raceLoading);
				this.output.text = this.raceLoading;
				return;
			case ObstacleCourse.RaceState.Finished:
				Debug.Log(winner + " WON!!");
				this.output.text = winner + " WON!!";
				return;
			default:
				return;
			}
		}

		// Token: 0x04007560 RID: 30048
		public string raceStarted = "RACE STARTED!";

		// Token: 0x04007561 RID: 30049
		public string raceLoading = "RACE LOADING...";

		// Token: 0x04007562 RID: 30050
		[SerializeField]
		private TextMeshPro output;
	}
}
