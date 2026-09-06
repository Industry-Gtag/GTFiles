using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000DD3 RID: 3539
[AttributeUsage(AttributeTargets.Method)]
public class OnExitPlay_Run : OnExitPlay_Attribute
{
	// Token: 0x060056B3 RID: 22195 RVA: 0x001C453B File Offset: 0x001C273B
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
