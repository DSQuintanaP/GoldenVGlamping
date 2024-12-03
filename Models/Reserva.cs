using System;
using System.Collections.Generic;

namespace GoldenV.Models;

public partial class Reserva
{
    public int IdReserva { get; set; }

    public int? IdCliente { get; set; }

    public DateTime? FechaReserva { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFinalizacion { get; set; }

    public decimal? SubTotal { get; set; }

    public decimal? Iva { get; set; }

    public decimal? Descuento { get; set; }

    public decimal? MontoTotal { get; set; }

    public int? MetodoPago { get; set; }

    public int? IdEstadoReserva { get; set; }

    public virtual ICollection<Abono> Abonos { get; set; } = new List<Abono>();

    public virtual ICollection<DetalleReservaPaquete> DetalleReservaPaquetes { get; set; } = new List<DetalleReservaPaquete>();

    public virtual ICollection<DetalleReservaServicio> DetalleReservaServicios { get; set; } = new List<DetalleReservaServicio>();

    public virtual Cliente? IdClienteNavigation { get; set; }

    public virtual EstadosReserva? IdEstadoReservaNavigation { get; set; }

    public virtual MetodoPago? MetodoPagoNavigation { get; set; }
}
