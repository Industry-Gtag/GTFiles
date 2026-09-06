using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001371 RID: 4977
	public class SimpleTransformAnimatorCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x06007CA8 RID: 31912 RVA: 0x0028BB2C File Offset: 0x00289D2C
		private void DebugToggle()
		{
			this.Toggle();
		}

		// Token: 0x06007CA9 RID: 31913 RVA: 0x0028BB34 File Offset: 0x00289D34
		private void DebugA()
		{
			this.TogglePoseA();
		}

		// Token: 0x06007CAA RID: 31914 RVA: 0x0028BB3C File Offset: 0x00289D3C
		private void DebugB()
		{
			this.TogglePoseB();
		}

		// Token: 0x17000C2C RID: 3116
		// (get) Token: 0x06007CAB RID: 31915 RVA: 0x0028BB44 File Offset: 0x00289D44
		// (set) Token: 0x06007CAC RID: 31916 RVA: 0x0028BB4C File Offset: 0x00289D4C
		public bool TickRunning { get; set; }

		// Token: 0x06007CAD RID: 31917 RVA: 0x0028BB55 File Offset: 0x00289D55
		private void OnEnable()
		{
			this.posBlendCurrent = this.posBlendTarget;
			this.UpdateTransform();
		}

		// Token: 0x06007CAE RID: 31918 RVA: 0x0028BB69 File Offset: 0x00289D69
		private void OnDisable()
		{
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveCallbackTarget(this);
				this.TickRunning = false;
			}
		}

		// Token: 0x06007CAF RID: 31919 RVA: 0x0028BB80 File Offset: 0x00289D80
		private void CheckAnimationNeeded()
		{
			bool flag = false;
			bool flag2 = Mathf.Approximately(this.posBlendCurrent, this.posBlendTarget);
			switch (this.animMode)
			{
			case SimpleTransformAnimatorCosmetic.animModes.stepToTargetPos:
				flag = !flag2;
				break;
			case SimpleTransformAnimatorCosmetic.animModes.animateOneshot:
				flag = this.loopAnim || !flag2;
				break;
			}
			if (flag && !this.TickRunning)
			{
				TickSystem<object>.AddCallbackTarget(this);
				this.TickRunning = true;
				this.isAnimating = true;
				return;
			}
			if (!flag && this.TickRunning)
			{
				TickSystem<object>.RemoveCallbackTarget(this);
				this.TickRunning = false;
				this.isAnimating = false;
			}
		}

		// Token: 0x06007CB0 RID: 31920 RVA: 0x0028BC14 File Offset: 0x00289E14
		public void Tick()
		{
			float num = 1f / this.animationDuration;
			this.posBlendCurrent = Mathf.MoveTowards(this.posBlendCurrent, this.posBlendTarget, Time.deltaTime * num);
			switch (this.animMode)
			{
			default:
				this.UpdateTransform();
				this.CheckAnimationNeeded();
				return;
			}
		}

		// Token: 0x06007CB1 RID: 31921 RVA: 0x0028BC74 File Offset: 0x00289E74
		private void UpdateTransform()
		{
			Vector3 vector = this.targetTransform.position;
			Quaternion quaternion = this.targetTransform.rotation;
			float num = this.InterpolationCurve.Evaluate(this.posBlendCurrent);
			if (this.animatedProperties == SimpleTransformAnimatorCosmetic.animatedPropertyChoices.Position || this.animatedProperties == SimpleTransformAnimatorCosmetic.animatedPropertyChoices.PositionAndRotation)
			{
				vector = Vector3.Lerp(this.poseA.position, this.poseB.position, num);
			}
			if (this.animatedProperties == SimpleTransformAnimatorCosmetic.animatedPropertyChoices.Rotation || this.animatedProperties == SimpleTransformAnimatorCosmetic.animatedPropertyChoices.PositionAndRotation)
			{
				quaternion = Quaternion.Slerp(this.poseA.rotation, this.poseB.rotation, num);
			}
			this.targetTransform.SetPositionAndRotation(vector, quaternion);
		}

		// Token: 0x06007CB2 RID: 31922 RVA: 0x0028BD15 File Offset: 0x00289F15
		public void Toggle()
		{
			this.animMode = SimpleTransformAnimatorCosmetic.animModes.stepToTargetPos;
			this.posBlendTarget = ((this.posBlendTarget < 0.5f) ? 1f : 0f);
			this.CheckAnimationNeeded();
		}

		// Token: 0x06007CB3 RID: 31923 RVA: 0x0028BD43 File Offset: 0x00289F43
		public void TogglePoseA()
		{
			this.animMode = SimpleTransformAnimatorCosmetic.animModes.stepToTargetPos;
			this.posBlendTarget = 0f;
			this.CheckAnimationNeeded();
		}

		// Token: 0x06007CB4 RID: 31924 RVA: 0x0028BD5D File Offset: 0x00289F5D
		public void TogglePoseB()
		{
			this.animMode = SimpleTransformAnimatorCosmetic.animModes.stepToTargetPos;
			this.posBlendTarget = 1f;
			this.CheckAnimationNeeded();
		}

		// Token: 0x06007CB5 RID: 31925 RVA: 0x0028BD77 File Offset: 0x00289F77
		public void playAnimationOneshot()
		{
			this.animMode = SimpleTransformAnimatorCosmetic.animModes.animateOneshot;
			this.posBlendCurrent = 0f;
			this.posBlendTarget = 1f;
			this.CheckAnimationNeeded();
		}

		// Token: 0x06007CB6 RID: 31926 RVA: 0x0028BD9C File Offset: 0x00289F9C
		private void DebugPlayAnimationOneShot()
		{
			this.playAnimationOneshot();
		}

		// Token: 0x04008F59 RID: 36697
		private SimpleTransformAnimatorCosmetic.animModes animMode;

		// Token: 0x04008F5A RID: 36698
		[Tooltip("Shapes how the transform will interpolate over the course of the animation.")]
		public AnimationCurve InterpolationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04008F5B RID: 36699
		[SerializeField]
		[Tooltip("The object that will animate (blend) between the poses.")]
		private Transform targetTransform;

		// Token: 0x04008F5C RID: 36700
		[SerializeField]
		[Tooltip("Start pose (blend value 0).")]
		private Transform poseA;

		// Token: 0x04008F5D RID: 36701
		[SerializeField]
		[Tooltip("End pose (blend value 1).")]
		private Transform poseB;

		// Token: 0x04008F5E RID: 36702
		[FormerlySerializedAs("transitionTime")]
		[SerializeField]
		[Tooltip("Total time (in seconds) to animate fully between poses.")]
		private float animationDuration = 1f;

		// Token: 0x04008F5F RID: 36703
		[SerializeField]
		[Tooltip("Controls what aspect of the transform is affected by the blend.")]
		private SimpleTransformAnimatorCosmetic.animatedPropertyChoices animatedProperties = SimpleTransformAnimatorCosmetic.animatedPropertyChoices.PositionAndRotation;

		// Token: 0x04008F60 RID: 36704
		private bool loopAnim;

		// Token: 0x04008F61 RID: 36705
		private float posBlendCurrent;

		// Token: 0x04008F62 RID: 36706
		private float posBlendTarget;

		// Token: 0x04008F63 RID: 36707
		private bool isAnimating;

		// Token: 0x02001372 RID: 4978
		public enum animatedPropertyChoices
		{
			// Token: 0x04008F66 RID: 36710
			Position,
			// Token: 0x04008F67 RID: 36711
			Rotation,
			// Token: 0x04008F68 RID: 36712
			PositionAndRotation
		}

		// Token: 0x02001373 RID: 4979
		public enum animModes
		{
			// Token: 0x04008F6A RID: 36714
			stepToTargetPos,
			// Token: 0x04008F6B RID: 36715
			animateBounce,
			// Token: 0x04008F6C RID: 36716
			animateOneshot
		}
	}
}
