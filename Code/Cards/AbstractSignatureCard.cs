using System.Reflection.Emit;
using BaseLib.Abstracts;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using SignatureLib.Code.Config;
using SignatureLib.Code.Extensions;
using SignatureLib.Code.Utils;
using Color = Godot.Color;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace SignatureLib.Code.Cards;

public abstract class AbstractSignatureCard : CustomCardModel {
	private static Logger Logger { get; } = new(nameof(AbstractSignatureCard), LogType.Generic);

	private static HashSet<ModelId> Checked { get; } = new();

	public virtual string SignaturePortraitPath =>
		this.PortraitPath.Replace("/cards/", "/signature/")
			.Replace(@"\cards\", @"\signature\");

	public virtual bool SignaturePredicate => true;

	public AbstractSignatureCard(int cost, CardType type, CardRarity rarity, TargetType target) :
			base(cost, type, rarity, target) {
		if (!Checked.Contains(this.Id)) {
			Checked.Add(this.Id);

			if (ResourceLoader.Exists(this.SignaturePortraitPath)) {
				SignatureLibHelper.Register(this.Id, new SignatureInfo()
						.Portrait(_ => this.SignaturePortraitPath)
						.Predicate(_ => this.SignaturePredicate));
			}
			else {
				Logger.Warn($"Card {this.Id.Entry} does not have a signature portrait at " +
				            $"{this.SignaturePortraitPath}. Skipped registration.");
			}
		}
	}
}
