using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02001457 RID: 5207
	public struct Aabb
	{
		// Token: 0x17000C94 RID: 3220
		// (get) Token: 0x0600832B RID: 33579 RVA: 0x002AF89F File Offset: 0x002ADA9F
		// (set) Token: 0x0600832C RID: 33580 RVA: 0x002AF8AC File Offset: 0x002ADAAC
		public float MinX
		{
			get
			{
				return this.Min.x;
			}
			set
			{
				this.Min.x = value;
			}
		}

		// Token: 0x17000C95 RID: 3221
		// (get) Token: 0x0600832D RID: 33581 RVA: 0x002AF8BA File Offset: 0x002ADABA
		// (set) Token: 0x0600832E RID: 33582 RVA: 0x002AF8C7 File Offset: 0x002ADAC7
		public float MinY
		{
			get
			{
				return this.Min.y;
			}
			set
			{
				this.Min.y = value;
			}
		}

		// Token: 0x17000C96 RID: 3222
		// (get) Token: 0x0600832F RID: 33583 RVA: 0x002AF8D5 File Offset: 0x002ADAD5
		// (set) Token: 0x06008330 RID: 33584 RVA: 0x002AF8E2 File Offset: 0x002ADAE2
		public float MinZ
		{
			get
			{
				return this.Min.z;
			}
			set
			{
				this.Min.z = value;
			}
		}

		// Token: 0x17000C97 RID: 3223
		// (get) Token: 0x06008331 RID: 33585 RVA: 0x002AF8F0 File Offset: 0x002ADAF0
		// (set) Token: 0x06008332 RID: 33586 RVA: 0x002AF8FD File Offset: 0x002ADAFD
		public float MaxX
		{
			get
			{
				return this.Max.x;
			}
			set
			{
				this.Max.x = value;
			}
		}

		// Token: 0x17000C98 RID: 3224
		// (get) Token: 0x06008333 RID: 33587 RVA: 0x002AF90B File Offset: 0x002ADB0B
		// (set) Token: 0x06008334 RID: 33588 RVA: 0x002AF918 File Offset: 0x002ADB18
		public float MaxY
		{
			get
			{
				return this.Max.y;
			}
			set
			{
				this.Max.y = value;
			}
		}

		// Token: 0x17000C99 RID: 3225
		// (get) Token: 0x06008335 RID: 33589 RVA: 0x002AF926 File Offset: 0x002ADB26
		// (set) Token: 0x06008336 RID: 33590 RVA: 0x002AF933 File Offset: 0x002ADB33
		public float MaxZ
		{
			get
			{
				return this.Max.z;
			}
			set
			{
				this.Max.z = value;
			}
		}

		// Token: 0x17000C9A RID: 3226
		// (get) Token: 0x06008337 RID: 33591 RVA: 0x002AF941 File Offset: 0x002ADB41
		public Vector3 Center
		{
			get
			{
				return 0.5f * (this.Min + this.Max);
			}
		}

		// Token: 0x17000C9B RID: 3227
		// (get) Token: 0x06008338 RID: 33592 RVA: 0x002AF960 File Offset: 0x002ADB60
		public Vector3 Size
		{
			get
			{
				Vector3 vector = this.Max - this.Min;
				vector.x = Mathf.Max(0f, vector.x);
				vector.y = Mathf.Max(0f, vector.y);
				vector.z = Mathf.Max(0f, vector.z);
				return vector;
			}
		}

		// Token: 0x17000C9C RID: 3228
		// (get) Token: 0x06008339 RID: 33593 RVA: 0x002AF9C5 File Offset: 0x002ADBC5
		public static Aabb Empty
		{
			get
			{
				return new Aabb(new Vector3(float.MaxValue, float.MaxValue, float.MaxValue), new Vector3(float.MinValue, float.MinValue, float.MinValue));
			}
		}

		// Token: 0x0600833A RID: 33594 RVA: 0x002AF9F4 File Offset: 0x002ADBF4
		public static Aabb FromPoint(Vector3 p)
		{
			Aabb empty = Aabb.Empty;
			empty.Include(p);
			return empty;
		}

		// Token: 0x0600833B RID: 33595 RVA: 0x002AFA10 File Offset: 0x002ADC10
		public static Aabb FromPoints(Vector3 a, Vector3 b)
		{
			Aabb empty = Aabb.Empty;
			empty.Include(a);
			empty.Include(b);
			return empty;
		}

		// Token: 0x0600833C RID: 33596 RVA: 0x002AFA34 File Offset: 0x002ADC34
		public Aabb(Vector3 min, Vector3 max)
		{
			this.Min = min;
			this.Max = max;
		}

		// Token: 0x0600833D RID: 33597 RVA: 0x002AFA44 File Offset: 0x002ADC44
		public void Include(Vector3 p)
		{
			this.MinX = Mathf.Min(this.MinX, p.x);
			this.MinY = Mathf.Min(this.MinY, p.y);
			this.MinZ = Mathf.Min(this.MinZ, p.z);
			this.MaxX = Mathf.Max(this.MaxX, p.x);
			this.MaxY = Mathf.Max(this.MaxY, p.y);
			this.MaxZ = Mathf.Max(this.MaxZ, p.z);
		}

		// Token: 0x0600833E RID: 33598 RVA: 0x002AFADC File Offset: 0x002ADCDC
		public bool Contains(Vector3 p)
		{
			return this.MinX <= p.x && this.MinY <= p.y && this.MinZ <= p.z && this.MaxX >= p.x && this.MaxY >= p.y && this.MaxZ >= p.z;
		}

		// Token: 0x0600833F RID: 33599 RVA: 0x002AFB42 File Offset: 0x002ADD42
		public bool ContainsX(Vector3 p)
		{
			return this.MinX <= p.x && this.MaxX >= p.x;
		}

		// Token: 0x06008340 RID: 33600 RVA: 0x002AFB65 File Offset: 0x002ADD65
		public bool ContainsY(Vector3 p)
		{
			return this.MinY <= p.y && this.MaxY >= p.y;
		}

		// Token: 0x06008341 RID: 33601 RVA: 0x002AFB88 File Offset: 0x002ADD88
		public bool ContainsZ(Vector3 p)
		{
			return this.MinZ <= p.z && this.MaxZ >= p.z;
		}

		// Token: 0x06008342 RID: 33602 RVA: 0x002AFBAC File Offset: 0x002ADDAC
		public bool Intersects(Aabb rhs)
		{
			return this.MinX <= rhs.MaxX && this.MinY <= rhs.MaxY && this.MinZ <= rhs.MaxZ && this.MaxX >= rhs.MinX && this.MaxY >= rhs.MinY && this.MaxZ >= rhs.MinZ;
		}

		// Token: 0x06008343 RID: 33603 RVA: 0x002AFC18 File Offset: 0x002ADE18
		public bool Intersects(ref BoingEffector.Params effector)
		{
			if (!effector.Bits.IsBitSet(0))
			{
				return this.Intersects(Aabb.FromPoint(effector.CurrPosition).Expand(effector.Radius));
			}
			return this.Intersects(Aabb.FromPoints(effector.PrevPosition, effector.CurrPosition).Expand(effector.Radius));
		}

		// Token: 0x06008344 RID: 33604 RVA: 0x002AFC78 File Offset: 0x002ADE78
		public Aabb Expand(float amount)
		{
			this.MinX -= amount;
			this.MinY -= amount;
			this.MinZ -= amount;
			this.MaxX += amount;
			this.MaxY += amount;
			this.MaxZ += amount;
			return this;
		}

		// Token: 0x04009458 RID: 37976
		public Vector3 Min;

		// Token: 0x04009459 RID: 37977
		public Vector3 Max;
	}
}
