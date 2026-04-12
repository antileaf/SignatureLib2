using System.ComponentModel;
using BaseLib.Utils;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.Settings;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace SignatureLib.Code.Ui;

public partial class NSignatureTickbox : NSettingsTickbox {
	private static Logger Logger { get; } = new(nameof(NSignatureTickbox), LogType.Generic);

	public NSignatureTickbox() {
		SetCustomMinimumSize(new Vector2(64f, 64f));
		this.SizeFlagsHorizontal = SizeFlags.ShrinkEnd;
		this.SizeFlagsVertical = SizeFlags.Fill;
		this.FocusMode = FocusModeEnum.All;
		this.MouseFilter = MouseFilterEnum.Pass;

		this.TransferAllNodes<NSignatureTickbox>(SceneHelper.GetScenePath("screens/settings_tickbox"));
	}

	public override void _Ready() {
		this.ConnectSignals();

		// Traverse traverse = Traverse.Create(this);

		// Control tickedImage = traverse.Field("_tickedImage").GetValue<Control>();
		// tickedImage.Position = new Vector2(-150f, 0f);
		//
		// Control notTickedImage = traverse.Field("_notTickedImage").GetValue<Control>();
		// notTickedImage.Position = new Vector2(-150f, 0f);
	}

	public void SetTickedDeferred(bool value) {
		this.CallDeferred(nameof(SetTickedInternal), value);
	}

	private void SetTickedInternal(bool ticked) {
		this.IsTicked = ticked;
	}

	public void EnableAndShow() {
		this.CallDeferred(nameof(Enable));
		this.CallDeferred(CanvasItem.MethodName.Show);
	}

	public void DisableAndHide() {
		this.CallDeferred(nameof(Disable));
		this.CallDeferred(CanvasItem.MethodName.Hide);
	}

	// public void OnToggle(NTickbox _) {
	// 	Logger.Info($"OnToggle, this.IsTicked = {this.IsTicked}");
	//
	// 	// if (this.IsTicked) {
	// 	// 	((Control)AccessTools.DeclaredField(typeof(NTickbox), "_tickedImage").GetValue(this)).Show();
	// 	// 	((Control)AccessTools.DeclaredField(typeof(NTickbox), "_notTickedImage").GetValue(this)).Hide();
	// 	// }
	// 	// else {
	// 	// 	((Control)AccessTools.DeclaredField(typeof(NTickbox), "_tickedImage").GetValue(this)).Hide();
	// 	// 	((Control)AccessTools.DeclaredField(typeof(NTickbox), "_notTickedImage").GetValue(this)).Show();
	// 	// }
	// }
}
