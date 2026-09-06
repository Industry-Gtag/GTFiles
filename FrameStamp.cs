using System;
using UnityEngine;

// Token: 0x02000AF2 RID: 2802
[Serializable]
public struct FrameStamp
{
	// Token: 0x170006D0 RID: 1744
	// (get) Token: 0x060047D5 RID: 18389 RVA: 0x00183E21 File Offset: 0x00182021
	public int framesElapsed
	{
		get
		{
			return Time.frameCount - this._lastFrame;
		}
	}

	// Token: 0x060047D6 RID: 18390 RVA: 0x00183E30 File Offset: 0x00182030
	public static FrameStamp Now()
	{
		return new FrameStamp
		{
			_lastFrame = Time.frameCount
		};
	}

	// Token: 0x060047D7 RID: 18391 RVA: 0x00183E52 File Offset: 0x00182052
	public override string ToString()
	{
		return string.Format("{0} frames elapsed", this.framesElapsed);
	}

	// Token: 0x060047D8 RID: 18392 RVA: 0x00183E69 File Offset: 0x00182069
	public override int GetHashCode()
	{
		return StaticHash.Compute(this._lastFrame);
	}

	// Token: 0x060047D9 RID: 18393 RVA: 0x00183E76 File Offset: 0x00182076
	public static implicit operator int(FrameStamp fs)
	{
		return fs.framesElapsed;
	}

	// Token: 0x060047DA RID: 18394 RVA: 0x00183E80 File Offset: 0x00182080
	public static implicit operator FrameStamp(int framesElapsed)
	{
		return new FrameStamp
		{
			_lastFrame = Time.frameCount - framesElapsed
		};
	}

	// Token: 0x04005A4F RID: 23119
	private int _lastFrame;
}
