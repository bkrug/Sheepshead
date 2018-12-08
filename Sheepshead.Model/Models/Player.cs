
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Sheepshead.Model.Models
{

using System;
    using System.Collections.Generic;
    
public partial class Player
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Player()
    {

        this.Hands = new HashSet<Hand>();

        this.Hands1 = new HashSet<Hand>();

        this.Hands2 = new HashSet<Hand>();

        this.Tricks = new HashSet<Trick>();

        this.CardsPlayed = new HashSet<CardsPlayed>();

    }


    public int Id { get; set; }

    public string Name { get; set; }

    public string Cards { get; set; }

    public System.Guid GameId { get; set; }



    public virtual Game Game { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Hand> Hands { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Hand> Hands1 { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Hand> Hands2 { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Trick> Tricks { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<CardsPlayed> CardsPlayed { get; set; }

}

}
