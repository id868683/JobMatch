using JobMatch.Data;

namespace JobMatch.Models
{
    public class Job
    {
       
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Salary { get; set; }
        public string Image {  get; set; }

        public int CompanyId { get; set; }  //foreign key
        public Company Company { get; set; }

    }
}
