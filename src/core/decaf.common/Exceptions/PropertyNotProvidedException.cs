using System;
using System.Runtime.Serialization;

namespace decaf.common.Exceptions
{
    [Serializable]
    public class PropertyNotProvidedException : InvalidOperationException
	{
		public PropertyNotProvidedException(string property)
            : base(property) { }

        public PropertyNotProvidedException(string property, string reason)
            : base(GetMessage(property, reason)) { }
        
        public PropertyNotProvidedException(string property, Exception innerException, string reason = null)
            : base(GetMessage(property, reason), innerException) { }
        
        public PropertyNotProvidedException(Exception innerException, string reason = null)
            : base(reason, innerException) { }


        protected PropertyNotProvidedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        private static string GetMessage(string property, string reason)
        {
            var msg = $"Property \"{property}\" is required for this operation.";
            if (!string.IsNullOrWhiteSpace(reason))
                msg = $"{msg} {reason}";
            return msg;
        }
    }
}

