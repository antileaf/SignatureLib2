using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace SignatureLib.Code.Utils;

public abstract class SignatureHelper {
	private static Logger Logger { get; } = new(nameof(SignatureHelper), LogType.Generic);

	private static Dictionary<ModelId, SignatureInfo> _registered = new();

	public static void register(ModelId id, SignatureInfo info) {
		if (_registered.ContainsKey(id))
			Logger.Warn($"Multiple registration of {id.Entry} detected");
		else
			_registered.Add(id, info);
	}

	public static bool IsRegistered(ModelId id) => _registered.ContainsKey(id);

	public static SignatureInfo GetInfo(ModelId id) => _registered[id];
}
