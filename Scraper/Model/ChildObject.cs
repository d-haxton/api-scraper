using System;

namespace Scraper.Model
{
    public abstract class ChildObject : BaseObject, IComparable<ChildObject>
    {
        public override string Id
        {
            get => base.Id;
            set
            {
                if (Id != value)
                {
                    if (Id != null)
                    {
                        throw new InvalidOperationException("An object may not have one Uid and then be assigned a new one.");
                    }

                    base.Id = value;
                }
            }
        }

        public int CompareTo(ChildObject obj)
        {
            return string.Compare(Id, obj.Id, StringComparison.Ordinal);
        }
    }
}
