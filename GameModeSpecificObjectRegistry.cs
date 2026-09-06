using System;
using System.Collections.Generic;
using GorillaGameModes;
using UnityEngine;

// Token: 0x020000AC RID: 172
public class GameModeSpecificObjectRegistry : MonoBehaviour
{
	// Token: 0x0600042D RID: 1069 RVA: 0x00018996 File Offset: 0x00016B96
	private void OnEnable()
	{
		GameModeSpecificObject.OnAwake += this.GameModeSpecificObject_OnAwake;
		GameModeSpecificObject.OnDestroyed += this.GameModeSpecificObject_OnDestroyed;
		GameMode.OnStartGameMode += this.GameMode_OnStartGameMode;
	}

	// Token: 0x0600042E RID: 1070 RVA: 0x000189CB File Offset: 0x00016BCB
	private void OnDisable()
	{
		GameModeSpecificObject.OnAwake -= this.GameModeSpecificObject_OnAwake;
		GameModeSpecificObject.OnDestroyed -= this.GameModeSpecificObject_OnDestroyed;
		GameMode.OnStartGameMode -= this.GameMode_OnStartGameMode;
	}

	// Token: 0x0600042F RID: 1071 RVA: 0x00018A00 File Offset: 0x00016C00
	private void GameModeSpecificObject_OnAwake(GameModeSpecificObject obj)
	{
		foreach (GameModeType gameModeType in obj.GameModes)
		{
			if (!this.gameModeSpecificObjects.ContainsKey(gameModeType))
			{
				this.gameModeSpecificObjects.Add(gameModeType, new List<GameModeSpecificObject>());
			}
			this.gameModeSpecificObjects[gameModeType].Add(obj);
		}
		if (GameMode.ActiveGameMode == null)
		{
			obj.gameObject.SetActive(obj.Validation == GameModeSpecificObject.ValidationMethod.Exclusion);
			return;
		}
		obj.gameObject.SetActive(obj.CheckValid(GameMode.ActiveGameMode.GameType()));
	}

	// Token: 0x06000430 RID: 1072 RVA: 0x00018ABC File Offset: 0x00016CBC
	private void GameModeSpecificObject_OnDestroyed(GameModeSpecificObject obj)
	{
		foreach (GameModeType gameModeType in obj.GameModes)
		{
			if (this.gameModeSpecificObjects.ContainsKey(gameModeType))
			{
				this.gameModeSpecificObjects[gameModeType].Remove(obj);
			}
		}
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x00018B2C File Offset: 0x00016D2C
	private void GameMode_OnStartGameMode(GameModeType newGameModeType)
	{
		if (this.currentGameType == newGameModeType)
		{
			return;
		}
		if (this.gameModeSpecificObjects.ContainsKey(this.currentGameType))
		{
			foreach (GameModeSpecificObject gameModeSpecificObject in this.gameModeSpecificObjects[this.currentGameType])
			{
				gameModeSpecificObject.gameObject.SetActive(gameModeSpecificObject.CheckValid(newGameModeType));
			}
		}
		if (this.gameModeSpecificObjects.ContainsKey(newGameModeType))
		{
			foreach (GameModeSpecificObject gameModeSpecificObject2 in this.gameModeSpecificObjects[newGameModeType])
			{
				gameModeSpecificObject2.gameObject.SetActive(gameModeSpecificObject2.CheckValid(newGameModeType));
			}
		}
		this.currentGameType = newGameModeType;
	}

	// Token: 0x04000492 RID: 1170
	private Dictionary<GameModeType, List<GameModeSpecificObject>> gameModeSpecificObjects = new Dictionary<GameModeType, List<GameModeSpecificObject>>();

	// Token: 0x04000493 RID: 1171
	private GameModeType currentGameType = GameModeType.Count;
}
