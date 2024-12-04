using Microsoft.AspNetCore.Mvc.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GoldenV.Models;

public partial class Reserva
{
    public int IdReserva { get; set; }

    [Required]
    public int? IdCliente { get; set; }

    [Required]
    public DateTime? FechaReserva { get; set; } = DateTime.Now;

    [Required]
    public DateTime? FechaInicio { get; set; }

    [Required]
    public DateTime? FechaFinalizacion { get; set; }

    [Required]
    public decimal SubTotal { get; set; }

    [Required]
    public decimal Iva { get; set; }

    [Required]
    public decimal Descuento { get; set; }

    [Required]
    public decimal MontoTotal { get; set; }

    [Required]
    public int? MetodoPago { get; set; }

    public int? IdEstadoReserva { get; set; }

    public virtual ICollection<Abono> Abonos { get; set; } = new List<Abono>();

    public virtual ICollection<DetalleReservaPaquete> DetalleReservaPaquetes { get; set; } = new List<DetalleReservaPaquete>();

    public virtual ICollection<DetalleReservaServicio> DetalleReservaServicios { get; set; } = new List<DetalleReservaServicio>();

    public virtual Cliente? IdClienteNavigation { get; set; }

    public virtual EstadosReserva? IdEstadoReservaNavigation { get; set; }

    public virtual MetodoPago? MetodoPagoNavigation { get; set; }
}
