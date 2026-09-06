using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000602 RID: 1538
public class GameBallPlayerLocal : MonoBehaviour
{
	// Token: 0x06002658 RID: 9816 RVA: 0x000CB000 File Offset: 0x000C9200
	private void Awake()
	{
		GameBallPlayerLocal.instance = this;
		this.hands = new GameBallPlayerLocal.HandData[2];
		this.inputData = new GameBallPlayerLocal.InputData[2];
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.inputData[i] = new GameBallPlayerLocal.InputData(32);
		}
		Application.quitting += GameBallPlayerLocal._OnApplicationQuit;
	}

	// Token: 0x06002659 RID: 9817 RVA: 0x000CB060 File Offset: 0x000C9260
	private static void _OnApplicationQuit()
	{
		if (MonkeBallGame.Instance != null)
		{
			MonkeBallGame.Instance.OnPlayerDestroy();
		}
	}

	// Token: 0x0600265A RID: 9818 RVA: 0x000CB079 File Offset: 0x000C9279
	private void OnApplicationPause(bool pause)
	{
		if (pause && MonkeBallGame.Instance != null)
		{
			MonkeBallGame.Instance.OnPlayerDestroy();
		}
	}

	// Token: 0x0600265B RID: 9819 RVA: 0x000CB095 File Offset: 0x000C9295
	private void OnDestroy()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (MonkeBallGame.Instance != null)
		{
			MonkeBallGame.Instance.OnPlayerDestroy();
		}
	}

	// Token: 0x0600265C RID: 9820 RVA: 0x000CB0B8 File Offset: 0x000C92B8
	public void OnUpdateInteract()
	{
		if (!ZoneManagement.IsInZone(GTZone.arena))
		{
			return;
		}
		for (int i = 0; i < this.inputData.Length; i++)
		{
			this.UpdateInput(i);
		}
		for (int j = 0; j < this.hands.Length; j++)
		{
			this.UpdateHand(j);
		}
	}

	// Token: 0x0600265D RID: 9821 RVA: 0x000CB104 File Offset: 0x000C9304
	private void UpdateInput(int handIndex)
	{
		XRNode xrnode = this.GetXRNode(handIndex);
		GameBallPlayerLocal.InputDataMotion inputDataMotion = default(GameBallPlayerLocal.InputDataMotion);
		InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(xrnode);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.devicePosition, out inputDataMotion.position);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceRotation, out inputDataMotion.rotation);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceVelocity, out inputDataMotion.velocity);
		deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out inputDataMotion.angVelocity);
		inputDataMotion.time = Time.timeAsDouble;
		this.inputData[handIndex].AddInput(inputDataMotion);
	}

	// Token: 0x0600265E RID: 9822 RVA: 0x000CB190 File Offset: 0x000C9390
	private void UpdateHand(int handIndex)
	{
		if (GameBallManager.Instance == null)
		{
			return;
		}
		if (!this.gamePlayer.GetGameBallId(handIndex).IsValid())
		{
			this.UpdateHandEmpty(handIndex);
			return;
		}
		this.UpdateHandHolding(handIndex);
	}

	// Token: 0x0600265F RID: 9823 RVA: 0x000CB1D4 File Offset: 0x000C93D4
	public void SetGrabbed(GameBallId gameBallId, int handIndex)
	{
		GameBallPlayerLocal.HandData handData = this.hands[handIndex];
		handData.gripPressedTime = 0.0;
		this.hands[handIndex] = handData;
		this.UpdateStuckState();
	}

	// Token: 0x06002660 RID: 9824 RVA: 0x000CB211 File Offset: 0x000C9411
	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameBallId.Invalid, handIndex);
	}

	// Token: 0x06002661 RID: 9825 RVA: 0x000CB220 File Offset: 0x000C9420
	public void ClearAllGrabbed()
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			this.ClearGrabbed(i);
		}
	}

	// Token: 0x06002662 RID: 9826 RVA: 0x000CB248 File Offset: 0x000C9448
	private void UpdateStuckState()
	{
		bool flag = false;
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.gamePlayer.GetGameBallId(i).IsValid())
			{
				flag = true;
				break;
			}
		}
		GTPlayer.Instance.disableMovement = flag;
	}

	// Token: 0x06002663 RID: 9827 RVA: 0x000CB290 File Offset: 0x000C9490
	private void UpdateHandEmpty(int handIndex)
	{
		GameBallPlayerLocal.HandData handData = this.hands[handIndex];
		bool flag = ControllerInputPoller.GripFloat(this.GetXRNode(handIndex)) > 0.7f;
		double timeAsDouble = Time.timeAsDouble;
		if (flag && !handData.gripWasHeld)
		{
			handData.gripPressedTime = timeAsDouble;
		}
		double num = timeAsDouble - handData.gripPressedTime;
		handData.gripWasHeld = flag;
		this.hands[handIndex] = handData;
		if (flag && num < 0.15000000596046448)
		{
			Vector3 position = this.GetHandTransform(handIndex).position;
			GameBallId gameBallId = GameBallManager.Instance.TryGrabLocal(position, this.gamePlayer.teamId);
			float num2 = 0.15f;
			if (gameBallId.IsValid())
			{
				bool flag2 = GameBallPlayerLocal.IsLeftHand(handIndex);
				BodyDockPositions myBodyDockPositions = GorillaTagger.Instance.offlineVRRig.myBodyDockPositions;
				object obj = (flag2 ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform);
				GameBall gameBall = GameBallManager.Instance.GetGameBall(gameBallId);
				Vector3 vector = gameBall.transform.position;
				Vector3 vector2 = gameBall.transform.position - position;
				if (vector2.sqrMagnitude > num2 * num2)
				{
					vector = position + vector2.normalized * num2;
				}
				object obj2 = obj;
				Vector3 vector3 = obj2.InverseTransformPoint(vector);
				Quaternion quaternion = Quaternion.Inverse(obj2.rotation) * gameBall.transform.rotation;
				obj2.InverseTransformPoint(gameBall.transform.position);
				GameBallManager.Instance.RequestGrabBall(gameBallId, flag2, vector3, quaternion);
			}
		}
	}

	// Token: 0x06002664 RID: 9828 RVA: 0x000CB41C File Offset: 0x000C961C
	private void UpdateHandHolding(int handIndex)
	{
		XRNode xrnode = this.GetXRNode(handIndex);
		if (ControllerInputPoller.GripFloat(xrnode) <= 0.7f)
		{
			InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(xrnode);
			Vector3 vector;
			deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out vector);
			Quaternion quaternion;
			deviceAtXRNode.TryGetFeatureValue(CommonUsages.deviceRotation, out quaternion);
			Transform transform = GorillaTagger.Instance.offlineVRRig.transform;
			Quaternion rotation = GTPlayer.Instance.turnParent.transform.rotation;
			GameBallPlayerLocal.InputData inputData = this.inputData[handIndex];
			Vector3 vector2 = inputData.GetMaxSpeed(0f, 0.05f) * inputData.GetAvgVel(0f, 0.05f).normalized;
			vector2 = rotation * vector2;
			vector2 *= transform.localScale.x;
			vector = rotation * -(Quaternion.Inverse(quaternion) * vector);
			GameBallId gameBallId = this.gamePlayer.GetGameBallId(handIndex);
			GameBall gameBall = GameBallManager.Instance.GetGameBall(gameBallId);
			if (gameBall == null)
			{
				return;
			}
			if (gameBall.IsLaunched)
			{
				return;
			}
			if (gameBall.disc)
			{
				Vector3 vector3 = gameBall.transform.rotation * gameBall.localDiscUp;
				vector3.Normalize();
				float num = Vector3.Dot(vector3, vector);
				vector = vector3 * num;
				vector *= 1.25f;
				vector2 *= 1.25f;
			}
			else
			{
				vector2 *= 1.5f;
			}
			GorillaVelocityTracker bodyVelocityTracker = GTPlayer.Instance.bodyVelocityTracker;
			vector2 += bodyVelocityTracker.GetAverageVelocity(true, 0.05f, false);
			GameBallManager.Instance.RequestThrowBall(gameBallId, GameBallPlayerLocal.IsLeftHand(handIndex), vector2, vector);
		}
	}

	// Token: 0x06002665 RID: 9829 RVA: 0x000CB5D4 File Offset: 0x000C97D4
	private XRNode GetXRNode(int handIndex)
	{
		if (handIndex != 0)
		{
			return XRNode.RightHand;
		}
		return XRNode.LeftHand;
	}

	// Token: 0x06002666 RID: 9830 RVA: 0x000CB5DC File Offset: 0x000C97DC
	private Transform GetHandTransform(int handIndex)
	{
		BodyDockPositions myBodyDockPositions = GorillaTagger.Instance.offlineVRRig.myBodyDockPositions;
		return ((handIndex == 0) ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform).parent;
	}

	// Token: 0x06002667 RID: 9831 RVA: 0x000CAF4B File Offset: 0x000C914B
	public static bool IsLeftHand(int handIndex)
	{
		return handIndex == 0;
	}

	// Token: 0x06002668 RID: 9832 RVA: 0x000CAF51 File Offset: 0x000C9151
	public static int GetHandIndex(bool leftHand)
	{
		if (!leftHand)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06002669 RID: 9833 RVA: 0x000CB60F File Offset: 0x000C980F
	public void PlayCatchFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength, 0.1f);
	}

	// Token: 0x0600266A RID: 9834 RVA: 0x000CB62B File Offset: 0x000C982B
	public void PlayThrowFx(bool isLeftHand)
	{
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength * 0.15f, 0.1f);
	}

	// Token: 0x040031D9 RID: 12761
	public GameBallPlayer gamePlayer;

	// Token: 0x040031DA RID: 12762
	private const int MAX_INPUT_HISTORY = 32;

	// Token: 0x040031DB RID: 12763
	private GameBallPlayerLocal.HandData[] hands;

	// Token: 0x040031DC RID: 12764
	private GameBallPlayerLocal.InputData[] inputData;

	// Token: 0x040031DD RID: 12765
	[OnEnterPlay_SetNull]
	public static volatile GameBallPlayerLocal instance;

	// Token: 0x02000603 RID: 1539
	private enum HandGrabState
	{
		// Token: 0x040031DF RID: 12767
		Empty,
		// Token: 0x040031E0 RID: 12768
		Holding
	}

	// Token: 0x02000604 RID: 1540
	private struct HandData
	{
		// Token: 0x040031E1 RID: 12769
		public GameBallPlayerLocal.HandGrabState grabState;

		// Token: 0x040031E2 RID: 12770
		public bool gripWasHeld;

		// Token: 0x040031E3 RID: 12771
		public double gripPressedTime;

		// Token: 0x040031E4 RID: 12772
		public GameBallId grabbedGameBallId;
	}

	// Token: 0x02000605 RID: 1541
	public struct InputDataMotion
	{
		// Token: 0x040031E5 RID: 12773
		public double time;

		// Token: 0x040031E6 RID: 12774
		public Vector3 position;

		// Token: 0x040031E7 RID: 12775
		public Quaternion rotation;

		// Token: 0x040031E8 RID: 12776
		public Vector3 velocity;

		// Token: 0x040031E9 RID: 12777
		public Vector3 angVelocity;
	}

	// Token: 0x02000606 RID: 1542
	public class InputData
	{
		// Token: 0x0600266C RID: 9836 RVA: 0x000CB64D File Offset: 0x000C984D
		public InputData(int maxInputs)
		{
			this.maxInputs = maxInputs;
			this.inputMotionHistory = new List<GameBallPlayerLocal.InputDataMotion>(maxInputs);
		}

		// Token: 0x0600266D RID: 9837 RVA: 0x000CB668 File Offset: 0x000C9868
		public void AddInput(GameBallPlayerLocal.InputDataMotion data)
		{
			if (this.inputMotionHistory.Count >= this.maxInputs)
			{
				this.inputMotionHistory.RemoveAt(0);
			}
			this.inputMotionHistory.Add(data);
		}

		// Token: 0x0600266E RID: 9838 RVA: 0x000CB698 File Offset: 0x000C9898
		public float GetMaxSpeed(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			float num3 = 0f;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GameBallPlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
				if (inputDataMotion.time <= num2)
				{
					if (inputDataMotion.time < num)
					{
						break;
					}
					float sqrMagnitude = inputDataMotion.velocity.sqrMagnitude;
					if (sqrMagnitude > num3)
					{
						num3 = sqrMagnitude;
					}
				}
			}
			return Mathf.Sqrt(num3);
		}

		// Token: 0x0600266F RID: 9839 RVA: 0x000CB714 File Offset: 0x000C9914
		public Vector3 GetAvgVel(float ignoreRecent, float window)
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = timeAsDouble - (double)ignoreRecent - (double)window;
			double num2 = timeAsDouble - (double)ignoreRecent;
			Vector3 vector = Vector3.zero;
			int num3 = 0;
			for (int i = this.inputMotionHistory.Count - 1; i >= 0; i--)
			{
				GameBallPlayerLocal.InputDataMotion inputDataMotion = this.inputMotionHistory[i];
				if (inputDataMotion.time <= num2)
				{
					if (inputDataMotion.time < num)
					{
						break;
					}
					vector += inputDataMotion.velocity;
					num3++;
				}
			}
			if (num3 == 0)
			{
				return Vector3.zero;
			}
			return vector / (float)num3;
		}

		// Token: 0x040031EA RID: 12778
		public int maxInputs;

		// Token: 0x040031EB RID: 12779
		public List<GameBallPlayerLocal.InputDataMotion> inputMotionHistory;
	}
}
