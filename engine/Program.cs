﻿namespace Beforevents
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Linq;

    public struct Nation
    {
        public string Name;
        public List<District> Districts;
    }

    public struct District
    {
        public string Name;
        public List<Event> Events;
    }

    public struct Event
    {
        public string? Title;
        public string? Description;
        public string? Where;
        public string? From;
        public string? To;
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

    public struct JObj
    {
        public List<Event> Events;
        public string LastUpdate;
    }

    public class EngineMain
    {
        public static void Main(string[] args)
        {
            List<Nation> world = new List<Nation>();
            Directory.GetDirectories("world").ToList().ForEach(x => world.Add(new Nation() { Name = x, Districts = new List<District>() }));
            for (int i = 0; i < world.Count; i++)
            {
                dynamic d = JObject.Parse(File.ReadAllText(world[i].Name + "\\districts.json"));
                world[i].Districts = new List<District>(); //d.Districts.Select(y => new District() { Name = y.Name, Events = new List<Event>() });
            }

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

            JObj obj = new JObj();
            obj.Events = events;
            obj.LastUpdate = DateTime.Now.ToString();
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            Loader l = new Loader();
            l.UploadGitHub(json);
        }
    }
}