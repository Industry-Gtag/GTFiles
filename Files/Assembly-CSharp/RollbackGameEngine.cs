using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

internal class RollbackGameEngine<T> : IResimulateRequest
{
	private struct GameStateAtFrame
	{
		public T gameState;

		public int frame;
	}

	private IArcadeSimulator<T> simulator;

	private int resimulateFromFrame;

	private int nextAnchorStateFrame;

	private int anchorStateFrameInterval;

	private int anchorStateMaxCount;

	private List<GameStateAtFrame> anchorStates;

	private GameStateAtFrame currentState;

	private int settleFrames;

	private int finalizedFrame = -1;

	private double nextFrameTimestamp;

	public float frameDuration;

	private ArcadeMachineJoystick[] sticks;

	private ArcadeButtons[] buttonsArray;

	private ArcadeButtons[] buttonsArrayLastFrame;

	public int currentFrame => currentState.frame;

	public RollbackGameEngine(IArcadeSimulator<T> simulator, ArcadeMachineJoystick[] sticks, int frameRate = 30, float timeBetweenAnchorStates = 0.5f, float maxRollbackDuration = 6f, float settleDuration = 1.5f)
	{
		this.simulator = simulator;
		this.sticks = sticks;
		buttonsArray = new ArcadeButtons[sticks.Length];
		buttonsArrayLastFrame = new ArcadeButtons[sticks.Length];
		anchorStateFrameInterval = Mathf.FloorToInt(timeBetweenAnchorStates * (float)frameRate);
		frameDuration = 1f / (float)frameRate;
		anchorStateMaxCount = Mathf.CeilToInt(maxRollbackDuration / timeBetweenAnchorStates);
		anchorStates = new List<GameStateAtFrame>(anchorStateMaxCount);
		settleFrames = Mathf.CeilToInt(settleDuration * (float)frameRate);
		for (int i = 0; i < sticks.Length; i++)
		{
			sticks[i].Init(this, i);
		}
	}

	public void FinalizeJoinGameData(out double anchorFrameTimestamp, out int anchorFrame, out T anchorState)
	{
		int num = currentState.frame - settleFrames;
		int count = 0;
		for (int num2 = anchorStates.Count - 1; num2 > 0; num2--)
		{
			if (anchorStates[num2].frame <= num)
			{
				count = num2;
				break;
			}
		}
		anchorStates.RemoveRange(0, count);
		finalizedFrame = anchorStates[0].frame;
		anchorFrame = anchorStates[0].frame;
		anchorState = anchorStates[0].gameState;
		anchorFrameTimestamp = nextFrameTimestamp - (double)(currentState.frame + 1 - anchorFrame) * (double)frameDuration;
	}

	public void JoinGameInProgress(double anchorFrameTimestamp, int anchorFrame, T anchorState)
	{
		anchorStates.Clear();
		currentState = new GameStateAtFrame
		{
			frame = anchorFrame,
			gameState = anchorState
		};
		anchorStates.Add(currentState);
		nextAnchorStateFrame = anchorFrame + anchorStateFrameInterval;
		finalizedFrame = anchorFrame;
		int num = anchorFrame + (int)Math.Floor((PhotonNetwork.Time - anchorFrameTimestamp) / (double)frameDuration);
		nextFrameTimestamp = anchorFrameTimestamp + (double)(num + 1 - anchorFrame) * (double)frameDuration;
		while (currentState.frame < num)
		{
			NextFrame(withAudio: false);
		}
	}

