using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x0200075F RID: 1887
public class GRBay : MonoBehaviour
{
	// Token: 0x06002FE0 RID: 12256 RVA: 0x001044E6 File Offset: 0x001026E6
	private void Awake()
	{
		if (this.playerName != null)
		{
			this.playerName.text = null;
		}
		if (this.maxDropText != null)
		{
			this.maxDropText.text = null;
		}
	}

	// Token: 0x06002FE1 RID: 12257 RVA: 0x0010451C File Offset: 0x0010271C
	public void Setup(GhostReactor reactor)
	{
		this.reactor = reactor;
		if (this.shuttleLoc != GRShuttleGroupLoc.Invalid && this.shuttleIndex >= 0 && this.shuttleIndex < 10)
		{
			this.unlockShuttle = GRElevatorManager._instance.GetPlayerShuttle(this.shuttleLoc, this.shuttleIndex);
			if (this.unlockShuttle != null)
			{
				this.unlockShuttle.SetBay(this);
			}
		}
		this.Refresh();
	}

	// Token: 0x06002FE2 RID: 12258 RVA: 0x00104588 File Offset: 0x00102788
	public void SetOpen(bool open)
	{
		if (this.hideWhenOpen != null)
		{
			for (int i = 0; i < this.hideWhenOpen.Count; i++)
			{
				if (this.hideWhenOpen[i] != null)
				{
					this.hideWhenOpen[i].SetActive(!open);
				}
				else
				{
					Debug.LogErrorFormat("Why is hideWhenOpen null {0} at {1}", new object[]
					{
						base.gameObject.name,
						i
					});
				}
			}
		}
		else
		{
			Debug.LogErrorFormat("Why is hideWhenOpen null {0}", new object[] { base.gameObject.name });
		}
		if (this.hideWhenClosed != null)
		{
			for (int j = 0; j < this.hideWhenClosed.Count; j++)
			{
				if (this.hideWhenClosed[j] != null)
				{
					this.hideWhenClosed[j].SetActive(open);
				}
				else
				{
					Debug.LogErrorFormat("Why is hideWhenClosed null {0} at {1} ", new object[]
					{
						base.gameObject.name,
						j
					});
				}
			}
		}
		else
		{
			Debug.LogErrorFormat("Why is hideWhenClosed null {0}", new object[] { base.gameObject.name });
		}
		if (this.bayDoorAnimation != null && this.isOpen != open)
		{
			if (open)
			{
				this.bayDoorAnimation.Play("BayDoor_Open");
				this.bayDoorAnimation.PlayQueued("BayDoor_Open_Idle");
			}
			else
			{
				this.bayDoorAnimation.Play("BayDoor_Close");
				this.bayDoorAnimation.PlayQueued("BayDoor_Close_Idle");
			}
		}
		this.isOpen = open;
	}

	// Token: 0x06002FE3 RID: 12259 RVA: 0x0010471C File Offset: 0x0010291C
	public void Refresh()
	{
		bool flag = true;
		if (this.unlockShuttle != null)
		{
			NetPlayer owner = this.unlockShuttle.GetOwner();
			bool flag2 = owner != null && this.unlockShuttle.IsPodUnlocked();
			flag = this.unlockShuttle.GetState() == GRShuttleState.Docked && flag2;
			if (this.playerName != null)
			{
				this.playerName.text = ((!flag2) ? null : owner.SanitizedNickName);
			}
			if (this.maxDropText != null)
			{
				int num = this.unlockShuttle.GetMaxDropFloor() + 1;
				this.maxDropText.text = ((!flag2) ? null : num.ToString());
			}
			for (int i = 0; i < this.showWhenOwned.Count; i++)
			{
				this.showWhenOwned[i].SetActive(flag2);
			}
			for (int j = 0; j < this.showWhenNotOwned.Count; j++)
			{
				this.showWhenNotOwned[j].SetActive(!flag2);
			}
		}
		else if (this.unlockByDrillLevel > 0)
		{
			flag = (this.reactor != null && this.reactor.GetDepthLevel() >= this.unlockByDrillLevel) || GhostReactorManager.bayUnlockEnabled;
		}
		this.SetOpen(flag);
	}

	// Token: 0x04003D56 RID: 15702
	public List<GameObject> hideWhenOpen;

	// Token: 0x04003D57 RID: 15703
	public List<GameObject> hideWhenClosed;

	// Token: 0x04003D58 RID: 15704
	public Animation bayDoorAnimation;

	// Token: 0x04003D59 RID: 15705
	private bool isOpen;

	// Token: 0x04003D5A RID: 15706
	public TMP_Text playerName;

	// Token: 0x04003D5B RID: 15707
	public TMP_Text maxDropText;

	// Token: 0x04003D5C RID: 15708
	public List<GameObject> showWhenOwned;

	// Token: 0x04003D5D RID: 15709
	public List<GameObject> showWhenNotOwned;

	// Token: 0x04003D5E RID: 15710
	public int unlockByDrillLevel = -1;

	// Token: 0x04003D5F RID: 15711
	public GRShuttleGroupLoc shuttleLoc = GRShuttleGroupLoc.Invalid;

	// Token: 0x04003D60 RID: 15712
	public int shuttleIndex = -1;

	// Token: 0x04003D61 RID: 15713
	[NonSerialized]
	public bool debugForceUnlockedByLevel;

	// Token: 0x04003D62 RID: 15714
	private GRShuttle unlockShuttle;

	// Token: 0x04003D63 RID: 15715
	private GhostReactor reactor;
}
