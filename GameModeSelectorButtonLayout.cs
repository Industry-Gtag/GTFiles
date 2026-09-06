using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Pool;

// Token: 0x020000A5 RID: 165
public class GameModeSelectorButtonLayout : MonoBehaviour
{
	// Token: 0x0600040B RID: 1035 RVA: 0x00017F64 File Offset: 0x00016164
	private void OnEnable()
	{
		this.SetupButtons();
		NetworkSystem.Instance.OnJoinedRoomEvent += this.SetupButtons;
		if (this.superToggleButton != null)
		{
			this.superToggleButton.onPressed += this._OnPressedSuperToggleButton;
		}
	}

	// Token: 0x0600040C RID: 1036 RVA: 0x00017FC0 File Offset: 0x000161C0
	private void OnDisable()
	{
		NetworkSystem.Instance.OnJoinedRoomEvent -= this.SetupButtons;
		if (this.superToggleButton != null)
		{
			this.superToggleButton.onPressed -= this._OnPressedSuperToggleButton;
		}
	}

	// Token: 0x0600040D RID: 1037 RVA: 0x00018014 File Offset: 0x00016214
	protected virtual async void SetupButtons()
	{
		int count = 0;
		while (GorillaComputer.instance == null)
		{
			await Task.Delay(100);
		}
		bool flag = GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone != this.zone;
		HashSet<GameModeType> modesForZone = GameMode.GameModeZoneMapping.GetModesForZone(this.zone, NetworkSystem.Instance.SessionIsPrivate);
		HashSet<GameModeType> hashSet;
		using (CollectionPool<HashSet<GameModeType>, GameModeType>.Get(out hashSet))
		{
			bool flag2 = modesForZone.Contains(GameModeType.SuperCasual) || modesForZone.Contains(GameModeType.SuperInfect);
			this.superToggleButton.transform.parent.gameObject.SetActive(flag2);
			this.superToggleButton.isOn = flag2 && PlayerPrefFlags.Check(PlayerPrefFlags.Flag.GAME_MODE_SELECTOR_IS_SUPER);
			this.superToggleButton.UpdateColor();
			if (this.superToggleButton.isOn)
			{
				using (HashSet<GameModeType>.Enumerator enumerator = modesForZone.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						GameModeType gameModeType = enumerator.Current;
						if (gameModeType != GameModeType.Casual && gameModeType != GameModeType.Infection)
						{
							hashSet.Add(gameModeType);
						}
					}
					goto IL_01B8;
				}
			}
			foreach (GameModeType gameModeType2 in modesForZone)
			{
				if (gameModeType2 != GameModeType.SuperCasual && gameModeType2 != GameModeType.SuperInfect)
				{
					hashSet.Add(gameModeType2);
				}
			}
			IL_01B8:
			foreach (GameModeType gameModeType3 in hashSet)
			{
				if (count == this.currentButtons.Count)
				{
					this.currentButtons.Add(Object.Instantiate<ModeSelectButton>(this.pf_button, base.transform));
				}
				ModeSelectButton modeSelectButton = this.currentButtons[count];
				modeSelectButton.transform.localPosition = new Vector3((float)count * -0.15f, 0f, 0f);
				modeSelectButton.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
				modeSelectButton.WarningScreen = this.warningScreen;
				string empty = string.Empty;
				if (NetworkSystem.Instance.SessionIsSubscription)
				{
					GameMode.GameModeZoneMapping.IsBigRoomMode(gameModeType3);
				}
				modeSelectButton.SetInfo(gameModeType3.ToString() + empty, GameMode.GameModeZoneMapping.GetModeName(gameModeType3), GameMode.GameModeZoneMapping.IsNew(gameModeType3), GameMode.GameModeZoneMapping.GetCountdown(gameModeType3));
				modeSelectButton.gameObject.SetActive(true);
				count++;
				flag |= string.Equals(GorillaComputer.instance.currentGameMode.Value, gameModeType3.ToString(), StringComparison.CurrentCultureIgnoreCase);
			}
			for (int i = count; i < this.currentButtons.Count; i++)
			{
				this.currentButtons[i].gameObject.SetActive(false);
			}
			if (!flag)
			{
				GorillaComputer.instance.SetGameModeWithoutButton(this.currentButtons[0].gameMode);
			}
		}
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x0001804C File Offset: 0x0001624C
	private void _OnPressedSuperToggleButton(GorillaPressableButton btn, bool isLeftHandPress)
	{
		if (GorillaComputer.instance == null)
		{
			Debug.Log("[GT/GameModeSelectorButtonLayout]  Tried pressing SUPER button but `GorillaComputer` is not ready.", this);
			return;
		}
		if (NetworkSystem.Instance == null)
		{
			Debug.Log("[GT/GameModeSelectorButtonLayout]  Tried pressing SUPER button but `NetworkSystem` is not ready.", this);
			return;
		}
		btn.isOn = !btn.isOn;
		PlayerPrefFlags.Set(PlayerPrefFlags.Flag.GAME_MODE_SELECTOR_IS_SUPER, btn.isOn);
		this.SetupButtons();
		HashSet<GameModeType> modesForZone = GameMode.GameModeZoneMapping.GetModesForZone(this.zone, NetworkSystem.Instance.SessionIsPrivate);
		GameModeType lastPressedGameModeType = GorillaComputer.instance.lastPressedGameModeType;
		GameModeType gameModeType;
		if ((lastPressedGameModeType == GameModeType.Casual || lastPressedGameModeType == GameModeType.SuperCasual) && modesForZone.Contains(GameModeType.Casual) && modesForZone.Contains(GameModeType.SuperCasual))
		{
			gameModeType = (btn.isOn ? GameModeType.SuperCasual : GameModeType.Casual);
		}
		else if ((lastPressedGameModeType == GameModeType.Infection || lastPressedGameModeType == GameModeType.SuperInfect) && modesForZone.Contains(GameModeType.Infection) && modesForZone.Contains(GameModeType.SuperInfect))
		{
			gameModeType = (btn.isOn ? GameModeType.SuperInfect : GameModeType.Infection);
		}
		else
		{
			gameModeType = lastPressedGameModeType;
		}
		GorillaComputer.instance.OnModeSelectButtonPress(gameModeType.ToString(), isLeftHandPress);
	}

	// Token: 0x04000476 RID: 1142
	private const string preLog = "[GT/GameModeSelectorButtonLayout]  ";

	// Token: 0x04000477 RID: 1143
	private const string preErr = "ERROR!!!  ";

	// Token: 0x04000478 RID: 1144
	[SerializeField]
	protected GorillaPressableButton superToggleButton;

	// Token: 0x04000479 RID: 1145
	[SerializeField]
	protected ModeSelectButton pf_button;

	// Token: 0x0400047A RID: 1146
	[SerializeField]
	protected GTZone zone;

	// Token: 0x0400047B RID: 1147
	[SerializeField]
	protected PartyGameModeWarning warningScreen;

	// Token: 0x0400047C RID: 1148
	protected List<ModeSelectButton> currentButtons = new List<ModeSelectButton>();
}
