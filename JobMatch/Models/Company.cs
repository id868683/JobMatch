using System.Collections.Generic;

namespace JobMatch.Models
{
    public class Company
    {

        public int Id { get; set; }  //primary key
        public string Name { get; set; }

        public ICollection<Job> Jobs { get; set; }
    }
}
