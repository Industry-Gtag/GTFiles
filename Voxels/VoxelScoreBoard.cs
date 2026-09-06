using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Fusion;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Voxels
{
	// Token: 0x020013C7 RID: 5063
	[NetworkBehaviourWeaved(0)]
	public class VoxelScoreBoard : NetworkComponent
	{
		// Token: 0x06007EA4 RID: 32420 RVA: 0x00297EAC File Offset: 0x002960AC
		private new void Start()
		{
			this._materialNames = new string[this.materialSet.Materials.Length];
			for (int i = 0; i < this._materialNames.Length; i++)
			{
				this._materialNames[i] = this.materialSet.Materials[i].name;
			}
			this.UpdateDisplay();
		}

		// Token: 0x06007EA5 RID: 32421 RVA: 0x00297F08 File Offset: 0x00296108
		private void UpdateDisplay()
		{
			if (this._entries.Count == 0)
			{
				this.text.text = "";
				return;
			}
			this.sb.Clear();
			int num = this.lineLength - (this.nameWidth + this._materialNames.Length * this.columnWidth);
			this.<UpdateDisplay>g__AddText|11_0("    NAME", this.nameWidth + num);
			foreach (string text in this._materialNames)
			{
				this.<UpdateDisplay>g__AddText|11_0(text, this.columnWidth);
			}
			this.sb.Append("\n");
			foreach (VoxelScoreBoard.VoxelScoreEntry voxelScoreEntry in this._entries)
			{
				if (Utils.PlayerInRoom(voxelScoreEntry.actorNumber))
				{
					NetPlayer netPlayer = NetPlayer.Get(voxelScoreEntry.actorNumber);
					string text2 = ((netPlayer != null) ? netPlayer.NickName : null) ?? "UNKNOWN";
					this.<UpdateDisplay>g__AddText|11_0(text2, this.nameWidth + num - this.columnOffset);
					this.<UpdateDisplay>g__AddTextRight|11_1((voxelScoreEntry.amount1 / 100).ToString(), this.columnWidth);
					if (this._materialNames.Length > 1)
					{
						this.<UpdateDisplay>g__AddTextRight|11_1((voxelScoreEntry.amount2 / 100).ToString(), this.columnWidth);
					}
					if (this._materialNames.Length > 2)
					{
						this.<UpdateDisplay>g__AddTextRight|11_1((voxelScoreEntry.amount3 / 100).ToString(), this.columnWidth);
					}
					this.sb.Append("\n");
				}
			}
			this.text.alignment = TextAlignmentOptions.TopLeft;
			this.text.text = this.sb.ToString();
		}

		// Token: 0x06007EA6 RID: 32422 RVA: 0x00002C2D File Offset: 0x00000E2D
		public override void WriteDataFusion()
		{
		}

		// Token: 0x06007EA7 RID: 32423 RVA: 0x00002C2D File Offset: 0x00000E2D
		public override void ReadDataFusion()
		{
		}

		// Token: 0x06007EA8 RID: 32424 RVA: 0x002980E0 File Offset: 0x002962E0
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			stream.SendNext(this._entries.Count);
			foreach (VoxelScoreBoard.VoxelScoreEntry voxelScoreEntry in this._entries)
			{
				stream.SendNext(voxelScoreEntry.actorNumber);
				stream.SendNext(voxelScoreEntry.amount1);
				stream.SendNext(voxelScoreEntry.amount2);
				stream.SendNext(voxelScoreEntry.amount3);
			}
		}

		// Token: 0x06007EA9 RID: 32425 RVA: 0x00298188 File Offset: 0x00296388
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient && !info.Sender.IsLocal)
			{
				return;
			}
			int num = (int)stream.ReceiveNext();
			if (num < 0 || num > 20)
			{
				return;
			}
			if (this._entries.Count > num)
			{
				this._entries.RemoveRange(num, this._entries.Count - num);
			}
			for (int i = 0; i < num; i++)
			{
				VoxelScoreBoard.VoxelScoreEntry voxelScoreEntry = new VoxelScoreBoard.VoxelScoreEntry
				{
					actorNumber = (int)stream.ReceiveNext(),
					amount1 = (int)stream.ReceiveNext(),
					amount2 = (int)stream.ReceiveNext(),
					amount3 = (int)stream.ReceiveNext()
				};
				if (i < this._entries.Count)
				{
					this._entries[i] = voxelScoreEntry;
				}
				else
				{
					this._entries.Add(voxelScoreEntry);
				}
			}
			if (this._materialNames != null)
			{
				this.UpdateDisplay();
			}
		}

		// Token: 0x06007EAA RID: 32426 RVA: 0x00298288 File Offset: 0x00296488
		private new void OnEnable()
		{
			NetworkBehaviourUtils.InternalOnEnable(this);
			VoxelEvents.OnResourcesMinedAuthority += this.OnResourcesMined;
			RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
			RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
			RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerEnteredRoom);
			RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
		}

		// Token: 0x06007EAB RID: 32427 RVA: 0x00298318 File Offset: 0x00296518
		private new void OnDisable()
		{
			NetworkBehaviourUtils.InternalOnDisable(this);
			VoxelEvents.OnResourcesMinedAuthority -= this.OnResourcesMined;
			RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
			RoomSystem.LeftRoomEvent -= new Action(this.OnLeftRoom);
			RoomSystem.PlayerJoinedEvent -= new Action<NetPlayer>(this.OnPlayerEnteredRoom);
			RoomSystem.PlayerLeftEvent -= new Action<NetPlayer>(this.OnPlayerLeftRoom);
		}

		// Token: 0x06007EAC RID: 32428 RVA: 0x002983A8 File Offset: 0x002965A8
		private void OnJoinedRoom()
		{
			if (base.IsLocallyOwned)
			{
				int scoreLine = this.GetScoreLine(PhotonUtils.LocalNetPlayer);
				for (int i = 0; i < this._entries.Count; i++)
				{
					if (i != scoreLine)
					{
						this._entries.RemoveAt(i--);
					}
				}
				foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
				{
					this.GetScoreLine(netPlayer);
				}
			}
			this.UpdateDisplay();
		}

		// Token: 0x06007EAD RID: 32429 RVA: 0x00298420 File Offset: 0x00296620
		private void OnLeftRoom()
		{
			this._entries.Clear();
			this.UpdateDisplay();
		}

		// Token: 0x06007EAE RID: 32430 RVA: 0x00298433 File Offset: 0x00296633
		private void OnPlayerEnteredRoom(NetPlayer player)
		{
			if (base.IsLocallyOwned)
			{
				this.GetScoreLine(player);
			}
			this.UpdateDisplay();
		}

		// Token: 0x06007EAF RID: 32431 RVA: 0x0029844C File Offset: 0x0029664C
		private void OnPlayerLeftRoom(NetPlayer player)
		{
			if (base.IsLocallyOwned)
			{
				for (int i = 0; i < this._entries.Count; i++)
				{
					if (this._entries[i].actorNumber == player.ActorNumber)
					{
						this._entries.RemoveAt(i);
						return;
					}
				}
			}
			this.UpdateDisplay();
		}

		// Token: 0x06007EB0 RID: 32432 RVA: 0x002984A4 File Offset: 0x002966A4
		private int GetScoreLine(NetPlayer player)
		{
			for (int i = 0; i < this._entries.Count; i++)
			{
				if (this._entries[i].actorNumber == player.ActorNumber)
				{
					return i;
				}
			}
			VoxelScoreBoard.VoxelScoreEntry voxelScoreEntry = new VoxelScoreBoard.VoxelScoreEntry(player.ActorNumber);
			this._entries.Add(voxelScoreEntry);
			return this._entries.Count - 1;
		}

		// Token: 0x06007EB1 RID: 32433 RVA: 0x00298508 File Offset: 0x00296708
		private void OnResourcesMined(NetPlayer player, VoxelWorld world, int[] resources)
		{
			if (world.MaterialSet != this.materialSet)
			{
				return;
			}
			int scoreLine = this.GetScoreLine(player);
			VoxelScoreBoard.VoxelScoreEntry voxelScoreEntry = this._entries[scoreLine];
			voxelScoreEntry.Add(resources);
			this._entries[scoreLine] = voxelScoreEntry;
			this.UpdateDisplay();
		}

		// Token: 0x06007EB3 RID: 32435 RVA: 0x002985AC File Offset: 0x002967AC
		[CompilerGenerated]
		private void <UpdateDisplay>g__AddText|11_0(string text, int length)
		{
			if (text.Length > length)
			{
				this.sb.Append(text.Substring(0, length));
				return;
			}
			this.sb.Append(text);
			for (int i = 0; i < length - text.Length; i++)
			{
				this.sb.Append(" ");
			}
		}

		// Token: 0x06007EB4 RID: 32436 RVA: 0x00298608 File Offset: 0x00296808
		[CompilerGenerated]
		private void <UpdateDisplay>g__AddTextRight|11_1(string text, int length)
		{
			if (text.Length > length)
			{
				this.sb.Append(text.Substring(0, length));
				return;
			}
			for (int i = 0; i < length - text.Length; i++)
			{
				this.sb.Append(" ");
			}
			this.sb.Append(text);
		}

		// Token: 0x06007EB5 RID: 32437 RVA: 0x00002E6F File Offset: 0x0000106F
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
		}

		// Token: 0x06007EB6 RID: 32438 RVA: 0x00002E7B File Offset: 0x0000107B
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
		}

		// Token: 0x04009129 RID: 37161
		[SerializeField]
		private VoxelMaterialSet materialSet;

		// Token: 0x0400912A RID: 37162
		[SerializeField]
		private TMP_Text text;

		// Token: 0x0400912B RID: 37163
		private string[] _materialNames;

		// Token: 0x0400912C RID: 37164
		private List<VoxelScoreBoard.VoxelScoreEntry> _entries = new List<VoxelScoreBoard.VoxelScoreEntry>();

		// Token: 0x0400912D RID: 37165
		private StringBuilder sb = new StringBuilder(220);

		// Token: 0x0400912E RID: 37166
		public int lineLength = 50;

		// Token: 0x0400912F RID: 37167
		public int nameWidth = 15;

		// Token: 0x04009130 RID: 37168
		public int columnWidth = 10;

		// Token: 0x04009131 RID: 37169
		public int columnOffset = -4;

		// Token: 0x020013C8 RID: 5064
		[Serializable]
		public struct VoxelScoreEntry
		{
			// Token: 0x06007EB7 RID: 32439 RVA: 0x00298664 File Offset: 0x00296864
			public VoxelScoreEntry(int actorNumber)
			{
				this.actorNumber = actorNumber;
				this.amount1 = 0;
				this.amount2 = 0;
				this.amount3 = 0;
			}

			// Token: 0x06007EB8 RID: 32440 RVA: 0x00298682 File Offset: 0x00296882
			public void Add(int[] resources)
			{
				this.amount1 += resources[0];
				if (resources.Length == 1)
				{
					return;
				}
				this.amount2 += resources[1];
				if (resources.Length == 2)
				{
					return;
				}
				this.amount3 += resources[2];
			}

			// Token: 0x04009132 RID: 37170
			public int actorNumber;

			// Token: 0x04009133 RID: 37171
			public int amount1;

			// Token: 0x04009134 RID: 37172
			public int amount2;

			// Token: 0x04009135 RID: 37173
			public int amount3;
		}
	}
}
