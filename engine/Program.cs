namespace Beforevents
{
    using Newtonsoft.Json;
    using System.Linq;

    public class EngineMain
    {
        public static World<Event> CycleAll(World<Finder> world)
        {
            World<Event> ret = new World<Event>();

            //List<Event> events = new List<Event>();
            //List<Finder>? finders = JsonConvert.DeserializeObject<List<Finder>>(File.ReadAllText("db.json"));
            //for (int i = 0; i < finders?.Count; i++)
            //{
            //    bool result = true;
            //    try
            //    {
            //        Extractor.Extract(finders[i], ref events);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //        result = false;
            //    }
            //    Console.WriteLine(Environment.NewLine + (result ? "Completed without errors" : "Completed WITH ERRORS"));
            //}

            return ret;
        }

        public static void Main(string[] args)
        {
            World<Finder> world = new World<Finder>();
            world.Nations = new List<Nation<Finder>>();
            world.Finders = new List<Finder>();
            Directory.GetDirectories("world").ToList().ForEach(x =>
            {
                List<District<Finder>> districts = new List<District<Finder>>();
                Directory.GetDirectories(x).ToList().ForEach(y => districts.Add(new District<Finder>() { Name = y, Finders = JsonConvert.DeserializeObject<List<Finder>>(File.ReadAllText(y + "\\db.json")) }));
                List<Finder> finders = new List<Finder>();
                if (File.Exists(x + "\\db.json"))
                    finders = JsonConvert.DeserializeObject<List<Finder>>(File.ReadAllText(x + "\\db.json"));
                world.Nations.Add(new Nation<Finder>() { Name = x, Districts = districts, Finders = finders });
            });
            if (File.Exists("world\\db.json"))
                world.Finders.AddRange(JsonConvert.DeserializeObject<List<Finder>>(File.ReadAllText("world\\db.json")));

            World<Event> events = CycleAll(world);

            //JObj obj = new JObj();
            //obj.Events = events;
            //obj.LastUpdate = DateTime.Now.ToString();
            Loader l = new Loader();
            l.UploadGitHub(JsonConvert.SerializeObject(events, Formatting.Indented));
        }
    }
}