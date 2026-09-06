using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace UnityEngine.XR.Interaction.Toolkit
{
	// Token: 0x02000F39 RID: 3897
	public class GorillaSnapTurn : LocomotionProvider, ITickSystemTick
	{
		// Token: 0x17000941 RID: 2369
		// (get) Token: 0x06005F9C RID: 24476 RVA: 0x001E59C0 File Offset: 0x001E3BC0
		// (set) Token: 0x06005F9D RID: 24477 RVA: 0x001E59C8 File Offset: 0x001E3BC8
		public bool TickRunning { get; set; }

		// Token: 0x17000942 RID: 2370
		// (get) Token: 0x06005F9E RID: 24478 RVA: 0x001E59D1 File Offset: 0x001E3BD1
		// (set) Token: 0x06005F9F RID: 24479 RVA: 0x001E59D9 File Offset: 0x001E3BD9
		public GorillaSnapTurn.InputAxes turnUsage
		{
			get
			{
				return this.m_TurnUsage;
			}
			set
			{
				this.m_TurnUsage = value;
			}
		}

		// Token: 0x17000943 RID: 2371
		// (get) Token: 0x06005FA0 RID: 24480 RVA: 0x001E59E2 File Offset: 0x001E3BE2
		// (set) Token: 0x06005FA1 RID: 24481 RVA: 0x001E59EA File Offset: 0x001E3BEA
		public List<XRController> controllers
		{
			get
			{
				return this.m_Controllers;
			}
			set
			{
				this.m_Controllers = value;
			}
		}

		// Token: 0x17000944 RID: 2372
		// (get) Token: 0x06005FA2 RID: 24482 RVA: 0x001E59F3 File Offset: 0x001E3BF3
		// (set) Token: 0x06005FA3 RID: 24483 RVA: 0x001E59FB File Offset: 0x001E3BFB
		public float turnAmount
		{
			get
			{
				return this.m_TurnAmount;
			}
			set
			{
				this.m_TurnAmount = value;
			}
		}

		// Token: 0x17000945 RID: 2373
		// (get) Token: 0x06005FA4 RID: 24484 RVA: 0x001E5A04 File Offset: 0x001E3C04
		// (set) Token: 0x06005FA5 RID: 24485 RVA: 0x001E5A0C File Offset: 0x001E3C0C
		public float debounceTime
		{
			get
			{
				return this.m_DebounceTime;
			}
			set
			{
				this.m_DebounceTime = value;
			}
		}

		// Token: 0x17000946 RID: 2374
		// (get) Token: 0x06005FA6 RID: 24486 RVA: 0x001E5A15 File Offset: 0x001E3C15
		// (set) Token: 0x06005FA7 RID: 24487 RVA: 0x001E5A1D File Offset: 0x001E3C1D
		public float deadZone
		{
			get
			{
				return this.m_DeadZone;
			}
			set
			{
				this.m_DeadZone = value;
			}
		}

		// Token: 0x17000947 RID: 2375
		// (get) Token: 0x06005FA8 RID: 24488 RVA: 0x001E5A26 File Offset: 0x001E3C26
		// (set) Token: 0x06005FA9 RID: 24489 RVA: 0x001E5A2E File Offset: 0x001E3C2E
		public string turnType
		{
			get
			{
				return this.m_TurnType;
			}
			private set
			{
				this.m_TurnType = value;
			}
		}

		// Token: 0x17000948 RID: 2376
		// (get) Token: 0x06005FAA RID: 24490 RVA: 0x001E5A37 File Offset: 0x001E3C37
		// (set) Token: 0x06005FAB RID: 24491 RVA: 0x001E5A3F File Offset: 0x001E3C3F
		public int turnFactor
		{
			get
			{
				return this.m_TurnFactor;
			}
			private set
			{
				this.m_TurnFactor = value;
			}
		}

		// Token: 0x17000949 RID: 2377
		// (get) Token: 0x06005FAC RID: 24492 RVA: 0x001E5A48 File Offset: 0x001E3C48
		public static GorillaSnapTurn CachedSnapTurnRef
		{
			get
			{
				if (GorillaSnapTurn._cachedReference == null)
				{
					Debug.LogError("[SNAP_TURN] Tried accessing static cached reference, but was still null. Trying to find component in scene");
					GorillaSnapTurn._cachedReference = Object.FindAnyObjectByType<GorillaSnapTurn>();
				}
				return GorillaSnapTurn._cachedReference;
			}
		}

		// Token: 0x06005FAD RID: 24493 RVA: 0x001E5A70 File Offset: 0x001E3C70
		protected override void Awake()
		{
			base.Awake();
			if (GorillaSnapTurn._cachedReference != null)
			{
				Debug.LogError("[SNAP_TURN] A [GorillaSnapTurn] component already exists in the scene");
				return;
			}
			GorillaSnapTurn._cachedReference = this;
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06005FAE RID: 24494 RVA: 0x001E5A9C File Offset: 0x001E3C9C
		public void Tick()
		{
			this.ValidateTurningOverriders();
			if (this.m_Controllers.Count > 0)
			{
				this.EnsureControllerDataListSize();
				for (int i = 0; i < this.m_Controllers.Count; i++)
				{
					XRController xrcontroller = this.m_Controllers[i];
					if (!(xrcontroller == null) && xrcontroller.enableInputActions)
					{
						float num = 0f;
						if (xrcontroller.controllerNode == XRNode.RightHand)
						{
							num = ControllerInputPoller.instance.rightControllerPrimary2DAxis.x;
						}
						else if (xrcontroller.controllerNode == XRNode.LeftHand)
						{
							num = ControllerInputPoller.instance.leftControllerPrimary2DAxis.x;
						}
						if (num > this.deadZone)
						{
							this.StartTurn(this.m_TurnAmount);
						}
						else if (num < -this.deadZone)
						{
							this.StartTurn(-this.m_TurnAmount);
						}
						else
						{
							this.m_AxisReset = true;
						}
					}
				}
			}
			if (Mathf.Abs(this.m_CurrentTurnAmount) > 0f && base.TryPrepareLocomotion())
			{
				if (this.xrOrigin != null)
				{
					GTPlayer.Instance.Turn(this.m_CurrentTurnAmount);
				}
				this.m_CurrentTurnAmount = 0f;
				base.TryEndLocomotion();
			}
		}

		// Token: 0x06005FAF RID: 24495 RVA: 0x001E5BC0 File Offset: 0x001E3DC0
		private void EnsureControllerDataListSize()
		{
			if (this.m_Controllers.Count != this.m_ControllersWereActive.Count)
			{
				while (this.m_ControllersWereActive.Count < this.m_Controllers.Count)
				{
					this.m_ControllersWereActive.Add(false);
				}
				while (this.m_ControllersWereActive.Count < this.m_Controllers.Count)
				{
					this.m_ControllersWereActive.RemoveAt(this.m_ControllersWereActive.Count - 1);
				}
			}
		}

		// Token: 0x06005FB0 RID: 24496 RVA: 0x001E5C3D File Offset: 0x001E3E3D
		internal void FakeStartTurn(bool isLeft)
		{
			this.StartTurn(isLeft ? (-this.m_TurnAmount) : this.m_TurnAmount);
		}

		// Token: 0x06005FB1 RID: 24497 RVA: 0x001E5C58 File Offset: 0x001E3E58
		private void StartTurn(float amount)
		{
			if (this.m_TimeStarted + this.m_DebounceTime > Time.time && !this.m_AxisReset)
			{
				return;
			}
			if (base.isLocomotionActive)
			{
				return;
			}
			if (this.turningOverriders.Count > 0)
			{
				return;
			}
			this.m_TimeStarted = Time.time;
			this.m_CurrentTurnAmount = amount;
			this.m_AxisReset = false;
		}

		// Token: 0x06005FB2 RID: 24498 RVA: 0x001E5CB4 File Offset: 0x001E3EB4
		public void ChangeTurnMode(string turnMode, int turnSpeedFactor)
		{
			this.turnType = turnMode;
			this.turnFactor = turnSpeedFactor;
			if (turnMode == "SNAP")
			{
				this.m_DebounceTime = 0.5f;
				this.m_TurnAmount = 60f * this.ConvertedTurnFactor((float)turnSpeedFactor);
				return;
			}
			if (!(turnMode == "SMOOTH"))
			{
				this.m_DebounceTime = 0f;
				this.m_TurnAmount = 0f;
				return;
			}
			this.m_DebounceTime = 0f;
			this.m_TurnAmount = 360f * Time.fixedDeltaTime * this.ConvertedTurnFactor((float)turnSpeedFactor);
		}

		// Token: 0x06005FB3 RID: 24499 RVA: 0x001E5D47 File Offset: 0x001E3F47
		public float ConvertedTurnFactor(float newTurnSpeed)
		{
			return Mathf.Max(0.75f, 0.5f + newTurnSpeed / 10f * 1.5f);
		}

		// Token: 0x06005FB4 RID: 24500 RVA: 0x001E5D66 File Offset: 0x001E3F66
		public void SetTurningOverride(ISnapTurnOverride caller)
		{
			if (!this.turningOverriders.Contains(caller))
			{
				this.turningOverriders.Add(caller);
			}
		}

		// Token: 0x06005FB5 RID: 24501 RVA: 0x001E5D83 File Offset: 0x001E3F83
		public void UnsetTurningOverride(ISnapTurnOverride caller)
		{
			if (this.turningOverriders.Contains(caller))
			{
				this.turningOverriders.Remove(caller);
			}
		}

		// Token: 0x06005FB6 RID: 24502 RVA: 0x001E5DA0 File Offset: 0x001E3FA0
		public void ValidateTurningOverriders()
		{
			foreach (ISnapTurnOverride snapTurnOverride in this.turningOverriders)
			{
				if (snapTurnOverride == null || !snapTurnOverride.TurnOverrideActive())
				{
					this.turningOverriders.Remove(snapTurnOverride);
				}
			}
		}

		// Token: 0x06005FB7 RID: 24503 RVA: 0x001E5E04 File Offset: 0x001E4004
		public static void DisableSnapTurn()
		{
			Debug.Log("[SNAP_TURN] Disabling Snap Turn");
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				return;
			}
			GorillaSnapTurn._cachedTurnFactor = PlayerPrefs.GetInt("turnFactor");
			GorillaSnapTurn._cachedTurnType = PlayerPrefs.GetString("stickTurning");
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode("NONE", 0);
		}

		// Token: 0x06005FB8 RID: 24504 RVA: 0x001E5E57 File Offset: 0x001E4057
		public static void UpdateAndSaveTurnType(string mode)
		{
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				Debug.LogError("[SNAP_TURN] Failed to Update, [CachedSnapTurnRef] is NULL");
				return;
			}
			PlayerPrefs.SetString("stickTurning", mode);
			PlayerPrefs.Save();
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode(mode, GorillaSnapTurn.CachedSnapTurnRef.turnFactor);
		}

		// Token: 0x06005FB9 RID: 24505 RVA: 0x001E5E96 File Offset: 0x001E4096
		public static void UpdateAndSaveTurnFactor(int factor)
		{
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				Debug.LogError("[SNAP_TURN] Failed to Update, [CachedSnapTurnRef] is NULL");
				return;
			}
			PlayerPrefs.SetInt("turnFactor", factor);
			PlayerPrefs.Save();
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode(GorillaSnapTurn.CachedSnapTurnRef.turnType, factor);
		}

		// Token: 0x06005FBA RID: 24506 RVA: 0x001E5ED8 File Offset: 0x001E40D8
		public static void LoadSettingsFromPlayerPrefs()
		{
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				return;
			}
			string text = ((Application.platform == RuntimePlatform.Android) ? "NONE" : "SNAP");
			string @string = PlayerPrefs.GetString("stickTurning", text);
			int @int = PlayerPrefs.GetInt("turnFactor", 4);
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode(@string, @int);
		}

		// Token: 0x06005FBB RID: 24507 RVA: 0x001E5F30 File Offset: 0x001E4130
		public static void LoadSettingsFromCache()
		{
			if (GorillaSnapTurn.CachedSnapTurnRef == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(GorillaSnapTurn._cachedTurnType))
			{
				GorillaSnapTurn._cachedTurnType = ((Application.platform == RuntimePlatform.Android) ? "NONE" : "SNAP");
			}
			string cachedTurnType = GorillaSnapTurn._cachedTurnType;
			int cachedTurnFactor = GorillaSnapTurn._cachedTurnFactor;
			GorillaSnapTurn.CachedSnapTurnRef.ChangeTurnMode(cachedTurnType, cachedTurnFactor);
		}

		// Token: 0x04006E04 RID: 28164
		[Header("References")]
		[SerializeField]
		private XROrigin xrOrigin;

		// Token: 0x04006E05 RID: 28165
		private static readonly InputFeatureUsage<Vector2>[] m_Vec2UsageList = new InputFeatureUsage<Vector2>[]
		{
			CommonUsages.primary2DAxis,
			CommonUsages.secondary2DAxis
		};

		// Token: 0x04006E06 RID: 28166
		[SerializeField]
		[Tooltip("The 2D Input Axis on the primary devices that will be used to trigger a snap turn.")]
		private GorillaSnapTurn.InputAxes m_TurnUsage;

		// Token: 0x04006E07 RID: 28167
		[SerializeField]
		[Tooltip("A list of controllers that allow Snap Turn.  If an XRController is not enabled, or does not have input actions enabled.  Snap Turn will not work.")]
		private List<XRController> m_Controllers = new List<XRController>();

		// Token: 0x04006E08 RID: 28168
		[SerializeField]
		[Tooltip("The number of degrees clockwise to rotate when snap turning clockwise.")]
		private float m_TurnAmount = 45f;

		// Token: 0x04006E09 RID: 28169
		[SerializeField]
		[Tooltip("The amount of time that the system will wait before starting another snap turn.")]
		private float m_DebounceTime = 0.5f;

		// Token: 0x04006E0A RID: 28170
		[SerializeField]
		[Tooltip("The deadzone that the controller movement will have to be above to trigger a snap turn.")]
		private float m_DeadZone = 0.75f;

		// Token: 0x04006E0B RID: 28171
		private float m_CurrentTurnAmount;

		// Token: 0x04006E0C RID: 28172
		private float m_TimeStarted;

		// Token: 0x04006E0D RID: 28173
		private bool m_AxisReset;

		// Token: 0x04006E0E RID: 28174
		public float turnSpeed = 1f;

		// Token: 0x04006E0F RID: 28175
		private HashSet<ISnapTurnOverride> turningOverriders = new HashSet<ISnapTurnOverride>();

		// Token: 0x04006E10 RID: 28176
		private List<bool> m_ControllersWereActive = new List<bool>();

		// Token: 0x04006E11 RID: 28177
		private static int _cachedTurnFactor;

		// Token: 0x04006E12 RID: 28178
		private static string _cachedTurnType;

		// Token: 0x04006E13 RID: 28179
		private string m_TurnType = "";

		// Token: 0x04006E14 RID: 28180
		private int m_TurnFactor = 1;

		// Token: 0x04006E15 RID: 28181
		[OnEnterPlay_SetNull]
		private static GorillaSnapTurn _cachedReference;

		// Token: 0x02000F3A RID: 3898
		public enum InputAxes
		{
			// Token: 0x04006E17 RID: 28183
			Primary2DAxis,
			// Token: 0x04006E18 RID: 28184
			Secondary2DAxis
		}
	}
}
