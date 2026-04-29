using System.Reflection;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Pooling;
using MegaCrit.Sts2.Core.Nodes.Screens;
using SignatureLib.Code.Cards;
using SignatureLib.Code.Signature;
using SignatureLib.Code.Ui;
using SignatureLib.Code.Utils;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace SignatureLib.Code.Patches;

public class InspectPatch {
	private static Logger Logger { get; } = new(nameof(InspectPatch), LogType.Generic);

	public static readonly SpireField<NInspectCardScreen, InspectScreenOptions>
		InspectScreenOptionsField = new(() => null);

	[HarmonyPatch(typeof(NInspectCardScreen), "_Ready")]
	public static class ReadyPatch {
		[HarmonyPostfix]
		public static void Postfix(NInspectCardScreen __instance) {
			InspectScreenOptionsField.Set(__instance, new InspectScreenOptions(__instance));
		}
	}

	// [HarmonyPatch(typeof(NInspectCardScreen), "Close")]
	// public static class ClosePatch {
	// 	[HarmonyPostfix]
	// 	public static void Postfix(NInspectCardScreen __instance) {
	// 		InspectScreenOptionsField.Set(__instance, null);
	// 	}
	// }

	[HarmonyPatch(typeof(NInspectCardScreen), "UpdateCardDisplay")]
	public static class UpdateCardDisplayPatch {
		[HarmonyPostfix]
		public static void Postfix(NInspectCardScreen __instance, NCard ____card) {
			if (NCardPatch.Helper[____card] != null) {
				Logger.Debug($"Now Setting {____card.Model.Id.Entry} to AlwaysHovered");
				NCardPatch.Helper[____card].AlwaysHovered = !InspectScreenOptionsField.Get(__instance)?
						.HideDescriptionProperty ?? true;
			}
		}
	}

	[HarmonyPatch(typeof(NInspectCardScreen), "SetCard")]
	public static class SetCardPatch {
		[HarmonyPrefix]
		public static void Prefix(NInspectCardScreen __instance, int index, List<CardModel> ____cards) {
			// Logger.VeryDebug("Set card");

			// Logger.VeryDebug($"Set card {index}, cards: {cards?.Count}");

			index = Math.Clamp(index, 0, ____cards?.Count - 1 ?? 0);
			InspectScreenOptionsField.Get(__instance)?.SetCard(____cards?[index]);

			// Logger.VeryDebug("Set card complete");
		}
	}
}
