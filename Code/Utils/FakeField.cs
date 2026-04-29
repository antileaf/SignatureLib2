namespace SignatureLib.Code.Utils;

public class FakeField<TKey, TVal> {
	private readonly Func<TKey, TVal> _getter;

	public FakeField(Func<TKey, TVal> getter) {
		this._getter = getter;
	}

	public TVal this[TKey key] => this._getter.Invoke(key);
}
