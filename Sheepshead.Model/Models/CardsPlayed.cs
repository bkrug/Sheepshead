
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
    
public partial class CardsPlayed
{

    public int PlayerId { get; set; }

    public int TrickId { get; set; }

    public string Card { get; set; }

    public int SortOrder { get; set; }



    public virtual Player Player { get; set; }

    public virtual Trick Trick { get; set; }

}

}
