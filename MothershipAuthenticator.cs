using System;
using GorillaExtensions;
using Steamworks;
using UnityEngine;

// Token: 0x02000CD3 RID: 3283
public class MothershipAuthenticator : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06005154 RID: 20820 RVA: 0x001AF3F5 File Offset: 0x001AD5F5
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		if (MothershipAuthenticator.Instance == null)
		{
			MothershipAuthenticator.Instance = null;
		}
	}

	// Token: 0x06005155 RID: 20821 RVA: 0x001AF410 File Offset: 0x001AD610
	public void Awake()
	{
		if (MothershipAuthenticator.Instance == null)
		{
			MothershipAuthenticator.Instance = this;
		}
		else if (MothershipAuthenticator.Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		if (!MothershipClientApiUnity.IsEnabled())
		{
			Debug.Log("Mothership is not enabled.");
			return;
		}
		if (MothershipAuthenticator.Instance.SteamAuthenticator == null)
		{
			MothershipAuthenticator.Instance.SteamAuthenticator = MothershipAuthenticator.Instance.gameObject.GetOrAddComponent<SteamAuthenticator>();
		}
		MothershipClientApiUnity.SetLogCallback(delegate(MothershipLogLevel level, string message)
		{
			LogType logType;
			switch (level)
			{
			case MothershipLogLevel.INFO:
				logType = LogType.Log;
				break;
			case MothershipLogLevel.WARN:
				logType = LogType.Warning;
				break;
			case MothershipLogLevel.ERROR:
				logType = LogType.Error;
				break;
			default:
				logType = LogType.Log;
				break;
			}
			PersistLog.Log(logType, message);
		});
		MothershipClientApiUnity.SetAuthRefreshedCallback(delegate(string id)
		{
			this.BeginLoginFlow();
		});
	}

	// Token: 0x06005156 RID: 20822 RVA: 0x001AF4C8 File Offset: 0x001AD6C8
	public void BeginLoginFlow()
	{
		Debug.Log("making login call");
		this.LogInWithSteam();
	}

	// Token: 0x06005157 RID: 20823 RVA: 0x001AF4DA File Offset: 0x001AD6DA
	private void LogInWithInsecure()
	{
		MothershipClientApiUnity.LogInWithInsecure1(this.TestNickname, this.TestAccountId, delegate(LoginResponse LoginResponse)
		{
			Debug.Log("Logged in with Mothership Id " + LoginResponse.MothershipPlayerId);
			MothershipClientApiUnity.OpenNotificationsSocket();
			Action onLoginSuccess = this.OnLoginSuccess;
			if (onLoginSuccess == null)
			{
				return;
			}
			onLoginSuccess();
		}, delegate(MothershipError MothershipError, int errorCode)
		{
			Debug.LogError(string.Format("Failed to log in, error {0} trace ID: {1} status: {2} Mothership error code: {3}", new object[] { MothershipError.Message, MothershipError.TraceId, errorCode, MothershipError.MothershipErrorCode }));
			Action<int> onLoginAttemptFailure = this.OnLoginAttemptFailure;
			if (onLoginAttemptFailure != null)
			{
				onLoginAttemptFailure(1);
			}
			Action<string, string, string> onLoginFailure = this.OnLoginFailure;
			if (onLoginFailure == null)
			{
				return;
			}
			onLoginFailure(MothershipError.Message, MothershipError.MothershipErrorCode, MothershipError.TraceId);
		});
	}

	// Token: 0x06005158 RID: 20824 RVA: 0x001AF506 File Offset: 0x001AD706
	private void LogInWithSteam()
	{
		MothershipClientApiUnity.StartLoginWithSteam(delegate(PlayerSteamBeginLoginResponse resp)
		{
			Debug.Log(string.Format("Mothership: Steam Login started at {0}", DateTime.Now));
			string nonce = resp.Nonce;
			SteamAuthTicket ticketHandle = HAuthTicket.Invalid;
			Action<LoginResponse> <>9__4;
			Action<MothershipError, int> <>9__5;
			ticketHandle = this.SteamAuthenticator.GetAuthTicketForWebApi(nonce, delegate(string ticket)
			{
				Debug.Log(string.Format("Mothership: Attempting to complete login at {0}", DateTime.Now));
				string nonce2 = nonce;
				Action<LoginResponse> action;
				if ((action = <>9__4) == null)
				{
					action = (<>9__4 = delegate(LoginResponse successResp)
					{
						ticketHandle.Dispose();
						Debug.Log("Logged in to Mothership with Steam");
						MothershipClientApiUnity.OpenNotificationsSocket();
						Action onLoginSuccess = this.OnLoginSuccess;
						if (onLoginSuccess == null)
						{
							return;
						}
						onLoginSuccess();
					});
				}
				Action<MothershipError, int> action2;
				if ((action2 = <>9__5) == null)
				{
					action2 = (<>9__5 = delegate(MothershipError MothershipError, int errorCode)
					{
						ticketHandle.Dispose();
						Debug.LogError(string.Format("Couldn't log into Mothership with Steam error {0} trace ID: {1} status: {2} Mothership error code: {3}", new object[] { MothershipError.Message, MothershipError.TraceId, errorCode, MothershipError.MothershipErrorCode }));
						this.loginAttempts++;
						Action<int> onLoginAttemptFailure = this.OnLoginAttemptFailure;
						if (onLoginAttemptFailure != null)
						{
							onLoginAttemptFailure(this.loginAttempts);
						}
						if (MothershipError.StatusCode != 400 && this.loginAttempts < this.MaxLoginAttempts)
						{
							this.LogInWithSteam();
							return;
						}
						Action<string, string, string> onLoginFailure = this.OnLoginFailure;
						if (onLoginFailure == null)
						{
							return;
						}
						onLoginFailure(MothershipError.Message, MothershipError.MothershipErrorCode, MothershipError.TraceId);
					});
				}
				MothershipClientApiUnity.CompleteLoginWithSteam(nonce2, ticket, action, action2);
			}, delegate(EResult error)
			{
				string text = string.Format("Couldn't get an auth ticket for logging into Mothership with Steam: {0}", error);
				Debug.LogError(text);
				Action<int> onLoginAttemptFailure2 = this.OnLoginAttemptFailure;
				if (onLoginAttemptFailure2 != null)
				{
					onLoginAttemptFailure2(1);
				}
				Action<string, string, string> onLoginFailure2 = this.OnLoginFailure;
				if (onLoginFailure2 == null)
				{
					return;
				}
				onLoginFailure2(text, "", "");
			});
		}, delegate(MothershipError MothershipError, int errorCode)
		{
			Debug.LogError(string.Format("Couldn't start Mothership auth for Steam error {0} trace ID: {1} status: {2} Mothership error code: {3}", new object[] { MothershipError.Message, MothershipError.TraceId, errorCode, MothershipError.MothershipErrorCode }));
			Action<int> onLoginAttemptFailure3 = this.OnLoginAttemptFailure;
			if (onLoginAttemptFailure3 != null)
			{
				onLoginAttemptFailure3(1);
			}
			Action<string, string, string> onLoginFailure3 = this.OnLoginFailure;
			if (onLoginFailure3 == null)
			{
				return;
			}
			onLoginFailure3(MothershipError.Message, MothershipError.MothershipErrorCode, MothershipError.TraceId);
		});
	}

	// Token: 0x06005159 RID: 20825 RVA: 0x001AF526 File Offset: 0x001AD726
	public void OnEnable()
	{
		if (MothershipClientApiUnity.IsEnabled())
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			this.lastSliceUpdateTime = Time.unscaledTimeAsDouble;
		}
	}

	// Token: 0x0600515A RID: 20826 RVA: 0x001AF541 File Offset: 0x001AD741
	public void OnDisable()
	{
		if (MothershipClientApiUnity.IsEnabled())
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}
	}

	// Token: 0x0600515B RID: 20827 RVA: 0x001AF554 File Offset: 0x001AD754
	public void SliceUpdate()
	{
		double unscaledTimeAsDouble = Time.unscaledTimeAsDouble;
		float num = (float)(unscaledTimeAsDouble - this.lastSliceUpdateTime);
		this.lastSliceUpdateTime = unscaledTimeAsDouble;
		MothershipClientApiUnity.Tick(num);
	}

	// Token: 0x04006361 RID: 25441
	public static volatile MothershipAuthenticator Instance;

	// Token: 0x04006362 RID: 25442
	public MetaAuthenticator MetaAuthenticator;

	// Token: 0x04006363 RID: 25443
	public SteamAuthenticator SteamAuthenticator;

	// Token: 0x04006364 RID: 25444
	public string TestNickname;

	// Token: 0x04006365 RID: 25445
	public string TestAccountId;

	// Token: 0x04006366 RID: 25446
	public bool UseConstantTestAccountId;

	// Token: 0x04006367 RID: 25447
	private int loginAttempts;

	// Token: 0x04006368 RID: 25448
	public int MaxLoginAttempts = 5;

	// Token: 0x04006369 RID: 25449
	public Action OnLoginSuccess;

	// Token: 0x0400636A RID: 25450
	public Action<string, string, string> OnLoginFailure;

	// Token: 0x0400636B RID: 25451
	public Action<int> OnLoginAttemptFailure;

	// Token: 0x0400636C RID: 25452
	private double lastSliceUpdateTime;
}
