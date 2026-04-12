using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using BaseLib.Config;
using Godot;
using MegaCrit.Sts2.Core.Logging;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace SignatureLib.Code.Config;

public class SignatureLibConfig : SimpleModConfig {
	private static Logger Logger { get; set; } = new(nameof(SignatureLibConfig), LogType.Generic);

	[ConfigIgnore]
	public static Dictionary<string, bool> Enabled { get; set; } = new();

	[ConfigHideInUI]
	public static string SerializedEnabled { get; set; }

	public static bool AlwaysShowDescription { get; set; } = false;

	public static void OnLoadConfig() { // Called in patch
		Dictionary<string, bool>? deserializedEnabled = null;
		try {
			deserializedEnabled = JsonSerializer.Deserialize<Dictionary<string, bool>>(SerializedEnabled);
		}
		catch {
			Logger.Info("Error occured deserializing enabled config");
		}

		if (deserializedEnabled != null)
			Enabled = deserializedEnabled;
		else
			Logger.Warn("Failed to deserialize Enabled dictionary, using empty dictionary instead");
	}

	public static void SetEnabled(string cardId, bool enabled) {
		Enabled[cardId] = enabled;

		SerializedEnabled = JsonSerializer.Serialize(Enabled);
		ModConfig.SaveDebounced<SignatureLibConfig>();
	}

	public static bool? GetEnabled(string cardId) {
		return Enabled.ContainsKey(cardId) ? Enabled[cardId] : null;
	}
}
