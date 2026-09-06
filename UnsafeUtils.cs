using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Token: 0x02000DF7 RID: 3575
public static class UnsafeUtils
{
	// Token: 0x060057A5 RID: 22437 RVA: 0x001C9538 File Offset: 0x001C7738
	public unsafe static readonly ref T[] GetInternalArray<T>(this List<T> list)
	{
		if (list == null)
		{
			return Unsafe.NullRef<T[]>();
		}
		return ref Unsafe.As<List<T>, StrongBox<T[]>>(ref list)->Value;
	}

	// Token: 0x060057A6 RID: 22438 RVA: 0x001C9550 File Offset: 0x001C7750
	public unsafe static readonly ref T[] GetInvocationListUnsafe<T>(this T @delegate) where T : MulticastDelegate
	{
		if (@delegate == null)
		{
			return Unsafe.NullRef<T[]>();
		}
		return Unsafe.As<Delegate[], T[]>(ref Unsafe.As<T, UnsafeUtils._MultiDelegateFields>(ref @delegate)->delegates);
	}

	// Token: 0x02000DF8 RID: 3576
	[StructLayout(LayoutKind.Sequential)]
	private class _MultiDelegateFields : UnsafeUtils._DelegateFields
	{
		// Token: 0x0400681C RID: 26652
		public Delegate[] delegates;
	}

	// Token: 0x02000DF9 RID: 3577
	[StructLayout(LayoutKind.Sequential)]
	private class _DelegateFields
	{
		// Token: 0x0400681D RID: 26653
		public IntPtr method_ptr;

		// Token: 0x0400681E RID: 26654
		public IntPtr invoke_impl;

		// Token: 0x0400681F RID: 26655
		public object m_target;

		// Token: 0x04006820 RID: 26656
		public IntPtr method;

		// Token: 0x04006821 RID: 26657
		public IntPtr delegate_trampoline;

		// Token: 0x04006822 RID: 26658
		public IntPtr extra_arg;

		// Token: 0x04006823 RID: 26659
		public IntPtr method_code;

		// Token: 0x04006824 RID: 26660
		public IntPtr interp_method;

		// Token: 0x04006825 RID: 26661
		public IntPtr interp_invoke_impl;

		// Token: 0x04006826 RID: 26662
		public MethodInfo method_info;

		// Token: 0x04006827 RID: 26663
		public MethodInfo original_method_info;

		// Token: 0x04006828 RID: 26664
		public UnsafeUtils._DelegateData data;

		// Token: 0x04006829 RID: 26665
		public bool method_is_virtual;
	}

	// Token: 0x02000DFA RID: 3578
	[StructLayout(LayoutKind.Sequential)]
	private class _DelegateData
	{
		// Token: 0x0400682A RID: 26666
		public Type target_type;

		// Token: 0x0400682B RID: 26667
		public string method_name;

		// Token: 0x0400682C RID: 26668
		public bool curried_first_arg;
	}
}
