using System;
using GorillaExtensions;
using GorillaGameModes;
using GorillaNetworking;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000FDB RID: 4059
	public class VirtualStumpModeSelectButton : ModeSelectButton
	{
		// Token: 0x06006505 RID: 25861 RVA: 0x00208560 File Offset: 0x00206760
		public override void ButtonActivationWithHand(bool isLeftHand)
		{
			if (this.warningScreen.ShouldShowWarning)
			{
				this.warningScreen.Show();
			}
			else
			{
				GorillaComputer.instance.SetGameModeWithoutButton(this.gameMode);
			}
			if (GorillaComputer.instance.IsPlayerInVirtualStump() && RoomSystem.JoinedRoom && NetworkSystem.Instance.LocalPlayer.IsMasterClient && NetworkSystem.Instance.SessionIsPrivate)
			{
				if (GameMode.ActiveGameMode.IsNull())
				{
					GameMode.ChangeGameMode(this.gameMode);
					return;
				}
				if (GameMode.ActiveGameMode.GameType().ToString().ToLower() != this.gameMode.ToLower())
				{
					GameMode.ChangeGameMode(this.gameMode);
				}
			}
		}
	}
}
