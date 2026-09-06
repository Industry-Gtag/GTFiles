using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000FA2 RID: 4002
	public class PlayerTimerBoardLine : MonoBehaviour
	{
		// Token: 0x060063A6 RID: 25510 RVA: 0x00200A9F File Offset: 0x001FEC9F
		public void ResetData()
		{
			this.linePlayer = null;
			this.currentNickname = string.Empty;
			this.playerTimeStr = string.Empty;
			this.playerTimeSeconds = 0f;
		}

		// Token: 0x060063A7 RID: 25511 RVA: 0x00200ACC File Offset: 0x001FECCC
		public void SetLineData(NetPlayer netPlayer)
		{
			if (!netPlayer.InRoom || netPlayer == this.linePlayer)
			{
				return;
			}
			this.linePlayer = netPlayer;
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
			{
				this.rigContainer = rigContainer;
				this.playerVRRig = rigContainer.Rig;
			}
			this.InitializeLine();
		}

		// Token: 0x060063A8 RID: 25512 RVA: 0x00200B1A File Offset: 0x001FED1A
		public void InitializeLine()
		{
			this.currentNickname = string.Empty;
			this.UpdatePlayerText();
			this.UpdateTimeText();
		}

		// Token: 0x060063A9 RID: 25513 RVA: 0x00200B34 File Offset: 0x001FED34
		public void UpdateLine()
		{
			if (this.linePlayer != null)
			{
				if (this.playerNameVisible != this.playerVRRig.playerNameVisible)
				{
					this.UpdatePlayerText();
					this.parentBoard.IsDirty = true;
				}
				string text = this.playerTimeStr;
				this.UpdateTimeText();
				if (!this.playerTimeStr.Equals(text))
				{
					this.parentBoard.IsDirty = true;
				}
			}
		}

		// Token: 0x060063AA RID: 25514 RVA: 0x00200B9C File Offset: 0x001FED9C
		private void UpdatePlayerText()
		{
			try
			{
				if (this.rigContainer.IsNull() || this.playerVRRig.IsNull())
				{
					this.playerNameVisible = this.NormalizeName(this.linePlayer.NickName != this.currentNickname, this.linePlayer.NickName);
					this.currentNickname = this.linePlayer.NickName;
				}
				else if (this.rigContainer.Initialized)
				{
					this.playerNameVisible = this.playerVRRig.playerNameVisible;
				}
				else if (this.currentNickname.IsNullOrEmpty() || GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(this.linePlayer.UserId))
				{
					this.playerNameVisible = this.NormalizeName(this.linePlayer.NickName != this.currentNickname, this.linePlayer.NickName);
				}
			}
			catch (Exception)
			{
				this.playerNameVisible = this.linePlayer.DefaultName;
				MonkeAgent.instance.SendReport("NmError", this.linePlayer.UserId, this.linePlayer.NickName);
			}
		}

		// Token: 0x060063AB RID: 25515 RVA: 0x00200CD0 File Offset: 0x001FEED0
		private void UpdateTimeText()
		{
			if (this.linePlayer == null || !(PlayerTimerManager.instance != null))
			{
				this.playerTimeStr = "--:--:--";
				return;
			}
			this.playerTimeSeconds = PlayerTimerManager.instance.GetLastDurationForPlayer(this.linePlayer.ActorNumber);
			if (this.playerTimeSeconds > 0f)
			{
				this.playerTimeStr = TimeSpan.FromSeconds((double)this.playerTimeSeconds).ToString("mm\\:ss\\:ff");
				return;
			}
			this.playerTimeStr = "--:--:--";
		}

		// Token: 0x060063AC RID: 25516 RVA: 0x00200D54 File Offset: 0x001FEF54
		public string NormalizeName(bool doIt, string text)
		{
			if (doIt)
			{
				if (GorillaComputer.instance.CheckAutoBanListForName(text))
				{
					text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => Utils.IsASCIILetterOrDigit(c)));
					if (text.Length > 12)
					{
						text = text.Substring(0, 12);
					}
					text = text.ToUpper();
				}
				else
				{
					text = "BADGORILLA";
					MonkeAgent.instance.SendReport("evading the name ban", this.linePlayer.UserId, this.linePlayer.NickName);
				}
			}
			return text;
		}

		// Token: 0x060063AD RID: 25517 RVA: 0x00200DF8 File Offset: 0x001FEFF8
		public static int CompareByTotalTime(PlayerTimerBoardLine lineA, PlayerTimerBoardLine lineB)
		{
			if (lineA.playerTimeSeconds > 0f && lineB.playerTimeSeconds > 0f)
			{
				return lineA.playerTimeSeconds.CompareTo(lineB.playerTimeSeconds);
			}
			if (lineA.playerTimeSeconds <= 0f)
			{
				return 1;
			}
			if (lineB.playerTimeSeconds <= 0f)
			{
				return -1;
			}
			return 0;
		}

		// Token: 0x0400725A RID: 29274
		public string playerNameVisible;

		// Token: 0x0400725B RID: 29275
		public string playerTimeStr;

		// Token: 0x0400725C RID: 29276
		private float playerTimeSeconds;

		// Token: 0x0400725D RID: 29277
		public NetPlayer linePlayer;

		// Token: 0x0400725E RID: 29278
		public VRRig playerVRRig;

		// Token: 0x0400725F RID: 29279
		public PlayerTimerBoard parentBoard;

		// Token: 0x04007260 RID: 29280
		internal RigContainer rigContainer;

		// Token: 0x04007261 RID: 29281
		private string currentNickname;
	}
}
