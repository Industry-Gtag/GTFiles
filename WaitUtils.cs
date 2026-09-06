using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

// Token: 0x02000E00 RID: 3584
public static class WaitUtils
{
	// Token: 0x060057D9 RID: 22489 RVA: 0x001C9F87 File Offset: 0x001C8187
	public static WaitForSeconds WaitForSeconds(float seconds)
	{
		WaitUtils._waitForSecondsSetter(seconds);
		return WaitUtils._waitForSeconds;
	}

	// Token: 0x04006834 RID: 26676
	private static WaitForSeconds _waitForSeconds = new WaitForSeconds(1f);

	// Token: 0x04006835 RID: 26677
	private static ParameterExpression _param = Expression.Parameter(typeof(float));

	// Token: 0x04006836 RID: 26678
	private static Action<float> _waitForSecondsSetter = Expression.Lambda<Action<float>>(Expression.Assign(Expression.Field(Expression.Constant(WaitUtils._waitForSeconds, typeof(WaitForSeconds)), typeof(WaitForSeconds).GetField("m_Seconds", BindingFlags.Instance | BindingFlags.NonPublic)), WaitUtils._param), new ParameterExpression[] { WaitUtils._param }).Compile();
}
