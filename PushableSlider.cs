using System;
using UnityEngine;

// Token: 0x02000586 RID: 1414
public class PushableSlider : MonoBehaviour
{
	// Token: 0x060023F0 RID: 9200 RVA: 0x000C1C3B File Offset: 0x000BFE3B
	public void Awake()
	{
		this.Initialize();
	}

	// Token: 0x060023F1 RID: 9201 RVA: 0x000C1C43 File Offset: 0x000BFE43
	private void Initialize()
	{
		if (this._initialized)
		{
			return;
		}
		this._initialized = true;
		this._localSpace = base.transform.worldToLocalMatrix;
		this._startingPos = base.transform.localPosition;
	}

	// Token: 0x060023F2 RID: 9202 RVA: 0x000C1C78 File Offset: 0x000BFE78
	private void OnTriggerStay(Collider other)
	{
		if (!base.enabled)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent == null)
		{
			return;
		}
		Vector3 vector = this._localSpace.MultiplyPoint3x4(other.transform.position);
		Vector3 vector2 = base.transform.localPosition - this._startingPos - vector;
		float num = Mathf.Abs(vector2.x);
		if (num < this.farPushDist)
		{
			Vector3 currentVelocity = componentInParent.currentVelocity;
			if (Mathf.Sign(vector2.x) != Mathf.Sign((this._localSpace.rotation * currentVelocity).x))
			{
				return;
			}
			vector2.x = Mathf.Sign(vector2.x) * (this.farPushDist - num);
			vector2.y = 0f;
			vector2.z = 0f;
			Vector3 vector3 = base.transform.localPosition - this._startingPos + vector2;
			vector3.x = Mathf.Clamp(vector3.x, this.minXOffset, this.maxXOffset);
			base.transform.localPosition = this.GetXOffsetVector(vector3.x + this._startingPos.x);
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x060023F3 RID: 9203 RVA: 0x000C1DDB File Offset: 0x000BFFDB
	private Vector3 GetXOffsetVector(float x)
	{
		return new Vector3(x, this._startingPos.y, this._startingPos.z);
	}

	// Token: 0x060023F4 RID: 9204 RVA: 0x000C1DFC File Offset: 0x000BFFFC
	public void SetProgress(float value)
	{
		this.Initialize();
		value = Mathf.Clamp(value, 0f, 1f);
		float num = Mathf.Lerp(this.minXOffset, this.maxXOffset, value);
		base.transform.localPosition = this.GetXOffsetVector(this._startingPos.x + num);
		this._previousLocalPosition = new Vector3(num, 0f, 0f);
		this._cachedProgress = value;
	}

	// Token: 0x060023F5 RID: 9205 RVA: 0x000C1E70 File Offset: 0x000C0070
	public float GetProgress()
	{
		this.Initialize();
		Vector3 vector = base.transform.localPosition - this._startingPos;
		if (vector == this._previousLocalPosition)
		{
			return this._cachedProgress;
		}
		this._previousLocalPosition = vector;
		this._cachedProgress = (vector.x - this.minXOffset) / (this.maxXOffset - this.minXOffset);
		return this._cachedProgress;
	}

	// Token: 0x04002F37 RID: 12087
	[SerializeField]
	private float farPushDist = 0.015f;

	// Token: 0x04002F38 RID: 12088
	[SerializeField]
	private float maxXOffset;

	// Token: 0x04002F39 RID: 12089
	[SerializeField]
	private float minXOffset;

	// Token: 0x04002F3A RID: 12090
	private Matrix4x4 _localSpace;

	// Token: 0x04002F3B RID: 12091
	private Vector3 _startingPos;

	// Token: 0x04002F3C RID: 12092
	private Vector3 _previousLocalPosition;

	// Token: 0x04002F3D RID: 12093
	private float _cachedProgress;

	// Token: 0x04002F3E RID: 12094
	private bool _initialized;
}
