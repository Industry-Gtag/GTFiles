using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000D43 RID: 3395
internal static class ProjectileTracker
{
	// Token: 0x0600541E RID: 21534 RVA: 0x001BB724 File Offset: 0x001B9924
	static ProjectileTracker()
	{
		RoomSystem.LeftRoomEvent += new Action(ProjectileTracker.ClearProjectiles);
		RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(ProjectileTracker.RemovePlayerProjectiles);
	}

	// Token: 0x0600541F RID: 21535 RVA: 0x001BB790 File Offset: 0x001B9990
	public static void RemovePlayerProjectiles(NetPlayer player)
	{
		LoopingArray<ProjectileTracker.ProjectileInfo> loopingArray;
		if (ProjectileTracker.m_playerProjectiles.TryGetValue(player, out loopingArray))
		{
			ProjectileTracker.ResetPlayerProjectiles(loopingArray);
			ProjectileTracker.m_playerProjectiles.Remove(player);
			ProjectileTracker.m_projectileInfoPool.Return(loopingArray);
		}
	}

	// Token: 0x06005420 RID: 21536 RVA: 0x001BB7CC File Offset: 0x001B99CC
	private static void ClearProjectiles()
	{
		foreach (LoopingArray<ProjectileTracker.ProjectileInfo> loopingArray in ProjectileTracker.m_playerProjectiles.Values)
		{
			ProjectileTracker.ResetPlayerProjectiles(loopingArray);
			ProjectileTracker.m_projectileInfoPool.Return(loopingArray);
		}
		ProjectileTracker.m_playerProjectiles.Clear();
	}

	// Token: 0x06005421 RID: 21537 RVA: 0x001BB838 File Offset: 0x001B9A38
	private static void ResetPlayerProjectiles(LoopingArray<ProjectileTracker.ProjectileInfo> projectiles)
	{
		for (int i = 0; i < projectiles.Length; i++)
		{
			SlingshotProjectile projectileInstance = projectiles[i].projectileInstance;
			if (!projectileInstance.IsNull() && projectileInstance.projectileOwner != NetworkSystem.Instance.LocalPlayer && projectileInstance.gameObject.activeSelf)
			{
				projectileInstance.Deactivate();
			}
		}
	}

	// Token: 0x06005422 RID: 21538 RVA: 0x001BB890 File Offset: 0x001B9A90
	public static int AddAndIncrementLocalProjectile(SlingshotProjectile projectile, Vector3 intialVelocity, Vector3 initialPosition, float scale)
	{
		SlingshotProjectile projectileInstance = ProjectileTracker.m_localProjectiles[ProjectileTracker.m_localProjectiles.CurrentIndex].projectileInstance;
		if (projectileInstance.IsNotNull() && projectileInstance != projectile && projectileInstance.projectileOwner == NetworkSystem.Instance.LocalPlayer && projectileInstance.gameObject.activeSelf)
		{
			projectileInstance.Deactivate();
		}
		ProjectileTracker.ProjectileInfo projectileInfo = new ProjectileTracker.ProjectileInfo(PhotonNetwork.Time, intialVelocity, initialPosition, scale, projectile);
		return ProjectileTracker.m_localProjectiles.AddAndIncrement(in projectileInfo);
	}

	// Token: 0x06005423 RID: 21539 RVA: 0x001BB90C File Offset: 0x001B9B0C
	public static void AddRemotePlayerProjectile(NetPlayer player, SlingshotProjectile projectile, int projectileIndex, double timeShot, Vector3 intialVelocity, Vector3 initialPosition, float scale)
	{
		LoopingArray<ProjectileTracker.ProjectileInfo> loopingArray;
		if (!ProjectileTracker.m_playerProjectiles.ContainsKey(player))
		{
			loopingArray = ProjectileTracker.m_projectileInfoPool.Take();
			ProjectileTracker.m_playerProjectiles[player] = loopingArray;
		}
		else
		{
			loopingArray = ProjectileTracker.m_playerProjectiles[player];
		}
		if (projectileIndex < 0 || projectileIndex >= loopingArray.Length)
		{
			MonkeAgent.instance.SendReport("invlProj", player.UserId, player.NickName);
			return;
		}
		SlingshotProjectile projectileInstance = loopingArray[projectileIndex].projectileInstance;
		if (projectileInstance.IsNotNull() && projectileInstance.projectileOwner == player && projectileInstance.gameObject.activeSelf)
		{
			projectileInstance.Deactivate();
		}
		ProjectileTracker.ProjectileInfo projectileInfo = new ProjectileTracker.ProjectileInfo(timeShot, intialVelocity, initialPosition, scale, projectile);
		loopingArray[projectileIndex] = projectileInfo;
	}

