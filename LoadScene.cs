using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000030 RID: 48
public class LoadScene : MonoBehaviour
{
	// Token: 0x060000B0 RID: 176 RVA: 0x000056FE File Offset: 0x000038FE
	public IEnumerator Start()
	{
		yield return new WaitForSecondsRealtime(this._delay);
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(this._sceneName, LoadSceneMode.Single);
		while (asyncOperation.progress < 0.99f)
		{
			yield return null;
		}
		asyncOperation.allowSceneActivation = true;
		yield break;
	}

	// Token: 0x040000D0 RID: 208
	[SerializeField]
	private float _delay;

	// Token: 0x040000D1 RID: 209
	[SerializeField]
	private string _sceneName;
}
