using System;
using System.Collections.Generic;

// Token: 0x020003CD RID: 973
[Serializable]
public class ScenePerformanceData
{
	// Token: 0x06001757 RID: 5975 RVA: 0x00086FFC File Offset: 0x000851FC
	public ScenePerformanceData(string mapName, int gorillaCount, int droppedFrames, int msHigh, int medianMS, int medianFPS, int medianDrawCalls, List<int> msCaptures)
	{
		this._mapName = mapName;
		this._gorillaCount = gorillaCount;
		this._droppedFrames = droppedFrames;
		this._msHigh = msHigh;
		this._medianMS = medianMS;
		this._medianFPS = medianFPS;
		this._medianDrawCallCount = medianDrawCalls;
		this._msCaptures = new List<int>(msCaptures);
	}

	// Token: 0x04002290 RID: 8848
	public string _mapName;

	// Token: 0x04002291 RID: 8849
	public int _gorillaCount;

	// Token: 0x04002292 RID: 8850
	public int _droppedFrames;

	// Token: 0x04002293 RID: 8851
	public int _msHigh;

	// Token: 0x04002294 RID: 8852
	public int _medianMS;

	// Token: 0x04002295 RID: 8853
	public int _medianFPS;

	// Token: 0x04002296 RID: 8854
	public int _medianDrawCallCount;

	// Token: 0x04002297 RID: 8855
	public List<int> _msCaptures;
}
