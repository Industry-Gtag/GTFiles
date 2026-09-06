using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020010EE RID: 4334
	public class GhostReactorProgression : MonoBehaviour
	{
		// Token: 0x06006CBB RID: 27835 RVA: 0x00232678 File Offset: 0x00230878
		public void Awake()
		{
			GhostReactorProgression.instance = this;
		}

		// Token: 0x06006CBC RID: 27836 RVA: 0x00232680 File Offset: 0x00230880
		public void Start()
		{
			if (ProgressionManager.Instance != null)
			{
				ProgressionManager.Instance.OnTrackRead += this.OnTrackRead;
				ProgressionManager.Instance.OnTrackSet += this.OnTrackSet;
				ProgressionManager.Instance.OnNodeUnlocked += delegate(string a, string b)
				{
					this.OnNodeUnlocked();
				};
				return;
			}
			Debug.Log("GRP: ProgressionManager is null!");
		}

		// Token: 0x06006CBD RID: 27837 RVA: 0x002326E8 File Offset: 0x002308E8
		public async void GetStartingProgression(GRPlayer grPlayer)
		{
			await ProgressionUtil.WaitForMothershipSessionToken();
			this._grPlayer = grPlayer;
			ProgressionManager.Instance.GetProgression(this.progressionTrackId);
			if (this._grPlayer.gamePlayer.IsLocal())
			{
				this._grPlayer.mothershipId = MothershipClientContext.MothershipId;
				ProgressionManager.Instance.GetShiftCredit(this._grPlayer.mothershipId);
			}
		}

		// Token: 0x06006CBE RID: 27838 RVA: 0x00232727 File Offset: 0x00230927
		public void SetProgression(int progressionAmountToAdd, GRPlayer grPlayer)
		{
			this._grPlayer = grPlayer;
			ProgressionManager.Instance.SetProgression(this.progressionTrackId, progressionAmountToAdd);
		}

		// Token: 0x06006CBF RID: 27839 RVA: 0x00232741 File Offset: 0x00230941
		public void UnlockProgressionTreeNode(string treeId, string nodeId, GhostReactor reactor)
		{
			this._reactor = reactor;
			ProgressionManager.Instance.UnlockNode(treeId, nodeId);
		}

		// Token: 0x06006CC0 RID: 27840 RVA: 0x00232758 File Offset: 0x00230958
		private void OnTrackRead(string trackId, int progress)
		{
			if (this._grPlayer == null)
			{
				Debug.Log("GRP: OnTrackRead Failure: player is null");
				return;
			}
			if (trackId != this.progressionTrackId)
			{
				Debug.Log(string.Format("GRP: OnTrackRead Failure: track [{0}] progressionTrack [{1}] progress {2}", trackId, this.progressionTrackId, progress));
				return;
			}
			this._grPlayer.SetProgressionData(progress, progress, false);
		}

		// Token: 0x06006CC1 RID: 27841 RVA: 0x002327B7 File Offset: 0x002309B7
		private void OnTrackSet(string trackId, int progress)
		{
			if (this._grPlayer == null)
			{
				return;
			}
			if (trackId != this.progressionTrackId)
			{
				return;
			}
			this._grPlayer.SetProgressionData(progress, this._grPlayer.CurrentProgression.redeemedPoints, false);
		}

		// Token: 0x06006CC2 RID: 27842 RVA: 0x002327F4 File Offset: 0x002309F4
		private void OnNodeUnlocked()
		{
			if (this._reactor != null && this._reactor.toolProgression != null)
			{
				this._reactor.UpdateLocalPlayerFromProgression();
			}
		}

		// Token: 0x06006CC3 RID: 27843 RVA: 0x00232824 File Offset: 0x00230A24
		[return: TupleElementNames(new string[] { "tier", "grade", "totalPointsToNextLevel", "partialPointsToNextLevel" })]
		public static ValueTuple<int, int, int, int> GetGradePointDetails(int points)
		{
			GhostReactorProgression.LoadGRPSO();
			int num = 0;
			int num2 = 0;
			int i;
			for (i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				num2 = num;
				num += GhostReactorProgression.grPSO.progressionData[i].grades * GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
				if (points < num)
				{
					break;
				}
			}
			if (points > num)
			{
				return new ValueTuple<int, int, int, int>(i - 1, 0, 0, 0);
			}
			int pointsPerGrade = GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
			int num3 = (points - num2) / pointsPerGrade;
			int num4 = (points - num2) % pointsPerGrade;
			return new ValueTuple<int, int, int, int>(i, num3, pointsPerGrade, num4);
		}

		// Token: 0x06006CC4 RID: 27844 RVA: 0x002328CC File Offset: 0x00230ACC
		public static string GetTitleNameAndGrade(int points)
		{
			GhostReactorProgression.LoadGRPSO();
			int num = 0;
			for (int i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				num += GhostReactorProgression.grPSO.progressionData[i].grades * GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
				if (points < num)
				{
					return GhostReactorProgression.grPSO.progressionData[i].tierName + " " + (GhostReactorProgression.grPSO.progressionData[i].grades - Mathf.FloorToInt((float)((num - points) / GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade)) + 1).ToString();
				}
			}
			return "null";
		}

		// Token: 0x06006CC5 RID: 27845 RVA: 0x00232998 File Offset: 0x00230B98
		public static string GetTitleName(int points)
		{
			GhostReactorProgression.LoadGRPSO();
			int num = 0;
			for (int i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				num += GhostReactorProgression.grPSO.progressionData[i].grades * GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
				if (points < num)
				{
					return GhostReactorProgression.grPSO.progressionData[i].tierName;
				}
			}
			return "null";
		}

		// Token: 0x06006CC6 RID: 27846 RVA: 0x00232A14 File Offset: 0x00230C14
		public static string GetTitleNameFromLevel(int level)
		{
			GhostReactorProgression.LoadGRPSO();
			for (int i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				if (GhostReactorProgression.grPSO.progressionData[i].tierId >= level)
				{
					return GhostReactorProgression.grPSO.progressionData[i].tierName;
				}
			}
			return "null";
		}

		// Token: 0x06006CC7 RID: 27847 RVA: 0x00232A74 File Offset: 0x00230C74
		public static int GetGrade(int points)
		{
			GhostReactorProgression.LoadGRPSO();
			int num = 0;
			for (int i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				num += GhostReactorProgression.grPSO.progressionData[i].grades * GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
				if (points < num)
				{
					return GhostReactorProgression.grPSO.progressionData[i].grades - Mathf.FloorToInt((float)((num - points) / GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade)) + 1;
				}
			}
			return -1;
		}

		// Token: 0x06006CC8 RID: 27848 RVA: 0x00232B10 File Offset: 0x00230D10
		public static int GetTitleLevel(int points)
		{
			GhostReactorProgression.LoadGRPSO();
			int num = 0;
			for (int i = 0; i < GhostReactorProgression.grPSO.progressionData.Count; i++)
			{
				num += GhostReactorProgression.grPSO.progressionData[i].grades * GhostReactorProgression.grPSO.progressionData[i].pointsPerGrade;
				if (points < num)
				{
					return GhostReactorProgression.grPSO.progressionData[i].tierId;
				}
			}
			return -1;
		}

		// Token: 0x06006CC9 RID: 27849 RVA: 0x00232B87 File Offset: 0x00230D87
		public static void LoadGRPSO()
		{
			if (GhostReactorProgression.grPSO == null)
			{
				GhostReactorProgression.grPSO = Resources.Load<GRProgressionScriptableObject>("ProgressionTiersData");
			}
		}

		// Token: 0x04007D13 RID: 32019
		public static GhostReactorProgression instance;

		// Token: 0x04007D14 RID: 32020
		private string progressionTrackId = "a0208736-e696-489b-81cd-c0c772489cc5";

		// Token: 0x04007D15 RID: 32021
		private GRPlayer _grPlayer;

		// Token: 0x04007D16 RID: 32022
		private GhostReactor _reactor;

		// Token: 0x04007D17 RID: 32023
		public static GRProgressionScriptableObject grPSO;

		// Token: 0x04007D18 RID: 32024
		public const string grPSODirectory = "ProgressionTiersData";
	}
}
