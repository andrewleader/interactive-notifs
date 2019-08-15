using System;

namespace InteractiveNotifs.Api
{
    public class Device
    {
        public DeviceType Type { get; set; }

        /// <summary>
        /// The push identifier, like the URL to send to
        /// </summary>
        public string Identifier { get; set; }
    }

    public enum DeviceType
    {
        Android,
        iOS,
        Windows,
        Web
    }
}
