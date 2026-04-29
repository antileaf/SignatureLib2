using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using SignatureLib.Code.Config;
using SignatureLib.Code.Extensions;
using SignatureLib.Code.Patches;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace SignatureLib.Code.Signature;

public class NCardHelper {
	private static Logger Logger { get; } = new(nameof(NCardHelper), LogType.Generic);

	private const float HalfCardSize = 512;

	private readonly NCard _nCard;
	public ModelId? Id => this._nCard.Model?.Id ?? null;
	public CardModel? Model => this._nCard.Model;

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
		this._tween?.Kill();
		this._tween = null;

		if (this.Model == null) {
			Logger.Warn("OnReload: Model == null");
			return;
		}

		Traverse traverse = Traverse.Create(this._nCard);

		this._signatureControl = new Control();
		this._signatureControl.Name = "SignatureControl";
		this._signatureControl.Size = new Vector2(HalfCardSize, HalfCardSize);
		this._signatureControl.Position = new Vector2(-HalfCardSize / 2, -HalfCardSize / 2);
		this._signatureControl.MouseFilter = Control.MouseFilterEnum.Ignore;

		Control cardContainer = traverse.Property("Body").GetValue<Control>();
		cardContainer.AddChildSafely(this._signatureControl);
		cardContainer.MoveChildSafely(this._signatureControl,
				traverse.Field<TextureRect>("_frame").Value.GetIndex() + 1);

		// if (CardModelHelper.Get(this.Model) == null) {
		// 	Logger.Warn($"OnReload: CardModelPatch.Helper[{this.Model.Id.Entry}] == null");
		// 	return;
		// }

		this._signatureTextureRect = new TextureRect();
		this._signatureTextureRect.Size = new Vector2(HalfCardSize, HalfCardSize);
		this._signatureTextureRect.ExpandMode = TextureRect.ExpandModeEnum.FitHeight;
		this._signatureTextureRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
		this._signatureTextureRect.Texture = CardModelHelper.Get(this.Model).SignaturePortrait;
		this._signatureTextureRect.MouseFilter = Control.MouseFilterEnum.Ignore;

		this._textShadow = new TextureRect();
		this._textShadow.Size = new Vector2(HalfCardSize, HalfCardSize);
		this._textShadow.ExpandMode = TextureRect.ExpandModeEnum.FitHeight;
		this._textShadow.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
		this._textShadow.Texture = CardModelHelper.Get(this.Model).SignatureTextBg;
		this._textShadow.MouseFilter = Control.MouseFilterEnum.Ignore;

		this._signatureControl.AddChildSafely(this._signatureTextureRect);
		this._signatureControl.AddChildSafely(this._textShadow);

		this._description = traverse.Field<MegaRichTextLabel>("_descriptionLabel").Value;

		TextureRect frame = traverse.Field<TextureRect>("_frame").Value;
		TextureRect portraitBorder = traverse.Field<TextureRect>("_portraitBorder").Value;
		TextureRect titleBanner = traverse.Field<TextureRect>("_banner").Value;
		TextureRect portrait = traverse.Field<TextureRect>("_portrait").Value;
		NinePatchRect type = traverse.Field<NinePatchRect>("_typePlaque").Value;

		if (this._description is null)
			Logger.Warn("description is null");

		if (CardModelHelper.Get(this.Model).Enabled) {
			frame.Hide();
			portraitBorder.Hide();
			titleBanner.Hide();
			portrait.Hide();
			type.SetPosition(new Vector2(type.Position.X, 176.0f));
			this._signatureControl?.Show();
			this._textShadow?.SetModulate(new Color(1f, 1f, 1f, 0f));
			this._description?.SetModulate(new Color(1f, 1f, 1f, 0f));
		}
		else {
			frame.Show();
			portraitBorder.Show();
			titleBanner.Show();
			portrait.Show();
			type.SetPosition(new Vector2(type.Position.X, 1f));
			this._signatureControl?.Hide();
			this._description?.SetModulate(new Color(1f, 1f, 1f, 1f));
		}

		// Logger.Info("Hello?");

		this.AlwaysHovered = SignatureLibConfig.AlwaysShowDescription;
	}
}
