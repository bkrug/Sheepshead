﻿

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
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


public partial class SheepsheadEntities : DbContext
{
    public SheepsheadEntities()
        : base("name=SheepsheadEntities")
    {

    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        throw new UnintentionalCodeFirstException();
    }


    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Hand> Hands { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Trick> Tricks { get; set; }

    public virtual DbSet<CardsPlayed> CardsPlayeds { get; set; }

}

}

