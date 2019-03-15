using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SpellLibrary
{
    public partial class SpellCheckResponse
    {
        [JsonProperty("_type")]
        public string Type { get; set; }

        [JsonProperty("flaggedTokens")]
        public List<FlaggedToken> FlaggedTokens { get; set; }

        [JsonProperty("correctionType")]
        public string CorrectionType { get; set; }
    }

    public partial class FlaggedToken
    {
        [JsonProperty("offset")]
        public long Offset { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("suggestions")]
        public List<Suggestion> Suggestions { get; set; }
    }

    public partial class Suggestion
    {
        [JsonProperty("suggestion")]
        public string SuggestionSuggestion { get; set; }

        [JsonProperty("score")]
        public long Score { get; set; }
    }

    public partial class SpellCheckResponse
    {
        public static SpellCheckResponse FromJson(string json) => JsonConvert.DeserializeObject<SpellCheckResponse>(json, SpellLibrary.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this SpellCheckResponse self) => JsonConvert.SerializeObject(self, SpellLibrary.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
