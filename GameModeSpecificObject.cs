using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GorillaGameModes;
using UnityEngine;

// Token: 0x020000A8 RID: 168
public class GameModeSpecificObject : MonoBehaviour
{
	// Token: 0x14000009 RID: 9
	// (add) Token: 0x0600041D RID: 1053 RVA: 0x00018758 File Offset: 0x00016958
	// (remove) Token: 0x0600041E RID: 1054 RVA: 0x0001878C File Offset: 0x0001698C
	public static event GameModeSpecificObject.GameModeSpecificObjectDelegate OnAwake;

	// Token: 0x1400000A RID: 10
	// (add) Token: 0x0600041F RID: 1055 RVA: 0x000187C0 File Offset: 0x000169C0
	// (remove) Token: 0x06000420 RID: 1056 RVA: 0x000187F4 File Offset: 0x000169F4
	public static event GameModeSpecificObject.GameModeSpecificObjectDelegate OnDestroyed;

	// Token: 0x1700004B RID: 75
	// (get) Token: 0x06000421 RID: 1057 RVA: 0x00018827 File Offset: 0x00016A27
	public GameModeSpecificObject.ValidationMethod Validation
	{
		get
		{
			return this.validationMethod;
		}
	}

	// Token: 0x1700004C RID: 76
	// (get) Token: 0x06000422 RID: 1058 RVA: 0x0001882F File Offset: 0x00016A2F
	public List<GameModeType> GameModes
	{
		get
		{
			return this.gameModes;
		}
	}

	// Token: 0x06000423 RID: 1059 RVA: 0x00018838 File Offset: 0x00016A38
	private async void Awake()
	{
		this.gameModes = new List<GameModeType>(this._gameModes);
		await Task.Yield();
		if (GameModeSpecificObject.OnAwake != null)
		{
			GameModeSpecificObject.OnAwake(this);
		}
	}

	// Token: 0x06000424 RID: 1060 RVA: 0x0001886F File Offset: 0x00016A6F
	private void OnDestroy()
	{
		if (GameModeSpecificObject.OnDestroyed != null)
		{
			GameModeSpecificObject.OnDestroyed(this);
		}
	}

	// Token: 0x06000425 RID: 1061 RVA: 0x00018883 File Offset: 0x00016A83
	public bool CheckValid(GameModeType gameMode)
	{
		if (this.validationMethod == GameModeSpecificObject.ValidationMethod.Exclusion)
		{
			return !this.gameModes.Contains(gameMode);
		}
		return this.gameModes.Contains(gameMode);
	}

	// Token: 0x04000488 RID: 1160
	[SerializeField]
	private GameModeSpecificObject.ValidationMethod validationMethod;

	// Token: 0x04000489 RID: 1161
	[SerializeField]
	private GameModeType[] _gameModes;

	// Token: 0x0400048A RID: 1162
	private List<GameModeType> gameModes;

	// Token: 0x020000A9 RID: 169
	// (Invoke) Token: 0x06000428 RID: 1064
	public delegate void GameModeSpecificObjectDelegate(GameModeSpecificObject gameModeSpecificObject);

	// Token: 0x020000AA RID: 170
	[Serializable]
	public enum ValidationMethod
	{
		// Token: 0x0400048C RID: 1164
		Inclusion,
		// Token: 0x0400048D RID: 1165
		Exclusion
	}
}
