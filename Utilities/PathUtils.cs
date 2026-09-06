using System;

namespace Utilities
{
	// Token: 0x02000F24 RID: 3876
	public static class PathUtils
	{
		// Token: 0x06005F0A RID: 24330 RVA: 0x001E370C File Offset: 0x001E190C
		public static string Resolve(params string[] subPaths)
		{
			if (subPaths == null || subPaths.Length == 0)
			{
				return null;
			}
			string[] array = string.Concat(subPaths).Split(PathUtils.kPathSeps, StringSplitOptions.RemoveEmptyEntries);
			return Uri.UnescapeDataString(new Uri(string.Join("/", array)).AbsolutePath);
		}

		// Token: 0x04006D97 RID: 28055
		private static readonly char[] kPathSeps = new char[] { '\\', '/' };

		// Token: 0x04006D98 RID: 28056
		private const string kFwdSlash = "/";
	}
}
