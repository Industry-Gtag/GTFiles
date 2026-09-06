using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001304 RID: 4868
	[Serializable]
	public class ContinuousProperty
	{
		// Token: 0x06007A2B RID: 31275 RVA: 0x0027D9C0 File Offset: 0x0027BBC0
		private static ContinuousProperty.Cast GetTargetCast(Object o)
		{
			ContinuousProperty.Cast cast;
			if (!(o is ParticleSystem))
			{
				if (!(o is SkinnedMeshRenderer))
				{
					if (!(o is Animator))
					{
						if (!(o is AudioSource))
						{
							if (!(o is VoiceShiftCosmetic))
							{
								if (!(o is Rigidbody))
								{
									if (!(o is Transform))
									{
										if (!(o is Renderer))
										{
											if (!(o is Behaviour))
											{
												if (!(o is GameObject))
												{
													cast = ContinuousProperty.Cast.Null;
												}
												else
												{
													cast = ContinuousProperty.Cast.GameObject;
												}
											}
											else
											{
												cast = ContinuousProperty.Cast.Behaviour;
											}
										}
										else
										{
											cast = ContinuousProperty.Cast.Renderer;
										}
									}
									else
									{
										cast = ContinuousProperty.Cast.Transform;
									}
								}
								else
								{
									cast = ContinuousProperty.Cast.Rigidbody;
								}
							}
							else
							{
								cast = ContinuousProperty.Cast.VoicePitchShiftCosmetic;
							}
						}
						else
						{
							cast = ContinuousProperty.Cast.AudioSource;
						}
					}
					else
					{
						cast = ContinuousProperty.Cast.Animator;
					}
				}
				else
				{
					cast = ContinuousProperty.Cast.SkinnedMeshRenderer;
				}
			}
			else
			{
				cast = ContinuousProperty.Cast.ParticleSystem;
			}
			return cast;
		}

		// Token: 0x06007A2C RID: 31276 RVA: 0x0027DA74 File Offset: 0x0027BC74
		public static bool CastMatches(ContinuousProperty.Cast cast, ContinuousProperty.Cast test)
		{
			if (cast <= ContinuousProperty.Cast.Any)
			{
				if (cast == ContinuousProperty.Cast.Null)
				{
					return false;
				}
				if (cast == ContinuousProperty.Cast.Any)
				{
					return true;
				}
			}
			else
			{
				if (cast == ContinuousProperty.Cast.Renderer)
				{
					return test == ContinuousProperty.Cast.Renderer || test == ContinuousProperty.Cast.SkinnedMeshRenderer;
				}
				if (cast == ContinuousProperty.Cast.Behaviour)
				{
					return test != ContinuousProperty.Cast.Transform && test != ContinuousProperty.Cast.GameObject && test != ContinuousProperty.Cast.Rigidbody;
				}
			}
			return test == cast;
		}

		// Token: 0x06007A2D RID: 31277 RVA: 0x0027DAED File Offset: 0x0027BCED
		public static bool HasAllFlags(ContinuousProperty.DataFlags flags, ContinuousProperty.DataFlags test)
		{
			return (flags & test) == test;
		}

		// Token: 0x06007A2E RID: 31278 RVA: 0x0027DAF5 File Offset: 0x0027BCF5
		public static bool HasAnyFlag(ContinuousProperty.DataFlags flags, ContinuousProperty.DataFlags test)
		{
			return (flags & test) > ContinuousProperty.DataFlags.None;
		}

		// Token: 0x06007A2F RID: 31279 RVA: 0x0027DB00 File Offset: 0x0027BD00
		private static void GetAllValidObjectsNonAlloc(Transform t, List<Object> objects)
		{
			objects.Clear();
			objects.Add(t.gameObject);
			foreach (Component @object in t.GetComponents<Component>())
			{
				if (ContinuousProperty.IsValidObject(@object.GetType()))
				{
					objects.Add(@object);
				}
			}
		}

		// Token: 0x06007A30 RID: 31280 RVA: 0x0027DB4C File Offset: 0x0027BD4C
		private static bool IsValidObject(global::System.Type t)
		{
			return t != typeof(Renderer) && t != typeof(ParticleSystemRenderer);
		}

		// Token: 0x06007A31 RID: 31281 RVA: 0x0027DB74 File Offset: 0x0027BD74
		public ContinuousProperty()
		{
		}

		// Token: 0x06007A32 RID: 31282 RVA: 0x0027DBD8 File Offset: 0x0027BDD8
		public ContinuousProperty(ContinuousPropertyModeSO mode, Transform initialTarget, Vector2 range = default(Vector2))
		{
			this.mode = mode;
			this.target = initialTarget;
			this.range = range;
			this.ShiftTarget(0);
		}

		// Token: 0x17000BD3 RID: 3027
		// (get) Token: 0x06007A33 RID: 31283 RVA: 0x0027DC58 File Offset: 0x0027BE58
		private string ModeTooltip
		{
			get
			{
				if (!this.mode)
				{
					return "";
				}
				return string.Format("{0}: {1}", this.mode.type, this.mode.GetDescriptionForCast(ContinuousProperty.GetTargetCast(this.target)));
			}
		}

		// Token: 0x17000BD4 RID: 3028
		// (get) Token: 0x06007A34 RID: 31284 RVA: 0x0027DCA8 File Offset: 0x0027BEA8
		private bool ModeInfoVisible
		{
			get
			{
				return this.mode == null;
			}
		}

		// Token: 0x17000BD5 RID: 3029
		// (get) Token: 0x06007A35 RID: 31285 RVA: 0x0027DCB6 File Offset: 0x0027BEB6
		private bool ModeErrorVisible
		{
			get
			{
				return !this.IsValid();
			}
		}

		// Token: 0x17000BD6 RID: 3030
		// (get) Token: 0x06007A36 RID: 31286 RVA: 0x0027DCC1 File Offset: 0x0027BEC1
		private string ModeErrorMessage
		{
			get
			{
				if (!(this.mode != null))
				{
					return "How did we get here?";
				}
				return "I couldn't find any valid target to apply my '" + this.mode.name + "' to in the whole prefab.\n\n" + this.mode.ListValidCasts();
			}
		}

		// Token: 0x17000BD7 RID: 3031
		// (get) Token: 0x06007A37 RID: 31287 RVA: 0x0027DCFC File Offset: 0x0027BEFC
		public ContinuousPropertyModeSO Mode
		{
			get
			{
				return this.mode;
			}
		}

		// Token: 0x17000BD8 RID: 3032
		// (get) Token: 0x06007A38 RID: 31288 RVA: 0x0027DD04 File Offset: 0x0027BF04
		public ContinuousProperty.Type MyType
		{
			get
			{
				if (!(this.mode != null))
				{
					return ContinuousProperty.Type.Color;
				}
				return this.mode.type;
			}
		}

		// Token: 0x17000BD9 RID: 3033
		// (get) Token: 0x06007A39 RID: 31289 RVA: 0x0027DD21 File Offset: 0x0027BF21
		private bool HasTarget
		{
			get
			{
				return this.MyType != ContinuousProperty.Type.UnityEvent;
			}
		}

		// Token: 0x17000BDA RID: 3034
		// (get) Token: 0x06007A3A RID: 31290 RVA: 0x0027DD30 File Offset: 0x0027BF30
		private bool TargetInfoVisible
		{
			get
			{
				return this.HasTarget && this.target == null;
			}
		}

		// Token: 0x17000BDB RID: 3035
		// (get) Token: 0x06007A3B RID: 31291 RVA: 0x0027DD48 File Offset: 0x0027BF48
		private string TargetTooltip
		{
			get
			{
				if (!(this.mode != null))
				{
					return "";
				}
				return this.mode.ListValidCasts();
			}
		}

		// Token: 0x17000BDC RID: 3036
		// (get) Token: 0x06007A3C RID: 31292 RVA: 0x0027DD69 File Offset: 0x0027BF69
		private bool ShiftButtonsVisible
		{
			get
			{
				return this.mode != null;
			}
		}

		// Token: 0x17000BDD RID: 3037
		// (get) Token: 0x06007A3D RID: 31293 RVA: 0x0027DD77 File Offset: 0x0027BF77
		public Object Target
		{
			get
			{
				return this.target;
			}
		}

		// Token: 0x06007A3E RID: 31294 RVA: 0x0027DD7F File Offset: 0x0027BF7F
		private void PreviousTarget()
		{
			this.ShiftTarget(-1);
		}

		// Token: 0x06007A3F RID: 31295 RVA: 0x0027DD89 File Offset: 0x0027BF89
		private void NextTarget()
		{
			this.ShiftTarget(1);
		}

		// Token: 0x06007A40 RID: 31296 RVA: 0x0027DD94 File Offset: 0x0027BF94
		public bool ShiftTarget(int shiftAmount)
		{
			if (this.mode == null)
			{
				return false;
			}
			int num = -1;
			Transform transform;
			if (!(this.target != null))
			{
				transform = null;
			}
			else
			{
				GameObject gameObject = this.target as GameObject;
				transform = ((gameObject != null) ? gameObject.transform : null) ?? ((Component)this.target).transform;
			}
			Transform transform2 = transform;
			Transform transform3 = transform2;
			if (transform3 == null)
			{
				return false;
			}
			Stack<Transform> stack = new Stack<Transform>();
			stack.Push(transform3);
			List<Object> list = new List<Object>();
			List<Object> list2 = new List<Object>();
			Transform transform4;
			while (stack.TryPop(out transform4))
			{
				if (num < 0 && transform4 == transform2)
				{
					num = list.Count;
				}
				ContinuousProperty.GetAllValidObjectsNonAlloc(transform4, list2);
				foreach (Object @object in list2)
				{
					if (this.mode.IsCastValid(ContinuousProperty.GetTargetCast(@object)))
					{
						if (@object == this.target)
						{
							num = list.Count;
						}
						list.Add(@object);
					}
				}
				for (int i = transform4.childCount - 1; i >= 0; i--)
				{
					stack.Push(transform4.GetChild(i));
				}
			}
			if (list.Count == 0)
			{
				return false;
			}
			this.target = list[(num < 0) ? 0 : ((num + shiftAmount + list.Count) % list.Count)];
			return true;
		}

		// Token: 0x06007A41 RID: 31297 RVA: 0x0027DF14 File Offset: 0x0027C114
		private void OnModeOrTargetChanged()
		{
			if (!this.IsValid())
			{
				this.ShiftTarget(0);
			}
		}

		// Token: 0x17000BDE RID: 3038
		// (get) Token: 0x06007A42 RID: 31298 RVA: 0x0027DF26 File Offset: 0x0027C126
		// (set) Token: 0x06007A43 RID: 31299 RVA: 0x0027DF2E File Offset: 0x0027C12E
		public bool IsShaderProperty_Cached { get; private set; }

		// Token: 0x17000BDF RID: 3039
		// (get) Token: 0x06007A44 RID: 31300 RVA: 0x0027DF37 File Offset: 0x0027C137
		// (set) Token: 0x06007A45 RID: 31301 RVA: 0x0027DF3F File Offset: 0x0027C13F
		public bool UsesThreshold_Cached { get; private set; }

		// Token: 0x06007A46 RID: 31302 RVA: 0x0027DF48 File Offset: 0x0027C148
		public bool IsValid()
		{
			return this.mode == null || this.target == null || this.mode.IsCastValid(ContinuousProperty.GetTargetCast(this.target));
		}

		// Token: 0x06007A47 RID: 31303 RVA: 0x0027DF7E File Offset: 0x0027C17E
		public int GetTargetInstanceID()
		{
			return this.target.GetInstanceID();
		}

		// Token: 0x06007A48 RID: 31304 RVA: 0x0027DF8B File Offset: 0x0027C18B
		private bool HasAllFlags(ContinuousProperty.DataFlags test)
		{
			return this.mode != null && ContinuousProperty.HasAllFlags(this.mode.GetFlagsForClosestCast(ContinuousProperty.GetTargetCast(this.target)), test);
		}

		// Token: 0x06007A49 RID: 31305 RVA: 0x0027DFB9 File Offset: 0x0027C1B9
		private bool HasAnyFlag(ContinuousProperty.DataFlags test)
		{
			return this.mode != null && ContinuousProperty.HasAnyFlag(this.mode.GetFlagsForClosestCast(ContinuousProperty.GetTargetCast(this.target)), test);
		}

		// Token: 0x17000BE0 RID: 3040
		// (get) Token: 0x06007A4A RID: 31306 RVA: 0x0027DFE7 File Offset: 0x0027C1E7
		private bool HasGradient
		{
			get
			{
				return this.HasAllFlags(ContinuousProperty.DataFlags.HasColor);
			}
		}

		// Token: 0x17000BE1 RID: 3041
		// (get) Token: 0x06007A4B RID: 31307 RVA: 0x0027DFF0 File Offset: 0x0027C1F0
		private bool HasCurve
		{
			get
			{
				return this.HasAllFlags(ContinuousProperty.DataFlags.HasCurve);
			}
		}

		// Token: 0x06007A4C RID: 31308 RVA: 0x0027DFFC File Offset: 0x0027C1FC
		private string DynamicIntLabel()
		{
			if (!this.HasAllFlags(ContinuousProperty.DataFlags.IsShaderProperty))
			{
				ContinuousProperty.Type myType = this.MyType;
				if (myType != ContinuousProperty.Type.Color && myType != ContinuousProperty.Type.BlendShape)
				{
					return "Int Value";
				}
			}
			return "Material Index";
		}

		// Token: 0x17000BE2 RID: 3042
		// (get) Token: 0x06007A4D RID: 31309 RVA: 0x0027E02C File Offset: 0x0027C22C
		private bool HasInt
		{
			get
			{
				return this.HasAllFlags(ContinuousProperty.DataFlags.HasInteger);
			}
		}

		// Token: 0x17000BE3 RID: 3043
		// (get) Token: 0x06007A4E RID: 31310 RVA: 0x0027E035 File Offset: 0x0027C235
		public int IntValue
		{
			get
			{
				return this.intValue;
			}
		}

		// Token: 0x06007A4F RID: 31311 RVA: 0x0027E03D File Offset: 0x0027C23D
		private string DynamicStringLabel()
		{
			if (this.HasAllFlags(ContinuousProperty.DataFlags.IsShaderProperty))
			{
				return "Property Name";
			}
			if (this.HasAllFlags(ContinuousProperty.DataFlags.IsAnimatorParameter))
			{
				return "Parameter Name";
			}
			return "String Value";
		}

		// Token: 0x17000BE4 RID: 3044
		// (get) Token: 0x06007A50 RID: 31312 RVA: 0x0027E064 File Offset: 0x0027C264
		private bool HasString
		{
			get
			{
				return this.HasAnyFlag(ContinuousProperty.DataFlags.IsShaderProperty | ContinuousProperty.DataFlags.IsAnimatorParameter);
			}
		}

		// Token: 0x17000BE5 RID: 3045
		// (get) Token: 0x06007A51 RID: 31313 RVA: 0x0027E06E File Offset: 0x0027C26E
		public string StringValue
		{
			get
			{
				return this.stringValue;
			}
		}

		// Token: 0x17000BE6 RID: 3046
		// (get) Token: 0x06007A52 RID: 31314 RVA: 0x0027E076 File Offset: 0x0027C276
		private bool HasBezier
		{
			get
			{
				return this.MyType == ContinuousProperty.Type.BezierInterpolation;
			}
		}

		// Token: 0x17000BE7 RID: 3047
		// (get) Token: 0x06007A53 RID: 31315 RVA: 0x0027E081 File Offset: 0x0027C281
		private bool MissingBezier
		{
			get
			{
				return this.bezierCurve == null;
			}
		}

		// Token: 0x17000BE8 RID: 3048
		// (get) Token: 0x06007A54 RID: 31316 RVA: 0x0027E08F File Offset: 0x0027C28F
		private bool AxisError
		{
			get
			{
				return !Enum.IsDefined(typeof(ContinuousProperty.RotationAxis), this.localAxis);
			}
		}

		// Token: 0x17000BE9 RID: 3049
		// (get) Token: 0x06007A55 RID: 31317 RVA: 0x0027E0AE File Offset: 0x0027C2AE
		private bool HasAxisMode
		{
			get
			{
				return this.HasAllFlags(ContinuousProperty.DataFlags.HasAxis);
			}
		}

		// Token: 0x17000BEA RID: 3050
		// (get) Token: 0x06007A56 RID: 31318 RVA: 0x0027E0B7 File Offset: 0x0027C2B7
		private bool InterpolationError
		{
			get
			{
				return !Enum.IsDefined(typeof(ContinuousProperty.InterpolationMode), this.interpolationMode);
			}
		}

		// Token: 0x17000BEB RID: 3051
		// (get) Token: 0x06007A57 RID: 31319 RVA: 0x0027E0D6 File Offset: 0x0027C2D6
		private bool HasInterpolationMode
		{
			get
			{
				return this.HasAllFlags(ContinuousProperty.DataFlags.HasInterpolation);
			}
		}

		// Token: 0x17000BEC RID: 3052
		// (get) Token: 0x06007A58 RID: 31320 RVA: 0x0027E0E0 File Offset: 0x0027C2E0
		private bool HasStopAction
		{
			get
			{
				return this.MyType == ContinuousProperty.Type.PlayStop && this.target is ParticleSystem;
			}
		}

		// Token: 0x17000BED RID: 3053
		// (get) Token: 0x06007A59 RID: 31321 RVA: 0x0027E0FC File Offset: 0x0027C2FC
		private bool HasXforms
		{
			get
			{
				return this.MyType == ContinuousProperty.Type.TransformInterpolation;
			}
		}

		// Token: 0x17000BEE RID: 3054
		// (get) Token: 0x06007A5A RID: 31322 RVA: 0x0027E107 File Offset: 0x0027C307
		private bool MissingXforms
		{
			get
			{
				return this.transformA == null || this.transformB == null;
			}
		}

		// Token: 0x17000BEF RID: 3055
		// (get) Token: 0x06007A5B RID: 31323 RVA: 0x0027E125 File Offset: 0x0027C325
		private bool HasOffsets
		{
			get
			{
				return this.MyType == ContinuousProperty.Type.OffsetInterpolation;
			}
		}

		// Token: 0x17000BF0 RID: 3056
		// (get) Token: 0x06007A5C RID: 31324 RVA: 0x0027E131 File Offset: 0x0027C331
		private string ThresholdErrorMessage
		{
			get
			{
				return "The threshold will always be " + (((this.thresholdOption == ContinuousProperty.ThresholdOption.Normal) ^ (this.range.x >= this.range.y)) ? "true." : "false.");
			}
		}

		// Token: 0x17000BF1 RID: 3057
		// (get) Token: 0x06007A5D RID: 31325 RVA: 0x0027E170 File Offset: 0x0027C370
		private string ThresholdTooltip
		{
			get
			{
				if (!this.ThresholdError)
				{
					return "The threshold will be true" + ((this.thresholdOption == ContinuousProperty.ThresholdOption.Normal) ? ((this.range.x > 0f && this.range.y < 1f) ? string.Format(" between {0} and {1}", this.range.x, this.range.y) : ((this.range.x > 0f) ? (" above " + this.range.x.ToString()) : (" below " + this.range.y.ToString()))) : (((this.range.x > 0f) ? (" below " + this.range.x.ToString()) : "") + ((this.range.x > 0f && this.range.y < 1f) ? " and" : "") + ((this.range.y < 1f) ? (" above " + this.range.y.ToString()) : ""))) + ", and false otherwise.";
				}
				return this.ThresholdErrorMessage;
			}
		}

		// Token: 0x17000BF2 RID: 3058
		// (get) Token: 0x06007A5E RID: 31326 RVA: 0x0027E2E2 File Offset: 0x0027C4E2
		private bool HasThreshold
		{
			get
			{
				return this.HasAllFlags(ContinuousProperty.DataFlags.HasThreshold);
			}
		}

		// Token: 0x17000BF3 RID: 3059
		// (get) Token: 0x06007A5F RID: 31327 RVA: 0x0027E2F0 File Offset: 0x0027C4F0
		private bool ThresholdError
		{
			get
			{
				return (this.range.x <= 0f && this.range.y >= 1f) || this.range.x >= this.range.y;
			}
		}

		// Token: 0x17000BF4 RID: 3060
		// (get) Token: 0x06007A60 RID: 31328 RVA: 0x0027E33E File Offset: 0x0027C53E
		private bool HasEventMode
		{
			get
			{
				return this.MyType == ContinuousProperty.Type.UnityEvent && !this.HasAnyFlag(ContinuousProperty.DataFlags.HasThreshold);
			}
		}

		// Token: 0x17000BF5 RID: 3061
		// (get) Token: 0x06007A61 RID: 31329 RVA: 0x0027E35A File Offset: 0x0027C55A
		private bool HasUnityEvent
		{
			get
			{
				return this.MyType == ContinuousProperty.Type.UnityEvent;
			}
		}

		// Token: 0x17000BF6 RID: 3062
		// (get) Token: 0x06007A62 RID: 31330 RVA: 0x0027E366 File Offset: 0x0027C566
		public bool RunOnlyLocally
		{
			get
			{
				return this.runOnlyLocally;
			}
		}

		// Token: 0x06007A63 RID: 31331 RVA: 0x0027E36E File Offset: 0x0027C56E
		public void SetRigIsLocal(bool v)
		{
			this.rigLocal = v;
		}

		// Token: 0x06007A64 RID: 31332 RVA: 0x0027E378 File Offset: 0x0027C578
		public void Init()
		{
			if (this.mode == null)
			{
				this.internalSwitchValue = 0;
				return;
			}
			ContinuousProperty.Type type = this.mode.type;
			ContinuousProperty.Cast cast = this.mode.GetClosestCast(ContinuousProperty.GetTargetCast(this.target));
			ContinuousProperty.DataFlags dataFlags = this.mode.GetFlagsForCast(cast);
			if (cast == ContinuousProperty.Cast.Null || (type == ContinuousProperty.Type.BezierInterpolation && this.MissingBezier) || (type == ContinuousProperty.Type.TransformInterpolation && this.MissingXforms) || (type == ContinuousProperty.Type.UnityEvent && this.unityEvent == null))
			{
				this.internalSwitchValue = 0;
				this.IsShaderProperty_Cached = false;
				this.UsesThreshold_Cached = false;
				return;
			}
			if (type == ContinuousProperty.Type.Color && ContinuousProperty.CastMatches(ContinuousProperty.Cast.Renderer, cast))
			{
				type = ContinuousProperty.Type.ShaderColor;
				cast = ContinuousProperty.Cast.Renderer;
				dataFlags |= ContinuousProperty.DataFlags.IsShaderProperty;
				this.stringValue = "_BaseColor";
			}
			else if (type == ContinuousProperty.Type.PlayStop && cast == ContinuousProperty.Cast.Animator)
			{
				type = ContinuousProperty.Type.EnableDisable;
				cast = ContinuousProperty.Cast.Behaviour;
			}
			this.internalSwitchValue = (int)(type | (ContinuousProperty.Type)cast | (ContinuousProperty.Type)(this.HasAxisMode ? this.localAxis : ((ContinuousProperty.RotationAxis)0)) | (ContinuousProperty.Type)(this.HasInterpolationMode ? this.interpolationMode : ((ContinuousProperty.InterpolationMode)0)) | (ContinuousProperty.Type)(this.HasEventMode ? this.eventMode : ((ContinuousProperty.EventMode)0)));
			this.IsShaderProperty_Cached = ContinuousProperty.HasAllFlags(dataFlags, ContinuousProperty.DataFlags.IsShaderProperty);
			this.UsesThreshold_Cached = ContinuousProperty.HasAllFlags(dataFlags, ContinuousProperty.DataFlags.HasThreshold);
			if (cast == ContinuousProperty.Cast.ParticleSystem)
			{
				this.particleMain = ((ParticleSystem)this.target).main;
				this.particleEmission = ((ParticleSystem)this.target).emission;
				this.speedCurveCache = this.particleMain.startSpeed;
				this.rateCurveCache = this.particleEmission.rateOverTime;
			}
			if (this.IsShaderProperty_Cached)
			{
				this.stringHash = Shader.PropertyToID(this.stringValue);
			}
			else if (ContinuousProperty.HasAllFlags(dataFlags, ContinuousProperty.DataFlags.IsAnimatorParameter))
			{
				this.stringHash = Animator.StringToHash(this.stringValue);
			}
			if (!ContinuousProperty.HasAnyFlag(dataFlags, ContinuousProperty.DataFlags.HasCurve))
			{
				this.curve = AnimationCurves.Linear;
			}
		}

		// Token: 0x06007A65 RID: 31333 RVA: 0x0027E547 File Offset: 0x0027C747
		public void InitThreshold()
		{
			if (!this.UsesThreshold_Cached)
			{
				return;
			}
			this.CheckThreshold(0f);
			if (this.IsShaderProperty_Cached)
			{
				return;
			}
			this.previousBoolValue = !this.previousBoolValue;
			this.Apply(0f, 0f, null);
		}

		// Token: 0x06007A66 RID: 31334 RVA: 0x0027E588 File Offset: 0x0027C788
		public void Apply(float f, float deltaTime, MaterialPropertyBlock mpb)
		{
			if (this.runOnlyLocally && !this.rigLocal)
			{
				return;
			}
			int num = this.internalSwitchValue | (int)this.CheckThreshold(f);
			if (num <= 1057808)
			{
				if (num <= 6157)
				{
					if (num <= 3083)
					{
						if (num <= 2049)
						{
							if (num == 0)
							{
								return;
							}
							if (num != 2049)
							{
								return;
							}
							((Transform)this.target).localScale = this.curve.Evaluate(f) * Vector3.one;
							return;
						}
						else
						{
							if (num == 3072)
							{
								this.particleMain.startColor = this.color.Evaluate(f);
								return;
							}
							if (num == 3073)
							{
								this.particleMain.startSize = this.curve.Evaluate(f);
								return;
							}
							if (num != 3083)
							{
								return;
							}
							this.particleMain.startSpeed = this.ScaleCurve(in this.speedCurveCache, this.curve.Evaluate(f));
							return;
						}
					}
					else if (num <= 4098)
					{
						if (num == 3084)
						{
							this.particleEmission.rateOverTime = this.ScaleCurve(in this.rateCurveCache, this.curve.Evaluate(f));
							return;
						}
						if (num != 4098)
						{
							return;
						}
						((SkinnedMeshRenderer)this.target).SetBlendShapeWeight(this.intValue, this.curve.Evaluate(f) * 100f);
						return;
					}
					else
					{
						if (num == 5123)
						{
							((Animator)this.target).SetFloat(this.stringHash, this.curve.Evaluate(f));
							return;
						}
						if (num == 5131)
						{
							((Animator)this.target).speed = this.curve.Evaluate(f);
							return;
						}
						if (num != 6157)
						{
							return;
						}
						((AudioSource)this.target).volume = Mathf.Clamp01(this.curve.Evaluate(f));
						return;
					}
				}
				else if (num <= 1051663)
				{
					if (num <= 7173)
					{
						if (num == 6158)
						{
							((AudioSource)this.target).pitch = Mathf.Clamp(this.curve.Evaluate(f), -3f, 3f);
							return;
						}
						switch (num)
						{
						case 7171:
							mpb.SetFloat(this.stringHash, this.curve.Evaluate(f));
							return;
						case 7172:
							mpb.SetVector(this.stringHash, new Vector2(this.curve.Evaluate(f), 0f));
							return;
						case 7173:
							mpb.SetColor(this.stringHash, this.color.Evaluate(f));
							return;
						default:
							return;
						}
					}
					else
					{
						if (num == 11278)
						{
							((VoiceShiftCosmetic)this.target).Pitch = this.curve.Evaluate(f);
							return;
						}
						if (num == 1049617)
						{
							this.unityEvent.Invoke(this.curve.Evaluate(f));
							return;
						}
						if (num != 1051663)
						{
							return;
						}
						((ParticleSystem)this.target).Play();
						return;
					}
				}
				else if (num <= 1054735)
				{
					if (num != 1053706)
					{
						if (num == 1053714)
						{
							((Animator)this.target).SetTrigger(this.stringHash);
							return;
						}
						if (num != 1054735)
						{
							return;
						}
						((AudioSource)this.target).Play();
						return;
					}
				}
				else
				{
					if (num == 1055760)
					{
						goto IL_07AB;
					}
					if (num == 1056784)
					{
						goto IL_07C2;
					}
					if (num != 1057808)
					{
						return;
					}
					goto IL_07D9;
				}
			}
			else if (num <= 3150858)
			{
				if (num <= 2103311)
				{
					if (num <= 2100239)
					{
						if (num == 2098193)
						{
							return;
						}
						if (num != 2100239)
						{
							return;
						}
						((ParticleSystem)this.target).Stop(true, this.stopType);
						return;
					}
					else if (num != 2102282)
					{
						if (num == 2102290)
						{
							return;
						}
						if (num != 2103311)
						{
							return;
						}
						((AudioSource)this.target).Stop();
						return;
					}
				}
				else if (num <= 2106384)
				{
					if (num == 2104336)
					{
						goto IL_07AB;
					}
					if (num == 2105360)
					{
						goto IL_07C2;
					}
					if (num != 2106384)
					{
						return;
					}
					goto IL_07D9;
				}
				else
				{
					if (num != 3146769 && num != 3148815)
					{
						return;
					}
					return;
				}
			}
			else if (num <= 3154960)
			{
				if (num <= 3151887)
				{
					if (num != 3150866)
					{
						return;
					}
					return;
				}
				else
				{
					if (num != 3152912 && num != 3153936)
					{
						return;
					}
					return;
				}
			}
			else if (num <= 8389649)
			{
				if (num == 4195345)
				{
					this.unityEvent.Invoke(this.curve.Evaluate(f));
					return;
				}
				switch (num)
				{
				case 4196358:
					((Transform)this.target).position = this.bezierCurve.GetPoint(this.curve.Evaluate(f));
					return;
				case 4196359:
					((Transform)this.target).localRotation = Quaternion.Euler(this.curve.Evaluate(f) * 360f, 0f, 0f);
					return;
				case 4196360:
					((Transform)this.target).position = Vector3.Lerp(this.transformA.position, this.transformB.position, this.curve.Evaluate(f));
					return;
				case 4196361:
					((Transform)this.target).localPosition = Vector3.Lerp(this.offsetA.pos, this.offsetB.pos, this.curve.Evaluate(f));
					return;
				default:
				{
					if (num != 8389649)
					{
						return;
					}
					float num2 = this.curve.Evaluate(f);
					float num3 = 1f / num2;
					this.frequencyTimer += deltaTime;
					if (this.frequencyTimer >= num3)
					{
						this.frequencyTimer = Mathf.Repeat(this.frequencyTimer - num3, num3);
						this.unityEvent.Invoke(num2);
						return;
					}
					return;
				}
				}
			}
			else
			{
				switch (num)
				{
				case 8390662:
					((Transform)this.target).rotation = Quaternion.LookRotation(this.bezierCurve.GetDirection(this.curve.Evaluate(f)));
					return;
				case 8390663:
					((Transform)this.target).localRotation = Quaternion.Euler(0f, this.curve.Evaluate(f) * 360f, 0f);
					return;
				case 8390664:
					((Transform)this.target).rotation = Quaternion.Slerp(this.transformA.rotation, this.transformB.rotation, this.curve.Evaluate(f));
					return;
				case 8390665:
					((Transform)this.target).localRotation = Quaternion.Slerp(this.offsetA.rot, this.offsetB.rot, this.curve.Evaluate(f));
					return;
				default:
					if (num != 12583953)
					{
						switch (num)
						{
						case 12584966:
						{
							float num4 = this.curve.Evaluate(f);
							((Transform)this.target).SetPositionAndRotation(this.bezierCurve.GetPoint(num4), Quaternion.LookRotation(this.bezierCurve.GetDirection(num4)));
							return;
						}
						case 12584967:
							((Transform)this.target).localRotation = Quaternion.Euler(0f, 0f, this.curve.Evaluate(f) * 360f);
							return;
						case 12584968:
						{
							Vector3 vector;
							Quaternion quaternion;
							this.transformA.GetPositionAndRotation(out vector, out quaternion);
							Vector3 vector2;
							Quaternion quaternion2;
							this.transformB.GetPositionAndRotation(out vector2, out quaternion2);
							float num5 = this.curve.Evaluate(f);
							((Transform)this.target).SetPositionAndRotation(Vector3.Lerp(vector, vector2, num5), Quaternion.Slerp(quaternion, quaternion2, num5));
							return;
						}
						case 12584969:
						{
							float num6 = this.curve.Evaluate(f);
							((Transform)this.target).SetLocalPositionAndRotation(Vector3.Lerp(this.offsetA.pos, this.offsetB.pos, num6), Quaternion.Slerp(this.offsetA.rot, this.offsetB.rot, num6));
							return;
						}
						default:
							return;
						}
					}
					else
					{
						float num7 = this.curve.Evaluate(f);
						float num8 = 1f - Mathf.Exp(-num7 * deltaTime);
						if (Random.value < num8)
						{
							this.unityEvent.Invoke(num7);
							return;
						}
						return;
					}
					break;
				}
			}
			((Animator)this.target).SetBool(this.stringHash, this.previousBoolValue);
			return;
			IL_07AB:
			((Renderer)this.target).enabled = this.previousBoolValue;
			return;
			IL_07C2:
			((Behaviour)this.target).enabled = this.previousBoolValue;
			return;
			IL_07D9:
			((GameObject)this.target).SetActive(this.previousBoolValue);
		}

		// Token: 0x06007A67 RID: 31335 RVA: 0x0027EE5C File Offset: 0x0027D05C
		private ParticleSystem.MinMaxCurve ScaleCurve(in ParticleSystem.MinMaxCurve inCurve, float scale)
		{
			ParticleSystem.MinMaxCurve minMaxCurve = inCurve;
			switch (minMaxCurve.mode)
			{
			case ParticleSystemCurveMode.Constant:
				minMaxCurve.constant *= scale;
				break;
			case ParticleSystemCurveMode.Curve:
			case ParticleSystemCurveMode.TwoCurves:
				minMaxCurve.curveMultiplier *= scale;
				break;
			case ParticleSystemCurveMode.TwoConstants:
				minMaxCurve.constantMin *= scale;
				minMaxCurve.constantMax *= scale;
				break;
			}
			return minMaxCurve;
		}

		// Token: 0x06007A68 RID: 31336 RVA: 0x0027EED4 File Offset: 0x0027D0D4
		private bool CheckContinuousEvent(float f, float deltaTime)
		{
			ContinuousProperty.EventMode eventMode = this.eventMode;
			if (eventMode == ContinuousProperty.EventMode.Passthrough)
			{
				return true;
			}
			if (eventMode != ContinuousProperty.EventMode.Frequency)
			{
				if (eventMode != ContinuousProperty.EventMode.AveragePerSecond)
				{
					return false;
				}
				float num = 1f - Mathf.Exp(-f * deltaTime);
				return Random.value < num;
			}
			else
			{
				this.frequencyTimer += deltaTime;
				if (this.frequencyTimer < f)
				{
					return false;
				}
				this.frequencyTimer = Mathf.Repeat(this.frequencyTimer - f, f);
				return true;
			}
		}

		// Token: 0x06007A69 RID: 31337 RVA: 0x0027EF50 File Offset: 0x0027D150
		private ContinuousProperty.ThresholdResult CheckThreshold(float f)
		{
			if (!this.UsesThreshold_Cached)
			{
				return ContinuousProperty.ThresholdResult.Null;
			}
			bool flag = f >= this.range.x && f <= this.range.y;
			if (!this.previousBoolValue && ((this.thresholdOption == ContinuousProperty.ThresholdOption.Normal && flag) || (this.thresholdOption == ContinuousProperty.ThresholdOption.Invert && !flag)))
			{
				this.previousBoolValue = true;
				return ContinuousProperty.ThresholdResult.RisingEdge;
			}
			if (this.previousBoolValue && ((this.thresholdOption == ContinuousProperty.ThresholdOption.Normal && !flag) || (this.thresholdOption == ContinuousProperty.ThresholdOption.Invert && flag)))
			{
				this.previousBoolValue = false;
				return ContinuousProperty.ThresholdResult.FallingEdge;
			}
			return ContinuousProperty.ThresholdResult.Unchanged;
		}

		// Token: 0x04008B98 RID: 35736
		[SerializeField]
		private ContinuousPropertyModeSO mode;

		// Token: 0x04008B99 RID: 35737
		[FormerlySerializedAs("component")]
		[SerializeField]
		protected Object target;

		// Token: 0x04008B9C RID: 35740
		[SerializeField]
		private Gradient color;

		// Token: 0x04008B9D RID: 35741
		[SerializeField]
		private AnimationCurve curve = AnimationCurves.Linear;

		// Token: 0x04008B9E RID: 35742
		[FormerlySerializedAs("materialIndex")]
		[SerializeField]
		private int intValue;

		// Token: 0x04008B9F RID: 35743
		[SerializeField]
		private string stringValue;

		// Token: 0x04008BA0 RID: 35744
		[SerializeField]
		private BezierCurve bezierCurve;

		// Token: 0x04008BA1 RID: 35745
		private const string ENUM_ERROR = "Internal values were changed at some point. Please select a new value.";

		// Token: 0x04008BA2 RID: 35746
		[SerializeField]
		private ContinuousProperty.RotationAxis localAxis = ContinuousProperty.RotationAxis.X;

		// Token: 0x04008BA3 RID: 35747
		[SerializeField]
		private ContinuousProperty.InterpolationMode interpolationMode = ContinuousProperty.InterpolationMode.PositionAndRotation;

		// Token: 0x04008BA4 RID: 35748
		[SerializeField]
		private ParticleSystemStopBehavior stopType = ParticleSystemStopBehavior.StopEmitting;

		// Token: 0x04008BA5 RID: 35749
		[SerializeField]
		private Transform transformA;

		// Token: 0x04008BA6 RID: 35750
		[SerializeField]
		private Transform transformB;

		// Token: 0x04008BA7 RID: 35751
		[SerializeField]
		private XformOffset offsetA;

		// Token: 0x04008BA8 RID: 35752
		[SerializeField]
		private XformOffset offsetB;

		// Token: 0x04008BA9 RID: 35753
		[SerializeField]
		private Vector2 range = new Vector2(0.5f, 1f);

		// Token: 0x04008BAA RID: 35754
		[SerializeField]
		private ContinuousProperty.ThresholdOption thresholdOption = ContinuousProperty.ThresholdOption.Normal;

		// Token: 0x04008BAB RID: 35755
		[SerializeField]
		private ContinuousProperty.EventMode eventMode = ContinuousProperty.EventMode.Passthrough;

		// Token: 0x04008BAC RID: 35756
		[SerializeField]
		private UnityEvent<float> unityEvent;

		// Token: 0x04008BAD RID: 35757
		[Tooltip("Check this box if only the owner/local player is supposed to run this property.")]
		[SerializeField]
		private bool runOnlyLocally;

		// Token: 0x04008BAE RID: 35758
		private bool rigLocal;

		// Token: 0x04008BAF RID: 35759
		private int internalSwitchValue;

		// Token: 0x04008BB0 RID: 35760
		private ParticleSystem.MainModule particleMain;

		// Token: 0x04008BB1 RID: 35761
		private ParticleSystem.EmissionModule particleEmission;

		// Token: 0x04008BB2 RID: 35762
		private ParticleSystem.MinMaxCurve speedCurveCache;

		// Token: 0x04008BB3 RID: 35763
		private ParticleSystem.MinMaxCurve rateCurveCache;

		// Token: 0x04008BB4 RID: 35764
		private float frequencyTimer;

		// Token: 0x04008BB5 RID: 35765
		private bool previousBoolValue;

		// Token: 0x04008BB6 RID: 35766
		private int stringHash;

		// Token: 0x02001305 RID: 4869
		public enum Type
		{
			// Token: 0x04008BB8 RID: 35768
			Color,
			// Token: 0x04008BB9 RID: 35769
			Scale,
			// Token: 0x04008BBA RID: 35770
			BlendShape,
			// Token: 0x04008BBB RID: 35771
			Float,
			// Token: 0x04008BBC RID: 35772
			ShaderVector2_X,
			// Token: 0x04008BBD RID: 35773
			ShaderColor,
			// Token: 0x04008BBE RID: 35774
			BezierInterpolation,
			// Token: 0x04008BBF RID: 35775
			AxisAngle,
			// Token: 0x04008BC0 RID: 35776
			TransformInterpolation,
			// Token: 0x04008BC1 RID: 35777
			OffsetInterpolation,
			// Token: 0x04008BC2 RID: 35778
			Boolean,
			// Token: 0x04008BC3 RID: 35779
			Speed,
			// Token: 0x04008BC4 RID: 35780
			Rate,
			// Token: 0x04008BC5 RID: 35781
			Volume,
			// Token: 0x04008BC6 RID: 35782
			Pitch,
			// Token: 0x04008BC7 RID: 35783
			PlayStop,
			// Token: 0x04008BC8 RID: 35784
			EnableDisable,
			// Token: 0x04008BC9 RID: 35785
			UnityEvent,
			// Token: 0x04008BCA RID: 35786
			Trigger
		}

		// Token: 0x02001306 RID: 4870
		public enum Cast
		{
			// Token: 0x04008BCC RID: 35788
			Null,
			// Token: 0x04008BCD RID: 35789
			Any = 1024,
			// Token: 0x04008BCE RID: 35790
			Transform = 2048,
			// Token: 0x04008BCF RID: 35791
			ParticleSystem = 3072,
			// Token: 0x04008BD0 RID: 35792
			SkinnedMeshRenderer = 4096,
			// Token: 0x04008BD1 RID: 35793
			Animator = 5120,
			// Token: 0x04008BD2 RID: 35794
			AudioSource = 6144,
			// Token: 0x04008BD3 RID: 35795
			Renderer = 7168,
			// Token: 0x04008BD4 RID: 35796
			Behaviour = 8192,
			// Token: 0x04008BD5 RID: 35797
			GameObject = 9216,
			// Token: 0x04008BD6 RID: 35798
			Rigidbody = 10240,
			// Token: 0x04008BD7 RID: 35799
			VoicePitchShiftCosmetic = 11264
		}

		// Token: 0x02001307 RID: 4871
		[Flags]
		public enum DataFlags
		{
			// Token: 0x04008BD9 RID: 35801
			None = 0,
			// Token: 0x04008BDA RID: 35802
			[Tooltip("Expose the AnimationCurve for single values")]
			HasCurve = 1,
			// Token: 0x04008BDB RID: 35803
			[Tooltip("Expose the Gradient for colors")]
			HasColor = 2,
			// Token: 0x04008BDC RID: 35804
			[Tooltip("Select which axis it should rotate on")]
			HasAxis = 4,
			// Token: 0x04008BDD RID: 35805
			[Tooltip("Expose the integer, usually for material index")]
			HasInteger = 8,
			// Token: 0x04008BDE RID: 35806
			[Tooltip("Select whether to use position, rotation, or both when interpolating")]
			HasInterpolation = 16,
			// Token: 0x04008BDF RID: 35807
			[Tooltip("Expose the string and hash it into a shader property ID")]
			IsShaderProperty = 32,
			// Token: 0x04008BE0 RID: 35808
			[Tooltip("Expose the string and hash it into an animator parameter ID")]
			IsAnimatorParameter = 64,
			// Token: 0x04008BE1 RID: 35809
			[Tooltip("Expose the threshold range as a dual slider")]
			HasThreshold = 128
		}

		// Token: 0x02001308 RID: 4872
		private enum ThresholdResult
		{
			// Token: 0x04008BE3 RID: 35811
			Null,
			// Token: 0x04008BE4 RID: 35812
			RisingEdge = 1048576,
			// Token: 0x04008BE5 RID: 35813
			FallingEdge = 2097152,
			// Token: 0x04008BE6 RID: 35814
			Unchanged = 3145728
		}

		// Token: 0x02001309 RID: 4873
		private enum ThresholdOption
		{
			// Token: 0x04008BE8 RID: 35816
			Invert,
			// Token: 0x04008BE9 RID: 35817
			Normal
		}

		// Token: 0x0200130A RID: 4874
		private enum RotationAxis
		{
			// Token: 0x04008BEB RID: 35819
			X = 4194304,
			// Token: 0x04008BEC RID: 35820
			Y = 8388608,
			// Token: 0x04008BED RID: 35821
			Z = 12582912
		}

		// Token: 0x0200130B RID: 4875
		public enum InterpolationMode
		{
			// Token: 0x04008BEF RID: 35823
			Position = 4194304,
			// Token: 0x04008BF0 RID: 35824
			Rotation = 8388608,
			// Token: 0x04008BF1 RID: 35825
			PositionAndRotation = 12582912
		}

		// Token: 0x0200130C RID: 4876
		public enum EventMode
		{
			// Token: 0x04008BF3 RID: 35827
			Passthrough = 4194304,
			// Token: 0x04008BF4 RID: 35828
			Frequency = 8388608,
			// Token: 0x04008BF5 RID: 35829
			AveragePerSecond = 12582912
		}
	}
}
