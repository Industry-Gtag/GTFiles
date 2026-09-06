using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012F8 RID: 4856
	public class DrillFX : MonoBehaviour
	{
		// Token: 0x060079BE RID: 31166 RVA: 0x0027B71C File Offset: 0x0027991C
		protected void Awake()
		{
			if (!DrillFX.appIsQuittingHandlerIsSubscribed)
			{
				DrillFX.appIsQuittingHandlerIsSubscribed = true;
				Application.quitting += DrillFX.HandleApplicationQuitting;
			}
			this.hasFX = this.fx != null;
			if (this.hasFX)
			{
				this.fxEmissionModule = this.fx.emission;
				this.fxEmissionMaxRate = this.fxEmissionModule.rateOverTimeMultiplier;
				this.fxShapeModule = this.fx.shape;
				this.fxShapeMaxRadius = this.fxShapeModule.radius;
			}
			this.hasAudio = this.loopAudio != null;
			if (this.hasAudio)
			{
				this.audioMaxVolume = this.loopAudio.volume;
				this.loopAudio.volume = 0f;
				this.loopAudio.loop = true;
				this.loopAudio.GTPlay();
			}
		}

		// Token: 0x060079BF RID: 31167 RVA: 0x0027B7F8 File Offset: 0x002799F8
		protected void OnEnable()
		{
			if (DrillFX.appIsQuitting)
			{
				return;
			}
			if (this.hasFX)
			{
				this.fxEmissionModule.rateOverTimeMultiplier = 0f;
			}
			if (this.hasAudio)
			{
				this.loopAudio.volume = 0f;
				this.loopAudio.loop = true;
				this.loopAudio.GTPlay();
			}
			this.ValidateLineCastPositions();
		}

		// Token: 0x060079C0 RID: 31168 RVA: 0x0027B85C File Offset: 0x00279A5C
		protected void OnDisable()
		{
			if (DrillFX.appIsQuitting)
			{
				return;
			}
			if (this.hasFX)
			{
				this.fxEmissionModule.rateOverTimeMultiplier = 0f;
			}
			if (this.hasAudio)
			{
				this.loopAudio.volume = 0f;
				this.loopAudio.GTStop();
			}
		}

		// Token: 0x060079C1 RID: 31169 RVA: 0x0027B8AC File Offset: 0x00279AAC
		protected void LateUpdate()
		{
			if (DrillFX.appIsQuitting)
			{
				return;
			}
			Transform transform = base.transform;
			RaycastHit raycastHit;
			Vector3 vector = (Physics.Linecast(transform.TransformPoint(this.lineCastStart), transform.TransformPoint(this.lineCastEnd), out raycastHit, this.lineCastLayerMask, QueryTriggerInteraction.Ignore) ? raycastHit.point : this.lineCastEnd);
			Vector3 vector2 = transform.InverseTransformPoint(vector);
			float num = Mathf.Clamp01(Vector3.Distance(this.lineCastStart, vector2) / this.maxDepth);
			if (this.hasFX)
			{
				this.fxEmissionModule.rateOverTimeMultiplier = this.fxEmissionMaxRate * this.fxEmissionCurve.Evaluate(num);
				this.fxShapeModule.position = vector2;
				this.fxShapeModule.radius = Mathf.Lerp(this.fxShapeMaxRadius, this.fxMinRadiusScale * this.fxShapeMaxRadius, num);
			}
			if (this.hasAudio)
			{
				this.loopAudio.volume = Mathf.MoveTowards(this.loopAudio.volume, this.audioMaxVolume * this.loopAudioVolumeCurve.Evaluate(num), this.loopAudioVolumeTransitionSpeed * Time.deltaTime);
			}
		}

		// Token: 0x060079C2 RID: 31170 RVA: 0x0027B9C2 File Offset: 0x00279BC2
		private static void HandleApplicationQuitting()
		{
			DrillFX.appIsQuitting = true;
		}

		// Token: 0x060079C3 RID: 31171 RVA: 0x0027B9CC File Offset: 0x00279BCC
		private bool ValidateLineCastPositions()
		{
			this.maxDepth = Vector3.Distance(this.lineCastStart, this.lineCastEnd);
			if (this.maxDepth > 1E-45f)
			{
				return true;
			}
			if (Application.isPlaying)
			{
				Debug.Log("DrillFX: lineCastStart and End are too close together. Disabling component.", this);
				base.enabled = false;
			}
			return false;
		}

		// Token: 0x04008B07 RID: 35591
		[SerializeField]
		private ParticleSystem fx;

		// Token: 0x04008B08 RID: 35592
		[SerializeField]
		private AnimationCurve fxEmissionCurve;

		// Token: 0x04008B09 RID: 35593
		[SerializeField]
		private float fxMinRadiusScale = 0.01f;

		// Token: 0x04008B0A RID: 35594
		[Tooltip("Right click menu has custom menu items. Anything starting with \"- \" is custom.")]
		[SerializeField]
		private AudioSource loopAudio;

		// Token: 0x04008B0B RID: 35595
		[SerializeField]
		private AnimationCurve loopAudioVolumeCurve;

		// Token: 0x04008B0C RID: 35596
		[Tooltip("Higher value makes it reach the target volume faster.")]
		[SerializeField]
		private float loopAudioVolumeTransitionSpeed = 3f;

		// Token: 0x04008B0D RID: 35597
		[FormerlySerializedAs("layerMask")]
		[Tooltip("The collision layers the line cast should intersect with")]
		[SerializeField]
		private LayerMask lineCastLayerMask;

		// Token: 0x04008B0E RID: 35598
		[Tooltip("The position in local space that the line cast starts.")]
		[SerializeField]
		private Vector3 lineCastStart = Vector3.zero;

		// Token: 0x04008B0F RID: 35599
		[Tooltip("The position in local space that the line cast ends.")]
		[SerializeField]
		private Vector3 lineCastEnd = Vector3.forward;

		// Token: 0x04008B10 RID: 35600
		private static bool appIsQuitting;

		// Token: 0x04008B11 RID: 35601
		private static bool appIsQuittingHandlerIsSubscribed;

		// Token: 0x04008B12 RID: 35602
		private float maxDepth;

		// Token: 0x04008B13 RID: 35603
		private bool hasFX;

		// Token: 0x04008B14 RID: 35604
		private ParticleSystem.EmissionModule fxEmissionModule;

		// Token: 0x04008B15 RID: 35605
		private float fxEmissionMaxRate;

		// Token: 0x04008B16 RID: 35606
		private ParticleSystem.ShapeModule fxShapeModule;

		// Token: 0x04008B17 RID: 35607
		private float fxShapeMaxRadius;

		// Token: 0x04008B18 RID: 35608
		private bool hasAudio;

		// Token: 0x04008B19 RID: 35609
		private float audioMaxVolume;
	}
}
