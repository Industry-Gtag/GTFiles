using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.UI
{
	// Token: 0x02000FED RID: 4077
	public class GorillaKeyWrapper<TBinding> : MonoBehaviour where TBinding : Enum
	{
		// Token: 0x0600655D RID: 25949 RVA: 0x0020A48C File Offset: 0x0020868C
		public void Start()
		{
			if (!this.defineButtonsManually)
			{
				this.FindMatchingButtons(base.gameObject);
				return;
			}
			if (this.buttons.Count > 0)
			{
				for (int i = this.buttons.Count - 1; i >= 0; i--)
				{
					if (this.buttons[i].IsNull())
					{
						this.buttons.RemoveAt(i);
					}
					else
					{
						this.buttons[i].OnKeyButtonPressed.AddListener(new UnityAction<TBinding>(this.OnKeyButtonPressed));
					}
				}
			}
		}

		// Token: 0x0600655E RID: 25950 RVA: 0x0020A518 File Offset: 0x00208718
		public void OnDestroy()
		{
			for (int i = 0; i < this.buttons.Count; i++)
			{
				if (this.buttons[i].IsNotNull())
				{
					this.buttons[i].OnKeyButtonPressed.RemoveListener(new UnityAction<TBinding>(this.OnKeyButtonPressed));
				}
			}
		}

		// Token: 0x0600655F RID: 25951 RVA: 0x0020A570 File Offset: 0x00208770
		public void FindMatchingButtons(GameObject obj)
		{
			if (obj.IsNull())
			{
				return;
			}
			for (int i = 0; i < obj.transform.childCount; i++)
			{
				Transform child = obj.transform.GetChild(i);
				if (child.IsNotNull())
				{
					this.FindMatchingButtons(child.gameObject);
				}
			}
			GorillaKeyButton<TBinding> component = obj.GetComponent<GorillaKeyButton<TBinding>>();
			if (component.IsNotNull() && !this.buttons.Contains(component))
			{
				this.buttons.Add(component);
				component.OnKeyButtonPressed.AddListener(new UnityAction<TBinding>(this.OnKeyButtonPressed));
			}
		}

		// Token: 0x06006560 RID: 25952 RVA: 0x0020A5FD File Offset: 0x002087FD
		private void OnKeyButtonPressed(TBinding binding)
		{
			UnityEvent<TBinding> onKeyPressed = this.OnKeyPressed;
			if (onKeyPressed == null)
			{
				return;
			}
			onKeyPressed.Invoke(binding);
		}

		// Token: 0x04007453 RID: 29779
		public UnityEvent<TBinding> OnKeyPressed = new UnityEvent<TBinding>();

		// Token: 0x04007454 RID: 29780
		public bool defineButtonsManually;

		// Token: 0x04007455 RID: 29781
		public List<GorillaKeyButton<TBinding>> buttons = new List<GorillaKeyButton<TBinding>>();
	}
}
