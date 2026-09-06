using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000249 RID: 585
public class MonkeVoteResult : MonoBehaviour
{
	// Token: 0x1700018D RID: 397
	// (get) Token: 0x06000FAE RID: 4014 RVA: 0x000554E9 File Offset: 0x000536E9
	// (set) Token: 0x06000FAF RID: 4015 RVA: 0x000554F4 File Offset: 0x000536F4
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

	// Token: 0x06000FB0 RID: 4016 RVA: 0x00055518 File Offset: 0x00053718
	public void ShowResult(string questionOption, int percentage, bool showVote, bool showPrediction, bool isWinner)
	{
		this._optionText.text = questionOption;
		this._optionIndicator.SetActive(true);
		this._scoreText.text = ((percentage >= 0) ? string.Format("{0}%", percentage) : "--");
		this._voteIndicator.SetActive(showVote);
		this._guessWinIndicator.SetActive(showPrediction && isWinner);
		this._guessLoseIndicator.SetActive(showPrediction && !isWinner);
		this._youWinIndicator.SetActive(isWinner && showPrediction);
		this._mostPopularIndicator.SetActive(isWinner);
		this.ShowRockPile(percentage);
	}

	// Token: 0x06000FB1 RID: 4017 RVA: 0x000555BC File Offset: 0x000537BC
	public void HideResult()
	{
		this._optionIndicator.SetActive(false);
		this._voteIndicator.SetActive(false);
		this._guessWinIndicator.SetActive(false);
		this._guessLoseIndicator.SetActive(false);
		this._youWinIndicator.SetActive(false);
		this._mostPopularIndicator.SetActive(false);
		this.ShowRockPile(0);
	}

	// Token: 0x06000FB2 RID: 4018 RVA: 0x00055618 File Offset: 0x00053818
	private void ShowRockPile(int percentage)
	{
		this._rockPiles.Show(percentage);
	}

	// Token: 0x06000FB3 RID: 4019 RVA: 0x00055628 File Offset: 0x00053828
	public void SetDynamicMeshesVisible(bool visible)
	{
		this._mostPopularIndicator.SetActive(visible);
		this._voteIndicator.SetActive(visible);
		this._guessWinIndicator.SetActive(visible);
		this._guessLoseIndicator.SetActive(visible);
		this._rockPiles.Show(visible ? 100 : (-1));
	}

	// Token: 0x040012DF RID: 4831
	[SerializeField]
	private GameObject _optionIndicator;

	// Token: 0x040012E0 RID: 4832
	[SerializeField]
	private TMP_Text _optionText;

	// Token: 0x040012E1 RID: 4833
	[FormerlySerializedAs("_scoreLabelPost")]
	[SerializeField]
	private GameObject _scoreIndicator;

	// Token: 0x040012E2 RID: 4834
	[SerializeField]
	private TMP_Text _scoreText;

	// Token: 0x040012E3 RID: 4835
	[SerializeField]
	private GameObject _voteIndicator;

	// Token: 0x040012E4 RID: 4836
	[SerializeField]
	private GameObject _guessWinIndicator;

	// Token: 0x040012E5 RID: 4837
	[SerializeField]
	private GameObject _guessLoseIndicator;

	// Token: 0x040012E6 RID: 4838
	[SerializeField]
	private GameObject _mostPopularIndicator;

	// Token: 0x040012E7 RID: 4839
	[SerializeField]
	private GameObject _youWinIndicator;

	// Token: 0x040012E8 RID: 4840
	[SerializeField]
	private RockPiles _rockPiles;

	// Token: 0x040012E9 RID: 4841
	private MonkeVoteMachine _machine;

	// Token: 0x040012EA RID: 4842
	private string _text = string.Empty;

	// Token: 0x040012EB RID: 4843
	private bool _canVote;

	// Token: 0x040012EC RID: 4844
	private float _rockPileHeight;
}
