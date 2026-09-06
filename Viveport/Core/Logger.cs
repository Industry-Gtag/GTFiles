using System;
using System.Reflection;

namespace Viveport.Core
{
	// Token: 0x02000EE7 RID: 3815
	public class Logger
	{
		// Token: 0x06005CE6 RID: 23782 RVA: 0x001DCA6C File Offset: 0x001DAC6C
		public static void Log(string message)
		{
			if (!Logger._hasDetected || Logger._usingUnityLog)
			{
				Logger.UnityLog(message);
				return;
			}
			Logger.ConsoleLog(message);
		}

		// Token: 0x06005CE7 RID: 23783 RVA: 0x001DCA89 File Offset: 0x001DAC89
		private static void ConsoleLog(string message)
		{
			Console.WriteLine(message);
			Logger._hasDetected = true;
		}

		// Token: 0x06005CE8 RID: 23784 RVA: 0x001DCA98 File Offset: 0x001DAC98
		private static void UnityLog(string message)
		{
			try
			{
				if (Logger._unityLogType == null)
				{
					Logger._unityLogType = Logger.GetType("UnityEngine.Debug");
				}
				Logger._unityLogType.GetMethod("Log", new Type[] { typeof(string) }).Invoke(null, new object[] { message });
				Logger._usingUnityLog = true;
			}
			catch (Exception)
			{
				Logger.ConsoleLog(message);
				Logger._usingUnityLog = false;
			}
			Logger._hasDetected = true;
		}

		// Token: 0x06005CE9 RID: 23785 RVA: 0x001DCB24 File Offset: 0x001DAD24
		private static Type GetType(string typeName)
		{
			Type type = Type.GetType(typeName);
			if (type != null)
			{
				return type;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				type = assemblies[i].GetType(typeName);
				if (type != null)
				{
					return type;
				}
			}
			return null;
		}

		// Token: 0x04006CAB RID: 27819
		private const string LoggerTypeNameUnity = "UnityEngine.Debug";

		// Token: 0x04006CAC RID: 27820
		private static bool _hasDetected;

		// Token: 0x04006CAD RID: 27821
		private static bool _usingUnityLog = true;

		// Token: 0x04006CAE RID: 27822
		private static Type _unityLogType;
	}
}
