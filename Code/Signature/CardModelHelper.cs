using Godot;
using MegaCrit.Sts2.Core.Models;

namespace SignatureLib.Code.Signature;

public class CardModelHelper {
	private readonly CardModel _card;

	private Func<CardModel, string> SignaturePortraitPath { get; }

	private Func<CardModel, bool> SignaturePredicate { get; }

	public CardModelHelper(CardModel card, SignatureInfo info) {
		this._card = card;
		this.SignaturePortraitPath = info.SignaturePortraitPath;
		this.SignaturePredicate = info.SignaturePredicate;
	}
}
