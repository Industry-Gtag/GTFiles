using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ArcadeMachineJoystick : HandHold, ISnapTurnOverride, IRequestableOwnershipGuardCallbacks
{
	private struct InputHistoryEntry
	{
		public ArcadeButtons state;

		public int frame;
	}

	private class HistoryComparer : IComparer<InputHistoryEntry>
	{
		public int Compare(InputHistoryEntry a, InputHistoryEntry b)
		{
			return a.frame.CompareTo(b.frame);
		}
	}

	private XRNode xrNode;

	private RequestableOwnershipGuard guard;

	private GorillaSnapTurn snapTurn;

	private bool snapTurnOverride;

	private IResimulateRequest resimulate;

	private List<byte> scratchBytes = new List<byte>();

	private bool historyHasChanged;

	private int lastSentHeader;

	private byte[] lastSentArray;

	private List<InputHistoryEntry> inputHistory = new List<InputHistoryEntry>();

	private List<InputHistoryEntry> incomingInputHistory = new List<InputHistoryEntry>();

	private int historyDurationFrames = 180;

	private int lastTickFrame;

	private const int MaxHistoryPackedBytes = 4096;

	private HistoryComparer historyComparer = new HistoryComparer();

	public bool heldByLocalPlayer { get; private set; }

	public bool IsHeldRightHanded
	{
		get
		{
			if (heldByLocalPlayer)
			{
				return xrNode == XRNode.RightHand;
			}
			return false;
		}
	}

	public int player { get; private set; }

	public void Init(IResimulateRequest resimulate, int player)
	{
		this.resimulate = resimulate;
		this.player = player;
		guard = GetComponent<RequestableOwnershipGuard>();
		guard.AddCallbackTarget(this);
	}

	public void BindController(bool leftHand)
	{
		xrNode = (leftHand ? XRNode.LeftHand : XRNode.RightHand);
		heldByLocalPlayer = true;
		if (!leftHand)
		{
			if (!snapTurn)
			{
				snapTurn = GorillaTagger.Instance.GetComponent<GorillaSnapTurn>();
			}
			if (snapTurn != null)
			{
				snapTurnOverride = true;
				snapTurn.SetTurningOverride(this);
			}
		}
		if (PhotonNetwork.IsMasterClient)
		{
			guard.TransferOwnership(PhotonNetwork.LocalPlayer);
		}
		else if (!guard.isMine)
		{
			guard.RequestOwnership(OnOwnershipSuccess, OnOwnershipFail);
		}
	}

	private void OnOwnershipSuccess()
	{
	}

	private void OnOwnershipFail()
	{
		ForceRelease();
	}

	public void UnbindController()
	{
		heldByLocalPlayer = false;
		if (snapTurnOverride)
		{
			snapTurnOverride = false;
			snapTurn.UnsetTurningOverride(this);
		}
	}

	private ArcadeButtons OnInputUpdate()
	{
		ArcadeButtons arcadeButtons = (ArcadeButtons)0;
		if (heldByLocalPlayer)
		{
			arcadeButtons |= ArcadeButtons.GRAB;
			if (ControllerInputPoller.Primary2DAxis(xrNode).y > 0.5f)
			{
				arcadeButtons |= ArcadeButtons.UP;
			}
			if (ControllerInputPoller.Primary2DAxis(xrNode).y < -0.5f)
			{
				arcadeButtons |= ArcadeButtons.DOWN;
			}
			if (ControllerInputPoller.Primary2DAxis(xrNode).x < -0.5f)
			{
				arcadeButtons |= ArcadeButtons.LEFT;
			}
			if (ControllerInputPoller.Primary2DAxis(xrNode).x > 0.5f)
			{
				arcadeButtons |= ArcadeButtons.RIGHT;
			}
			if (ControllerInputPoller.PrimaryButtonPress(xrNode))
			{
				arcadeButtons |= ArcadeButtons.B0;
			}
			if (ControllerInputPoller.SecondaryButtonPress(xrNode))
			{
				arcadeButtons |= ArcadeButtons.B1;
			}
			if (ControllerInputPoller.TriggerFloat(xrNode) > 0.5f)
			{
				arcadeButtons |= ArcadeButtons.TRIGGER;
			}
		}
		return arcadeButtons;
	}

	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != info.photonView.Owner || stream.Count < 2 || !(stream.ReceiveNext() is int num))
		{
			return;
		}
		int num2 = num >> 8;
		byte b = (byte)(num & 0xFF);
		if (num2 < 0 || !(stream.ReceiveNext() is byte[] array) || array.Length > 4096)
		{
			return;
		}
		incomingInputHistory.Clear();
		incomingInputHistory.Add(new InputHistoryEntry
		{
			frame = num2,
			state = (ArcadeButtons)b
		});
		for (int i = 0; i < array.Length; i++)
		{
			byte num3 = array[i];
			int num4 = num3 >> 3;
			int num5 = num3 & 7;
			b ^= (byte)(1 << num5);
			if (num4 == 0)
			{
				continue;
			}
			if (num4 == 31)
			{
				if (i + 1 >= array.Length)
				{
					return;
				}
				i++;
				num4 = array[i];
				if (num4 == 255)
				{
					if (i + 2 >= array.Length)
					{
						return;
					}
					num4 = 255 * array[i + 1] + array[i + 2];
					i += 2;
				}
				if (num4 == 0)
				{
					return;
				}
			}
			num2 += num4;
			incomingInputHistory.Add(new InputHistoryEntry
			{
				frame = num2,
				state = (ArcadeButtons)b
			});
		}
		for (int j = 0; j < incomingInputHistory.Count; j++)
		{
			InputHistoryEntry inputHistoryEntry = incomingInputHistory[j];
			ArcadeButtons inputState = GetInputState(inputHistoryEntry.frame);
			if (inputHistoryEntry.state != inputState)
			{
				resimulate.RequestResimulate(inputHistoryEntry.frame);
				break;
			}
		}
		List<InputHistoryEntry> list = incomingInputHistory;
		List<InputHistoryEntry> list2 = inputHistory;
		inputHistory = list;
		incomingInputHistory = list2;
	}

	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		_ = historyHasChanged;
		if (lastSentArray != null && !historyHasChanged)
		{
			stream.SendNext(lastSentHeader);
			stream.SendNext(lastSentArray);
			return;
		}
		historyHasChanged = false;
		scratchBytes.Clear();
		int num;
		byte b;
		if (inputHistory.Count == 0)
		{
			num = 0;
			b = 0;
		}
		else
		{
			num = inputHistory[0].frame;
			b = (byte)inputHistory[0].state;
		}
		int num2 = (num << 8) | (b & 0xFF);
		stream.SendNext(num2);
		lastSentHeader = num2;
		for (int i = 1; i < inputHistory.Count; i++)
		{
			InputHistoryEntry inputHistoryEntry = inputHistory[i];
			int num3 = inputHistoryEntry.frame - num;
			byte b2 = (byte)((byte)inputHistoryEntry.state ^ b);
			byte b3 = 0;
			while (b2 != 0)
			{
				byte b4 = (byte)(b2 & (b2 - 1));
				switch (b2 - b4)
				{
				case 1:
					b3 = 0;
					break;
				case 2:
					b3 = 1;
					break;
				case 4:
					b3 = 2;
					break;
				case 8:
					b3 = 3;
					break;
				case 16:
					b3 = 4;
					break;
				case 32:
					b3 = 5;
					break;
				case 64:
					b3 = 6;
					break;
				case 128:
					b3 = 7;
					break;
				}
				if (b4 != 0)
				{
					scratchBytes.Add(b3);
				}
				b2 = b4;
			}
			byte b5 = 0;
			byte b6 = 0;
			if (num3 > 30)
			{
				if (num3 >= 255)
				{
					b6 = (byte)(num3 / 255);
					b5 = (byte)(num3 - b6 * 255);
				}
				else
				{
					b5 = (byte)num3;
				}
				num3 = 31;
			}
			scratchBytes.Add((byte)((num3 << 3) | b3));
			if (b6 != 0)
			{
				scratchBytes.Add(byte.MaxValue);
				scratchBytes.Add(b6);
				scratchBytes.Add(b5);
			}
			else if (b5 != 0)
			{
				scratchBytes.Add(b5);
			}
			num = inputHistoryEntry.frame;
			b = (byte)inputHistoryEntry.state;
		}
		byte[] array = new byte[scratchBytes.Count];
		scratchBytes.CopyTo(array);
		lastSentArray = array;
		stream.SendNext(array);
	}

	public void ReceiveRemoteState(ArcadeButtons newState)
	{
	}

	public bool TurnOverrideActive()
	{
		return snapTurnOverride;
	}

	public override bool CanBeGrabbed(GorillaGrabber grabber)
	{
		return true;
	}

	public void ForceRelease()
	{
		heldByLocalPlayer = false;
	}

	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		if (heldByLocalPlayer && (toPlayer == null || !toPlayer.IsLocal))
		{
			ForceRelease();
		}
	}

	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return !heldByLocalPlayer;
	}

	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return !heldByLocalPlayer;
	}

	public void OnMyOwnerLeft()
	{
	}

	public void OnMyCreatorLeft()
	{
	}

	public void ClearInputHistory()
	{
		inputHistory.Clear();
		historyHasChanged = true;
	}

	public void Tick(int frame)
	{
		lastTickFrame = frame;
		bool? flag = guard.photonView.Owner?.IsLocal;
		if (!flag.HasValue || flag != true || (inputHistory.Count > 0 && frame <= inputHistory[inputHistory.Count - 1].frame))
		{
			return;
		}
		ArcadeButtons arcadeButtons = OnInputUpdate();
		if (inputHistory.Count == 0 || arcadeButtons != inputHistory[inputHistory.Count - 1].state)
		{
			inputHistory.Add(new InputHistoryEntry
			{
				frame = frame,
				state = arcadeButtons
			});
			int num = frame - historyDurationFrames;
			int num2 = 0;
			while (inputHistory.Count > 1 && inputHistory[1].frame < num)
			{
				inputHistory.RemoveAt(0);
				num2++;
			}
			_ = 0;
			historyHasChanged = true;
		}
	}

	private string InputHistoryToString(List<InputHistoryEntry> inputHistory)
	{
		string text = "[";
		foreach (InputHistoryEntry item in inputHistory)
		{
			text += $"{item.frame}:{item.state}\n";
		}
		return text + "]";
	}

	public ArcadeButtons GetInputState(int frame)
	{
		if (inputHistory.Count == 0)
		{
			return (ArcadeButtons)0;
		}
		InputHistoryEntry item = new InputHistoryEntry
		{
			frame = frame
		};
		int num = inputHistory.BinarySearch(item, historyComparer);
		if (num < 0)
		{
			int num2 = ~num;
			return inputHistory[(num2 != 0) ? (num2 - 1) : 0].state;
		}
		return inputHistory[num].state;
	}

	public void GetInputStates(int frame, out ArcadeButtons thisFrame, out ArcadeButtons lastFrame)
	{
		if (inputHistory.Count == 0)
		{
			thisFrame = (ArcadeButtons)0;
			lastFrame = (ArcadeButtons)0;
			return;
		}
		InputHistoryEntry item = new InputHistoryEntry
		{
			frame = frame
		};
		int num = inputHistory.BinarySearch(item, historyComparer);
		int num3;
		if (num < 0)
		{
			int num2 = ~num;
			num3 = ((num2 != 0) ? (num2 - 1) : 0);
		}
		else
		{
			num3 = num;
		}
		thisFrame = inputHistory[num3].state;
		lastFrame = inputHistory[(num3 > 0) ? (num3 - 1) : 0].state;
	}
}
