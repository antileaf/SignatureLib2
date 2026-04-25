using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using SignatureLib.Code.Extensions;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace SignatureLib.Code.Signature;

public class NCardHelper {
	private static Logger Logger { get; } = new(nameof(NCardHelper), LogType.Generic);

	private const float HalfCardSize = 512;

	private readonly NCard _nCard;
	public ModelId? Id => this._nCard.Model?.Id ?? null;

	private Control? _signatureControl;
	private TextureRect? _signatureTextureRect;
	private TextureRect? _textShadow;
	private MegaRichTextLabel? _description;

	private Tween? _tween = null;

	private bool? _hasSignature;

	private float SignatureTransparency => this._textShadow?.Modulate.A ?? 1f;

	private bool _signatureHovered = false;
	public bool SignatureHovered {
		get => this._signatureHovered;
		set {
			if (this._signatureHovered != value) {
				this._signatureHovered = value;

				if (this.Id == null) {
					Logger.Warn("SignatureHovered.set: Id == null");
					return;
				}

				if (!SignatureLib.IsEnabled(this.Id)) {
					Logger.Info($"SignatureHovered.set: Signature of {this.Id.Entry} is disabled!");
					return;
				}

				if (!this._alwaysHovered) {
					this._tween?.Kill();
					if ((this._tween = this._nCard.CreateTween()) != null) {
						this._tween.SetParallel(true);

						float targetAlpha = value ? 1f : 0f;
						float duration = 0.3f * Mathf.Abs(targetAlpha - this.SignatureTransparency);

						this._tween.TweenProperty(this._textShadow,
								"modulate:a",targetAlpha, duration);
						this._tween.TweenProperty(this._description,
								"modulate:a", targetAlpha, duration);

						Logger.VeryDebug("targetAlpha = " + targetAlpha + " duration = " + duration);
					}
				}
			}
		}
	}

	private bool _alwaysHovered = false;
	public bool AlwaysHovered {
		get => this._alwaysHovered;
		set {
			if (this.Id == null) {
				Logger.Warn("AlwaysHovered.set: NCardHelper.Id == null");
				return;
			}

			if (!SignatureLib.IsEnabled(this.Id)) {
				Logger.Info($"AlwaysHovered.set: Signature of {this.Id.Entry} is disabled!!");
				return;
			}

			this._alwaysHovered = value;
			this._tween?.Kill();
			this._tween = null;

			if (this._textShadow == null || this._description == null)
				Logger.Warn("AlwaysHovered.set: TextShadow or description == null");

			this._textShadow?.SetModulate(new Color(1f, 1f, 1f, value ? 1f : 0f));
			this._description?.SetModulate(new Color(1f, 1f, 1f, value ? 1f : 0f));

			this.SignatureHovered = value;
		}
	}

	public NCardHelper(NCard nCard) {
		this._nCard = nCard;
	}

	public void OnReload() {
		// TODO: move from AbstractSignatureCard.OnReload()
	}
}
