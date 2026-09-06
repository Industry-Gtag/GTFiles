using System;
using System.Collections;
using GorillaLocomotion.Gameplay;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.ScavengerHunt
{
	// Token: 0x02001006 RID: 4102
	public class ScavengerTarget : MonoBehaviour, IGorillaGrabable
	{
		// Token: 0x06006632 RID: 26162 RVA: 0x0020E0B6 File Offset: 0x0020C2B6
		private void Awake()
		{
			base.StartCoroutine(this.ConnectToScavengerManager());
		}

		// Token: 0x06006633 RID: 26163 RVA: 0x0020E0C5 File Offset: 0x0020C2C5
		private IEnumerator ConnectToScavengerManager()
		{
			int num;
			for (int i = 0; i < 30; i = num)
			{
				if (!(ScavengerManager.Instance == null))
				{
					ScavengerManager.Instance.RegisterTarget(this);
					this._manager = ScavengerManager.Instance;
					yield break;
				}
				yield return null;
				num = i + 1;
			}
			Object.Destroy(this);
			throw new Exception(string.Format("No ScavengerManager found within {0} frames of attempts.", 30));
			yield break;
		}

		// Token: 0x06006634 RID: 26164 RVA: 0x0020E0D4 File Offset: 0x0020C2D4
		public void Collect()
		{
			this._manager.Collect(this);
		}

		// Token: 0x06006635 RID: 26165 RVA: 0x00023F0C File Offset: 0x0002210C
		public bool MomentaryGrabOnly()
		{
			return true;
		}

		// Token: 0x06006636 RID: 26166 RVA: 0x0020E0E2 File Offset: 0x0020C2E2
		public bool CanBeGrabbed(GorillaGrabber grabber)
		{
			return !this._manager.IsCollected(this);
		}

		// Token: 0x06006637 RID: 26167 RVA: 0x0020E0F3 File Offset: 0x0020C2F3
		public void OnGrabbed(GorillaGrabber grabber, out Transform grabbedTransform, out Vector3 localGrabbedPosition)
		{
			this.Collect();
			grabbedTransform = base.transform;
			localGrabbedPosition = base.transform.InverseTransformPoint(grabber.transform.position);
		}

		// Token: 0x06006638 RID: 26168 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnGrabReleased(GorillaGrabber grabber)
		{
		}

		// Token: 0x0600663A RID: 26170 RVA: 0x00014B5B File Offset: 0x00012D5B
		string IGorillaGrabable.get_name()
		{
			return base.name;
		}

		// Token: 0x0400752A RID: 29994
		public string HuntName;

		// Token: 0x0400752B RID: 29995
		public string TargetName;

		// Token: 0x0400752C RID: 29996
		public string DisplayName;

		// Token: 0x0400752D RID: 29997
		public UnityEvent[] TargetCollected;

		// Token: 0x0400752E RID: 29998
		public UnityEvent<ScavengerTarget>[] TargetCollectedArg;

		// Token: 0x0400752F RID: 29999
		private ScavengerManager _manager;
	}
}
