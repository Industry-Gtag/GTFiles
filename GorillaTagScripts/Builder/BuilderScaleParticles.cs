using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02001048 RID: 4168
	public class BuilderScaleParticles : MonoBehaviour
	{
		// Token: 0x060067F3 RID: 26611 RVA: 0x00217010 File Offset: 0x00215210
		private void OnEnable()
		{
			if (this.useLossyScale)
			{
				this.setScaleNextFrame = true;
				this.enableFrame = Time.frameCount;
			}
		}

		// Token: 0x060067F4 RID: 26612 RVA: 0x0021702C File Offset: 0x0021522C
		private void LateUpdate()
		{
			if (this.setScaleNextFrame && Time.frameCount > this.enableFrame)
			{
				if (this.useLossyScale)
				{
					this.SetScale(base.transform.lossyScale.x);
				}
				this.setScaleNextFrame = false;
			}
		}

		// Token: 0x060067F5 RID: 26613 RVA: 0x00217068 File Offset: 0x00215268
		private void OnDisable()
		{
			if (this.useLossyScale)
			{
				this.RevertScale();
			}
		}

		// Token: 0x060067F6 RID: 26614 RVA: 0x00217078 File Offset: 0x00215278
		public void SetScale(float inScale)
		{
			bool isPlaying = this.system.isPlaying;
			if (isPlaying)
			{
				this.system.Stop();
				this.system.Clear();
			}
			if (Mathf.Approximately(inScale, this.scale))
			{
				if (this.autoPlay || isPlaying)
				{
					this.system.Play(true);
				}
				return;
			}
			this.scale = inScale;
			this.RevertScale();
			if (Mathf.Approximately(this.scale, 1f))
			{
				if (this.autoPlay || isPlaying)
				{
					this.system.Play(true);
				}
				return;
			}
			ParticleSystem.MainModule main = this.system.main;
			this.gravityMod = main.gravityModifierMultiplier;
			main.gravityModifierMultiplier = this.gravityMod * this.scale;
			if (main.startSize3D)
			{
				ParticleSystem.MinMaxCurve startSizeX = main.startSizeX;
				this.sizeCurveXCache = main.startSizeX;
				this.ScaleCurve(ref startSizeX, this.scale);
				main.startSizeX = startSizeX;
				ParticleSystem.MinMaxCurve startSizeY = main.startSizeY;
				this.sizeCurveYCache = main.startSizeY;
				this.ScaleCurve(ref startSizeY, this.scale);
				main.startSizeY = startSizeY;
				ParticleSystem.MinMaxCurve startSizeZ = main.startSizeZ;
				this.sizeCurveZCache = main.startSizeZ;
				this.ScaleCurve(ref startSizeZ, this.scale);
				main.startSizeZ = startSizeZ;
			}
			else
			{
				ParticleSystem.MinMaxCurve startSize = main.startSize;
				this.sizeCurveCache = main.startSize;
				this.ScaleCurve(ref startSize, this.scale);
				main.startSize = startSize;
			}
			ParticleSystem.MinMaxCurve startSpeed = main.startSpeed;
			this.speedCurveCache = main.startSpeed;
			this.ScaleCurve(ref startSpeed, this.scale);
			main.startSpeed = startSpeed;
			if (this.scaleShape)
			{
				ParticleSystem.ShapeModule shape = this.system.shape;
				this.shapeScale = shape.scale;
				shape.scale = this.shapeScale * this.scale;
			}
			if (this.scaleVelocityLifetime)
			{
				ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = this.system.velocityOverLifetime;
				this.lifetimeVelocityX = velocityOverLifetime.x;
				this.lifetimeVelocityY = velocityOverLifetime.y;
				this.lifetimeVelocityZ = velocityOverLifetime.z;
				ParticleSystem.MinMaxCurve minMaxCurve = velocityOverLifetime.x;
				this.ScaleCurve(ref minMaxCurve, this.scale);
				velocityOverLifetime.x = minMaxCurve;
				minMaxCurve = velocityOverLifetime.y;
				this.ScaleCurve(ref minMaxCurve, this.scale);
				velocityOverLifetime.y = minMaxCurve;
				minMaxCurve = velocityOverLifetime.z;
				this.ScaleCurve(ref minMaxCurve, this.scale);
				velocityOverLifetime.z = minMaxCurve;
			}
			if (this.scaleVelocityLimitLifetime)
			{
				ParticleSystem.LimitVelocityOverLifetimeModule limitVelocityOverLifetime = this.system.limitVelocityOverLifetime;
				this.limitMultiplier = limitVelocityOverLifetime.limitMultiplier;
				limitVelocityOverLifetime.limitMultiplier = this.limitMultiplier * this.scale;
			}
			if (this.scaleForceOverLife)
			{
				ParticleSystem.ForceOverLifetimeModule forceOverLifetime = this.system.forceOverLifetime;
				this.forceX = forceOverLifetime.x;
				this.forceY = forceOverLifetime.y;
				this.forceZ = forceOverLifetime.z;
				ParticleSystem.MinMaxCurve minMaxCurve2 = forceOverLifetime.x;
				this.ScaleCurve(ref minMaxCurve2, this.scale);
				forceOverLifetime.x = minMaxCurve2;
				minMaxCurve2 = forceOverLifetime.y;
				this.ScaleCurve(ref minMaxCurve2, this.scale);
				forceOverLifetime.y = minMaxCurve2;
				minMaxCurve2 = forceOverLifetime.z;
				this.ScaleCurve(ref minMaxCurve2, this.scale);
				forceOverLifetime.z = minMaxCurve2;
			}
			if (this.autoPlay || isPlaying)
			{
				this.system.Play(true);
			}
			this.shouldRevert = true;
		}

		// Token: 0x060067F7 RID: 26615 RVA: 0x002173E8 File Offset: 0x002155E8
		private void ScaleCurve(ref ParticleSystem.MinMaxCurve curve, float scale)
		{
			switch (curve.mode)
			{
			case ParticleSystemCurveMode.Constant:
				curve.constant *= scale;
				return;
			case ParticleSystemCurveMode.Curve:
			case ParticleSystemCurveMode.TwoCurves:
				curve.curveMultiplier *= scale;
				return;
			case ParticleSystemCurveMode.TwoConstants:
				curve.constantMin *= scale;
				curve.constantMax *= scale;
				return;
			default:
				return;
			}
		}

		// Token: 0x060067F8 RID: 26616 RVA: 0x00217450 File Offset: 0x00215650
		public void RevertScale()
		{
			if (!this.shouldRevert)
			{
				return;
			}
			ParticleSystem.MainModule main = this.system.main;
			main.gravityModifierMultiplier = this.gravityMod;
			main.startSpeed = this.speedCurveCache;
			if (main.startSize3D)
			{
				main.startSizeX = this.sizeCurveXCache;
				main.startSizeY = this.sizeCurveYCache;
				main.startSizeZ = this.sizeCurveZCache;
			}
			else
			{
				main.startSize = this.sizeCurveCache;
			}
			if (this.scaleShape)
			{
				this.system.shape.scale = this.shapeScale;
			}
			if (this.scaleVelocityLifetime)
			{
				ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = this.system.velocityOverLifetime;
				velocityOverLifetime.x = this.lifetimeVelocityX;
				velocityOverLifetime.y = this.lifetimeVelocityY;
				velocityOverLifetime.z = this.lifetimeVelocityZ;
			}
			if (this.scaleVelocityLimitLifetime)
			{
				this.system.limitVelocityOverLifetime.limitMultiplier = this.limitMultiplier;
			}
			if (this.scaleForceOverLife)
			{
				ParticleSystem.ForceOverLifetimeModule forceOverLifetime = this.system.forceOverLifetime;
				forceOverLifetime.x = this.forceX;
				forceOverLifetime.y = this.forceY;
				forceOverLifetime.z = this.forceZ;
			}
			this.scale = 1f;
			this.shouldRevert = false;
		}

		// Token: 0x04007700 RID: 30464
		private float scale = 1f;

		// Token: 0x04007701 RID: 30465
		[Tooltip("Scale particles on enable using lossy scale")]
		[SerializeField]
		private bool useLossyScale;

		// Token: 0x04007702 RID: 30466
		[Tooltip("Play particles after scaling")]
		[SerializeField]
		private bool autoPlay;

		// Token: 0x04007703 RID: 30467
		[SerializeField]
		private ParticleSystem system;

		// Token: 0x04007704 RID: 30468
		[SerializeField]
		private bool scaleShape;

		// Token: 0x04007705 RID: 30469
		[SerializeField]
		private bool scaleVelocityLifetime;

		// Token: 0x04007706 RID: 30470
		[SerializeField]
		private bool scaleVelocityLimitLifetime;

		// Token: 0x04007707 RID: 30471
		[SerializeField]
		private bool scaleForceOverLife;

		// Token: 0x04007708 RID: 30472
		private float gravityMod = 1f;

		// Token: 0x04007709 RID: 30473
		private ParticleSystem.MinMaxCurve speedCurveCache;

		// Token: 0x0400770A RID: 30474
		private ParticleSystem.MinMaxCurve sizeCurveCache;

		// Token: 0x0400770B RID: 30475
		private ParticleSystem.MinMaxCurve sizeCurveXCache;

		// Token: 0x0400770C RID: 30476
		private ParticleSystem.MinMaxCurve sizeCurveYCache;

		// Token: 0x0400770D RID: 30477
		private ParticleSystem.MinMaxCurve sizeCurveZCache;

		// Token: 0x0400770E RID: 30478
		private ParticleSystem.MinMaxCurve forceX;

		// Token: 0x0400770F RID: 30479
		private ParticleSystem.MinMaxCurve forceY;

		// Token: 0x04007710 RID: 30480
		private ParticleSystem.MinMaxCurve forceZ;

		// Token: 0x04007711 RID: 30481
		private Vector3 shapeScale = Vector3.one;

		// Token: 0x04007712 RID: 30482
		private ParticleSystem.MinMaxCurve lifetimeVelocityX;

		// Token: 0x04007713 RID: 30483
		private ParticleSystem.MinMaxCurve lifetimeVelocityY;

		// Token: 0x04007714 RID: 30484
		private ParticleSystem.MinMaxCurve lifetimeVelocityZ;

		// Token: 0x04007715 RID: 30485
		private float limitMultiplier = 1f;

		// Token: 0x04007716 RID: 30486
		private bool shouldRevert;

		// Token: 0x04007717 RID: 30487
		private bool setScaleNextFrame;

		// Token: 0x04007718 RID: 30488
		private int enableFrame;
	}
}
