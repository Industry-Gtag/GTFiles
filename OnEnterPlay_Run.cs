using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000DCA RID: 3530
[AttributeUsage(AttributeTargets.Method)]
public class OnEnterPlay_Run : OnEnterPlay_Attribute
{
	// Token: 0x060056A0 RID: 22176 RVA: 0x001C453B File Offset: 0x001C273B
	public override void OnEnterPlay(MethodInfo method)
	{
		if (!method.IsStatic)
		{
			Debug.LogError(string.Format("Can't Run non-static method {0}.{1}", method.DeclaringType, method.Name));
			return;
		}
		method.Invoke(null, new object[0]);
	}
}
