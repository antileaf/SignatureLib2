using MegaCrit.Sts2.Core.Nodes;

namespace SignatureLib.Code.Extensions;

public static class GodotExtensions {
	public static void MoveChildSafely(this Godot.Node node, Godot.Node child, int index) {
		if (NGame.IsMainThread())
			node.MoveChild(child, index);
		else
			node.CallDeferred(Godot.Node.MethodName.MoveChild, child, index);
	}

	public static void ReparentSafely(this Godot.Node node, Godot.Node newParent) {
		if (NGame.IsMainThread())
			node.Reparent(newParent);
		else
			node.CallDeferred(Godot.Node.MethodName.Reparent, newParent);
	}
}
