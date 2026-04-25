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

	public static readonly SpireField<CardModel, HashSet<NCard>> NCards =
			new(() => []);

	public static readonly SpireField<CardModel, bool?> HasSignatureField = new(() => null);

	public static bool HasSignature(CardModel card) {
		return HasSignatureField[card] ??=
				SignatureHelper.IsRegistered(card.Id) &&
				ResourceLoader.Exists(SignatureHelper.GetInfo(card.Id).SignaturePortraitPath.Invoke(card));
	}

	// public bool HasSignature => this._hasSignature ??= ResourceLoader.Exists(
	// 		this.SignaturePortraitPath.Invoke(this._card));

	public static readonly SpireField<CardModel, CardModelHelper?> Helper = new(() => null);

	[HarmonyPatch(typeof(NCard), "SubscribeToModel")]
	public static class SubscribePatch {
		[HarmonyPostfix]
		public static void Postfix(NCard inst, CardModel? model) {
			if (model != null && inst.IsInsideTree())
				NCards[model]?.Add(inst);
		}
	}

	[HarmonyPatch(typeof(NCard), "UnsubscribeFromModel")]
	public static class UnsubscribePatch {
		[HarmonyPostfix]
		public static void Postfix(NCard inst, CardModel? model) {
			if (model != null)
				NCards[model]?.Remove(inst);
		}
	}
}
