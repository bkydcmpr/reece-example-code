using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Reece.Example.MVC.UnitTesting.Models
{
    public class Prospect
    {
        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Phone")]
        public string Phone { get; set; }

        public Guid ID { get; set; }
    }
}