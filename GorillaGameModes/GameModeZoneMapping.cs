using System;
using System.Collections.Generic;
using GameObjectScheduling;
using GorillaNetworking;
using UnityEngine;

namespace GorillaGameModes
{
	// Token: 0x02000F31 RID: 3889
	[CreateAssetMenu(fileName = "New Game Mode Zone Map", menuName = "Game Settings/Game Mode Zone Map", order = 2)]
	public class GameModeZoneMapping : ScriptableObject
	{
		// Token: 0x1700093E RID: 2366
		// (get) Token: 0x06005F78 RID: 24440 RVA: 0x001E49C9 File Offset: 0x001E2BC9
		public HashSet<GameModeType> AllModes
		{
			get
			{
				this.Init();
				return this.allModes;
			}
		}

		// Token: 0x06005F79 RID: 24441 RVA: 0x001E49D8 File Offset: 0x001E2BD8
		private void Init()
		{
			if (this.allModes != null)
			{
				return;
			}
			this.allModes = new HashSet<GameModeType>();
			for (int i = 0; i < this.defaultGameModes.Length; i++)
			{
				this.allModes.Add(this.defaultGameModes[i]);
			}
			this.bigRoomZoneGameModesLookup = new Dictionary<GTZone, HashSet<GameModeType>>();
			this.publicZoneGameModesLookup = new Dictionary<GTZone, HashSet<GameModeType>>();
			this.privateZoneGameModesLookup = new Dictionary<GTZone, HashSet<GameModeType>>();
			for (int j = 0; j < this.zoneGameModes.Length; j++)
			{
				for (int k = 0; k < this.zoneGameModes[j].zone.Length; k++)
				{
					this.publicZoneGameModesLookup.Add(this.zoneGameModes[j].zone[k], new HashSet<GameModeType>(this.zoneGameModes[j].modes));
					for (int l = 0; l < this.zoneGameModes[j].modes.Length; l++)
					{
						if (!this.allModes.Contains(this.zoneGameModes[j].modes[l]))
						{
							this.allModes.Add(this.zoneGameModes[j].modes[l]);
						}
					}
					if (this.zoneGameModes[j].privateModes.Length != 0)
					{
						this.privateZoneGameModesLookup.Add(this.zoneGameModes[j].zone[k], new HashSet<GameModeType>(this.zoneGameModes[j].privateModes));
						for (int m = 0; m < this.zoneGameModes[j].privateModes.Length; m++)
						{
							if (!this.allModes.Contains(this.zoneGameModes[j].privateModes[m]))
							{
								this.allModes.Add(this.zoneGameModes[j].privateModes[m]);
							}
						}
					}
					else
					{
						this.privateZoneGameModesLookup.Add(this.zoneGameModes[j].zone[k], new HashSet<GameModeType>(this.zoneGameModes[j].modes));
					}
				}
			}
			this.modeNameLookup = new Dictionary<GameModeType, string>();
			for (int n = 0; n < this.gameModeNameOverrides.Length; n++)
			{
				this.modeNameLookup.Add(this.gameModeNameOverrides[n].mode, this.gameModeNameOverrides[n].displayName);
			}
			this.isNewLookup = new HashSet<GameModeType>(this.newThisUpdate);
			this.gameModeTypeCountdownsLookup = new Dictionary<GameModeType, CountdownTextDate>();
			for (int num = 0; num < this.gameModeTypeCountdowns.Length; num++)
			{
				this.gameModeTypeCountdownsLookup.Add(this.gameModeTypeCountdowns[num].mode, this.gameModeTypeCountdowns[num].countdownTextDate);
			}
		}

		// Token: 0x06005F7A RID: 24442 RVA: 0x001E4CA8 File Offset: 0x001E2EA8
		public HashSet<GameModeType> GetModesForZone(GTZone zone, bool isPrivate)
		{
			this.Init();
			if (isPrivate && this.privateZoneGameModesLookup.ContainsKey(zone))
			{
				return this.privateZoneGameModesLookup[zone];
			}
			if (this.publicZoneGameModesLookup.ContainsKey(zone))
			{
				return this.publicZoneGameModesLookup[zone];
			}
			return new HashSet<GameModeType>(this.defaultGameModes);
		}

