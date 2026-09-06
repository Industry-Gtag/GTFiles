using System;
using System.Collections;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200121D RID: 4637
	public class CustomMapTestingScript : GorillaPressableButton
	{
		// Token: 0x06007590 RID: 30096 RVA: 0x002648D4 File Offset: 0x00262AD4
		public override void ButtonActivation()
		{
			base.ButtonActivation();
			base.StartCoroutine(this.ButtonPressed_Local());
		}

		// Token: 0x06007591 RID: 30097 RVA: 0x002648E9 File Offset: 0x00262AE9
		private IEnumerator ButtonPressed_Local()
		{
			this.isOn = true;
			this.UpdateColor();
			yield return new WaitForSeconds(this.debounceTime);
			this.isOn = false;
			this.UpdateColor();
			yield break;
		}
	}
}
