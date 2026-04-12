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

				if (__instance.CardModel is AbstractSignatureCard signatureCard &&
						!SignatureLibConfig.AlwaysShowDescription) {
					signatureCard.SignatureHovered = true;
				}
			};

			__instance.Hitbox.MouseExited += delegate {
				if (__instance.CardModel is AbstractSignatureCard signatureCard &&
						!SignatureLibConfig.AlwaysShowDescription) {
					signatureCard.SignatureHovered = false;
				}
			};
		}
	}
}
