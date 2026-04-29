using System.Reflection.Emit;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Models;
using SignatureLib.Code.Config;
using SignatureLib.Code.Extensions;
using SignatureLib.Code.Utils;

namespace SignatureLib.Code.Signature;

public class CardModelHelper {
	public static CardModelHelper Get(CardModel card) => new(card);

	private readonly CardModel _card;

	private Func<CardModel, string> SignaturePortraitPath =>
			SignatureLibHelper.GetInfo(this._card.Id).SignaturePortraitPath;

	private Func<CardModel, bool> SignaturePredicate =>
			SignatureLibHelper.GetInfo(this._card.Id).SignaturePredicate;

	public Texture2D? SignaturePortrait => PreloadManager.Cache.GetTexture2D(
		this.SignaturePortraitPath.Invoke(this._card));
	public Texture2D SignatureTextBg => PreloadManager.Cache.GetTexture2D("desc_shadow.png".CardItemPath());

	// public bool HasSignature => this._hasSignature ??= ResourceLoader.Exists(
	// 		this.SignaturePortraitPath.Invoke(this._card));

	public ModelId Id => this._card.Id;
	public bool Enabled => SignatureLib.IsEnabled(this.Id);
	public bool ShouldShowSignature => this.Enabled && this.SignaturePredicate.Invoke(this._card);

	public CardModelHelper(CardModel card) {
		this._card = card;
		// this.SignaturePortraitPath = info.SignaturePortraitPath;
		// this.SignaturePredicate = info.SignaturePredicate;
	}
}
