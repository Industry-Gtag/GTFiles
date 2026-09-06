using System;

namespace GorillaTag
{
	// Token: 0x02001226 RID: 4646
	public abstract class DelegateListProcessorPlusMinus<T1, T2> : ListProcessorAbstract<T2> where T1 : DelegateListProcessorPlusMinus<T1, T2>, new() where T2 : Delegate
	{
		// Token: 0x060075C1 RID: 30145 RVA: 0x0026509E File Offset: 0x0026329E
		protected DelegateListProcessorPlusMinus()
		{
		}

		// Token: 0x060075C2 RID: 30146 RVA: 0x002650A6 File Offset: 0x002632A6
		protected DelegateListProcessorPlusMinus(int capacity)
			: base(capacity)
		{
		}

		// Token: 0x060075C3 RID: 30147 RVA: 0x002650AF File Offset: 0x002632AF
		public static T1 operator +(DelegateListProcessorPlusMinus<T1, T2> left, T2 right)
		{
			if (left == null)
			{
				left = new T1();
			}
			if (right == null)
			{
				return (T1)((object)left);
			}
			left.Add(in right);
			return (T1)((object)left);
		}

		// Token: 0x060075C4 RID: 30148 RVA: 0x002650E0 File Offset: 0x002632E0
		public static T1 operator -(DelegateListProcessorPlusMinus<T1, T2> left, T2 right)
		{
			if (left == null)
			{
				return default(T1);
			}
			if (right == null)
			{
				return (T1)((object)left);
			}
			left.Remove(in right);
			return (T1)((object)left);
		}
	}
}
