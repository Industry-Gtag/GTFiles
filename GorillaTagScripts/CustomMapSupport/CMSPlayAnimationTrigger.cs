using System;
using System.Collections.Generic;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000FBC RID: 4028
	public class CMSPlayAnimationTrigger : CMSTrigger
	{
		// Token: 0x06006422 RID: 25634 RVA: 0x00203274 File Offset: 0x00201474
		public override void CopyTriggerSettings(TriggerSettings settings)
		{
			if (settings.GetType() == typeof(PlayAnimationTriggerSettings))
			{
				PlayAnimationTriggerSettings playAnimationTriggerSettings = (PlayAnimationTriggerSettings)settings;
				this.animatedObjects = playAnimationTriggerSettings.animatedObjects;
				this.animationName = playAnimationTriggerSettings.animationName;
			}
			for (int i = this.animatedObjects.Count - 1; i >= 0; i--)
			{
				if (this.animatedObjects[i].IsNull())
				{
					this.animatedObjects.RemoveAt(i);
				}
			}
			base.CopyTriggerSettings(settings);
		}

		// Token: 0x06006423 RID: 25635 RVA: 0x002032F8 File Offset: 0x002014F8
		public override void Trigger(double triggerTime = -1.0, bool originatedLocally = false, bool ignoreTriggerCount = false)
		{
			base.Trigger(triggerTime, originatedLocally, ignoreTriggerCount);
			foreach (GameObject gameObject in this.animatedObjects)
			{
				Animator component = gameObject.GetComponent<Animator>();
				if (component.IsNotNull())
				{
					component.Play(this.animationName);
				}
			}
		}

		// Token: 0x040072F2 RID: 29426
		public List<GameObject> animatedObjects = new List<GameObject>();

		// Token: 0x040072F3 RID: 29427
		public string animationName = "";
	}
}
