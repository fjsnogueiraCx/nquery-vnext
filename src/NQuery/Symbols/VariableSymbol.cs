using System;

namespace NQuery.Symbols
{
    public class VariableSymbol : Symbol
    {
        private readonly Type _type;

        private object _value;

        public VariableSymbol(string name, Type type)
            : this(name, type, null)
        {
        }

        public VariableSymbol(string name, Type type, object value)
            : base(name)
        {
            _type = type;
            Value = value;
        }

        public override SymbolKind Kind
        {
            get { return SymbolKind.Variable; }
        }

        public override Type Type
        {
            get { return _type; }
        }

        public object Value
        {
            get { return _value; }
            set
            {
                if (value != null && !_type.IsInstanceOfType(value))
                    throw new ArgumentException(string.Format("The value '{0}' cannot be assigned to variable of type {1}", value, _type), "value");

                _value = value;
            }
        }
    }
}