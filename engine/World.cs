namespace Beforevents
{
    public struct World<T>
    {
        public List<Nation<T>> Nations { get; set; }
        public List<T> Finders { get; set; }
    }

    public struct Nation<T>
    {
        public string Name { get; set; }
        public List<District<T>> Districts { get; set; }
        public List<T> Finders { get; set; }
    }

    public struct District<T>
    {
        public string Name { get; set; }
        public List<T> Finders { get; set; }
    }
}