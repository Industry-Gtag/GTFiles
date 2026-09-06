using System;
using UnityEngine;

// Token: 0x02000600 RID: 1536
public class GameBallPlayer : MonoBehaviour
{
	// Token: 0x06002646 RID: 9798 RVA: 0x000CAD0C File Offset: 0x000C8F0C
	private void Awake()
	{
		this.hands = new GameBallPlayer.HandData[2];
		for (int i = 0; i < 2; i++)
		{
			this.ClearGrabbed(i);
		}
		this.teamId = -1;
	}

	// Token: 0x06002647 RID: 9799 RVA: 0x000CAD40 File Offset: 0x000C8F40
	public void CleanupPlayer()
	{
		MonkeBallPlayer component = base.GetComponent<MonkeBallPlayer>();
		if (component != null)
		{
			component.currGoalZone = null;
			for (int i = 0; i < MonkeBallGame.Instance.goalZones.Count; i++)
			{
				MonkeBallGame.Instance.goalZones[i].CleanupPlayer(component);
			}
		}
	}

	// Token: 0x06002648 RID: 9800 RVA: 0x000CAD94 File Offset: 0x000C8F94
	public void SetGrabbed(GameBallId gameBallId, int handIndex)
	{
		if (gameBallId.IsValid())
		{
			this.ClearGrabbedIfHeld(gameBallId);
		}
		GameBallPlayer.HandData handData = this.hands[handIndex];
		handData.grabbedGameBallId = gameBallId;
		this.hands[handIndex] = handData;
	}

	// Token: 0x06002649 RID: 9801 RVA: 0x000CADD4 File Offset: 0x000C8FD4
	public void ClearGrabbedIfHeld(GameBallId gameBallId)
	{
		for (int i = 0; i < 2; i++)
		{
			if (this.hands[i].grabbedGameBallId == gameBallId)
			{
				this.ClearGrabbed(i);
			}
		}
	}

	// Token: 0x0600264A RID: 9802 RVA: 0x000CAE0D File Offset: 0x000C900D
	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameBallId.Invalid, handIndex);
	}

	// Token: 0x0600264B RID: 9803 RVA: 0x000CAE1C File Offset: 0x000C901C
	public void ClearAllGrabbed()
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			this.ClearGrabbed(i);
		}
	}

	// Token: 0x0600264C RID: 9804 RVA: 0x000CAE43 File Offset: 0x000C9043
	public void SetInGoalZone(bool inZone)
	{
		if (inZone)
		{
			this.inGoalZone++;
			return;
		}
		this.inGoalZone--;
	}

	// Token: 0x0600264D RID: 9805 RVA: 0x000CAE68 File Offset: 0x000C9068
	public bool IsHoldingBall()
	{
		return this.GetGameBallId().IsValid();
	}

	// Token: 0x0600264E RID: 9806 RVA: 0x000CAE83 File Offset: 0x000C9083
	public GameBallId GetGameBallId(int handIndex)
	{
		return this.hands[handIndex].grabbedGameBallId;
	}

	// Token: 0x0600264F RID: 9807 RVA: 0x000CAE98 File Offset: 0x000C9098
	public int FindHandIndex(GameBallId gameBallId)
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.hands[i].grabbedGameBallId == gameBallId)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06002650 RID: 9808 RVA: 0x000CAED4 File Offset: 0x000C90D4
	public GameBallId GetGameBallId()
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.hands[i].grabbedGameBallId.IsValid())
			{
				return this.hands[i].grabbedGameBallId;
			}
		}
		return GameBallId.Invalid;
	}

	// Token: 0x06002651 RID: 9809 RVA: 0x000CAF23 File Offset: 0x000C9123
	public bool IsLocalPlayer()
	{
		return VRRigCache.Instance.localRig.Creator.ActorNumber == this.rig.OwningNetPlayer.ActorNumber;
	}

	// Token: 0x06002652 RID: 9810 RVA: 0x000CAF4B File Offset: 0x000C914B
	public static bool IsLeftHand(int handIndex)
	{
		return handIndex == 0;
	}

	// Token: 0x06002653 RID: 9811 RVA: 0x000CAF51 File Offset: 0x000C9151
	public static int GetHandIndex(bool leftHand)
	{
		if (!leftHand)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06002654 RID: 9812 RVA: 0x000CAF5C File Offset: 0x000C915C
	public static VRRig GetRig(int actorNumber)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
		RigContainer rigContainer;
		if (player == null || player.IsNull || !VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return null;
		}
		return rigContainer.Rig;
	}

	// Token: 0x06002655 RID: 9813 RVA: 0x000CAF98 File Offset: 0x000C9198
	public static GameBallPlayer GetGamePlayer(int actorNumber)
	{
		if (actorNumber < 0)
		{
			return null;
		}
		VRRig vrrig = GameBallPlayer.GetRig(actorNumber);
		if (vrrig == null)
		{
			return null;
		}
		return vrrig.GetComponent<GameBallPlayer>();
	}

	// Token: 0x06002656 RID: 9814 RVA: 0x000CAFC4 File Offset: 0x000C91C4
	public static GameBallPlayer GetGamePlayer(Collider collider, bool bodyOnly = false)
	{
		Transform transform = collider.transform;
		while (transform != null)
		{
			GameBallPlayer component = transform.GetComponent<GameBallPlayer>();
			if (component != null)
			{
				return component;
			}
			if (bodyOnly)
			{
				break;
			}
			transform = transform.parent;
		}
		return null;
	}

	// Token: 0x040031D1 RID: 12753
	public VRRig rig;

	// Token: 0x040031D2 RID: 12754
	public int teamId;

	// Token: 0x040031D3 RID: 12755
	private GameBallPlayer.HandData[] hands;

	// Token: 0x040031D4 RID: 12756
	public const int MAX_HANDS = 2;

	// Token: 0x040031D5 RID: 12757
	public const int LEFT_HAND = 0;

	// Token: 0x040031D6 RID: 12758
	public const int RIGHT_HAND = 1;

	// Token: 0x040031D7 RID: 12759
	private int inGoalZone;

	// Token: 0x02000601 RID: 1537
	private struct HandData
	{
		// Token: 0x040031D8 RID: 12760
		public GameBallId grabbedGameBallId;
	}
}
