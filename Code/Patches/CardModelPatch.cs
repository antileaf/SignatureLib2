using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using SignatureLib.Code.Signature;
using SignatureLib.Code.Utils;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace SignatureLib.Code.Patches;

public static class CardModelPatch {
	private static Logger Logger { get; } = new(nameof(CardModelPatch), LogType.Generic);

	// public static readonly SpireField<CardModel, HashSet<NCard>> NCards =
	// 		new(() => []);

	// public static readonly SpireField<CardModel, bool?> HasSignatureField = new(() => null);

	// public static bool HasSignature(CardModel card) {
	// 	return HasSignatureField[card] ??=
	// 			SignatureLibHelper.IsRegistered(card.Id) &&
	// 			ResourceLoader.Exists(SignatureLibHelper.GetInfo(card.Id).SignaturePortraitPath.Invoke(card));
	// }

	// public static readonly FakeField<CardModel, CardModelHelper> Helper =
	// 		new(card => new CardModelHelper(card));

	// [HarmonyPatch(typeof(CardModel), MethodType.Constructor)]
	// public static class ConstructorPatch {
	// 	[HarmonyPostfix]
	// 	public static void Postfix(CardModel __instance) {
	// 		Helper[__instance] = new CardModelHelper(__instance);
	// 	}
	// }

	// [HarmonyPatch(typeof(NCard), nameof(NCard.Model), MethodType.Setter)]
	// public static class ModelSetterPatch {
	// 	[HarmonyPostfix]
	// 	public static void Postfix(NCard __instance, ref CardModel? ____model, CardModel? value) {
	// 		if (____model != null)
	// 			NCards[____model]?.Remove(__instance);
	//
	// 		if (value != null && SignatureLibHelper.IsRegistered(value.Id))
	// 			NCards[value].Add(__instance);
	// 	}
	// }
}
