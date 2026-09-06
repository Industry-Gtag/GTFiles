using System;
using UnityEngine;

namespace com.AnotherAxiom.SpaceFight
{
	// Token: 0x0200116F RID: 4463
	public class SpaceFight : ArcadeGame
	{
		// Token: 0x06006FEA RID: 28650 RVA: 0x00241240 File Offset: 0x0023F440
		private void Update()
		{
			for (int i = 0; i < 2; i++)
			{
				if (base.getButtonState(i, ArcadeButtons.UP))
				{
					this.move(this.player[i], 0.15f);
					this.clamp(this.player[i]);
				}
				if (base.getButtonState(i, ArcadeButtons.RIGHT))
				{
					this.turn(this.player[i], true);
				}
				if (base.getButtonState(i, ArcadeButtons.LEFT))
				{
					this.turn(this.player[i], false);
				}
				if (this.projectilesFired[i])
				{
					this.move(this.projectile[i], 0.5f);
					if (Vector2.Distance(this.player[1 - i].localPosition, this.projectile[i].localPosition) < 0.25f)
					{
						base.PlaySound(1, 2);
						this.player[1 - i].Rotate(0f, 0f, 180f);
						this.projectilesFired[i] = false;
					}
					if (Mathf.Abs(this.projectile[i].localPosition.x) > this.tableSize.x || Mathf.Abs(this.projectile[i].localPosition.y) > this.tableSize.y)
					{
						this.projectilesFired[i] = false;
					}
				}
				if (!this.projectilesFired[i])
				{
					this.projectile[i].position = this.player[i].position;
					this.projectile[i].rotation = this.player[i].rotation;
				}
			}
		}

		// Token: 0x06006FEB RID: 28651 RVA: 0x002413D0 File Offset: 0x0023F5D0
		private void clamp(Transform tr)
		{
			tr.localPosition = new Vector2(Mathf.Clamp(tr.localPosition.x, -this.tableSize.x, this.tableSize.x), Mathf.Clamp(tr.localPosition.y, -this.tableSize.y, this.tableSize.y));
		}

		// Token: 0x06006FEC RID: 28652 RVA: 0x0024143B File Offset: 0x0023F63B
		protected override void ButtonDown(int player, ArcadeButtons button)
		{
			if (button == ArcadeButtons.TRIGGER)
			{
				if (!this.projectilesFired[player])
				{
					base.PlaySound(0, 3);
				}
				this.projectilesFired[player] = true;
			}
		}

		// Token: 0x06006FED RID: 28653 RVA: 0x00241460 File Offset: 0x0023F660
		private void move(Transform p, float speed)
		{
			p.Translate(p.up * Time.deltaTime * speed, Space.World);
		}

		// Token: 0x06006FEE RID: 28654 RVA: 0x0024147F File Offset: 0x0023F67F
		private void turn(Transform p, bool cw)
		{
			p.Rotate(0f, 0f, (float)(cw ? 180 : (-180)) * Time.deltaTime);
		}

		// Token: 0x06006FEF RID: 28655 RVA: 0x002414A8 File Offset: 0x0023F6A8
		public override byte[] GetNetworkState()
		{
			this.netStateCur.P1LocX = this.player[0].localPosition.x;
			this.netStateCur.P1LocY = this.player[0].localPosition.y;
			this.netStateCur.P1Rot = this.player[0].localRotation.eulerAngles.z;
			this.netStateCur.P2LocX = this.player[1].localPosition.x;
			this.netStateCur.P2LocY = this.player[1].localPosition.y;
			this.netStateCur.P2Rot = this.player[1].localRotation.eulerAngles.z;
			this.netStateCur.P1PrLocX = this.projectile[0].localPosition.x;
			this.netStateCur.P1PrLocY = this.projectile[0].localPosition.y;
			this.netStateCur.P2PrLocX = this.projectile[1].localPosition.x;
			this.netStateCur.P2PrLocY = this.projectile[1].localPosition.y;
			if (!this.netStateCur.Equals(this.netStateLast))
			{
				this.netStateLast = this.netStateCur;
				base.SwapNetStateBuffersAndStreams();
				ArcadeGame.WrapNetState(this.netStateLast, this.netStateMemStream);
			}
			return this.netStateBuffer;
		}

