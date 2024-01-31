using System.ComponentModel.DataAnnotations;

namespace lostborn_backend.Models
{
    public class IPAccess
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string IP_PLAN { get; set; }

        [Required]
        public int PLAN_YEAR { get; set; }

        [Required]
        public string OPRCLASS { get; set; }

        [Required]
        public int PLAN_STATUS { get; set; }

        [Required]
        public string ACCESS_PAGES { get; set; }

        [Required]
        public string ACCESS_TYPE { get; set; }

        [Required]
        public DateTime FROM_DT { get; set; }

        [Required]
        public DateTime TO_DT { get; set; }

        public IPAccess() { }

        public IPAccess(int ID, string IP_PLAN, int PLAN_YEAR, string OPRCLASS, int PLAN_STATUS, string ACCESS_PAGES, string ACCESS_TYPE, DateTime FROM_DT, DateTime TO_DT)
        {
            this.ID = ID;
            this.IP_PLAN = IP_PLAN;
            this.PLAN_YEAR = PLAN_YEAR;
            this.OPRCLASS = OPRCLASS;
            this.PLAN_STATUS = PLAN_STATUS;
            this.ACCESS_PAGES = ACCESS_PAGES;
            this.ACCESS_TYPE = ACCESS_TYPE;
            this.FROM_DT = FROM_DT;
            this.TO_DT = TO_DT;
        }
    }
}

