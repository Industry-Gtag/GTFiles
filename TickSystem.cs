using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaTag;
using UnityEngine;

// Token: 0x02000D55 RID: 3413
[DefaultExecutionOrder(0)]
internal abstract class TickSystem<T> : MonoBehaviour
{
	// Token: 0x06005478 RID: 21624 RVA: 0x001BC367 File Offset: 0x001BA567
	private void Awake()
	{
		base.transform.SetParent(null, true);
		Object.DontDestroyOnLoad(this);
	}

	// Token: 0x06005479 RID: 21625 RVA: 0x001BC37C File Offset: 0x001BA57C
	private void Update()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		TickSystem<T>.preTickCallbacks.TryRunCallbacks();
		TickSystem<T>.tickCallbacks.TryRunCallbacks();
	}

	// Token: 0x0600547A RID: 21626 RVA: 0x001BC39A File Offset: 0x001BA59A
	private void LateUpdate()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		TickSystem<T>.postTickCallbacks.TryRunCallbacks();
	}

	// Token: 0x0600547C RID: 21628 RVA: 0x001BC423 File Offset: 0x001BA623
	private static void OnEnterPlay()
	{
		TickSystem<T>.preTickCallbacks.Clear();
		TickSystem<T>.preTickWrapperTable.Clear();
		TickSystem<T>.tickCallbacks.Clear();
		TickSystem<T>.tickWrapperTable.Clear();
		TickSystem<T>.postTickCallbacks.Clear();
		TickSystem<T>.postTickWrapperTable.Clear();
	}

	// Token: 0x0600547D RID: 21629 RVA: 0x001BC464 File Offset: 0x001BA664
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddPreTickCallback(ITickSystemPre callback)
	{
		if (callback.PreTickRunning)
		{
			return;
		}
		TickSystem<T>.TickCallbackWrapperPre tickCallbackWrapperPre = TickSystem<T>.preTickWrapperPool.Take();
		tickCallbackWrapperPre.target = callback;
		TickSystem<T>.preTickWrapperTable[callback] = tickCallbackWrapperPre;
		TickSystem<T>.preTickCallbacks.Add(in tickCallbackWrapperPre);
		callback.PreTickRunning = true;
	}

	// Token: 0x0600547E RID: 21630 RVA: 0x001BC4AC File Offset: 0x001BA6AC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddTickCallback(ITickSystemTick callback)
	{
		if (callback.TickRunning)
		{
			return;
		}
		TickSystem<T>.TickCallbackWrapperTick tickCallbackWrapperTick = TickSystem<T>.tickWrapperPool.Take();
		tickCallbackWrapperTick.target = callback;
		TickSystem<T>.tickWrapperTable[callback] = tickCallbackWrapperTick;
		TickSystem<T>.tickCallbacks.Add(in tickCallbackWrapperTick);
		callback.TickRunning = true;
	}

	// Token: 0x0600547F RID: 21631 RVA: 0x001BC4F4 File Offset: 0x001BA6F4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddPostTickCallback(ITickSystemPost callback)
	{
		if (callback.PostTickRunning)
		{
			return;
		}
		TickSystem<T>.TickCallbackWrapperPost tickCallbackWrapperPost = TickSystem<T>.postTickWrapperPool.Take();
		tickCallbackWrapperPost.target = callback;
		TickSystem<T>.postTickWrapperTable[callback] = tickCallbackWrapperPost;
		TickSystem<T>.postTickCallbacks.Add(in tickCallbackWrapperPost);
		callback.PostTickRunning = true;
	}

	// Token: 0x06005480 RID: 21632 RVA: 0x001BC53B File Offset: 0x001BA73B
	public static void AddTickSystemCallBack(ITickSystem callback)
	{
		TickSystem<T>.AddPreTickCallback(callback);
		TickSystem<T>.AddTickCallback(callback);
		TickSystem<T>.AddPostTickCallback(callback);
	}

	// Token: 0x06005481 RID: 21633 RVA: 0x001BC550 File Offset: 0x001BA750
	public static void AddCallbackTarget(object target)
	{
		ITickSystem tickSystem = target as ITickSystem;
		if (tickSystem != null)
		{
			TickSystem<T>.AddTickSystemCallBack(tickSystem);
			return;
		}
		ITickSystemPre tickSystemPre = target as ITickSystemPre;
		if (tickSystemPre != null)
		{
			TickSystem<T>.AddPreTickCallback(tickSystemPre);
		}
		ITickSystemTick tickSystemTick = target as ITickSystemTick;
		if (tickSystemTick != null)
		{
			TickSystem<T>.AddTickCallback(tickSystemTick);
		}
		ITickSystemPost tickSystemPost = target as ITickSystemPost;
		if (tickSystemPost != null)
		{
			TickSystem<T>.AddPostTickCallback(tickSystemPost);
		}
	}

	// Token: 0x06005482 RID: 21634 RVA: 0x001BC5A0 File Offset: 0x001BA7A0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemovePreTickCallback(ITickSystemPre callback)
	{
		TickSystem<T>.TickCallbackWrapperPre tickCallbackWrapperPre;
		if (!callback.PreTickRunning || !TickSystem<T>.preTickWrapperTable.TryGetValue(callback, out tickCallbackWrapperPre))
		{
			return;
		}
		TickSystem<T>.preTickCallbacks.Remove(in tickCallbackWrapperPre);
		callback.PreTickRunning = false;
		TickSystem<T>.preTickWrapperPool.Return(tickCallbackWrapperPre);
	}

	// Token: 0x06005483 RID: 21635 RVA: 0x001BC5E4 File Offset: 0x001BA7E4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemoveTickCallback(ITickSystemTick callback)
	{
		TickSystem<T>.TickCallbackWrapperTick tickCallbackWrapperTick;
		if (!callback.TickRunning || !TickSystem<T>.tickWrapperTable.TryGetValue(callback, out tickCallbackWrapperTick))
		{
			return;
		}
		TickSystem<T>.tickCallbacks.Remove(in tickCallbackWrapperTick);
		callback.TickRunning = false;
		TickSystem<T>.tickWrapperPool.Return(tickCallbackWrapperTick);
	}

	// Token: 0x06005484 RID: 21636 RVA: 0x001BC628 File Offset: 0x001BA828
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemovePostTickCallback(ITickSystemPost callback)
	{
		TickSystem<T>.TickCallbackWrapperPost tickCallbackWrapperPost;
		if (!callback.PostTickRunning || !TickSystem<T>.postTickWrapperTable.TryGetValue(callback, out tickCallbackWrapperPost))
		{
			return;
		}
		TickSystem<T>.postTickCallbacks.Remove(in tickCallbackWrapperPost);
		callback.PostTickRunning = false;
		TickSystem<T>.postTickWrapperPool.Return(tickCallbackWrapperPost);
	}

	// Token: 0x06005485 RID: 21637 RVA: 0x001BC66C File Offset: 0x001BA86C
	public static void RemoveTickSystemCallback(ITickSystem callback)
	{
		TickSystem<T>.RemovePreTickCallback(callback);
		TickSystem<T>.RemoveTickCallback(callback);
		TickSystem<T>.RemovePostTickCallback(callback);
	}

	// Token: 0x06005486 RID: 21638 RVA: 0x001BC680 File Offset: 0x001BA880
	public static void RemoveCallbackTarget(object target)
	{
		ITickSystem tickSystem = target as ITickSystem;
		if (tickSystem != null)
		{
			TickSystem<T>.RemoveTickSystemCallback(tickSystem);
			return;
		}
		ITickSystemPre tickSystemPre = target as ITickSystemPre;
		if (tickSystemPre != null)
		{
			TickSystem<T>.RemovePreTickCallback(tickSystemPre);
		}
		ITickSystemTick tickSystemTick = target as ITickSystemTick;
		if (tickSystemTick != null)
		{
			TickSystem<T>.RemoveTickCallback(tickSystemTick);
		}
		ITickSystemPost tickSystemPost = target as ITickSystemPost;
		if (tickSystemPost != null)
		{
			TickSystem<T>.RemovePostTickCallback(tickSystemPost);
		}
	}

	// Token: 0x040065E6 RID: 26086
	private static readonly ObjectPool<TickSystem<T>.TickCallbackWrapperPre> preTickWrapperPool = new ObjectPool<TickSystem<T>.TickCallbackWrapperPre>(100);

	// Token: 0x040065E7 RID: 26087
	private static readonly CallbackContainer<TickSystem<T>.TickCallbackWrapperPre> preTickCallbacks = new CallbackContainer<TickSystem<T>.TickCallbackWrapperPre>();

	// Token: 0x040065E8 RID: 26088
	private static readonly Dictionary<ITickSystemPre, TickSystem<T>.TickCallbackWrapperPre> preTickWrapperTable = new Dictionary<ITickSystemPre, TickSystem<T>.TickCallbackWrapperPre>(100);

	// Token: 0x040065E9 RID: 26089
	private static readonly ObjectPool<TickSystem<T>.TickCallbackWrapperTick> tickWrapperPool = new ObjectPool<TickSystem<T>.TickCallbackWrapperTick>(100);

	// Token: 0x040065EA RID: 26090
	private static readonly CallbackContainer<TickSystem<T>.TickCallbackWrapperTick> tickCallbacks = new CallbackContainer<TickSystem<T>.TickCallbackWrapperTick>();

	// Token: 0x040065EB RID: 26091
	private static readonly Dictionary<ITickSystemTick, TickSystem<T>.TickCallbackWrapperTick> tickWrapperTable = new Dictionary<ITickSystemTick, TickSystem<T>.TickCallbackWrapperTick>(100);

	// Token: 0x040065EC RID: 26092
	private static readonly ObjectPool<TickSystem<T>.TickCallbackWrapperPost> postTickWrapperPool = new ObjectPool<TickSystem<T>.TickCallbackWrapperPost>(100);

	// Token: 0x040065ED RID: 26093
	private static readonly CallbackContainer<TickSystem<T>.TickCallbackWrapperPost> postTickCallbacks = new CallbackContainer<TickSystem<T>.TickCallbackWrapperPost>();

	// Token: 0x040065EE RID: 26094
	private static readonly Dictionary<ITickSystemPost, TickSystem<T>.TickCallbackWrapperPost> postTickWrapperTable = new Dictionary<ITickSystemPost, TickSystem<T>.TickCallbackWrapperPost>(100);

	// Token: 0x02000D56 RID: 3414
	private abstract class TickCallbackWrapper<U> : ObjectPoolEvents, ICallBack where U : class
	{
		// Token: 0x17000811 RID: 2065
		// (get) Token: 0x06005488 RID: 21640 RVA: 0x001BC6CE File Offset: 0x001BA8CE
		// (set) Token: 0x06005489 RID: 21641 RVA: 0x001BC6D6 File Offset: 0x001BA8D6
		public U target
		{
			get
			{
				return this.m_target;
			}
			set
			{
				this.m_target = value;
			}
		}

		// Token: 0x0600548A RID: 21642
		public abstract void CallBack();

		// Token: 0x0600548B RID: 21643 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnTaken()
		{
		}

		// Token: 0x0600548C RID: 21644 RVA: 0x001BC6E0 File Offset: 0x001BA8E0
		public void OnReturned()
		{
			this.target = default(U);
		}

		// Token: 0x040065EF RID: 26095
		protected U m_target;
	}

	// Token: 0x02000D57 RID: 3415
	private class TickCallbackWrapperPre : TickSystem<T>.TickCallbackWrapper<ITickSystemPre>
	{
		// Token: 0x0600548E RID: 21646 RVA: 0x001BC6FC File Offset: 0x001BA8FC
		public override void CallBack()
		{
			this.m_target.PreTick();
		}
	}

	// Token: 0x02000D58 RID: 3416
	private class TickCallbackWrapperTick : TickSystem<T>.TickCallbackWrapper<ITickSystemTick>
	{
		// Token: 0x06005490 RID: 21648 RVA: 0x001BC711 File Offset: 0x001BA911
		public override void CallBack()
		{
			this.m_target.Tick();
		}
	}

	// Token: 0x02000D59 RID: 3417
	private class TickCallbackWrapperPost : TickSystem<T>.TickCallbackWrapper<ITickSystemPost>
	{
		// Token: 0x06005492 RID: 21650 RVA: 0x001BC726 File Offset: 0x001BA926
		public override void CallBack()
		{
			this.m_target.PostTick();
		}
	}
}
