using System;

namespace GorillaNetworking
{
	// Token: 0x020010EA RID: 4330
	public static class GorillaKeyboardBindingExtensions
	{
		// Token: 0x06006CAB RID: 27819 RVA: 0x002322D0 File Offset: 0x002304D0
		public static bool FromNumberBindingToInt(this GorillaKeyboardBindings binding, out int result)
		{
			result = -1;
			switch (binding)
			{
			case GorillaKeyboardBindings.zero:
				result = 0;
				break;
			case GorillaKeyboardBindings.one:
				result = 1;
				break;
			case GorillaKeyboardBindings.two:
				result = 2;
				break;
			case GorillaKeyboardBindings.three:
				result = 3;
				break;
			case GorillaKeyboardBindings.four:
				result = 4;
				break;
			case GorillaKeyboardBindings.five:
				result = 5;
				break;
			case GorillaKeyboardBindings.six:
				result = 6;
				break;
			case GorillaKeyboardBindings.seven:
				result = 7;
				break;
			case GorillaKeyboardBindings.eight:
				result = 8;
				break;
			case GorillaKeyboardBindings.nine:
				result = 9;
				break;
			default:
				return false;
			}
			return true;
		}
	}
}
