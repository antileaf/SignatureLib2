using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Pooling;
using SignatureLib.Code.Extensions;
using SignatureLib.Code.Interfaces;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace SignatureLib.Code.Patches;

// Thanks to 诗音!
public class NCardModifyPatch {
	private static Logger Logger { get; } = new Logger("NCardModifyPatch", LogType.Generic);

	[HarmonyPatch(typeof(NCard),"Reload")]
	public static class ReloadPatch {
		[HarmonyPostfix]
		public static void Postfix(NCard __instance) {
			if (!__instance.IsNodeReady())
				return;

			if (__instance.Model is INCardModify nCardCreate) {
				Logger.Debug($"Running Reload for card {__instance.Model.Id}");

				nCardCreate.OnReload(__instance);
			}
		}
	}
	
	//-----------------------------------------------------------------------------------------------
	[HarmonyPatch(typeof(GodotTreeExtensions),nameof(GodotTreeExtensions.QueueFreeSafely))]
	public static class QueueFreeSafelyPatch {
		[HarmonyPrefix]
		public static bool Prefix(Godot.Node node) {
			if (node is NCard { Model: INCardModify { AllowNodePool: false } }) {
				node.QueueFreeSafelyNoPool();
				return false;
			}
			return true;
		}
	}
	// [HarmonyPatch(typeof(NInspectCardScreen),"SetCard")]
	// public static class SetCardPatch
	// {
	//	 [HarmonyPrefix]
	//	 public static bool Prefix(NInspectCardScreen __instance,ref NCard ____card,Vector2 ____cardPosition)
	//	 {
	//		 if (____card.Model is INCardModify modify)
	//		 {
	//			 Godot.Node parent = ____card.GetParent();
	//			 int index = ____card.GetIndex();
	//			 ____card.QueueFreeSafelyNoPool();
	//			 NCard nc=NodePool.Get<NCard>();
	//			 parent.AddChildSafely(nc);
	//			 parent.MoveChild(nc, index);
	//			 nc.Position = ____cardPosition;
	//			 ____card = nc;
	//		 }
	//		 return  true;
	//	 }
	//
	// }
	[HarmonyPatch(typeof(NCard),nameof(NCard.Model), MethodType.Setter)]
	public static class NCardModelSetPatch {
		[HarmonyPrefix]
		public static bool Prefix(NCard __instance, ref CardModel ____model, CardModel value) {
			if (____model != value && ____model is INCardModify modify) {
				try {
					AccessTools.Method(typeof(NCard), "UnsubscribeFromModel").Invoke(__instance, [____model]);
					NCard nc = NodePool.Get<NCard>();
					if (nc.Body == null)
						nc._Ready();
					
					Godot.Control control = nc.Body;
					Godot.Vector2 t = __instance.Body.Position;
					__instance.Body.Free();
					control.ReparentSafely(__instance);
					control.Position = t;
					____model = nc.Model;
					nc.QueueFreeSafelyNoPool();
					SetUniqueNameToOwner(control, __instance);
					__instance._Ready();
				}
				catch (Exception e) {
					MainFile.Logger.Warn(e.ToString());
				}
			}

			return true;
		}
	}

	private static void SetUniqueNameToOwner(Godot.Node node, Godot.Node parent) {
		node.UniqueNameInOwner = true;
		node.Owner = parent;
		foreach (Godot.Node child in node.GetChildren()) {
			SetUniqueNameToOwner(child, parent);
		}
	}
}
