using PayLend.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayLend.Core.Entities
{
    /// <summary>
    /// Classe base para os OverduePayments
    /// Loans que entram em incumprimentos
    /// </summary>
    public class OverduePayments : BaseEntity
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int LoanID { get; set; }

        [ForeignKey("LoanID")]
        public virtual Loans Loans { get; set; }

        public decimal NewDailyFee { get; set; }

        public OverduePaymentsStatusEnum Status { get; set; }

        public int ReasonID { get; set; }

        [ForeignKey("ReasonID")]
        public virtual OverduePaymentsReason OverduePaymentsReason { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateDate { get; set; }

        public int UserIDCreate { get; set; }

        public DateTime? ChangeDateTime { get; set; }

        public int UserIDChange { get; set; }

        public decimal NewDailyFeePer1000 { get; set; }

        [NotMapped]
        public int DelayDays { get; set; }
    }
}
