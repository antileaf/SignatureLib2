using System.Reflection;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Pooling;
using SignatureLib.Code.Extensions;
using SignatureLib.Code.Interfaces;
using SignatureLib.Code.Signature;
using SignatureLib.Code.Utils;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace SignatureLib.Code.Patches;

// Thanks to 诗音!
public class NCardPatch {
	private static Logger Logger { get; } = new(nameof(NCardPatch), LogType.Generic);

	public static readonly SpireField<NCard, NCardHelper?> Helper = new(() => null);

	[HarmonyPatch(typeof(NCard),"Reload")]
	public static class ReloadPatch {
		[HarmonyPostfix]
		public static void Postfix(NCard inst) {
			if (!inst.IsNodeReady() || inst.Model == null)
				return;

			if (SignatureHelper.IsRegistered(inst.Model.Id)) {
				Logger.Debug($"Running Reload for card {inst.Model.Id}");

				Helper[inst] ??= new NCardHelper(inst);
				Helper[inst]?.OnReload();
			}
		}
	}

	// [HarmonyPatch(typeof(GodotTreeExtensions),nameof(GodotTreeExtensions.QueueFreeSafely))]
	// public static class QueueFreeSafelyPatch {
	// 	[HarmonyPrefix]
	// 	public static bool Prefix(Godot.Node node) {
	// 		if (node is NCard { Model: INCardModify { AllowNodePool: false } }) {
	// 			node.QueueFreeSafelyNoPool();
	// 			return false;
	// 		}
	// 		return true;
	// 	}
	// }

	// [HarmonyPatch(typeof(NCard), nameof(NCard.Model), MethodType.Setter)]
	// public static class NCardModelSetPatch {
	// 	[HarmonyPrefix]
	// 	public static bool Prefix(NCard inst, ref CardModel ____model, CardModel value) {
	// 		if (____model != value && ____model is INCardModify modify) {
	// 			try {
	// 				AccessTools.Method(typeof(NCard), "UnsubscribeFromModel").Invoke(inst, [____model]);
	// 				NCard nc = NodePool.Get<NCard>();
	// 				if (nc.Body == null)
	// 					nc._Ready();
	//
	// 				Control control = nc.Body;
	// 				Vector2 t = inst.Body.Position;
	// 				inst.Body.Free();
	// 				control.ReparentSafely(inst);
	// 				control.Position = t;
	// 				____model = nc.Model;
	// 				nc.QueueFreeSafelyNoPool();
	// 				SetUniqueNameToOwner(control, inst);
	// 				inst._Ready();
	// 			}
	// 			catch (Exception e) {
	// 				MainFile.Logger.Warn(e.ToString());
	// 			}
	// 		}
	//
	// 		return true;
	// 	}
	// }

	// private static void SetUniqueNameToOwner(Node node, Node parent) {
	// 	node.UniqueNameInOwner = true;
	// 	node.Owner = parent;
	// 	foreach (Node child in node.GetChildren()) {
	// 		SetUniqueNameToOwner(child, parent);
	// 	}
	// }
}