	// Token: 0x06005424 RID: 21540 RVA: 0x001BB9BE File Offset: 0x001B9BBE
	public static ProjectileTracker.ProjectileInfo GetLocalProjectile(int index)
	{
		return ProjectileTracker.m_localProjectiles[index];
	}

	// Token: 0x06005425 RID: 21541 RVA: 0x001BB9CC File Offset: 0x001B9BCC
	public static ValueTuple<bool, ProjectileTracker.ProjectileInfo> GetAndRemoveRemotePlayerProjectile(NetPlayer player, int index)
	{
		ValueTuple<bool, ProjectileTracker.ProjectileInfo> valueTuple = new ValueTuple<bool, ProjectileTracker.ProjectileInfo>(false, default(ProjectileTracker.ProjectileInfo));
		LoopingArray<ProjectileTracker.ProjectileInfo> loopingArray;
		if (index < 0 || index >= ProjectileTracker.m_localProjectiles.Length || !ProjectileTracker.m_playerProjectiles.TryGetValue(player, out loopingArray))
		{
			return valueTuple;
		}
		ProjectileTracker.ProjectileInfo projectileInfo = loopingArray[index];
		if (projectileInfo.projectileInstance.IsNotNull())
		{
			valueTuple.Item1 = true;
			valueTuple.Item2 = projectileInfo;
			loopingArray[index] = default(ProjectileTracker.ProjectileInfo);
		}
		return valueTuple;
	}

	// Token: 0x040065B0 RID: 26032
	private static LoopingArray<ProjectileTracker.ProjectileInfo>.Pool m_projectileInfoPool = new LoopingArray<ProjectileTracker.ProjectileInfo>.Pool(50, 9);

	// Token: 0x040065B1 RID: 26033
	private static LoopingArray<ProjectileTracker.ProjectileInfo> m_localProjectiles = new LoopingArray<ProjectileTracker.ProjectileInfo>(50);

	// Token: 0x040065B2 RID: 26034
	public static readonly Dictionary<NetPlayer, LoopingArray<ProjectileTracker.ProjectileInfo>> m_playerProjectiles = new Dictionary<NetPlayer, LoopingArray<ProjectileTracker.ProjectileInfo>>(9);

	// Token: 0x02000D44 RID: 3396
	public struct ProjectileInfo
	{
		// Token: 0x06005426 RID: 21542 RVA: 0x001BBA42 File Offset: 0x001B9C42
		public ProjectileInfo(double newTime, Vector3 newVel, Vector3 origin, float newScale, SlingshotProjectile projectile)
		{
			this.timeLaunched = newTime;
			this.shotVelocity = newVel;
			this.launchOrigin = origin;
			this.scale = newScale;
			this.projectileInstance = projectile;
			this.hasImpactOverride = projectile.playerImpactEffectPrefab.IsNotNull();
		}

		// Token: 0x040065B3 RID: 26035
		public double timeLaunched;

		// Token: 0x040065B4 RID: 26036
		public Vector3 shotVelocity;

		// Token: 0x040065B5 RID: 26037
		public Vector3 launchOrigin;

		// Token: 0x040065B6 RID: 26038
		public float scale;

		// Token: 0x040065B7 RID: 26039
		public SlingshotProjectile projectileInstance;

		// Token: 0x040065B8 RID: 26040
		public bool hasImpactOverride;
	}
}
