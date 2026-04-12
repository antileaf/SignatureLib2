using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens;
using SignatureLib.Code.Extensions;
using SignatureLib.Code.Utils;
using Label = Godot.Label;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace SignatureLib.Code.Ui;

public class InspectScreenOptions {
	private static Logger Logger { get; } = new(nameof(InspectScreenOptions), LogType.Generic);

	public static readonly string SignatureLabelScenePath = "SignatureLabel.tscn".ScenePath();

	private NInspectCardScreen _inspectCardScreen;
	private NSignatureTickbox _signatureTickbox, _hideDescriptionTickbox;
	private CardModel? _currentCard;
	private bool _hideDescription = false;

	public bool SignatureProperty {
		get => this._currentCard != null && SignatureLibHelper.IsEnabled(this._currentCard.Id);
		set {
			if (this._currentCard == null)
				return;

			if (SignatureLibHelper.CardHasSignature(this._currentCard)) {
				if (SignatureLibHelper.IsEnabled(this._currentCard.Id) != value) {
					SignatureLibHelper.Enable(this._currentCard.Id, value);
					Logger.Debug($"Now signature is {value}");
					this.InvokeUpdateCardDisplayDeferred();
				}
			}
			else
				Logger.Warn("signature tickbox toggled with a card with no signature");
		}
	}

	public bool HideDescriptionProperty {
		get => this._hideDescription;
		set {
			if (this._currentCard == null)
				return;

			if (this._hideDescription != value) {
				this._hideDescription = value;

				if (SignatureLibHelper.CardHasSignature(this._currentCard))
					this.InvokeUpdateCardDisplayDeferred();
			}
		}
	}

	public InspectScreenOptions(NInspectCardScreen inst) {
		_inspectCardScreen = inst;

		_signatureTickbox = new NSignatureTickbox();
		_signatureTickbox.Position = new Vector2(270f, 700f);
		_signatureTickbox.Disable();
		_signatureTickbox.SetTickedDeferred(this.SignatureProperty);
		_signatureTickbox.Toggled += delegate(NTickbox _) {
			this.SignatureProperty = _signatureTickbox.IsTicked;

			this.HideDescriptionProperty = false;
			this._hideDescriptionTickbox?.SetTickedDeferred(false);

			if (!_signatureTickbox.IsTicked)
				this._hideDescriptionTickbox?.DisableAndHide();
			else
				this._hideDescriptionTickbox?.EnableAndShow();
		};

		_hideDescriptionTickbox = new NSignatureTickbox();
		_hideDescriptionTickbox.Position = new Vector2(270f, 780f);
		_hideDescriptionTickbox.Disable();
		_hideDescriptionTickbox.SetTickedDeferred(this.HideDescriptionProperty);
		_hideDescriptionTickbox.Toggled += delegate(NTickbox _) {
			this.HideDescriptionProperty = _hideDescriptionTickbox.IsTicked;
		};

		inst.AddChild(_signatureTickbox);
		inst.AddChild(_hideDescriptionTickbox);

		MegaLabel upgradeLabel = inst.GetNode<MegaLabel>((NodePath)"%ShowUpgradeLabel");

		string signatureLabelText = LocManager.Instance.Language == "zhs" ? "启用异画" : "Signature";
		string hideDescriptionLabelText = LocManager.Instance.Language == "zhs" ? "隐藏描述" : "Hide Description";

		Label signatureLabel = PreloadManager.Cache.GetScene(SignatureLabelScenePath).Instantiate<Label>();
		signatureLabel.Text = signatureLabelText;
		signatureLabel.AddThemeFontOverride("font", upgradeLabel.GetThemeFont("font"));
		signatureLabel.Position = new Vector2(80f, 0f);

		Label hideDescriptionLabel = PreloadManager.Cache.GetScene(SignatureLabelScenePath).Instantiate<Label>();
		hideDescriptionLabel.Text = hideDescriptionLabelText;
		hideDescriptionLabel.AddThemeFontOverride("font", upgradeLabel.GetThemeFont("font"));
		hideDescriptionLabel.Position = new Vector2(80f, 0f);

		this._signatureTickbox.AddChild(signatureLabel);
		this._hideDescriptionTickbox.AddChild(hideDescriptionLabel);

		// _signatureTickbox.Toggled += ToggleSignatureTickbox;
		// _hideDescriptionTickbox.Toggled += ToggleHideDescriptionTickbox;
	}

	private void InvokeUpdateCardDisplayDeferred() {
		this._inspectCardScreen.CallDeferred("UpdateCardDisplay", null);
		// AccessTools.DeclaredMethod(typeof(NInspectCardScreen), "UpdateCardDisplay")
		// 	.Invoke(this._inspectCardScreen, null);
	}

	// ~InspectScreenOptions() {
	// 	_signatureTickbox.Toggled -= ToggleSignatureTickbox;
	// 	_hideDescriptionTickbox.Toggled -= ToggleHideDescriptionTickbox;
	// }

	// public void ToggleSignatureTickbox(NTickbox _) {
	// }
	//
	// public void ToggleHideDescriptionTickbox(NTickbox _) {
	// 	if (_currentCard == null)
	// 		return;
	//
	// 	AccessTools.DeclaredMethod(typeof(NInspectCardScreen), "UpdateCardDisplay")
	// 		.Invoke(_inspectCardScreen, null);
	// }

	public void SetCard(CardModel? card) {
		Logger.Debug($"SetCard {card?.Id.ToString()??"(null)"}");

		_currentCard = card;

		if (card != null && SignatureLibHelper.CardHasSignature(card)) {
			this.SignatureProperty = SignatureLibHelper.IsEnabled(card.Id);
			this._signatureTickbox.SetTickedDeferred(this.SignatureProperty);
			// this.HideDescriptionProperty = this._hideDescription;
			// HideDescriptionProperty = false;

			this._signatureTickbox.EnableAndShow();

			if (this.SignatureProperty) {
				this._hideDescriptionTickbox.EnableAndShow();
				this._hideDescriptionTickbox.SetTickedDeferred(this.HideDescriptionProperty);
			}
			else {
				this._hideDescriptionTickbox.DisableAndHide();
				this.HideDescriptionProperty = false;
				this._hideDescriptionTickbox.SetTickedDeferred(false);
			}
		}
		else {
			this.SignatureProperty = false;
			this.HideDescriptionProperty = false;
			this._hideDescriptionTickbox.SetTickedDeferred(false);

			this._signatureTickbox.DisableAndHide();
			this._hideDescriptionTickbox.DisableAndHide();
		}
	}
}
