using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005AE RID: 1454
[RequireComponent(typeof(TransferrableObject))]
public class UnityEventOnGrab : MonoBehaviour
{
	// Token: 0x060024D6 RID: 9430 RVA: 0x000C5FAC File Offset: 0x000C41AC
	private void Awake()
	{
		TransferrableObject componentInParent = base.GetComponentInParent<TransferrableObject>();
		Behaviour[] behavioursEnabledOnlyWhileHeld = componentInParent.behavioursEnabledOnlyWhileHeld;
		Behaviour[] array = new Behaviour[behavioursEnabledOnlyWhileHeld.Length + 1];
		for (int i = 0; i < behavioursEnabledOnlyWhileHeld.Length; i++)
		{
			array[i] = behavioursEnabledOnlyWhileHeld[i];
		}
		array[behavioursEnabledOnlyWhileHeld.Length] = this;
		componentInParent.behavioursEnabledOnlyWhileHeld = array;
	}

	// Token: 0x060024D7 RID: 9431 RVA: 0x000C5FF3 File Offset: 0x000C41F3
	private void OnEnable()
	{
		UnityEvent unityEvent = this.onGrab;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x060024D8 RID: 9432 RVA: 0x000C6005 File Offset: 0x000C4205
	private void OnDisable()
	{
		UnityEvent unityEvent = this.onRelease;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x0400305F RID: 12383
	[SerializeField]
	private UnityEvent onGrab;

	// Token: 0x04003060 RID: 12384
	[SerializeField]
	private UnityEvent onRelease;
}
