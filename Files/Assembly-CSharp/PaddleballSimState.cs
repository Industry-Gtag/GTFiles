using UnityEngine;

public struct PaddleballSimState
{
	public int P0Y;

	public int P1Y;

	public int P2Y;

	public int P3Y;

	public Vector2Int BallPos;

	public int ScoreL;

	public int ScoreR;

	public Vector2Int BallTrajectory;

	public int BallSpeed;

	public bool GameActive;

	public int ActivePaddles;

	private static object[] serializedSpace = new object[12];

	public object[] Serialize()
	{
		serializedSpace[0] = (short)P0Y;
		serializedSpace[1] = (short)P1Y;
		serializedSpace[2] = (short)P2Y;
		serializedSpace[3] = (short)P3Y;
		serializedSpace[4] = (short)BallPos.x;
		serializedSpace[5] = (short)BallPos.y;
		serializedSpace[6] = (byte)(ScoreL * 12 + ScoreR);
		serializedSpace[7] = (short)BallTrajectory.x;
		serializedSpace[8] = (short)BallTrajectory.y;
		serializedSpace[9] = BallSpeed;
		serializedSpace[10] = (byte)ActivePaddles;
		serializedSpace[11] = GameActive;
		return serializedSpace;
	}

	public bool Deserialize(object[] data)
	{
		if (data.Length != 12 || !(data[0] is short p0Y) || !(data[1] is short p1Y) || !(data[2] is short p2Y) || !(data[3] is short p3Y) || !(data[4] is short x) || !(data[5] is short y) || !(data[6] is byte b) || !(data[7] is short x2) || !(data[8] is short y2) || !(data[9] is int ballSpeed) || !(data[10] is byte activePaddles) || !(data[11] is bool gameActive))
		{
			return false;
		}
		P0Y = p0Y;
		P1Y = p1Y;
		P2Y = p2Y;
		P3Y = p3Y;
		BallPos.x = x;
		BallPos.y = y;
		ScoreL = b / 12;
		ScoreR = b % 12;
		BallTrajectory.x = x2;
		BallTrajectory.y = y2;
		BallSpeed = ballSpeed;
		ActivePaddles = activePaddles;
		GameActive = gameActive;
		return true;
	}
}
