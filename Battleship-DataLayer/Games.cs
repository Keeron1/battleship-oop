//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Battleship_DataLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class Games
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Games()
        {
            this.Attacks = new HashSet<Attacks>();
            this.GameShipConfigurations = new HashSet<GameShipConfigurations>();
        }
    
        public int ID { get; set; }
        public string Title { get; set; }
        public string CreatorFK { get; set; }
        public string OpponentFK { get; set; }
        public bool Complete { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Attacks> Attacks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GameShipConfigurations> GameShipConfigurations { get; set; }
        public virtual Players Players { get; set; }
        public virtual Players Players1 { get; set; }
    }
}
