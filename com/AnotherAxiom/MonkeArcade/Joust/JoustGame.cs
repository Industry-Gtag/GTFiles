using System;
using UnityEngine;

namespace com.AnotherAxiom.MonkeArcade.Joust
{
	// Token: 0x02001175 RID: 4469
	public class JoustGame : ArcadeGame
	{
		// Token: 0x06007007 RID: 28679 RVA: 0x0024262A File Offset: 0x0024082A
		public override byte[] GetNetworkState()
		{
			return new byte[0];
		}

		// Token: 0x06007008 RID: 28680 RVA: 0x00002C2D File Offset: 0x00000E2D
		public override void SetNetworkState(byte[] obj)
		{
		}

		// Token: 0x06007009 RID: 28681 RVA: 0x00242632 File Offset: 0x00240832
		protected override void ButtonDown(int player, ArcadeButtons button)
		{
			if (button != ArcadeButtons.GRAB)
			{
				if (button == ArcadeButtons.TRIGGER)
				{
					this.joustPlayers[player].Flap();
					return;
				}
			}
			else
			{
				this.joustPlayers[player].gameObject.SetActive(true);
			}
		}

		// Token: 0x0600700A RID: 28682 RVA: 0x00242661 File Offset: 0x00240861
		protected override void ButtonUp(int player, ArcadeButtons button)
		{
			if (button == ArcadeButtons.GRAB)
			{
				this.joustPlayers[player].gameObject.SetActive(false);
			}
		}

		// Token: 0x0600700B RID: 28683 RVA: 0x0024267C File Offset: 0x0024087C
		private void Start()
		{
			for (int i = 0; i < this.joustPlayers.Length; i++)
			{
				this.joustPlayers[i].gameObject.SetActive(false);
			}
		}

		// Token: 0x0600700C RID: 28684 RVA: 0x002426B0 File Offset: 0x002408B0
		private void Update()
		{
			for (int i = 0; i < this.joustPlayers.Length; i++)
			{
				if (this.joustPlayers[i].gameObject.activeInHierarchy)
				{
					int num = (base.getButtonState(i, ArcadeButtons.LEFT) ? (-1) : 0) + (base.getButtonState(i, ArcadeButtons.RIGHT) ? 1 : 0);
					this.joustPlayers[i].HorizontalSpeed = Mathf.Clamp(this.joustPlayers[i].HorizontalSpeed + (float)num * Time.deltaTime, -1f, 1f);
				}
			}
		}

		// Token: 0x0600700D RID: 28685 RVA: 0x00002C2D File Offset: 0x00000E2D
		public override void OnTimeout()
		{
		}

		// Token: 0x04008014 RID: 32788
		[SerializeField]
		private JoustPlayer[] joustPlayers;
	}
}
