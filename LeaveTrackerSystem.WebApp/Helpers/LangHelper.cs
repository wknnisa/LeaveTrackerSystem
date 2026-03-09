using System.Text.Json;

namespace LeaveTrackerSystem.WebApp.Helpers
{
    public static class LangHelper
    {
        private static Dictionary<string, Dictionary<string, string>> _cache = new();

        public static string Get(HttpContext context, string key)
        {
            var lang = context.Session.GetString("Lang") ?? "EN";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", $"lang_{lang.ToLower()}.json");
            
            if (!_cache.ContainsKey(lang))
            {
                _cache.Clear();

                if (!File.Exists(filePath))
                {
                    return key;
                }

                try
                {
                    var json = File.ReadAllText(filePath);
                    var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                    _cache[lang] = dict;
                }
                catch
                {
                    return key;
                }
            }

            var langDict = _cache[lang];
            return langDict.TryGetValue(key, out var value) ? value : key;
        }
    }
}