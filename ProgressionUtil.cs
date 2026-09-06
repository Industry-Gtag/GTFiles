using System;
using System.Threading.Tasks;
using PlayFab;

// Token: 0x020009D5 RID: 2517
public class ProgressionUtil
{
	// Token: 0x060040A2 RID: 16546 RVA: 0x001585AC File Offset: 0x001567AC
	public static async Task WaitForMothershipSessionToken()
	{
		while (!MothershipClientContext.IsClientLoggedIn())
		{
			await Task.Delay(1000);
		}
	}

	// Token: 0x060040A3 RID: 16547 RVA: 0x001585E8 File Offset: 0x001567E8
	public static async Task WaitForPlayFabSessionTicket()
	{
		while (!PlayFabClientAPI.IsClientLoggedIn())
		{
			await Task.Delay(1000);
		}
	}
}
