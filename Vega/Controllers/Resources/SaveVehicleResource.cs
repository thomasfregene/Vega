using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Vega.Controllers.Resources
{
    public class SaveVehicleResource
    {
        public int Id { get; set; }

        //foreign key property
        public int ModelId { get; set; }

        //public Model Model { get; set; }//delete

        public bool IsRegistered { get; set; }

        [Required]
        public ContactResource Contact { get; set; }

       // public DateTime? LastUpdate { get; set; } delete

        public ICollection<int> Features { get; set; }

        public SaveVehicleResource()
        {
            Features = new Collection<int>();
        }

    }
}
