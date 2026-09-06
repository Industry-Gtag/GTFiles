using System;
using GorillaNetworking;
using KID.Model;
using UnityEngine;

// Token: 0x02000B99 RID: 2969
internal class UGCPermissionManager : MonoBehaviour
{
	// Token: 0x06004AFD RID: 19197 RVA: 0x00191086 File Offset: 0x0018F286
	public static void UsePlayFabSafety()
	{
		UGCPermissionManager.permissions = new UGCPermissionManager.PlayFabPermissions(new Action<UGCAccessLevel>(UGCPermissionManager.SetAccessLevel));
		UGCPermissionManager.permissions.Initialize();
	}

	// Token: 0x06004AFE RID: 19198 RVA: 0x001910A8 File Offset: 0x0018F2A8
	public static void UseKID()
	{
		UGCPermissionManager.permissions = new UGCPermissionManager.KIDPermissions(new Action<UGCAccessLevel>(UGCPermissionManager.SetAccessLevel));
		UGCPermissionManager.permissions.Initialize();
	}

	// Token: 0x17000737 RID: 1847
	// (get) Token: 0x06004AFF RID: 19199 RVA: 0x001910CA File Offset: 0x0018F2CA
	public static bool IsUGCDisabled
	{
		get
		{
			return UGCPermissionManager.accessLevel.GetValueOrDefault() != UGCAccessLevel.Full;
		}
	}

	// Token: 0x17000738 RID: 1848
	// (get) Token: 0x06004B00 RID: 19200 RVA: 0x001910DC File Offset: 0x0018F2DC
	public static bool FeaturedMapsOnly
	{
		get
		{
			return UGCPermissionManager.accessLevel.GetValueOrDefault() == UGCAccessLevel.FeaturedMapsOnly;
		}
	}

	// Token: 0x17000739 RID: 1849
	// (get) Token: 0x06004B01 RID: 19201 RVA: 0x001910EB File Offset: 0x0018F2EB
	public static bool HasNoMapAccess
	{
		get
		{
			return UGCPermissionManager.accessLevel.GetValueOrDefault() == UGCAccessLevel.Disabled;
		}
	}

	// Token: 0x06004B02 RID: 19202 RVA: 0x001910FA File Offset: 0x0018F2FA
	public static void CheckPermissions()
	{
		UGCPermissionManager.IUGCPermissions iugcpermissions = UGCPermissionManager.permissions;
		if (iugcpermissions == null)
		{
			return;
		}
		iugcpermissions.CheckPermissions();
	}

	// Token: 0x06004B03 RID: 19203 RVA: 0x0019110B File Offset: 0x0018F30B
	public static void SubscribeToUGCEnabled(Action callback)
	{
		UGCPermissionManager.onUGCEnabled = (Action)Delegate.Combine(UGCPermissionManager.onUGCEnabled, callback);
	}

	// Token: 0x06004B04 RID: 19204 RVA: 0x00191122 File Offset: 0x0018F322
	public static void UnsubscribeFromUGCEnabled(Action callback)
	{
		UGCPermissionManager.onUGCEnabled = (Action)Delegate.Remove(UGCPermissionManager.onUGCEnabled, callback);
	}

	// Token: 0x06004B05 RID: 19205 RVA: 0x00191139 File Offset: 0x0018F339
	public static void SubscribeToUGCDisabled(Action callback)
	{
		UGCPermissionManager.onUGCDisabled = (Action)Delegate.Combine(UGCPermissionManager.onUGCDisabled, callback);
	}

	// Token: 0x06004B06 RID: 19206 RVA: 0x00191150 File Offset: 0x0018F350
	public static void UnsubscribeFromUGCDisabled(Action callback)
	{
		UGCPermissionManager.onUGCDisabled = (Action)Delegate.Remove(UGCPermissionManager.onUGCDisabled, callback);
	}

	// Token: 0x06004B07 RID: 19207 RVA: 0x00191167 File Offset: 0x0018F367
	public static void SubscribeToVirtualStumpEnabled(Action callback)
	{
		UGCPermissionManager.onVirtualStumpEnabled = (Action)Delegate.Combine(UGCPermissionManager.onVirtualStumpEnabled, callback);
	}

	// Token: 0x06004B08 RID: 19208 RVA: 0x0019117E File Offset: 0x0018F37E
	public static void UnsubscribeFromVirtualStumpEnabled(Action callback)
	{
		UGCPermissionManager.onVirtualStumpEnabled = (Action)Delegate.Remove(UGCPermissionManager.onVirtualStumpEnabled, callback);
	}

