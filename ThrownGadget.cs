using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000126 RID: 294
public class ThrownGadget : MonoBehaviour
{
	// Token: 0x1400000F RID: 15
	// (add) Token: 0x0600073E RID: 1854 RVA: 0x000292EC File Offset: 0x000274EC
	// (remove) Token: 0x0600073F RID: 1855 RVA: 0x00029324 File Offset: 0x00027524
	public event Action OnActivated;

	// Token: 0x14000010 RID: 16
	// (add) Token: 0x06000740 RID: 1856 RVA: 0x0002935C File Offset: 0x0002755C
	// (remove) Token: 0x06000741 RID: 1857 RVA: 0x00029394 File Offset: 0x00027594
	public event Action OnThrown;

	// Token: 0x14000011 RID: 17
	// (add) Token: 0x06000742 RID: 1858 RVA: 0x000293CC File Offset: 0x000275CC
	// (remove) Token: 0x06000743 RID: 1859 RVA: 0x00029404 File Offset: 0x00027604
	public event Action OnHitSurface;

	// Token: 0x06000744 RID: 1860 RVA: 0x00029439 File Offset: 0x00027639
	private void OnEnable()
	{
		this.isHeldLocal = false;
		this.lastThrowerLocal = false;
	}

	// Token: 0x06000745 RID: 1861 RVA: 0x00029449 File Offset: 0x00027649
	public bool IsHeld()
	{
		return this.gameEntity.heldByActorNumber != -1;
	}

	// Token: 0x06000746 RID: 1862 RVA: 0x0002945C File Offset: 0x0002765C
	public bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x06000747 RID: 1863 RVA: 0x00029475 File Offset: 0x00027675
	public bool IsHeldByAnother()
	{
		return this.IsHeld() && !this.IsHeldLocal();
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x0002948C File Offset: 0x0002768C
	private bool IsButtonHeld()
	{
		if (!this.IsHeldLocal())
		{
			return false;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return false;
		}
		if (gamePlayer == null)
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		return num != -1 && ControllerInputPoller.TriggerFloat(GamePlayer.IsLeftHand(num) ? XRNode.LeftHand : XRNode.RightHand) > 0.25f;
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x000294F4 File Offset: 0x000276F4
	public void Update()
	{
		bool flag = this.IsHeldLocal();
		if (flag)
		{
			this.lastThrowerLocal = true;
			this.UpdateActivation();
		}
		else if (this.isHeldLocal)
		{
			Action onThrown = this.OnThrown;
			if (onThrown != null)
			{
				onThrown();
			}
		}
		else if (this.IsHeldByAnother())
		{
			this.lastThrowerLocal = false;
		}
		this.isHeldLocal = flag;
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x0002954C File Offset: 0x0002774C
	private void UpdateActivation()
	{
		bool flag = this.IsButtonHeld();
		if (!this.activationButtonLastInput && flag)
		{
			Action onActivated = this.OnActivated;
			if (onActivated != null)
			{
				onActivated();
			}
		}
		this.activationButtonLastInput = flag;
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x00029585 File Offset: 0x00027785
	public void OnCollisionEnter(Collision collision)
	{
		if (this.lastThrowerLocal)
		{
			Action onHitSurface = this.OnHitSurface;
			if (onHitSurface == null)
			{
				return;
			}
			onHitSurface();
		}
	}

	// Token: 0x04000986 RID: 2438
	public GameEntity gameEntity;

	// Token: 0x0400098A RID: 2442
	private bool isHeldLocal;

	// Token: 0x0400098B RID: 2443
	private bool lastThrowerLocal;

	// Token: 0x0400098C RID: 2444
	private bool activationButtonLastInput;
}
