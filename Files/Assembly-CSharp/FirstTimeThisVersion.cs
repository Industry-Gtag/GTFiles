using System.Threading.Tasks;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Events;

public class FirstTimeThisVersion : MonoBehaviour
{
	private const string PP_KEY = "Version%Build";

	[SerializeField]
	private UnityEvent OnFirstTimeThisVersion;

	[SerializeField]
	private UnityEvent OnFirstTimeThisBuild;

	private string oldData;

	private string newData;

	private async void Start()
	{
		while (GorillaComputer.instance == null || GorillaComputer.instance.GetServerTime().Year < 2000)
		{
			await Task.Delay(500);
		}
		oldData = PlayerPrefs.GetString("Version%Build", "0%0");
		newData = GorillaComputer.instance.version + "%" + GorillaComputer.instance.buildCode;
		if (oldData != newData)
		{
			if (oldData.Split("%")[0] != newData.Split("%")[0])
			{
				OnFirstTimeThisVersion?.Invoke();
			}
			OnFirstTimeThisBuild?.Invoke();
			PlayerPrefs.SetString("Version%Build", newData);
		}
	}

	private void InvokeVersion()
	{
		OnFirstTimeThisVersion?.Invoke();
	}

	private void InvokeBuild()
	{
		OnFirstTimeThisBuild?.Invoke();
	}
}
