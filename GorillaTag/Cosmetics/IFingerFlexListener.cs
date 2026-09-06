using System;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001344 RID: 4932
	public interface IFingerFlexListener
	{
		// Token: 0x06007BB4 RID: 31668 RVA: 0x00023F0C File Offset: 0x0002210C
		bool FingerFlexValidation(bool isLeftHand)
		{
			return true;
		}

		// Token: 0x06007BB5 RID: 31669
		void OnButtonPressed(bool isLeftHand, float value);

		// Token: 0x06007BB6 RID: 31670
		void OnButtonReleased(bool isLeftHand, float value);

		// Token: 0x06007BB7 RID: 31671
		void OnButtonPressStayed(bool isLeftHand, float value);

		// Token: 0x02001345 RID: 4933
		public enum ComponentActivator
		{
			// Token: 0x04008DCA RID: 36298
			FingerReleased,
			// Token: 0x04008DCB RID: 36299
			FingerFlexed,
			// Token: 0x04008DCC RID: 36300
			FingerStayed
		}
	}
}
