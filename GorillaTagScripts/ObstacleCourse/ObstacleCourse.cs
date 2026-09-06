using System;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x0200100A RID: 4106
	public class ObstacleCourse : MonoBehaviour
	{
		// Token: 0x170009CB RID: 2507
		// (get) Token: 0x0600664A RID: 26186 RVA: 0x0020E63E File Offset: 0x0020C83E
		// (set) Token: 0x0600664B RID: 26187 RVA: 0x0020E646 File Offset: 0x0020C846
		public int winnerActorNumber { get; private set; }

		// Token: 0x0600664C RID: 26188 RVA: 0x0020E650 File Offset: 0x0020C850
		private void Awake()
		{
			this.numPlayersOnCourse = 0;
			for (int i = 0; i < this.zoneTriggers.Length; i++)
			{
				ObstacleCourseZoneTrigger obstacleCourseZoneTrigger = this.zoneTriggers[i];
				if (!(obstacleCourseZoneTrigger == null))
				{
					obstacleCourseZoneTrigger.OnPlayerTriggerEnter += this.OnPlayerEnterZone;
					obstacleCourseZoneTrigger.OnPlayerTriggerExit += this.OnPlayerExitZone;
				}
			}
			this.TappableBell.OnTapped += this.OnEndLineTrigger;
		}

		// Token: 0x0600664D RID: 26189 RVA: 0x0020E6C4 File Offset: 0x0020C8C4
		private void OnDestroy()
		{
			for (int i = 0; i < this.zoneTriggers.Length; i++)
			{
				ObstacleCourseZoneTrigger obstacleCourseZoneTrigger = this.zoneTriggers[i];
				if (!(obstacleCourseZoneTrigger == null))
				{
					obstacleCourseZoneTrigger.OnPlayerTriggerEnter -= this.OnPlayerEnterZone;
					obstacleCourseZoneTrigger.OnPlayerTriggerExit -= this.OnPlayerExitZone;
				}
			}
			this.TappableBell.OnTapped -= this.OnEndLineTrigger;
		}

		// Token: 0x0600664E RID: 26190 RVA: 0x0020E731 File Offset: 0x0020C931
		private void Start()
		{
			this.RestartTimer(false);
		}

		// Token: 0x0600664F RID: 26191 RVA: 0x0020E73A File Offset: 0x0020C93A
		public void InvokeUpdate()
		{
			if (NetworkSystem.Instance.InRoom && ObstacleCourseManager.Instance.IsMine && this.currentState == ObstacleCourse.RaceState.Finished && Time.time - this.startTime >= this.cooldownTime)
			{
				this.RestartTimer(true);
			}
		}

		// Token: 0x06006650 RID: 26192 RVA: 0x0020E778 File Offset: 0x0020C978
		public void OnPlayerEnterZone(Collider other)
		{
			if (ObstacleCourseManager.Instance.IsMine)
			{
				this.numPlayersOnCourse++;
			}
		}

		// Token: 0x06006651 RID: 26193 RVA: 0x0020E794 File Offset: 0x0020C994
		public void OnPlayerExitZone(Collider other)
		{
			if (ObstacleCourseManager.Instance.IsMine)
			{
				this.numPlayersOnCourse--;
			}
		}

		// Token: 0x06006652 RID: 26194 RVA: 0x0020E7B0 File Offset: 0x0020C9B0
		private void RestartTimer(bool playFx = true)
		{
			this.UpdateState(ObstacleCourse.RaceState.Started, playFx);
		}

		// Token: 0x06006653 RID: 26195 RVA: 0x0020E7BA File Offset: 0x0020C9BA
		private void EndRace()
		{
			this.UpdateState(ObstacleCourse.RaceState.Finished, true);
			this.startTime = Time.time;
		}

		// Token: 0x06006654 RID: 26196 RVA: 0x0020E7D0 File Offset: 0x0020C9D0
		public void PlayWinningEffects()
		{
			if (this.confettiParticle)
			{
				this.confettiParticle.Play();
			}
			if (this.bannerRenderer)
			{
				UberShaderProperty baseColor = UberShader.BaseColor;
				Material material = this.bannerRenderer.material;
				RigContainer rigContainer = this.winnerRig;
				baseColor.SetValue<Color?>(material, (rigContainer != null) ? new Color?(rigContainer.Rig.playerColor) : null);
			}
			this.audioSource.GTPlay();
		}

		// Token: 0x06006655 RID: 26197 RVA: 0x0020E846 File Offset: 0x0020CA46
		public void OnEndLineTrigger(VRRig rig)
		{
			if (ObstacleCourseManager.Instance.IsMine && this.currentState == ObstacleCourse.RaceState.Started)
			{
				this.winnerActorNumber = rig.creator.ActorNumber;
				this.winnerRig = rig.rigContainer;
				this.EndRace();
			}
		}

		// Token: 0x06006656 RID: 26198 RVA: 0x0020E87F File Offset: 0x0020CA7F
		public void Deserialize(int _winnerActorNumber, ObstacleCourse.RaceState _currentState)
		{
			if (!ObstacleCourseManager.Instance.IsMine)
			{
				this.winnerActorNumber = _winnerActorNumber;
				VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(this.winnerActorNumber), out this.winnerRig);
				this.UpdateState(_currentState, true);
			}
		}

		// Token: 0x06006657 RID: 26199 RVA: 0x0020E8C0 File Offset: 0x0020CAC0
		private void UpdateState(ObstacleCourse.RaceState state, bool playFX = true)
		{
			this.currentState = state;
			WinnerScoreboard winnerScoreboard = this.scoreboard;
			RigContainer rigContainer = this.winnerRig;
			winnerScoreboard.UpdateBoard((rigContainer != null) ? rigContainer.Rig.playerNameVisible : null, this.currentState);
			if (this.currentState == ObstacleCourse.RaceState.Finished)
			{
				this.PlayWinningEffects();
			}
			else if (this.currentState == ObstacleCourse.RaceState.Started && this.bannerRenderer)
			{
				UberShader.BaseColor.SetValue<Color>(this.bannerRenderer.material, Color.white);
			}
			this.UpdateStartingGate();
		}

		// Token: 0x06006658 RID: 26200 RVA: 0x0020E944 File Offset: 0x0020CB44
		private void UpdateStartingGate()
		{
			if (this.currentState == ObstacleCourse.RaceState.Finished)
			{
				this.leftGate.transform.RotateAround(this.leftGate.transform.position, Vector3.up, 90f);
				this.rightGate.transform.RotateAround(this.rightGate.transform.position, Vector3.up, -90f);
				return;
			}
			if (this.currentState == ObstacleCourse.RaceState.Started)
			{
				this.leftGate.transform.RotateAround(this.leftGate.transform.position, Vector3.up, -90f);
				this.rightGate.transform.RotateAround(this.rightGate.transform.position, Vector3.up, 90f);
			}
		}

		// Token: 0x04007540 RID: 30016
		public WinnerScoreboard scoreboard;

		// Token: 0x04007542 RID: 30018
		private RigContainer winnerRig;

		// Token: 0x04007543 RID: 30019
		public ObstacleCourseZoneTrigger[] zoneTriggers;

		// Token: 0x04007544 RID: 30020
		[HideInInspector]
		public ObstacleCourse.RaceState currentState;

		// Token: 0x04007545 RID: 30021
		[SerializeField]
		private ParticleSystem confettiParticle;

		// Token: 0x04007546 RID: 30022
		[SerializeField]
		private Renderer bannerRenderer;

		// Token: 0x04007547 RID: 30023
		[SerializeField]
		private TappableBell TappableBell;

		// Token: 0x04007548 RID: 30024
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04007549 RID: 30025
		[SerializeField]
		private float cooldownTime = 20f;

		// Token: 0x0400754A RID: 30026
		public GameObject leftGate;

		// Token: 0x0400754B RID: 30027
		public GameObject rightGate;

		// Token: 0x0400754C RID: 30028
		private int numPlayersOnCourse;

		// Token: 0x0400754D RID: 30029
		private float startTime;

		// Token: 0x0200100B RID: 4107
		public enum RaceState
		{
			// Token: 0x0400754F RID: 30031
			Started,
			// Token: 0x04007550 RID: 30032
			Waiting,
			// Token: 0x04007551 RID: 30033
			Finished
		}
	}
}
