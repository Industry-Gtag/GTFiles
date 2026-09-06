using System;

// Token: 0x020006AF RID: 1711
[Serializable]
public struct GroupJoinZoneAB
{
	// Token: 0x06002A9A RID: 10906 RVA: 0x000E5918 File Offset: 0x000E3B18
	public static GroupJoinZoneAB operator &(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return new GroupJoinZoneAB
		{
			a = (one.a & two.a),
			b = (one.b & two.b)
		};
	}

	// Token: 0x06002A9B RID: 10907 RVA: 0x000E5958 File Offset: 0x000E3B58
	public static GroupJoinZoneAB operator |(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return new GroupJoinZoneAB
		{
			a = (one.a | two.a),
			b = (one.b | two.b)
		};
	}

	// Token: 0x06002A9C RID: 10908 RVA: 0x000E5998 File Offset: 0x000E3B98
	public static GroupJoinZoneAB operator ~(GroupJoinZoneAB z)
	{
		return new GroupJoinZoneAB
		{
			a = ~z.a,
			b = ~z.b
		};
	}

	// Token: 0x06002A9D RID: 10909 RVA: 0x000E59CA File Offset: 0x000E3BCA
	public static bool operator ==(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return one.a == two.a && one.b == two.b;
	}

	// Token: 0x06002A9E RID: 10910 RVA: 0x000E59EA File Offset: 0x000E3BEA
	public static bool operator !=(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return one.a != two.a || one.b != two.b;
	}

	// Token: 0x06002A9F RID: 10911 RVA: 0x000E5A0D File Offset: 0x000E3C0D
	public bool HasAnyFlag(GroupJoinZoneAB other)
	{
		return (this.a & other.a) != ~(GroupJoinZoneA.Basement | GroupJoinZoneA.Beach | GroupJoinZoneA.Cave | GroupJoinZoneA.Canyon | GroupJoinZoneA.City | GroupJoinZoneA.Clouds | GroupJoinZoneA.Forest | GroupJoinZoneA.Mountain | GroupJoinZoneA.Rotating | GroupJoinZoneA.Mines | GroupJoinZoneA.Arena | GroupJoinZoneA.ArenaTunnel | GroupJoinZoneA.Hoverboard | GroupJoinZoneA.TreeRoom | GroupJoinZoneA.MountainTunnel | GroupJoinZoneA.BasementTunnel | GroupJoinZoneA.RotatingTunnel | GroupJoinZoneA.BeachTunnel | GroupJoinZoneA.CloudsElevator | GroupJoinZoneA.MinesTunnel | GroupJoinZoneA.CavesComputer | GroupJoinZoneA.Metropolis | GroupJoinZoneA.MetropolisTunnel | GroupJoinZoneA.Attic | GroupJoinZoneA.Arcade | GroupJoinZoneA.ArcadeTunnel | GroupJoinZoneA.Bayou | GroupJoinZoneA.BayouTunnel | GroupJoinZoneA.CustomMaps | GroupJoinZoneA.MallConnector | GroupJoinZoneA.MonkeBlocks | GroupJoinZoneA.GTFC) || (this.b & other.b) > (GroupJoinZoneB)0;
	}

	// Token: 0x06002AA0 RID: 10912 RVA: 0x000E5A30 File Offset: 0x000E3C30
	public override bool Equals(object other)
	{
		return this == (GroupJoinZoneAB)other;
	}

	// Token: 0x06002AA1 RID: 10913 RVA: 0x000E5A43 File Offset: 0x000E3C43
	public override int GetHashCode()
	{
		return this.a.GetHashCode() ^ this.b.GetHashCode();
	}

	// Token: 0x06002AA2 RID: 10914 RVA: 0x000E5A68 File Offset: 0x000E3C68
	public static implicit operator GroupJoinZoneAB(int d)
	{
		return new GroupJoinZoneAB
		{
			a = (GroupJoinZoneA)d
		};
	}

	// Token: 0x06002AA3 RID: 10915 RVA: 0x000E5A88 File Offset: 0x000E3C88
	public override string ToString()
	{
		if (this.b == (GroupJoinZoneB)0)
		{
			return this.a.ToString();
		}
		if (this.a != ~(GroupJoinZoneA.Basement | GroupJoinZoneA.Beach | GroupJoinZoneA.Cave | GroupJoinZoneA.Canyon | GroupJoinZoneA.City | GroupJoinZoneA.Clouds | GroupJoinZoneA.Forest | GroupJoinZoneA.Mountain | GroupJoinZoneA.Rotating | GroupJoinZoneA.Mines | GroupJoinZoneA.Arena | GroupJoinZoneA.ArenaTunnel | GroupJoinZoneA.Hoverboard | GroupJoinZoneA.TreeRoom | GroupJoinZoneA.MountainTunnel | GroupJoinZoneA.BasementTunnel | GroupJoinZoneA.RotatingTunnel | GroupJoinZoneA.BeachTunnel | GroupJoinZoneA.CloudsElevator | GroupJoinZoneA.MinesTunnel | GroupJoinZoneA.CavesComputer | GroupJoinZoneA.Metropolis | GroupJoinZoneA.MetropolisTunnel | GroupJoinZoneA.Attic | GroupJoinZoneA.Arcade | GroupJoinZoneA.ArcadeTunnel | GroupJoinZoneA.Bayou | GroupJoinZoneA.BayouTunnel | GroupJoinZoneA.CustomMaps | GroupJoinZoneA.MallConnector | GroupJoinZoneA.MonkeBlocks | GroupJoinZoneA.GTFC))
		{
			return this.a.ToString() + "," + this.b.ToString();
		}
		return this.b.ToString();
	}

	// Token: 0x040037A2 RID: 14242
	public GroupJoinZoneA a;

	// Token: 0x040037A3 RID: 14243
	public GroupJoinZoneB b;
}
