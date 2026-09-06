using System;
using System.IO;
using System.Text;

namespace GorillaTag.Scripts.Utilities
{
	// Token: 0x0200124D RID: 4685
	public class StringWriterWithEncoding : StringWriter
	{
		// Token: 0x17000B8A RID: 2954
		// (get) Token: 0x060076B9 RID: 30393 RVA: 0x00267B2D File Offset: 0x00265D2D
		public override Encoding Encoding { get; }

		// Token: 0x060076BA RID: 30394 RVA: 0x00267B35 File Offset: 0x00265D35
		public StringWriterWithEncoding(Encoding encoding)
		{
			this.Encoding = encoding;
		}
	}
}
