using System;
using System.Collections.Generic;

// Token: 0x02000333 RID: 819
public static class DelegateExtensions
{
	// Token: 0x06001445 RID: 5189 RVA: 0x0006D8F4 File Offset: 0x0006BAF4
	public static List<string> ToStringList(this Delegate[] invocationList)
	{
		List<string> list = new List<string>();
		if (invocationList != null)
		{
			foreach (Delegate @delegate in invocationList)
			{
				string name = @delegate.Method.Name;
				string text = ((@delegate.Target != null) ? @delegate.Target.GetType().FullName : "Static Method");
				list.Add(text + "." + name);
			}
		}
		return list;
	}

	// Token: 0x06001446 RID: 5190 RVA: 0x0006D961 File Offset: 0x0006BB61
	public static string ToText(this Delegate[] invocationList)
	{
		return string.Join(", ", invocationList.ToStringList());
	}
}
