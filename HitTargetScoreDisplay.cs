using System;
using System.Collections;
using GorillaTag;
using UnityEngine;

// Token: 0x020004B1 RID: 1201
public class HitTargetScoreDisplay : MonoBehaviour
{
	// Token: 0x06001D4E RID: 7502 RVA: 0x0009E9E0 File Offset: 0x0009CBE0
	protected void Awake()
	{
		this.rotateTimeTotal = 180f / (float)this.rotateSpeed;
		this.matPropBlock = new MaterialPropertyBlock();
		this.networkedScore.AddCallback(new Action<int>(this.OnScoreChanged), true);
		this.ResetRotation();
		this.tensOld = 0;
		this.hundredsOld = 0;
		this.matPropBlock.SetVector(ShaderProps._BaseMap_ST, this.numberSheet[0]);
		this.singlesRend.SetPropertyBlock(this.matPropBlock);
		this.tensRend.SetPropertyBlock(this.matPropBlock);
		this.hundredsRend.SetPropertyBlock(this.matPropBlock);
	}

	// Token: 0x06001D4F RID: 7503 RVA: 0x0009EA86 File Offset: 0x0009CC86
	private void OnDestroy()
	{
		this.networkedScore.RemoveCallback(new Action<int>(this.OnScoreChanged));
	}

	// Token: 0x06001D50 RID: 7504 RVA: 0x0009EAA0 File Offset: 0x0009CCA0
	private void ResetRotation()
	{
		Quaternion rotation = base.transform.rotation;
		this.singlesCard.rotation = rotation;
		this.tensCard.rotation = rotation;
		this.hundredsCard.rotation = rotation;
	}

	// Token: 0x06001D51 RID: 7505 RVA: 0x0009EADD File Offset: 0x0009CCDD
	private IEnumerator RotatingCo()
	{
		float timeElapsedSinceHit = 0f;
		int singlesPlace = this.currentScore % 10;
		int tensPlace = this.currentScore / 10 % 10;
		bool tensChange = this.tensOld != tensPlace;
		this.tensOld = tensPlace;
		int hundredsPlace = this.currentScore / 100 % 10;
		bool hundredsChange = this.hundredsOld != hundredsPlace;
		this.hundredsOld = hundredsPlace;
		bool digitsChange = true;
		while (timeElapsedSinceHit < this.rotateTimeTotal)
		{
			this.singlesCard.Rotate((float)this.rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
			Vector3 localEulerAngles = this.singlesCard.localEulerAngles;
			localEulerAngles.x = Mathf.Clamp(localEulerAngles.x, 0f, 180f);
			this.singlesCard.localEulerAngles = localEulerAngles;
			if (tensChange)
			{
				this.tensCard.Rotate((float)this.rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
				Vector3 localEulerAngles2 = this.tensCard.localEulerAngles;
				localEulerAngles2.x = Mathf.Clamp(localEulerAngles2.x, 0f, 180f);
				this.tensCard.localEulerAngles = localEulerAngles2;
			}
			if (hundredsChange)
			{
				this.hundredsCard.Rotate((float)this.rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
				Vector3 localEulerAngles3 = this.hundredsCard.localEulerAngles;
				localEulerAngles3.x = Mathf.Clamp(localEulerAngles3.x, 0f, 180f);
				this.hundredsCard.localEulerAngles = localEulerAngles3;
			}
			if (digitsChange && timeElapsedSinceHit >= this.rotateTimeTotal / 2f)
			{
				this.matPropBlock.SetVector(ShaderProps._BaseMap_ST, this.numberSheet[singlesPlace]);
				this.singlesRend.SetPropertyBlock(this.matPropBlock);
				if (tensChange)
				{
					this.matPropBlock.SetVector(ShaderProps._BaseMap_ST, this.numberSheet[tensPlace]);
					this.tensRend.SetPropertyBlock(this.matPropBlock);
				}
				if (hundredsChange)
				{
					this.matPropBlock.SetVector(ShaderProps._BaseMap_ST, this.numberSheet[hundredsPlace]);
					this.hundredsRend.SetPropertyBlock(this.matPropBlock);
				}
				digitsChange = false;
			}
			yield return null;
			timeElapsedSinceHit += Time.deltaTime;
		}
		this.ResetRotation();
		yield break;
		yield break;
	}

	// Token: 0x06001D52 RID: 7506 RVA: 0x0009EAEC File Offset: 0x0009CCEC
	private void OnScoreChanged(int newScore)
	{
		if (newScore == this.currentScore)
		{
			return;
		}
		if (this.currentRotationCoroutine != null)
		{
			base.StopCoroutine(this.currentRotationCoroutine);
		}
		this.currentScore = newScore;
		if (base.gameObject.activeInHierarchy)
		{
			this.currentRotationCoroutine = base.StartCoroutine(this.RotatingCo());
		}
	}

	// Token: 0x04002785 RID: 10117
	[SerializeField]
	private WatchableIntSO networkedScore;

	// Token: 0x04002786 RID: 10118
	private int currentScore;

	// Token: 0x04002787 RID: 10119
	private int tensOld;

	// Token: 0x04002788 RID: 10120
	private int hundredsOld;

	// Token: 0x04002789 RID: 10121
	private float rotateTimeTotal;

	// Token: 0x0400278A RID: 10122
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x0400278B RID: 10123
	private readonly Vector4[] numberSheet = new Vector4[]
	{
		new Vector4(1f, 1f, 0.8f, -0.5f),
		new Vector4(1f, 1f, 0f, 0f),
		new Vector4(1f, 1f, 0.2f, 0f),
		new Vector4(1f, 1f, 0.4f, 0f),
		new Vector4(1f, 1f, 0.6f, 0f),
		new Vector4(1f, 1f, 0.8f, 0f),
		new Vector4(1f, 1f, 0f, -0.5f),
		new Vector4(1f, 1f, 0.2f, -0.5f),
		new Vector4(1f, 1f, 0.4f, -0.5f),
		new Vector4(1f, 1f, 0.6f, -0.5f)
	};

	// Token: 0x0400278C RID: 10124
	public int rotateSpeed = 180;

	// Token: 0x0400278D RID: 10125
	public Transform singlesCard;

	// Token: 0x0400278E RID: 10126
	public Transform tensCard;

	// Token: 0x0400278F RID: 10127
	public Transform hundredsCard;

	// Token: 0x04002790 RID: 10128
	public Renderer singlesRend;

	// Token: 0x04002791 RID: 10129
	public Renderer tensRend;

	// Token: 0x04002792 RID: 10130
	public Renderer hundredsRend;

	// Token: 0x04002793 RID: 10131
	private Coroutine currentRotationCoroutine;
}
