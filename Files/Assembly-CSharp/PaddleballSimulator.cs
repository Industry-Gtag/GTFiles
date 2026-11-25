using GorillaExtensions;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PaddleballSimulator : MonoBehaviour, IArcadeSimulator<PaddleballSimState>, IArcadeSimulator
{
	[SerializeField]
	private GameObject[] paddleVisuals;

	[SerializeField]
	private GameObject ballVisual;

	[SerializeField]
	private GameObject titleVisual;

	[SerializeField]
	private TextMeshPro scoreVisual;

	[SerializeField]
	private int paddleSpeed = 17;

	[SerializeField]
	private int initialBallSpeed = 2;

	[SerializeField]
	private int ballSpeedBoost = 1;

	[SerializeField]
	private int ballDeflectDivisor = 16;

	[SerializeField]
	private int ballDeflectMax = 16;

	[SerializeField]
	private int MaxScore = 10;

	[SerializeField]
	private Vector2Int tableHalfSize;

	[SerializeField]
	private Vector2Int ballHalfSize;

	[SerializeField]
	private Vector2Int paddleHalfSize;

	private Vector2Int tableSizeBall;

	private Vector2Int tableSizePaddle;

	private Vector2Int ballPaddleCollisionSize;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioClip wallBounceSound;

	[SerializeField]
	private AudioClip paddleBounceSound;

	[SerializeField]
	private AudioClip scoreSound;

	[SerializeField]
	private AudioClip winGameSound;

	private RollbackGameEngine<PaddleballSimState> rollbackEngine;

	[SerializeField]
	private ArcadeMachineJoystick[] sticks;

	private int[] paddleX;

	[SerializeField]
	private float visualScale = 0.0004f;

	private int lastDisplayedScoreL = -1;

	private int lastDisplayedScoreR = -1;

	public int currentFrame => rollbackEngine.currentFrame;

	private void Start()
	{
		rollbackEngine = new RollbackGameEngine<PaddleballSimState>(this, sticks);
		paddleX = new int[4]
		{
			(int)(paddleVisuals[0].transform.localPosition.x / visualScale),
			(int)(paddleVisuals[1].transform.localPosition.x / visualScale),
			(int)(paddleVisuals[2].transform.localPosition.x / visualScale),
			(int)(paddleVisuals[3].transform.localPosition.x / visualScale)
		};
		tableSizeBall = tableHalfSize - ballHalfSize;
		tableSizePaddle = tableHalfSize - paddleHalfSize;
		ballPaddleCollisionSize = ballHalfSize + paddleHalfSize;
		StartGame();
	}

	private void Update()
	{
		if (!PhotonNetwork.InRoom)
		{
			ShowTitleOnly();
		}
		else
		{
			rollbackEngine.Update();
		}
	}

	private void ShowTitleOnly()
	{
		SetVisible(titleVisual, visible: true);
		SetVisible(ballVisual, visible: false);
		SetVisible((scoreVisual != null) ? scoreVisual.gameObject : null, visible: false);
		for (int i = 0; i < paddleVisuals.Length; i++)
		{
			SetVisible(paddleVisuals[i], visible: false);
		}
	}

	public void Display(PaddleballSimState gameState)
	{
		SetVisible(titleVisual, !gameState.GameActive);
		SetVisible(ballVisual, gameState.GameActive);
		SetVisible(scoreVisual.gameObject, visible: true);
		for (int i = 0; i < paddleVisuals.Length; i++)
		{
			SetVisible(paddleVisuals[i], gameState.GameActive && IsPaddleActive(in gameState, i));
		}
		ballVisual.transform.localPosition = new Vector3((float)gameState.BallPos.x * visualScale, (float)gameState.BallPos.y * visualScale, ballVisual.transform.localPosition.z);
		paddleVisuals[0].transform.localPosition = paddleVisuals[0].transform.localPosition.WithY((float)gameState.P0Y * visualScale);
		paddleVisuals[1].transform.localPosition = paddleVisuals[1].transform.localPosition.WithY((float)gameState.P1Y * visualScale);
		paddleVisuals[2].transform.localPosition = paddleVisuals[2].transform.localPosition.WithY((float)gameState.P2Y * visualScale);
		paddleVisuals[3].transform.localPosition = paddleVisuals[3].transform.localPosition.WithY((float)gameState.P3Y * visualScale);
		if (gameState.ScoreL != lastDisplayedScoreL || gameState.ScoreR != lastDisplayedScoreR)
		{
			scoreVisual.text = $"<color=#00FF00>{gameState.ScoreL}</color> <color=#C000FF>{gameState.ScoreR}</color>";
			lastDisplayedScoreL = gameState.ScoreL;
			lastDisplayedScoreR = gameState.ScoreR;
		}
	}

	private static void SetVisible(GameObject obj, bool visible)
	{
		if (obj != null && obj.activeSelf != visible)
		{
			obj.SetActive(visible);
		}
	}

	private static bool IsPaddleActive(in PaddleballSimState gameState, int player)
	{
		return (gameState.ActivePaddles & (1 << player)) != 0;
	}

	public void StartGame()
	{
		rollbackEngine.StartGame(new PaddleballSimState
		{
			GameActive = false,
			BallSpeed = initialBallSpeed
		});
	}

	private void StartMatch(ref PaddleballSimState gameState, int frame)
	{
		gameState.GameActive = true;
		gameState.ScoreL = 0;
		gameState.ScoreR = 0;
		gameState.BallPos = Vector2Int.zero;
		gameState.BallSpeed = initialBallSpeed;
		gameState.BallTrajectory = new Vector2Int(((frame & 1) == 0) ? 75 : (-75), frame * 31 % 51 - 25);
	}

	public void JoinGameInProgress(double anchorFrameTimestamp, int anchorFrame, object[] anchorFrameData)
	{
		PaddleballSimState anchorState = default(PaddleballSimState);
		if (anchorState.Deserialize(anchorFrameData))
		{
			rollbackEngine.JoinGameInProgress(anchorFrameTimestamp, anchorFrame, anchorState);
		}
	}

	public void FinalizeJoinGameData(out double anchorFrameTimestamp, out int anchorFrame, out object[] anchorFrameData)
	{
		rollbackEngine.FinalizeJoinGameData(out anchorFrameTimestamp, out anchorFrame, out var anchorState);
		anchorFrameData = anchorState.Serialize();
	}

	public void Tick(ref PaddleballSimState gameState, ArcadeButtons[] playerInputs, ArcadeButtons[] playerInputsLastFrame, int frame, bool withAudio = false)
	{
		bool flag = false;
		gameState.ActivePaddles = 0;
		for (int i = 0; i < playerInputs.Length; i++)
		{
			if (playerInputs[i].HasFlag(ArcadeButtons.GRAB))
			{
				gameState.ActivePaddles |= 1 << i;
				if (!playerInputsLastFrame[i].HasFlag(ArcadeButtons.GRAB))
				{
					flag = true;
				}
			}
		}
		if (!gameState.GameActive)
		{
			if (!flag)
			{
				return;
			}
			StartMatch(ref gameState, frame);
		}
		Vector2Int ballPos = gameState.BallPos;
		Vector2Int ballTrajectory = gameState.BallTrajectory;
		Vector2Int vector2Int = new Vector2Int(gameState.BallTrajectory.x * gameState.BallSpeed / 100, gameState.BallTrajectory.y * gameState.BallSpeed / 100);
		gameState.BallPos.x += vector2Int.x;
		gameState.BallPos.y += vector2Int.y;
		if (gameState.BallPos.y > tableSizeBall.y)
		{
			gameState.BallPos.y = tableSizeBall.y;
			gameState.BallTrajectory.y = -gameState.BallTrajectory.y;
			PlaySound(wallBounceSound, withAudio);
		}
		if (gameState.BallPos.y < -tableSizeBall.y)
		{
			gameState.BallPos.y = -tableSizeBall.y;
			gameState.BallTrajectory.y = -gameState.BallTrajectory.y;
			PlaySound(wallBounceSound, withAudio);
		}
		if (gameState.BallPos.x > tableSizeBall.x)
		{
			gameState.BallPos.x = tableSizeBall.x;
			gameState.BallTrajectory.x = -gameState.BallTrajectory.x;
			gameState.ScoreL++;
			if (gameState.ScoreL >= MaxScore)
			{
				gameState.GameActive = false;
				PlaySound(winGameSound, withAudio);
				return;
			}
			PlaySound(scoreSound, withAudio);
			gameState.BallSpeed = initialBallSpeed + ballSpeedBoost * Mathf.Min(gameState.ScoreL, gameState.ScoreR);
		}
		if (gameState.BallPos.x < -tableSizeBall.x)
		{
			gameState.BallPos.x = -tableSizeBall.x;
			gameState.BallTrajectory.x = -gameState.BallTrajectory.x;
			gameState.ScoreR++;
			if (gameState.ScoreR >= MaxScore)
			{
				gameState.GameActive = false;
				PlaySound(winGameSound, withAudio);
				return;
			}
			PlaySound(scoreSound, withAudio);
			gameState.BallSpeed = initialBallSpeed + ballSpeedBoost * Mathf.Min(gameState.ScoreL, gameState.ScoreR);
		}
		for (int j = 0; j < playerInputs.Length; j++)
		{
			ArcadeButtons arcadeButtons = playerInputs[j];
			int num = j switch
			{
				0 => gameState.P0Y, 
				1 => gameState.P1Y, 
				2 => gameState.P2Y, 
				3 => gameState.P3Y, 
				_ => 0, 
			};
			if (arcadeButtons.HasFlag(ArcadeButtons.UP))
			{
				num += paddleSpeed;
			}
			else if (arcadeButtons.HasFlag(ArcadeButtons.DOWN))
			{
				num -= paddleSpeed;
			}
			num = Mathf.Clamp(num, -tableSizePaddle.y, tableSizePaddle.y);
			switch (j)
			{
			case 0:
				gameState.P0Y = num;
				break;
			case 1:
				gameState.P1Y = num;
				break;
			case 2:
				gameState.P2Y = num;
				break;
			case 3:
				gameState.P3Y = num;
				break;
			}
			if (IsPaddleActive(in gameState, j) && Mathf.Abs(gameState.BallPos.x - paddleX[j]) < ballPaddleCollisionSize.x && Mathf.Abs(gameState.BallPos.y - num) < ballPaddleCollisionSize.y)
			{
				gameState.BallTrajectory.y = Mathf.Clamp((gameState.BallPos.y - num) / ballDeflectDivisor, -ballDeflectMax, ballDeflectMax);
				gameState.BallTrajectory.x = Mathf.RoundToInt(Mathf.Sqrt(10000 - gameState.BallTrajectory.y * gameState.BallTrajectory.y));
				gameState.BallTrajectory.x = 100 - (100 - gameState.BallTrajectory.x) / 2;
				if (j > 1)
				{
					gameState.BallTrajectory.x = -gameState.BallTrajectory.x;
				}
				if (ballTrajectory.x > 0 != gameState.BallTrajectory.x > 0)
				{
					PlaySound(paddleBounceSound, withAudio);
					gameState.BallSpeed += ballSpeedBoost;
				}
				else if (gameState.BallPos.x < paddleX[j] != ballPos.x < paddleX[j])
				{
					gameState.BallSpeed += ballSpeedBoost;
				}
			}
		}
	}

	private void PlaySound(AudioClip clip, bool withAudio)
	{
		if (withAudio)
		{
			audioSource.PlayOneShot(clip);
		}
	}
}
