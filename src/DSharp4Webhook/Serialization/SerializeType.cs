using System;

namespace DSharp4Webhook.Serialization
{
    /// <summary>
    ///     Data serialization types for discord.
    /// </summary>
    public enum SerializeType
    {
        MULTIPART_FORM_DATA,
        APPLICATION_JSON
    }

    public static class SerializeTypeConverter
    {
        /// <exception cref="InvalidOperationException">
        ///     This type was not defined.
        /// </exception>
        public static string Convert(SerializeType type)
        {
            return type switch
            {
                SerializeType.APPLICATION_JSON => "application/json",
                SerializeType.MULTIPART_FORM_DATA => "multipart/form-data",
                _ => throw new InvalidOperationException("This type was not implemented"),
            };
        }
    }
}
