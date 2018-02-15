namespace Scraper.Model
{
    public abstract class BaseObject
    {
        public virtual string Id { get; set; }

        public override string ToString()
        {
            return $"{GetType().Name}: {Id}";
        }
    }
}