		// Token: 0x06006FF0 RID: 28656 RVA: 0x00241628 File Offset: 0x0023F828
		public override void SetNetworkState(byte[] b)
		{
			SpaceFight.SpaceFlightNetState spaceFlightNetState = (SpaceFight.SpaceFlightNetState)ArcadeGame.UnwrapNetState(b);
			this.player[0].localPosition = new Vector2(spaceFlightNetState.P1LocX, spaceFlightNetState.P1LocY);
			this.player[0].localRotation = Quaternion.Euler(0f, 0f, spaceFlightNetState.P1Rot);
			this.player[1].localPosition = new Vector2(spaceFlightNetState.P2LocX, spaceFlightNetState.P2LocY);
			this.player[1].localRotation = Quaternion.Euler(0f, 0f, spaceFlightNetState.P2Rot);
			this.projectile[0].localPosition = new Vector2(spaceFlightNetState.P1PrLocX, spaceFlightNetState.P1PrLocY);
			this.projectile[1].localPosition = new Vector2(spaceFlightNetState.P2PrLocX, spaceFlightNetState.P2PrLocY);
		}

		// Token: 0x06006FF1 RID: 28657 RVA: 0x00002C2D File Offset: 0x00000E2D
		protected override void ButtonUp(int player, ArcadeButtons button)
		{
		}

		// Token: 0x06006FF2 RID: 28658 RVA: 0x00002C2D File Offset: 0x00000E2D
		public override void OnTimeout()
		{
		}

		// Token: 0x04007FCF RID: 32719
		[SerializeField]
		private Transform[] player;

		// Token: 0x04007FD0 RID: 32720
		[SerializeField]
		private Transform[] projectile;

		// Token: 0x04007FD1 RID: 32721
		[SerializeField]
		private Vector2 tableSize;

		// Token: 0x04007FD2 RID: 32722
		private bool[] projectilesFired = new bool[2];

		// Token: 0x04007FD3 RID: 32723
		private SpaceFight.SpaceFlightNetState netStateLast;

		// Token: 0x04007FD4 RID: 32724
		private SpaceFight.SpaceFlightNetState netStateCur;

		// Token: 0x02001170 RID: 4464
		[Serializable]
		private struct SpaceFlightNetState : IEquatable<SpaceFight.SpaceFlightNetState>
		{
			// Token: 0x06006FF4 RID: 28660 RVA: 0x00241728 File Offset: 0x0023F928
			public bool Equals(SpaceFight.SpaceFlightNetState other)
			{
				return this.P1LocX.Approx(other.P1LocX, 1E-06f) && this.P1LocY.Approx(other.P1LocY, 1E-06f) && this.P1Rot.Approx(other.P1Rot, 1E-06f) && this.P2LocX.Approx(other.P2LocX, 1E-06f) && this.P2LocY.Approx(other.P2LocY, 1E-06f) && this.P1Rot.Approx(other.P1Rot, 1E-06f) && this.P1PrLocX.Approx(other.P1PrLocX, 1E-06f) && this.P1PrLocY.Approx(other.P1PrLocY, 1E-06f) && this.P2PrLocX.Approx(other.P2PrLocX, 1E-06f) && this.P2PrLocY.Approx(other.P2PrLocY, 1E-06f);
			}

			// Token: 0x04007FD5 RID: 32725
			public float P1LocX;

			// Token: 0x04007FD6 RID: 32726
			public float P1LocY;

			// Token: 0x04007FD7 RID: 32727
			public float P1Rot;

			// Token: 0x04007FD8 RID: 32728
			public float P2LocX;

			// Token: 0x04007FD9 RID: 32729
			public float P2LocY;

			// Token: 0x04007FDA RID: 32730
			public float P2Rot;

			// Token: 0x04007FDB RID: 32731
			public float P1PrLocX;

			// Token: 0x04007FDC RID: 32732
			public float P1PrLocY;

			// Token: 0x04007FDD RID: 32733
			public float P2PrLocX;

			// Token: 0x04007FDE RID: 32734
			public float P2PrLocY;
		}
	}
}
