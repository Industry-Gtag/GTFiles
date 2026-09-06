using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x02000A3B RID: 2619
[Serializable]
public sealed class ComponentFunctionReference<TResult>
{
	// Token: 0x1700065C RID: 1628
	// (get) Token: 0x06004349 RID: 17225 RVA: 0x001663A4 File Offset: 0x001645A4
	public bool IsValid
	{
		get
		{
			return this._selection.component || !string.IsNullOrEmpty(this._selection.methodName);
		}
	}

	// Token: 0x0600434A RID: 17226 RVA: 0x001663CD File Offset: 0x001645CD
	private IEnumerable<ValueDropdownItem<ComponentFunctionReference<TResult>.MethodRef>> GetMethodOptions()
	{
		if (this._target == null)
		{
			yield break;
		}
		yield return new ValueDropdownItem<ComponentFunctionReference<TResult>.MethodRef>("NONE", default(ComponentFunctionReference<TResult>.MethodRef));
		Type type = typeof(GameObject);
		BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		foreach (MethodInfo methodInfo in type.GetMethods(flags))
		{
			if (methodInfo.GetParameters().Length == 0 && methodInfo.ReturnType == typeof(TResult))
			{
				string text = type.Name + "/" + methodInfo.Name;
				yield return new ValueDropdownItem<ComponentFunctionReference<TResult>.MethodRef>(text, new ComponentFunctionReference<TResult>.MethodRef(this._target, methodInfo));
			}
		}
		MethodInfo[] array = null;
		foreach (Component comp in this._target.GetComponents<Component>())
		{
			type = comp.GetType();
			foreach (MethodInfo methodInfo2 in type.GetMethods(flags))
			{
				if (methodInfo2.GetParameters().Length == 0 && methodInfo2.ReturnType == typeof(TResult))
				{
					string text2 = type.Name + "/" + methodInfo2.Name;
					yield return new ValueDropdownItem<ComponentFunctionReference<TResult>.MethodRef>(text2, new ComponentFunctionReference<TResult>.MethodRef(comp, methodInfo2));
				}
			}
			array = null;
			comp = null;
		}
		Component[] array2 = null;
		yield break;
	}

	// Token: 0x0600434B RID: 17227 RVA: 0x001663E0 File Offset: 0x001645E0
	public TResult Invoke()
	{
		if (this._cached == null)
		{
			this.Cache();
		}
		if (this._cached == null)
		{
			return default(TResult);
		}
		return this._cached();
	}

	// Token: 0x0600434C RID: 17228 RVA: 0x00166418 File Offset: 0x00164618
	public void Cache()
	{
		this._cached = null;
		if (this._selection.component == null || string.IsNullOrEmpty(this._selection.methodName))
		{
			return;
		}
		MethodInfo method = this._selection.component.GetType().GetMethod(this._selection.methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
		if (method != null)
		{
			this._cached = (Func<TResult>)Delegate.CreateDelegate(typeof(Func<TResult>), this._selection.component, method);
		}
	}

	// Token: 0x0400552C RID: 21804
	[SerializeField]
	private GameObject _target;

	// Token: 0x0400552D RID: 21805
	[SerializeField]
	private ComponentFunctionReference<TResult>.MethodRef _selection;

	// Token: 0x0400552E RID: 21806
	private Func<TResult> _cached;

	// Token: 0x02000A3C RID: 2620
	[Serializable]
	private struct MethodRef
	{
		// Token: 0x0600434E RID: 17230 RVA: 0x001664AB File Offset: 0x001646AB
		public MethodRef(Object obj, MethodInfo m)
		{
			this.component = obj;
			this.methodName = m.Name;
		}

		// Token: 0x0400552F RID: 21807
		public Object component;

		// Token: 0x04005530 RID: 21808
		public string methodName;
	}
}
