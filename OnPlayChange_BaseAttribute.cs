using System;
using System.Reflection;

// Token: 0x02000DC5 RID: 3525
public class OnPlayChange_BaseAttribute : Attribute
{
	// Token: 0x06005695 RID: 22165 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void OnEnterPlay(FieldInfo field)
	{
	}

	// Token: 0x06005696 RID: 22166 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void OnEnterPlay(MethodInfo method)
	{
	}
}