	public void StartGame(T startState)
	{
		currentState = new GameStateAtFrame
		{
			frame = 0,
			gameState = startState
		};
		ArcadeMachineJoystick[] array = sticks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ClearInputHistory();
		}
		anchorStates.Clear();
		anchorStates.Add(currentState);
		nextAnchorStateFrame = anchorStateFrameInterval;
		finalizedFrame = -1;
		if (PhotonNetwork.Time == 0.0)
		{
			nextFrameTimestamp = 0.0;
		}
		else
		{
			nextFrameTimestamp = PhotonNetwork.Time + (double)frameDuration;
		}
		resimulateFromFrame = -1;
	}

	public void RequestResimulate(int fromFrame)
	{
		resimulateFromFrame = ((resimulateFromFrame < 0) ? fromFrame : Math.Min(resimulateFromFrame, fromFrame));
	}

	public void Update()
	{
		if (nextFrameTimestamp == 0.0)
		{
			if (!PhotonNetwork.InRoom || PhotonNetwork.Time == 0.0)
			{
				return;
			}
			nextFrameTimestamp = PhotonNetwork.Time + (double)frameDuration;
		}
		double time = PhotonNetwork.Time;
		while (time >= nextFrameTimestamp)
		{
			if (resimulateFromFrame >= 0)
			{
				HandleRollback(resimulateFromFrame, currentState.frame);
				resimulateFromFrame = -1;
			}
			Tick();
			nextFrameTimestamp += frameDuration;
			if (Math.Abs(nextFrameTimestamp - time) > 10.0)
			{
				break;
			}
		}
	}

	public void Rewind()
	{
		HandleRollback(currentState.frame - 1, currentState.frame - 1);
		simulator.Display(currentState.gameState);
	}

	public void Tick()
	{
		int frame = currentState.frame + 1;
		ArcadeMachineJoystick[] array = sticks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Tick(frame);
		}
		NextFrame();
		simulator.Display(currentState.gameState);
	}

	private void NextFrame(bool withAudio = true)
	{
		int frame = currentState.frame + 1;
		PopulateInputs(buttonsArray, buttonsArrayLastFrame, frame);
		simulator.Tick(ref currentState.gameState, buttonsArray, buttonsArrayLastFrame, frame, withAudio);
		currentState.frame = frame;
		if (currentState.frame == nextAnchorStateFrame)
		{
			if (anchorStates.Count == anchorStateMaxCount)
			{
				anchorStates.RemoveAt(0);
			}
			anchorStates.Add(currentState);
			nextAnchorStateFrame += anchorStateFrameInterval;
		}
	}

	private void HandleRollback(int fromFrame, int toFrame)
	{
		fromFrame = Math.Max(fromFrame, finalizedFrame + 1);
		GameStateAtFrame gameStateAtFrame = currentState;
		int num = anchorStates.Count - 1;
		for (int i = 0; i < anchorStates.Count; i++)
		{
			GameStateAtFrame gameStateAtFrame2 = anchorStates[i];
			if (gameStateAtFrame2.frame == fromFrame - 1)
			{
				gameStateAtFrame = gameStateAtFrame2;
				num = i;
				break;
			}
			if (gameStateAtFrame2.frame >= fromFrame)
			{
				if (i == 0)
				{
					gameStateAtFrame = gameStateAtFrame2;
					num = i;
				}
				else
				{
					gameStateAtFrame = anchorStates[i - 1];
					num = i - 1;
				}
				break;
			}
			gameStateAtFrame = gameStateAtFrame2;
			num = i;
		}
		anchorStates.RemoveRange(num + 1, anchorStates.Count - (num + 1));
		nextAnchorStateFrame = anchorStates[num].frame + anchorStateFrameInterval;
		currentState = gameStateAtFrame;
		while (currentState.frame < toFrame)
		{
			NextFrame(withAudio: false);
		}
	}

	private void PopulateInputs(ArcadeButtons[] buttonsArray, int frame)
	{
		for (int i = 0; i < buttonsArray.Length; i++)
		{
			buttonsArray[i] = sticks[i].GetInputState(frame);
		}
	}

	private void PopulateInputs(ArcadeButtons[] buttonsArray, ArcadeButtons[] buttonsArrayLastFrame, int frame)
	{
		for (int i = 0; i < buttonsArray.Length; i++)
		{
			sticks[i].GetInputStates(frame, out buttonsArray[i], out buttonsArrayLastFrame[i]);
		}
	}
}
