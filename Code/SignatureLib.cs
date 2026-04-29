using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using SignatureLib.Code.Cards;
using SignatureLib.Code.Config;
using SignatureLib.Code.Patches;
using SignatureLib.Code.Utils;

namespace SignatureLib.Code;

public abstract class SignatureLib {
	private static Logger Logger { get; } = new(nameof(SignatureLib), LogType.Generic);

	public static bool IsEnabled(ModelId id) {
		return SignatureLibConfig.GetEnabled(id.ToString()) == true;
	}

	public static void Enable(ModelId id, bool enabled) {
		if (!SignatureLibHelper.IsRegistered(id)) {
			Logger.Warn($"enable(): Card with ID {id} does not have a signature");
			return;
		}

		SignatureLibConfig.SetEnabled(id.ToString(), enabled);
	}
}
