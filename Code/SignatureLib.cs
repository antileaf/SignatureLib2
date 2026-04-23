using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using SignatureLib.Code.Cards;
using SignatureLib.Code.Config;

namespace SignatureLib.Code;

public abstract class SignatureLib {
	private static Logger Logger { get; } = new(nameof(SignatureLib), LogType.Generic);

	public static bool CardHasSignature(CardModel card) {
		if (card is AbstractSignatureCard signatureCard)
			return signatureCard.HasSignature;

		return false; // may be added in future
	}

	public static bool HasSignature(ModelId id) {
		return ModelDb.AllCards.FirstOrDefault(c => c.Id == id) is
			AbstractSignatureCard { HasSignature : true };
	}

	public static bool IsEnabled(ModelId id) {
		return HasSignature(id) && SignatureLibConfig.GetEnabled(id.ToString()) == true;
	}

	public static void Enable(ModelId id, bool enabled) {
		if (!HasSignature(id)) {
			Logger.Warn($"enable(): Card with ID {id} does not have a signature");
			return;
		}

		SignatureLibConfig.SetEnabled(id.ToString(), enabled);
	}
}
