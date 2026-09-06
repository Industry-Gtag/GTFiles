using System;
using GorillaTagScripts.UI;

namespace GorillaTagScripts.VirtualStumpCustomMaps.UI
{
	// Token: 0x02000FE2 RID: 4066
	public class CustomMapsKeyboard : GorillaKeyWrapper<CustomMapKeyboardBinding>
	{
		// Token: 0x06006539 RID: 25913 RVA: 0x00209444 File Offset: 0x00207644
		public static string BindingToString(CustomMapKeyboardBinding binding)
		{
			return CustomMapsKeyButton.BindingToString(binding);
		}
	}
}
