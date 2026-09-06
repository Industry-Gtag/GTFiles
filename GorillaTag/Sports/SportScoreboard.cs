using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Sports
{
	// Token: 0x02001249 RID: 4681
	[RequireComponent(typeof(AudioSource))]
	[NetworkBehaviourWeaved(2)]
	public class SportScoreboard : NetworkComponent
	{
		// Token: 0x060076A1 RID: 30369 RVA: 0x00267568 File Offset: 0x00265768
		protected override void Awake()
		{
			base.Awake();
			SportScoreboard.Instance = this;
			this.audioSource = base.GetComponent<AudioSource>();
			this.scoreVisuals = new SportScoreboardVisuals[this.teamParameters.Count];
			for (int i = 0; i < this.teamParameters.Count; i++)
			{
				this.teamScores.Add(0);
				this.teamScoresPrev.Add(0);
			}
		}

		// Token: 0x060076A2 RID: 30370 RVA: 0x002675D1 File Offset: 0x002657D1
		public void RegisterTeamVisual(int TeamIndex, SportScoreboardVisuals visuals)
		{
			this.scoreVisuals[TeamIndex] = visuals;
			this.UpdateScoreboard();
		}

		// Token: 0x060076A3 RID: 30371 RVA: 0x002675E4 File Offset: 0x002657E4
		private void UpdateScoreboard()
		{
			for (int i = 0; i < this.teamParameters.Count; i++)
			{
				if (!(this.scoreVisuals[i] == null))
				{
					int num = this.teamScores[i];
					if (this.scoreVisuals[i].score1s != null)
					{
						this.scoreVisuals[i].score1s.SetUVOffset(num % 10);
					}
					if (this.scoreVisuals[i].score10s != null)
					{
						this.scoreVisuals[i].score10s.SetUVOffset(num / 10 % 10);
					}
				}
			}
		}

		// Token: 0x060076A4 RID: 30372 RVA: 0x00267680 File Offset: 0x00265880
		private void OnScoreUpdated()
		{
			for (int i = 0; i < this.teamScores.Count; i++)
			{
				if (this.teamScores[i] > this.teamScoresPrev[i] && this.teamParameters[i].goalScoredAudio != null && this.teamScores[i] < this.matchEndScore)
				{
					this.audioSource.GTPlayOneShot(this.teamParameters[i].goalScoredAudio, 1f);
				}
				this.teamScoresPrev[i] = this.teamScores[i];
			}
			if (!this.runningMatchEndCoroutine)
			{
				for (int j = 0; j < this.teamScores.Count; j++)
				{
					if (this.teamScores[j] >= this.matchEndScore)
					{
						base.StartCoroutine(this.MatchEndCoroutine(j));
						break;
					}
				}
			}
			this.UpdateScoreboard();
		}

		// Token: 0x060076A5 RID: 30373 RVA: 0x00267774 File Offset: 0x00265974
		public void TeamScored(int team)
		{
			if (base.IsMine && !this.runningMatchEndCoroutine)
			{
				if (team >= 0 && team < this.teamScores.Count)
				{
					this.teamScores[team] = this.teamScores[team] + 1;
				}
				this.OnScoreUpdated();
			}
		}

		// Token: 0x060076A6 RID: 30374 RVA: 0x002677C4 File Offset: 0x002659C4
		public void ResetScores()
		{
			if (base.IsMine && !this.runningMatchEndCoroutine)
			{
				for (int i = 0; i < this.teamScores.Count; i++)
				{
					this.teamScores[i] = 0;
				}
				this.OnScoreUpdated();
			}
		}

		// Token: 0x060076A7 RID: 30375 RVA: 0x0026780A File Offset: 0x00265A0A
		private IEnumerator MatchEndCoroutine(int winningTeam)
		{
			this.runningMatchEndCoroutine = true;
			if (winningTeam >= 0 && winningTeam < this.teamParameters.Count && this.teamParameters[winningTeam].matchWonAudio != null)
			{
				this.audioSource.GTPlayOneShot(this.teamParameters[winningTeam].matchWonAudio, 1f);
			}
			yield return new WaitForSeconds(this.matchEndScoreResetDelayTime);
			this.runningMatchEndCoroutine = false;
			this.ResetScores();
			yield break;
		}

		// Token: 0x17000B87 RID: 2951
		// (get) Token: 0x060076A8 RID: 30376 RVA: 0x00267820 File Offset: 0x00265A20
		[Networked]
		[Capacity(2)]
		[NetworkedWeaved(0, 2)]
		[NetworkedWeavedArray(2, 1, typeof(ElementReaderWriterInt32))]
		public unsafe NetworkArray<int> Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing SportScoreboard.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return new NetworkArray<int>((byte*)(this.Ptr + 0), 2, ElementReaderWriterInt32.GetInstance());
			}
		}

		// Token: 0x060076A9 RID: 30377 RVA: 0x0026785C File Offset: 0x00265A5C
		public override void WriteDataFusion()
		{
			this.Data.CopyFrom(this.teamScores, 0, this.teamScores.Count);
		}

		// Token: 0x060076AA RID: 30378 RVA: 0x0026788C File Offset: 0x00265A8C
		public override void ReadDataFusion()
		{
			this.teamScores.Clear();
			this.Data.CopyTo(this.teamScores);
			this.OnScoreUpdated();
		}

		// Token: 0x060076AB RID: 30379 RVA: 0x002678C0 File Offset: 0x00265AC0
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			for (int i = 0; i < this.teamScores.Count; i++)
			{
				stream.SendNext(this.teamScores[i]);
			}
		}

		// Token: 0x060076AC RID: 30380 RVA: 0x002678FC File Offset: 0x00265AFC
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			for (int i = 0; i < this.teamScores.Count; i++)
			{
				this.teamScores[i] = (int)stream.ReceiveNext();
			}
			this.OnScoreUpdated();
		}

		// Token: 0x060076AE RID: 30382 RVA: 0x00267977 File Offset: 0x00265B77
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			NetworkBehaviourUtils.InitializeNetworkArray<int>(this.Data, this._Data, "Data");
		}

		// Token: 0x060076AF RID: 30383 RVA: 0x00267999 File Offset: 0x00265B99
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			NetworkBehaviourUtils.CopyFromNetworkArray<int>(this.Data, ref this._Data);
		}

		// Token: 0x0400861A RID: 34330
		[OnEnterPlay_SetNull]
		public static SportScoreboard Instance;

		// Token: 0x0400861B RID: 34331
		[SerializeField]
		private List<SportScoreboard.TeamParameters> teamParameters = new List<SportScoreboard.TeamParameters>();

		// Token: 0x0400861C RID: 34332
		[SerializeField]
		private int matchEndScore = 3;

		// Token: 0x0400861D RID: 34333
		[SerializeField]
		private float matchEndScoreResetDelayTime = 3f;

		// Token: 0x0400861E RID: 34334
		private List<int> teamScores = new List<int>();

		// Token: 0x0400861F RID: 34335
		private List<int> teamScoresPrev = new List<int>();

		// Token: 0x04008620 RID: 34336
		private bool runningMatchEndCoroutine;

		// Token: 0x04008621 RID: 34337
		private AudioSource audioSource;

		// Token: 0x04008622 RID: 34338
		private SportScoreboardVisuals[] scoreVisuals;

		// Token: 0x04008623 RID: 34339
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 2)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private int[] _Data;

		// Token: 0x0200124A RID: 4682
		[Serializable]
		private class TeamParameters
		{
			// Token: 0x04008624 RID: 34340
			[SerializeField]
			public AudioClip matchWonAudio;

			// Token: 0x04008625 RID: 34341
			[SerializeField]
			public AudioClip goalScoredAudio;
		}
	}
}
