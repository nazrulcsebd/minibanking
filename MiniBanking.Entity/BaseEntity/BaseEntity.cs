using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBanking.Entity
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        //public Guid CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        //public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
