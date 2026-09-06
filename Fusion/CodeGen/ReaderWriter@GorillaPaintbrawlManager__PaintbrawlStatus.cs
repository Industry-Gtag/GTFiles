using System;
using System.Runtime.CompilerServices;

namespace Fusion.CodeGen
{
	// Token: 0x02001493 RID: 5267
	[WeaverGenerated]
	internal struct ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus : IElementReaderWriter<GorillaPaintbrawlManager.PaintbrawlStatus>
	{
		// Token: 0x060083F2 RID: 33778 RVA: 0x002B1F16 File Offset: 0x002B0116
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public unsafe GorillaPaintbrawlManager.PaintbrawlStatus Read(byte* data, int index)
		{
			return *(GorillaPaintbrawlManager.PaintbrawlStatus*)(data + index * 4);
		}

		// Token: 0x060083F3 RID: 33779 RVA: 0x002B1C80 File Offset: 0x002AFE80
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public unsafe ref GorillaPaintbrawlManager.PaintbrawlStatus ReadRef(byte* data, int index)
		{
			return ref *(GorillaPaintbrawlManager.PaintbrawlStatus*)(data + index * 4);
		}

		// Token: 0x060083F4 RID: 33780 RVA: 0x002B1F26 File Offset: 0x002B0126
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public unsafe void Write(byte* data, int index, GorillaPaintbrawlManager.PaintbrawlStatus val)
		{
			*(GorillaPaintbrawlManager.PaintbrawlStatus*)(data + index * 4) = val;
		}

		// Token: 0x060083F5 RID: 33781 RVA: 0x00023F0C File Offset: 0x0002210C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public int GetElementWordCount()
		{
			return 1;
		}

		// Token: 0x060083F6 RID: 33782 RVA: 0x002B1F38 File Offset: 0x002B0138
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public int GetElementHashCode(GorillaPaintbrawlManager.PaintbrawlStatus val)
		{
			return val.GetHashCode();
		}

		// Token: 0x060083F7 RID: 33783 RVA: 0x002B1F54 File Offset: 0x002B0154
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[WeaverGenerated]
		public static IElementReaderWriter<GorillaPaintbrawlManager.PaintbrawlStatus> GetInstance()
		{
			if (ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus.Instance == null)
			{
				ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus.Instance = default(ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus);
			}
			return ReaderWriter@GorillaPaintbrawlManager__PaintbrawlStatus.Instance;
		}

		// Token: 0x040095D6 RID: 38358
		[WeaverGenerated]
		public static IElementReaderWriter<GorillaPaintbrawlManager.PaintbrawlStatus> Instance;
	}
}
