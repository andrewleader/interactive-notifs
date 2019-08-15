using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InteractiveNotifs.HubBlazorApp.ViewModels
{
    public static class DefaultBlocks
    {
        public const string Default = @"{
    ""view"": {
        ""content"": {
            ""text"": [
                ""New expense report"",
                ""$34.21 from Thomas Fennel""
            ],
            ""actions"": [
                {
                    ""type"": ""Action"",
                    ""title"": ""View"",
                    ""command"": {
                        ""type"": ""Command.OpenUrl"",
                        ""url"": ""https://templates.office.com/en-us/expense-report-with-mileage-tm00000029""
                    }
                },
                {
                    ""type"": ""Action"",
                    ""title"": ""Approve"",
                    ""command"": {
                        ""type"": ""Command.Http"",
                        ""url"": ""https://raw.githubusercontent.com/microsoft/AdaptiveCards/master/THIRD-PARTY-NOTICES.TXT""
                    }
                },
                {
                    ""type"": ""Action"",
                    ""title"": ""Decline"",
                    ""command"": {
                        ""type"": ""Command.Http"",
                        ""url"": ""https://github.com/microsoft/AdaptiveCards/blob/master/.editorconfig""
                    }
                }
            ]
        }
    }
}";
    }
}