	// Token: 0x06004B09 RID: 19209 RVA: 0x00191195 File Offset: 0x0018F395
	public static void SubscribeToVirtualStumpDisabled(Action callback)
	{
		UGCPermissionManager.onVirtualStumpDisabled = (Action)Delegate.Combine(UGCPermissionManager.onVirtualStumpDisabled, callback);
	}

	// Token: 0x06004B0A RID: 19210 RVA: 0x001911AC File Offset: 0x0018F3AC
	public static void UnsubscribeFromVirtualStumpDisabled(Action callback)
	{
		UGCPermissionManager.onVirtualStumpDisabled = (Action)Delegate.Remove(UGCPermissionManager.onVirtualStumpDisabled, callback);
	}

	// Token: 0x06004B0B RID: 19211 RVA: 0x001911C4 File Offset: 0x0018F3C4
	private static void SetAccessLevel(UGCAccessLevel level)
	{
		UGCAccessLevel? ugcaccessLevel = UGCPermissionManager.accessLevel;
		if ((level == ugcaccessLevel.GetValueOrDefault()) & (ugcaccessLevel != null))
		{
			return;
		}
		bool flag = UGCPermissionManager.accessLevel != null;
		ugcaccessLevel = UGCPermissionManager.accessLevel;
		UGCAccessLevel ugcaccessLevel2 = UGCAccessLevel.Full;
		bool flag2 = (ugcaccessLevel.GetValueOrDefault() == ugcaccessLevel2) & (ugcaccessLevel != null);
		bool flag3;
		if (UGCPermissionManager.accessLevel != null)
		{
			ugcaccessLevel = UGCPermissionManager.accessLevel;
			ugcaccessLevel2 = UGCAccessLevel.Disabled;
			flag3 = !((ugcaccessLevel.GetValueOrDefault() == ugcaccessLevel2) & (ugcaccessLevel != null));
		}
		else
		{
			flag3 = false;
		}
		bool flag4 = flag3;
		UGCPermissionManager.accessLevel = new UGCAccessLevel?(level);
		bool flag5 = level == UGCAccessLevel.Full;
		bool flag6 = level > UGCAccessLevel.Disabled;
		if (!flag || flag2 != flag5)
		{
			if (flag5)
			{
				Action action = UGCPermissionManager.onUGCEnabled;
				if (action != null)
				{
					action();
				}
			}
			else
			{
				Action action2 = UGCPermissionManager.onUGCDisabled;
				if (action2 != null)
				{
					action2();
				}
			}
		}
		if (!flag || flag4 != flag6)
		{
			if (flag6)
			{
				Action action3 = UGCPermissionManager.onVirtualStumpEnabled;
				if (action3 == null)
				{
					return;
				}
				action3();
				return;
			}
			else
			{
				Action action4 = UGCPermissionManager.onVirtualStumpDisabled;
				if (action4 == null)
				{
					return;
				}
				action4();
			}
		}
	}

	// Token: 0x04005DEB RID: 24043
	[OnEnterPlay_SetNull]
	private static UGCPermissionManager.IUGCPermissions permissions;

	// Token: 0x04005DEC RID: 24044
	[OnEnterPlay_SetNull]
	private static Action onUGCEnabled;

	// Token: 0x04005DED RID: 24045
	[OnEnterPlay_SetNull]
	private static Action onUGCDisabled;

	// Token: 0x04005DEE RID: 24046
	[OnEnterPlay_SetNull]
	private static Action onVirtualStumpEnabled;

	// Token: 0x04005DEF RID: 24047
	[OnEnterPlay_SetNull]
	private static Action onVirtualStumpDisabled;

	// Token: 0x04005DF0 RID: 24048
	private static UGCAccessLevel? accessLevel;

	// Token: 0x02000B9A RID: 2970
	private interface IUGCPermissions
	{
		// Token: 0x06004B0D RID: 19213
		void Initialize();

		// Token: 0x06004B0E RID: 19214
		void CheckPermissions();
	}

	// Token: 0x02000B9B RID: 2971
	private class PlayFabPermissions : UGCPermissionManager.IUGCPermissions
	{
		// Token: 0x06004B0F RID: 19215 RVA: 0x001912AE File Offset: 0x0018F4AE
		public PlayFabPermissions(Action<UGCAccessLevel> setAccessLevel)
		{
			this.setAccessLevel = setAccessLevel;
		}

