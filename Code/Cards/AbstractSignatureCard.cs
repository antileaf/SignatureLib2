using BaseLib.Abstracts;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Cards;
using SignatureLib.Code.Config;
using SignatureLib.Code.Extensions;
using SignatureLib.Code.Interfaces;
using SignatureLib.Code.Utils;
using Color = Godot.Color;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace SignatureLib.Code.Cards;

public abstract class AbstractSignatureCard(int cost, CardType type, CardRarity rarity, TargetType target) :
		CustomCardModel(cost, type, rarity, target), INCardModify {
	private static Logger Logger { get; } = new(nameof(AbstractSignatureCard), LogType.Generic);

	private const float Size = 512;

	public virtual string SignaturePortraitPath =>
		this.PortraitPath.Replace("/cards/", "/signature/");

	public Texture2D? SignaturePortrait => PreloadManager.Cache.GetTexture2D(this.SignaturePortraitPath);
	public Texture2D SignatureTextBg => PreloadManager.Cache.GetTexture2D("desc_shadow.png".CardItemPath());

	private bool? _hasSignature;
	public bool HasSignature => this._hasSignature ??= ResourceLoader.Exists(this.SignaturePortraitPath);

	public virtual bool SignaturePredicate() => true;
	public bool ShouldUseSignature() => this.HasSignature && this.SignaturePredicate();

	private NCard? _nCard;
	private Control? _signatureControl;
	private TextureRect? _signatureTextureRect;
	private TextureRect? _textShadow;
	private MegaRichTextLabel? _description;

	private Tween? _curTween = null;

	private float SignatureTransparency => this._textShadow?.Modulate.A ?? 1f;

	private bool _signatureHovered = false;
	public bool SignatureHovered {
		get => this._signatureHovered;
		set {
			if (!this.HasSignature)
				return; // TODO: re-write in future

			if (this._signatureHovered == value)
				return;

			this._signatureHovered = value;

			if (!SignatureLibHelper.IsEnabled(this.Id)) {
				Logger.Debug($"Signature of {this.Id.Entry} is disabled");
				return;
			}

			if (this._alwaysHovered)
				Logger.Debug("Tried to set SignatureHovered to " + value + " but _alwaysHovered is true");

			Logger.VeryDebug("Signature hovered changed to " + value);

			if (!this._alwaysHovered) {
				this._curTween?.Kill();
				if ((this._curTween = this._nCard?.CreateTween()) != null) {
					this._curTween.SetParallel(true);

					float targetAlpha = value ? 1f : 0f;
					float duration = 0.3f * Mathf.Abs(targetAlpha - this.SignatureTransparency);

					this._curTween.TweenProperty(this._textShadow,
						"modulate:a",targetAlpha, duration);
					this._curTween.TweenProperty(this._description,
						"modulate:a", targetAlpha, duration);

					Logger.VeryDebug("targetAlpha = " + targetAlpha + " duration = " + duration);
				}
			}
		}
	}

	private bool _alwaysHovered = false;
	public void AlwaysHovered(bool value) {
		if (!this.HasSignature || !SignatureLibHelper.IsEnabled(this.Id))
			return;

		// if (this._alwaysHovered == value)
		// 	return;

		this._alwaysHovered = value;
		this._curTween?.Kill();
		this._curTween = null;

		if (this._textShadow == null)
			Logger.Info("textShadow is null");
		if (this._description == null)
			Logger.Info("description is null");

		this._textShadow?.SetModulate(new Color(1f, 1f, 1f, value ? 1f : 0f));
		this._description?.SetModulate(new Color(1f, 1f, 1f, value ? 1f : 0f));

		this.SignatureHovered = value;
	}

	public virtual void OnReload(NCard card) {
		Logger.Debug($"Card {this.Id.Entry} reloaded");

		this._curTween?.Kill();
		this._curTween = null;

		this._nCard = card;
		Traverse traverse = Traverse.Create(card);

		this._signatureControl = new Control();
		this._signatureControl.Size = new Vector2(Size, Size);
		this._signatureControl.Position = new Vector2(-Size / 2, -Size / 2);
		this._signatureControl.MouseFilter = Control.MouseFilterEnum.Ignore;

		Control cardContainer = traverse.Property("Body").GetValue<Control>();
		cardContainer.AddChildSafely(this._signatureControl);
		cardContainer.MoveChildSafely(this._signatureControl,
			traverse.Field<TextureRect>("_frame").Value.GetIndex() + 1);

		if (this.HasSignature) {
			this._signatureTextureRect = new TextureRect();
			this._signatureTextureRect.Size = new Vector2(Size, Size);
			this._signatureTextureRect.ExpandMode = TextureRect.ExpandModeEnum.FitHeight;
			this._signatureTextureRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
			this._signatureTextureRect.Texture = this.SignaturePortrait;
			this._signatureTextureRect.MouseFilter = Control.MouseFilterEnum.Ignore;

			this._textShadow = new TextureRect();
			this._textShadow.Size = new Vector2(Size, Size);
			this._textShadow.ExpandMode = TextureRect.ExpandModeEnum.FitHeight;
			this._textShadow.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
			this._textShadow.Texture = this.SignatureTextBg;
			this._textShadow.MouseFilter = Control.MouseFilterEnum.Ignore;

			this._signatureControl.AddChildSafely(this._signatureTextureRect);
			this._signatureControl.AddChildSafely(this._textShadow);

			this._description = traverse.Field<MegaRichTextLabel>("_descriptionLabel").Value;

			this.UpdateSignature(SignatureLibHelper.IsEnabled(this.Id));
		}
		else
			Logger.Debug($"Card {this.Id.Entry} does not have Signature. Not modifying.");
	}

	public virtual void UpdateSignature(bool enabled) {
		Traverse traverse = Traverse.Create(this._nCard);

		TextureRect frame = traverse.Field<TextureRect>("_frame").Value;
		TextureRect portraitBorder = traverse.Field<TextureRect>("_portraitBorder").Value;
		TextureRect titleBanner = traverse.Field<TextureRect>("_banner").Value;
		TextureRect portrait = traverse.Field<TextureRect>("_portrait").Value;
		NinePatchRect type = traverse.Field<NinePatchRect>("_typePlaque").Value;

		if (this._description is null)
			Logger.Warn("description is null");

		if (enabled) {
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

		this.AlwaysHovered(SignatureLibConfig.AlwaysShowDescription);
	}
}
