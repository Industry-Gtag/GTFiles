using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

// Token: 0x020007B1 RID: 1969
public class GREntityDebugCanvas : MonoBehaviour
{
	// Token: 0x0600326E RID: 12910 RVA: 0x00114684 File Offset: 0x00112884
	private void Awake()
	{
		this.builder = new StringBuilder(50);
	}

	// Token: 0x0600326F RID: 12911 RVA: 0x00114694 File Offset: 0x00112894
	private void Start()
	{
		if (this.text == null && this.textPanelPrefab != null)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.textPanelPrefab, base.transform.position + this.prefabAttachOffset, Quaternion.identity, base.transform);
			this.text = gameObject.GetComponent<TMP_Text>();
		}
		if (this.text != null)
		{
			this.text.fontSize = this.fontSize;
			this.text.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003270 RID: 12912 RVA: 0x00114728 File Offset: 0x00112928
	private bool UpdateActive()
	{
		bool entityDebugEnabled = GhostReactorManager.entityDebugEnabled;
		if (this.text != null)
		{
			this.text.gameObject.SetActive(entityDebugEnabled);
		}
		return entityDebugEnabled;
	}

	// Token: 0x06003271 RID: 12913 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Update()
	{
	}

	// Token: 0x06003272 RID: 12914 RVA: 0x0011475C File Offset: 0x0011295C
	private void UpdateText()
	{
		if (this.text)
		{
			this.builder.Clear();
			List<IGameEntityDebugComponent> list = new List<IGameEntityDebugComponent>();
			base.GetComponents<IGameEntityDebugComponent>(list);
			foreach (IGameEntityDebugComponent gameEntityDebugComponent in list)
			{
				List<string> list2 = new List<string>();
				gameEntityDebugComponent.GetDebugTextLines(out list2);
				foreach (string text in list2)
				{
					this.builder.AppendLine(text);
				}
			}
			this.text.text = this.builder.ToString();
		}
	}

	// Token: 0x04004153 RID: 16723
	[SerializeField]
	public TMP_Text text;

	// Token: 0x04004154 RID: 16724
	public GameObject textPanelPrefab;

	// Token: 0x04004155 RID: 16725
	public Vector3 prefabAttachOffset = new Vector3(0f, 0.5f, 0f);

	// Token: 0x04004156 RID: 16726
	public float fontSize = 100f;

	// Token: 0x04004157 RID: 16727
	private StringBuilder builder;
}
