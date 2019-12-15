using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeRun.Models
{
    public class CurrentOrder
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public string CoffeeSize { get; set; }
        public string CoffeeType { get; set; }
        public bool Paid { get; set; }
    }
}
