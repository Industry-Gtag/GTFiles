using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000247 RID: 583
public class MonkeVoteOption : MonoBehaviour
{
	// Token: 0x14000020 RID: 32
	// (add) Token: 0x06000F97 RID: 3991 RVA: 0x00055220 File Offset: 0x00053420
	// (remove) Token: 0x06000F98 RID: 3992 RVA: 0x00055258 File Offset: 0x00053458
	public event Action<MonkeVoteOption, Collider> OnVote;

	// Token: 0x1700018A RID: 394
	// (get) Token: 0x06000F99 RID: 3993 RVA: 0x0005528D File Offset: 0x0005348D
	// (set) Token: 0x06000F9A RID: 3994 RVA: 0x00055298 File Offset: 0x00053498
	public string Text
	{
		get
		{
			return this._text;
		}
		set
		{
			TMP_Text optionText = this._optionText;
			this._text = value;
			optionText.text = value;
		}
	}

	// Token: 0x1700018B RID: 395
	// (get) Token: 0x06000F9B RID: 3995 RVA: 0x000552BA File Offset: 0x000534BA
	// (set) Token: 0x06000F9C RID: 3996 RVA: 0x000552C4 File Offset: 0x000534C4
	public bool CanVote
	{
		get
		{
			return this._canVote;
		}
		set
		{
			Collider trigger = this._trigger;
			this._canVote = value;
			trigger.enabled = value;
		}
	}

	// Token: 0x06000F9D RID: 3997 RVA: 0x000552E6 File Offset: 0x000534E6
	private void Reset()
	{
		this.Configure();
	}

	// Token: 0x06000F9E RID: 3998 RVA: 0x000552F0 File Offset: 0x000534F0
	private void Configure()
	{
		foreach (Collider collider in base.GetComponentsInChildren<Collider>())
		{
			if (collider.isTrigger)
			{
				this._trigger = collider;
				break;
			}
		}
		if (!this._optionText)
		{
			this._optionText = base.GetComponentInChildren<TMP_Text>();
		}
	}

	// Token: 0x06000F9F RID: 3999 RVA: 0x00055340 File Offset: 0x00053540
	private void OnTriggerEnter(Collider other)
	{
		if (!this.IsValidVotingRock(other))
		{
			return;
		}
		Action<MonkeVoteOption, Collider> onVote = this.OnVote;
		if (onVote == null)
		{
			return;
		}
		onVote(this, other);
	}

	// Token: 0x06000FA0 RID: 4000 RVA: 0x00055360 File Offset: 0x00053560
	private bool IsValidVotingRock(Collider other)
	{
		SlingshotProjectile component = other.GetComponent<SlingshotProjectile>();
		return component && component.projectileOwner.IsLocal;
	}

	// Token: 0x06000FA1 RID: 4001 RVA: 0x00055389 File Offset: 0x00053589
	public void ResetState()
	{
		this.OnVote = null;
		this.ShowIndicators(false, false, true);
	}

	// Token: 0x06000FA2 RID: 4002 RVA: 0x0005539B File Offset: 0x0005359B
	public void ShowIndicators(bool showVote, bool showPrediction, bool instant = true)
	{
		this._voteIndicator.SetVisible(showVote, instant);
		this._guessIndicator.SetVisible(showPrediction, instant);
	}

	// Token: 0x06000FA3 RID: 4003 RVA: 0x000553B7 File Offset: 0x000535B7
	private void Vote()
	{
		this.SendVote(null);
	}

	// Token: 0x06000FA4 RID: 4004 RVA: 0x000553C0 File Offset: 0x000535C0
	private void SendVote(Collider other)
	{
		if (!this._canVote)
		{
			return;
		}
		Action<MonkeVoteOption, Collider> onVote = this.OnVote;
		if (onVote == null)
		{
			return;
		}
		onVote(this, other);
	}

	// Token: 0x06000FA5 RID: 4005 RVA: 0x000553DD File Offset: 0x000535DD
	public void SetDynamicMeshesVisible(bool visible)
	{
		this._voteIndicator.SetVisible(visible, true);
		this._guessIndicator.SetVisible(visible, true);
	}

	// Token: 0x040012D4 RID: 4820
	[SerializeField]
	private Collider _trigger;

	// Token: 0x040012D5 RID: 4821
	[SerializeField]
	private TMP_Text _optionText;

	// Token: 0x040012D6 RID: 4822
	[SerializeField]
	private VotingCard _voteIndicator;

	// Token: 0x040012D7 RID: 4823
	[FormerlySerializedAs("_predictionIndicator")]
	[SerializeField]
	private VotingCard _guessIndicator;

	// Token: 0x040012D9 RID: 4825
	private string _text = string.Empty;

	// Token: 0x040012DA RID: 4826
	private bool _canVote;
}