		// Token: 0x06004B10 RID: 19216 RVA: 0x001912C0 File Offset: 0x0018F4C0
		public void Initialize()
		{
			bool safety = PlayFabAuthenticator.instance.GetSafety();
			Action<UGCAccessLevel> action = this.setAccessLevel;
			if (action == null)
			{
				return;
			}
			action(safety ? UGCAccessLevel.Disabled : UGCAccessLevel.Full);
		}

		// Token: 0x06004B11 RID: 19217 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void CheckPermissions()
		{
		}

		// Token: 0x04005DF1 RID: 24049
		private Action<UGCAccessLevel> setAccessLevel;
	}

	// Token: 0x02000B9C RID: 2972
	private class KIDPermissions : UGCPermissionManager.IUGCPermissions
	{
		// Token: 0x06004B12 RID: 19218 RVA: 0x001912F1 File Offset: 0x0018F4F1
		public KIDPermissions(Action<UGCAccessLevel> setAccessLevel)
		{
			this.setAccessLevel = setAccessLevel;
		}

		// Token: 0x06004B13 RID: 19219 RVA: 0x00191300 File Offset: 0x0018F500
		private void SetAccessLevel(UGCAccessLevel level)
		{
			Action<UGCAccessLevel> action = this.setAccessLevel;
			if (action == null)
			{
				return;
			}
			action(level);
		}

		// Token: 0x06004B14 RID: 19220 RVA: 0x00191313 File Offset: 0x0018F513
		public void Initialize()
		{
			Debug.Log("[UGCPermissionManager][KID] Initializing with KID");
			this.CheckPermissions();
			KIDManager.RegisterSessionUpdatedCallback_UGC(new Action<bool, Permission.ManagedByEnum>(this.OnKIDSessionUpdate));
		}

		// Token: 0x06004B15 RID: 19221 RVA: 0x00191338 File Offset: 0x0018F538
		public void CheckPermissions()
		{
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Mods);
			bool item = KIDManager.CheckFeatureOptIn(EKIDFeatures.Mods, null).Item2;
			this.ProcessPermissionKID(item, permissionDataByFeature.Enabled, permissionDataByFeature.ManagedBy);
		}

		// Token: 0x06004B16 RID: 19222 RVA: 0x0019136C File Offset: 0x0018F56C
		private void OnKIDSessionUpdate(bool isEnabled, Permission.ManagedByEnum managedBy)
		{
			Debug.Log("[UGCPermissionManager][KID] KID session update.");
			bool item = KIDManager.CheckFeatureOptIn(EKIDFeatures.Mods, null).Item2;
			this.ProcessPermissionKID(item, isEnabled, managedBy);
		}

		// Token: 0x06004B17 RID: 19223 RVA: 0x0019139C File Offset: 0x0018F59C
		private void ProcessPermissionKID(bool hasOptedIn, bool isEnabled, Permission.ManagedByEnum managedBy)
		{
			Debug.LogFormat("[UGCPermissionManager][KID] Process KID permissions - opted in: [{0}], enabled: [{1}], managedBy: [{2}].", new object[] { hasOptedIn, isEnabled, managedBy });
			if (managedBy == Permission.ManagedByEnum.PROHIBITED)
			{
				Debug.Log("[UGCPermissionManager][KID] KID UGC prohibited.");
				this.SetAccessLevel(UGCAccessLevel.Disabled);
				return;
			}
			if (managedBy != Permission.ManagedByEnum.PLAYER)
			{
				if (managedBy == Permission.ManagedByEnum.GUARDIAN)
				{
					Debug.LogFormat("[UGCPermissionManager][KID] KID UGC managed by guardian. (opted in: [{0}], enabled: [{1}])", new object[] { hasOptedIn, isEnabled });
					this.SetAccessLevel(isEnabled ? UGCAccessLevel.Full : UGCAccessLevel.FeaturedMapsOnly);
				}
				return;
			}
			if (isEnabled)
			{
				Debug.Log("[UGCPermissionManager][KID] KID UGC managed by player and enabled - opting in and enabling UGC.");
				if (!hasOptedIn)
				{
					KIDManager.SetFeatureOptIn(EKIDFeatures.Mods, true);
				}
				this.SetAccessLevel(UGCAccessLevel.Full);
				return;
			}
			Debug.LogFormat("[UGCPermissionManager][KID] KID UGC managed by player and disabled by default - using opt in status. (opted in: [{0}])", new object[] { hasOptedIn });
			this.SetAccessLevel(hasOptedIn ? UGCAccessLevel.Full : UGCAccessLevel.FeaturedMapsOnly);
		}

		// Token: 0x04005DF2 RID: 24050
		private Action<UGCAccessLevel> setAccessLevel;
	}
}
