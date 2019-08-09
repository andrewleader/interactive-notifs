using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InteractiveNotifs.Hub.Models
{
    public class NotificationViewModel
    {
        public string AdaptiveBlock { get; set; } = @"{
  ""view"": {
    ""content"": {
      ""text"": [
        ""Sample notification"",
        ""This is a basic notification""
      ]
    }
  }
}";
    }
}
