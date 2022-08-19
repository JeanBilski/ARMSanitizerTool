using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ARMSanitizerTool.Models
{
    internal class RootArmObject
    {
        [JsonPropertyName("resources")]
        public List<ResourceObject>? Resources { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }
    }
}
