using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001C5 RID: 453
public class BlinkingText : MonoBehaviour
{
	// Token: 0x06000C0B RID: 3083 RVA: 0x00041C54 File Offset: 0x0003FE54
	private void Awake()
	{
		this.textComponent = base.GetComponent<Text>();
	}

	// Token: 0x06000C0C RID: 3084 RVA: 0x00041C64 File Offset: 0x0003FE64
	private void Update()
	{
		if (this.isOn && Time.time > this.lastTime + this.cycleTime * this.dutyCycle)
		{
			this.isOn = false;
			this.textComponent.enabled = false;
			return;
		}
		if (!this.isOn && Time.time > this.lastTime + this.cycleTime)
		{
			this.lastTime = Time.time;
			this.isOn = true;
			this.textComponent.enabled = true;
		}
	}

	// Token: 0x04000EB0 RID: 3760
	public float cycleTime;

	// Token: 0x04000EB1 RID: 3761
	public float dutyCycle;

	// Token: 0x04000EB2 RID: 3762
	private bool isOn;

	// Token: 0x04000EB3 RID: 3763
	private float lastTime;

	// Token: 0x04000EB4 RID: 3764
	private Text textComponent;
}
