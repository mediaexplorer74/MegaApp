// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using MonkeyFinder.Model;
//
//    var monkeys = Monkeys.FromJson(jsonString);

using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MegaApp.Model
{
    public class Url
    {
        public string type { get; set; }
        public string url { get; set; }
    }


    // Special class for SelectedItem manipulations =) 
    public partial class Monkey
    {
        public int id { get; set; }
        public string Text { get; set; }
        public string description { get; set; }
        public string modified { get; set; }
        public string resourceURI { get; set; }
        //public Events events { get; set; }
        public List<Url> urls { get; set; }
    }

    /*
    public partial class Monkey
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Category")]
        public string Category { get; set; }

        [JsonProperty("Headline")]
        public string Headline { get; set; }

        [JsonProperty("Subhead")]
        public string Subhead { get; set; }

        [JsonProperty("DateLine")]
        public string DateLine { get; set; }

        [JsonProperty("Image")]
        public string Image { get; set; }
           
        
    }
    */

    public partial class Monkey
    {
        public static Monkey[] FromJson(string json) => JsonConvert.DeserializeObject<Monkey[]>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Monkey[] self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
