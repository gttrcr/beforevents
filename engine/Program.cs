namespace WhaToDo
{
    using Newtonsoft.Json;

    public struct Event
    {
        public string? Title;
        public string? Description;
        public string? Where;
        public DateTime? From;
        public DateTime? To;
        public Uri? Maps;
    }

    public struct Finder
    {
        public string Url;
        public string? Events;
        public string? Title;
        public string? Description;
        public string? Where;
        public string? From;
        public string? To;
    }

    public class WhaToDoMain
    {
        public static void Main(string[] args)
        {
            List<Event> events = new List<Event>();
            List<Finder>? finders = JsonConvert.DeserializeObject<List<Finder>>(File.ReadAllText("db.json"));
            for (int i = 0; i < finders?.Count; i++)
            {
                bool result = true;
                try
                {
                    Extractor.Extract(finders[i], ref events);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    result = false;
                }
                Console.WriteLine(Environment.NewLine + (result ? "Completed without errors" : "Completed WITH ERRORS"));
            }
            string json = JsonConvert.SerializeObject(events, Formatting.Indented);
            File.WriteAllText("api_get_list.json", json);
            Loader.UploadGitHub("api_get_list.json", json);
        }
    }
}