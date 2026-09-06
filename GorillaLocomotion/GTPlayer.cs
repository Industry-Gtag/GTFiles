using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AA;
using BoingKit;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using GorillaLocomotion.Swimming;
using GorillaTag;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion
{
	// Token: 0x02001186 RID: 4486
	public class GTPlayer : MonoBehaviour
	{
		// Token: 0x17000AD8 RID: 2776
		// (get) Token: 0x0600706B RID: 28779 RVA: 0x002438CB File Offset: 0x00241ACB
		public static GTPlayer Instance
		{
			get
			{
				return GTPlayer._instance;
			}
		}

		// Token: 0x17000AD9 RID: 2777
		// (get) Token: 0x0600706C RID: 28780 RVA: 0x002438D4 File Offset: 0x00241AD4
		private float bodyInitialHeight
		{
			get
			{
				if (GorillaIK.playerIK == null || !GorillaIK.playerIK.usingUpdatedIK)
				{
					return this._bodyInitialHeight;
				}
				return Mathf.Max(0.2f, Vector3.Dot(GorillaIK.playerIK.bodyBone.up, GTPlayerTransform.Up)) * this._bodyInitialHeight;
			}
		}

		// Token: 0x17000ADA RID: 2778
		// (get) Token: 0x0600706D RID: 28781 RVA: 0x0024392B File Offset: 0x00241B2B
		public GTPlayer.HandState LeftHand
		{
			get
			{
				return this.leftHand;
			}
		}

		// Token: 0x17000ADB RID: 2779
		// (get) Token: 0x0600706E RID: 28782 RVA: 0x00243933 File Offset: 0x00241B33
		public readonly ref GTPlayer.HandState LeftHandRef
		{
			get
			{
				return ref this.leftHand;
			}
		}

		// Token: 0x17000ADC RID: 2780
		// (get) Token: 0x0600706F RID: 28783 RVA: 0x0024393B File Offset: 0x00241B3B
		public GTPlayer.HandState RightHand
		{
			get
			{
				return this.rightHand;
			}
		}

		// Token: 0x17000ADD RID: 2781
		// (get) Token: 0x06007070 RID: 28784 RVA: 0x00243943 File Offset: 0x00241B43
		public readonly ref GTPlayer.HandState RightHandRef
		{
			get
			{
				return ref this.rightHand;
			}
		}

		// Token: 0x06007071 RID: 28785 RVA: 0x0024394B File Offset: 0x00241B4B
		public int GetMaterialTouchIndex(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).materialTouchIndex;
		}

		// Token: 0x06007072 RID: 28786 RVA: 0x00243963 File Offset: 0x00241B63
		public GorillaSurfaceOverride GetSurfaceOverride(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).surfaceOverride;
		}

		// Token: 0x06007073 RID: 28787 RVA: 0x0024397B File Offset: 0x00241B7B
		public RaycastHit GetTouchHitInfo(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).hitInfo;
		}

		// Token: 0x06007074 RID: 28788 RVA: 0x00243993 File Offset: 0x00241B93
		public bool IsHandTouching(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).wasColliding;
		}

		// Token: 0x06007075 RID: 28789 RVA: 0x002439AB File Offset: 0x00241BAB
		public GorillaVelocityTracker GetHandVelocityTracker(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).velocityTracker;
		}

		// Token: 0x06007076 RID: 28790 RVA: 0x002439C3 File Offset: 0x00241BC3
		public GorillaVelocityTracker GetInteractPointVelocityTracker(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).interactPointVelocityTracker;
		}

		// Token: 0x06007077 RID: 28791 RVA: 0x002439DB File Offset: 0x00241BDB
		public Transform GetControllerTransform(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).controllerTransform;
		}

		// Token: 0x06007078 RID: 28792 RVA: 0x002439F3 File Offset: 0x00241BF3
		public Transform GetHandFollower(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).handFollower;
		}

		// Token: 0x06007079 RID: 28793 RVA: 0x00243A0B File Offset: 0x00241C0B
		public Vector3 GetHandOffset(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).handOffset;
		}

		// Token: 0x0600707A RID: 28794 RVA: 0x00243A23 File Offset: 0x00241C23
		public Quaternion GetHandRotOffset(bool isLeftHand)
		{
			return (isLeftHand ? this.leftHand : this.rightHand).handRotOffset;
		}

		// Token: 0x0600707B RID: 28795 RVA: 0x00243A3B File Offset: 0x00241C3B
		public Vector3 GetHandPosition(bool isLeftHand, StiltID stiltID = StiltID.None)
		{
			return ((stiltID != StiltID.None) ? this.stiltStates[(int)stiltID] : (isLeftHand ? this.leftHand : this.rightHand)).lastPosition;
		}

		// Token: 0x0600707C RID: 28796 RVA: 0x00243A68 File Offset: 0x00241C68
		public void GetHandTapData(bool isLeftHand, StiltID stiltID, out bool wasHandTouching, out bool wasSliding, out int handMatIndex, out GorillaSurfaceOverride surfaceOverride, out RaycastHit handHitInfo, out Vector3 handPosition, out GorillaVelocityTracker handVelocityTracker)
		{
			((stiltID != StiltID.None) ? this.stiltStates[(int)stiltID] : (isLeftHand ? this.leftHand : this.rightHand)).GetHandTapData(out wasHandTouching, out wasSliding, out handMatIndex, out surfaceOverride, out handHitInfo, out handPosition, out handVelocityTracker);
		}

		// Token: 0x0600707D RID: 28797 RVA: 0x00243AAD File Offset: 0x00241CAD
		public void SetHandOffsets(bool isLeftHand, Vector3 handOffset, Quaternion handRotOffset)
		{
			if (isLeftHand)
			{
				this.leftHand.handOffset = handOffset;
				this.leftHand.handRotOffset = handRotOffset;
				return;
			}
			this.rightHand.handOffset = handOffset;
			this.rightHand.handRotOffset = handRotOffset;
		}

		// Token: 0x17000ADE RID: 2782
		// (get) Token: 0x0600707E RID: 28798 RVA: 0x00243AE3 File Offset: 0x00241CE3
		// (set) Token: 0x0600707F RID: 28799 RVA: 0x00243AEB File Offset: 0x00241CEB
		public Rigidbody playerRigidBody { get; private set; }

		// Token: 0x17000ADF RID: 2783
		// (get) Token: 0x06007080 RID: 28800 RVA: 0x00243AF4 File Offset: 0x00241CF4
		public Vector3 LastPosition
		{
			get
			{
				return this.lastPosition;
			}
		}

		// Token: 0x17000AE0 RID: 2784
		// (get) Token: 0x06007081 RID: 28801 RVA: 0x00243AFC File Offset: 0x00241CFC
		public Vector3 InstantaneousVelocity
		{
			get
			{
				return this.currentVelocity;
			}
		}

		// Token: 0x17000AE1 RID: 2785
		// (get) Token: 0x06007082 RID: 28802 RVA: 0x00243B04 File Offset: 0x00241D04
		public Vector3 AveragedVelocity
		{
			get
			{
				return this.averagedVelocity;
			}
		}

		// Token: 0x17000AE2 RID: 2786
		// (get) Token: 0x06007083 RID: 28803 RVA: 0x00243B0C File Offset: 0x00241D0C
		public Transform CosmeticsHeadTarget
		{
			get
			{
				return this.cosmeticsHeadTarget;
			}
		}

		// Token: 0x17000AE3 RID: 2787
		// (get) Token: 0x06007084 RID: 28804 RVA: 0x00243B14 File Offset: 0x00241D14
		public float scale
		{
			get
			{
				return this.scaleMultiplier * this.nativeScale;
			}
		}

		// Token: 0x17000AE4 RID: 2788
		// (get) Token: 0x06007085 RID: 28805 RVA: 0x00243B23 File Offset: 0x00241D23
		public float NativeScale
		{
			get
			{
				return this.nativeScale;
			}
		}

		// Token: 0x17000AE5 RID: 2789
		// (get) Token: 0x06007086 RID: 28806 RVA: 0x00243B2B File Offset: 0x00241D2B
		public float ScaleMultiplier
		{
			get
			{
				return this.scaleMultiplier;
			}
		}

		// Token: 0x06007087 RID: 28807 RVA: 0x00243B33 File Offset: 0x00241D33
		public void SetScaleMultiplier(float s)
		{
			this.scaleMultiplier = s;
		}

		// Token: 0x06007088 RID: 28808 RVA: 0x00243B3C File Offset: 0x00241D3C
		public void SetNativeScale(NativeSizeChangerSettings s)
		{
			float num = this.nativeScale;
			if (s != null && s.playerSizeScale > 0f && s.playerSizeScale != 1f)
			{
				this.activeSizeChangerSettings = s;
			}
			else
			{
				this.activeSizeChangerSettings = null;
			}
			if (this.activeSizeChangerSettings == null)
			{
				this.nativeScale = 1f;
			}
			else
			{
				this.nativeScale = this.activeSizeChangerSettings.playerSizeScale;
			}
			if (num != this.nativeScale && NetworkSystem.Instance.InRoom)
			{
				GorillaTagger.Instance.myVRRig != null;
			}
		}

		// Token: 0x17000AE6 RID: 2790
		// (get) Token: 0x06007089 RID: 28809 RVA: 0x00243BC7 File Offset: 0x00241DC7
		public bool IsDefaultScale
		{
			get
			{
				return Mathf.Abs(1f - this.scale) < 0.001f;
			}
		}

		// Token: 0x17000AE7 RID: 2791
		// (get) Token: 0x0600708A RID: 28810 RVA: 0x00243BE1 File Offset: 0x00241DE1
		public bool turnedThisFrame
		{
			get
			{
				return this.degreesTurnedThisFrame != 0f;
			}
		}

		// Token: 0x17000AE8 RID: 2792
		// (get) Token: 0x0600708B RID: 28811 RVA: 0x00243BF3 File Offset: 0x00241DF3
		public List<GTPlayer.MaterialData> materialData
		{
			get
			{
				return this.materialDatasSO.datas;
			}
		}

		// Token: 0x0600708C RID: 28812 RVA: 0x00243C00 File Offset: 0x00241E00
		public PlayerSwimmingParameters GetSwimmingParams(GTPlayer.LiquidType liquidType)
		{
			return this.swimmingParamsList[(int)liquidType];
		}

		// Token: 0x0600708D RID: 28813 RVA: 0x00243C0E File Offset: 0x00241E0E
		public PlayerSwimmingParameters GetSwimmingParams(WaterVolume volume)
		{
			return this.GetSwimmingParams((volume != null) ? volume.LiquidType : GTPlayer.LiquidType.Water);
		}

		// Token: 0x17000AE9 RID: 2793
		// (get) Token: 0x0600708E RID: 28814 RVA: 0x00243C28 File Offset: 0x00241E28
		// (set) Token: 0x0600708F RID: 28815 RVA: 0x00243C30 File Offset: 0x00241E30
		protected bool IsFrozen { get; set; }

		// Token: 0x17000AEA RID: 2794
		// (get) Token: 0x06007090 RID: 28816 RVA: 0x00243C39 File Offset: 0x00241E39
		// (set) Token: 0x06007091 RID: 28817 RVA: 0x00243C41 File Offset: 0x00241E41
		public bool forcedUnderwater { get; set; }

		// Token: 0x17000AEB RID: 2795
		// (get) Token: 0x06007092 RID: 28818 RVA: 0x00243C4A File Offset: 0x00241E4A
		// (set) Token: 0x06007093 RID: 28819 RVA: 0x00243C52 File Offset: 0x00241E52
		public float siJumpMultiplier { get; set; } = 1f;

		// Token: 0x17000AEC RID: 2796
		// (get) Token: 0x06007094 RID: 28820 RVA: 0x00243C5B File Offset: 0x00241E5B
		public List<WaterVolume> HeadOverlappingWaterVolumes
		{
			get
			{
				return this.headOverlappingWaterVolumes;
			}
		}

		// Token: 0x17000AED RID: 2797
		// (get) Token: 0x06007095 RID: 28821 RVA: 0x00243C63 File Offset: 0x00241E63
		public bool InWater
		{
			get
			{
				return this.bodyInWater;
			}
		}

		// Token: 0x17000AEE RID: 2798
		// (get) Token: 0x06007096 RID: 28822 RVA: 0x00243C6B File Offset: 0x00241E6B
		public bool HeadInWater
		{
			get
			{
				return this.headInWater;
			}
		}

		// Token: 0x17000AEF RID: 2799
		// (get) Token: 0x06007097 RID: 28823 RVA: 0x00243C73 File Offset: 0x00241E73
		public WaterVolume CurrentWaterVolume
		{
			get
			{
				if (this.bodyOverlappingWaterVolumes.Count <= 0 || !(this.bodyOverlappingWaterVolumes[0] != null))
				{
					return null;
				}
				return this.bodyOverlappingWaterVolumes[0];
			}
		}

		// Token: 0x17000AF0 RID: 2800
		// (get) Token: 0x06007098 RID: 28824 RVA: 0x00243CA5 File Offset: 0x00241EA5
		public WaterVolume.SurfaceQuery WaterSurfaceForHead
		{
			get
			{
				return this.waterSurfaceForHead;
			}
		}

		// Token: 0x17000AF1 RID: 2801
		// (get) Token: 0x06007099 RID: 28825 RVA: 0x00243CAD File Offset: 0x00241EAD
		public WaterVolume LeftHandWaterVolume
		{
			get
			{
				return this.leftHandWaterVolume;
			}
		}

		// Token: 0x17000AF2 RID: 2802
		// (get) Token: 0x0600709A RID: 28826 RVA: 0x00243CB5 File Offset: 0x00241EB5
		public WaterVolume RightHandWaterVolume
		{
			get
			{
				return this.rightHandWaterVolume;
			}
		}

		// Token: 0x17000AF3 RID: 2803
		// (get) Token: 0x0600709B RID: 28827 RVA: 0x00243CBD File Offset: 0x00241EBD
		public WaterVolume.SurfaceQuery LeftHandWaterSurface
		{
			get
			{
				return this.leftHandWaterSurface;
			}
		}

		// Token: 0x17000AF4 RID: 2804
		// (get) Token: 0x0600709C RID: 28828 RVA: 0x00243CC5 File Offset: 0x00241EC5
		public WaterVolume.SurfaceQuery RightHandWaterSurface
		{
			get
			{
				return this.rightHandWaterSurface;
			}
		}

		// Token: 0x17000AF5 RID: 2805
		// (get) Token: 0x0600709D RID: 28829 RVA: 0x00243CCD File Offset: 0x00241ECD
		public Vector3 LastLeftHandPosition
		{
			get
			{
				return this.leftHand.lastPosition;
			}
		}

		// Token: 0x17000AF6 RID: 2806
		// (get) Token: 0x0600709E RID: 28830 RVA: 0x00243CDA File Offset: 0x00241EDA
		public Vector3 LastRightHandPosition
		{
			get
			{
				return this.rightHand.lastPosition;
			}
		}

		// Token: 0x17000AF7 RID: 2807
		// (get) Token: 0x0600709F RID: 28831 RVA: 0x00243CE7 File Offset: 0x00241EE7
		public Vector3 RigidbodyVelocity
		{
			get
			{
				return this.playerRigidBody.linearVelocity;
			}
		}

		// Token: 0x17000AF8 RID: 2808
		// (get) Token: 0x060070A0 RID: 28832 RVA: 0x00243CF4 File Offset: 0x00241EF4
		public Vector3 HeadCenterPosition
		{
			get
			{
				return this.headCollider.transform.position + this.headCollider.transform.rotation * new Vector3(0f, 0f, -0.11f);
			}
		}

		// Token: 0x17000AF9 RID: 2809
		// (get) Token: 0x060070A1 RID: 28833 RVA: 0x00243D34 File Offset: 0x00241F34
		public bool HandContactingSurface
		{
			get
			{
				return this.leftHand.isColliding || this.rightHand.isColliding;
			}
		}

		// Token: 0x17000AFA RID: 2810
		// (get) Token: 0x060070A2 RID: 28834 RVA: 0x00243D50 File Offset: 0x00241F50
		public bool BodyOnGround
		{
			get
			{
				return this.bodyGroundContactTime >= Time.time - 0.05f;
			}
		}

		// Token: 0x17000AFB RID: 2811
		// (get) Token: 0x060070A3 RID: 28835 RVA: 0x00243D68 File Offset: 0x00241F68
		public bool IsGroundedHand
		{
			get
			{
				return this.HandContactingSurface || this.isClimbing || this.leftHand.isHolding || this.rightHand.isHolding;
			}
		}

		// Token: 0x17000AFC RID: 2812
		// (get) Token: 0x060070A4 RID: 28836 RVA: 0x00243D94 File Offset: 0x00241F94
		public bool IsGroundedButt
		{
			get
			{
				return this.BodyOnGround;
			}
		}

		// Token: 0x17000AFD RID: 2813
		// (get) Token: 0x060070A5 RID: 28837 RVA: 0x00243D9C File Offset: 0x00241F9C
		// (set) Token: 0x060070A6 RID: 28838 RVA: 0x00243DA4 File Offset: 0x00241FA4
		public int TentacleActiveAtFrame { get; set; }

		// Token: 0x17000AFE RID: 2814
		// (get) Token: 0x060070A7 RID: 28839 RVA: 0x00243DAD File Offset: 0x00241FAD
		public bool IsTentacleActive
		{
			get
			{
				return this.TentacleActiveAtFrame >= Time.frameCount;
			}
		}

		// Token: 0x17000AFF RID: 2815
		// (get) Token: 0x060070A8 RID: 28840 RVA: 0x00243DBF File Offset: 0x00241FBF
		// (set) Token: 0x060070A9 RID: 28841 RVA: 0x00243DC7 File Offset: 0x00241FC7
		public int LaserZiplineActiveAtFrame { get; set; }

		// Token: 0x17000B00 RID: 2816
		// (get) Token: 0x060070AA RID: 28842 RVA: 0x00243DD0 File Offset: 0x00241FD0
		public bool IsLaserZiplineActive
		{
			get
			{
				return this.LaserZiplineActiveAtFrame >= Time.frameCount;
			}
		}

		// Token: 0x17000B01 RID: 2817
		// (get) Token: 0x060070AB RID: 28843 RVA: 0x00243DE2 File Offset: 0x00241FE2
		// (set) Token: 0x060070AC RID: 28844 RVA: 0x00243DEA File Offset: 0x00241FEA
		public int ThrusterActiveAtFrame { get; set; }

		// Token: 0x17000B02 RID: 2818
		// (get) Token: 0x060070AD RID: 28845 RVA: 0x00243DF3 File Offset: 0x00241FF3
		public bool IsThrusterActive
		{
			get
			{
				return this.ThrusterActiveAtFrame >= Time.frameCount;
			}
		}

		// Token: 0x17000B03 RID: 2819
		// (set) Token: 0x060070AE RID: 28846 RVA: 0x00243E05 File Offset: 0x00242005
		public Quaternion PlayerRotationOverride
		{
			set
			{
				this.playerRotationOverride = value;
				this.playerRotationOverrideFrame = Time.frameCount;
			}
		}

		// Token: 0x17000B04 RID: 2820
		// (get) Token: 0x060070AF RID: 28847 RVA: 0x00243E19 File Offset: 0x00242019
		// (set) Token: 0x060070B0 RID: 28848 RVA: 0x00243E21 File Offset: 0x00242021
		public bool IsBodySliding { get; set; }

		// Token: 0x17000B05 RID: 2821
		// (get) Token: 0x060070B1 RID: 28849 RVA: 0x00243E2A File Offset: 0x0024202A
		// (set) Token: 0x060070B2 RID: 28850 RVA: 0x00243E32 File Offset: 0x00242032
		public bool bodyGroundIsSlippery { get; private set; }

		// Token: 0x17000B06 RID: 2822
		// (get) Token: 0x060070B3 RID: 28851 RVA: 0x00243E3B File Offset: 0x0024203B
		public GorillaClimbable CurrentClimbable
		{
			get
			{
				return this.currentClimbable;
			}
		}

		// Token: 0x17000B07 RID: 2823
		// (get) Token: 0x060070B4 RID: 28852 RVA: 0x00243E43 File Offset: 0x00242043
		public GorillaHandClimber CurrentClimber
		{
			get
			{
				return this.currentClimber;
			}
		}

		// Token: 0x17000B08 RID: 2824
		// (get) Token: 0x060070B5 RID: 28853 RVA: 0x00243E4B File Offset: 0x0024204B
		// (set) Token: 0x060070B6 RID: 28854 RVA: 0x00243E53 File Offset: 0x00242053
		public float jumpMultiplier
		{
			get
			{
				return this._jumpMultiplier;
			}
			set
			{
				this._jumpMultiplier = value;
			}
		}

		// Token: 0x17000B09 RID: 2825
		// (get) Token: 0x060070B7 RID: 28855 RVA: 0x00243E5C File Offset: 0x0024205C
		// (set) Token: 0x060070B8 RID: 28856 RVA: 0x00243E64 File Offset: 0x00242064
		public float LastTouchedGroundAtNetworkTime { get; private set; }

		// Token: 0x17000B0A RID: 2826
		// (get) Token: 0x060070B9 RID: 28857 RVA: 0x00243E6D File Offset: 0x0024206D
		// (set) Token: 0x060070BA RID: 28858 RVA: 0x00243E75 File Offset: 0x00242075
		public float LastHandTouchedGroundAtNetworkTime { get; private set; }

		// Token: 0x060070BB RID: 28859 RVA: 0x00243E80 File Offset: 0x00242080
		public void EnableStilt(StiltID stiltID, bool isLeftHand, Vector3 currentTipWorldPos, float maxArmLength, bool canTag, bool canStun, float customBoostFactor = 0f, GorillaVelocityTracker velocityTracker = null)
		{
			this.stiltStates[(int)stiltID] = new GTPlayer.HandState
			{
				isActive = true,
				controllerTransform = (isLeftHand ? this.leftHand : this.rightHand).controllerTransform,
				velocityTracker = ((velocityTracker != null) ? velocityTracker : (isLeftHand ? this.leftHand : this.rightHand).velocityTracker),
				handRotOffset = Quaternion.identity,
				canTag = canTag,
				canStun = canStun,
				customBoostFactor = customBoostFactor,
				hasCustomBoost = (customBoostFactor > 0f)
			};
			this.stiltStates[(int)stiltID].Init(this, isLeftHand, maxArmLength);
			this.UpdateStiltOffset(stiltID, currentTipWorldPos);
		}

		// Token: 0x060070BC RID: 28860 RVA: 0x00243F46 File Offset: 0x00242146
		public void DisableStilt(StiltID stiltID)
		{
			this.stiltStates[(int)stiltID].isActive = false;
		}

		// Token: 0x060070BD RID: 28861 RVA: 0x00243F5A File Offset: 0x0024215A
		public void UpdateStiltOffset(StiltID stiltID, Vector3 currentTipWorldPos)
		{
			this.stiltStates[(int)stiltID].handOffset = this.stiltStates[(int)stiltID].controllerTransform.InverseTransformPoint(currentTipWorldPos);
		}

		// Token: 0x060070BE RID: 28862 RVA: 0x00243F84 File Offset: 0x00242184
		private void Awake()
		{
			if (GTPlayer._instance != null && GTPlayer._instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				GTPlayer._instance = this;
				GTPlayer.hasInstance = true;
			}
			this.InitializeValues();
			this.playerRigidbodyInterpolationDefault = this.playerRigidBody.interpolation;
			this.playerRigidBody.maxAngularVelocity = 0f;
			this.bodyOffsetVector = new Vector3(0f, -this.bodyCollider.height / 2f, 0f);
			this._bodyInitialHeight = this.bodyCollider.height;
			this.bodyInitialRadius = this.bodyCollider.radius;
			this.rayCastNonAllocColliders = new RaycastHit[5];
			this.crazyCheckVectors = new Vector3[7];
			this.emptyHit = default(RaycastHit);
			this.crazyCheckVectors[0] = Vector3.up;
			this.crazyCheckVectors[1] = Vector3.down;
			this.crazyCheckVectors[2] = Vector3.left;
			this.crazyCheckVectors[3] = Vector3.right;
			this.crazyCheckVectors[4] = Vector3.forward;
			this.crazyCheckVectors[5] = Vector3.back;
			this.crazyCheckVectors[6] = Vector3.zero;
			if (this.controllerState == null)
			{
				this.controllerState = base.GetComponent<ConnectedControllerHandler>();
			}
			this.layerChanger = base.GetComponent<LayerChanger>();
			this.bodyTouchedSurfaces = new Dictionary<GameObject, PhysicsMaterial>();
			if (Application.isPlaying)
			{
				Application.onBeforeRender += this.OnBeforeRenderInit;
			}
		}

		// Token: 0x060070BF RID: 28863 RVA: 0x00244118 File Offset: 0x00242318
		protected void Start()
		{
			if (this.mainCamera == null)
			{
				this.mainCamera = Camera.main;
			}
			this.mainCamera.farClipPlane = 500f;
			this.lastScale = this.scale;
			this.layerChanger.InitializeLayers(base.transform);
			float num = Quaternion.Angle(Quaternion.identity, GorillaTagger.Instance.offlineVRRig.transform.rotation) * Mathf.Sign(Vector3.Dot(Vector3.up, GorillaTagger.Instance.offlineVRRig.transform.right));
			this.Turn(num);
		}

		// Token: 0x060070C0 RID: 28864 RVA: 0x002441B5 File Offset: 0x002423B5
		protected void OnDestroy()
		{
			if (GTPlayer._instance == this)
			{
				GTPlayer._instance = null;
				GTPlayer.hasInstance = false;
			}
			if (this.climbHelper)
			{
				Object.Destroy(this.climbHelper.gameObject);
			}
		}

		// Token: 0x060070C1 RID: 28865 RVA: 0x002441F0 File Offset: 0x002423F0
		public void InitializeValues()
		{
			Physics.SyncTransforms();
			this.playerRigidBody = base.GetComponent<Rigidbody>();
			this.velocityHistory = new Vector3[this.velocityHistorySize];
			this.slideAverageHistory = new Vector3[this.velocityHistorySize];
			for (int i = 0; i < this.velocityHistory.Length; i++)
			{
				this.velocityHistory[i] = Vector3.zero;
				this.slideAverageHistory[i] = Vector3.zero;
			}
			this.leftHand.Init(this, true, this.maxArmLength);
			this.rightHand.Init(this, false, this.maxArmLength);
			this.lastHeadPosition = this.headCollider.transform.position;
			this.velocityIndex = 0;
			this.averagedVelocity = Vector3.zero;
			this.slideVelocity = Vector3.zero;
			this.lastPosition = base.transform.position;
			this.lastRealTime = Time.realtimeSinceStartup;
			this.lastOpenHeadPosition = this.headCollider.transform.position;
			this.bodyCollider.transform.position = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + this.bodyOffsetVector;
			this.bodyCollider.transform.eulerAngles = new Vector3(0f, this.headCollider.transform.eulerAngles.y, 0f);
			this.ForceRigidBodySync();
		}

		// Token: 0x060070C2 RID: 28866 RVA: 0x0024435C File Offset: 0x0024255C
		public void SetHalloweenLevitation(float levitateStrength, float levitateDuration, float levitateBlendOutDuration, float levitateBonusStrength, float levitateBonusOffAtYSpeed, float levitateBonusFullAtYSpeed)
		{
			this.halloweenLevitationStrength = levitateStrength;
			this.halloweenLevitationFullStrengthDuration = levitateDuration;
			this.halloweenLevitationTotalDuration = levitateDuration + levitateBlendOutDuration;
			this.halloweenLevitateBonusFullAtYSpeed = levitateBonusFullAtYSpeed;
			this.halloweenLevitateBonusOffAtYSpeed = levitateBonusFullAtYSpeed;
			this.halloweenLevitationBonusStrength = levitateBonusStrength;
		}

		// Token: 0x060070C3 RID: 28867 RVA: 0x0024438D File Offset: 0x0024258D
		public void TeleportToTrain(bool enable)
		{
			this.teleportToTrain = enable;
		}

		// Token: 0x060070C4 RID: 28868 RVA: 0x00244398 File Offset: 0x00242598
		public void TeleportCleanup()
		{
			this.ClearHandHolds();
			this.leftHand.OnTeleport();
			this.rightHand.OnTeleport();
			this.lastHeadPosition = this.headCollider.transform.position;
			this.lastOpenHeadPosition = this.lastHeadPosition;
			this.lastPosition = base.transform.position;
			for (int i = 0; i < 12; i++)
			{
				if (this.stiltStates[i].isActive)
				{
					this.stiltStates[i].OnTeleport();
				}
			}
			Physics.SyncTransforms();
			GorillaTagger.Instance.offlineVRRig.transform.position = this.lastPosition;
			GorillaTagger.Instance.offlineVRRig.leftHandLink.BreakLink();
			GorillaTagger.Instance.offlineVRRig.rightHandLink.BreakLink();
			this.ForceRigidBodySync();
		}

		// Token: 0x060070C5 RID: 28869 RVA: 0x00244474 File Offset: 0x00242674
		public void TeleportTo(Vector3 position, Quaternion rotation, bool keepVelocity = false, bool center = false)
		{
			if (center)
			{
				Vector3 position2 = base.transform.position;
				Vector3 vector = this.mainCamera.transform.position - position2;
				position -= vector;
			}
			this.ClearHandHolds();
			if (this.playerRigidBody != null)
			{
				Quaternion quaternion = rotation * Quaternion.Inverse(this.playerRigidBody.rotation);
				Vector3 linearVelocity = this.playerRigidBody.linearVelocity;
				this.playerRigidBody.isKinematic = true;
				this.playerRigidBody.position = position;
				this.playerRigidBody.rotation = rotation;
				this.playerRigidBody.isKinematic = false;
				this.playerRigidBody.linearVelocity = quaternion * linearVelocity;
			}
			base.transform.position = position;
			base.transform.rotation = rotation;
			this.lastHeadPosition = this.headCollider.transform.position;
			this.lastPosition = position;
			this.lastOpenHeadPosition = this.headCollider.transform.position;
			this.leftHand.OnTeleport();
			this.rightHand.OnTeleport();
			for (int i = 0; i < 12; i++)
			{
				if (this.stiltStates[i].isActive)
				{
					this.stiltStates[i].OnTeleport();
				}
			}
			if (!keepVelocity)
			{
				this.playerRigidBody.linearVelocity = Vector3.zero;
			}
			this.bodyCollider.transform.position = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + this.bodyOffsetVector;
			this.bodyCollider.transform.eulerAngles = new Vector3(0f, this.headCollider.transform.eulerAngles.y, 0f);
			Physics.SyncTransforms();
			GorillaTagger.Instance.offlineVRRig.transform.position = position;
			GorillaTagger.Instance.offlineVRRig.leftHandLink.BreakLink();
			GorillaTagger.Instance.offlineVRRig.rightHandLink.BreakLink();
			this.ForceRigidBodySync();
		}

		// Token: 0x060070C6 RID: 28870 RVA: 0x00244684 File Offset: 0x00242884
		public void TeleportTo(Transform destination, bool matchDestinationRotation = true, bool maintainVelocity = true)
		{
			Vector3 position = base.transform.position;
			Vector3 vector = this.mainCamera.transform.position - position;
			Vector3 vector2 = destination.position - vector;
			float num = destination.rotation.eulerAngles.y - this.mainCamera.transform.rotation.eulerAngles.y;
			Vector3 vector3 = this.currentVelocity;
			if (!maintainVelocity)
			{
				this.SetPlayerVelocity(Vector3.zero);
			}
			else if (matchDestinationRotation)
			{
				vector3 = Quaternion.AngleAxis(num, base.transform.up) * this.currentVelocity;
				this.SetPlayerVelocity(vector3);
			}
			if (matchDestinationRotation)
			{
				this.Turn(num);
			}
			this.TeleportTo(vector2, base.transform.rotation, false, false);
			if (maintainVelocity)
			{
				this.SetPlayerVelocity(vector3);
			}
			this.ForceRigidBodySync();
		}

		// Token: 0x060070C7 RID: 28871 RVA: 0x00244765 File Offset: 0x00242965
		public void AddForce(Vector3 force, ForceMode mode)
		{
			if (mode == ForceMode.VelocityChange)
			{
				this.playerRigidBody.AddForce(force * this.playerRigidBody.mass, ForceMode.Impulse);
				return;
			}
			this.playerRigidBody.AddForce(force, mode);
		}

		// Token: 0x060070C8 RID: 28872 RVA: 0x00244798 File Offset: 0x00242998
		public void SetPlayerVelocity(Vector3 newVelocity)
		{
			for (int i = 0; i < this.velocityHistory.Length; i++)
			{
				this.velocityHistory[i] = newVelocity;
			}
			this.playerRigidBody.AddForce(newVelocity - this.playerRigidBody.linearVelocity, ForceMode.VelocityChange);
		}

		// Token: 0x17000B0B RID: 2827
		// (get) Token: 0x060070C9 RID: 28873 RVA: 0x002447E2 File Offset: 0x002429E2
		public int GravityOverrideCount
		{
			get
			{
				return this.gravityOverrides.Count;
			}
		}

		// Token: 0x060070CA RID: 28874 RVA: 0x002447EF File Offset: 0x002429EF
		public void SetGravityOverride(Object caller, Action<GTPlayer> gravityFunction)
		{
			this.gravityOverrides[caller] = gravityFunction;
		}

		// Token: 0x060070CB RID: 28875 RVA: 0x002447FE File Offset: 0x002429FE
		public void UnsetGravityOverride(Object caller)
		{
			this.gravityOverrides.Remove(caller);
		}

		// Token: 0x060070CC RID: 28876 RVA: 0x00244810 File Offset: 0x00242A10
		private void ApplyGravityOverrides()
		{
			foreach (KeyValuePair<Object, Action<GTPlayer>> keyValuePair in this.gravityOverrides)
			{
				if (keyValuePair.Value != null)
				{
					keyValuePair.Value(this);
				}
			}
		}

		// Token: 0x060070CD RID: 28877 RVA: 0x00244874 File Offset: 0x00242A74
		public void ApplyKnockback(Vector3 direction, float speed, bool forceOffTheGround = false)
		{
			if (forceOffTheGround)
			{
				if (this.leftHand.wasColliding || this.rightHand.wasColliding)
				{
					this.leftHand.wasColliding = false;
					this.rightHand.wasColliding = false;
					this.playerRigidBody.transform.position += this.minimumRaycastDistance * this.scale * Vector3.up;
				}
				this.didAJump = true;
				this.SetMaximumSlipThisFrame();
			}
			if (speed > 0.01f)
			{
				float num = Vector3.Dot(this.averagedVelocity, direction);
				float num2 = Mathf.InverseLerp(1.5f, 0.5f, num / speed);
				Vector3 vector = this.averagedVelocity + direction * speed * num2;
				this.playerRigidBody.linearVelocity = vector;
				for (int i = 0; i < this.velocityHistory.Length; i++)
				{
					this.velocityHistory[i] = vector;
				}
			}
		}

		// Token: 0x060070CE RID: 28878 RVA: 0x00244964 File Offset: 0x00242B64
		public void ApplyClampedKnockback(Vector3 direction, float speed, float boostMultiplier, bool forceOffTheGround = false)
		{
			if (forceOffTheGround)
			{
				if (this.leftHand.wasColliding || this.rightHand.wasColliding)
				{
					this.leftHand.wasColliding = false;
					this.rightHand.wasColliding = false;
					this.playerRigidBody.transform.position += this.minimumRaycastDistance * this.scale * Vector3.up;
				}
				this.didAJump = true;
				this.SetMaximumSlipThisFrame();
			}
			if (speed > 0.01f)
			{
				float num = Vector3.Dot(this.playerRigidBody.linearVelocity, direction.normalized);
				if (num >= speed)
				{
					return;
				}
				float num2 = Mathf.Clamp(speed - num, 0f, speed * boostMultiplier);
				Vector3 vector = this.playerRigidBody.linearVelocity + direction.normalized * num2;
				this.playerRigidBody.linearVelocity = vector;
				for (int i = 0; i < this.velocityHistory.Length; i++)
				{
					this.velocityHistory[i] = vector;
				}
			}
		}

		// Token: 0x060070CF RID: 28879 RVA: 0x00244A68 File Offset: 0x00242C68
		public void FixedUpdate()
		{
			this.AntiTeleportTechnology();
			this.IsFrozen = GorillaTagger.Instance.offlineVRRig.IsFrozen || this.debugFreezeTag;
			bool isDefaultScale = this.IsDefaultScale;
			this.playerRigidBody.useGravity = false;
			if (this.gravityOverrides.Count > 0)
			{
				this.ApplyGravityOverrides();
			}
			else if (this.halloweenLevitationBonusStrength > 0f || this.halloweenLevitationStrength > 0f)
			{
				float num = Time.time - this.lastTouchedGroundTimestamp;
				if (num < this.halloweenLevitationTotalDuration)
				{
					this.playerRigidBody.AddForce(Vector3.up * (this.halloweenLevitationStrength * Mathf.InverseLerp(this.halloweenLevitationFullStrengthDuration, this.halloweenLevitationTotalDuration, num)), ForceMode.Acceleration);
				}
				float y = this.playerRigidBody.linearVelocity.y;
				if (y <= this.halloweenLevitateBonusFullAtYSpeed)
				{
					this.playerRigidBody.AddForce(Vector3.up * this.halloweenLevitationBonusStrength, ForceMode.Acceleration);
				}
				else if (y <= this.halloweenLevitateBonusOffAtYSpeed)
				{
					float num2 = Mathf.InverseLerp(this.halloweenLevitateBonusOffAtYSpeed, this.halloweenLevitateBonusFullAtYSpeed, this.playerRigidBody.linearVelocity.y);
					this.playerRigidBody.AddForce(Vector3.up * (this.halloweenLevitationBonusStrength * num2), ForceMode.Acceleration);
				}
			}
			if (this.enableHoverMode)
			{
				this.playerRigidBody.linearVelocity = this.HoverboardFixedUpdate(this.playerRigidBody.linearVelocity);
			}
			else
			{
				this.didHoverLastFrame = false;
			}
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.bodyInWater = false;
			Vector3 vector = this.swimmingVelocity;
			PlayerSwimmingParameters swimmingParams = this.GetSwimmingParams(GTPlayer.LiquidType.Water);
			this.swimmingVelocity = Vector3.MoveTowards(this.swimmingVelocity, Vector3.zero, swimmingParams.swimmingVelocityOutOfWaterDrainRate * fixedDeltaTime);
			this.leftHandNonDiveHapticsAmount = 0f;
			this.rightHandNonDiveHapticsAmount = 0f;
			if (this.bodyOverlappingWaterVolumes.Count > 0 || this.forcedUnderwater)
			{
				WaterVolume waterVolume = null;
				float num3 = float.MinValue;
				Vector3 vector2 = this.headCollider.transform.position + GTPlayerTransform.PhysicsDown * swimmingParams.floatingWaterLevelBelowHead * this.scale;
				this.activeWaterCurrents.Clear();
				for (int i = 0; i < this.bodyOverlappingWaterVolumes.Count; i++)
				{
					WaterVolume.SurfaceQuery surfaceQuery;
					if (this.bodyOverlappingWaterVolumes[i] == null)
					{
						this.bodyOverlappingWaterVolumes.RemoveAt(i);
						i--;
					}
					else if (this.bodyOverlappingWaterVolumes[i].GetSurfaceQueryForPoint(vector2, out surfaceQuery, false))
					{
						float num4 = Vector3.Dot(surfaceQuery.surfacePoint - vector2, surfaceQuery.surfaceNormal);
						if (num4 > num3)
						{
							num3 = num4;
							waterVolume = this.bodyOverlappingWaterVolumes[i];
							this.waterSurfaceForHead = surfaceQuery;
						}
						WaterCurrent waterCurrent = this.bodyOverlappingWaterVolumes[i].Current;
						if (waterCurrent != null && num4 > 0f && !this.activeWaterCurrents.Contains(waterCurrent))
						{
							this.activeWaterCurrents.Add(waterCurrent);
						}
					}
				}
				if (this.forcedUnderwater && waterVolume == null)
				{
					this.waterSurfaceForHead = new WaterVolume.SurfaceQuery
					{
						surfacePoint = this.headCollider.transform.position + GTPlayerTransform.PhysicsUp * 1000f,
						surfaceNormal = GTPlayerTransform.PhysicsUp,
						maxDepth = 2000f
					};
					num3 = 1000f;
				}
				if (waterVolume != null || this.forcedUnderwater)
				{
					Vector3 linearVelocity = this.playerRigidBody.linearVelocity;
					float magnitude = linearVelocity.magnitude;
					bool flag = this.headInWater;
					float num5 = Vector3.Dot(this.waterSurfaceForHead.surfacePoint - this.headCollider.transform.position, this.waterSurfaceForHead.surfaceNormal);
					float num6 = Vector3.Dot(this.headCollider.transform.position - (this.waterSurfaceForHead.surfacePoint - this.waterSurfaceForHead.surfaceNormal * this.waterSurfaceForHead.maxDepth), this.waterSurfaceForHead.surfaceNormal);
					this.headInWater = (this.forcedUnderwater || (num5 > 0f && num6 > 0f)) && waterVolume != null && waterVolume.LiquidType != GTPlayer.LiquidType.SwimInAir;
					if (this.headInWater && !flag)
					{
						this.audioSetToUnderwater = true;
						this.audioManager.SetMixerSnapshot(this.audioManager.underwaterSnapshot, 0.1f);
					}
					else if (!this.headInWater && flag)
					{
						this.audioSetToUnderwater = false;
						this.audioManager.UnsetMixerSnapshot(0.1f);
					}
					float num7 = Vector3.Dot(this.waterSurfaceForHead.surfacePoint - vector2, this.waterSurfaceForHead.surfaceNormal);
					float num8 = Vector3.Dot(vector2 - (this.waterSurfaceForHead.surfacePoint - this.waterSurfaceForHead.surfaceNormal * this.waterSurfaceForHead.maxDepth), this.waterSurfaceForHead.surfaceNormal);
					this.bodyInWater = this.forcedUnderwater || (num7 > 0f && num8 > 0f);
					if (this.bodyInWater)
					{
						GTPlayer.LiquidProperties liquidProperties = this.liquidPropertiesList[(int)((waterVolume != null) ? waterVolume.LiquidType : GTPlayer.LiquidType.Water)];
						PlayerSwimmingParameters swimmingParams2 = this.GetSwimmingParams(waterVolume);
						float num12;
						if (swimmingParams2.extendBouyancyFromSpeed)
						{
							float num9 = Mathf.Clamp(Vector3.Dot(linearVelocity / this.scale, this.waterSurfaceForHead.surfaceNormal), swimmingParams2.speedToBouyancyExtensionMinMax.x, swimmingParams2.speedToBouyancyExtensionMinMax.y);
							float num10 = swimmingParams2.speedToBouyancyExtension.Evaluate(num9);
							this.buoyancyExtension = Mathf.Max(this.buoyancyExtension, num10);
							float num11 = Mathf.InverseLerp(0f, swimmingParams2.buoyancyFadeDist + this.buoyancyExtension, num3 / this.scale + this.buoyancyExtension);
							this.buoyancyExtension = Spring.DamperDecayExact(this.buoyancyExtension, swimmingParams2.buoyancyExtensionDecayHalflife, fixedDeltaTime, 1E-05f);
							num12 = num11;
						}
						else
						{
							num12 = Mathf.InverseLerp(0f, swimmingParams2.buoyancyFadeDist, num3 / this.scale);
						}
						Vector3 vector3 = -(GTPlayerTransform.PhysicsDown * Physics.gravity.magnitude * this.scale) * (liquidProperties.buoyancy * num12);
						if (this.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
						{
							vector3 *= this.frozenBodyBuoyancyFactor;
						}
						this.playerRigidBody.AddForce(vector3, ForceMode.Acceleration);
						Vector3 vector4 = Vector3.zero;
						Vector3 vector5 = Vector3.zero;
						for (int j = 0; j < this.activeWaterCurrents.Count; j++)
						{
							WaterCurrent waterCurrent2 = this.activeWaterCurrents[j];
							Vector3 vector6 = linearVelocity + vector4;
							Vector3 vector7;
							Vector3 vector8;
							if (waterCurrent2.GetCurrentAtPoint(this.bodyCollider.transform.position, vector6, fixedDeltaTime, out vector7, out vector8))
							{
								vector5 += vector7;
								vector4 += vector8;
							}
						}
						if (magnitude > Mathf.Epsilon)
						{
							float num13 = 0.01f;
							Vector3 vector9 = linearVelocity / magnitude;
							Vector3 right = this.leftHand.handFollower.right;
							Vector3 vector10 = -this.rightHand.handFollower.right;
							Vector3 forward = this.leftHand.handFollower.forward;
							Vector3 forward2 = this.rightHand.handFollower.forward;
							Vector3 vector11 = vector9;
							float num14 = 0f;
							float num15 = 0f;
							float num16 = 0f;
							if (swimmingParams2.applyDiveSteering && !this.disableMovement && isDefaultScale)
							{
								float num17 = Vector3.Dot(linearVelocity - vector5, vector9);
								float num18 = Mathf.Clamp(num17, swimmingParams2.swimSpeedToRedirectAmountMinMax.x, swimmingParams2.swimSpeedToRedirectAmountMinMax.y);
								float num19 = swimmingParams2.swimSpeedToRedirectAmount.Evaluate(num18);
								num18 = Mathf.Clamp(num17, swimmingParams2.swimSpeedToMaxRedirectAngleMinMax.x, swimmingParams2.swimSpeedToMaxRedirectAngleMinMax.y);
								float num20 = swimmingParams2.swimSpeedToMaxRedirectAngle.Evaluate(num18);
								float num21 = Mathf.Acos(Vector3.Dot(vector9, forward)) / 3.1415927f * -2f + 1f;
								float num22 = Mathf.Acos(Vector3.Dot(vector9, forward2)) / 3.1415927f * -2f + 1f;
								float num23 = Mathf.Clamp(num21, swimmingParams2.palmFacingToRedirectAmountMinMax.x, swimmingParams2.palmFacingToRedirectAmountMinMax.y);
								float num24 = Mathf.Clamp(num22, swimmingParams2.palmFacingToRedirectAmountMinMax.x, swimmingParams2.palmFacingToRedirectAmountMinMax.y);
								float num25 = ((!float.IsNaN(num23)) ? swimmingParams2.palmFacingToRedirectAmount.Evaluate(num23) : 0f);
								float num26 = ((!float.IsNaN(num24)) ? swimmingParams2.palmFacingToRedirectAmount.Evaluate(num24) : 0f);
								Vector3 vector12 = Vector3.ProjectOnPlane(vector9, right);
								Vector3 vector13 = Vector3.ProjectOnPlane(vector9, right);
								float num27 = Mathf.Min(vector12.magnitude, 1f);
								float num28 = Mathf.Min(vector13.magnitude, 1f);
								float magnitude2 = this.leftHand.velocityTracker.GetAverageVelocity(false, swimmingParams2.diveVelocityAveragingWindow, false).magnitude;
								float magnitude3 = this.rightHand.velocityTracker.GetAverageVelocity(false, swimmingParams2.diveVelocityAveragingWindow, false).magnitude;
								float num29 = Mathf.Clamp(magnitude2, swimmingParams2.handSpeedToRedirectAmountMinMax.x, swimmingParams2.handSpeedToRedirectAmountMinMax.y);
								float num30 = Mathf.Clamp(magnitude3, swimmingParams2.handSpeedToRedirectAmountMinMax.x, swimmingParams2.handSpeedToRedirectAmountMinMax.y);
								float num31 = swimmingParams2.handSpeedToRedirectAmount.Evaluate(num29);
								float num32 = swimmingParams2.handSpeedToRedirectAmount.Evaluate(num30);
								float averageSpeedChangeMagnitudeInDirection = this.leftHand.velocityTracker.GetAverageSpeedChangeMagnitudeInDirection(right, false, swimmingParams2.diveVelocityAveragingWindow);
								float averageSpeedChangeMagnitudeInDirection2 = this.rightHand.velocityTracker.GetAverageSpeedChangeMagnitudeInDirection(vector10, false, swimmingParams2.diveVelocityAveragingWindow);
								float num33 = Mathf.Clamp(averageSpeedChangeMagnitudeInDirection, swimmingParams2.handAccelToRedirectAmountMinMax.x, swimmingParams2.handAccelToRedirectAmountMinMax.y);
								float num34 = Mathf.Clamp(averageSpeedChangeMagnitudeInDirection2, swimmingParams2.handAccelToRedirectAmountMinMax.x, swimmingParams2.handAccelToRedirectAmountMinMax.y);
								float num35 = swimmingParams2.handAccelToRedirectAmount.Evaluate(num33);
								float num36 = swimmingParams2.handAccelToRedirectAmount.Evaluate(num34);
								num14 = Mathf.Min(num25, Mathf.Min(num31, num35));
								float num37 = ((Vector3.Dot(vector9, forward) > 0f) ? (Mathf.Min(num14, num19) * num27) : 0f);
								num15 = Mathf.Min(num26, Mathf.Min(num32, num36));
								float num38 = ((Vector3.Dot(vector9, forward2) > 0f) ? (Mathf.Min(num15, num19) * num28) : 0f);
								if (swimmingParams2.reduceDiveSteeringBelowVelocityPlane)
								{
									Vector3 vector14;
									if (Vector3.Dot(this.headCollider.transform.up, vector9) > 0.95f)
									{
										vector14 = -this.headCollider.transform.forward;
									}
									else
									{
										vector14 = Vector3.Cross(Vector3.Cross(vector9, this.headCollider.transform.up), vector9).normalized;
									}
									Vector3 position = this.headCollider.transform.position;
									Vector3 vector15 = position - this.leftHand.handFollower.position;
									Vector3 vector16 = position - this.rightHand.handFollower.position;
									float reduceDiveSteeringBelowPlaneFadeStartDist = swimmingParams2.reduceDiveSteeringBelowPlaneFadeStartDist;
									float reduceDiveSteeringBelowPlaneFadeEndDist = swimmingParams2.reduceDiveSteeringBelowPlaneFadeEndDist;
									float num39 = Vector3.Dot(vector15, GTPlayerTransform.PhysicsUp);
									float num40 = Vector3.Dot(vector16, GTPlayerTransform.PhysicsUp);
									float num41 = Vector3.Dot(vector15, vector14);
									float num42 = Vector3.Dot(vector16, vector14);
									float num43 = 1f - Mathf.InverseLerp(reduceDiveSteeringBelowPlaneFadeStartDist, reduceDiveSteeringBelowPlaneFadeEndDist, Mathf.Min(Mathf.Abs(num39), Mathf.Abs(num41)));
									float num44 = 1f - Mathf.InverseLerp(reduceDiveSteeringBelowPlaneFadeStartDist, reduceDiveSteeringBelowPlaneFadeEndDist, Mathf.Min(Mathf.Abs(num40), Mathf.Abs(num42)));
									num37 *= num43;
									num38 *= num44;
								}
								float num45 = num38 + num37;
								Vector3 vector17 = Vector3.zero;
								if (swimmingParams2.applyDiveSteering && num45 > num13)
								{
									vector17 = ((num37 * vector12 + num38 * vector13) / num45).normalized;
									vector17 = Vector3.Lerp(vector9, vector17, num45);
									vector11 = Vector3.RotateTowards(vector9, vector17, 0.017453292f * num20 * fixedDeltaTime, 0f);
								}
								else
								{
									vector11 = vector9;
								}
								num16 = Mathf.Clamp01((num14 + num15) * 0.5f);
							}
							float num46 = Mathf.Clamp(Vector3.Dot(vector, vector9), 0f, magnitude);
							float num47 = magnitude - num46;
							if (swimmingParams2.applyDiveSwimVelocityConversion && !this.disableMovement && num16 > num13 && num46 < swimmingParams2.diveMaxSwimVelocityConversion)
							{
								float num48 = Mathf.Min(swimmingParams2.diveSwimVelocityConversionRate * fixedDeltaTime, num47) * num16;
								num46 += num48;
								num47 -= num48;
							}
							float num49 = swimmingParams2.swimUnderWaterDampingHalfLife * liquidProperties.dampingFactor;
							float num50 = swimmingParams2.baseUnderWaterDampingHalfLife * liquidProperties.dampingFactor;
							float num51 = Spring.DamperDecayExact(num46 / this.scale, num49, fixedDeltaTime, 1E-05f) * this.scale;
							float num52 = Spring.DamperDecayExact(num47 / this.scale, num50, fixedDeltaTime, 1E-05f) * this.scale;
							if (swimmingParams2.applyDiveDampingMultiplier && !this.disableMovement)
							{
								float num53 = Mathf.Lerp(1f, swimmingParams2.diveDampingMultiplier, num16);
								num51 = Mathf.Lerp(num46, num51, num53);
								num52 = Mathf.Lerp(num47, num52, num53);
								float num54 = Mathf.Clamp((1f - num14) * (num46 + num47), swimmingParams2.nonDiveDampingHapticsAmountMinMax.x + num13, swimmingParams2.nonDiveDampingHapticsAmountMinMax.y - num13);
								float num55 = Mathf.Clamp((1f - num15) * (num46 + num47), swimmingParams2.nonDiveDampingHapticsAmountMinMax.x + num13, swimmingParams2.nonDiveDampingHapticsAmountMinMax.y - num13);
								this.leftHandNonDiveHapticsAmount = swimmingParams2.nonDiveDampingHapticsAmount.Evaluate(num54);
								this.rightHandNonDiveHapticsAmount = swimmingParams2.nonDiveDampingHapticsAmount.Evaluate(num55);
							}
							this.swimmingVelocity = num51 * vector11 + vector4 * this.scale;
							this.playerRigidBody.linearVelocity = this.swimmingVelocity + num52 * vector11;
						}
					}
				}
			}
			else if (this.audioSetToUnderwater)
			{
				this.audioSetToUnderwater = false;
				this.audioManager.UnsetMixerSnapshot(0.1f);
			}
			this.handleClimbing(Time.fixedDeltaTime);
			this.stuckHandsCheckFixedUpdate();
			this.FixedUpdate_HandHolds(Time.fixedDeltaTime);
		}

		// Token: 0x17000B0C RID: 2828
		// (get) Token: 0x060070D0 RID: 28880 RVA: 0x00245928 File Offset: 0x00243B28
		// (set) Token: 0x060070D1 RID: 28881 RVA: 0x00245930 File Offset: 0x00243B30
		public bool isHoverAllowed { get; private set; }

		// Token: 0x17000B0D RID: 2829
		// (get) Token: 0x060070D2 RID: 28882 RVA: 0x00245939 File Offset: 0x00243B39
		// (set) Token: 0x060070D3 RID: 28883 RVA: 0x00245941 File Offset: 0x00243B41
		public bool enableHoverMode { get; private set; }

		// Token: 0x060070D4 RID: 28884 RVA: 0x0024594A File Offset: 0x00243B4A
		public void SetHoverboardPosRot(Vector3 worldPos, Quaternion worldRot)
		{
			this.hoverboardPlayerLocalPos = this.headCollider.transform.InverseTransformPoint(worldPos);
			this.hoverboardPlayerLocalRot = this.headCollider.transform.InverseTransformRotation(worldRot);
		}

		// Token: 0x060070D5 RID: 28885 RVA: 0x0024597C File Offset: 0x00243B7C
		private void HoverboardLateUpdate()
		{
			Vector3 eulerAngles = this.headCollider.transform.eulerAngles;
			bool flag = false;
			for (int i = 0; i < this.hoverboardCasts.Length; i++)
			{
				GTPlayer.HoverBoardCast hoverBoardCast = this.hoverboardCasts[i];
				RaycastHit raycastHit;
				hoverBoardCast.didHit = Physics.SphereCast(new Ray(this.hoverboardVisual.transform.TransformPoint(hoverBoardCast.localOrigin), this.hoverboardVisual.transform.rotation * hoverBoardCast.localDirection), hoverBoardCast.sphereRadius, out raycastHit, hoverBoardCast.distance, this.hoverboardLocomotionLayers);
				if (hoverBoardCast.didHit)
				{
					HoverboardCantHover hoverboardCantHover;
					if (raycastHit.collider.TryGetComponent<HoverboardCantHover>(out hoverboardCantHover))
					{
						hoverBoardCast.didHit = false;
					}
					else
					{
						hoverBoardCast.pointHit = raycastHit.point;
						hoverBoardCast.normalHit = raycastHit.normal;
					}
				}
				this.hoverboardCasts[i] = hoverBoardCast;
				if (hoverBoardCast.didHit)
				{
					flag = true;
				}
			}
			this.hasHoverPoint = flag;
			this.bodyCollider.enabled = (this.bodyCollider.transform.position - this.hoverboardVisual.transform.TransformPoint(GTPlayerTransform.Up * this.hoverBodyCollisionRadiusUpOffset)).IsLongerThan(this.hoverBodyHasCollisionsOutsideRadius);
		}

		// Token: 0x060070D6 RID: 28886 RVA: 0x00245AC4 File Offset: 0x00243CC4
		private Vector3 HoverboardFixedUpdate(Vector3 velocity)
		{
			this.hoverboardVisual.transform.position = this.headCollider.transform.TransformPoint(this.hoverboardPlayerLocalPos);
			this.hoverboardVisual.transform.rotation = this.headCollider.transform.TransformRotation(this.hoverboardPlayerLocalRot);
			if (this.didHoverLastFrame)
			{
				velocity += Vector3.up * this.hoverGeneralUpwardForce * Time.fixedDeltaTime;
			}
			Vector3 position = this.hoverboardVisual.transform.position;
			Vector3 vector = position + velocity * Time.fixedDeltaTime;
			Vector3 vector2 = this.hoverboardVisual.transform.forward;
			Vector3 vector3 = (this.hoverboardCasts[0].didHit ? this.hoverboardCasts[0].normalHit : Vector3.up);
			bool flag = false;
			for (int i = 0; i < this.hoverboardCasts.Length; i++)
			{
				GTPlayer.HoverBoardCast hoverBoardCast = this.hoverboardCasts[i];
				if (hoverBoardCast.didHit)
				{
					Vector3 vector4 = position + Vector3.Project(hoverBoardCast.pointHit - position, vector2);
					Vector3 vector5 = vector + Vector3.Project(hoverBoardCast.pointHit - position, vector2);
					bool flag2 = hoverBoardCast.isSolid || Vector3.Dot(hoverBoardCast.normalHit, hoverBoardCast.pointHit - vector5) + this.hoverIdealHeight > 0f;
					float num = (hoverBoardCast.isSolid ? (Vector3.Dot(hoverBoardCast.normalHit, hoverBoardCast.pointHit - this.hoverboardVisual.transform.TransformPoint(hoverBoardCast.localOrigin + hoverBoardCast.localDirection * hoverBoardCast.distance)) + hoverBoardCast.sphereRadius) : (Vector3.Dot(hoverBoardCast.normalHit, hoverBoardCast.pointHit - vector4) + this.hoverIdealHeight));
					if (flag2)
					{
						flag = true;
						this.boostEnabledUntilTimestamp = Time.time + this.hoverboardBoostGracePeriod;
						if (Vector3.Dot(velocity, hoverBoardCast.normalHit) < 0f)
						{
							velocity = Vector3.ProjectOnPlane(velocity, hoverBoardCast.normalHit);
						}
						this.playerRigidBody.transform.position += hoverBoardCast.normalHit * num;
						Vector3 vector6 = this.turnParent.transform.rotation * (this.hoverboardVisual.IsLeftHanded ? this.leftHand.velocityTracker : this.rightHand.velocityTracker).GetAverageVelocity(false, 0.15f, false);
						if (Vector3.Dot(vector6, hoverBoardCast.normalHit) < 0f)
						{
							velocity -= Vector3.Project(vector6, hoverBoardCast.normalHit) * this.hoverSlamJumpStrengthFactor * Time.fixedDeltaTime;
						}
						vector = position + velocity * Time.fixedDeltaTime;
					}
				}
			}
			float num2 = Mathf.Abs(Mathf.DeltaAngle(0f, Mathf.Acos(Vector3.Dot(this.hoverboardVisual.transform.up, Vector3.ProjectOnPlane(vector3, vector2).normalized)) * 57.29578f));
			float num3 = this.hoverCarveAngleResponsiveness.Evaluate(num2);
			vector2 = (vector2 + Vector3.ProjectOnPlane(this.hoverboardVisual.transform.up, vector3) * this.hoverTiltAdjustsForwardFactor).normalized;
			if (!flag)
			{
				this.didHoverLastFrame = false;
				num3 = 0f;
			}
			Vector3 vector7 = velocity;
			if (this.enableHoverMode && this.hasHoverPoint)
			{
				Vector3 vector8 = Vector3.ProjectOnPlane(velocity, vector3);
				Vector3 vector9 = velocity - vector8;
				Vector3 vector10 = Vector3.Project(vector8, vector2);
				float num4 = vector8.magnitude;
				if (num4 <= this.hoveringSlowSpeed)
				{
					num4 *= this.hoveringSlowStoppingFactor;
				}
				Vector3 vector11 = vector8 - vector10;
				float num5 = 0f;
				bool flag3 = false;
				if (num3 > 0f)
				{
					if (vector11.IsLongerThan(vector10))
					{
						num5 = Mathf.Min((vector11.magnitude - vector10.magnitude) * this.hoverCarveSidewaysSpeedLossFactor * num3, num4);
						if (num5 > 0f && num4 > this.hoverMinGrindSpeed)
						{
							flag3 = true;
							this.hoverboardVisual.PlayGrindHaptic();
						}
						num4 -= num5;
					}
					vector11 *= 1f - num3 * this.sidewaysDrag;
					if (!this.leftHand.isColliding && !this.rightHand.isColliding)
					{
						velocity = (vector10 + vector11).normalized * num4 + vector9;
					}
				}
				else
				{
					velocity = vector8.normalized * num4 + vector9;
				}
				float magnitude = (velocity - vector7).magnitude;
				this.hoverboardAudio.UpdateAudioLoop(velocity.magnitude, this.bodyVelocityTracker.GetAverageVelocity(true, 0.15f, false).magnitude, magnitude, flag3 ? num5 : 0f);
				if (magnitude > 0f && !flag3)
				{
					this.hoverboardVisual.PlayCarveHaptic(magnitude);
				}
			}
			else
			{
				this.hoverboardAudio.UpdateAudioLoop(0f, this.bodyVelocityTracker.GetAverageVelocity(true, 0.15f, false).magnitude, 0f, 0f);
			}
			return velocity;
		}

		// Token: 0x060070D7 RID: 28887 RVA: 0x00246034 File Offset: 0x00244234
		public void GrabPersonalHoverboard(bool isLeftHand, Vector3 pos, Quaternion rot, Color col)
		{
			if (this.hoverboardVisual.IsHeld)
			{
				this.hoverboardVisual.DropFreeBoard();
			}
			this.hoverboardVisual.SetIsHeld(isLeftHand, pos, rot, col);
			this.hoverboardVisual.ProxyGrabHandle(isLeftHand);
			FreeHoverboardManager.instance.PreserveMaxHoverboardsConstraint(NetworkSystem.Instance.LocalPlayer.ActorNumber);
		}

		// Token: 0x060070D8 RID: 28888 RVA: 0x00246090 File Offset: 0x00244290
		public void AddHoverArea(HoverboardAreaTrigger area)
		{
			if (area == null)
			{
				return;
			}
			for (int i = 0; i < this.inHoverAreas.Count; i++)
			{
				if (this.inHoverAreas[i] == area)
				{
					return;
				}
			}
			this.inHoverAreas.Add(area);
			this.RefreshHoverAllowed();
		}

		// Token: 0x060070D9 RID: 28889 RVA: 0x002460E4 File Offset: 0x002442E4
		public void RemoveHoverArea(HoverboardAreaTrigger area)
		{
			if (area == null)
			{
				return;
			}
			for (int i = this.inHoverAreas.Count - 1; i >= 0; i--)
			{
				if (this.inHoverAreas[i] == area)
				{
					this.inHoverAreas.RemoveAt(i);
					this.RefreshHoverAllowed();
					return;
				}
			}
		}

		// Token: 0x060070DA RID: 28890 RVA: 0x0024613C File Offset: 0x0024433C
		public void AddHoverDisabler(ForceDisableHoverboardTrigger disabler)
		{
			if (disabler == null)
			{
				return;
			}
			for (int i = 0; i < this.inHoverDisablers.Count; i++)
			{
				if (this.inHoverDisablers[i] == disabler)
				{
					return;
				}
			}
			this.inHoverDisablers.Add(disabler);
			this.RefreshHoverAllowed();
		}

		// Token: 0x060070DB RID: 28891 RVA: 0x00246190 File Offset: 0x00244390
		public void RemoveHoverDisabler(ForceDisableHoverboardTrigger disabler)
		{
			if (disabler == null)
			{
				return;
			}
			for (int i = this.inHoverDisablers.Count - 1; i >= 0; i--)
			{
				if (this.inHoverDisablers[i] == disabler)
				{
					this.inHoverDisablers.RemoveAt(i);
					this.RefreshHoverAllowed();
					return;
				}
			}
		}

		// Token: 0x060070DC RID: 28892 RVA: 0x002461E6 File Offset: 0x002443E6
		public void ForceHoverDisallowed()
		{
			if (this.inHoverAreas.Count == 0 && this.inHoverDisablers.Count == 0)
			{
				return;
			}
			this.inHoverAreas.Clear();
			this.inHoverDisablers.Clear();
			this.RefreshHoverAllowed();
		}

		// Token: 0x060070DD RID: 28893 RVA: 0x00246220 File Offset: 0x00244420
		private void RefreshHoverAllowed()
		{
			bool flag = this.inHoverAreas.Count > 0 && this.inHoverDisablers.Count == 0;
			if (flag == this.isHoverAllowed)
			{
				return;
			}
			Debug.Log(string.Format("HoverAllowed {0} because {1} > 0 && {2} == 0", flag, this.inHoverAreas.Count, this.inHoverDisablers.Count));
			this.isHoverAllowed = flag;
			if (!flag && this.enableHoverMode)
			{
				base.StartCoroutine(this.DelayedRemoveHoverboard());
			}
		}

		// Token: 0x060070DE RID: 28894 RVA: 0x002462AB File Offset: 0x002444AB
		private IEnumerator DelayedRemoveHoverboard()
		{
			yield return null;
			if (!this.isHoverAllowed && this.enableHoverMode)
			{
				this.SetHoverActive(false);
				VRRig.LocalRig.hoverboardVisual.SetNotHeld();
			}
			yield break;
		}

		// Token: 0x060070DF RID: 28895 RVA: 0x002462BC File Offset: 0x002444BC
		public void SetHoverActive(bool enable)
		{
			if (enable && !this.isHoverAllowed)
			{
				return;
			}
			this.enableHoverMode = enable;
			if (!enable)
			{
				this.bodyCollider.enabled = true;
				this.hasHoverPoint = false;
				this.didHoverLastFrame = false;
				for (int i = 0; i < this.hoverboardCasts.Length; i++)
				{
					this.hoverboardCasts[i].didHit = false;
				}
				this.hoverboardAudio.Stop();
			}
		}

		// Token: 0x060070E0 RID: 28896 RVA: 0x0024632C File Offset: 0x0024452C
		private void BodyCollider()
		{
			if (this.MaxSphereSizeForNoOverlap(this.bodyInitialRadius * this.scale, this.PositionWithOffset(this.headCollider.transform, this.bodyOffset), false, out this.bodyMaxRadius))
			{
				if (this.scale > 0f)
				{
					this.bodyCollider.radius = this.bodyMaxRadius / this.scale;
				}
				if (Physics.SphereCast(this.PositionWithOffset(this.headCollider.transform, this.bodyOffset), this.bodyMaxRadius, GTPlayerTransform.Down, out this.bodyHitInfo, this.bodyInitialHeight * this.scale - this.bodyMaxRadius, this.locomotionEnabledLayers, QueryTriggerInteraction.Ignore))
				{
					this.bodyCollider.height = (this.bodyHitInfo.distance + this.bodyMaxRadius) / this.scale;
				}
				else
				{
					this.bodyHitInfo = this.emptyHit;
					this.bodyCollider.height = this.bodyInitialHeight;
				}
				if (!this.bodyCollider.gameObject.activeSelf)
				{
					this.bodyCollider.gameObject.SetActive(true);
				}
			}
			else
			{
				this.bodyCollider.gameObject.SetActive(false);
			}
			this.bodyCollider.height = Mathf.Lerp(this.bodyCollider.height, this.bodyInitialHeight, this.bodyLerp);
			this.bodyCollider.radius = Mathf.Lerp(this.bodyCollider.radius, this.bodyInitialRadius, this.bodyLerp);
			this.bodyOffsetVector = GTPlayerTransform.Down * this.bodyCollider.height / 2f;
			this.bodyCollider.transform.position = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + this.bodyOffsetVector * this.scale;
			this.bodyCollider.transform.rotation = Quaternion.FromToRotation(this.headCollider.transform.up, GTPlayerTransform.Up) * this.headCollider.transform.rotation;
		}

		// Token: 0x060070E1 RID: 28897 RVA: 0x0024654B File Offset: 0x0024474B
		private Vector3 PositionWithOffset(Transform transformToModify, Vector3 offsetVector)
		{
			return transformToModify.position + transformToModify.rotation * offsetVector * this.scale;
		}

		// Token: 0x060070E2 RID: 28898 RVA: 0x00246570 File Offset: 0x00244770
		public void ScaleAwayFromPoint(float oldScale, float newScale, Vector3 scaleCenter)
		{
			if (oldScale < newScale)
			{
				this.lastHeadPosition = GTPlayer.ScalePointAwayFromCenter(this.lastHeadPosition, this.headCollider.radius, oldScale, newScale, scaleCenter);
				this.leftHand.lastPosition = GTPlayer.ScalePointAwayFromCenter(this.leftHand.lastPosition, this.minimumRaycastDistance, oldScale, newScale, scaleCenter);
				this.rightHand.lastPosition = GTPlayer.ScalePointAwayFromCenter(this.rightHand.lastPosition, this.minimumRaycastDistance, oldScale, newScale, scaleCenter);
			}
		}

		// Token: 0x060070E3 RID: 28899 RVA: 0x002465E8 File Offset: 0x002447E8
		private static Vector3 ScalePointAwayFromCenter(Vector3 point, float baseRadius, float oldScale, float newScale, Vector3 scaleCenter)
		{
			float magnitude = (point - scaleCenter).magnitude;
			float num = magnitude + Mathf.Epsilon + baseRadius * (newScale - oldScale);
			return scaleCenter + (point - scaleCenter) * num / magnitude;
		}

		// Token: 0x060070E4 RID: 28900 RVA: 0x00246630 File Offset: 0x00244830
		private void OnBeforeRenderInit()
		{
			if (Application.isPlaying && !this.hasCorrectedForTracking && this.mainCamera != null && this.mainCamera.transform.localPosition != Vector3.zero)
			{
				this.ForceRigidBodySync();
				base.transform.position -= this.mainCamera.transform.localPosition;
				this.hasCorrectedForTracking = true;
			}
			Application.onBeforeRender -= this.OnBeforeRenderInit;
		}

		// Token: 0x060070E5 RID: 28901 RVA: 0x002466BC File Offset: 0x002448BC
		private void LateUpdate()
		{
			Vector3 vector = this.antiDriftLastPosition.GetValueOrDefault();
			if (this.antiDriftLastPosition == null)
			{
				vector = base.transform.position;
				this.antiDriftLastPosition = new Vector3?(vector);
			}
			if ((double)(this.antiDriftLastPosition.Value - base.transform.position).sqrMagnitude < 1E-08)
			{
				base.transform.position = this.antiDriftLastPosition.Value;
			}
			else
			{
				this.antiDriftLastPosition = new Vector3?(base.transform.position);
			}
			if (!this.hasCorrectedForTracking && this.mainCamera.transform.localPosition != Vector3.zero)
			{
				base.transform.position -= this.mainCamera.transform.localPosition;
				this.hasCorrectedForTracking = true;
				Application.onBeforeRender -= this.OnBeforeRenderInit;
			}
			if (this.playerRigidBody.isKinematic)
			{
				return;
			}
			float time = Time.time;
			Vector3 position = this.headCollider.transform.position;
			this.turnParent.transform.localScale = VRRig.LocalRig.transform.localScale;
			this.playerRigidBody.MovePosition(this.playerRigidBody.position + position - this.headCollider.transform.position);
			if (Mathf.Abs(this.lastScale - this.scale) > 0.001f)
			{
				if (this.mainCamera == null)
				{
					this.mainCamera = Camera.main;
				}
				this.mainCamera.nearClipPlane = ((this.scale > 0.5f) ? 0.01f : 0.002f);
			}
			this.lastScale = this.scale;
			this.debugLastRightHandPosition = this.rightHand.lastPosition;
			this.debugPlatformDeltaPosition = this.MovingSurfaceMovement();
			if (this.debugMovement)
			{
				this.tempRealTime = Time.time;
				this.calcDeltaTime = Time.deltaTime;
				this.lastRealTime = this.tempRealTime;
			}
			else
			{
				this.tempRealTime = Time.realtimeSinceStartup;
				this.calcDeltaTime = this.tempRealTime - this.lastRealTime;
				this.lastRealTime = this.tempRealTime;
				if (this.calcDeltaTime > 0.1f)
				{
					this.calcDeltaTime = 0.05f;
				}
			}
			Vector3 vector2;
			if (this.lastFrameHasValidTouchPos && this.lastPlatformTouched != null && GTPlayer.ComputeWorldHitPoint(this.lastHitInfoHand, this.lastFrameTouchPosLocal, out vector2))
			{
				this.refMovement = vector2 - this.lastFrameTouchPosWorld;
			}
			else
			{
				this.refMovement = Vector3.zero;
			}
			Vector3 vector3 = Vector3.zero;
			Quaternion quaternion = Quaternion.identity;
			Vector3 vector4 = this.headCollider.transform.position;
			Vector3 vector5;
			if (this.lastMovingSurfaceContact != GTPlayer.MovingSurfaceContactPoint.NONE && GTPlayer.ComputeWorldHitPoint(this.lastMovingSurfaceHit, this.lastMovingSurfaceTouchLocal, out vector5))
			{
				if (this.wasMovingSurfaceMonkeBlock && (this.lastMonkeBlock == null || this.lastMonkeBlock.state != BuilderPiece.State.AttachedAndPlaced))
				{
					this.movingSurfaceOffset = Vector3.zero;
				}
				else
				{
					this.movingSurfaceOffset = vector5 - this.lastMovingSurfaceTouchWorld;
					vector3 = this.movingSurfaceOffset / this.calcDeltaTime;
					quaternion = this.lastMovingSurfaceHit.collider.transform.rotation * Quaternion.Inverse(this.lastMovingSurfaceRot);
					vector4 = vector5;
				}
			}
			else
			{
				this.movingSurfaceOffset = Vector3.zero;
			}
			float num = 40f * this.scale;
			if (vector3.sqrMagnitude >= num * num)
			{
				this.movingSurfaceOffset = Vector3.zero;
				vector3 = Vector3.zero;
				quaternion = Quaternion.identity;
			}
			if (!this.didAJump && (this.leftHand.wasColliding || this.rightHand.wasColliding))
			{
				base.transform.position = base.transform.position + 4.9f * GTPlayerTransform.PhysicsDown * this.calcDeltaTime * this.calcDeltaTime * this.scale;
				if (Vector3.Dot(this.averagedVelocity, this.slideAverageNormal) <= 0f && Vector3.Dot(GTPlayerTransform.PhysicsUp, this.slideAverageNormal) > 0f)
				{
					base.transform.position = base.transform.position - Vector3.Project(Mathf.Min(this.stickDepth * this.scale, Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude * this.calcDeltaTime) * this.slideAverageNormal, GTPlayerTransform.PhysicsDown);
				}
			}
			if (!this.didAJump && this.anyHandWasSliding)
			{
				base.transform.position = base.transform.position + this.slideVelocity * this.calcDeltaTime;
				this.slideVelocity += 9.8f * GTPlayerTransform.PhysicsDown * this.calcDeltaTime * this.scale;
			}
			float num2 = ((Time.time > this.boostEnabledUntilTimestamp) ? 0f : (Time.deltaTime * Mathf.Clamp(this.playerRigidBody.linearVelocity.magnitude * this.hoverboardPaddleBoostMultiplier, 0f, this.hoverboardPaddleBoostMax)));
			int num3 = 0;
			Vector3 vector6 = Vector3.zero;
			this.anyHandIsColliding = false;
			this.anyHandIsSliding = false;
			this.anyHandIsSticking = false;
			this.leftHand.FirstIteration(ref vector6, ref num3, num2);
			this.rightHand.FirstIteration(ref vector6, ref num3, num2);
			for (int i = 0; i < 12; i++)
			{
				if (this.stiltStates[i].isActive)
				{
					this.stiltStates[i].FirstIteration(ref vector6, ref num3, 0f);
				}
			}
			if (num3 != 0)
			{
				vector6 /= (float)num3;
			}
			if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.RIGHT || this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.LEFT)
			{
				vector6 += this.movingSurfaceOffset;
			}
			else if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.BODY)
			{
				Vector3 vector7 = this.lastHeadPosition + this.movingSurfaceOffset - this.headCollider.transform.position;
				vector6 += vector7;
			}
			if (!this.MaxSphereSizeForNoOverlap(this.headCollider.radius * 0.9f * this.scale, this.lastHeadPosition, true, out this.maxSphereSize1) && !this.CrazyCheck2(this.headCollider.radius * 0.9f * 0.75f * this.scale, this.lastHeadPosition))
			{
				this.lastHeadPosition = this.lastOpenHeadPosition;
			}
			Vector3 vector8;
			float num4;
			if (this.IterativeCollisionSphereCast(this.lastHeadPosition, this.headCollider.radius * 0.9f * this.scale, this.headCollider.transform.position + vector6 - this.lastHeadPosition, Vector3.zero, out vector8, false, out num4, out this.junkHit, true))
			{
				vector6 = vector8 - this.headCollider.transform.position;
			}
			if (!this.MaxSphereSizeForNoOverlap(this.headCollider.radius * 0.9f * this.scale, this.lastHeadPosition + vector6, true, out this.maxSphereSize1) || !this.CrazyCheck2(this.headCollider.radius * 0.9f * 0.75f * this.scale, this.lastHeadPosition + vector6))
			{
				this.lastHeadPosition = this.lastOpenHeadPosition;
				vector6 = this.lastHeadPosition - this.headCollider.transform.position;
			}
			else if (this.headCollider.radius * 0.9f * 0.825f * this.scale < this.maxSphereSize1)
			{
				this.lastOpenHeadPosition = this.headCollider.transform.position + vector6;
			}
			if (vector6 != Vector3.zero)
			{
				base.transform.position += vector6;
			}
			if (this.lastMovingSurfaceContact != GTPlayer.MovingSurfaceContactPoint.NONE && quaternion != Quaternion.identity && !this.isClimbing && !this.rightHand.isHolding && !this.leftHand.isHolding)
			{
				this.RotateWithSurface(quaternion, vector4);
			}
			this.lastHeadPosition = this.headCollider.transform.position;
			this.areBothTouching = (!this.leftHand.isColliding && !this.leftHand.wasColliding) || (!this.rightHand.isColliding && !this.rightHand.wasColliding);
			this.TakeMyHand_ProcessMovement();
			this.HandleTentacleMovement();
			this.anyHandIsColliding = false;
			this.anyHandIsSliding = false;
			this.anyHandIsSticking = false;
			this.leftHand.FinalizeHandPosition();
			this.rightHand.FinalizeHandPosition();
			for (int j = 0; j < 12; j++)
			{
				if (this.stiltStates[j].isActive)
				{
					this.stiltStates[j].FinalizeHandPosition();
					GTPlayer.HandState handState = this.stiltStates[j];
					GorillaTagger.Instance.SetExtraHandPosition((StiltID)j, handState.finalPositionThisFrame, handState.canTag, handState.canStun);
				}
			}
			Vector3 vector9 = this.lastPosition;
			GTPlayer.MovingSurfaceContactPoint movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.NONE;
			int num5 = -1;
			int num6 = -1;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = this.rightHand.isColliding && this.IsTouchingMovingSurface(this.rightHand.GetLastPosition(), this.rightHand.lastHitInfo, out num5, out flag, out flag2);
			if (flag4 && !flag)
			{
				movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.RIGHT;
				this.lastMovingSurfaceHit = this.rightHand.lastHitInfo;
			}
			else
			{
				bool flag5 = false;
				BuilderPiece builderPiece = (flag4 ? this.lastMonkeBlock : null);
				if (this.leftHand.isColliding && this.IsTouchingMovingSurface(this.leftHand.GetLastPosition(), this.leftHand.lastHitInfo, out num6, out flag5, out flag3))
				{
					if (flag5 && flag2 == flag3)
					{
						if (flag && num6.Equals(num5) && (double)Vector3.Dot(this.leftHand.lastHitInfo.point - this.leftHand.GetLastPosition(), this.rightHand.lastHitInfo.point - this.rightHand.GetLastPosition()) < 0.3)
						{
							movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.RIGHT;
							this.lastMovingSurfaceHit = this.rightHand.lastHitInfo;
							this.lastMonkeBlock = builderPiece;
						}
					}
					else
					{
						movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.LEFT;
						this.lastMovingSurfaceHit = this.leftHand.lastHitInfo;
					}
				}
			}
			this.StoreVelocities();
			if (this.InWater)
			{
				PlayerGameEvents.PlayerSwam((this.lastPosition - vector9).magnitude, this.currentVelocity.magnitude);
			}
			else
			{
				PlayerGameEvents.PlayerMoved((this.lastPosition - vector9).magnitude, this.currentVelocity.magnitude);
			}
			this.didAJump = false;
			bool flag6 = this.exitMovingSurface;
			this.exitMovingSurface = false;
			if (this.leftHand.IsSlipOverriddenToMax() && this.rightHand.IsSlipOverriddenToMax())
			{
				this.didAJump = true;
				this.exitMovingSurface = true;
			}
			else if (this.anyHandIsSliding)
			{
				this.slideAverageNormal = Vector3.zero;
				int num7 = 0;
				this.averageSlipPercentage = 0f;
				bool flag7 = false;
				if (this.leftHand.isSliding)
				{
					this.slideAverageNormal += this.leftHand.slideNormal.normalized;
					this.averageSlipPercentage += this.leftHand.slipPercentage;
					num7++;
				}
				if (this.rightHand.isSliding)
				{
					flag7 = true;
					this.slideAverageNormal += this.rightHand.slideNormal.normalized;
					this.averageSlipPercentage += this.rightHand.slipPercentage;
					num7++;
				}
				for (int k = 0; k < this.stiltStates.Length; k++)
				{
					if (this.stiltStates[k].isActive && this.stiltStates[k].isSliding)
					{
						if (!this.stiltStates[k].isLeftHand)
						{
							flag7 = true;
						}
						this.slideAverageNormal += this.stiltStates[k].slideNormal.normalized;
						this.averageSlipPercentage += this.stiltStates[k].slipPercentage;
						num7++;
					}
				}
				this.slideAverageNormal = this.slideAverageNormal.normalized;
				this.averageSlipPercentage /= (float)num7;
				if (num7 == 1)
				{
					this.surfaceDirection = (flag7 ? Vector3.ProjectOnPlane(this.rightHand.handFollower.forward, this.rightHand.slideNormal) : Vector3.ProjectOnPlane(this.leftHand.handFollower.forward, this.leftHand.slideNormal));
					if (Vector3.Dot(this.slideVelocity, this.surfaceDirection) > 0f)
					{
						this.slideVelocity = Vector3.Project(this.slideVelocity, Vector3.Slerp(this.slideVelocity, this.surfaceDirection.normalized * this.slideVelocity.magnitude, this.slideControl));
					}
					else
					{
						this.slideVelocity = Vector3.Project(this.slideVelocity, Vector3.Slerp(this.slideVelocity, -this.surfaceDirection.normalized * this.slideVelocity.magnitude, this.slideControl));
					}
				}
				if (!this.anyHandWasSliding)
				{
					this.slideVelocity = ((Vector3.Dot(this.playerRigidBody.linearVelocity, this.slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(this.playerRigidBody.linearVelocity, this.slideAverageNormal) : this.playerRigidBody.linearVelocity);
				}
				else
				{
					this.slideVelocity = ((Vector3.Dot(this.slideVelocity, this.slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(this.slideVelocity, this.slideAverageNormal) : this.slideVelocity);
				}
				this.slideVelocity = this.slideVelocity.normalized * Mathf.Min(this.slideVelocity.magnitude, Mathf.Max(0.5f, this.averagedVelocity.magnitude * 2f));
				this.playerRigidBody.linearVelocity = Vector3.zero;
			}
			else if (this.anyHandIsColliding)
			{
				if (!this.turnedThisFrame)
				{
					this.playerRigidBody.linearVelocity = Vector3.zero;
				}
				else
				{
					this.playerRigidBody.linearVelocity = this.playerRigidBody.linearVelocity.normalized * Mathf.Min(2f, this.playerRigidBody.linearVelocity.magnitude);
				}
			}
			else if (this.anyHandWasSliding)
			{
				this.playerRigidBody.linearVelocity = ((Vector3.Dot(this.slideVelocity, this.slideAverageNormal) <= 0f) ? Vector3.ProjectOnPlane(this.slideVelocity, this.slideAverageNormal) : this.slideVelocity);
			}
			if (this.anyHandIsColliding && !this.disableMovement && !this.turnedThisFrame && !this.didAJump)
			{
				if (this.anyHandIsSliding)
				{
					if (Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude > this.slideVelocityLimit * this.scale && Vector3.Dot(this.averagedVelocity, this.slideAverageNormal) > 0f && Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude > Vector3.Project(this.slideVelocity, this.slideAverageNormal).magnitude)
					{
						this.leftHand.isSliding = false;
						this.rightHand.isSliding = false;
						for (int l = 0; l < this.stiltStates.Length; l++)
						{
							this.stiltStates[l].isSliding = false;
						}
						this.anyHandIsSliding = false;
						this.didAJump = true;
						float num8 = this.ApplyNativeScaleAdjustment(Mathf.Min(this.maxJumpSpeed * this.ExtraVelMaxMultiplier(), this.jumpMultiplier * this.ExtraVelMultiplier() * Vector3.Project(this.averagedVelocity, this.slideAverageNormal).magnitude));
						this.playerRigidBody.linearVelocity = num8 * this.siJumpMultiplier * this.slideAverageNormal.normalized + Vector3.ProjectOnPlane(this.slideVelocity, this.slideAverageNormal);
						if (num8 > this.slideVelocityLimit * this.scale * this.exitMovingSurfaceThreshold)
						{
							this.exitMovingSurface = true;
						}
					}
				}
				else if (this.averagedVelocity.magnitude > this.velocityLimit * this.scale)
				{
					float num9 = ((this.InWater && this.CurrentWaterVolume != null) ? this.liquidPropertiesList[(int)this.CurrentWaterVolume.LiquidType].surfaceJumpFactor : 1f);
					float num10 = this.ApplyNativeScaleAdjustment(this.enableHoverMode ? Mathf.Min(this.hoverMaxPaddleSpeed, this.averagedVelocity.magnitude) : Mathf.Min(this.maxJumpSpeed * this.ExtraVelMaxMultiplier(), this.jumpMultiplier * this.ExtraVelMultiplier() * num9 * this.averagedVelocity.magnitude));
					Vector3 vector10 = num10 * this.siJumpMultiplier * this.averagedVelocity.normalized;
					this.didAJump = true;
					this.playerRigidBody.linearVelocity = vector10;
					if (this.InWater)
					{
						this.swimmingVelocity += vector10 * this.GetSwimmingParams(this.CurrentWaterVolume).underwaterJumpsAsSwimVelocityFactor;
					}
					if (num10 > this.velocityLimit * this.scale * this.exitMovingSurfaceThreshold)
					{
						this.exitMovingSurface = true;
					}
				}
			}
			this.stuckHandsCheckLateUpdate(ref this.leftHand.finalPositionThisFrame, ref this.rightHand.finalPositionThisFrame);
			if (this.lastPlatformTouched != null && this.currentPlatform == null)
			{
				if (!this.playerRigidBody.isKinematic)
				{
					this.playerRigidBody.linearVelocity += this.refMovement / this.calcDeltaTime;
				}
				this.refMovement = Vector3.zero;
			}
			if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.NONE)
			{
				if (!this.playerRigidBody.isKinematic)
				{
					this.playerRigidBody.linearVelocity += this.lastMovingSurfaceVelocity;
				}
				this.lastMovingSurfaceVelocity = Vector3.zero;
			}
			if (this.enableHoverMode)
			{
				this.HoverboardLateUpdate();
			}
			else
			{
				this.hasHoverPoint = false;
			}
			Vector3 vector11 = Vector3.zero;
			float num11 = 0f;
			float num12 = 0f;
			if (this.bodyInWater)
			{
				Vector3 vector12;
				if (this.GetSwimmingVelocityForHand(this.leftHand.lastPosition, this.leftHand.finalPositionThisFrame, this.leftHand.controllerTransform.right, this.calcDeltaTime, ref this.leftHandWaterVolume, ref this.leftHandWaterSurface, out vector12) && !this.turnedThisFrame)
				{
					num11 = Mathf.InverseLerp(0f, 0.2f, vector12.magnitude) * this.GetSwimmingParams(this.leftHandWaterVolume).swimmingHapticsStrength;
					vector11 += vector12;
				}
				Vector3 vector13;
				if (this.GetSwimmingVelocityForHand(this.rightHand.lastPosition, this.rightHand.finalPositionThisFrame, -this.rightHand.controllerTransform.right, this.calcDeltaTime, ref this.rightHandWaterVolume, ref this.rightHandWaterSurface, out vector13) && !this.turnedThisFrame)
				{
					num12 = Mathf.InverseLerp(0f, 0.15f, vector13.magnitude) * this.GetSwimmingParams(this.rightHandWaterVolume).swimmingHapticsStrength;
					vector11 += vector13;
				}
			}
			Vector3 vector14 = Vector3.zero;
			PlayerSwimmingParameters swimmingParams = this.GetSwimmingParams(this.leftHandWaterVolume);
			Vector3 vector15;
			if (swimmingParams.allowWaterSurfaceJumps && time - this.lastWaterSurfaceJumpTimeLeft > this.waterSurfaceJumpCooldown && this.CheckWaterSurfaceJump(this.leftHand.lastPosition, this.leftHand.finalPositionThisFrame, this.leftHand.controllerTransform.right, this.leftHand.velocityTracker.GetAverageVelocity(false, 0.1f, false) * this.scale, swimmingParams, this.leftHandWaterVolume, this.leftHandWaterSurface, out vector15))
			{
				if (time - this.lastWaterSurfaceJumpTimeRight > this.waterSurfaceJumpCooldown)
				{
					vector14 += vector15;
				}
				this.lastWaterSurfaceJumpTimeLeft = Time.time;
				GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			}
			PlayerSwimmingParameters swimmingParams2 = this.GetSwimmingParams(this.rightHandWaterVolume);
			Vector3 vector16;
			if (swimmingParams2.allowWaterSurfaceJumps && time - this.lastWaterSurfaceJumpTimeRight > this.waterSurfaceJumpCooldown && this.CheckWaterSurfaceJump(this.rightHand.lastPosition, this.rightHand.finalPositionThisFrame, -this.rightHand.controllerTransform.right, this.rightHand.velocityTracker.GetAverageVelocity(false, 0.1f, false) * this.scale, swimmingParams2, this.rightHandWaterVolume, this.rightHandWaterSurface, out vector16))
			{
				if (time - this.lastWaterSurfaceJumpTimeLeft > this.waterSurfaceJumpCooldown)
				{
					vector14 += vector16;
				}
				this.lastWaterSurfaceJumpTimeRight = Time.time;
				GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
			}
			vector14 = Vector3.ClampMagnitude(vector14, Mathf.Max(swimmingParams.waterSurfaceJumpMaxSpeed, swimmingParams2.waterSurfaceJumpMaxSpeed) * this.scale);
			float num13 = Mathf.Max(num11, this.leftHandNonDiveHapticsAmount);
			if (num13 > 0.001f && time - this.lastWaterSurfaceJumpTimeLeft > GorillaTagger.Instance.tapHapticDuration)
			{
				GorillaTagger.Instance.DoVibration(XRNode.LeftHand, num13, this.calcDeltaTime);
			}
			float num14 = Mathf.Max(num12, this.rightHandNonDiveHapticsAmount);
			if (num14 > 0.001f && time - this.lastWaterSurfaceJumpTimeRight > GorillaTagger.Instance.tapHapticDuration)
			{
				GorillaTagger.Instance.DoVibration(XRNode.RightHand, num14, this.calcDeltaTime);
			}
			if (!this.disableMovement)
			{
				this.swimmingVelocity += vector11;
				if (!this.playerRigidBody.isKinematic)
				{
					this.playerRigidBody.linearVelocity += vector11 + vector14;
				}
			}
			else
			{
				this.swimmingVelocity = Vector3.zero;
			}
			if (GorillaGameManager.instance is GorillaFreezeTagManager)
			{
				if (!this.IsFrozen || !this.primaryButtonPressed)
				{
					this.IsBodySliding = false;
					this.lastSlopeDirection = Vector3.zero;
					if (this.bodyTouchedSurfaces.Count > 0)
					{
						foreach (KeyValuePair<GameObject, PhysicsMaterial> keyValuePair in this.bodyTouchedSurfaces)
						{
							MeshCollider meshCollider;
							if (keyValuePair.Key.TryGetComponent<MeshCollider>(out meshCollider))
							{
								meshCollider.material = keyValuePair.Value;
							}
						}
						this.bodyTouchedSurfaces.Clear();
					}
				}
				else if (this.BodyOnGround && this.primaryButtonPressed)
				{
					float num15 = this.bodyInitialHeight / 2f - this.bodyInitialRadius;
					RaycastHit raycastHit;
					if (Physics.SphereCast(this.bodyCollider.transform.position - new Vector3(0f, num15, 0f), this.bodyInitialRadius - 0.01f, Vector3.down, out raycastHit, 1f, ~LayerMask.GetMask(new string[] { "Gorilla Body Collider", "GorillaInteractable" }), QueryTriggerInteraction.Ignore))
					{
						this.IsBodySliding = true;
						MeshCollider meshCollider2;
						if (!this.bodyTouchedSurfaces.ContainsKey(raycastHit.transform.gameObject) && raycastHit.transform.gameObject.TryGetComponent<MeshCollider>(out meshCollider2))
						{
							this.bodyTouchedSurfaces.Add(raycastHit.transform.gameObject, meshCollider2.material);
							raycastHit.transform.gameObject.GetComponent<MeshCollider>().material = this.slipperyMaterial;
						}
					}
				}
				else
				{
					this.IsBodySliding = false;
					this.lastSlopeDirection = Vector3.zero;
				}
			}
			else
			{
				this.IsBodySliding = false;
				if (this.bodyTouchedSurfaces.Count > 0)
				{
					foreach (KeyValuePair<GameObject, PhysicsMaterial> keyValuePair2 in this.bodyTouchedSurfaces)
					{
						MeshCollider meshCollider3;
						if (keyValuePair2.Key.TryGetComponent<MeshCollider>(out meshCollider3))
						{
							meshCollider3.material = keyValuePair2.Value;
						}
					}
					this.bodyTouchedSurfaces.Clear();
				}
			}
			this.leftHand.OnEndOfFrame();
			this.rightHand.OnEndOfFrame();
			for (int m = 0; m < 12; m++)
			{
				if (this.stiltStates[m].isActive)
				{
					this.stiltStates[m].OnEndOfFrame();
				}
			}
			this.leftHand.PositionHandFollower();
			this.rightHand.PositionHandFollower();
			this.anyHandWasSliding = this.anyHandIsSliding;
			this.anyHandWasColliding = this.anyHandIsColliding;
			this.anyHandWasSticking = this.anyHandIsSticking;
			if (this.anyHandIsSticking)
			{
				this.lastTouchedGroundTimestamp = Time.time;
			}
			if (PhotonNetwork.InRoom)
			{
				if (this.IsGroundedHand || this.IsTentacleActive || this.IsThrusterActive)
				{
					this.LastHandTouchedGroundAtNetworkTime = (float)PhotonNetwork.Time;
					this.LastTouchedGroundAtNetworkTime = (float)PhotonNetwork.Time;
				}
				else if (this.IsGroundedButt || this.IsLaserZiplineActive)
				{
					this.LastTouchedGroundAtNetworkTime = (float)PhotonNetwork.Time;
				}
			}
			else
			{
				this.LastHandTouchedGroundAtNetworkTime = 0f;
				this.LastTouchedGroundAtNetworkTime = 0f;
			}
			this.degreesTurnedThisFrame = 0f;
			this.lastPlatformTouched = this.currentPlatform;
			this.currentPlatform = null;
			this.lastMovingSurfaceVelocity = vector3;
			Vector3 vector17;
			if (GTPlayer.ComputeLocalHitPoint(this.lastHitInfoHand, out vector17))
			{
				this.lastFrameHasValidTouchPos = true;
				this.lastFrameTouchPosLocal = vector17;
				this.lastFrameTouchPosWorld = this.lastHitInfoHand.point;
			}
			else
			{
				this.lastFrameHasValidTouchPos = false;
				this.lastFrameTouchPosLocal = Vector3.zero;
				this.lastFrameTouchPosWorld = Vector3.zero;
			}
			this.lastRigidbodyPosition = this.playerRigidBody.transform.position;
			RaycastHit raycastHit2 = this.emptyHit;
			this.BodyCollider();
			if (this.bodyHitInfo.collider != null)
			{
				this.wasBodyOnGround = true;
				raycastHit2 = this.bodyHitInfo;
			}
			else if (movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.NONE && this.bodyCollider.gameObject.activeSelf)
			{
				bool flag8 = false;
				this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
				Vector3 vector18 = this.PositionWithOffset(this.headCollider.transform, this.bodyOffset) + (this.bodyInitialHeight * this.scale - this.bodyMaxRadius) * GTPlayerTransform.Down;
				this.bufferCount = Physics.SphereCastNonAlloc(vector18, this.bodyMaxRadius, GTPlayerTransform.Down, this.rayCastNonAllocColliders, this.minimumRaycastDistance * this.scale, this.locomotionEnabledLayers.value);
				if (this.bufferCount > 0)
				{
					this.tempHitInfo = this.rayCastNonAllocColliders[0];
					for (int n = 0; n < this.bufferCount; n++)
					{
						if (this.tempHitInfo.distance > 0f && (!flag8 || this.rayCastNonAllocColliders[n].distance < this.tempHitInfo.distance))
						{
							flag8 = true;
							raycastHit2 = this.rayCastNonAllocColliders[n];
						}
					}
				}
				this.wasBodyOnGround = flag8;
			}
			int num16 = -1;
			bool flag9 = false;
			bool flag10;
			if (this.wasBodyOnGround && movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.NONE && this.IsTouchingMovingSurface(this.PositionWithOffset(this.headCollider.transform, this.bodyOffset), raycastHit2, out num16, out flag10, out flag9) && !flag10)
			{
				movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.BODY;
				this.lastMovingSurfaceHit = raycastHit2;
			}
			Vector3 vector19;
			if (movingSurfaceContactPoint != GTPlayer.MovingSurfaceContactPoint.NONE && GTPlayer.ComputeLocalHitPoint(this.lastMovingSurfaceHit, out vector19))
			{
				this.lastMovingSurfaceTouchLocal = vector19;
				this.lastMovingSurfaceTouchWorld = this.lastMovingSurfaceHit.point;
				this.lastMovingSurfaceRot = this.lastMovingSurfaceHit.collider.transform.rotation;
				this.lastAttachedToMovingSurfaceFrame = Time.frameCount;
			}
			else
			{
				movingSurfaceContactPoint = GTPlayer.MovingSurfaceContactPoint.NONE;
				this.lastMovingSurfaceTouchLocal = Vector3.zero;
				this.lastMovingSurfaceTouchWorld = Vector3.zero;
				this.lastMovingSurfaceRot = Quaternion.identity;
			}
			Vector3 vector20 = this.lastMovingSurfaceTouchWorld;
			int num17 = -1;
			bool flag11 = false;
			switch (movingSurfaceContactPoint)
			{
			case GTPlayer.MovingSurfaceContactPoint.NONE:
				if (flag6)
				{
					this.exitMovingSurface = true;
				}
				num17 = -1;
				break;
			case GTPlayer.MovingSurfaceContactPoint.RIGHT:
				num17 = num5;
				flag11 = flag2;
				vector20 = GorillaTagger.Instance.offlineVRRig.rightHandTransform.position;
				break;
			case GTPlayer.MovingSurfaceContactPoint.LEFT:
				num17 = num6;
				flag11 = flag3;
				vector20 = GorillaTagger.Instance.offlineVRRig.leftHandTransform.position;
				break;
			case GTPlayer.MovingSurfaceContactPoint.BODY:
				num17 = num16;
				flag11 = flag9;
				vector20 = GorillaTagger.Instance.offlineVRRig.bodyTransform.position;
				break;
			}
			if (!flag11)
			{
				this.lastMonkeBlock = null;
			}
			if (num17 != this.lastMovingSurfaceID || this.lastMovingSurfaceContact != movingSurfaceContactPoint || flag11 != this.wasMovingSurfaceMonkeBlock)
			{
				if (num17 == -1)
				{
					if (Time.frameCount - this.lastAttachedToMovingSurfaceFrame > 3)
					{
						VRRig.DetachLocalPlayerFromMovingSurface();
						this.lastMovingSurfaceID = -1;
					}
				}
				else if (flag11)
				{
					if (this.lastMonkeBlock != null)
					{
						VRRig.AttachLocalPlayerToMovingSurface(num17, movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.LEFT, movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.BODY, this.lastMonkeBlock.transform.InverseTransformPoint(vector20), flag11);
						this.lastMovingSurfaceID = num17;
					}
					else
					{
						VRRig.DetachLocalPlayerFromMovingSurface();
						this.lastMovingSurfaceID = -1;
					}
				}
				else if (MovingSurfaceManager.instance != null)
				{
					MovingSurface movingSurface;
					if (MovingSurfaceManager.instance.TryGetMovingSurface(num17, out movingSurface))
					{
						VRRig.AttachLocalPlayerToMovingSurface(num17, movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.LEFT, movingSurfaceContactPoint == GTPlayer.MovingSurfaceContactPoint.BODY, movingSurface.transform.InverseTransformPoint(vector20), flag11);
						this.lastMovingSurfaceID = num17;
					}
					else
					{
						VRRig.DetachLocalPlayerFromMovingSurface();
						this.lastMovingSurfaceID = -1;
					}
				}
				else
				{
					VRRig.DetachLocalPlayerFromMovingSurface();
					this.lastMovingSurfaceID = -1;
				}
			}
			if (this.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.NONE && movingSurfaceContactPoint != GTPlayer.MovingSurfaceContactPoint.NONE)
			{
				this.SetPlayerVelocity(Vector3.zero);
			}
			this.lastMovingSurfaceContact = movingSurfaceContactPoint;
			this.wasMovingSurfaceMonkeBlock = flag11;
			if (this.activeSizeChangerSettings != null)
			{
				if (this.activeSizeChangerSettings.ExpireOnDistance > 0f && Vector3.Distance(base.transform.position, this.activeSizeChangerSettings.WorldPosition) > this.activeSizeChangerSettings.ExpireOnDistance)
				{
					this.SetNativeScale(null);
				}
				if (this.activeSizeChangerSettings.ExpireAfterSeconds > 0f && Time.time - this.activeSizeChangerSettings.ActivationTime > this.activeSizeChangerSettings.ExpireAfterSeconds)
				{
					this.SetNativeScale(null);
				}
			}
			TakeMyHand_HandLink grabbedLink = VRRig.LocalRig.leftHandLink.grabbedLink;
			if (grabbedLink != null)
			{
				double time2 = PhotonNetwork.Time;
				float lastHandTouchedGroundAtNetworkTime = this.LastHandTouchedGroundAtNetworkTime;
				double time3 = PhotonNetwork.Time;
				float lastHandTouchedGroundAtNetworkTime2 = grabbedLink.myRig.LastHandTouchedGroundAtNetworkTime;
			}
			if (this.didAJump || this.anyHandIsColliding || this.anyHandIsSliding || this.anyHandIsSticking || this.IsGroundedHand || this.forceRBSync)
			{
				this.playerRigidBody.position = base.transform.position;
				this.playerRigidBody.rotation = base.transform.rotation;
				this.forceRBSync = false;
			}
		}

		// Token: 0x060070E6 RID: 28902 RVA: 0x00248658 File Offset: 0x00246858
		private float ApplyNativeScaleAdjustment(float adjustedMagnitude)
		{
			if (this.nativeScale > 0f && this.nativeScale != 1f)
			{
				return adjustedMagnitude *= this.nativeScaleMagnitudeAdjustmentFactor.Evaluate(this.nativeScale);
			}
			return adjustedMagnitude;
		}

		// Token: 0x060070E7 RID: 28903 RVA: 0x0024868C File Offset: 0x0024688C
		private float RotateWithSurface(Quaternion rotationDelta, Vector3 pivot)
		{
			Quaternion quaternion;
			Quaternion quaternion2;
			QuaternionUtil.DecomposeSwingTwist(rotationDelta, GTPlayerTransform.PhysicsUp, out quaternion, out quaternion2);
			float num = quaternion2.eulerAngles.y;
			if (num > 270f)
			{
				num -= 360f;
			}
			else if (num > 90f)
			{
				num -= 180f;
			}
			if (Mathf.Abs(num) < 90f * this.calcDeltaTime)
			{
				this.turnParent.transform.RotateAround(pivot, base.transform.up, num);
				return num;
			}
			return 0f;
		}

		// Token: 0x060070E8 RID: 28904 RVA: 0x00248710 File Offset: 0x00246910
		private void stuckHandsCheckFixedUpdate()
		{
			Vector3 currentHandPosition = this.leftHand.GetCurrentHandPosition();
			this.stuckLeft = !this.controllerState.LeftValid || (this.leftHand.isColliding && (currentHandPosition - this.leftHand.GetLastPosition()).magnitude > this.unStickDistance * this.scale && !Physics.Raycast(this.headCollider.transform.position, (currentHandPosition - this.headCollider.transform.position).normalized, (currentHandPosition - this.headCollider.transform.position).magnitude, this.locomotionEnabledLayers.value));
			Vector3 currentHandPosition2 = this.rightHand.GetCurrentHandPosition();
			this.stuckRight = !this.controllerState.RightValid || (this.rightHand.isColliding && (currentHandPosition2 - this.rightHand.GetLastPosition()).magnitude > this.unStickDistance * this.scale && !Physics.Raycast(this.headCollider.transform.position, (currentHandPosition2 - this.headCollider.transform.position).normalized, (currentHandPosition2 - this.headCollider.transform.position).magnitude, this.locomotionEnabledLayers.value));
		}

		// Token: 0x060070E9 RID: 28905 RVA: 0x0024889C File Offset: 0x00246A9C
		private void stuckHandsCheckLateUpdate(ref Vector3 finalLeftHandPosition, ref Vector3 finalRightHandPosition)
		{
			if (this.stuckLeft)
			{
				finalLeftHandPosition = this.leftHand.GetCurrentHandPosition();
				this.stuckLeft = (this.leftHand.isColliding = false);
			}
			if (this.stuckRight)
			{
				finalRightHandPosition = this.rightHand.GetCurrentHandPosition();
				this.stuckRight = (this.rightHand.isColliding = false);
			}
		}

		// Token: 0x060070EA RID: 28906 RVA: 0x00248908 File Offset: 0x00246B08
		private void handleClimbing(float deltaTime)
		{
			if (this.isClimbing && (this.inOverlay || this.climbHelper == null || this.currentClimbable == null || !this.currentClimbable.isActiveAndEnabled))
			{
				this.EndClimbing(this.currentClimber, false, false);
			}
			Vector3 vector = Vector3.zero;
			if (this.isClimbing && (this.currentClimber.transform.position - this.climbHelper.position).magnitude > 1f)
			{
				this.EndClimbing(this.currentClimber, false, false);
			}
			if (this.isClimbing)
			{
				this.playerRigidBody.linearVelocity = Vector3.zero;
				this.climbHelper.localPosition = Vector3.MoveTowards(this.climbHelper.localPosition, this.climbHelperTargetPos, deltaTime * 12f);
				vector = this.currentClimber.transform.position - this.climbHelper.position;
				vector = ((vector.sqrMagnitude > this.maxArmLength * this.maxArmLength) ? (vector.normalized * this.maxArmLength) : vector);
				if (this.isClimbableMoving)
				{
					Quaternion quaternion = this.currentClimbable.transform.rotation * Quaternion.Inverse(this.lastClimbableRotation);
					this.RotateWithSurface(quaternion, this.currentClimber.handRoot.position);
					this.lastClimbableRotation = this.currentClimbable.transform.rotation;
				}
				this.playerRigidBody.position = this.playerRigidBody.position - vector;
				if (this.currentSwing)
				{
					this.currentSwing.lastGrabTime = Time.time;
				}
			}
		}

		// Token: 0x060070EB RID: 28907 RVA: 0x00248AC7 File Offset: 0x00246CC7
		public void RequestTentacleMove(bool isLeftHand, Vector3 move)
		{
			if (isLeftHand)
			{
				this.hasLeftHandTentacleMove = true;
				this.leftHandTentacleMove = move;
				return;
			}
			this.hasRightHandTentacleMove = true;
			this.rightHandTentacleMove = move;
		}

		// Token: 0x060070EC RID: 28908 RVA: 0x00248AEC File Offset: 0x00246CEC
		private bool HandleTentacleMovement()
		{
			Vector3 vector;
			if (this.hasLeftHandTentacleMove)
			{
				if (this.hasRightHandTentacleMove)
				{
					vector = (this.leftHandTentacleMove + this.rightHandTentacleMove) * 0.5f;
					this.hasRightHandTentacleMove = (this.hasLeftHandTentacleMove = false);
				}
				else
				{
					vector = this.leftHandTentacleMove;
					this.hasLeftHandTentacleMove = false;
				}
			}
			else
			{
				if (!this.hasRightHandTentacleMove)
				{
					return false;
				}
				vector = this.rightHandTentacleMove;
				this.hasRightHandTentacleMove = false;
			}
			this.playerRigidBody.transform.position += vector;
			this.playerRigidBody.linearVelocity = Vector3.zero;
			return true;
		}

		// Token: 0x060070ED RID: 28909 RVA: 0x00248B90 File Offset: 0x00246D90
		public HandLinkAuthorityStatus TakeMyHand_GetSelfHandLinkAuthority()
		{
			int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
			if (this.IsGroundedHand)
			{
				return new HandLinkAuthorityStatus(HandLinkAuthorityType.HandGrounded);
			}
			if ((double)(this.LastHandTouchedGroundAtNetworkTime + 1f) > PhotonNetwork.Time)
			{
				return new HandLinkAuthorityStatus(HandLinkAuthorityType.ResidualHandGrounded, this.LastHandTouchedGroundAtNetworkTime, actorNumber);
			}
			if (this.IsGroundedButt)
			{
				return new HandLinkAuthorityStatus(HandLinkAuthorityType.ButtGrounded);
			}
			return new HandLinkAuthorityStatus(HandLinkAuthorityType.None, this.LastTouchedGroundAtNetworkTime, actorNumber);
		}

		// Token: 0x060070EE RID: 28910 RVA: 0x00248BF8 File Offset: 0x00246DF8
		private void TakeMyHand_ProcessMovement()
		{
			TakeMyHand_HandLink leftHandLink = VRRig.LocalRig.leftHandLink;
			TakeMyHand_HandLink rightHandLink = VRRig.LocalRig.rightHandLink;
			bool flag = leftHandLink.grabbedLink != null;
			bool flag2 = rightHandLink.grabbedLink != null;
			if (!flag && !flag2)
			{
				return;
			}
			HandLinkAuthorityStatus handLinkAuthorityStatus = this.TakeMyHand_GetSelfHandLinkAuthority();
			int num = -1;
			HandLinkAuthorityStatus chainAuthority = new HandLinkAuthorityStatus(HandLinkAuthorityType.None);
			if (flag)
			{
				chainAuthority = leftHandLink.GetChainAuthority(out num);
			}
			int num2 = -1;
			HandLinkAuthorityStatus chainAuthority2 = new HandLinkAuthorityStatus(HandLinkAuthorityType.None);
			if (flag2)
			{
				chainAuthority2 = rightHandLink.GetChainAuthority(out num2);
			}
			if (flag && flag2)
			{
				if (leftHandLink.grabbedPlayer == rightHandLink.grabbedPlayer)
				{
					switch (handLinkAuthorityStatus.CompareTo(chainAuthority))
					{
					case -1:
						this.TakeMyHand_PositionChild_LocalPlayer(leftHandLink, rightHandLink);
						return;
					case 0:
						this.TakeMyHand_PositionBoth_BothHands(leftHandLink, rightHandLink);
						return;
					case 1:
						this.TakeMyHand_PositionChild_RemotePlayer_BothHands(leftHandLink, rightHandLink);
						return;
					default:
						return;
					}
				}
				else
				{
					int num3 = handLinkAuthorityStatus.CompareTo(chainAuthority);
					int num4 = handLinkAuthorityStatus.CompareTo(chainAuthority2);
					switch (num3 * 3 + num4)
					{
					case -3:
					case -2:
						this.TakeMyHand_PositionChild_LocalPlayer(leftHandLink);
						this.TakeMyHand_PositionChild_RemotePlayer(rightHandLink);
						return;
					case -1:
					case 2:
						this.TakeMyHand_PositionChild_LocalPlayer(rightHandLink);
						this.TakeMyHand_PositionChild_RemotePlayer(leftHandLink);
						return;
					case 0:
						this.TakeMyHand_PositionTriple(leftHandLink, rightHandLink);
						return;
					case 1:
						this.TakeMyHand_PositionBoth(leftHandLink);
						this.TakeMyHand_PositionChild_RemotePlayer(rightHandLink);
						return;
					case 3:
						this.TakeMyHand_PositionBoth(rightHandLink);
						this.TakeMyHand_PositionChild_RemotePlayer(leftHandLink);
						return;
					case 4:
						this.TakeMyHand_PositionChild_RemotePlayer(leftHandLink);
						this.TakeMyHand_PositionChild_RemotePlayer(rightHandLink);
						return;
					}
					switch (chainAuthority.CompareTo(chainAuthority2))
					{
					case -1:
						this.TakeMyHand_PositionChild_LocalPlayer(rightHandLink);
						this.TakeMyHand_PositionChild_RemotePlayer(leftHandLink);
						return;
					case 0:
						if (num > num2)
						{
							this.TakeMyHand_PositionChild_LocalPlayer(rightHandLink);
							this.TakeMyHand_PositionChild_RemotePlayer(leftHandLink);
							return;
						}
						if (num < num2)
						{
							this.TakeMyHand_PositionChild_LocalPlayer(leftHandLink);
							this.TakeMyHand_PositionChild_RemotePlayer(rightHandLink);
							return;
						}
						this.TakeMyHand_PositionChild_LocalPlayer(leftHandLink, rightHandLink);
						return;
					case 1:
						this.TakeMyHand_PositionChild_LocalPlayer(leftHandLink);
						this.TakeMyHand_PositionChild_RemotePlayer(rightHandLink);
						return;
					default:
						return;
					}
				}
			}
			else if (flag)
			{
				switch (handLinkAuthorityStatus.CompareTo(chainAuthority))
				{
				case -1:
					this.TakeMyHand_PositionChild_LocalPlayer(leftHandLink);
					return;
				case 0:
					this.TakeMyHand_PositionBoth(leftHandLink);
					return;
				case 1:
					this.TakeMyHand_PositionChild_RemotePlayer(leftHandLink);
					return;
				default:
					return;
				}
			}
			else
			{
				switch (handLinkAuthorityStatus.CompareTo(chainAuthority2))
				{
				case -1:
					this.TakeMyHand_PositionChild_LocalPlayer(rightHandLink);
					return;
				case 0:
					this.TakeMyHand_PositionBoth(rightHandLink);
					return;
				case 1:
					this.TakeMyHand_PositionChild_RemotePlayer(rightHandLink);
					return;
				default:
					return;
				}
			}
		}

		// Token: 0x060070EF RID: 28911 RVA: 0x00248E4C File Offset: 0x0024704C
		private void TakeMyHand_PositionTriple(TakeMyHand_HandLink linkA, TakeMyHand_HandLink linkB)
		{
			Vector3 vector = linkA.LinkPosition - linkA.grabbedLink.LinkPosition;
			Vector3 vector2 = linkB.LinkPosition - linkB.grabbedLink.LinkPosition;
			Vector3 vector3 = (vector + vector2) * 0.33f;
			bool flag;
			bool flag2;
			linkA.grabbedLink.myRig.TrySweptOffsetMove(vector - vector3, out flag, out flag2);
			bool flag3;
			bool flag4;
			linkB.grabbedLink.myRig.TrySweptOffsetMove(vector2 - vector3, out flag3, out flag4);
			if (!(in vector3).IsMagnitudeValid(5f * this.scale))
			{
				linkA.BreakLink();
				linkB.BreakLink();
				return;
			}
			this.playerRigidBody.MovePosition(this.playerRigidBody.position - vector3);
			this.playerRigidBody.linearVelocity = Vector3.zero;
		}

		// Token: 0x060070F0 RID: 28912 RVA: 0x00248F20 File Offset: 0x00247120
		private void TakeMyHand_PositionBoth(TakeMyHand_HandLink link)
		{
			Vector3 vector = (link.grabbedLink.LinkPosition - link.LinkPosition) * 0.5f;
			bool flag;
			bool flag2;
			link.grabbedLink.myRig.TrySweptOffsetMove(-vector, out flag, out flag2);
			if (flag || flag2)
			{
				this.TakeMyHand_PositionChild_LocalPlayer(link);
			}
			else
			{
				if (!(in vector).IsMagnitudeValid(5f * this.scale))
				{
					link.BreakLink();
					return;
				}
				this.playerRigidBody.transform.position += vector;
			}
			this.playerRigidBody.linearVelocity = Vector3.zero;
		}

		// Token: 0x060070F1 RID: 28913 RVA: 0x00248FC0 File Offset: 0x002471C0
		private void TakeMyHand_PositionBoth_BothHands(TakeMyHand_HandLink link1, TakeMyHand_HandLink link2)
		{
			Vector3 vector = (link1.grabbedLink.LinkPosition - link1.LinkPosition) * 0.5f;
			Vector3 vector2 = (link2.grabbedLink.LinkPosition - link2.LinkPosition) * 0.5f;
			Vector3 vector3 = (vector + vector2) * 0.5f;
			bool flag;
			bool flag2;
			link1.grabbedLink.myRig.TrySweptOffsetMove(-vector3, out flag, out flag2);
			if (flag || flag2)
			{
				this.TakeMyHand_PositionChild_LocalPlayer(link1, link2);
			}
			else
			{
				if (!(in vector3).IsMagnitudeValid(5f * this.scale))
				{
					link1.BreakLink();
					link2.BreakLink();
					return;
				}
				this.playerRigidBody.transform.position += vector3;
			}
			this.playerRigidBody.linearVelocity = Vector3.zero;
		}

		// Token: 0x060070F2 RID: 28914 RVA: 0x00249098 File Offset: 0x00247298
		private void TakeMyHand_PositionChild_LocalPlayer(TakeMyHand_HandLink parentLink)
		{
			Vector3 vector = parentLink.grabbedLink.LinkPosition - parentLink.LinkPosition;
			if (!(in vector).IsMagnitudeValid(5f * this.scale))
			{
				parentLink.BreakLink();
				return;
			}
			this.playerRigidBody.transform.position += vector;
			this.playerRigidBody.linearVelocity = Vector3.zero;
		}

		// Token: 0x060070F3 RID: 28915 RVA: 0x00249104 File Offset: 0x00247304
		private void TakeMyHand_PositionChild_LocalPlayer(TakeMyHand_HandLink linkA, TakeMyHand_HandLink linkB)
		{
			Vector3 vector = linkA.grabbedLink.LinkPosition - linkA.LinkPosition;
			Vector3 vector2 = linkB.grabbedLink.LinkPosition - linkB.LinkPosition;
			Vector3 vector3 = (vector + vector2) * 0.5f;
			if (!(in vector3).IsMagnitudeValid(5f * this.scale))
			{
				linkA.BreakLink();
				linkB.BreakLink();
				return;
			}
			this.playerRigidBody.transform.position += vector3;
			this.playerRigidBody.linearVelocity = Vector3.zero;
		}

		// Token: 0x060070F4 RID: 28916 RVA: 0x002491A0 File Offset: 0x002473A0
		private void TakeMyHand_PositionChild_RemotePlayer(TakeMyHand_HandLink childLink)
		{
			Vector3 vector = childLink.LinkPosition - childLink.grabbedLink.LinkPosition;
			bool flag;
			bool flag2;
			childLink.grabbedLink.myRig.TrySweptOffsetMove(vector, out flag, out flag2);
			if (flag || flag2)
			{
				this.TakeMyHand_PositionChild_LocalPlayer(childLink);
			}
		}

		// Token: 0x060070F5 RID: 28917 RVA: 0x002491E8 File Offset: 0x002473E8
		private void TakeMyHand_PositionChild_RemotePlayer_BothHands(TakeMyHand_HandLink childLink1, TakeMyHand_HandLink childLink2)
		{
			Vector3 vector = childLink1.LinkPosition - childLink1.grabbedLink.LinkPosition;
			Vector3 vector2 = childLink2.LinkPosition - childLink2.grabbedLink.LinkPosition;
			Vector3 vector3 = (vector + vector2) * 0.5f;
			bool flag;
			bool flag2;
			childLink1.grabbedLink.myRig.TrySweptOffsetMove(vector3, out flag, out flag2);
			if (flag || flag2)
			{
				this.TakeMyHand_PositionChild_LocalPlayer(childLink1, childLink2);
			}
		}

		// Token: 0x060070F6 RID: 28918 RVA: 0x00249258 File Offset: 0x00247458
		private bool IterativeCollisionSphereCast(Vector3 startPosition, float sphereRadius, Vector3 movementVector, Vector3 boostVector, out Vector3 endPosition, bool singleHand, out float slipPercentage, out RaycastHit iterativeHitInfo, bool fullSlide)
		{
			slipPercentage = this.defaultSlideFactor;
			if (!this.CollisionsSphereCast(startPosition, sphereRadius, movementVector, out endPosition, out this.tempIterativeHit))
			{
				iterativeHitInfo = this.tempIterativeHit;
				endPosition = Vector3.zero;
				return false;
			}
			this.firstPosition = endPosition;
			iterativeHitInfo = this.tempIterativeHit;
			this.slideFactor = this.GetSlidePercentage(iterativeHitInfo);
			slipPercentage = ((this.slideFactor != this.defaultSlideFactor) ? this.slideFactor : ((!singleHand) ? this.defaultSlideFactor : 0.001f));
			if (fullSlide)
			{
				slipPercentage = 1f;
			}
			this.movementToProjectedAboveCollisionPlane = Vector3.ProjectOnPlane(startPosition + movementVector - this.firstPosition, iterativeHitInfo.normal) * slipPercentage;
			Vector3 vector = Vector3.zero;
			if (boostVector.IsLongerThan(0f))
			{
				vector = Vector3.ProjectOnPlane(boostVector, iterativeHitInfo.normal);
				this.movementToProjectedAboveCollisionPlane += vector;
				this.CollisionsSphereCast(this.firstPosition, sphereRadius, vector, out endPosition, out this.tempIterativeHit);
				this.firstPosition = endPosition;
			}
			if (this.CollisionsSphereCast(this.firstPosition, sphereRadius, this.movementToProjectedAboveCollisionPlane, out endPosition, out this.tempIterativeHit))
			{
				iterativeHitInfo = this.tempIterativeHit;
				return true;
			}
			if (this.CollisionsSphereCast(this.movementToProjectedAboveCollisionPlane + this.firstPosition, sphereRadius, startPosition + movementVector + vector - (this.movementToProjectedAboveCollisionPlane + this.firstPosition), out endPosition, out this.tempIterativeHit))
			{
				iterativeHitInfo = this.tempIterativeHit;
				return true;
			}
			endPosition = Vector3.zero;
			return false;
		}

		// Token: 0x060070F7 RID: 28919 RVA: 0x00249414 File Offset: 0x00247614
		private bool CollisionsSphereCast(Vector3 startPosition, float sphereRadius, Vector3 movementVector, out Vector3 finalPosition, out RaycastHit collisionsHitInfo)
		{
			this.MaxSphereSizeForNoOverlap(sphereRadius, startPosition, false, out this.maxSphereSize1);
			bool flag = false;
			this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
			this.bufferCount = Physics.SphereCastNonAlloc(startPosition, this.maxSphereSize1, movementVector.normalized, this.rayCastNonAllocColliders, movementVector.magnitude, this.locomotionEnabledLayers.value);
			if (this.bufferCount > 0)
			{
				this.tempHitInfo = this.rayCastNonAllocColliders[0];
				for (int i = 0; i < this.bufferCount; i++)
				{
					if (this.tempHitInfo.distance > 0f && (!flag || this.rayCastNonAllocColliders[i].distance < this.tempHitInfo.distance))
					{
						flag = true;
						this.tempHitInfo = this.rayCastNonAllocColliders[i];
					}
				}
			}
			if (flag)
			{
				collisionsHitInfo = this.tempHitInfo;
				finalPosition = collisionsHitInfo.point + collisionsHitInfo.normal * sphereRadius;
				this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
				this.bufferCount = Physics.RaycastNonAlloc(startPosition, (finalPosition - startPosition).normalized, this.rayCastNonAllocColliders, (finalPosition - startPosition).magnitude, this.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore);
				if (this.bufferCount > 0)
				{
					this.tempHitInfo = this.rayCastNonAllocColliders[0];
					for (int j = 0; j < this.bufferCount; j++)
					{
						if (this.rayCastNonAllocColliders[j].collider && this.rayCastNonAllocColliders[j].distance < this.tempHitInfo.distance)
						{
							this.tempHitInfo = this.rayCastNonAllocColliders[j];
						}
					}
					finalPosition = startPosition + movementVector.normalized * this.tempHitInfo.distance;
				}
				this.MaxSphereSizeForNoOverlap(sphereRadius, finalPosition, false, out this.maxSphereSize2);
				this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
				this.bufferCount = Physics.SphereCastNonAlloc(startPosition, Mathf.Min(this.maxSphereSize1, this.maxSphereSize2), (finalPosition - startPosition).normalized, this.rayCastNonAllocColliders, (finalPosition - startPosition).magnitude, this.locomotionEnabledLayers.value);
				if (this.bufferCount > 0)
				{
					this.tempHitInfo = this.rayCastNonAllocColliders[0];
					for (int k = 0; k < this.bufferCount; k++)
					{
						if (this.rayCastNonAllocColliders[k].collider != null && this.rayCastNonAllocColliders[k].distance < this.tempHitInfo.distance)
						{
							this.tempHitInfo = this.rayCastNonAllocColliders[k];
						}
					}
					finalPosition = startPosition + this.tempHitInfo.distance * (finalPosition - startPosition).normalized;
					collisionsHitInfo = this.tempHitInfo;
				}
				return true;
			}
			this.ClearRaycasthitBuffer(ref this.rayCastNonAllocColliders);
			this.bufferCount = Physics.RaycastNonAlloc(startPosition, movementVector.normalized, this.rayCastNonAllocColliders, movementVector.magnitude, this.locomotionEnabledLayers.value);
			if (this.bufferCount > 0)
			{
				this.tempHitInfo = this.rayCastNonAllocColliders[0];
				for (int l = 0; l < this.bufferCount; l++)
				{
					if (this.rayCastNonAllocColliders[l].collider != null && this.rayCastNonAllocColliders[l].distance < this.tempHitInfo.distance)
					{
						this.tempHitInfo = this.rayCastNonAllocColliders[l];
					}
				}
				collisionsHitInfo = this.tempHitInfo;
				finalPosition = startPosition;
				return true;
			}
			finalPosition = startPosition + movementVector;
			collisionsHitInfo = default(RaycastHit);
			return false;
		}

		// Token: 0x060070F8 RID: 28920 RVA: 0x00249824 File Offset: 0x00247A24
		public float GetSlidePercentage(RaycastHit raycastHit)
		{
			if (this.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
			{
				return this.FreezeTagSlidePercentage();
			}
			this.currentOverride = raycastHit.collider.gameObject.GetComponent<GorillaSurfaceOverride>();
			BasePlatform component = raycastHit.collider.gameObject.GetComponent<BasePlatform>();
			if (component != null)
			{
				this.currentPlatform = component;
			}
			if (this.currentOverride != null)
			{
				if (this.currentOverride.slidePercentageOverride >= 0f)
				{
					return this.currentOverride.slidePercentageOverride;
				}
				this.currentMaterialIndex = this.currentOverride.overrideIndex;
				if (this.currentMaterialIndex < 0 || this.currentMaterialIndex >= this.materialData.Count)
				{
					return this.defaultSlideFactor;
				}
				if (!this.materialData[this.currentMaterialIndex].overrideSlidePercent)
				{
					return this.defaultSlideFactor;
				}
				return this.materialData[this.currentMaterialIndex].slidePercent;
			}
			else
			{
				this.meshCollider = raycastHit.collider as MeshCollider;
				if (this.meshCollider == null || this.meshCollider.sharedMesh == null || this.meshCollider.convex)
				{
					return this.defaultSlideFactor;
				}
				this.collidedMesh = this.meshCollider.sharedMesh;
				if (!this.meshTrianglesDict.TryGetValue(this.collidedMesh, out this.sharedMeshTris))
				{
					this.sharedMeshTris = this.collidedMesh.triangles;
					this.meshTrianglesDict.Add(this.collidedMesh, (int[])this.sharedMeshTris.Clone());
				}
				this.vertex1 = this.sharedMeshTris[raycastHit.triangleIndex * 3];
				this.vertex2 = this.sharedMeshTris[raycastHit.triangleIndex * 3 + 1];
				this.vertex3 = this.sharedMeshTris[raycastHit.triangleIndex * 3 + 2];
				this.slideRenderer = raycastHit.collider.GetComponent<Renderer>();
				if (this.slideRenderer != null)
				{
					this.slideRenderer.GetSharedMaterials(this.tempMaterialArray);
				}
				else
				{
					this.tempMaterialArray.Clear();
				}
				if (this.tempMaterialArray.Count > 1)
				{
					for (int i = 0; i < this.tempMaterialArray.Count; i++)
					{
						this.collidedMesh.GetTriangles(this.trianglesList, i);
						int j = 0;
						while (j < this.trianglesList.Count)
						{
							if (this.trianglesList[j] == this.vertex1 && this.trianglesList[j + 1] == this.vertex2 && this.trianglesList[j + 2] == this.vertex3)
							{
								this.findMatName = this.tempMaterialArray[i].name;
								if (this.findMatName.EndsWith("Uber"))
								{
									string text = this.findMatName;
									this.findMatName = text.Substring(0, text.Length - 4);
								}
								this.foundMatData = this.materialData.Find((GTPlayer.MaterialData matData) => matData.matName == this.findMatName);
								this.currentMaterialIndex = this.materialData.FindIndex((GTPlayer.MaterialData matData) => matData.matName == this.findMatName);
								if (this.currentMaterialIndex == -1)
								{
									this.currentMaterialIndex = 0;
								}
								if (!this.foundMatData.overrideSlidePercent)
								{
									return this.defaultSlideFactor;
								}
								return this.foundMatData.slidePercent;
							}
							else
							{
								j += 3;
							}
						}
					}
				}
				else if (this.tempMaterialArray.Count > 0)
				{
					return this.defaultSlideFactor;
				}
				this.currentMaterialIndex = 0;
				return this.defaultSlideFactor;
			}
		}

		// Token: 0x060070F9 RID: 28921 RVA: 0x00249BBC File Offset: 0x00247DBC
		public bool IsTouchingMovingSurface(Vector3 rayOrigin, RaycastHit raycastHit, out int movingSurfaceId, out bool sideTouch, out bool isMonkeBlock)
		{
			movingSurfaceId = -1;
			sideTouch = false;
			isMonkeBlock = false;
			float num = Vector3.Dot(rayOrigin - raycastHit.point, Vector3.up);
			if (num < -0.3f)
			{
				return false;
			}
			if (num < 0f)
			{
				sideTouch = true;
			}
			if (raycastHit.collider == null)
			{
				return false;
			}
			MovingSurface component = raycastHit.collider.GetComponent<MovingSurface>();
			if (component != null)
			{
				isMonkeBlock = false;
				movingSurfaceId = component.GetID();
				return true;
			}
			if (!BuilderTable.IsLocalPlayerInBuilderZone())
			{
				return false;
			}
			BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(raycastHit.collider);
			if (builderPieceFromCollider != null && builderPieceFromCollider.IsPieceMoving())
			{
				isMonkeBlock = true;
				movingSurfaceId = builderPieceFromCollider.pieceId;
				this.lastMonkeBlock = builderPieceFromCollider;
				return true;
			}
			sideTouch = false;
			return false;
		}

		// Token: 0x060070FA RID: 28922 RVA: 0x00249C78 File Offset: 0x00247E78
		public void Turn(float degrees)
		{
			Vector3 vector = this.headCollider.transform.position;
			bool flag = this.rightHand.isColliding || this.rightHand.isHolding;
			bool flag2 = this.leftHand.isColliding || this.leftHand.isHolding;
			if (flag != flag2 && flag)
			{
				vector = this.rightHand.controllerTransform.position;
			}
			if (flag != flag2 && flag2)
			{
				vector = this.leftHand.controllerTransform.position;
			}
			this.turnParent.transform.RotateAround(vector, base.transform.up, degrees);
			this.degreesTurnedThisFrame = degrees;
			this.averagedVelocity = Vector3.zero;
			Quaternion quaternion = Quaternion.AngleAxis(degrees, base.transform.up);
			for (int i = 0; i < this.velocityHistory.Length; i++)
			{
				this.velocityHistory[i] = quaternion * this.velocityHistory[i];
				this.averagedVelocity += this.velocityHistory[i];
			}
			this.averagedVelocity /= (float)this.velocityHistorySize;
		}

		// Token: 0x060070FB RID: 28923 RVA: 0x00249DB4 File Offset: 0x00247FB4
		public void BeginClimbing(GorillaClimbable climbable, GorillaHandClimber hand, GorillaClimbableRef climbableRef = null)
		{
			if (this.currentClimber != null)
			{
				this.EndClimbing(this.currentClimber, true, false);
			}
			try
			{
				Action<GorillaHandClimber, GorillaClimbableRef> onBeforeClimb = climbable.onBeforeClimb;
				if (onBeforeClimb != null)
				{
					onBeforeClimb(hand, climbableRef);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex);
			}
			Rigidbody rigidbody;
			climbable.TryGetComponent<Rigidbody>(out rigidbody);
			this.VerifyClimbHelper();
			this.climbHelper.SetParent(climbable.transform);
			this.climbHelper.position = hand.transform.position;
			Vector3 localPosition = this.climbHelper.localPosition;
			if (climbable.snapX)
			{
				GTPlayer.<BeginClimbing>g__SnapAxis|458_0(ref localPosition.x, climbable.maxDistanceSnap);
			}
			if (climbable.snapY)
			{
				GTPlayer.<BeginClimbing>g__SnapAxis|458_0(ref localPosition.y, climbable.maxDistanceSnap);
			}
			if (climbable.snapZ)
			{
				GTPlayer.<BeginClimbing>g__SnapAxis|458_0(ref localPosition.z, climbable.maxDistanceSnap);
			}
			this.climbHelperTargetPos = localPosition;
			climbable.isBeingClimbed = true;
			hand.isClimbing = true;
			this.currentClimbable = climbable;
			this.currentClimber = hand;
			this.isClimbing = true;
			if (climbable.climbOnlyWhileSmall)
			{
				BuilderPiece componentInParent = climbable.GetComponentInParent<BuilderPiece>();
				if (componentInParent != null && componentInParent.IsPieceMoving())
				{
					this.isClimbableMoving = true;
					this.lastClimbableRotation = climbable.transform.rotation;
				}
				else
				{
					this.isClimbableMoving = false;
				}
			}
			else
			{
				this.isClimbableMoving = false;
			}
			GorillaRopeSegment gorillaRopeSegment;
			GorillaZipline gorillaZipline;
			PhotonView photonView;
			PhotonViewXSceneRef photonViewXSceneRef;
			if (climbable.TryGetComponent<GorillaRopeSegment>(out gorillaRopeSegment) && gorillaRopeSegment.swing)
			{
				this.currentSwing = gorillaRopeSegment.swing;
				this.currentSwing.AttachLocalPlayer(hand.xrNode, climbable.transform, this.climbHelperTargetPos, this.averagedVelocity);
			}
			else if (climbable.transform.parent && climbable.transform.parent.TryGetComponent<GorillaZipline>(out gorillaZipline))
			{
				this.currentZipline = gorillaZipline;
			}
			else if (climbable.TryGetComponent<PhotonView>(out photonView))
			{
				VRRig.AttachLocalPlayerToPhotonView(photonView, hand.xrNode, this.climbHelperTargetPos, this.averagedVelocity);
			}
			else if (climbable.TryGetComponent<PhotonViewXSceneRef>(out photonViewXSceneRef))
			{
				VRRig.AttachLocalPlayerToPhotonView(photonViewXSceneRef.photonView, hand.xrNode, this.climbHelperTargetPos, this.averagedVelocity);
			}
			GorillaTagger.Instance.StartVibration(this.currentClimber.xrNode == XRNode.LeftHand, 0.6f, 0.06f);
			if (climbable.clip)
			{
				GorillaTagger.Instance.offlineVRRig.PlayClimbSound(climbable.clip, hand.xrNode == XRNode.LeftHand);
			}
		}

		// Token: 0x060070FC RID: 28924 RVA: 0x0024A020 File Offset: 0x00248220
		private void VerifyClimbHelper()
		{
			if (this.climbHelper == null || this.climbHelper.gameObject == null)
			{
				this.climbHelper = new GameObject("Climb Helper").transform;
			}
		}

		// Token: 0x060070FD RID: 28925 RVA: 0x0024A058 File Offset: 0x00248258
		public void EndClimbing(GorillaHandClimber hand, bool startingNewClimb, bool doDontReclimb = false)
		{
			if (hand != this.currentClimber)
			{
				return;
			}
			hand.SetCanRelease(true);
			if (!startingNewClimb)
			{
				this.enablePlayerGravity(true);
			}
			Rigidbody rigidbody = null;
			if (this.currentClimbable)
			{
				this.currentClimbable.TryGetComponent<Rigidbody>(out rigidbody);
				this.currentClimbable.isBeingClimbed = false;
			}
			Vector3 vector = Vector3.zero;
			if (this.currentClimber)
			{
				this.currentClimber.isClimbing = false;
				if (doDontReclimb)
				{
					this.currentClimber.dontReclimbLast = this.currentClimbable;
				}
				else
				{
					this.currentClimber.dontReclimbLast = null;
				}
				this.currentClimber.queuedToBecomeValidToGrabAgain = true;
				this.currentClimber.lastAutoReleasePos = this.currentClimber.handRoot.localPosition;
				if (!startingNewClimb && this.currentClimbable)
				{
					GorillaVelocityTracker interactPointVelocityTracker = this.GetInteractPointVelocityTracker(this.currentClimber.xrNode == XRNode.LeftHand);
					if (rigidbody)
					{
						this.playerRigidBody.linearVelocity = rigidbody.linearVelocity;
					}
					else if (this.currentSwing)
					{
						this.playerRigidBody.linearVelocity = this.currentSwing.velocityTracker.GetAverageVelocity(true, 0.25f, false);
					}
					else if (this.currentZipline)
					{
						this.playerRigidBody.linearVelocity = this.currentZipline.GetCurrentDirection() * this.currentZipline.currentSpeed;
					}
					else
					{
						this.playerRigidBody.linearVelocity = Vector3.zero;
					}
					vector = this.turnParent.transform.rotation * -interactPointVelocityTracker.GetAverageVelocity(false, 0.1f, true) * this.scale;
					vector = Vector3.ClampMagnitude(vector, 5.5f * this.scale);
					this.playerRigidBody.AddForce(vector, ForceMode.VelocityChange);
				}
			}
			if (this.currentSwing)
			{
				this.currentSwing.DetachLocalPlayer();
			}
			PhotonView photonView;
			PhotonViewXSceneRef photonViewXSceneRef;
			if (this.currentClimbable.TryGetComponent<PhotonView>(out photonView) || this.currentClimbable.TryGetComponent<PhotonViewXSceneRef>(out photonViewXSceneRef) || this.currentClimbable.IsPlayerAttached)
			{
				VRRig.DetachLocalPlayerFromPhotonView();
			}
			if (!startingNewClimb && vector.magnitude > 2f && this.currentClimbable && this.currentClimbable.clipOnFullRelease)
			{
				GorillaTagger.Instance.offlineVRRig.PlayClimbSound(this.currentClimbable.clipOnFullRelease, hand.xrNode == XRNode.LeftHand);
			}
			this.currentClimbable = null;
			this.currentClimber = null;
			this.currentSwing = null;
			this.currentZipline = null;
			this.isClimbing = false;
		}

		// Token: 0x060070FE RID: 28926 RVA: 0x0024A2E8 File Offset: 0x002484E8
		public void ResetRigidbodyInterpolation()
		{
			this.playerRigidBody.interpolation = this.playerRigidbodyInterpolationDefault;
		}

		// Token: 0x17000B0E RID: 2830
		// (get) Token: 0x060070FF RID: 28927 RVA: 0x0024A2FB File Offset: 0x002484FB
		// (set) Token: 0x06007100 RID: 28928 RVA: 0x0024A308 File Offset: 0x00248508
		public RigidbodyInterpolation RigidbodyInterpolation
		{
			get
			{
				return this.playerRigidBody.interpolation;
			}
			set
			{
				this.playerRigidBody.interpolation = value;
			}
		}

		// Token: 0x06007101 RID: 28929 RVA: 0x0024A316 File Offset: 0x00248516
		private void enablePlayerGravity(bool useGravity)
		{
			this.playerRigidBody.useGravity = useGravity;
		}

		// Token: 0x06007102 RID: 28930 RVA: 0x0024A324 File Offset: 0x00248524
		public void SetVelocity(Vector3 velocity)
		{
			this.playerRigidBody.linearVelocity = velocity;
		}

		// Token: 0x06007103 RID: 28931 RVA: 0x0024A332 File Offset: 0x00248532
		internal void RigidbodyMovePosition(Vector3 pos)
		{
			this.playerRigidBody.MovePosition(pos);
		}

		// Token: 0x06007104 RID: 28932 RVA: 0x0024A340 File Offset: 0x00248540
		public void TempFreezeHand(bool isLeft, float freezeDuration)
		{
			(isLeft ? this.leftHand : this.rightHand).TempFreezeHand(freezeDuration);
		}

		// Token: 0x06007105 RID: 28933 RVA: 0x0024A368 File Offset: 0x00248568
		private void StoreVelocities()
		{
			this.velocityIndex = (this.velocityIndex + 1) % this.velocityHistorySize;
			this.currentVelocity = (base.transform.position - this.lastPosition - GTPlayerTransform.RotationPosOffsetChange - this.MovingSurfaceMovement()) / this.calcDeltaTime;
			this.velocityHistory[this.velocityIndex] = this.currentVelocity;
			this.averagedVelocity = this.velocityHistory.Average();
			this.lastPosition = base.transform.position;
			GTPlayerTransform.ResetRotationPositionOffset();
		}

		// Token: 0x06007106 RID: 28934 RVA: 0x0024A404 File Offset: 0x00248604
		private void AntiTeleportTechnology()
		{
			if ((this.headCollider.transform.position - this.lastHeadPosition).magnitude >= this.teleportThresholdNoVel + this.playerRigidBody.linearVelocity.magnitude * this.calcDeltaTime)
			{
				this.ForceRigidBodySync();
				base.transform.position = base.transform.position + this.lastHeadPosition - this.headCollider.transform.position;
			}
		}

		// Token: 0x06007107 RID: 28935 RVA: 0x0024A494 File Offset: 0x00248694
		private bool MaxSphereSizeForNoOverlap(float testRadius, Vector3 checkPosition, bool ignoreOneWay, out float overlapRadiusTest)
		{
			overlapRadiusTest = testRadius;
			this.overlapAttempts = 0;
			int num = 100;
			while (this.overlapAttempts < num && overlapRadiusTest > testRadius * 0.75f)
			{
				this.ClearColliderBuffer(ref this.overlapColliders);
				this.bufferCount = Physics.OverlapSphereNonAlloc(checkPosition, overlapRadiusTest, this.overlapColliders, this.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore);
				if (ignoreOneWay)
				{
					int num2 = 0;
					for (int i = 0; i < this.bufferCount; i++)
					{
						if (this.overlapColliders[i].CompareTag("NoCrazyCheck"))
						{
							num2++;
						}
					}
					if (num2 == this.bufferCount)
					{
						return true;
					}
				}
				if (this.bufferCount <= 0)
				{
					overlapRadiusTest *= 0.995f;
					return true;
				}
				overlapRadiusTest = Mathf.Lerp(testRadius, 0f, (float)this.overlapAttempts / (float)num);
				this.overlapAttempts++;
			}
			return false;
		}

		// Token: 0x06007108 RID: 28936 RVA: 0x0024A574 File Offset: 0x00248774
		private bool CrazyCheck2(float sphereSize, Vector3 startPosition)
		{
			for (int i = 0; i < this.crazyCheckVectors.Length; i++)
			{
				if (this.NonAllocRaycast(startPosition, startPosition + this.crazyCheckVectors[i] * sphereSize) > 0)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06007109 RID: 28937 RVA: 0x0024A5BC File Offset: 0x002487BC
		private int NonAllocRaycast(Vector3 startPosition, Vector3 endPosition)
		{
			Vector3 vector = endPosition - startPosition;
			int num = Physics.RaycastNonAlloc(startPosition, vector, this.rayCastNonAllocColliders, vector.magnitude, this.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore);
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (!this.rayCastNonAllocColliders[i].collider.gameObject.CompareTag("NoCrazyCheck"))
				{
					num2++;
				}
			}
			return num2;
		}

		// Token: 0x0600710A RID: 28938 RVA: 0x0024A628 File Offset: 0x00248828
		private void ClearColliderBuffer(ref Collider[] colliders)
		{
			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i] = null;
			}
		}

		// Token: 0x0600710B RID: 28939 RVA: 0x0024A64C File Offset: 0x0024884C
		private void ClearRaycasthitBuffer(ref RaycastHit[] raycastHits)
		{
			for (int i = 0; i < raycastHits.Length; i++)
			{
				raycastHits[i] = this.emptyHit;
			}
		}

		// Token: 0x0600710C RID: 28940 RVA: 0x0024A676 File Offset: 0x00248876
		private Vector3 MovingSurfaceMovement()
		{
			return this.refMovement + this.movingSurfaceOffset;
		}

		// Token: 0x0600710D RID: 28941 RVA: 0x0024A68C File Offset: 0x0024888C
		private static bool ComputeLocalHitPoint(RaycastHit hit, out Vector3 localHitPoint)
		{
			if (hit.collider == null || hit.point.sqrMagnitude < 0.001f)
			{
				localHitPoint = Vector3.zero;
				return false;
			}
			localHitPoint = hit.collider.transform.InverseTransformPoint(hit.point);
			return true;
		}

		// Token: 0x0600710E RID: 28942 RVA: 0x0024A6EA File Offset: 0x002488EA
		private static bool ComputeWorldHitPoint(RaycastHit hit, Vector3 localPoint, out Vector3 worldHitPoint)
		{
			if (hit.collider == null)
			{
				worldHitPoint = Vector3.zero;
				return false;
			}
			worldHitPoint = hit.collider.transform.TransformPoint(localPoint);
			return true;
		}

		// Token: 0x0600710F RID: 28943 RVA: 0x0024A724 File Offset: 0x00248924
		private float ExtraVelMultiplier()
		{
			float num = 1f;
			if (this.leftHand.surfaceOverride != null)
			{
				num = Mathf.Max(num, this.leftHand.surfaceOverride.extraVelMultiplier);
			}
			if (this.rightHand.surfaceOverride != null)
			{
				num = Mathf.Max(num, this.rightHand.surfaceOverride.extraVelMultiplier);
			}
			return num;
		}

		// Token: 0x06007110 RID: 28944 RVA: 0x0024A78C File Offset: 0x0024898C
		private float ExtraVelMaxMultiplier()
		{
			float num = 1f;
			if (this.leftHand.surfaceOverride != null)
			{
				num = Mathf.Max(num, this.leftHand.surfaceOverride.extraVelMaxMultiplier);
			}
			if (this.rightHand.surfaceOverride != null)
			{
				num = Mathf.Max(num, this.rightHand.surfaceOverride.extraVelMaxMultiplier);
			}
			return num * this.scale;
		}

		// Token: 0x06007111 RID: 28945 RVA: 0x0024A7FD File Offset: 0x002489FD
		public void SetMaximumSlipThisFrame()
		{
			this.leftHand.slipSetToMaxFrameIdx = Time.frameCount;
			this.rightHand.slipSetToMaxFrameIdx = Time.frameCount;
		}

		// Token: 0x06007112 RID: 28946 RVA: 0x0024A81F File Offset: 0x00248A1F
		public void SetLeftMaximumSlipThisFrame()
		{
			this.leftHand.slipSetToMaxFrameIdx = Time.frameCount;
		}

		// Token: 0x06007113 RID: 28947 RVA: 0x0024A831 File Offset: 0x00248A31
		public void SetRightMaximumSlipThisFrame()
		{
			this.rightHand.slipSetToMaxFrameIdx = Time.frameCount;
		}

		// Token: 0x06007114 RID: 28948 RVA: 0x0024A843 File Offset: 0x00248A43
		public void ChangeLayer(string layerName)
		{
			if (this.layerChanger != null)
			{
				this.layerChanger.ChangeLayer(base.transform.parent, layerName);
			}
		}

		// Token: 0x06007115 RID: 28949 RVA: 0x0024A86A File Offset: 0x00248A6A
		public void RestoreLayer()
		{
			if (this.layerChanger != null)
			{
				this.layerChanger.RestoreOriginalLayers();
			}
		}

		// Token: 0x06007116 RID: 28950 RVA: 0x0024A888 File Offset: 0x00248A88
		public void OnEnterWaterVolume(Collider playerCollider, WaterVolume volume)
		{
			if (this.activeSizeChangerSettings != null && this.activeSizeChangerSettings.ExpireInWater)
			{
				this.SetNativeScale(null);
			}
			if (playerCollider == this.headCollider)
			{
				if (!this.headOverlappingWaterVolumes.Contains(volume))
				{
					this.headOverlappingWaterVolumes.Add(volume);
					return;
				}
			}
			else if (playerCollider == this.bodyCollider && !this.bodyOverlappingWaterVolumes.Contains(volume))
			{
				this.bodyOverlappingWaterVolumes.Add(volume);
			}
		}

		// Token: 0x06007117 RID: 28951 RVA: 0x0024A902 File Offset: 0x00248B02
		public void OnExitWaterVolume(Collider playerCollider, WaterVolume volume)
		{
			if (playerCollider == this.headCollider)
			{
				this.headOverlappingWaterVolumes.Remove(volume);
				return;
			}
			if (playerCollider == this.bodyCollider)
			{
				this.bodyOverlappingWaterVolumes.Remove(volume);
			}
		}

		// Token: 0x06007118 RID: 28952 RVA: 0x0024A93C File Offset: 0x00248B3C
		private bool GetSwimmingVelocityForHand(Vector3 startingHandPosition, Vector3 endingHandPosition, Vector3 palmForwardDirection, float dt, ref WaterVolume contactingWaterVolume, ref WaterVolume.SurfaceQuery waterSurface, out Vector3 swimmingVelocityChange)
		{
			contactingWaterVolume = null;
			this.bufferCount = Physics.OverlapSphereNonAlloc(endingHandPosition, this.minimumRaycastDistance, this.overlapColliders, this.waterLayer.value, QueryTriggerInteraction.Collide);
			if (this.bufferCount > 0)
			{
				float num = float.MinValue;
				for (int i = 0; i < this.bufferCount; i++)
				{
					WaterVolume component = this.overlapColliders[i].GetComponent<WaterVolume>();
					WaterVolume.SurfaceQuery surfaceQuery;
					if (component != null && component.GetSurfaceQueryForPoint(endingHandPosition, out surfaceQuery, false))
					{
						float num2 = Vector3.Dot(surfaceQuery.surfacePoint, GTPlayerTransform.PhysicsUp);
						if (num2 > num)
						{
							num = num2;
							contactingWaterVolume = component;
							waterSurface = surfaceQuery;
						}
					}
				}
			}
			if (this.forcedUnderwater || contactingWaterVolume != null)
			{
				Vector3 vector = endingHandPosition - startingHandPosition;
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = this.playerRigidBody.transform.position - this.lastRigidbodyPosition;
				if (this.turnedThisFrame)
				{
					Vector3 vector4 = startingHandPosition - this.headCollider.transform.position;
					vector2 = Quaternion.AngleAxis(this.degreesTurnedThisFrame, GTPlayerTransform.PhysicsUp) * vector4 - vector4;
				}
				float num3 = Vector3.Dot(vector - vector2 - vector3, palmForwardDirection);
				float num4 = 0f;
				if (num3 > 0f)
				{
					float num5 = -1f;
					float num6 = -1f;
					if (!this.forcedUnderwater)
					{
						Plane surfacePlane = waterSurface.surfacePlane;
						num5 = (this.forcedUnderwater ? (-1f) : surfacePlane.GetDistanceToPoint(startingHandPosition));
						num6 = (this.forcedUnderwater ? (-1f) : surfacePlane.GetDistanceToPoint(endingHandPosition));
					}
					if (num5 <= 0f && num6 <= 0f)
					{
						num4 = 1f;
					}
					else if (num5 > 0f && num6 <= 0f)
					{
						num4 = -num6 / (num5 - num6);
					}
					else if (num5 <= 0f && num6 > 0f)
					{
						num4 = -num5 / (num6 - num5);
					}
					if (num4 > Mathf.Epsilon)
					{
						float resistance = this.liquidPropertiesList[(int)(this.forcedUnderwater ? GTPlayer.LiquidType.Water : contactingWaterVolume.LiquidType)].resistance;
						swimmingVelocityChange = -palmForwardDirection * num3 * 2f * resistance * num4;
						Vector3 forward = this.mainCamera.transform.forward;
						if (Vector3.Dot(forward, GTPlayerTransform.PhysicsDown) > 0f)
						{
							Vector3 vector5 = Vector3.ProjectOnPlane(forward, GTPlayerTransform.PhysicsUp);
							float magnitude = vector5.magnitude;
							vector5 /= magnitude;
							float num7 = Vector3.Dot(swimmingVelocityChange, vector5);
							if (num7 > 0f)
							{
								Vector3 vector6 = vector5 * num7;
								float num8 = Vector3.Dot(forward, GTPlayerTransform.PhysicsUp);
								swimmingVelocityChange = swimmingVelocityChange - vector6 + vector6 * magnitude + GTPlayerTransform.PhysicsUp * num8 * num7;
							}
						}
						return true;
					}
				}
			}
			swimmingVelocityChange = Vector3.zero;
			return false;
		}

		// Token: 0x06007119 RID: 28953 RVA: 0x0024AC50 File Offset: 0x00248E50
		private bool CheckWaterSurfaceJump(Vector3 startingHandPosition, Vector3 endingHandPosition, Vector3 palmForwardDirection, Vector3 handAvgVelocity, PlayerSwimmingParameters parameters, WaterVolume contactingWaterVolume, WaterVolume.SurfaceQuery waterSurface, out Vector3 jumpVelocity)
		{
			if (contactingWaterVolume != null)
			{
				Plane surfacePlane = waterSurface.surfacePlane;
				bool flag = handAvgVelocity.sqrMagnitude > parameters.waterSurfaceJumpHandSpeedThreshold * parameters.waterSurfaceJumpHandSpeedThreshold;
				if (surfacePlane.GetSide(startingHandPosition) && !surfacePlane.GetSide(endingHandPosition) && flag)
				{
					float num = Vector3.Dot(palmForwardDirection, -waterSurface.surfaceNormal);
					float num2 = Vector3.Dot(handAvgVelocity.normalized, -waterSurface.surfaceNormal);
					float num3 = parameters.waterSurfaceJumpPalmFacingCurve.Evaluate(Mathf.Clamp(num, 0.01f, 0.99f));
					float num4 = parameters.waterSurfaceJumpHandVelocityFacingCurve.Evaluate(Mathf.Clamp(num2, 0.01f, 0.99f));
					jumpVelocity = -handAvgVelocity * parameters.waterSurfaceJumpAmount * num3 * num4;
					return true;
				}
			}
			jumpVelocity = Vector3.zero;
			return false;
		}

		// Token: 0x0600711A RID: 28954 RVA: 0x0024AD49 File Offset: 0x00248F49
		private bool TryNormalize(Vector3 input, out Vector3 normalized, out float magnitude, float eps = 0.0001f)
		{
			magnitude = input.magnitude;
			if (magnitude > eps)
			{
				normalized = input / magnitude;
				return true;
			}
			normalized = Vector3.zero;
			return false;
		}

		// Token: 0x0600711B RID: 28955 RVA: 0x0024AD76 File Offset: 0x00248F76
		private bool TryNormalizeDown(Vector3 input, out Vector3 normalized, out float magnitude, float eps = 0.0001f)
		{
			magnitude = input.magnitude;
			if (magnitude > 1f)
			{
				normalized = input / magnitude;
				return true;
			}
			if (magnitude >= eps)
			{
				normalized = input;
				return true;
			}
			normalized = Vector3.zero;
			return false;
		}

		// Token: 0x0600711C RID: 28956 RVA: 0x0024ADB8 File Offset: 0x00248FB8
		private float FreezeTagSlidePercentage()
		{
			if (this.materialData[this.currentMaterialIndex].overrideSlidePercent && this.materialData[this.currentMaterialIndex].slidePercent > this.freezeTagHandSlidePercent)
			{
				return this.materialData[this.currentMaterialIndex].slidePercent;
			}
			return this.freezeTagHandSlidePercent;
		}

		// Token: 0x0600711D RID: 28957 RVA: 0x0024AE18 File Offset: 0x00249018
		private void OnCollisionStay(global::UnityEngine.Collision collision)
		{
			this.bodyCollisionContactsCount = collision.GetContacts(this.bodyCollisionContacts);
			float num = -1f;
			for (int i = 0; i < this.bodyCollisionContactsCount; i++)
			{
				float num2 = Vector3.Dot(this.bodyCollisionContacts[i].normal, GTPlayerTransform.Up);
				if (num2 > num)
				{
					this.bodyGroundContact = this.bodyCollisionContacts[i];
					num = num2;
				}
			}
			float num3 = 0.5f;
			if (num > num3)
			{
				this.bodyGroundContactTime = Time.time;
				Collider otherCollider = this.bodyGroundContact.otherCollider;
				this.bodyGroundIsSlippery = otherCollider != null && otherCollider.sharedMaterial != null && otherCollider.sharedMaterial.staticFriction <= 0.0001f && otherCollider.sharedMaterial.dynamicFriction <= 0.0001f;
			}
		}

		// Token: 0x0600711E RID: 28958 RVA: 0x0024AEF8 File Offset: 0x002490F8
		public async void DoLaunch(Vector3 velocity)
		{
			if (this.isClimbing)
			{
				this.EndClimbing(this.CurrentClimber, false, false);
			}
			this.playerRigidBody.linearVelocity = velocity;
			this.disableMovement = true;
			await Task.Delay(1);
			this.disableMovement = false;
		}

		// Token: 0x0600711F RID: 28959 RVA: 0x0024AF37 File Offset: 0x00249137
		private void OnEnable()
		{
			RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
		}

		// Token: 0x06007120 RID: 28960 RVA: 0x0024AF54 File Offset: 0x00249154
		private void OnJoinedRoom()
		{
			if (this.activeSizeChangerSettings != null && this.activeSizeChangerSettings.ExpireOnRoomJoin)
			{
				this.SetNativeScale(null);
			}
		}

		// Token: 0x06007121 RID: 28961 RVA: 0x0024AF72 File Offset: 0x00249172
		private void OnDisable()
		{
			RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
		}

		// Token: 0x06007122 RID: 28962 RVA: 0x0024AF8F File Offset: 0x0024918F
		public void ForceRigidBodySync()
		{
			this.forceRBSync = true;
		}

		// Token: 0x06007123 RID: 28963 RVA: 0x0024AF98 File Offset: 0x00249198
		internal void ClearHandHolds()
		{
			this.leftHand.isHolding = false;
			this.rightHand.isHolding = false;
			this.wasHoldingHandhold = false;
			this.activeHandHold = default(GTPlayer.HandHoldState);
			this.secondaryHandHold = default(GTPlayer.HandHoldState);
			this.OnChangeActiveHandhold();
		}

		// Token: 0x06007124 RID: 28964 RVA: 0x0024AFD8 File Offset: 0x002491D8
		internal void AddHandHold(Transform objectHeld, Vector3 localPositionHeld, GorillaGrabber grabber, bool forLeftHand, bool rotatePlayerWhenHeld, out Vector3 grabbedVelocity)
		{
			if (!this.leftHand.isHolding && !this.rightHand.isHolding)
			{
				grabbedVelocity = -this.bodyCollider.attachedRigidbody.linearVelocity;
				this.playerRigidBody.AddForce(grabbedVelocity, ForceMode.VelocityChange);
			}
			else
			{
				grabbedVelocity = Vector3.zero;
			}
			this.secondaryHandHold = this.activeHandHold;
			Vector3 position = grabber.transform.position;
			this.activeHandHold = new GTPlayer.HandHoldState
			{
				grabber = grabber,
				objectHeld = objectHeld,
				localPositionHeld = localPositionHeld,
				localRotationalOffset = grabber.transform.rotation.eulerAngles.y - objectHeld.rotation.eulerAngles.y,
				applyRotation = rotatePlayerWhenHeld
			};
			if (forLeftHand)
			{
				this.leftHand.isHolding = true;
			}
			else
			{
				this.rightHand.isHolding = true;
			}
			this.OnChangeActiveHandhold();
		}

		// Token: 0x06007125 RID: 28965 RVA: 0x0024B0DC File Offset: 0x002492DC
		internal void RemoveHandHold(GorillaGrabber grabber, bool forLeftHand)
		{
			this.activeHandHold.objectHeld == grabber;
			if (this.activeHandHold.grabber == grabber)
			{
				this.activeHandHold = this.secondaryHandHold;
			}
			this.secondaryHandHold = default(GTPlayer.HandHoldState);
			if (forLeftHand)
			{
				this.leftHand.isHolding = false;
			}
			else
			{
				this.rightHand.isHolding = false;
			}
			this.OnChangeActiveHandhold();
		}

		// Token: 0x06007126 RID: 28966 RVA: 0x0024B14C File Offset: 0x0024934C
		private void OnChangeActiveHandhold()
		{
			if (this.activeHandHold.objectHeld != null)
			{
				PhotonView photonView;
				if (this.activeHandHold.objectHeld.TryGetComponent<PhotonView>(out photonView))
				{
					VRRig.AttachLocalPlayerToPhotonView(photonView, this.activeHandHold.grabber.XrNode, this.activeHandHold.localPositionHeld, this.averagedVelocity);
					return;
				}
				PhotonViewXSceneRef photonViewXSceneRef;
				if (this.activeHandHold.objectHeld.TryGetComponent<PhotonViewXSceneRef>(out photonViewXSceneRef))
				{
					PhotonView photonView2 = photonViewXSceneRef.photonView;
					if (photonView2 != null)
					{
						VRRig.AttachLocalPlayerToPhotonView(photonView2, this.activeHandHold.grabber.XrNode, this.activeHandHold.localPositionHeld, this.averagedVelocity);
						return;
					}
				}
				BuilderPieceHandHold builderPieceHandHold;
				if (this.activeHandHold.objectHeld.TryGetComponent<BuilderPieceHandHold>(out builderPieceHandHold) && builderPieceHandHold.IsHandHoldMoving())
				{
					this.isHandHoldMoving = true;
					this.lastHandHoldRotation = builderPieceHandHold.transform.rotation;
					this.movingHandHoldReleaseVelocity = this.playerRigidBody.linearVelocity;
				}
				else
				{
					this.isHandHoldMoving = false;
					this.lastHandHoldRotation = Quaternion.identity;
					this.movingHandHoldReleaseVelocity = Vector3.zero;
				}
			}
			VRRig.DetachLocalPlayerFromPhotonView();
		}

		// Token: 0x06007127 RID: 28967 RVA: 0x0024B25C File Offset: 0x0024945C
		private void FixedUpdate_HandHolds(float timeDelta)
		{
			if (this.activeHandHold.objectHeld == null)
			{
				if (this.wasHoldingHandhold)
				{
					this.playerRigidBody.linearVelocity = Vector3.ClampMagnitude(this.secondLastPreHandholdVelocity, 5.5f * this.scale);
				}
				this.wasHoldingHandhold = false;
				return;
			}
			Vector3 vector = this.activeHandHold.objectHeld.TransformPoint(this.activeHandHold.localPositionHeld);
			Vector3 position = this.activeHandHold.grabber.transform.position;
			this.secondLastPreHandholdVelocity = this.lastPreHandholdVelocity;
			this.lastPreHandholdVelocity = this.playerRigidBody.linearVelocity;
			this.wasHoldingHandhold = true;
			if (this.isHandHoldMoving)
			{
				this.lastPreHandholdVelocity = this.movingHandHoldReleaseVelocity;
				this.playerRigidBody.linearVelocity = Vector3.zero;
				Vector3 vector2 = vector - position;
				this.playerRigidBody.transform.position += vector2;
				this.movingHandHoldReleaseVelocity = vector2 / timeDelta;
				Quaternion quaternion = this.activeHandHold.objectHeld.rotation * Quaternion.Inverse(this.lastHandHoldRotation);
				this.RotateWithSurface(quaternion, vector);
				this.lastHandHoldRotation = this.activeHandHold.objectHeld.rotation;
				return;
			}
			this.playerRigidBody.linearVelocity = (vector - position) / timeDelta;
			if (this.activeHandHold.applyRotation)
			{
				this.turnParent.transform.RotateAround(vector, base.transform.up, this.activeHandHold.localRotationalOffset - (this.activeHandHold.grabber.transform.rotation.eulerAngles.y - this.activeHandHold.objectHeld.rotation.eulerAngles.y));
			}
		}

		// Token: 0x0600712C RID: 28972 RVA: 0x0024B850 File Offset: 0x00249A50
		[CompilerGenerated]
		internal static void <BeginClimbing>g__SnapAxis|458_0(ref float val, float maxDist)
		{
			if (val > maxDist)
			{
				val = maxDist;
				return;
			}
			if (val < -maxDist)
			{
				val = -maxDist;
			}
		}

		// Token: 0x0400806D RID: 32877
		public static LayerMask LocomotionEnabledLayers = 201327105;

		// Token: 0x0400806E RID: 32878
		private static GTPlayer _instance;

		// Token: 0x0400806F RID: 32879
		public static bool hasInstance = false;

		// Token: 0x04008070 RID: 32880
		public Camera mainCamera;

		// Token: 0x04008071 RID: 32881
		public SphereCollider headCollider;

		// Token: 0x04008072 RID: 32882
		public CapsuleCollider bodyCollider;

		// Token: 0x04008073 RID: 32883
		private float bodyInitialRadius;

		// Token: 0x04008074 RID: 32884
		private float _bodyInitialHeight;

		// Token: 0x04008075 RID: 32885
		private float currentBodyHeight;

		// Token: 0x04008076 RID: 32886
		private double frameCount;

		// Token: 0x04008077 RID: 32887
		private RaycastHit bodyHitInfo;

		// Token: 0x04008078 RID: 32888
		private RaycastHit lastHitInfoHand;

		// Token: 0x04008079 RID: 32889
		public GorillaVelocityTracker bodyVelocityTracker;

		// Token: 0x0400807A RID: 32890
		public PlayerAudioManager audioManager;

		// Token: 0x0400807B RID: 32891
		[SerializeField]
		private GTPlayer.HandState leftHand;

		// Token: 0x0400807C RID: 32892
		[SerializeField]
		private GTPlayer.HandState rightHand;

		// Token: 0x0400807D RID: 32893
		private GTPlayer.HandState[] stiltStates = new GTPlayer.HandState[12];

		// Token: 0x0400807E RID: 32894
		private bool anyHandIsColliding;

		// Token: 0x0400807F RID: 32895
		private bool anyHandWasColliding;

		// Token: 0x04008080 RID: 32896
		private bool anyHandIsSliding;

		// Token: 0x04008081 RID: 32897
		private bool anyHandWasSliding;

		// Token: 0x04008082 RID: 32898
		private bool anyHandIsSticking;

		// Token: 0x04008083 RID: 32899
		private bool anyHandWasSticking;

		// Token: 0x04008084 RID: 32900
		private bool forceRBSync;

		// Token: 0x04008085 RID: 32901
		public Vector3 lastHeadPosition;

		// Token: 0x04008086 RID: 32902
		private Vector3 lastRigidbodyPosition;

		// Token: 0x04008088 RID: 32904
		private RigidbodyInterpolation playerRigidbodyInterpolationDefault;

		// Token: 0x04008089 RID: 32905
		public int velocityHistorySize;

		// Token: 0x0400808A RID: 32906
		public float maxArmLength = 1f;

		// Token: 0x0400808B RID: 32907
		public float unStickDistance = 1f;

		// Token: 0x0400808C RID: 32908
		public float velocityLimit;

		// Token: 0x0400808D RID: 32909
		public float slideVelocityLimit;

		// Token: 0x0400808E RID: 32910
		public float maxJumpSpeed;

		// Token: 0x0400808F RID: 32911
		private float _jumpMultiplier;

		// Token: 0x04008090 RID: 32912
		public float minimumRaycastDistance = 0.05f;

		// Token: 0x04008091 RID: 32913
		public float defaultSlideFactor = 0.03f;

		// Token: 0x04008092 RID: 32914
		public float slidingMinimum = 0.9f;

		// Token: 0x04008093 RID: 32915
		public float defaultPrecision = 0.995f;

		// Token: 0x04008094 RID: 32916
		public float teleportThresholdNoVel = 1f;

		// Token: 0x04008095 RID: 32917
		public float frictionConstant = 1f;

		// Token: 0x04008096 RID: 32918
		public float slideControl = 0.00425f;

		// Token: 0x04008097 RID: 32919
		public float stickDepth = 0.01f;

		// Token: 0x04008098 RID: 32920
		private Vector3[] velocityHistory;

		// Token: 0x04008099 RID: 32921
		private Vector3[] slideAverageHistory;

		// Token: 0x0400809A RID: 32922
		private int velocityIndex;

		// Token: 0x0400809B RID: 32923
		private Vector3 currentVelocity;

		// Token: 0x0400809C RID: 32924
		private Vector3 averagedVelocity;

		// Token: 0x0400809D RID: 32925
		private Vector3 lastPosition;

		// Token: 0x0400809E RID: 32926
		public Vector3 bodyOffset;

		// Token: 0x0400809F RID: 32927
		public LayerMask locomotionEnabledLayers;

		// Token: 0x040080A0 RID: 32928
		public LayerMask hoverboardLocomotionLayers;

		// Token: 0x040080A1 RID: 32929
		public LayerMask waterLayer;

		// Token: 0x040080A2 RID: 32930
		public bool wasHeadTouching;

		// Token: 0x040080A3 RID: 32931
		public int currentMaterialIndex;

		// Token: 0x040080A4 RID: 32932
		public Vector3 headSlideNormal;

		// Token: 0x040080A5 RID: 32933
		public float headSlipPercentage;

		// Token: 0x040080A6 RID: 32934
		[SerializeField]
		private Transform cosmeticsHeadTarget;

		// Token: 0x040080A7 RID: 32935
		[SerializeField]
		private float nativeScale = 1f;

		// Token: 0x040080A8 RID: 32936
		[SerializeField]
		private float scaleMultiplier = 1f;

		// Token: 0x040080A9 RID: 32937
		private NativeSizeChangerSettings activeSizeChangerSettings;

		// Token: 0x040080AA RID: 32938
		public bool debugMovement;

		// Token: 0x040080AB RID: 32939
		public bool disableMovement;

		// Token: 0x040080AC RID: 32940
		[NonSerialized]
		public bool inOverlay;

		// Token: 0x040080AD RID: 32941
		[NonSerialized]
		public bool isUserPresent;

		// Token: 0x040080AE RID: 32942
		public GameObject turnParent;

		// Token: 0x040080AF RID: 32943
		[SerializeField]
		public GameObject RecordingRig;

		// Token: 0x040080B0 RID: 32944
		public GorillaSurfaceOverride currentOverride;

		// Token: 0x040080B1 RID: 32945
		public MaterialDatasSO materialDatasSO;

		// Token: 0x040080B2 RID: 32946
		private float degreesTurnedThisFrame;

		// Token: 0x040080B3 RID: 32947
		private Vector3 bodyOffsetVector;

		// Token: 0x040080B4 RID: 32948
		private Vector3 movementToProjectedAboveCollisionPlane;

		// Token: 0x040080B5 RID: 32949
		private MeshCollider meshCollider;

		// Token: 0x040080B6 RID: 32950
		private Mesh collidedMesh;

		// Token: 0x040080B7 RID: 32951
		private GTPlayer.MaterialData foundMatData;

		// Token: 0x040080B8 RID: 32952
		private string findMatName;

		// Token: 0x040080B9 RID: 32953
		private int vertex1;

		// Token: 0x040080BA RID: 32954
		private int vertex2;

		// Token: 0x040080BB RID: 32955
		private int vertex3;

		// Token: 0x040080BC RID: 32956
		private List<int> trianglesList = new List<int>(1000000);

		// Token: 0x040080BD RID: 32957
		private Dictionary<Mesh, int[]> meshTrianglesDict = new Dictionary<Mesh, int[]>(128);

		// Token: 0x040080BE RID: 32958
		private int[] sharedMeshTris;

		// Token: 0x040080BF RID: 32959
		private float lastRealTime;

		// Token: 0x040080C0 RID: 32960
		private float calcDeltaTime;

		// Token: 0x040080C1 RID: 32961
		private float tempRealTime;

		// Token: 0x040080C2 RID: 32962
		private Vector3 slideVelocity;

		// Token: 0x040080C3 RID: 32963
		private Vector3 slideAverageNormal;

		// Token: 0x040080C4 RID: 32964
		private RaycastHit tempHitInfo;

		// Token: 0x040080C5 RID: 32965
		private RaycastHit junkHit;

		// Token: 0x040080C6 RID: 32966
		private Vector3 firstPosition;

		// Token: 0x040080C7 RID: 32967
		private RaycastHit tempIterativeHit;

		// Token: 0x040080C8 RID: 32968
		private float maxSphereSize1;

		// Token: 0x040080C9 RID: 32969
		private float maxSphereSize2;

		// Token: 0x040080CA RID: 32970
		private Collider[] overlapColliders = new Collider[10];

		// Token: 0x040080CB RID: 32971
		private int overlapAttempts;

		// Token: 0x040080CC RID: 32972
		private float averageSlipPercentage;

		// Token: 0x040080CD RID: 32973
		private Vector3 surfaceDirection;

		// Token: 0x040080CE RID: 32974
		public float iceThreshold = 0.9f;

		// Token: 0x040080CF RID: 32975
		private float bodyMaxRadius;

		// Token: 0x040080D0 RID: 32976
		public float bodyLerp = 0.17f;

		// Token: 0x040080D1 RID: 32977
		private bool areBothTouching;

		// Token: 0x040080D2 RID: 32978
		private float slideFactor;

		// Token: 0x040080D3 RID: 32979
		[DebugOption]
		public bool didAJump;

		// Token: 0x040080D4 RID: 32980
		private bool updateRB;

		// Token: 0x040080D5 RID: 32981
		private Renderer slideRenderer;

		// Token: 0x040080D6 RID: 32982
		private RaycastHit[] rayCastNonAllocColliders;

		// Token: 0x040080D7 RID: 32983
		private Vector3[] crazyCheckVectors;

		// Token: 0x040080D8 RID: 32984
		private RaycastHit emptyHit;

		// Token: 0x040080D9 RID: 32985
		private int bufferCount;

		// Token: 0x040080DA RID: 32986
		private Vector3 lastOpenHeadPosition;

		// Token: 0x040080DB RID: 32987
		private List<Material> tempMaterialArray = new List<Material>(16);

		// Token: 0x040080DC RID: 32988
		private Vector3? antiDriftLastPosition;

		// Token: 0x040080DD RID: 32989
		private const float CameraFarClipDefault = 500f;

		// Token: 0x040080DE RID: 32990
		private const float CameraNearClipDefault = 0.01f;

		// Token: 0x040080DF RID: 32991
		private const float CameraNearClipTiny = 0.002f;

		// Token: 0x040080E0 RID: 32992
		private Dictionary<GameObject, PhysicsMaterial> bodyTouchedSurfaces;

		// Token: 0x040080E1 RID: 32993
		private bool primaryButtonPressed = true;

		// Token: 0x040080E2 RID: 32994
		[Header("Swimming")]
		public List<PlayerSwimmingParameters> swimmingParamsList = new List<PlayerSwimmingParameters>(16);

		// Token: 0x040080E3 RID: 32995
		public WaterParameters waterParams;

		// Token: 0x040080E4 RID: 32996
		public List<GTPlayer.LiquidProperties> liquidPropertiesList = new List<GTPlayer.LiquidProperties>(16);

		// Token: 0x040080E5 RID: 32997
		public bool debugDrawSwimming;

		// Token: 0x040080E6 RID: 32998
		[Header("Slam/Hit effects")]
		public GameObject wizardStaffSlamEffects;

		// Token: 0x040080E7 RID: 32999
		public GameObject geodeHitEffects;

		// Token: 0x040080E8 RID: 33000
		[Header("Freeze Tag")]
		public float freezeTagHandSlidePercent = 0.88f;

		// Token: 0x040080E9 RID: 33001
		public bool debugFreezeTag;

		// Token: 0x040080EA RID: 33002
		public float frozenBodyBuoyancyFactor = 1.5f;

		// Token: 0x040080EC RID: 33004
		[Space]
		private WaterVolume leftHandWaterVolume;

		// Token: 0x040080ED RID: 33005
		private WaterVolume rightHandWaterVolume;

		// Token: 0x040080EE RID: 33006
		private WaterVolume.SurfaceQuery leftHandWaterSurface;

		// Token: 0x040080EF RID: 33007
		private WaterVolume.SurfaceQuery rightHandWaterSurface;

		// Token: 0x040080F0 RID: 33008
		private Vector3 swimmingVelocity = Vector3.zero;

		// Token: 0x040080F1 RID: 33009
		private WaterVolume.SurfaceQuery waterSurfaceForHead;

		// Token: 0x040080F2 RID: 33010
		private bool bodyInWater;

		// Token: 0x040080F3 RID: 33011
		private bool headInWater;

		// Token: 0x040080F4 RID: 33012
		private bool audioSetToUnderwater;

		// Token: 0x040080F5 RID: 33013
		private float buoyancyExtension;

		// Token: 0x040080F8 RID: 33016
		private float lastWaterSurfaceJumpTimeLeft = -1f;

		// Token: 0x040080F9 RID: 33017
		private float lastWaterSurfaceJumpTimeRight = -1f;

		// Token: 0x040080FA RID: 33018
		private float waterSurfaceJumpCooldown = 0.1f;

		// Token: 0x040080FB RID: 33019
		private float leftHandNonDiveHapticsAmount;

		// Token: 0x040080FC RID: 33020
		private float rightHandNonDiveHapticsAmount;

		// Token: 0x040080FD RID: 33021
		private List<WaterVolume> headOverlappingWaterVolumes = new List<WaterVolume>(16);

		// Token: 0x040080FE RID: 33022
		private List<WaterVolume> bodyOverlappingWaterVolumes = new List<WaterVolume>(16);

		// Token: 0x040080FF RID: 33023
		private List<WaterCurrent> activeWaterCurrents = new List<WaterCurrent>(16);

		// Token: 0x04008103 RID: 33027
		private Quaternion playerRotationOverride = Quaternion.identity;

		// Token: 0x04008104 RID: 33028
		private int playerRotationOverrideFrame = -1;

		// Token: 0x04008105 RID: 33029
		private float playerRotationOverrideDecayRate = Mathf.Exp(1.5f);

		// Token: 0x04008107 RID: 33031
		private ContactPoint[] bodyCollisionContacts = new ContactPoint[8];

		// Token: 0x04008108 RID: 33032
		private int bodyCollisionContactsCount;

		// Token: 0x04008109 RID: 33033
		private ContactPoint bodyGroundContact;

		// Token: 0x0400810A RID: 33034
		private float bodyGroundContactTime;

		// Token: 0x0400810C RID: 33036
		private const float movingSurfaceVelocityLimit = 40f;

		// Token: 0x0400810D RID: 33037
		private bool exitMovingSurface;

		// Token: 0x0400810E RID: 33038
		private float exitMovingSurfaceThreshold = 6f;

		// Token: 0x0400810F RID: 33039
		private bool isClimbableMoving;

		// Token: 0x04008110 RID: 33040
		private Quaternion lastClimbableRotation;

		// Token: 0x04008111 RID: 33041
		private int lastAttachedToMovingSurfaceFrame;

		// Token: 0x04008112 RID: 33042
		private const int MIN_FRAMES_OFF_SURFACE_TO_DETACH = 3;

		// Token: 0x04008113 RID: 33043
		private bool isHandHoldMoving;

		// Token: 0x04008114 RID: 33044
		private Quaternion lastHandHoldRotation;

		// Token: 0x04008115 RID: 33045
		private Vector3 movingHandHoldReleaseVelocity;

		// Token: 0x04008116 RID: 33046
		private GTPlayer.MovingSurfaceContactPoint lastMovingSurfaceContact;

		// Token: 0x04008117 RID: 33047
		private int lastMovingSurfaceID = -1;

		// Token: 0x04008118 RID: 33048
		private BuilderPiece lastMonkeBlock;

		// Token: 0x04008119 RID: 33049
		private Quaternion lastMovingSurfaceRot;

		// Token: 0x0400811A RID: 33050
		private RaycastHit lastMovingSurfaceHit;

		// Token: 0x0400811B RID: 33051
		private Vector3 lastMovingSurfaceTouchLocal;

		// Token: 0x0400811C RID: 33052
		private Vector3 lastMovingSurfaceTouchWorld;

		// Token: 0x0400811D RID: 33053
		private Vector3 movingSurfaceOffset;

		// Token: 0x0400811E RID: 33054
		private bool wasMovingSurfaceMonkeBlock;

		// Token: 0x0400811F RID: 33055
		private Vector3 lastMovingSurfaceVelocity;

		// Token: 0x04008120 RID: 33056
		private bool wasBodyOnGround;

		// Token: 0x04008121 RID: 33057
		private BasePlatform currentPlatform;

		// Token: 0x04008122 RID: 33058
		private BasePlatform lastPlatformTouched;

		// Token: 0x04008123 RID: 33059
		private Vector3 lastFrameTouchPosLocal;

		// Token: 0x04008124 RID: 33060
		private Vector3 lastFrameTouchPosWorld;

		// Token: 0x04008125 RID: 33061
		private bool lastFrameHasValidTouchPos;

		// Token: 0x04008126 RID: 33062
		private Vector3 refMovement = Vector3.zero;

		// Token: 0x04008127 RID: 33063
		private Vector3 platformTouchOffset;

		// Token: 0x04008128 RID: 33064
		private Vector3 debugLastRightHandPosition;

		// Token: 0x04008129 RID: 33065
		private Vector3 debugPlatformDeltaPosition;

		// Token: 0x0400812A RID: 33066
		public double tempFreezeRightHandEnableTime;

		// Token: 0x0400812B RID: 33067
		public double tempFreezeLeftHandEnableTime;

		// Token: 0x0400812C RID: 33068
		private const float climbingMaxThrowSpeed = 5.5f;

		// Token: 0x0400812D RID: 33069
		private const float climbHelperSmoothSnapSpeed = 12f;

		// Token: 0x0400812E RID: 33070
		[NonSerialized]
		public bool isClimbing;

		// Token: 0x0400812F RID: 33071
		private GorillaClimbable currentClimbable;

		// Token: 0x04008130 RID: 33072
		private GorillaHandClimber currentClimber;

		// Token: 0x04008131 RID: 33073
		private Vector3 climbHelperTargetPos = Vector3.zero;

		// Token: 0x04008132 RID: 33074
		private Transform climbHelper;

		// Token: 0x04008133 RID: 33075
		private GorillaRopeSwing currentSwing;

		// Token: 0x04008134 RID: 33076
		private GorillaZipline currentZipline;

		// Token: 0x04008135 RID: 33077
		[SerializeField]
		private ConnectedControllerHandler controllerState;

		// Token: 0x04008136 RID: 33078
		public int sizeLayerMask;

		// Token: 0x04008137 RID: 33079
		public bool InReportMenu;

		// Token: 0x04008138 RID: 33080
		private LayerChanger layerChanger;

		// Token: 0x0400813B RID: 33083
		private bool hasCorrectedForTracking;

		// Token: 0x0400813C RID: 33084
		private float halloweenLevitationStrength;

		// Token: 0x0400813D RID: 33085
		private float halloweenLevitationFullStrengthDuration;

		// Token: 0x0400813E RID: 33086
		private float halloweenLevitationTotalDuration = 1f;

		// Token: 0x0400813F RID: 33087
		private float halloweenLevitationBonusStrength;

		// Token: 0x04008140 RID: 33088
		private float halloweenLevitateBonusOffAtYSpeed;

		// Token: 0x04008141 RID: 33089
		private float halloweenLevitateBonusFullAtYSpeed = 1f;

		// Token: 0x04008142 RID: 33090
		private float lastTouchedGroundTimestamp;

		// Token: 0x04008143 RID: 33091
		private bool teleportToTrain;

		// Token: 0x04008144 RID: 33092
		public bool isAttachedToTrain;

		// Token: 0x04008145 RID: 33093
		private bool stuckLeft;

		// Token: 0x04008146 RID: 33094
		private bool stuckRight;

		// Token: 0x04008147 RID: 33095
		private float lastScale;

		// Token: 0x04008148 RID: 33096
		private Vector3 currentSlopDirection;

		// Token: 0x04008149 RID: 33097
		private Vector3 lastSlopeDirection = Vector3.zero;

		// Token: 0x0400814A RID: 33098
		private readonly Dictionary<Object, Action<GTPlayer>> gravityOverrides = new Dictionary<Object, Action<GTPlayer>>();

		// Token: 0x0400814D RID: 33101
		private readonly List<HoverboardAreaTrigger> inHoverAreas = new List<HoverboardAreaTrigger>(2);

		// Token: 0x0400814E RID: 33102
		private readonly List<ForceDisableHoverboardTrigger> inHoverDisablers = new List<ForceDisableHoverboardTrigger>(1);

		// Token: 0x0400814F RID: 33103
		[Header("Hoverboard")]
		[SerializeField]
		private float hoverIdealHeight = 0.5f;

		// Token: 0x04008150 RID: 33104
		[SerializeField]
		private float hoverCarveSidewaysSpeedLossFactor = 1f;

		// Token: 0x04008151 RID: 33105
		[SerializeField]
		private AnimationCurve hoverCarveAngleResponsiveness;

		// Token: 0x04008152 RID: 33106
		[SerializeField]
		private HoverboardVisual hoverboardVisual;

		// Token: 0x04008153 RID: 33107
		[SerializeField]
		private float sidewaysDrag = 0.1f;

		// Token: 0x04008154 RID: 33108
		[SerializeField]
		private float hoveringSlowSpeed = 0.1f;

		// Token: 0x04008155 RID: 33109
		[SerializeField]
		private float hoveringSlowStoppingFactor = 0.95f;

		// Token: 0x04008156 RID: 33110
		[SerializeField]
		private float hoverboardPaddleBoostMultiplier = 0.1f;

		// Token: 0x04008157 RID: 33111
		[SerializeField]
		private float hoverboardPaddleBoostMax = 10f;

		// Token: 0x04008158 RID: 33112
		[SerializeField]
		private float hoverboardBoostGracePeriod = 1f;

		// Token: 0x04008159 RID: 33113
		[SerializeField]
		private float hoverBodyHasCollisionsOutsideRadius = 0.5f;

		// Token: 0x0400815A RID: 33114
		[SerializeField]
		private float hoverBodyCollisionRadiusUpOffset = 0.2f;

		// Token: 0x0400815B RID: 33115
		[SerializeField]
		private float hoverGeneralUpwardForce = 8f;

		// Token: 0x0400815C RID: 33116
		[SerializeField]
		private float hoverTiltAdjustsForwardFactor = 0.2f;

		// Token: 0x0400815D RID: 33117
		[SerializeField]
		private float hoverMinGrindSpeed = 1f;

		// Token: 0x0400815E RID: 33118
		[SerializeField]
		private float hoverSlamJumpStrengthFactor = 25f;

		// Token: 0x0400815F RID: 33119
		[SerializeField]
		private float hoverMaxPaddleSpeed = 35f;

		// Token: 0x04008160 RID: 33120
		[SerializeField]
		private HoverboardAudio hoverboardAudio;

		// Token: 0x04008161 RID: 33121
		private bool hasHoverPoint;

		// Token: 0x04008162 RID: 33122
		private float boostEnabledUntilTimestamp;

		// Token: 0x04008163 RID: 33123
		private GTPlayer.HoverBoardCast[] hoverboardCasts = new GTPlayer.HoverBoardCast[]
		{
			new GTPlayer.HoverBoardCast
			{
				localOrigin = new Vector3(0f, 1f, 0.36f),
				localDirection = Vector3.down,
				distance = 1f,
				sphereRadius = 0.2f,
				intersectToVelocityCap = 0.1f
			},
			new GTPlayer.HoverBoardCast
			{
				localOrigin = new Vector3(0f, 0.05f, 0.36f),
				localDirection = Vector3.forward,
				distance = 0.25f,
				sphereRadius = 0.01f,
				intersectToVelocityCap = 0f,
				isSolid = true
			},
			new GTPlayer.HoverBoardCast
			{
				localOrigin = new Vector3(0f, 0.05f, -0.1f),
				localDirection = -Vector3.forward,
				distance = 0.24f,
				sphereRadius = 0.01f,
				intersectToVelocityCap = 0f,
				isSolid = true
			}
		};

		// Token: 0x04008164 RID: 33124
		private Vector3 hoverboardPlayerLocalPos;

		// Token: 0x04008165 RID: 33125
		private Quaternion hoverboardPlayerLocalRot;

		// Token: 0x04008166 RID: 33126
		private bool didHoverLastFrame;

		// Token: 0x04008167 RID: 33127
		private bool hasLeftHandTentacleMove;

		// Token: 0x04008168 RID: 33128
		private bool hasRightHandTentacleMove;

		// Token: 0x04008169 RID: 33129
		private Vector3 leftHandTentacleMove;

		// Token: 0x0400816A RID: 33130
		private Vector3 rightHandTentacleMove;

		// Token: 0x0400816B RID: 33131
		private GTPlayer.HandHoldState activeHandHold;

		// Token: 0x0400816C RID: 33132
		private GTPlayer.HandHoldState secondaryHandHold;

		// Token: 0x0400816D RID: 33133
		public PhysicsMaterial slipperyMaterial;

		// Token: 0x0400816E RID: 33134
		private bool wasHoldingHandhold;

		// Token: 0x0400816F RID: 33135
		private Vector3 secondLastPreHandholdVelocity;

		// Token: 0x04008170 RID: 33136
		private Vector3 lastPreHandholdVelocity;

		// Token: 0x04008171 RID: 33137
		[Header("Native Scale Adjustment")]
		[SerializeField]
		private AnimationCurve nativeScaleMagnitudeAdjustmentFactor;

		// Token: 0x02001187 RID: 4487
		[Serializable]
		public struct HandState
		{
			// Token: 0x0600712D RID: 28973 RVA: 0x0024B868 File Offset: 0x00249A68
			public void Init(GTPlayer gtPlayer, bool isLeftHand, float maxArmLength)
			{
				this.gtPlayer = gtPlayer;
				this.isLeftHand = isLeftHand;
				this.maxArmLength = maxArmLength;
				this.lastPosition = this.controllerTransform.position;
				this.lastRotation = this.controllerTransform.rotation;
				if (this.handFollower != null)
				{
					this.handFollower.transform.position = this.lastPosition;
					this.handFollower.transform.rotation = this.lastRotation;
				}
				this.wasColliding = false;
				this.slipSetToMaxFrameIdx = -1;
			}

			// Token: 0x0600712E RID: 28974 RVA: 0x0024B8F4 File Offset: 0x00249AF4
			public void OnTeleport()
			{
				this.wasColliding = false;
				this.isColliding = false;
				this.isSliding = false;
				this.wasSliding = false;
				if (this.handFollower != null)
				{
					this.handFollower.position = this.controllerTransform.position;
					this.handFollower.rotation = this.controllerTransform.rotation;
				}
				this.lastPosition = this.controllerTransform.position;
				this.lastRotation = this.controllerTransform.rotation;
			}

			// Token: 0x0600712F RID: 28975 RVA: 0x0024B979 File Offset: 0x00249B79
			public Vector3 GetLastPosition()
			{
				return this.lastPosition + this.gtPlayer.MovingSurfaceMovement();
			}

			// Token: 0x06007130 RID: 28976 RVA: 0x0024B991 File Offset: 0x00249B91
			public bool SlipOverriddenToMax()
			{
				return this.slipSetToMaxFrameIdx == Time.frameCount;
			}

			// Token: 0x06007131 RID: 28977 RVA: 0x0024B9A0 File Offset: 0x00249BA0
			public void FirstIteration(ref Vector3 totalMove, ref int divisor, float paddleBoostFactor)
			{
				if (this.hasCustomBoost)
				{
					this.boostVectorThisFrame = this.gtPlayer.turnParent.transform.rotation * -this.velocityTracker.GetAverageVelocity(false, 0.15f, false) * this.customBoostFactor;
				}
				else
				{
					this.boostVectorThisFrame = (this.gtPlayer.enableHoverMode ? (this.gtPlayer.turnParent.transform.rotation * -this.velocityTracker.GetAverageVelocity(false, 0.15f, false) * paddleBoostFactor) : Vector3.zero);
				}
				Vector3 vector = this.GetCurrentHandPosition() + this.gtPlayer.movingSurfaceOffset;
				Vector3 vector2 = this.GetLastPosition();
				Vector3 vector3 = vector - vector2;
				bool flag = this.gtPlayer.lastMovingSurfaceContact == GTPlayer.MovingSurfaceContactPoint.LEFT;
				if (!this.gtPlayer.didAJump && this.wasSliding && Vector3.Dot(this.gtPlayer.slideAverageNormal, GTPlayerTransform.PhysicsUp) > 0f)
				{
					vector3 += Vector3.Project(-this.gtPlayer.slideAverageNormal * this.gtPlayer.stickDepth * this.gtPlayer.scale, GTPlayerTransform.PhysicsDown);
				}
				float num = this.gtPlayer.minimumRaycastDistance * this.gtPlayer.scale;
				if (this.gtPlayer.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
				{
					num = (this.gtPlayer.minimumRaycastDistance + VRRig.LocalRig.iceCubeRight.transform.localScale.y / 2f) * this.gtPlayer.scale;
				}
				Vector3 vector4 = Vector3.zero;
				if (flag && !this.gtPlayer.exitMovingSurface)
				{
					vector4 = Vector3.Project(-this.gtPlayer.lastMovingSurfaceHit.normal * (this.gtPlayer.stickDepth * this.gtPlayer.scale), GTPlayerTransform.PhysicsDown);
					if (this.gtPlayer.scale < 0.5f)
					{
						Vector3 normalized = this.gtPlayer.MovingSurfaceMovement().normalized;
						if (normalized != Vector3.zero)
						{
							float num2 = Vector3.Dot(GTPlayerTransform.PhysicsUp, normalized);
							if ((double)num2 > 0.9 || (double)num2 < -0.9)
							{
								vector4 *= 6f;
								num *= 1.1f;
							}
						}
					}
				}
				Vector3 vector5;
				RaycastHit raycastHit;
				Vector3 vector6;
				if (this.gtPlayer.IterativeCollisionSphereCast(vector2, num, vector3 + vector4, this.boostVectorThisFrame, out vector5, true, out this.slipPercentage, out raycastHit, this.SlipOverriddenToMax()) && !this.isHolding && !this.gtPlayer.InReportMenu)
				{
					if (this.wasColliding && this.slipPercentage <= this.gtPlayer.defaultSlideFactor && !this.boostVectorThisFrame.IsLongerThan(0f))
					{
						vector6 = vector2 - vector;
					}
					else
					{
						vector6 = vector5 - vector;
					}
					this.isSliding = this.slipPercentage > this.gtPlayer.iceThreshold;
					this.slideNormal = this.gtPlayer.tempHitInfo.normal;
					this.isColliding = true;
					this.materialTouchIndex = this.gtPlayer.currentMaterialIndex;
					this.surfaceOverride = this.gtPlayer.currentOverride;
					this.gtPlayer.lastHitInfoHand = raycastHit;
					this.lastHitInfo = raycastHit;
				}
				else
				{
					vector6 = Vector3.zero;
					this.slipPercentage = 0f;
					this.isSliding = false;
					this.slideNormal = GTPlayerTransform.PhysicsUp;
					this.isColliding = false;
					this.materialTouchIndex = 0;
					this.surfaceOverride = null;
				}
				bool flag2 = (this.isLeftHand ? this.gtPlayer.controllerState.LeftValid : this.gtPlayer.controllerState.RightValid);
				this.isColliding = this.isColliding && flag2;
				this.isSliding = this.isSliding && flag2;
				if (this.isColliding)
				{
					this.gtPlayer.anyHandIsColliding = true;
					if (this.isSliding)
					{
						this.gtPlayer.anyHandIsSliding = true;
					}
					else
					{
						this.gtPlayer.anyHandIsSticking = true;
					}
				}
				if (this.isColliding || this.wasColliding)
				{
					if (!this.surfaceOverride || !this.surfaceOverride.disablePushBackEffect)
					{
						totalMove += vector6;
					}
					divisor++;
				}
			}

			// Token: 0x06007132 RID: 28978 RVA: 0x0024BE24 File Offset: 0x0024A024
			public void FinalizeHandPosition()
			{
				Vector3 vector = this.GetLastPosition();
				if (Time.time < this.tempFreezeUntilTimestamp)
				{
					this.finalPositionThisFrame = vector;
				}
				else
				{
					Vector3 vector2 = this.GetCurrentHandPosition() - vector;
					float num = this.gtPlayer.minimumRaycastDistance * this.gtPlayer.scale;
					if (this.gtPlayer.IsFrozen && GorillaGameManager.instance is GorillaFreezeTagManager)
					{
						num = (this.gtPlayer.minimumRaycastDistance + VRRig.LocalRig.iceCubeRight.transform.localScale.y / 2f) * this.gtPlayer.scale;
					}
					Vector3 vector3;
					float num2;
					RaycastHit raycastHit;
					if (this.gtPlayer.IterativeCollisionSphereCast(vector, num, vector2, this.boostVectorThisFrame, out vector3, this.gtPlayer.areBothTouching, out num2, out raycastHit, false) && !this.isHolding)
					{
						this.isColliding = true;
						this.isSliding = num2 > this.gtPlayer.iceThreshold;
						this.materialTouchIndex = this.gtPlayer.currentMaterialIndex;
						this.surfaceOverride = this.gtPlayer.currentOverride;
						this.gtPlayer.lastHitInfoHand = raycastHit;
						this.lastHitInfo = raycastHit;
						this.finalPositionThisFrame = vector3;
					}
					else
					{
						this.finalPositionThisFrame = this.GetCurrentHandPosition();
					}
				}
				bool flag = (this.isLeftHand ? this.gtPlayer.controllerState.LeftValid : this.gtPlayer.controllerState.RightValid);
				this.isColliding = this.isColliding && flag;
				this.isSliding = this.isSliding && flag;
				if (this.isColliding)
				{
					this.gtPlayer.anyHandIsColliding = true;
					if (this.isSliding)
					{
						this.gtPlayer.anyHandIsSliding = true;
						return;
					}
					this.gtPlayer.anyHandIsSticking = true;
				}
			}

			// Token: 0x06007133 RID: 28979 RVA: 0x0024B991 File Offset: 0x00249B91
			public bool IsSlipOverriddenToMax()
			{
				return this.slipSetToMaxFrameIdx == Time.frameCount;
			}

			// Token: 0x06007134 RID: 28980 RVA: 0x0024BFDC File Offset: 0x0024A1DC
			public Vector3 GetCurrentHandPosition()
			{
				Vector3 position = this.gtPlayer.headCollider.transform.position;
				if (this.gtPlayer.inOverlay)
				{
					return position + this.gtPlayer.headCollider.transform.up * -0.5f * this.gtPlayer.scale;
				}
				Vector3 vector = this.gtPlayer.PositionWithOffset(this.controllerTransform, this.handOffset);
				if ((vector - position).IsShorterThan(this.maxArmLength * this.gtPlayer.scale))
				{
					return vector;
				}
				return position + (vector - position).normalized * this.maxArmLength * this.gtPlayer.scale;
			}

			// Token: 0x06007135 RID: 28981 RVA: 0x0024C0AC File Offset: 0x0024A2AC
			public void PositionHandFollower()
			{
				this.handFollower.position = this.finalPositionThisFrame;
				this.handFollower.rotation = this.lastRotation;
			}

			// Token: 0x06007136 RID: 28982 RVA: 0x0024C0D0 File Offset: 0x0024A2D0
			public void OnEndOfFrame()
			{
				this.wasColliding = this.isColliding;
				this.wasSliding = this.isSliding;
				this.lastPosition = this.finalPositionThisFrame;
				if (Time.time > this.tempFreezeUntilTimestamp)
				{
					this.lastRotation = this.controllerTransform.rotation * this.handRotOffset;
				}
			}

			// Token: 0x06007137 RID: 28983 RVA: 0x0024C12A File Offset: 0x0024A32A
			public void TempFreezeHand(float freezeDuration)
			{
				this.tempFreezeUntilTimestamp = Math.Max(this.tempFreezeUntilTimestamp, Time.time + freezeDuration);
			}

			// Token: 0x06007138 RID: 28984 RVA: 0x0024C144 File Offset: 0x0024A344
			public void GetHandTapData(out bool wasHandTouching, out bool wasSliding, out int handMatIndex, out GorillaSurfaceOverride surfaceOverride, out RaycastHit handHitInfo, out Vector3 handPosition, out GorillaVelocityTracker handVelocityTracker)
			{
				wasHandTouching = this.wasColliding;
				wasSliding = this.wasSliding;
				handMatIndex = this.materialTouchIndex;
				surfaceOverride = this.surfaceOverride;
				handHitInfo = this.lastHitInfo;
				handPosition = this.finalPositionThisFrame;
				handVelocityTracker = this.velocityTracker;
			}

			// Token: 0x04008172 RID: 33138
			[NonSerialized]
			public Vector3 lastPosition;

			// Token: 0x04008173 RID: 33139
			[NonSerialized]
			public Quaternion lastRotation;

			// Token: 0x04008174 RID: 33140
			[NonSerialized]
			public bool isLeftHand;

			// Token: 0x04008175 RID: 33141
			[NonSerialized]
			public bool wasColliding;

			// Token: 0x04008176 RID: 33142
			[NonSerialized]
			public bool isColliding;

			// Token: 0x04008177 RID: 33143
			[NonSerialized]
			public bool wasSliding;

			// Token: 0x04008178 RID: 33144
			[NonSerialized]
			public bool isSliding;

			// Token: 0x04008179 RID: 33145
			[NonSerialized]
			public bool isHolding;

			// Token: 0x0400817A RID: 33146
			[NonSerialized]
			public Vector3 slideNormal;

			// Token: 0x0400817B RID: 33147
			[NonSerialized]
			public float slipPercentage;

			// Token: 0x0400817C RID: 33148
			[NonSerialized]
			public Vector3 hitPoint;

			// Token: 0x0400817D RID: 33149
			[NonSerialized]
			private Vector3 boostVectorThisFrame;

			// Token: 0x0400817E RID: 33150
			[NonSerialized]
			public Vector3 finalPositionThisFrame;

			// Token: 0x0400817F RID: 33151
			[NonSerialized]
			public int slipSetToMaxFrameIdx;

			// Token: 0x04008180 RID: 33152
			[NonSerialized]
			public int materialTouchIndex;

			// Token: 0x04008181 RID: 33153
			[NonSerialized]
			public GorillaSurfaceOverride surfaceOverride;

			// Token: 0x04008182 RID: 33154
			[NonSerialized]
			public RaycastHit hitInfo;

			// Token: 0x04008183 RID: 33155
			[NonSerialized]
			public RaycastHit lastHitInfo;

			// Token: 0x04008184 RID: 33156
			[NonSerialized]
			private GTPlayer gtPlayer;

			// Token: 0x04008185 RID: 33157
			[SerializeField]
			public Transform handFollower;

			// Token: 0x04008186 RID: 33158
			[SerializeField]
			public Transform controllerTransform;

			// Token: 0x04008187 RID: 33159
			[SerializeField]
			public GorillaVelocityTracker velocityTracker;

			// Token: 0x04008188 RID: 33160
			[SerializeField]
			public GorillaVelocityTracker interactPointVelocityTracker;

			// Token: 0x04008189 RID: 33161
			[SerializeField]
			public Vector3 handOffset;

			// Token: 0x0400818A RID: 33162
			[SerializeField]
			public Quaternion handRotOffset;

			// Token: 0x0400818B RID: 33163
			[NonSerialized]
			public float tempFreezeUntilTimestamp;

			// Token: 0x0400818C RID: 33164
			[NonSerialized]
			public bool canTag;

			// Token: 0x0400818D RID: 33165
			[NonSerialized]
			public bool canStun;

			// Token: 0x0400818E RID: 33166
			private float maxArmLength;

			// Token: 0x0400818F RID: 33167
			[NonSerialized]
			public bool isActive;

			// Token: 0x04008190 RID: 33168
			[NonSerialized]
			public float customBoostFactor;

			// Token: 0x04008191 RID: 33169
			[NonSerialized]
			public bool hasCustomBoost;
		}

		// Token: 0x02001188 RID: 4488
		private enum MovingSurfaceContactPoint
		{
			// Token: 0x04008193 RID: 33171
			NONE,
			// Token: 0x04008194 RID: 33172
			RIGHT,
			// Token: 0x04008195 RID: 33173
			LEFT,
			// Token: 0x04008196 RID: 33174
			BODY
		}

		// Token: 0x02001189 RID: 4489
		[Serializable]
		public struct MaterialData
		{
			// Token: 0x04008197 RID: 33175
			public string matName;

			// Token: 0x04008198 RID: 33176
			public bool overrideAudio;

			// Token: 0x04008199 RID: 33177
			public AudioClip audio;

			// Token: 0x0400819A RID: 33178
			public bool overrideSlidePercent;

			// Token: 0x0400819B RID: 33179
			public float slidePercent;

			// Token: 0x0400819C RID: 33180
			public int surfaceEffectIndex;
		}

		// Token: 0x0200118A RID: 4490
		[Serializable]
		public struct LiquidProperties
		{
			// Token: 0x0400819D RID: 33181
			[Range(0f, 2f)]
			[Tooltip("0: no resistance just like air, 1: full resistance like solid geometry")]
			public float resistance;

			// Token: 0x0400819E RID: 33182
			[Range(0f, 3f)]
			[Tooltip("0: no buoyancy. 1: Fully compensates gravity. 2: net force is upwards equal to gravity")]
			public float buoyancy;

			// Token: 0x0400819F RID: 33183
			[Range(0f, 100f)]
			[Tooltip("Damping Half-life Multiplier")]
			public float dampingFactor;

			// Token: 0x040081A0 RID: 33184
			[Range(0f, 1f)]
			public float surfaceJumpFactor;
		}

		// Token: 0x0200118B RID: 4491
		public enum LiquidType
		{
			// Token: 0x040081A2 RID: 33186
			Water,
			// Token: 0x040081A3 RID: 33187
			Lava,
			// Token: 0x040081A4 RID: 33188
			SwimInAir
		}

		// Token: 0x0200118C RID: 4492
		private struct HoverBoardCast
		{
			// Token: 0x040081A5 RID: 33189
			public Vector3 localOrigin;

			// Token: 0x040081A6 RID: 33190
			public Vector3 localDirection;

			// Token: 0x040081A7 RID: 33191
			public float sphereRadius;

			// Token: 0x040081A8 RID: 33192
			public float distance;

			// Token: 0x040081A9 RID: 33193
			public float intersectToVelocityCap;

			// Token: 0x040081AA RID: 33194
			public bool isSolid;

			// Token: 0x040081AB RID: 33195
			public bool didHit;

			// Token: 0x040081AC RID: 33196
			public Vector3 pointHit;

			// Token: 0x040081AD RID: 33197
			public Vector3 normalHit;
		}

		// Token: 0x0200118D RID: 4493
		private struct HandHoldState
		{
			// Token: 0x040081AE RID: 33198
			public GorillaGrabber grabber;

			// Token: 0x040081AF RID: 33199
			public Transform objectHeld;

			// Token: 0x040081B0 RID: 33200
			public Vector3 localPositionHeld;

			// Token: 0x040081B1 RID: 33201
			public float localRotationalOffset;

			// Token: 0x040081B2 RID: 33202
			public bool applyRotation;
		}
	}
}
