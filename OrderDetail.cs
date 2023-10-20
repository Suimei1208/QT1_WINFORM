namespace QT1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrderDetail")]
    public partial class OrderDetail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public int? OrderID { get; set; }

        public int? ItemID { get; set; }

        public int? Quantity { get; set; }

        public double? UnitAmount { get; set; }

        public virtual Item Item { get; set; }

        public virtual Order Order { get; set; }
    }
}
