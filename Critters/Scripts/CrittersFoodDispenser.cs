using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Critters.Scripts
{
	// Token: 0x020013EC RID: 5100
	public class CrittersFoodDispenser : CrittersActor
	{
		// Token: 0x060080B2 RID: 32946 RVA: 0x0029D9EA File Offset: 0x0029BBEA
		public override void Initialize()
		{
			base.Initialize();
			this.heldByPlayer = false;
		}

		// Token: 0x060080B3 RID: 32947 RVA: 0x0029D9F9 File Offset: 0x0029BBF9
		public override void GrabbedBy(CrittersActor grabbingActor, bool positionOverride = false, Quaternion localRotation = default(Quaternion), Vector3 localOffset = default(Vector3), bool disableGrabbing = false)
		{
			base.GrabbedBy(grabbingActor, positionOverride, localRotation, localOffset, disableGrabbing);
			this.heldByPlayer = grabbingActor.isOnPlayer;
		}

		// Token: 0x060080B4 RID: 32948 RVA: 0x0029DA14 File Offset: 0x0029BC14
		protected override void RemoteGrabbedBy(CrittersActor grabbingActor)
		{
			base.RemoteGrabbedBy(grabbingActor);
			this.heldByPlayer = grabbingActor.isOnPlayer;
		}

		// Token: 0x060080B5 RID: 32949 RVA: 0x0029DA29 File Offset: 0x0029BC29
		public override void Released(bool keepWorldPosition, Quaternion rotation = default(Quaternion), Vector3 position = default(Vector3), Vector3 impulseVelocity = default(Vector3), Vector3 impulseAngularVelocity = default(Vector3))
		{
			base.Released(keepWorldPosition, rotation, position, impulseVelocity, impulseAngularVelocity);
			this.heldByPlayer = false;
		}

		// Token: 0x060080B6 RID: 32950 RVA: 0x0029DA3F File Offset: 0x0029BC3F
		protected override void HandleRemoteReleased()
		{
			base.HandleRemoteReleased();
			this.heldByPlayer = false;
		}

		// Token: 0x040091BF RID: 37311
		[FormerlySerializedAs("isHeldByPlayer")]
		public bool heldByPlayer;
	}
}
