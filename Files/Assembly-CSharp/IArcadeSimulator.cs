internal interface IArcadeSimulator
{
	int currentFrame { get; }

	void FinalizeJoinGameData(out double anchorFrameTimestamp, out int anchorFrame, out object[] stateData);

	void JoinGameInProgress(double anchorFrameTimestamp, int anchorFrame, object[] stateData);

	void StartGame();
}
internal interface IArcadeSimulator<T> : IArcadeSimulator
{
	void Tick(ref T gameState, ArcadeButtons[] playerInputs, ArcadeButtons[] playerInputsLastFrame, int frame, bool withAudio = false);

	void Display(T gameState);
}
