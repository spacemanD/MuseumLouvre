using System;
using System.Collections.Generic;

namespace DAL.EF.Entities
{
    public class Collection
    {
        public int CollectionId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public List<Exhibit> Exhibits { get; set; }
    }
}

