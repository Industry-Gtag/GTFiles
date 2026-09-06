using System;

namespace BoingKit
{
	// Token: 0x02001429 RID: 5161
	public struct Version : IEquatable<Version>
	{
		// Token: 0x17000C80 RID: 3200
		// (get) Token: 0x06008223 RID: 33315 RVA: 0x002A9148 File Offset: 0x002A7348
		public readonly int MajorVersion { get; }

		// Token: 0x17000C81 RID: 3201
		// (get) Token: 0x06008224 RID: 33316 RVA: 0x002A9150 File Offset: 0x002A7350
		public readonly int MinorVersion { get; }

		// Token: 0x17000C82 RID: 3202
		// (get) Token: 0x06008225 RID: 33317 RVA: 0x002A9158 File Offset: 0x002A7358
		public readonly int Revision { get; }

		// Token: 0x06008226 RID: 33318 RVA: 0x002A9160 File Offset: 0x002A7360
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				this.MajorVersion.ToString(),
				".",
				this.MinorVersion.ToString(),
				".",
				this.Revision.ToString()
			});
		}

		// Token: 0x06008227 RID: 33319 RVA: 0x002A91BB File Offset: 0x002A73BB
		public bool IsValid()
		{
			return this.MajorVersion >= 0 && this.MinorVersion >= 0 && this.Revision >= 0;
		}

		// Token: 0x06008228 RID: 33320 RVA: 0x002A91DD File Offset: 0x002A73DD
		public Version(int majorVersion = -1, int minorVersion = -1, int revision = -1)
		{
			this.MajorVersion = majorVersion;
			this.MinorVersion = minorVersion;
			this.Revision = revision;
		}

		// Token: 0x06008229 RID: 33321 RVA: 0x002A91F4 File Offset: 0x002A73F4
		public static bool operator ==(Version lhs, Version rhs)
		{
			return lhs.IsValid() && rhs.IsValid() && (lhs.MajorVersion == rhs.MajorVersion && lhs.MinorVersion == rhs.MinorVersion) && lhs.Revision == rhs.Revision;
		}

		// Token: 0x0600822A RID: 33322 RVA: 0x002A9249 File Offset: 0x002A7449
		public static bool operator !=(Version lhs, Version rhs)
		{
			return !(lhs == rhs);
		}

		// Token: 0x0600822B RID: 33323 RVA: 0x002A9255 File Offset: 0x002A7455
		public override bool Equals(object obj)
		{
			return obj is Version && this.Equals((Version)obj);
		}

		// Token: 0x0600822C RID: 33324 RVA: 0x002A926D File Offset: 0x002A746D
		public bool Equals(Version other)
		{
			return this.MajorVersion == other.MajorVersion && this.MinorVersion == other.MinorVersion && this.Revision == other.Revision;
		}

		// Token: 0x0600822D RID: 33325 RVA: 0x002A92A0 File Offset: 0x002A74A0
		public override int GetHashCode()
		{
			return ((366299368 * -1521134295 + this.MajorVersion.GetHashCode()) * -1521134295 + this.MinorVersion.GetHashCode()) * -1521134295 + this.Revision.GetHashCode();
		}

		// Token: 0x04009350 RID: 37712
		public static readonly Version Invalid = new Version(-1, -1, -1);

		// Token: 0x04009351 RID: 37713
		public static readonly Version FirstTracked = new Version(1, 2, 33);

		// Token: 0x04009352 RID: 37714
		public static readonly Version LastUntracked = new Version(1, 2, 32);
	}
}