		// Token: 0x06005F7B RID: 24443 RVA: 0x001E4D00 File Offset: 0x001E2F00
		public bool IsBigRoomMode(GameModeType gameModeType)
		{
			for (int i = 0; i < this.bigRoomGameModes.Length; i++)
			{
				if (this.bigRoomGameModes[i] == gameModeType)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005F7C RID: 24444 RVA: 0x001E4D2E File Offset: 0x001E2F2E
		internal string GetModeName(GameModeType mode)
		{
			this.Init();
			if (this.modeNameLookup.ContainsKey(mode))
			{
				return this.modeNameLookup[mode];
			}
			return mode.ToString().ToUpper();
		}

		// Token: 0x06005F7D RID: 24445 RVA: 0x001E4D63 File Offset: 0x001E2F63
		internal bool IsNew(GameModeType mode)
		{
			this.Init();
			return this.isNewLookup.Contains(mode);
		}

		// Token: 0x06005F7E RID: 24446 RVA: 0x001E4D77 File Offset: 0x001E2F77
		internal CountdownTextDate GetCountdown(GameModeType mode)
		{
			this.Init();
			if (this.gameModeTypeCountdownsLookup.ContainsKey(mode))
			{
				return this.gameModeTypeCountdownsLookup[mode];
			}
			return null;
		}

		// Token: 0x06005F7F RID: 24447 RVA: 0x001E4D9C File Offset: 0x001E2F9C
		internal GameModeType VerifyModeForZone(GTZone zone, GameModeType mode, bool isPrivate)
		{
			if (GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				zone = GTZone.customMaps;
			}
			if (zone == GTZone.none)
			{
				if (this.allModes.Contains(mode))
				{
					return mode;
				}
				return GameModeType.Casual;
			}
			else
			{
				bool flag = PlayerPrefFlags.Check(PlayerPrefFlags.Flag.GAME_MODE_SELECTOR_IS_SUPER);
				if (!flag)
				{
					if (mode == GameModeType.SuperCasual)
					{
						mode = GameModeType.Casual;
					}
					else if (mode == GameModeType.SuperInfect)
					{
						mode = GameModeType.Infection;
					}
				}
				HashSet<GameModeType> hashSet;
				if (isPrivate && this.privateZoneGameModesLookup.ContainsKey(zone))
				{
					hashSet = this.privateZoneGameModesLookup[zone];
				}
				else if (this.publicZoneGameModesLookup.ContainsKey(zone))
				{
					hashSet = this.publicZoneGameModesLookup[zone];
				}
				else
				{
					hashSet = new HashSet<GameModeType>(this.defaultGameModes);
				}
				if (hashSet.Contains(mode))
				{
					return mode;
				}
				GameModeType gameModeType = GameModeType.Casual;
				foreach (GameModeType gameModeType2 in hashSet)
				{
					if (flag || (gameModeType2 != GameModeType.SuperCasual && gameModeType2 != GameModeType.SuperInfect))
					{
						return gameModeType2;
					}
				}
				return gameModeType;
			}
		}

		// Token: 0x04006DD3 RID: 28115
		[SerializeField]
		[TextArea(4, 40)]
		private string notes;

		// Token: 0x04006DD4 RID: 28116
		[SerializeField]
		private GameModeNameOverrides[] gameModeNameOverrides;

		// Token: 0x04006DD5 RID: 28117
		[SerializeField]
		private GameModeType[] defaultGameModes;

		// Token: 0x04006DD6 RID: 28118
		[SerializeField]
		private GameModeType[] bigRoomGameModes;

		// Token: 0x04006DD7 RID: 28119
		[SerializeField]
		private ZoneGameModes[] zoneGameModes;

		// Token: 0x04006DD8 RID: 28120
		[SerializeField]
		private GameModeTypeCountdown[] gameModeTypeCountdowns;

		// Token: 0x04006DD9 RID: 28121
		[SerializeField]
		private GameModeType[] newThisUpdate;

		// Token: 0x04006DDA RID: 28122
		private Dictionary<GTZone, HashSet<GameModeType>> bigRoomZoneGameModesLookup;

		// Token: 0x04006DDB RID: 28123
		private Dictionary<GTZone, HashSet<GameModeType>> publicZoneGameModesLookup;

		// Token: 0x04006DDC RID: 28124
		private Dictionary<GTZone, HashSet<GameModeType>> privateZoneGameModesLookup;

		// Token: 0x04006DDD RID: 28125
		private Dictionary<GameModeType, string> modeNameLookup;

		// Token: 0x04006DDE RID: 28126
		private HashSet<GameModeType> isNewLookup;

		// Token: 0x04006DDF RID: 28127
		private Dictionary<GameModeType, CountdownTextDate> gameModeTypeCountdownsLookup;

		// Token: 0x04006DE0 RID: 28128
		private HashSet<GameModeType> allModes;
	}
}
