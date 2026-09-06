using System;
using UnityEngine;

namespace GorillaGameModes
{
	// Token: 0x02000F2D RID: 3885
	public class GameModeString
	{
		// Token: 0x06005F37 RID: 24375 RVA: 0x001E3C8C File Offset: 0x001E1E8C
		public override string ToString()
		{
			return string.Concat(new string[] { this.zone, ";", this.queue, ";", this.gameType, ";", this.modId, ";", this.modFileId });
		}

		// Token: 0x06005F38 RID: 24376 RVA: 0x001E3CF4 File Offset: 0x001E1EF4
		public static GameModeString FromString(string gameModeString)
		{
			string[] array = gameModeString.Split(";", StringSplitOptions.None);
			if (array.Length != 5)
			{
				Debug.LogError("[GameModeString::FromString] Invalid game mode string: " + gameModeString);
				return null;
			}
			return new GameModeString
			{
				zone = array[0],
				queue = array[1],
				gameType = array[2],
				modId = array[3],
				modFileId = array[4]
			};
		}

		// Token: 0x06005F39 RID: 24377 RVA: 0x001E3D58 File Offset: 0x001E1F58
		public static bool DoesPropertyStringContainGameMode(string propertyString, string gameMode)
		{
			return GameModeString.GameTypeFromPropertyString(propertyString).Equals(gameMode, StringComparison.Ordinal);
		}

		// Token: 0x06005F3A RID: 24378 RVA: 0x001E3D6C File Offset: 0x001E1F6C
		public static ReadOnlySpan<char> GameTypeFromPropertyString(string propertyString)
		{
			if (string.IsNullOrEmpty(propertyString))
			{
				return null;
			}
			int num = propertyString.IndexOf(';');
			if (num < 0)
			{
				return null;
			}
			num = propertyString.IndexOf(';', num + 1);
			if (num < 0)
			{
				return null;
			}
			int num2 = propertyString.IndexOf(';', ++num);
			if (num2 < 0)
			{
				return null;
			}
			return propertyString.AsSpan(num, num2 - num);
		}

		// Token: 0x04006DBA RID: 28090
		public string zone;

		// Token: 0x04006DBB RID: 28091
		public string queue;

		// Token: 0x04006DBC RID: 28092
		public string gameType;

		// Token: 0x04006DBD RID: 28093
		public string modId;

		// Token: 0x04006DBE RID: 28094
		public string modFileId;
	}
}
