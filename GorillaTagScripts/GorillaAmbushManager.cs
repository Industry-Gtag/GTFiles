using System;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F8D RID: 3981
	public sealed class GorillaAmbushManager : GorillaTagManager
	{
		// Token: 0x060062F0 RID: 25328 RVA: 0x001FDB80 File Offset: 0x001FBD80
		public override GameModeType GameType()
		{
			if (!this.isGhostTag)
			{
				return GameModeType.Ambush;
			}
			return GameModeType.Ghost;
		}

		// Token: 0x17000977 RID: 2423
		// (get) Token: 0x060062F1 RID: 25329 RVA: 0x001FDB8D File Offset: 0x001FBD8D
		public static int HandEffectHash
		{
			get
			{
				return GorillaAmbushManager.handTapHash;
			}
		}

		// Token: 0x17000978 RID: 2424
		// (get) Token: 0x060062F2 RID: 25330 RVA: 0x001FDB94 File Offset: 0x001FBD94
		// (set) Token: 0x060062F3 RID: 25331 RVA: 0x001FDB9B File Offset: 0x001FBD9B
		public static float HandFXScaleModifier { get; private set; }

		// Token: 0x17000979 RID: 2425
		// (get) Token: 0x060062F4 RID: 25332 RVA: 0x001FDBA3 File Offset: 0x001FBDA3
		// (set) Token: 0x060062F5 RID: 25333 RVA: 0x001FDBAB File Offset: 0x001FBDAB
		public bool isGhostTag { get; private set; }

		// Token: 0x060062F6 RID: 25334 RVA: 0x001FDBB4 File Offset: 0x001FBDB4
		public override void Awake()
		{
			base.Awake();
			if (this.handTapFX != null)
			{
				GorillaAmbushManager.handTapHash = PoolUtils.GameObjHashCode(this.handTapFX);
			}
			GorillaAmbushManager.HandFXScaleModifier = this.handTapScaleFactor;
		}

		// Token: 0x060062F7 RID: 25335 RVA: 0x001FDBE5 File Offset: 0x001FBDE5
		private void Start()
		{
			this.hasScryingPlane = this.scryingPlaneRef.TryResolve<MeshRenderer>(out this.scryingPlane);
			this.hasScryingPlane3p = this.scryingPlane3pRef.TryResolve<MeshRenderer>(out this.scryingPlane3p);
		}

		// Token: 0x060062F8 RID: 25336 RVA: 0x001FDC15 File Offset: 0x001FBE15
		public override string GameModeName()
		{
			if (!this.isGhostTag)
			{
				return "AMBUSH";
			}
			return "GHOST";
		}

		// Token: 0x060062F9 RID: 25337 RVA: 0x001FDC2C File Offset: 0x001FBE2C
		public override string GameModeNameRoomLabel()
		{
			string text = (this.isGhostTag ? "GAME_MODE_GHOST_ROOM_LABEL" : "GAME_MODE_AMBUSH_ROOM_LABEL");
			string text2 = (this.isGhostTag ? "(GHOST GAME)" : "(AMBUSH GAME)");
			string text3;
			if (!LocalisationManager.TryGetKeyForCurrentLocale(text, out text3, text2))
			{
				Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [" + text + "]");
			}
			return text3;
		}

		// Token: 0x060062FA RID: 25338 RVA: 0x001FDC84 File Offset: 0x001FBE84
		public override void UpdatePlayerAppearance(VRRig rig)
		{
			int num = this.MyMatIndex(rig.creator);
			rig.ChangeMaterialLocal(num);
			bool flag = base.IsInfected(rig.Creator);
			bool flag2 = base.IsInfected(NetworkSystem.Instance.LocalPlayer);
			rig.bodyRenderer.SetGameModeBodyType(flag ? GorillaBodyType.Skeleton : GorillaBodyType.Default);
			rig.SetInvisibleToLocalPlayer(flag && !flag2);
			if (this.isGhostTag && rig.isOfflineVRRig)
			{
				CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(flag);
				if (this.hasScryingPlane)
				{
					this.scryingPlane.enabled = flag2;
				}
				if (this.hasScryingPlane3p)
				{
					this.scryingPlane3p.enabled = flag2;
				}
			}
		}

		// Token: 0x060062FB RID: 25339 RVA: 0x001FDD2A File Offset: 0x001FBF2A
		public override int MyMatIndex(NetPlayer forPlayer)
		{
			if (!base.IsInfected(forPlayer))
			{
				return 0;
			}
			return 13;
		}

		// Token: 0x060062FC RID: 25340 RVA: 0x001FDD3C File Offset: 0x001FBF3C
		public override void StopPlaying()
		{
			base.StopPlaying();
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				VRRig rig = rigContainer.Rig;
				GorillaSkin.ApplyToRig(rig, null, GorillaSkin.SkinType.gameMode);
				rig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Default);
				rig.SetInvisibleToLocalPlayer(false);
			}
			CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
			if (this.hasScryingPlane)
			{
				this.scryingPlane.enabled = false;
			}
			if (this.hasScryingPlane3p)
			{
				this.scryingPlane3p.enabled = false;
			}
		}

		// Token: 0x040071A2 RID: 29090
		public GameObject handTapFX;

		// Token: 0x040071A3 RID: 29091
		public GorillaSkin ambushSkin;

		// Token: 0x040071A4 RID: 29092
		[SerializeField]
		private AudioClip[] firstPersonTaggedSounds;

		// Token: 0x040071A5 RID: 29093
		[SerializeField]
		private float firstPersonTaggedSoundVolume;

		// Token: 0x040071A6 RID: 29094
		private static int handTapHash = -1;

		// Token: 0x040071A7 RID: 29095
		public float handTapScaleFactor = 0.5f;

		// Token: 0x040071A9 RID: 29097
		public float crawlingSpeedForMaxVolume;

		// Token: 0x040071AB RID: 29099
		[SerializeField]
		private XSceneRef scryingPlaneRef;

		// Token: 0x040071AC RID: 29100
		[SerializeField]
		private XSceneRef scryingPlane3pRef;

		// Token: 0x040071AD RID: 29101
		private const int STEALTH_MATERIAL_INDEX = 13;

		// Token: 0x040071AE RID: 29102
		private MeshRenderer scryingPlane;

		// Token: 0x040071AF RID: 29103
		private bool hasScryingPlane;

		// Token: 0x040071B0 RID: 29104
		private MeshRenderer scryingPlane3p;

		// Token: 0x040071B1 RID: 29105
		private bool hasScryingPlane3p;
	}
}
