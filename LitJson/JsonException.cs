using System;

namespace LitJson
{
	// Token: 0x02000EEC RID: 3820
	public class JsonException : ApplicationException
	{
		// Token: 0x06005D65 RID: 23909 RVA: 0x001DDA1D File Offset: 0x001DBC1D
		public JsonException()
		{
		}

		// Token: 0x06005D66 RID: 23910 RVA: 0x001DDA25 File Offset: 0x001DBC25
		internal JsonException(ParserToken token)
			: base(string.Format("Invalid token '{0}' in input string", token))
		{
		}

		// Token: 0x06005D67 RID: 23911 RVA: 0x001DDA3D File Offset: 0x001DBC3D
		internal JsonException(ParserToken token, Exception inner_exception)
			: base(string.Format("Invalid token '{0}' in input string", token), inner_exception)
		{
		}

		// Token: 0x06005D68 RID: 23912 RVA: 0x001DDA56 File Offset: 0x001DBC56
		internal JsonException(int c)
			: base(string.Format("Invalid character '{0}' in input string", (char)c))
		{
		}

		// Token: 0x06005D69 RID: 23913 RVA: 0x001DDA6F File Offset: 0x001DBC6F
		internal JsonException(int c, Exception inner_exception)
			: base(string.Format("Invalid character '{0}' in input string", (char)c), inner_exception)
		{
		}

		// Token: 0x06005D6A RID: 23914 RVA: 0x001DDA89 File Offset: 0x001DBC89
		public JsonException(string message)
			: base(message)
		{
		}

		// Token: 0x06005D6B RID: 23915 RVA: 0x001DDA92 File Offset: 0x001DBC92
		public JsonException(string message, Exception inner_exception)
			: base(message, inner_exception)
		{
		}
	}
}
