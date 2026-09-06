using System;
using Photon.Pun;

namespace GorillaNetworking
{
	// Token: 0x0200110D RID: 4365
	public readonly struct PhotonTimestamp : IEquatable<PhotonTimestamp>, IComparable<PhotonTimestamp>
	{
		// Token: 0x06006D6C RID: 28012 RVA: 0x00236016 File Offset: 0x00234216
		public PhotonTimestamp(double raw)
		{
			this.Value = PhotonTimestamp.Normalize(raw);
		}

		// Token: 0x17000A65 RID: 2661
		// (get) Token: 0x06006D6D RID: 28013 RVA: 0x00236024 File Offset: 0x00234224
		public static PhotonTimestamp Now
		{
			get
			{
				return new PhotonTimestamp(PhotonNetwork.Time);
			}
		}

		// Token: 0x06006D6E RID: 28014 RVA: 0x00236030 File Offset: 0x00234230
		private static double Normalize(double v)
		{
			double num = v % 4294967.296;
			if (num < 0.0)
			{
				num += 4294967.296;
			}
			return num;
		}

		// Token: 0x06006D6F RID: 28015 RVA: 0x00236064 File Offset: 0x00234264
		public static double Delta(PhotonTimestamp a, PhotonTimestamp b)
		{
			double num = b.Value - a.Value;
			if (num > 2147483.648)
			{
				num -= 4294967.296;
			}
			else if (num <= -2147483.648)
			{
				num += 4294967.296;
			}
			return num;
		}

		// Token: 0x06006D70 RID: 28016 RVA: 0x002360B2 File Offset: 0x002342B2
		public double SecondsUntil(PhotonTimestamp other)
		{
			return PhotonTimestamp.Delta(this, other);
		}

		// Token: 0x06006D71 RID: 28017 RVA: 0x002360C0 File Offset: 0x002342C0
		public double SecondsSince(PhotonTimestamp other)
		{
			return PhotonTimestamp.Delta(other, this);
		}

		// Token: 0x06006D72 RID: 28018 RVA: 0x002360CE File Offset: 0x002342CE
		public PhotonTimestamp AddSeconds(double seconds)
		{
			return new PhotonTimestamp(this.Value + seconds);
		}

		// Token: 0x06006D73 RID: 28019 RVA: 0x002360CE File Offset: 0x002342CE
		public static PhotonTimestamp operator +(PhotonTimestamp t, double seconds)
		{
			return new PhotonTimestamp(t.Value + seconds);
		}

		// Token: 0x06006D74 RID: 28020 RVA: 0x002360DD File Offset: 0x002342DD
		public static PhotonTimestamp operator -(PhotonTimestamp t, double seconds)
		{
			return new PhotonTimestamp(t.Value - seconds);
		}

		// Token: 0x06006D75 RID: 28021 RVA: 0x002360EC File Offset: 0x002342EC
		public static double operator -(PhotonTimestamp a, PhotonTimestamp b)
		{
			return PhotonTimestamp.Delta(b, a);
		}

		// Token: 0x06006D76 RID: 28022 RVA: 0x002360F5 File Offset: 0x002342F5
		public static bool operator <(PhotonTimestamp a, PhotonTimestamp b)
		{
			return PhotonTimestamp.Delta(a, b) > 0.0;
		}

		// Token: 0x06006D77 RID: 28023 RVA: 0x00236109 File Offset: 0x00234309
		public static bool operator >(PhotonTimestamp a, PhotonTimestamp b)
		{
			return PhotonTimestamp.Delta(a, b) < 0.0;
		}

		// Token: 0x06006D78 RID: 28024 RVA: 0x0023611D File Offset: 0x0023431D
		public static bool operator <=(PhotonTimestamp a, PhotonTimestamp b)
		{
			return PhotonTimestamp.Delta(a, b) >= 0.0;
		}

		// Token: 0x06006D79 RID: 28025 RVA: 0x00236134 File Offset: 0x00234334
		public static bool operator >=(PhotonTimestamp a, PhotonTimestamp b)
		{
			return PhotonTimestamp.Delta(a, b) <= 0.0;
		}

		// Token: 0x06006D7A RID: 28026 RVA: 0x0023614B File Offset: 0x0023434B
		public static bool operator ==(PhotonTimestamp a, PhotonTimestamp b)
		{
			return a.Value == b.Value;
		}

		// Token: 0x06006D7B RID: 28027 RVA: 0x0023615B File Offset: 0x0023435B
		public static bool operator !=(PhotonTimestamp a, PhotonTimestamp b)
		{
			return a.Value != b.Value;
		}

		// Token: 0x06006D7C RID: 28028 RVA: 0x00236170 File Offset: 0x00234370
		public int CompareTo(PhotonTimestamp other)
		{
			double num = PhotonTimestamp.Delta(this, other);
			if (num > 0.0)
			{
				return -1;
			}
			if (num < 0.0)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x06006D7D RID: 28029 RVA: 0x0023614B File Offset: 0x0023434B
		public bool Equals(PhotonTimestamp other)
		{
			return this.Value == other.Value;
		}

		// Token: 0x06006D7E RID: 28030 RVA: 0x002361A8 File Offset: 0x002343A8
		public override bool Equals(object obj)
		{
			if (obj is PhotonTimestamp)
			{
				PhotonTimestamp photonTimestamp = (PhotonTimestamp)obj;
				return this.Equals(photonTimestamp);
			}
			return false;
		}

		// Token: 0x06006D7F RID: 28031 RVA: 0x002361CD File Offset: 0x002343CD
		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		// Token: 0x06006D80 RID: 28032 RVA: 0x002361DA File Offset: 0x002343DA
		public override string ToString()
		{
			return this.Value.ToString("F3");
		}

		// Token: 0x04007DD1 RID: 32209
		public const double WrapPeriod = 4294967.296;

		// Token: 0x04007DD2 RID: 32210
		public const double HalfWrapPeriod = 2147483.648;

		// Token: 0x04007DD3 RID: 32211
		public readonly double Value;
	}
}
