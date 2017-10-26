using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingManagement.Models
{
    public class Space
    {
        private Level _level;
        private int _people;

        public int ID { get; set; }

        [Required]
        [StringLength(30)]
        public string Number { get; set; }

        [Range(0, 9999999999999999.99)]
        public decimal Surface { get; set; }

        public int People
        {
            get { return _people; }
            set
            {
                _people = value;
                if (_level != null)
                {
                    _level.People += People;
                }
            }
        }

        public bool Inhabited { get; set; }

        public int LevelID { get; set; }

        public Level Level
        {
            get { return _level; }
            set
            {
                _level = value;
                if(_level != null)
                    ClientID = _level.ClientID;
            }
        }

        public int SpaceTypeID { get; set; }
        public virtual SpaceType SpaceType { get; set; }

        public int ClientID { get; set; }
        public virtual Client Client { get; set; }

        public int SubClientID { get; set; }
        public virtual SubClient SubClient { get; set; }

        public virtual ICollection<Meter> Meters { get; set; }

        public virtual ICollection<SubMeter> SubMeters { get; set; }

        public virtual ICollection<Service> Services { get; set; }
    }
}