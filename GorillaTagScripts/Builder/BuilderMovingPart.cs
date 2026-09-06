using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x0200102D RID: 4141
	public class BuilderMovingPart : MonoBehaviour
	{
		// Token: 0x06006701 RID: 26369 RVA: 0x0021163C File Offset: 0x0020F83C
		private void Awake()
		{
			foreach (BuilderAttachGridPlane builderAttachGridPlane in this.myGridPlanes)
			{
				builderAttachGridPlane.movesOnPlace = true;
				builderAttachGridPlane.movingPart = this;
			}
			this.initLocalPos = base.transform.localPosition;
			this.initLocalRotation = base.transform.localRotation;
		}

		// Token: 0x06006702 RID: 26370 RVA: 0x00211690 File Offset: 0x0020F890
		private long NetworkTimeMs()
		{
			if (PhotonNetwork.InRoom)
			{
				return (long)((ulong)(PhotonNetwork.ServerTimestamp - this.myPiece.activatedTimeStamp + (int)this.startPercentageCycleOffset + int.MinValue));
			}
			return (long)(Time.time * 1000f);
		}

		// Token: 0x06006703 RID: 26371 RVA: 0x002116C5 File Offset: 0x0020F8C5
		private long CycleLengthMs()
		{
			return (long)(this.cycleDuration * 1000f);
		}

		// Token: 0x06006704 RID: 26372 RVA: 0x002116D4 File Offset: 0x0020F8D4
		public double PlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.CycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x06006705 RID: 26373 RVA: 0x002116FF File Offset: 0x0020F8FF
		public int CycleCount()
		{
			return (int)(this.NetworkTimeMs() / this.CycleLengthMs());
		}

		// Token: 0x06006706 RID: 26374 RVA: 0x0021170F File Offset: 0x0020F90F
		public float CycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.PlatformTime() / (double)this.cycleDuration), 0f, 1f);
		}

		// Token: 0x06006707 RID: 26375 RVA: 0x0021172F File Offset: 0x0020F92F
		public bool IsEvenCycle()
		{
			return this.CycleCount() % 2 == 0;
		}

		// Token: 0x06006708 RID: 26376 RVA: 0x0021173C File Offset: 0x0020F93C
		public void ActivateAtNode(byte node, int timestamp)
		{
			float num = (float)node;
			bool flag = (int)node > BuilderMovingPart.NUM_PAUSE_NODES;
			if (flag)
			{
				num -= (float)BuilderMovingPart.NUM_PAUSE_NODES;
			}
			num /= (float)BuilderMovingPart.NUM_PAUSE_NODES;
			num = Mathf.Clamp(num, 0f, 1f);
			if (num >= this.startPercentage)
			{
				int num2 = (int)((num - this.startPercentage) * (float)this.CycleLengthMs());
				int num3 = timestamp - num2;
				if (flag)
				{
					num3 -= (int)this.CycleLengthMs();
				}
				this.myPiece.activatedTimeStamp = num3;
			}
			else
			{
				int num4 = (int)((num + 2f - this.startPercentage) * (float)this.CycleLengthMs());
				if (flag)
				{
					num4 -= (int)this.CycleLengthMs();
				}
				this.myPiece.activatedTimeStamp = timestamp - num4;
			}
			this.SetMoving(true);
		}

		// Token: 0x06006709 RID: 26377 RVA: 0x002117F4 File Offset: 0x0020F9F4
		public int GetTimeOffsetMS()
		{
			int num = PhotonNetwork.ServerTimestamp - this.myPiece.activatedTimeStamp;
			uint num2 = (uint)(this.CycleLengthMs() * 2L);
			return num % (int)num2;
		}

		// Token: 0x0600670A RID: 26378 RVA: 0x00211820 File Offset: 0x0020FA20
		public byte GetNearestNode()
		{
			int num = Mathf.RoundToInt(this.currT * (float)BuilderMovingPart.NUM_PAUSE_NODES);
			if (!this.IsEvenCycle())
			{
				num += BuilderMovingPart.NUM_PAUSE_NODES;
			}
			return (byte)num;
		}

		// Token: 0x0600670B RID: 26379 RVA: 0x00211852 File Offset: 0x0020FA52
		public byte GetStartNode()
		{
			return (byte)Mathf.RoundToInt(this.startPercentage * (float)BuilderMovingPart.NUM_PAUSE_NODES);
		}

		// Token: 0x0600670C RID: 26380 RVA: 0x00211868 File Offset: 0x0020FA68
		public void PauseMovement(byte node)
		{
			this.SetMoving(false);
			bool flag = (int)node > BuilderMovingPart.NUM_PAUSE_NODES;
			float num = (float)node;
			if (flag)
			{
				num -= (float)BuilderMovingPart.NUM_PAUSE_NODES;
			}
			num /= (float)BuilderMovingPart.NUM_PAUSE_NODES;
			num = Mathf.Clamp(num, 0f, 1f);
			if (this.reverseDirOnCycle)
			{
				num = (flag ? (1f - num) : num);
			}
			if (this.reverseDir)
			{
				num = 1f - num;
			}
			BuilderMovingPart.BuilderMovingPartType builderMovingPartType = this.moveType;
			if (builderMovingPartType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				base.transform.localPosition = this.UpdatePointToPoint(num);
				return;
			}
			if (builderMovingPartType != BuilderMovingPart.BuilderMovingPartType.Rotation)
			{
				return;
			}
			this.UpdateRotation(num);
		}

		// Token: 0x0600670D RID: 26381 RVA: 0x00211900 File Offset: 0x0020FB00
		public void SetMoving(bool isMoving)
		{
			this.isMoving = isMoving;
			BuilderAttachGridPlane[] array = this.myGridPlanes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].isMoving = isMoving;
			}
			if (!isMoving)
			{
				this.ResetMovingGrid();
			}
		}

		// Token: 0x0600670E RID: 26382 RVA: 0x0021193C File Offset: 0x0020FB3C
		public void InitMovingGrid()
		{
			if (this.moveType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				this.distance = Vector3.Distance(this.endXf.position, this.startXf.position);
				float num = this.distance / (this.velocity * this.myPiece.GetScale());
				this.cycleDuration = num + this.cycleDelay;
				float num2 = this.cycleDelay / this.cycleDuration;
				Vector2 vector = new Vector2(num2 / 2f, 0f);
				Vector2 vector2 = new Vector2(1f - num2 / 2f, 1f);
				float num3 = (vector2.y - vector.y) / (vector2.x - vector.x);
				this.lerpAlpha = new AnimationCurve(new Keyframe[]
				{
					new Keyframe(num2 / 2f, 0f, 0f, num3),
					new Keyframe(1f - num2 / 2f, 1f, num3, 0f)
				});
			}
			else
			{
				this.cycleDuration = 1f / this.velocity;
			}
			this.currT = this.startPercentage;
			uint num4 = (uint)(this.cycleDuration * 1000f);
			uint num5 = 2147483648U % num4;
			uint num6 = (uint)(this.startPercentage * num4);
			if (num6 >= num5)
			{
				this.startPercentageCycleOffset = num6 - num5;
				return;
			}
			this.startPercentageCycleOffset = num6 + num4 + num4 - num5;
		}

		// Token: 0x0600670F RID: 26383 RVA: 0x00211AB0 File Offset: 0x0020FCB0
		public void UpdateMovingGrid()
		{
			this.Progress();
			BuilderMovingPart.BuilderMovingPartType builderMovingPartType = this.moveType;
			if (builderMovingPartType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				base.transform.localPosition = this.UpdatePointToPoint(this.percent);
				return;
			}
			if (builderMovingPartType != BuilderMovingPart.BuilderMovingPartType.Rotation)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.UpdateRotation(this.percent);
		}

		// Token: 0x06006710 RID: 26384 RVA: 0x00211B00 File Offset: 0x0020FD00
		private Vector3 UpdatePointToPoint(float perc)
		{
			float num = this.lerpAlpha.Evaluate(perc);
			return Vector3.Lerp(this.startXf.localPosition, this.endXf.localPosition, num);
		}

		// Token: 0x06006711 RID: 26385 RVA: 0x00211B38 File Offset: 0x0020FD38
		private void UpdateRotation(float perc)
		{
			Quaternion quaternion = Quaternion.AngleAxis(perc * 360f, Vector3.up);
			base.transform.localRotation = quaternion;
		}

		// Token: 0x06006712 RID: 26386 RVA: 0x00211B63 File Offset: 0x0020FD63
		private void ResetMovingGrid()
		{
			base.transform.SetLocalPositionAndRotation(this.initLocalPos, this.initLocalRotation);
		}

		// Token: 0x06006713 RID: 26387 RVA: 0x00211B7C File Offset: 0x0020FD7C
		private void Progress()
		{
			this.currT = this.CycleCompletionPercent();
			this.currForward = this.IsEvenCycle();
			this.percent = this.currT;
			if (this.reverseDirOnCycle)
			{
				this.percent = (this.currForward ? this.currT : (1f - this.currT));
			}
			if (this.reverseDir)
			{
				this.percent = 1f - this.percent;
			}
		}

		// Token: 0x06006714 RID: 26388 RVA: 0x00211BF4 File Offset: 0x0020FDF4
		public bool IsAnchoredToTable()
		{
			foreach (BuilderAttachGridPlane builderAttachGridPlane in this.myGridPlanes)
			{
				if (builderAttachGridPlane.attachIndex == builderAttachGridPlane.piece.attachIndex)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06006715 RID: 26389 RVA: 0x00211C30 File Offset: 0x0020FE30
		public void OnPieceDestroy()
		{
			this.ResetMovingGrid();
		}

		// Token: 0x040075E3 RID: 30179
		public BuilderPiece myPiece;

		// Token: 0x040075E4 RID: 30180
		public BuilderAttachGridPlane[] myGridPlanes;

		// Token: 0x040075E5 RID: 30181
		[SerializeField]
		private BuilderMovingPart.BuilderMovingPartType moveType;

		// Token: 0x040075E6 RID: 30182
		[SerializeField]
		private float startPercentage = 0.5f;

		// Token: 0x040075E7 RID: 30183
		[SerializeField]
		private float velocity;

		// Token: 0x040075E8 RID: 30184
		[SerializeField]
		private bool reverseDirOnCycle = true;

		// Token: 0x040075E9 RID: 30185
		[SerializeField]
		private bool reverseDir;

		// Token: 0x040075EA RID: 30186
		[SerializeField]
		private float cycleDelay = 0.25f;

		// Token: 0x040075EB RID: 30187
		[SerializeField]
		protected Transform startXf;

		// Token: 0x040075EC RID: 30188
		[SerializeField]
		protected Transform endXf;

		// Token: 0x040075ED RID: 30189
		public static int NUM_PAUSE_NODES = 32;

		// Token: 0x040075EE RID: 30190
		private AnimationCurve lerpAlpha;

		// Token: 0x040075EF RID: 30191
		public bool isMoving;

		// Token: 0x040075F0 RID: 30192
		private Quaternion initLocalRotation = Quaternion.identity;

		// Token: 0x040075F1 RID: 30193
		private Vector3 initLocalPos = Vector3.zero;

		// Token: 0x040075F2 RID: 30194
		private float cycleDuration;

		// Token: 0x040075F3 RID: 30195
		private float distance;

		// Token: 0x040075F4 RID: 30196
		private float currT;

		// Token: 0x040075F5 RID: 30197
		private float percent;

		// Token: 0x040075F6 RID: 30198
		private bool currForward;

		// Token: 0x040075F7 RID: 30199
		private float dtSinceServerUpdate;

		// Token: 0x040075F8 RID: 30200
		private int lastServerTimeStamp;

		// Token: 0x040075F9 RID: 30201
		private float rotateStartAmt;

		// Token: 0x040075FA RID: 30202
		private float rotateAmt;

		// Token: 0x040075FB RID: 30203
		private uint startPercentageCycleOffset;

		// Token: 0x0200102E RID: 4142
		public enum BuilderMovingPartType
		{
			// Token: 0x040075FD RID: 30205
			Translation,
			// Token: 0x040075FE RID: 30206
			Rotation
		}
	}
}
