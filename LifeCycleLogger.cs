using System;
using UnityEngine;

// Token: 0x020009F8 RID: 2552
public class LifeCycleLogger : MonoBehaviour
{
	// Token: 0x06004183 RID: 16771 RVA: 0x0015C665 File Offset: 0x0015A865
	private void Awake()
	{
		PersistLog.Log(string.Format("[AC][F{0}] {1} Awake", Time.frameCount, base.name));
	}

	// Token: 0x06004184 RID: 16772 RVA: 0x0015C686 File Offset: 0x0015A886
	private void Start()
	{
		PersistLog.Log(string.Format("[AC][F{0}] {1} Start", Time.frameCount, base.name));
	}

	// Token: 0x06004185 RID: 16773 RVA: 0x0015C6A7 File Offset: 0x0015A8A7
	private void OnEnable()
	{
		PersistLog.Log(string.Format("[AC][F{0}] {1} Enable", Time.frameCount, base.name));
	}

	// Token: 0x06004186 RID: 16774 RVA: 0x0015C6C8 File Offset: 0x0015A8C8
	private void OnDisable()
	{
		PersistLog.Log(string.Format("[AC][F{0}] {1} Disable", Time.frameCount, base.name));
	}

	// Token: 0x06004187 RID: 16775 RVA: 0x0015C6E9 File Offset: 0x0015A8E9
	private void OnDestroy()
	{
		PersistLog.Log(string.Format("[AC][F{0}] {1} OnDestroy", Time.frameCount, base.name));
	}
}
