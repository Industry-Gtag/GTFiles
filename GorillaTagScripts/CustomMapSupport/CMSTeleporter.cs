using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GT_CustomMapSupportRuntime;
using JetBrains.Annotations;
using UnityEngine;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000FBF RID: 4031
	public class CMSTeleporter : CMSTrigger
	{
		// Token: 0x0600643D RID: 25661 RVA: 0x00203CC0 File Offset: 0x00201EC0
		public override void CopyTriggerSettings(TriggerSettings settings)
		{
			if (settings.GetType() == typeof(TeleporterSettings))
			{
				TeleporterSettings teleporterSettings = (TeleporterSettings)settings;
				this.TeleportPoints = teleporterSettings.TeleportPoints;
				this.matchTeleportPointRotation = teleporterSettings.matchTeleportPointRotation;
				this.maintainVelocity = teleporterSettings.maintainVelocity;
			}
			for (int i = this.TeleportPoints.Count - 1; i >= 0; i--)
			{
				if (this.TeleportPoints[i] == null)
				{
					this.TeleportPoints.RemoveAt(i);
				}
			}
			base.CopyTriggerSettings(settings);
		}

		// Token: 0x0600643E RID: 25662 RVA: 0x00203D50 File Offset: 0x00201F50
		public override void Trigger(double triggerTime = -1.0, bool originatedLocally = false, bool ignoreTriggerCount = false)
		{
			base.Trigger(triggerTime, originatedLocally, ignoreTriggerCount);
			if (originatedLocally && GTPlayer.hasInstance)
			{
				GTPlayer instance = GTPlayer.Instance;
				if (this.TeleportPoints.Count != 0)
				{
					Transform transform = this.TeleportPoints[Random.Range(0, this.TeleportPoints.Count)];
					if (transform != null)
					{
						instance.TeleportTo(transform, this.matchTeleportPointRotation, this.maintainVelocity);
					}
				}
			}
		}

		// Token: 0x040072FF RID: 29439
		[Tooltip("Teleport points used to return the player to the map. Chosen at random.")]
		[SerializeField]
		[NotNull]
		public List<Transform> TeleportPoints = new List<Transform>();

		// Token: 0x04007300 RID: 29440
		public bool matchTeleportPointRotation;

		// Token: 0x04007301 RID: 29441
		public bool maintainVelocity;
	}
}
