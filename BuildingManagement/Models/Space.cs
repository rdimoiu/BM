using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuildingManagement.Models
{
    public class Space
    {
        public int ID { get; set; }

        [StringLength(30)]
        public string Number { get; set; }

        [Range(0, 9999999999999999.99)]
        public decimal Surface { get; set; }

        public int People { get; set; }
        
        public bool Inhabited { get; set; }

        [NotMapped]
        public int ClientID { get; set; }

        [NotMapped]
        public Client Client { get; set; }

        [NotMapped]
        public int SectionID { get; set; }

        [NotMapped]
        public Section Section { get; set; }

        public int LevelID { get; set; }

        public Level Level { get; set; }
        
        public int SpaceTypeID { get; set; }

        public virtual SpaceType SpaceType { get; set; }
        
        public int SubClientID { get; set; }

        public virtual SubClient SubClient { get; set; }

        public virtual ICollection<Meter> Meters { get; set; }

        public virtual ICollection<SubMeter> SubMeters { get; set; }

        public virtual ICollection<SubSubMeter> SubSubMeters { get; set; }

        public virtual ICollection<Service> Services { get; set; }
    }
}