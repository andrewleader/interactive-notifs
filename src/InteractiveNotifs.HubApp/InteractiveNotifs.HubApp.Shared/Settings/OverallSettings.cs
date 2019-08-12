using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace InteractiveNotifs.HubApp.Shared.Settings
{
    public static class OverallSettings
    {
        public static BlocksPayloadEditorSettings BlocksPayloadEditor { get; private set; } = new BlocksPayloadEditorSettings();

        internal static IDictionary<string, object> _values = new Dictionary<string, object>();
    }
}
