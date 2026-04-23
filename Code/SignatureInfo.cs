using MegaCrit.Sts2.Core.Models;

namespace SignatureLib.Code;

public class SignatureInfo {
	public Func<CardModel, string> SignaturePortraitPath { get; private set; } = card =>
			card.PortraitPath.Replace("/cards/", "/signature/").Replace(@"\cards\", @"\signature\")
					.Replace("/card/", "/signature/").Replace(@"\cards\", @"\signature\");

	public Func<CardModel, bool> SignaturePredicate { get; private set; } = _ => true;

	public SignatureInfo() {}

	public SignatureInfo Portrait(Func<CardModel, string> func) {
		this.SignaturePortraitPath = func;
		return this;
	}

	public SignatureInfo Portrait(string path) {
		this.SignaturePortraitPath = _ => path;
		return this;
	}

	public SignatureInfo Predicate(Func<CardModel, bool> predicate) {
		this.SignaturePredicate = predicate;
		return this;
	}
}
