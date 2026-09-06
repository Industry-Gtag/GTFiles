using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200024C RID: 588
public class VotingCard : MonoBehaviour
{
	// Token: 0x06000FB8 RID: 4024 RVA: 0x00055726 File Offset: 0x00053926
	private void MoveToOffPosition()
	{
		this._card.transform.position = this._offPosition.position;
	}

	// Token: 0x06000FB9 RID: 4025 RVA: 0x00055743 File Offset: 0x00053943
	private void MoveToOnPosition()
	{
		this._card.transform.position = this._onPosition.position;
	}

	// Token: 0x06000FBA RID: 4026 RVA: 0x00055760 File Offset: 0x00053960
	public void SetVisible(bool showVote, bool instant)
	{
		if (this._isVisible != showVote)
		{
			base.StopAllCoroutines();
		}
		if (instant)
		{
			this._card.transform.position = (showVote ? this._onPosition.position : this._offPosition.position);
			this._card.SetActive(showVote);
		}
		else if (showVote)
		{
			if (this._isVisible != showVote)
			{
				base.StartCoroutine(this.DoActivate());
			}
		}
		else
		{
			this._card.SetActive(false);
			this._card.transform.position = this._offPosition.position;
		}
		this._isVisible = showVote;
	}

	// Token: 0x06000FBB RID: 4027 RVA: 0x00055801 File Offset: 0x00053A01
	private IEnumerator DoActivate()
	{
		Vector3 from = this._offPosition.position;
		Vector3 to = this._onPosition.position;
		this._card.transform.position = from;
		this._card.SetActive(true);
		float lerpVal = 0f;
		while (lerpVal < 1f)
		{
			lerpVal += Time.deltaTime / this.activationTime;
			this._card.transform.position = Vector3.Lerp(from, to, lerpVal);
			yield return null;
		}
		yield break;
	}

	// Token: 0x040012F0 RID: 4848
	[SerializeField]
	private GameObject _card;

	// Token: 0x040012F1 RID: 4849
	[SerializeField]
	private Transform _offPosition;

	// Token: 0x040012F2 RID: 4850
	[SerializeField]
	private Transform _onPosition;

	// Token: 0x040012F3 RID: 4851
	[SerializeField]
	private float activationTime = 0.5f;

	// Token: 0x040012F4 RID: 4852
	private bool _isVisible;
}
