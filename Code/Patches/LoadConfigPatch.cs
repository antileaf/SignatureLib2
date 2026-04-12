using System.Reflection;
using BaseLib.Config;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using SignatureLib.Code.Config;

namespace SignatureLib.Code.Patches;

public class LoadConfigPatch {
	private static Logger Logger { get; set; } = new(nameof(LoadConfigPatch), LogType.Generic);

	[HarmonyPatch]
	public static class LoadPatch {
		static MethodBase TargetMethod() {
			return typeof(ModConfig)
				.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
				.First(m =>
						m.Name == "Load" &&
						!m.IsGenericMethod &&          // 排除 Load<T>
						!m.IsStatic &&                 // 排除 static
						m.GetParameters().Length == 0  // 无参数
				);
		}

		[HarmonyPostfix]
		public static void Postfix(ModConfig __instance) {
			if (__instance is SignatureLibConfig) {
				Logger.Info("SignatureLibConfig found, running LoadPatch.");
				SignatureLibConfig.OnLoadConfig();
			}
		}
	}
}
