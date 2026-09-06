using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.ScavengerHunt
{
	// Token: 0x02001008 RID: 4104
	public class ScavengerListener : MonoBehaviour
	{
		// Token: 0x06006641 RID: 26177 RVA: 0x0020E1E0 File Offset: 0x0020C3E0
		private async void OnEnable()
		{
			ScavengerManager.OnHuntCompleted = (Action<string, bool>)Delegate.Combine(ScavengerManager.OnHuntCompleted, new Action<string, bool>(this.OnHuntCompleted));
			ScavengerManager.OnTargetCollected = (Action<string, string, bool>)Delegate.Combine(ScavengerManager.OnTargetCollected, new Action<string, string, bool>(this.OnTargetCollected));
			while (ScavengerManager.Instance == null)
			{
				await Task.Delay(100);
			}
			this.hunt = ScavengerManager.Instance.GetHunt(this.huntName);
			if (this.hunt == null)
			{
				Debug.LogError(string.Concat(new string[] { "ScavengerManager on ", base.name, " couldn't find a hunt named ", this.huntName, ". Check your spelling!" }));
				ScavengerManager.OnHuntCompleted = (Action<string, bool>)Delegate.Remove(ScavengerManager.OnHuntCompleted, new Action<string, bool>(this.OnHuntCompleted));
				ScavengerManager.OnTargetCollected = (Action<string, string, bool>)Delegate.Remove(ScavengerManager.OnTargetCollected, new Action<string, string, bool>(this.OnTargetCollected));
			}
			else if (this.hunt.IsCompleted)
			{
				this.OnHuntCompleted(this.huntName, false);
			}
			else
			{
				this.setStatusText();
			}
		}

		// Token: 0x06006642 RID: 26178 RVA: 0x0020E218 File Offset: 0x0020C418
		private void setStatusText()
		{
			if (this.displayHuntStatus.Length != 0)
			{
				string text = this.displayHuntStatusHeading;
				for (int i = 0; i < this.hunt.Targets.Count; i++)
				{
					string text2 = this.hunt.Targets[i].DisplayName;
					if (text2.IsNullOrEmpty())
					{
						text2 = this.hunt.Targets[i].TargetName;
					}
					text = string.Concat(new string[]
					{
						text,
						"\n[",
						this.hunt.IsCollected(this.hunt.Targets[i]) ? "X" : " ",
						"] ",
						text2
					});
				}
				for (int j = 0; j < this.displayHuntStatus.Length; j++)
				{
					this.displayHuntStatus[j].text = text;
				}
			}
		}

		// Token: 0x06006643 RID: 26179 RVA: 0x0020E301 File Offset: 0x0020C501
		private void OnTargetCollected(string huntName, string itemName, bool firstCompletetion)
		{
			if (huntName != this.huntName)
			{
				return;
			}
			UnityEvent<string> onCollected = this.OnCollected;
			if (onCollected != null)
			{
				onCollected.Invoke(itemName);
			}
			if (firstCompletetion)
			{
				this.OnCollectedRealtime.Invoke(itemName);
			}
			this.setStatusText();
		}

		// Token: 0x06006644 RID: 26180 RVA: 0x0020E33C File Offset: 0x0020C53C
		private void OnHuntCompleted(string huntName, bool firstCompletetion)
		{
			if (huntName != this.huntName)
			{
				return;
			}
			UnityEvent onCompleted = this.OnCompleted;
			if (onCompleted != null)
			{
				onCompleted.Invoke();
			}
			if (firstCompletetion)
			{
				this.OnCompletedRealtime.Invoke();
			}
			ScavengerManager.OnHuntCompleted = (Action<string, bool>)Delegate.Remove(ScavengerManager.OnHuntCompleted, new Action<string, bool>(this.OnHuntCompleted));
			ScavengerManager.OnTargetCollected = (Action<string, string, bool>)Delegate.Remove(ScavengerManager.OnTargetCollected, new Action<string, string, bool>(this.OnTargetCollected));
			this.setStatusText();
		}

		// Token: 0x06006645 RID: 26181 RVA: 0x0020E3C0 File Offset: 0x0020C5C0
		private void OnDisable()
		{
			ScavengerManager.OnHuntCompleted = (Action<string, bool>)Delegate.Remove(ScavengerManager.OnHuntCompleted, new Action<string, bool>(this.OnHuntCompleted));
			ScavengerManager.OnTargetCollected = (Action<string, string, bool>)Delegate.Remove(ScavengerManager.OnTargetCollected, new Action<string, string, bool>(this.OnTargetCollected));
		}

		// Token: 0x06006646 RID: 26182 RVA: 0x0020E410 File Offset: 0x0020C610
		private void OnDestroy()
		{
			ScavengerManager.OnHuntCompleted = (Action<string, bool>)Delegate.Remove(ScavengerManager.OnHuntCompleted, new Action<string, bool>(this.OnHuntCompleted));
			ScavengerManager.OnTargetCollected = (Action<string, string, bool>)Delegate.Remove(ScavengerManager.OnTargetCollected, new Action<string, string, bool>(this.OnTargetCollected));
		}

		// Token: 0x04007534 RID: 30004
		[SerializeField]
		private string huntName;

		// Token: 0x04007535 RID: 30005
		[SerializeField]
		private UnityEvent OnCompleted;

		// Token: 0x04007536 RID: 30006
		[SerializeField]
		private UnityEvent OnCompletedRealtime;

		// Token: 0x04007537 RID: 30007
		[SerializeField]
		private UnityEvent<string> OnCollected;

		// Token: 0x04007538 RID: 30008
		[SerializeField]
		private UnityEvent<string> OnCollectedRealtime;

		// Token: 0x04007539 RID: 30009
		[SerializeField]
		private TMP_Text[] displayHuntStatus;

		// Token: 0x0400753A RID: 30010
		[SerializeField]
		private string displayHuntStatusHeading;

		// Token: 0x0400753B RID: 30011
		private ScavengerManager.Hunt hunt;
	}
}
