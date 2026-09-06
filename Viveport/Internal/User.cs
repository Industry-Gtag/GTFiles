using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Viveport.Internal
{
	// Token: 0x02000EDA RID: 3802
	internal class User
	{
		// Token: 0x06005C53 RID: 23635 RVA: 0x001DC457 File Offset: 0x001DA657
		static User()
		{
			Api.LoadLibraryManually("viveport_api");
		}

		// Token: 0x06005C54 RID: 23636
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportUser_IsReady")]
		internal static extern int IsReady(StatusCallback IsReadyCallback);

		// Token: 0x06005C55 RID: 23637
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportUser_IsReady")]
		internal static extern int IsReady_64(StatusCallback IsReadyCallback);

		// Token: 0x06005C56 RID: 23638
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportUser_GetUserID")]
		internal static extern int GetUserID(StringBuilder userId, int size);

		// Token: 0x06005C57 RID: 23639
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportUser_GetUserID")]
		internal static extern int GetUserID_64(StringBuilder userId, int size);

		// Token: 0x06005C58 RID: 23640
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportUser_GetUserName")]
		internal static extern int GetUserName(StringBuilder userName, int size);

		// Token: 0x06005C59 RID: 23641
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportUser_GetUserName")]
		internal static extern int GetUserName_64(StringBuilder userName, int size);

		// Token: 0x06005C5A RID: 23642
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportUser_GetUserAvatarUrl")]
		internal static extern int GetUserAvatarUrl(StringBuilder userAvatarUrl, int size);

		// Token: 0x06005C5B RID: 23643
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportUser_GetUserAvatarUrl")]
		internal static extern int GetUserAvatarUrl_64(StringBuilder userAvatarUrl, int size);
	}
}
