using System;

// Token: 0x020006CF RID: 1743
public struct GameEntityId
{
	// Token: 0x06002B93 RID: 11155 RVA: 0x000E8A70 File Offset: 0x000E6C70
	public bool IsValid()
	{
		return this.index != -1;
	}

	// Token: 0x06002B94 RID: 11156 RVA: 0x000E8A7E File Offset: 0x000E6C7E
	public static bool operator ==(GameEntityId obj1, GameEntityId obj2)
	{
		return obj1.index == obj2.index;
	}

	// Token: 0x06002B95 RID: 11157 RVA: 0x000E8A8E File Offset: 0x000E6C8E
	public static bool operator !=(GameEntityId obj1, GameEntityId obj2)
	{
		return obj1.index != obj2.index;
	}

	// Token: 0x06002B96 RID: 11158 RVA: 0x000E8AA4 File Offset: 0x000E6CA4
	public override bool Equals(object obj)
	{
		GameEntityId gameEntityId = (GameEntityId)obj;
		return this.index == gameEntityId.index;
	}

	// Token: 0x06002B97 RID: 11159 RVA: 0x000E8AC6 File Offset: 0x000E6CC6
	public override int GetHashCode()
	{
		return this.index.GetHashCode();
	}

	// Token: 0x04003859 RID: 14425
	public static GameEntityId Invalid = new GameEntityId
	{
		index = -1
	};

	// Token: 0x0400385A RID: 14426
	public int index;
}
