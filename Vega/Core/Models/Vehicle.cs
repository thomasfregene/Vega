using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vega.Core.Models
{
    [Table("Vehicles")]
    public class Vehicle
    {
        public int Id { get; set; }

        //foreign key property
        public int ModelId { get; set; }

        public Model Model { get; set; }

        public bool IsRegistered { get; set; }
        
        //contact info
        [Required]
        [StringLength((255))]
        public string ContactName { get; set; }

        [StringLength(255)]
        public string ContactEmail { get; set; }

        [Required]
        [StringLength(255)]
        public string ContactPhone { get; set; }

        public DateTime? LastUpdate { get; set; }

        public ICollection<VehicleFeature> Features { get; set; }

        public ICollection<Photo> Photos { get; set; }


        //always initialize collection properties
        public Vehicle()
        {
            Features = new Collection<VehicleFeature>();

            Photos = new Collection<Photo>();
        }
    }
}
