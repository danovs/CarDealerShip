//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CarDealerShip
{
    using System;
    using System.Collections.Generic;
    
    public partial class catalog
    {
        public int catalog_id { get; set; }
        public int car_id { get; set; }
        public Nullable<int> inventory_id { get; set; }
    
        public virtual car car { get; set; }
        public virtual inventory inventory { get; set; }
    }
}
