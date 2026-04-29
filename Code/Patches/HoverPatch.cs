using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Pooling;
using SignatureLib.Code.Cards;
using SignatureLib.Code.Config;
using SignatureLib.Code.Utils;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace SignatureLib.Code.Patches;

public class HoverPatch {
	private static Logger Logger { get; } = new(nameof(HoverPatch), LogType.Generic);

	[HarmonyPatch(typeof(NCardHolder), "ConnectSignals")]
	public static class ConnectSignalsPatch {
		[HarmonyPostfix]
		public static void Postfix(NCardHolder __instance) {
			__instance.Hitbox.MouseEntered += delegate {
				// Logger.VeryDebug("Mouse Entered");

				if (__instance is { CardNode: not null, CardModel: not null } &&
					    SignatureLibHelper.IsRegistered(__instance.CardModel.Id) &&
					    !SignatureLibConfig.AlwaysShowDescription) {
					if (NCardPatch.Helper[__instance.CardNode] != null) {
						// Logger.Warn("NCardHelper is null for card " + __instance.CardModel.Id.Entry);
						NCardPatch.Helper[__instance.CardNode].SignatureHovered = true;
					}
				}
			};

			__instance.Hitbox.MouseExited += delegate {
				// Logger.VeryDebug("Mouse Exited");

				if (__instance is { CardNode: not null, CardModel: not null } &&
					    SignatureLibHelper.IsRegistered(__instance.CardModel.Id) &&
					    !SignatureLibConfig.AlwaysShowDescription) {
					if (NCardPatch.Helper[__instance.CardNode] != null) {
						// Logger.Warn("NCardHelper is null for card " + __instance.CardModel.Id.Entry);
						NCardPatch.Helper[__instance.CardNode].SignatureHovered = false;
					}
				}
			};
		}
	}
}
