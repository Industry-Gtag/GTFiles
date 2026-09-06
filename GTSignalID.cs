using System;

// Token: 0x020008F0 RID: 2288
[Serializable]
public struct GTSignalID : IEquatable<GTSignalID>, IEquatable<int>
{
	// Token: 0x06003BF2 RID: 15346 RVA: 0x0014795C File Offset: 0x00145B5C
	public override bool Equals(object obj)
	{
		if (obj is GTSignalID)
		{
			GTSignalID gtsignalID = (GTSignalID)obj;
			return this.Equals(gtsignalID);
		}
		if (obj is int)
		{
			int num = (int)obj;
			return this.Equals(num);
		}
		return false;
	}

	// Token: 0x06003BF3 RID: 15347 RVA: 0x00147998 File Offset: 0x00145B98
	public bool Equals(GTSignalID other)
	{
		return this._id == other._id;
	}

	// Token: 0x06003BF4 RID: 15348 RVA: 0x001479A8 File Offset: 0x00145BA8
	public bool Equals(int other)
	{
		return this._id == other;
	}

	// Token: 0x06003BF5 RID: 15349 RVA: 0x001479B3 File Offset: 0x00145BB3
	public override int GetHashCode()
	{
		return this._id;
	}

	// Token: 0x06003BF6 RID: 15350 RVA: 0x001479BB File Offset: 0x00145BBB
	public static bool operator ==(GTSignalID x, GTSignalID y)
	{
		return x.Equals(y);
	}

	// Token: 0x06003BF7 RID: 15351 RVA: 0x001479C5 File Offset: 0x00145BC5
	public static bool operator !=(GTSignalID x, GTSignalID y)
	{
		return !x.Equals(y);
	}

	// Token: 0x06003BF8 RID: 15352 RVA: 0x001479B3 File Offset: 0x00145BB3
	public static implicit operator int(GTSignalID sid)
	{
		return sid._id;
	}

	// Token: 0x06003BF9 RID: 15353 RVA: 0x001479D4 File Offset: 0x00145BD4
	public static implicit operator GTSignalID(string s)
	{
		return new GTSignalID
		{
			_id = GTSignal.ComputeID(s)
		};
	}

	// Token: 0x04004C75 RID: 19573
	private int _id;
}
