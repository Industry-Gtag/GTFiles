using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000544 RID: 1348
[Serializable]
public class AdvancedItemState
{
	// Token: 0x06002215 RID: 8725 RVA: 0x000B6916 File Offset: 0x000B4B16
	public void Encode()
	{
		this._encodedValue = this.EncodeData();
	}

	// Token: 0x06002216 RID: 8726 RVA: 0x000B6924 File Offset: 0x000B4B24
	public void Decode()
	{
		AdvancedItemState advancedItemState = this.DecodeData(this._encodedValue);
		this.index = advancedItemState.index;
		this.preData = advancedItemState.preData;
		this.limitAxis = advancedItemState.limitAxis;
		this.reverseGrip = advancedItemState.reverseGrip;
		this.angle = advancedItemState.angle;
	}

	// Token: 0x06002217 RID: 8727 RVA: 0x000B697C File Offset: 0x000B4B7C
	public Quaternion GetQuaternion()
	{
		Vector3 one = Vector3.one;
		if (this.reverseGrip)
		{
			switch (this.limitAxis)
			{
			case LimitAxis.NoMovement:
				return Quaternion.identity;
			case LimitAxis.YAxis:
				return Quaternion.identity;
			case LimitAxis.XAxis:
			case LimitAxis.ZAxis:
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		return Quaternion.identity;
	}

	// Token: 0x06002218 RID: 8728 RVA: 0x000B69D0 File Offset: 0x000B4BD0
	[return: TupleElementNames(new string[] { "grabPointIndex", "YRotation", "XRotation", "ZRotation" })]
	public ValueTuple<int, float, float, float> DecodeAdvancedItemState(int encodedValue)
	{
		int num = (encodedValue >> 21) & 255;
		float num2 = (float)((encodedValue >> 14) & 127) / 128f * 360f;
		float num3 = (float)((encodedValue >> 7) & 127) / 128f * 360f;
		float num4 = (float)(encodedValue & 127) / 128f * 360f;
		return new ValueTuple<int, float, float, float>(num, num2, num3, num4);
	}

	// Token: 0x1700039F RID: 927
	// (get) Token: 0x06002219 RID: 8729 RVA: 0x000B6A2A File Offset: 0x000B4C2A
	private float EncodedDeltaRotation
	{
		get
		{
			return this.GetEncodedDeltaRotation();
		}
	}

	// Token: 0x0600221A RID: 8730 RVA: 0x000B6A32 File Offset: 0x000B4C32
	public float GetEncodedDeltaRotation()
	{
		return Mathf.Abs(Mathf.Atan2(this.angleVectorWhereUpIsStandard.x, this.angleVectorWhereUpIsStandard.y)) / 3.1415927f;
	}

	// Token: 0x0600221B RID: 8731 RVA: 0x000B6A5C File Offset: 0x000B4C5C
	public void DecodeDeltaRotation(float encodedDelta, bool isFlipped)
	{
		float num = encodedDelta * 3.1415927f;
		if (isFlipped)
		{
			this.angleVectorWhereUpIsStandard = new Vector2(-Mathf.Sin(num), Mathf.Cos(num));
		}
		else
		{
			this.angleVectorWhereUpIsStandard = new Vector2(Mathf.Sin(num), Mathf.Cos(num));
		}
		switch (this.limitAxis)
		{
		case LimitAxis.NoMovement:
		case LimitAxis.XAxis:
		case LimitAxis.ZAxis:
			return;
		case LimitAxis.YAxis:
		{
			Vector3 vector = new Vector3(this.angleVectorWhereUpIsStandard.x, 0f, this.angleVectorWhereUpIsStandard.y);
			Vector3 vector2 = (this.reverseGrip ? Vector3.down : Vector3.up);
			this.deltaRotation = Quaternion.LookRotation(vector, vector2);
			return;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x0600221C RID: 8732 RVA: 0x000B6B10 File Offset: 0x000B4D10
	public int EncodeData()
	{
		int num = 0;
		if ((this.index >= 32) | (this.index < 0))
		{
			throw new ArgumentOutOfRangeException(string.Format("Index is invalid {0}", this.index));
		}
		num |= this.index << 25;
		AdvancedItemState.PointType pointType = this.preData.pointType;
		num |= (int)((int)(pointType & (AdvancedItemState.PointType)7) << 22);
		num |= (int)((int)this.limitAxis << 19);
		num |= (this.reverseGrip ? 1 : 0) << 18;
		bool flag = this.angleVectorWhereUpIsStandard.x < 0f;
		if (pointType != AdvancedItemState.PointType.Standard)
		{
			if (pointType != AdvancedItemState.PointType.DistanceBased)
			{
				throw new ArgumentOutOfRangeException();
			}
			int num2 = (int)(this.GetEncodedDeltaRotation() * 512f) & 511;
			num |= (flag ? 1 : 0) << 17;
			num |= num2 << 9;
			int num3 = (int)(this.preData.distAlongLine * 256f) & 255;
			num |= num3;
		}
		else
		{
			int num4 = (int)(this.GetEncodedDeltaRotation() * 65536f) & 65535;
			num |= (flag ? 1 : 0) << 17;
			num |= num4 << 1;
		}
		return num;
	}

	// Token: 0x0600221D RID: 8733 RVA: 0x000B6C2C File Offset: 0x000B4E2C
	public AdvancedItemState DecodeData(int encoded)
	{
		AdvancedItemState advancedItemState = new AdvancedItemState();
		advancedItemState.index = (encoded >> 25) & 31;
		advancedItemState.limitAxis = (LimitAxis)((encoded >> 19) & 7);
		advancedItemState.reverseGrip = ((encoded >> 18) & 1) == 1;
		AdvancedItemState.PointType pointType = (AdvancedItemState.PointType)((encoded >> 22) & 7);
		if (pointType != AdvancedItemState.PointType.Standard)
		{
			if (pointType != AdvancedItemState.PointType.DistanceBased)
			{
				throw new ArgumentOutOfRangeException();
			}
			advancedItemState.preData = new AdvancedItemState.PreData
			{
				pointType = pointType,
				distAlongLine = (float)(encoded & 255) / 256f
			};
			this.DecodeDeltaRotation((float)((encoded >> 9) & 511) / 512f, ((encoded >> 17) & 1) > 0);
		}
		else
		{
			advancedItemState.preData = new AdvancedItemState.PreData
			{
				pointType = pointType
			};
			this.DecodeDeltaRotation((float)((encoded >> 1) & 65535) / 65536f, ((encoded >> 17) & 1) > 0);
		}
		return advancedItemState;
	}

	// Token: 0x04002D11 RID: 11537
	private int _encodedValue;

	// Token: 0x04002D12 RID: 11538
	public Vector2 angleVectorWhereUpIsStandard;

	// Token: 0x04002D13 RID: 11539
	public Quaternion deltaRotation;

	// Token: 0x04002D14 RID: 11540
	public int index;

	// Token: 0x04002D15 RID: 11541
	public AdvancedItemState.PreData preData;

	// Token: 0x04002D16 RID: 11542
	public LimitAxis limitAxis;

	// Token: 0x04002D17 RID: 11543
	public bool reverseGrip;

	// Token: 0x04002D18 RID: 11544
	public float angle;

	// Token: 0x02000545 RID: 1349
	[Serializable]
	public class PreData
	{
		// Token: 0x04002D19 RID: 11545
		public float distAlongLine;

		// Token: 0x04002D1A RID: 11546
		public AdvancedItemState.PointType pointType;
	}

	// Token: 0x02000546 RID: 1350
	public enum PointType
	{
		// Token: 0x04002D1C RID: 11548
		Standard,
		// Token: 0x04002D1D RID: 11549
		DistanceBased
	}
}
