using System;

namespace emotitron.CompressionTests
{
	// Token: 0x020013E6 RID: 5094
	public class BasicWriter
	{
		// Token: 0x06008095 RID: 32917 RVA: 0x0029D47E File Offset: 0x0029B67E
		public static void Reset()
		{
			BasicWriter.pos = 0;
		}

		// Token: 0x06008096 RID: 32918 RVA: 0x0029D486 File Offset: 0x0029B686
		public static byte[] BasicWrite(byte[] buffer, byte value)
		{
			buffer[BasicWriter.pos] = value;
			BasicWriter.pos++;
			return buffer;
		}

		// Token: 0x06008097 RID: 32919 RVA: 0x0029D49D File Offset: 0x0029B69D
		public static byte BasicRead(byte[] buffer)
		{
			byte b = buffer[BasicWriter.pos];
			BasicWriter.pos++;
			return b;
		}

		// Token: 0x040091AC RID: 37292
		public static int pos;
	}
}
