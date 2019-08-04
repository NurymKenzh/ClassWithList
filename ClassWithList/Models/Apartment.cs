using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClassWithList.Models
{
    public class Apartment
    {
        public int Id { get; set; }
        [Display(Name = "Number")]
        public int Number { get; set; }
        [Display(Name = "Rooms")]
        public IList<Room> Rooms { get; set; }
    }

    public class Room
    {
        public int Id { get; set; }
        public int ApartmentId { get; set; }
        public Apartment Apartment { get; set; }
        [Display(Name = "Type")]
        public string Type { get; set; }
        [Display(Name = "Area")]
        public double Area { get; set; }
    }
}