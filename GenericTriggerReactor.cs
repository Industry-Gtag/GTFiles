using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000B0 RID: 176
public class GenericTriggerReactor : MonoBehaviour, IBuildValidation
{
	// Token: 0x06000439 RID: 1081 RVA: 0x00018D12 File Offset: 0x00016F12
	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.ComponentName.Length == 0)
		{
			return true;
		}
		if (Type.GetType(this.ComponentName) == null)
		{
			Debug.LogError("GenericTriggerReactor :: ComponentName must specify a valid Component or be empty.");
			return false;
		}
		return true;
	}

	// Token: 0x0600043A RID: 1082 RVA: 0x00018D43 File Offset: 0x00016F43
	private void Awake()
	{
		this.componentType = Type.GetType(this.ComponentName);
		base.TryGetComponent<GorillaVelocityEstimator>(out this.gorillaVelocityEstimator);
	}

	// Token: 0x0600043B RID: 1083 RVA: 0x00018D63 File Offset: 0x00016F63
	private void OnTriggerEnter(Collider other)
	{
		this.OnTriggerTest(other, this.speedRangeEnter, this.GTOnTriggerEnter, this.idealMotionPlayRangeEnter);
	}

	// Token: 0x0600043C RID: 1084 RVA: 0x00018D7E File Offset: 0x00016F7E
	private void OnTriggerExit(Collider other)
	{
		this.OnTriggerTest(other, this.speedRangeExit, this.GTOnTriggerExit, this.idealMotionPlayRangeExit);
	}

	// Token: 0x0600043D RID: 1085 RVA: 0x00018D9C File Offset: 0x00016F9C
	private void OnTriggerTest(Collider other, Vector2 speedRange, UnityEvent unityEvent, Vector2 idealMotionPlay)
	{
		Component component;
		if (unityEvent != null && (this.componentType == null || other.TryGetComponent(this.componentType, out component)))
		{
			if (this.gorillaVelocityEstimator != null)
			{
				float magnitude = this.gorillaVelocityEstimator.linearVelocity.magnitude;
				if (magnitude < speedRange.x || magnitude > speedRange.y)
				{
					return;
				}
				if (this.idealMotion != null)
				{
					float num = Vector3.Dot(this.gorillaVelocityEstimator.linearVelocity.normalized, this.idealMotion.forward);
					if (num < idealMotionPlay.x || num > idealMotionPlay.y)
					{
						return;
					}
				}
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x0400049C RID: 1180
	[SerializeField]
	private string ComponentName = string.Empty;

	// Token: 0x0400049D RID: 1181
	[Space]
	[SerializeField]
	private Vector2 speedRangeEnter;

	// Token: 0x0400049E RID: 1182
	[SerializeField]
	private Vector2 speedRangeExit;

	// Token: 0x0400049F RID: 1183
	[Space]
	[SerializeField]
	private Transform idealMotion;

	// Token: 0x040004A0 RID: 1184
	[SerializeField]
	private Vector2 idealMotionPlayRangeEnter;

	// Token: 0x040004A1 RID: 1185
	[SerializeField]
	private Vector2 idealMotionPlayRangeExit;

	// Token: 0x040004A2 RID: 1186
	[Space]
	[SerializeField]
	private UnityEvent GTOnTriggerEnter;

	// Token: 0x040004A3 RID: 1187
	[SerializeField]
	private UnityEvent GTOnTriggerExit;

	// Token: 0x040004A4 RID: 1188
	private Type componentType;

	// Token: 0x040004A5 RID: 1189
	private GorillaVelocityEstimator gorillaVelocityEstimator;
}
