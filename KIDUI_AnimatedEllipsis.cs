using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02000BC2 RID: 3010
public class KIDUI_AnimatedEllipsis : MonoBehaviour
{
	// Token: 0x06004BEB RID: 19435 RVA: 0x00194B6A File Offset: 0x00192D6A
	private void Awake()
	{
		if (this._ellipsisObjects != null)
		{
			return;
		}
		this.SetupEllipsis();
	}

	// Token: 0x06004BEC RID: 19436 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Start()
	{
	}

	// Token: 0x06004BED RID: 19437 RVA: 0x00194B7B File Offset: 0x00192D7B
	private void OnDisable()
	{
		this.StopAnimation();
	}

	// Token: 0x06004BEE RID: 19438 RVA: 0x00194B84 File Offset: 0x00192D84
	private void SetupEllipsis()
	{
		if (this._ellipsisRoot == null)
		{
			this._ellipsisRoot = base.gameObject;
		}
		this._ellipsisObjects = new ValueTuple<GameObject, float, float, float>[this._ellipsisStartingValues.Count];
		for (int i = 0; i < this._ellipsisStartingValues.Count; i++)
		{
			float num = this._ellipsisStartingValues[i];
			this._ellipsisObjects[i].Item1 = Object.Instantiate<GameObject>(this._ellipsisPrefab, this._ellipsisRoot.transform);
			this._ellipsisObjects[i].Item1.transform.localScale = new Vector3(num, num, num);
			this._ellipsisObjects[i].Item2 = (this._ellipsisObjects[i].Item3 = num);
		}
	}

	// Token: 0x06004BEF RID: 19439 RVA: 0x00194C5A File Offset: 0x00192E5A
	private IEnumerator EllipsisAnimation()
	{
		int currIndex = 0;
		while (this._runAnimation)
		{
			for (int i = 0; i < this._ellipsisObjects.Length; i++)
			{
				int num = i - currIndex;
				if (num < 0)
				{
					num = this._ellipsisStartingValues.Count + num;
				}
				float num2 = this._ellipsisStartingValues[num];
				this._ellipsisObjects[i].Item1.transform.localScale = Vector3.one * num2;
			}
			int num3 = currIndex;
			currIndex = num3 + 1;
			if (currIndex >= this._ellipsisObjects.Length)
			{
				currIndex = 0;
			}
			yield return new WaitForSeconds(this._pauseBetweenScale);
		}
		yield break;
	}

	// Token: 0x06004BF0 RID: 19440 RVA: 0x00194C69 File Offset: 0x00192E69
	private IEnumerator EllipsisAnimation2()
	{
		float time = 0f;
		while (this._runAnimation)
		{
			for (int i = 0; i < this._ellipsisObjects.Length; i++)
			{
				float num = this._scaleDuration / (float)(this._ellipsisObjects.Length + 1) * (float)i;
				float num2 = this.LerpLoop(this._startingScale, this._endScale, time, num, this._scaleDuration);
				this._ellipsisObjects[i].Item1.transform.localScale = new Vector3(num2, num2, num2);
			}
			time += Time.deltaTime * this._animationSpeedMultiplier;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06004BF1 RID: 19441 RVA: 0x00194C78 File Offset: 0x00192E78
	public async Task StartAnimation()
	{
		if (this._ellipsisObjects == null)
		{
			this.SetupEllipsis();
		}
		if (this._animationCoroutine != null)
		{
			Debug.LogWarningFormat("[KID::UI::ELLIPSIS] Animation is already running.", Array.Empty<object>());
			await this.StopAnimation();
		}
		for (int i = 0; i < this._ellipsisCount; i++)
		{
			this._ellipsisObjects[i].Item1.transform.localScale = new Vector3(this._ellipsisObjects[i].Item2, this._ellipsisObjects[i].Item2, this._ellipsisObjects[i].Item2);
		}
		this._ellipsisRoot.SetActive(true);
		this._runAnimation = true;
		if (this._shouldLerp)
		{
			this._animationCoroutine = base.StartCoroutine(this.EllipsisAnimation2());
		}
		else
		{
			this._animationCoroutine = base.StartCoroutine(this.EllipsisAnimation());
		}
	}

	// Token: 0x06004BF2 RID: 19442 RVA: 0x00194CBC File Offset: 0x00192EBC
	public async Task StopAnimation()
	{
		this._runAnimation = false;
		base.StopAllCoroutines();
		await Task.Delay(100);
		this._animationCoroutine = null;
		this._ellipsisRoot.SetActive(false);
	}

	// Token: 0x06004BF3 RID: 19443 RVA: 0x00194D00 File Offset: 0x00192F00
	public float LerpLoop(float start, float end, float time, float offsetTime, float duration)
	{
		float num = (offsetTime - time) % duration / duration;
		float num2 = this._ellipsisAnimationCurve.Evaluate(num);
		return Mathf.Lerp(start, end, num2);
	}

	// Token: 0x04005ED2 RID: 24274
	[Header("Ellipsis Spawning")]
	[SerializeField]
	private bool _animateOnStart = true;

	// Token: 0x04005ED3 RID: 24275
	[SerializeField]
	private int _ellipsisCount = 3;

	// Token: 0x04005ED4 RID: 24276
	[SerializeField]
	private GameObject _ellipsisPrefab;

	// Token: 0x04005ED5 RID: 24277
	[SerializeField]
	private GameObject _ellipsisRoot;

	// Token: 0x04005ED6 RID: 24278
	[SerializeField]
	private List<float> _ellipsisStartingValues = new List<float>();

	// Token: 0x04005ED7 RID: 24279
	[Header("Animation Settings")]
	[SerializeField]
	private bool _shouldLerp;

	// Token: 0x04005ED8 RID: 24280
	[SerializeField]
	private AnimationCurve _ellipsisAnimationCurve;

	// Token: 0x04005ED9 RID: 24281
	[SerializeField]
	private float _animationSpeedMultiplier = 0.25f;

	// Token: 0x04005EDA RID: 24282
	[SerializeField]
	private float _startingScale = 0.33f;

	// Token: 0x04005EDB RID: 24283
	[SerializeField]
	private float _intermediaryScale = 0.66f;

	// Token: 0x04005EDC RID: 24284
	[SerializeField]
	private float _endScale = 1f;

	// Token: 0x04005EDD RID: 24285
	[SerializeField]
	private float _scaleDuration = 0.25f;

	// Token: 0x04005EDE RID: 24286
	[SerializeField]
	private float _pauseBetweenScale = 0.25f;

	// Token: 0x04005EDF RID: 24287
	[SerializeField]
	private float _pauseBetweenCycles = 0.5f;

	// Token: 0x04005EE0 RID: 24288
	private bool _runAnimation;

	// Token: 0x04005EE1 RID: 24289
	private float _nextChange;

	// Token: 0x04005EE2 RID: 24290
	[TupleElementNames(new string[] { "ellipsis", "startingScale", "currentScale", "lerpT" })]
	private ValueTuple<GameObject, float, float, float>[] _ellipsisObjects;

	// Token: 0x04005EE3 RID: 24291
	private Coroutine _animationCoroutine;
}
