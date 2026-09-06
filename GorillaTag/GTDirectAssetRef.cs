using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x020011E0 RID: 4576
	[Serializable]
	public struct GTDirectAssetRef<T> : IEquatable<T> where T : Object
	{
		// Token: 0x17000B42 RID: 2882
		// (get) Token: 0x06007453 RID: 29779 RVA: 0x0025D688 File Offset: 0x0025B888
		// (set) Token: 0x06007454 RID: 29780 RVA: 0x0025D690 File Offset: 0x0025B890
		public T obj
		{
			get
			{
				return this._obj;
			}
			set
			{
				this._obj = value;
				this.edAssetPath = null;
			}
		}

		// Token: 0x06007455 RID: 29781 RVA: 0x0025D690 File Offset: 0x0025B890
		public GTDirectAssetRef(T theObj)
		{
			this._obj = theObj;
			this.edAssetPath = null;
		}

		// Token: 0x06007456 RID: 29782 RVA: 0x0025D6A0 File Offset: 0x0025B8A0
		public static implicit operator T(GTDirectAssetRef<T> refObject)
		{
			return refObject.obj;
		}

		// Token: 0x06007457 RID: 29783 RVA: 0x0025D6AC File Offset: 0x0025B8AC
		public static implicit operator GTDirectAssetRef<T>(T other)
		{
			return new GTDirectAssetRef<T>
			{
				obj = other
			};
		}

		// Token: 0x06007458 RID: 29784 RVA: 0x0025D6CA File Offset: 0x0025B8CA
		public bool Equals(T other)
		{
			return this.obj == other;
		}

		// Token: 0x06007459 RID: 29785 RVA: 0x0025D6E4 File Offset: 0x0025B8E4
		public override bool Equals(object other)
		{
			T t = other as T;
			return t != null && this.Equals(t);
		}

		// Token: 0x0600745A RID: 29786 RVA: 0x0025D70E File Offset: 0x0025B90E
		public override int GetHashCode()
		{
			if (!(this.obj != null))
			{
				return 0;
			}
			return this.obj.GetHashCode();
		}

		// Token: 0x0600745B RID: 29787 RVA: 0x0025D735 File Offset: 0x0025B935
		public static bool operator ==(GTDirectAssetRef<T> left, T right)
		{
			return left.Equals(right);
		}

		// Token: 0x0600745C RID: 29788 RVA: 0x0025D73F File Offset: 0x0025B93F
		public static bool operator !=(GTDirectAssetRef<T> left, T right)
		{
			return !(left == right);
		}

		// Token: 0x04008427 RID: 33831
		[SerializeField]
		[HideInInspector]
		internal T _obj;

		// Token: 0x04008428 RID: 33832
		[FormerlySerializedAs("assetPath")]
		public string edAssetPath;
	}
}
