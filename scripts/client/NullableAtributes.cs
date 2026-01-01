namespace System.Runtime.CompilerServices {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method |
                    AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Event |
                    AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false)]
    internal sealed class NullableAttribute : Attribute {
        public NullableAttribute(byte b) { }
        public NullableAttribute(byte[] b) { }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    internal sealed class NullableContextAttribute : Attribute {
        public NullableContextAttribute(byte b) { }
    }
}
