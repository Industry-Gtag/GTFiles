using System;

namespace GorillaTagScripts
{
	// Token: 0x02000F5B RID: 3931
	public class BuilderOptionButton : GorillaPressableButton
	{
		// Token: 0x060060B2 RID: 24754 RVA: 0x001EAC4F File Offset: 0x001E8E4F
		public override void Start()
		{
			base.Start();
		}

		// Token: 0x060060B3 RID: 24755 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void OnDestroy()
		{
		}

		// Token: 0x060060B4 RID: 24756 RVA: 0x001EAC57 File Offset: 0x001E8E57
		public void Setup(Action<BuilderOptionButton, bool> onPressed)
		{
			this.onPressed = onPressed;
		}

		// Token: 0x060060B5 RID: 24757 RVA: 0x001EAC60 File Offset: 0x001E8E60
		public override void ButtonActivationWithHand(bool isLeftHand)
		{
			Action<BuilderOptionButton, bool> action = this.onPressed;
			if (action == null)
			{
				return;
			}
			action(this, isLeftHand);
		}

		// Token: 0x060060B6 RID: 24758 RVA: 0x001EAC74 File Offset: 0x001E8E74
		public void SetPressed(bool pressed)
		{
			this.buttonRenderer.material = (pressed ? this.pressedMaterial : this.unpressedMaterial);
		}

		// Token: 0x04006F54 RID: 28500
		private new Action<BuilderOptionButton, bool> onPressed;
	}
}
