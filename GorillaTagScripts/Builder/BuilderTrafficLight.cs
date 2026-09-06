using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x0200104E RID: 4174
	public class BuilderTrafficLight : MonoBehaviour, IBuilderPieceComponent
	{
		// Token: 0x0600682C RID: 26668 RVA: 0x00218A4B File Offset: 0x00216C4B
		private void Start()
		{
			this.materialProps = new MaterialPropertyBlock();
		}

		// Token: 0x0600682D RID: 26669 RVA: 0x00218A58 File Offset: 0x00216C58
		private void SetState(BuilderTrafficLight.LightState state)
		{
			this.lightState = state;
			if (this.materialProps == null)
			{
				this.materialProps = new MaterialPropertyBlock();
			}
			Color color = this.yellowOff;
			Color color2 = this.redOff;
			Color color3 = this.greenOff;
			switch (state)
			{
			case BuilderTrafficLight.LightState.Red:
				color2 = this.redOn;
				break;
			case BuilderTrafficLight.LightState.Yellow:
				color = this.yellowOn;
				break;
			case BuilderTrafficLight.LightState.Green:
				color3 = this.greenOn;
				break;
			}
			this.redLight.GetPropertyBlock(this.materialProps);
			this.materialProps.SetColor(ShaderProps._BaseColor, color2);
			this.redLight.SetPropertyBlock(this.materialProps);
			this.materialProps.SetColor(ShaderProps._BaseColor, color);
			this.yellowLight.SetPropertyBlock(this.materialProps);
			this.materialProps.SetColor(ShaderProps._BaseColor, color3);
			this.greenLight.SetPropertyBlock(this.materialProps);
		}

		// Token: 0x0600682E RID: 26670 RVA: 0x00218B38 File Offset: 0x00216D38
		private void Update()
		{
			if (this.piece == null || this.piece.state == BuilderPiece.State.AttachedAndPlaced)
			{
				float num = Time.time;
				if (PhotonNetwork.InRoom)
				{
					uint num2 = (uint)PhotonNetwork.ServerTimestamp;
					if (this.piece != null)
					{
						num2 = (uint)(PhotonNetwork.ServerTimestamp - this.piece.activatedTimeStamp);
					}
					num = num2 / 1000f;
				}
				float num3 = num % this.cycleDuration / this.cycleDuration;
				num3 = (num3 + this.startPercentageOffset) % 1f;
				int num4 = (int)this.stateCurve.Evaluate(num3);
				if (num4 != (int)this.lightState)
				{
					this.SetState((BuilderTrafficLight.LightState)num4);
				}
			}
		}

		// Token: 0x0600682F RID: 26671 RVA: 0x00218BDC File Offset: 0x00216DDC
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.SetState(BuilderTrafficLight.LightState.Off);
		}

		// Token: 0x06006830 RID: 26672 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPieceDestroy()
		{
		}

		// Token: 0x06006831 RID: 26673 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06006832 RID: 26674 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPieceActivate()
		{
		}

		// Token: 0x06006833 RID: 26675 RVA: 0x00218BDC File Offset: 0x00216DDC
		public void OnPieceDeactivate()
		{
			this.SetState(BuilderTrafficLight.LightState.Off);
		}

		// Token: 0x04007765 RID: 30565
		[SerializeField]
		private BuilderPiece piece;

		// Token: 0x04007766 RID: 30566
		[SerializeField]
		private MeshRenderer redLight;

		// Token: 0x04007767 RID: 30567
		[SerializeField]
		private MeshRenderer yellowLight;

		// Token: 0x04007768 RID: 30568
		[SerializeField]
		private MeshRenderer greenLight;

		// Token: 0x04007769 RID: 30569
		[SerializeField]
		private float cycleDuration = 10f;

		// Token: 0x0400776A RID: 30570
		[SerializeField]
		private float startPercentageOffset = 0.5f;

		// Token: 0x0400776B RID: 30571
		[SerializeField]
		private Color redOn = Color.red;

		// Token: 0x0400776C RID: 30572
		[SerializeField]
		private Color redOff = Color.gray;

		// Token: 0x0400776D RID: 30573
		[SerializeField]
		private Color yellowOn = Color.yellow;

		// Token: 0x0400776E RID: 30574
		[SerializeField]
		private Color yellowOff = Color.gray;

		// Token: 0x0400776F RID: 30575
		[SerializeField]
		private Color greenOn = Color.green;

		// Token: 0x04007770 RID: 30576
		[SerializeField]
		private Color greenOff = Color.gray;

		// Token: 0x04007771 RID: 30577
		private MaterialPropertyBlock materialProps;

		// Token: 0x04007772 RID: 30578
		[SerializeField]
		private AnimationCurve stateCurve;

		// Token: 0x04007773 RID: 30579
		private BuilderTrafficLight.LightState lightState = BuilderTrafficLight.LightState.Off;

		// Token: 0x0200104F RID: 4175
		private enum LightState
		{
			// Token: 0x04007775 RID: 30581
			Red,
			// Token: 0x04007776 RID: 30582
			Yellow,
			// Token: 0x04007777 RID: 30583
			Green,
			// Token: 0x04007778 RID: 30584
			Off
		}
	}
}
