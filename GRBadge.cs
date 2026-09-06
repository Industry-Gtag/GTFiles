using System;
using System.Collections;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000759 RID: 1881
public class GRBadge : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x06002FBE RID: 12222 RVA: 0x00103DDC File Offset: 0x00101FDC
	public void OnEntityInit()
	{
		this.gameEntity.manager.ghostReactorManager.reactor.employeeBadges.LinkBadgeToDispenser(this, (long)((int)this.gameEntity.createData));
	}

	// Token: 0x06002FBF RID: 12223 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002FC0 RID: 12224 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002FC1 RID: 12225 RVA: 0x00103E0C File Offset: 0x0010200C
	private void OnDestroy()
	{
		GhostReactor ghostReactor = GhostReactor.Get(this.gameEntity);
		if (ghostReactor != null && ghostReactor.employeeBadges != null)
		{
			ghostReactor.employeeBadges.RemoveBadge(this);
		}
	}

	// Token: 0x06002FC2 RID: 12226 RVA: 0x00103E48 File Offset: 0x00102048
	public void Setup(NetPlayer player, int index)
	{
		this.gameEntity.onlyGrabActorNumber = player.ActorNumber;
		this.dispenserIndex = index;
		this.actorNr = player.ActorNumber;
		GRPlayer grplayer = GRPlayer.Get(player.ActorNumber);
		bool flag = (int)this.gameEntity.GetState() == 1;
		if (player.IsLocal)
		{
			flag |= Time.timeAsDouble < grplayer.lastLeftWithBadgeAttachedTime + 60.0;
		}
		if (grplayer != null && flag)
		{
			base.transform.position = grplayer.badgeBodyAnchor.position;
			grplayer.AttachBadge(this);
		}
		this.RefreshText(player);
	}

	// Token: 0x06002FC3 RID: 12227 RVA: 0x00103EE8 File Offset: 0x001020E8
	public void RefreshText(NetPlayer player)
	{
		this.playerName.text = player.SanitizedNickName;
		GRPlayer grplayer = GRPlayer.Get(player.ActorNumber);
		if (grplayer != null && this.lastRedeemedPoints != grplayer.CurrentProgression.redeemedPoints)
		{
			this.lastRedeemedPoints = grplayer.CurrentProgression.redeemedPoints;
			this.playerTitle.text = GhostReactorProgression.GetTitleName(grplayer.CurrentProgression.redeemedPoints);
			this.playerLevel.text = GhostReactorProgression.GetGrade(grplayer.CurrentProgression.redeemedPoints).ToString();
		}
	}

	// Token: 0x06002FC4 RID: 12228 RVA: 0x00103F80 File Offset: 0x00102180
	public void Hide()
	{
		this.badgeMesh.enabled = false;
		this.playerName.gameObject.SetActive(false);
		this.playerTitle.gameObject.SetActive(false);
		this.playerLevel.gameObject.SetActive(false);
	}

	// Token: 0x06002FC5 RID: 12229 RVA: 0x00103FCC File Offset: 0x001021CC
	public void UnHide()
	{
		this.badgeMesh.enabled = true;
		this.playerName.gameObject.SetActive(true);
		this.playerTitle.gameObject.SetActive(true);
		this.playerLevel.gameObject.SetActive(true);
	}

	// Token: 0x06002FC6 RID: 12230 RVA: 0x00104018 File Offset: 0x00102218
	public bool IsAttachedToPlayer()
	{
		return (int)this.gameEntity.GetState() == 1;
	}

	// Token: 0x06002FC7 RID: 12231 RVA: 0x0010402C File Offset: 0x0010222C
	public void StartRetracting()
	{
		this.gameEntity.RequestState(this.gameEntity.id, 1L);
		this.PlayAttachFx();
		if (this.retractCoroutine != null)
		{
			base.StopCoroutine(this.retractCoroutine);
		}
		this.retractCoroutine = base.StartCoroutine(this.RetractCoroutine());
	}

	// Token: 0x06002FC8 RID: 12232 RVA: 0x0010407D File Offset: 0x0010227D
	private IEnumerator RetractCoroutine()
	{
		base.transform.localRotation = Quaternion.identity;
		Vector3 vector = base.transform.localPosition;
		for (float num = vector.sqrMagnitude; num > 1E-05f; num = vector.sqrMagnitude)
		{
			vector = Vector3.MoveTowards(vector, Vector3.zero, this.retractSpeed * Time.deltaTime);
			base.transform.localPosition = vector;
			yield return null;
			vector = base.transform.localPosition;
		}
		base.transform.localPosition = Vector3.zero;
		yield break;
	}

	// Token: 0x06002FC9 RID: 12233 RVA: 0x0010408C File Offset: 0x0010228C
	private void PlayAttachFx()
	{
		if (this.audioSource != null)
		{
			this.audioSource.volume = this.badgeAttachSoundVolume;
			this.audioSource.clip = this.badgeAttachSound;
			this.audioSource.Play();
		}
	}

	// Token: 0x04003D2C RID: 15660
	private const float RESTORE_BADGE_TO_DOCK_WINDOW = 60f;

	// Token: 0x04003D2D RID: 15661
	[SerializeField]
	private GameEntity gameEntity;

	// Token: 0x04003D2E RID: 15662
	[SerializeField]
	public TMP_Text playerName;

	// Token: 0x04003D2F RID: 15663
	[SerializeField]
	public TMP_Text playerTitle;

	// Token: 0x04003D30 RID: 15664
	[SerializeField]
	public TMP_Text playerLevel;

	// Token: 0x04003D31 RID: 15665
	[SerializeField]
	private MeshRenderer badgeMesh;

	// Token: 0x04003D32 RID: 15666
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003D33 RID: 15667
	[SerializeField]
	private float retractSpeed = 4f;

	// Token: 0x04003D34 RID: 15668
	[SerializeField]
	private AudioClip badgeAttachSound;

	// Token: 0x04003D35 RID: 15669
	[SerializeField]
	private float badgeAttachSoundVolume;

	// Token: 0x04003D36 RID: 15670
	[SerializeField]
	public int dispenserIndex;

	// Token: 0x04003D37 RID: 15671
	public int actorNr;

	// Token: 0x04003D38 RID: 15672
	private Coroutine retractCoroutine;

	// Token: 0x04003D39 RID: 15673
	private int lastRedeemedPoints = -1;

	// Token: 0x0200075A RID: 1882
	public enum BadgeState
	{
		// Token: 0x04003D3B RID: 15675
		AtDispenser,
		// Token: 0x04003D3C RID: 15676
		WithPlayer
	}
}
