using System;
using GorillaExtensions;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace com.AnotherAxiom.Paddleball
{
	// Token: 0x02001171 RID: 4465
	public class Paddleball : ArcadeGame
	{
		// Token: 0x06006FF5 RID: 28661 RVA: 0x0024183A File Offset: 0x0023FA3A
		protected override void Awake()
		{
			base.Awake();
			this.yPosToByteFactor = 255f / (2f * this.tableSizeBall.y);
			this.byteToYPosFactor = 1f / this.yPosToByteFactor;
		}

		// Token: 0x06006FF6 RID: 28662 RVA: 0x00241874 File Offset: 0x0023FA74
		private void Start()
		{
			this.whiteWinScreen.SetActive(false);
			this.blackWinScreen.SetActive(false);
			this.titleScreen.SetActive(true);
			this.ball.gameObject.SetActive(false);
			this.currentScreenMode = Paddleball.ScreenMode.Title;
			this.paddleIdle = new float[this.p.Length];
			for (int i = 0; i < this.p.Length; i++)
			{
				this.p[i].gameObject.SetActive(false);
				this.paddleIdle[i] = 30f;
			}
			this.gameBallSpeed = this.initialBallSpeed;
			this.scoreR = (this.scoreL = 0);
			this.scoreFormat = this.scoreDisplay.text;
			this.UpdateScore();
		}

		// Token: 0x06006FF7 RID: 28663 RVA: 0x00241938 File Offset: 0x0023FB38
		private void Update()
		{
			if (this.currentScreenMode == Paddleball.ScreenMode.Gameplay)
			{
				this.ball.Translate(this.ballTrajectory.normalized * Time.deltaTime * this.gameBallSpeed);
				if (this.ball.localPosition.y > this.tableSizeBall.y)
				{
					this.ball.localPosition = new Vector3(this.ball.localPosition.x, this.tableSizeBall.y, this.ball.localPosition.z);
					this.ballTrajectory.y = -this.ballTrajectory.y;
					base.PlaySound(0, 3);
				}
				if (this.ball.localPosition.y < -this.tableSizeBall.y)
				{
					this.ball.localPosition = new Vector3(this.ball.localPosition.x, -this.tableSizeBall.y, this.ball.localPosition.z);
					this.ballTrajectory.y = -this.ballTrajectory.y;
					base.PlaySound(0, 3);
				}
				if (this.ball.localPosition.x > this.tableSizeBall.x)
				{
					this.ball.localPosition = new Vector3(this.tableSizeBall.x, this.ball.localPosition.y, this.ball.localPosition.z);
					this.ballTrajectory.x = -this.ballTrajectory.x;
					this.gameBallSpeed = this.initialBallSpeed;
					this.scoreL++;
					this.UpdateScore();
					base.PlaySound(2, 3);
					if (this.scoreL >= 10)
					{
						this.ChangeScreen(Paddleball.ScreenMode.WhiteWin);
					}
				}
				if (this.ball.localPosition.x < -this.tableSizeBall.x)
				{
					this.ball.localPosition = new Vector3(-this.tableSizeBall.x, this.ball.localPosition.y, this.ball.localPosition.z);
					this.ballTrajectory.x = -this.ballTrajectory.x;
					this.gameBallSpeed = this.initialBallSpeed;
					this.scoreR++;
					this.UpdateScore();
					base.PlaySound(2, 3);
					if (this.scoreR >= 10)
					{
						this.ChangeScreen(Paddleball.ScreenMode.BlackWin);
					}
				}
			}
			if (this.returnToTitleAfterTimestamp != 0f && Time.time > this.returnToTitleAfterTimestamp)
			{
				this.ChangeScreen(Paddleball.ScreenMode.Title);
			}
			for (int i = 0; i < this.p.Length; i++)
			{
				if (base.IsPlayerLocallyControlled(i))
				{
					float num = this.requestedPos[i];
					if (base.getButtonState(i, ArcadeButtons.UP))
					{
						this.requestedPos[i] += Time.deltaTime * this.paddleSpeed;
					}
					else if (base.getButtonState(i, ArcadeButtons.DOWN))
					{
						this.requestedPos[i] -= Time.deltaTime * this.paddleSpeed;
					}
					this.requestedPos[i] = Mathf.Clamp(this.requestedPos[i], -this.tableSizePaddle.y, this.tableSizePaddle.y);
				}
				float num2;
				if (!NetworkSystem.Instance.InRoom || NetworkSystem.Instance.IsMasterClient)
				{
					num2 = Mathf.MoveTowards(this.p[i].transform.localPosition.y, this.requestedPos[i], Time.deltaTime * this.paddleSpeed);
				}
				else
				{
					num2 = Mathf.MoveTowards(this.p[i].transform.localPosition.y, this.officialPos[i], Time.deltaTime * this.paddleSpeed);
				}
				this.p[i].transform.localPosition = this.p[i].transform.localPosition.WithY(Mathf.Clamp(num2, -this.tableSizePaddle.y, this.tableSizePaddle.y));
				if (base.getButtonState(i, ArcadeButtons.GRAB))
				{
					this.paddleIdle[i] = 0f;
					Paddleball.ScreenMode screenMode = this.currentScreenMode;
					if (screenMode != Paddleball.ScreenMode.Title)
					{
						if (screenMode == Paddleball.ScreenMode.Gameplay)
						{
							this.returnToTitleAfterTimestamp = Time.time + 30f;
						}
					}
					else
					{
						this.ChangeScreen(Paddleball.ScreenMode.Gameplay);
					}
				}
				else
				{
					this.paddleIdle[i] += Time.deltaTime;
				}
				bool flag = this.paddleIdle[i] < 30f;
				if (this.p[i].gameObject.activeSelf != flag)
				{
					if (flag)
					{
						base.PlaySound(4, 3);
						Vector3 localPosition = this.p[i].transform.localPosition;
						localPosition.y = 0f;
						this.requestedPos[i] = localPosition.y;
						this.p[i].transform.localPosition = localPosition;
					}
					this.p[i].gameObject.SetActive(this.paddleIdle[i] < 30f);
				}
				if (this.p[i].gameObject.activeInHierarchy && Mathf.Abs(this.ball.localPosition.x - this.p[i].transform.localPosition.x) < 0.1f && Mathf.Abs(this.ball.localPosition.y - this.p[i].transform.localPosition.y) < 0.5f)
				{
					this.ballTrajectory.y = (this.ball.localPosition.y - this.p[i].transform.localPosition.y) * 1.25f;
					float x = this.ballTrajectory.x;
					if (this.p[i].Right)
					{
						this.ballTrajectory.x = Mathf.Abs(this.ballTrajectory.y) - 1f;
					}
					else
					{
						this.ballTrajectory.x = 1f - Mathf.Abs(this.ballTrajectory.y);
					}
					if (x > 0f != this.ballTrajectory.x > 0f)
					{
						base.PlaySound(1, 3);
					}
					this.ballTrajectory.Normalize();
					this.gameBallSpeed += this.ballSpeedBoost;
				}
			}
		}

		// Token: 0x06006FF8 RID: 28664 RVA: 0x00241FA0 File Offset: 0x002401A0
		private void UpdateScore()
		{
			if (this.scoreFormat == null)
			{
				return;
			}
			this.scoreL = Mathf.Clamp(this.scoreL, 0, 10);
			this.scoreR = Mathf.Clamp(this.scoreR, 0, 10);
			this.scoreDisplay.text = string.Format(this.scoreFormat, this.scoreL, this.scoreR);
		}

		// Token: 0x06006FF9 RID: 28665 RVA: 0x0024200A File Offset: 0x0024020A
		private float ByteToYPos(byte Y)
		{
			return (float)Y / this.yPosToByteFactor - this.tableSizeBall.y;
		}

		// Token: 0x06006FFA RID: 28666 RVA: 0x00242021 File Offset: 0x00240221
		private byte YPosToByte(float Y)
		{
			return (byte)Mathf.RoundToInt((Y + this.tableSizeBall.y) * this.yPosToByteFactor);
		}

		// Token: 0x06006FFB RID: 28667 RVA: 0x00242040 File Offset: 0x00240240
		public override byte[] GetNetworkState()
		{
			this.netStateCur.P0LocY = this.YPosToByte(this.p[0].transform.localPosition.y);
			this.netStateCur.P1LocY = this.YPosToByte(this.p[1].transform.localPosition.y);
			this.netStateCur.P2LocY = this.YPosToByte(this.p[2].transform.localPosition.y);
			this.netStateCur.P3LocY = this.YPosToByte(this.p[3].transform.localPosition.y);
			this.netStateCur.BallLocX = this.ball.localPosition.x;
			this.netStateCur.BallLocY = this.YPosToByte(this.ball.localPosition.y);
			this.netStateCur.BallTrajectoryX = (byte)((this.ballTrajectory.x + 1f) * 127.5f);
			this.netStateCur.BallTrajectoryY = (byte)((this.ballTrajectory.y + 1f) * 127.5f);
			this.netStateCur.BallSpeed = this.gameBallSpeed;
			this.netStateCur.ScoreLeft = this.scoreL;
			this.netStateCur.ScoreRight = this.scoreR;
			this.netStateCur.ScreenMode = (int)this.currentScreenMode;
			if (!this.netStateCur.Equals(this.netStateLast))
			{
				this.netStateLast = this.netStateCur;
				base.SwapNetStateBuffersAndStreams();
				ArcadeGame.WrapNetState(this.netStateLast, this.netStateMemStream);
			}
			return this.netStateBuffer;
		}

		// Token: 0x06006FFC RID: 28668 RVA: 0x002421F4 File Offset: 0x002403F4
		public override void SetNetworkState(byte[] b)
		{
			Paddleball.PaddleballNetState paddleballNetState = (Paddleball.PaddleballNetState)ArcadeGame.UnwrapNetState(b);
			this.officialPos[0] = this.ByteToYPos(paddleballNetState.P0LocY);
			this.officialPos[1] = this.ByteToYPos(paddleballNetState.P1LocY);
			this.officialPos[2] = this.ByteToYPos(paddleballNetState.P2LocY);
			this.officialPos[3] = this.ByteToYPos(paddleballNetState.P3LocY);
			Vector2 vector = new Vector2(paddleballNetState.BallLocX, this.ByteToYPos(paddleballNetState.BallLocY));
			Vector2 normalized = new Vector2((float)paddleballNetState.BallTrajectoryX * 0.007843138f - 1f, (float)paddleballNetState.BallTrajectoryY * 0.007843138f - 1f).normalized;
			Vector2 vector2 = vector - normalized * Vector2.Dot(vector, normalized);
			Vector2 vector3 = this.ball.localPosition.xy();
			Vector2 vector4 = vector3 - this.ballTrajectory * Vector2.Dot(vector3, this.ballTrajectory);
			if ((vector2 - vector4).IsLongerThan(0.1f))
			{
				this.ball.localPosition = vector;
				this.ballTrajectory = normalized.xy();
			}
			this.gameBallSpeed = paddleballNetState.BallSpeed;
			this.ChangeScreen((Paddleball.ScreenMode)paddleballNetState.ScreenMode);
			if (this.scoreL != paddleballNetState.ScoreLeft || this.scoreR != paddleballNetState.ScoreRight)
			{
				this.scoreL = paddleballNetState.ScoreLeft;
				this.scoreR = paddleballNetState.ScoreRight;
				this.UpdateScore();
			}
		}

		// Token: 0x06006FFD RID: 28669 RVA: 0x00002C2D File Offset: 0x00000E2D
		protected override void ButtonUp(int player, ArcadeButtons button)
		{
		}

		// Token: 0x06006FFE RID: 28670 RVA: 0x00002C2D File Offset: 0x00000E2D
		protected override void ButtonDown(int player, ArcadeButtons button)
		{
		}

		// Token: 0x06006FFF RID: 28671 RVA: 0x00242370 File Offset: 0x00240570
		private void ChangeScreen(Paddleball.ScreenMode mode)
		{
			if (this.currentScreenMode == mode)
			{
				return;
			}
			switch (this.currentScreenMode)
			{
			case Paddleball.ScreenMode.Title:
				this.titleScreen.SetActive(false);
				break;
			case Paddleball.ScreenMode.Gameplay:
				this.ball.gameObject.SetActive(false);
				break;
			case Paddleball.ScreenMode.WhiteWin:
				this.whiteWinScreen.SetActive(false);
				break;
			case Paddleball.ScreenMode.BlackWin:
				this.blackWinScreen.SetActive(false);
				break;
			}
			this.currentScreenMode = mode;
			switch (mode)
			{
			case Paddleball.ScreenMode.Title:
				this.gameBallSpeed = this.initialBallSpeed;
				this.scoreL = 0;
				this.scoreR = 0;
				this.UpdateScore();
				this.returnToTitleAfterTimestamp = 0f;
				this.titleScreen.SetActive(true);
				return;
			case Paddleball.ScreenMode.Gameplay:
				this.ball.gameObject.SetActive(true);
				this.returnToTitleAfterTimestamp = Time.time + 30f;
				return;
			case Paddleball.ScreenMode.WhiteWin:
				this.whiteWinScreen.SetActive(true);
				this.returnToTitleAfterTimestamp = Time.time + this.winScreenDuration;
				base.PlaySound(3, 3);
				return;
			case Paddleball.ScreenMode.BlackWin:
				this.blackWinScreen.SetActive(true);
				this.returnToTitleAfterTimestamp = Time.time + this.winScreenDuration;
				base.PlaySound(3, 3);
				return;
			default:
				return;
			}
		}

		// Token: 0x06007000 RID: 28672 RVA: 0x002424A7 File Offset: 0x002406A7
		public override void OnTimeout()
		{
			this.ChangeScreen(Paddleball.ScreenMode.Title);
		}

		// Token: 0x06007001 RID: 28673 RVA: 0x002424B0 File Offset: 0x002406B0
		public override void ReadPlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
		{
			this.requestedPos[player] = this.ByteToYPos((byte)stream.ReceiveNext());
		}

		// Token: 0x06007002 RID: 28674 RVA: 0x002424CB File Offset: 0x002406CB
		public override void WritePlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
		{
			stream.SendNext(this.YPosToByte(this.requestedPos[player]));
		}

		// Token: 0x04007FDF RID: 32735
		[SerializeField]
		private PaddleballPaddle[] p;

		// Token: 0x04007FE0 RID: 32736
		private float[] requestedPos = new float[4];

		// Token: 0x04007FE1 RID: 32737
		private float[] officialPos = new float[4];

		// Token: 0x04007FE2 RID: 32738
		[SerializeField]
		private Transform ball;

		// Token: 0x04007FE3 RID: 32739
		[SerializeField]
		private Vector2 ballTrajectory;

		// Token: 0x04007FE4 RID: 32740
		[SerializeField]
		private float paddleSpeed = 1f;

		// Token: 0x04007FE5 RID: 32741
		[SerializeField]
		private float initialBallSpeed = 1f;

		// Token: 0x04007FE6 RID: 32742
		[SerializeField]
		private float ballSpeedBoost = 0.02f;

		// Token: 0x04007FE7 RID: 32743
		private float gameBallSpeed = 1f;

		// Token: 0x04007FE8 RID: 32744
		[SerializeField]
		private Vector2 tableSizeBall;

		// Token: 0x04007FE9 RID: 32745
		[SerializeField]
		private Vector2 tableSizePaddle;

		// Token: 0x04007FEA RID: 32746
		[SerializeField]
		private GameObject blackWinScreen;

		// Token: 0x04007FEB RID: 32747
		[SerializeField]
		private GameObject whiteWinScreen;

		// Token: 0x04007FEC RID: 32748
		[SerializeField]
		private GameObject titleScreen;

		// Token: 0x04007FED RID: 32749
		[SerializeField]
		private float winScreenDuration;

		// Token: 0x04007FEE RID: 32750
		private float returnToTitleAfterTimestamp;

		// Token: 0x04007FEF RID: 32751
		private int scoreL;

		// Token: 0x04007FF0 RID: 32752
		private int scoreR;

		// Token: 0x04007FF1 RID: 32753
		private string scoreFormat;

		// Token: 0x04007FF2 RID: 32754
		[SerializeField]
		private TMP_Text scoreDisplay;

		// Token: 0x04007FF3 RID: 32755
		private float[] paddleIdle;

		// Token: 0x04007FF4 RID: 32756
		private Paddleball.ScreenMode currentScreenMode;

		// Token: 0x04007FF5 RID: 32757
		private const int AUDIO_WALLBOUNCE = 0;

		// Token: 0x04007FF6 RID: 32758
		private const int AUDIO_PADDLEBOUNCE = 1;

		// Token: 0x04007FF7 RID: 32759
		private const int AUDIO_SCORE = 2;

		// Token: 0x04007FF8 RID: 32760
		private const int AUDIO_WIN = 3;

		// Token: 0x04007FF9 RID: 32761
		private const int AUDIO_PLAYERJOIN = 4;

		// Token: 0x04007FFA RID: 32762
		private const int VAR_REQUESTEDPOS = 0;

		// Token: 0x04007FFB RID: 32763
		private const int MAXSCORE = 10;

		// Token: 0x04007FFC RID: 32764
		private float yPosToByteFactor;

		// Token: 0x04007FFD RID: 32765
		private float byteToYPosFactor;

		// Token: 0x04007FFE RID: 32766
		private const float directionToByteFactor = 127.5f;

		// Token: 0x04007FFF RID: 32767
		private const float byteToDirectionFactor = 0.007843138f;

		// Token: 0x04008000 RID: 32768
		private Paddleball.PaddleballNetState netStateLast;

		// Token: 0x04008001 RID: 32769
		private Paddleball.PaddleballNetState netStateCur;

		// Token: 0x02001172 RID: 4466
		private enum ScreenMode
		{
			// Token: 0x04008003 RID: 32771
			Title,
			// Token: 0x04008004 RID: 32772
			Gameplay,
			// Token: 0x04008005 RID: 32773
			WhiteWin,
			// Token: 0x04008006 RID: 32774
			BlackWin
		}

		// Token: 0x02001173 RID: 4467
		[Serializable]
		private struct PaddleballNetState : IEquatable<Paddleball.PaddleballNetState>
		{
			// Token: 0x06007004 RID: 28676 RVA: 0x00242540 File Offset: 0x00240740
			public bool Equals(Paddleball.PaddleballNetState other)
			{
				return this.P0LocY == other.P0LocY && this.P1LocY == other.P1LocY && this.P2LocY == other.P2LocY && this.P3LocY == other.P3LocY && this.BallLocX.Approx(other.BallLocX, 1E-06f) && this.BallLocY == other.BallLocY && this.BallTrajectoryX == other.BallTrajectoryX && this.BallTrajectoryY == other.BallTrajectoryY && this.BallSpeed.Approx(other.BallSpeed, 1E-06f) && this.ScoreLeft == other.ScoreLeft && this.ScoreRight == other.ScoreRight && this.ScreenMode == other.ScreenMode;
			}

			// Token: 0x04008007 RID: 32775
			public byte P0LocY;

			// Token: 0x04008008 RID: 32776
			public byte P1LocY;

			// Token: 0x04008009 RID: 32777
			public byte P2LocY;

			// Token: 0x0400800A RID: 32778
			public byte P3LocY;

			// Token: 0x0400800B RID: 32779
			public float BallLocX;

			// Token: 0x0400800C RID: 32780
			public byte BallLocY;

			// Token: 0x0400800D RID: 32781
			public byte BallTrajectoryX;

			// Token: 0x0400800E RID: 32782
			public byte BallTrajectoryY;

			// Token: 0x0400800F RID: 32783
			public float BallSpeed;

			// Token: 0x04008010 RID: 32784
			public int ScoreLeft;

			// Token: 0x04008011 RID: 32785
			public int ScoreRight;

			// Token: 0x04008012 RID: 32786
			public int ScreenMode;
		}
	}
}
