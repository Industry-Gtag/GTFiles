using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000DD0 RID: 3536
[AttributeUsage(AttributeTargets.Field)]
public class OnExitPlay_Set : OnExitPlay_Attribute
{
	// Token: 0x060056AD RID: 22189 RVA: 0x001C460C File Offset: 0x001C280C
	public OnExitPlay_Set(object value)
	{
		this.value = value;
	}

	// Token: 0x060056AE RID: 22190 RVA: 0x001C461B File Offset: 0x001C281B
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Set non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		field.SetValue(null, this.value);
	}

	// Token: 0x0400679B RID: 26523
	private object value;
}
