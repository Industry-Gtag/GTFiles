using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001CD RID: 461
public class IDCardScanner : MonoBehaviour
{
	// Token: 0x1400001B RID: 27
	// (add) Token: 0x06000C3B RID: 3131 RVA: 0x00042938 File Offset: 0x00040B38
	// (remove) Token: 0x06000C3C RID: 3132 RVA: 0x00042970 File Offset: 0x00040B70
	public event IDCardScanner.CardSwipeEvent OnPlayerCardSwipe;

	// Token: 0x06000C3D RID: 3133 RVA: 0x000429A8 File Offset: 0x00040BA8
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<ScannableIDCard>() != null)
		{
			UnityEvent unityEvent = this.onCardSwiped;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			GameEntity gameEntity = other.GetComponent<GameEntity>();
			if (gameEntity == null && other.attachedRigidbody != null)
			{
				gameEntity = other.attachedRigidbody.GetComponent<GameEntity>();
			}
			if (gameEntity != null && gameEntity.heldByActorNumber != -1)
			{
				bool flag = !this.requireSpecificPlayer || (this.restrictToPlayer != null && this.restrictToPlayer.ActorNumber == gameEntity.heldByActorNumber && gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);
				bool flag2 = !this.requireAuthority || gameEntity.manager.IsAuthority();
				if (flag && flag2)
				{
					UnityEvent<int> unityEvent2 = this.onCardSwipedByPlayer;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke(gameEntity.heldByActorNumber);
					}
					IDCardScanner.CardSwipeEvent onPlayerCardSwipe = this.OnPlayerCardSwipe;
					if (onPlayerCardSwipe == null)
					{
						return;
					}
					onPlayerCardSwipe(gameEntity.heldByActorNumber);
				}
			}
		}
	}

	// Token: 0x04000ED0 RID: 3792
	public UnityEvent onCardSwiped;

	// Token: 0x04000ED1 RID: 3793
	public UnityEvent<int> onCardSwipedByPlayer;

	// Token: 0x04000ED2 RID: 3794
	[Tooltip("Has to be risen externally, by the receiver of the card swipe")]
	public UnityEvent onSucceeded;

	// Token: 0x04000ED3 RID: 3795
	[Tooltip("Has to be risen externally, by the receiver of the card swipe")]
	public UnityEvent onFailed;

	// Token: 0x04000ED4 RID: 3796
	public bool requireSpecificPlayer;

	// Token: 0x04000ED5 RID: 3797
	public bool requireAuthority;

	// Token: 0x04000ED6 RID: 3798
	[NonSerialized]
	public NetPlayer restrictToPlayer;

	// Token: 0x020001CE RID: 462
	// (Invoke) Token: 0x06000C40 RID: 3136
	public delegate void CardSwipeEvent(int actorNumber);
}
