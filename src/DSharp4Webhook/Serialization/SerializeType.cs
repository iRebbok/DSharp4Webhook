using System;

namespace DSharp4Webhook.Serialization
{
    /// <summary>
    ///     Data serialization types for discord.
    /// </summary>
    public enum SerializeType
    {
        MULTIPART_FROM_DATA,
        APPLICATION_JSON
    }

    public static class SerializeTypeConverter
    {
        /// <exception cref="InvalidOperationException">
        ///     This type was not defined.
        /// </exception>
        public static string Convert(SerializeType type)
        {
            switch (type)
            {
                case SerializeType.APPLICATION_JSON:
                    return "application/json";
                case SerializeType.MULTIPART_FROM_DATA:
                    return "multipart/form-data";
            }

            throw new InvalidOperationException("This type was not implemented");
        }
    }
}
