using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;

namespace GoldenV.Models;

public partial class Servicio
{
    public int IdServicio { get; set; }

    [Required]
    public int? IdTipoServicio { get; set; }

    [Required]
    public string? NomServicio { get; set; }

    [Required]
    public decimal Costo { get; set; }

    [Required]
    public bool? Estado { get; set; }

    public virtual ICollection<DetalleReservaServicio> DetalleReservaServicios { get; set; } = new List<DetalleReservaServicio>();

    public virtual TipoServicio? IdTipoServicioNavigation { get; set; }

    public virtual ICollection<ImagenServicio> ImagenServicios { get; set; } = new List<ImagenServicio>();

    public virtual ICollection<PaqueteServicio> PaqueteServicios { get; set; } = new List<PaqueteServicio>();
}
