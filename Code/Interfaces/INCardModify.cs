using MegaCrit.Sts2.Core.Nodes.Cards;

namespace SignatureLib.Code.Interfaces;

// Thanks to 诗音!
public interface INCardModify {
	bool AllowNodePool => false;

	void OnReload(NCard card);
}
