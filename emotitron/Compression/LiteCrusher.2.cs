using System;

namespace emotitron.Compression
{
	// Token: 0x020013DB RID: 5083
	[Serializable]
	public abstract class LiteCrusher<T> : LiteCrusher where T : struct
	{
		// Token: 0x06008023 RID: 32803
		public abstract ulong Encode(T val);

		// Token: 0x06008024 RID: 32804
		public abstract T Decode(uint val);

		// Token: 0x06008025 RID: 32805
		public abstract ulong WriteValue(T val, byte[] buffer, ref int bitposition);

		// Token: 0x06008026 RID: 32806
		public abstract void WriteCValue(uint val, byte[] buffer, ref int bitposition);

		// Token: 0x06008027 RID: 32807
		public abstract T ReadValue(byte[] buffer, ref int bitposition);
	}
}
