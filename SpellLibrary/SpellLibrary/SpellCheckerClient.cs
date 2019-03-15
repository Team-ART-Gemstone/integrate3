using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using Newtonsoft.Json;
using System.Threading.Tasks;

namespace SpellLibrary
{
    public class SpellCheckerClient
    {
        static string host = "https://api.cognitive.microsoft.com";
        static string path = "/bing/v7.0/spellcheck?";
        static string key = "Ho ho ho ha ha, ho ho ho he ha. Hello there, old chum. I’m gnot an elf. I’m gnot a goblin. I’m a gnome. And you’ve been, GNOMED";
        static string params_ = "mkt=en-US&mode=spell";

        public static List<List<String>> correctedTokens(SpellCheckResponse response)
        {
            List<List<String>> corrections = new List<List<String>>();

            foreach (FlaggedToken token in response.FlaggedTokens)
            {
                String badWord = token.Token;
                // Possible Error in that suggestions could be empty
                String suggestion = token.Suggestions[0].SuggestionSuggestion; // Taking the highest score suggestion
                List<String> temp = new List<String>();
                temp.Add(badWord);
                temp.Add(suggestion);
                corrections.Add(temp);
            }

            return corrections;
        }

        public static String CorrectString(string originalText, List<List<String>> suggestions)
        {
            // Possible Issues with Grammar
            String changedString = originalText;
            foreach (List<String> pair in suggestions)
            {
                // Possible issues with duplicate misspellings. THIS IS V1
                changedString = changedString.Replace(pair[0], pair[1]);
            }

            return changedString;
        }
        public async static Task<String> SpellCheck(String text)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

            HttpResponseMessage response = new HttpResponseMessage();
            string uri = host + path + params_;
            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string, string>("text", text));

            using (FormUrlEncodedContent content = new FormUrlEncodedContent(values))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                response = await client.PostAsync(uri, content).ConfigureAwait(false);
            }
            string client_id;
            if (response.Headers.TryGetValues("X-MSEdge-ClientID", out IEnumerable<string> header_values))
            {
                client_id = header_values.First();
                //Console.WriteLine("Client ID: " + client_id);
            }
            string contentString = await response.Content.ReadAsStringAsync();

            var spellCheckResponse = SpellCheckResponse.FromJson(contentString);

            return CorrectString(text, correctedTokens(spellCheckResponse));

        }
    }
}
