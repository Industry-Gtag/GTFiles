using System;

// Token: 0x020005FC RID: 1532
public struct GameBallId
{
	// Token: 0x06002620 RID: 9760 RVA: 0x000C9998 File Offset: 0x000C7B98
	public GameBallId(int index)
	{
		this.index = index;
	}

	// Token: 0x06002621 RID: 9761 RVA: 0x000C99A1 File Offset: 0x000C7BA1
	public bool IsValid()
	{
		return this.index != -1;
	}

	// Token: 0x06002622 RID: 9762 RVA: 0x000C99AF File Offset: 0x000C7BAF
	public static bool operator ==(GameBallId obj1, GameBallId obj2)
	{
		return obj1.index == obj2.index;
	}

	// Token: 0x06002623 RID: 9763 RVA: 0x000C99BF File Offset: 0x000C7BBF
	public static bool operator !=(GameBallId obj1, GameBallId obj2)
	{
		return obj1.index != obj2.index;
	}

	// Token: 0x06002624 RID: 9764 RVA: 0x000C99D4 File Offset: 0x000C7BD4
	public override bool Equals(object obj)
	{
		GameBallId gameBallId = (GameBallId)obj;
		return this.index == gameBallId.index;
	}

	// Token: 0x06002625 RID: 9765 RVA: 0x000C99F6 File Offset: 0x000C7BF6
	public override int GetHashCode()
	{
		return this.index.GetHashCode();
	}

	// Token: 0x040031B6 RID: 12726
	public static GameBallId Invalid = new GameBallId(-1);

	// Token: 0x040031B7 RID: 12727
	public int index;
}
