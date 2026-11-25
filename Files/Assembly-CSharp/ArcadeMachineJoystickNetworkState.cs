using Photon.Pun;

public class ArcadeMachineJoystickNetworkState : NetworkComponent
{
	private ArcadeMachineJoystick joystick;

	private new void Awake()
	{
		joystick = GetComponent<ArcadeMachineJoystick>();
	}

	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		joystick.ReadDataPUN(stream, info);
	}

	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		joystick.WriteDataPUN(stream, info);
	}
}
