using System;

namespace DSharp4Webhook.Serialization
{
    /// <summary>
    ///     Data serialization types for discord.
    /// </summary>
    public enum SerializationType
    {
        NULL = 0,
        MULTIPART_FORM_DATA,
        APPLICATION_JSON
    }

    public static class SerializeTypeConverter
    {
        /// <summary>
        ///     Converts the type to a string.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     This type was not defined.
        /// </exception>
        public static string Convert(SerializationType type)
        {
            return type switch
            {
                SerializationType.APPLICATION_JSON => "application/json",
                SerializationType.MULTIPART_FORM_DATA => "multipart/form-data",
                _ => throw new InvalidOperationException("This type was not implemented"),
            };
        }
    }
}
