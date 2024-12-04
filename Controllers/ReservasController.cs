using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GoldenV.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Globalization;

namespace GoldenV.Controllers
{
    public class ReservasController : Controller
    {
        private readonly GoldenVglampingContext _context;

        public ReservasController(GoldenVglampingContext context)
        {
            _context = context;
        }

        // GET: Reservas
        public async Task<IActionResult> Indice()/*--Viejo--Metodo-*/
        {
            var goldenVglampingContext = _context.Reservas.Include(r => r.IdClienteNavigation).Include(r => r.IdEstadoReservaNavigation).Include(r => r.MetodoPagoNavigation);
            return View(await goldenVglampingContext.ToListAsync());
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Detalles(int? id) /*--Viejo--Metodo-*/
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.IdClienteNavigation)
                .Include(r => r.IdEstadoReservaNavigation)
                .Include(r => r.MetodoPagoNavigation)
                .FirstOrDefaultAsync(m => m.IdReserva == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        /*-----------------------------------------------------------------------------------------------------------------------------------------------*/

        // GET: Reservas/Details/5
        public IActionResult Details(int id)/*--Nuevo--Metodo-*/
        {
            // Obtener la reserva con todos los detalles
            var reserva = _context.Reservas
                .Include(r => r.IdClienteNavigation)  // Incluir cliente                
                .Include(r => r.IdEstadoReservaNavigation)  // Incluir estado de reserva
                .Include(r => r.MetodoPagoNavigation)  // Incluir método de pago                
                .Include(r => r.DetalleReservaPaquetes)  // Incluir detalles de paquetes
                .Include(r => r.DetalleReservaServicios)  // Incluir detalles de servicios
                .ThenInclude(ds => ds.IdServicioNavigation)  // Incluir servicios en detalles
                .FirstOrDefault(r => r.IdReserva == id);

        //// POST: Reservas/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("IdReserva,IdCliente,FechaReserva,FechaInicio,FechaFinalizacion,SubTotal,Iva,Descuento,MontoTotal,MetodoPago,IdEstadoReserva")] Reserva reserva)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(reserva);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "IdCliente", reserva.IdCliente);
        //    ViewData["IdEstadoReserva"] = new SelectList(_context.EstadosReservas, "IdEstadoReserva", "IdEstadoReserva", reserva.IdEstadoReserva);
        //    ViewData["MetodoPago"] = new SelectList(_context.MetodoPagos, "IdMetodoPago", "IdMetodoPago", reserva.MetodoPago);
        //    return View(reserva);
        //}

        /*-----------------------------------------------------------------------------------------------------------------------------------------------*/

        public async Task<IActionResult> Create()
        {
            CargarDatosVista(); // Cargar clientes, métodos de pago y estados de reserva

            // Cargar los paquetes disponibles con Id, Nombre y Precio
            var paquetes = await _context.Paquetes
                .Select(p => new { p.IdPaquete, p.NomPaquete, p.Costo })
                .ToListAsync();

            ViewBag.IdPaquete = new SelectList(paquetes, "IdPaquete", "Nombre");
            ViewBag.Paquetes = paquetes; // Pasar paquetes para uso en JavaScript

        /*-----------------------------------------------------------------------------------------------------------------------------------------------*/
        // GET: Reservas/Create
        public IActionResult Registrar() /*--Viejo--Metodo-*/
        {
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "IdCliente");
            ViewData["IdEstadoReserva"] = new SelectList(_context.EstadosReservas, "IdEstadoReserva", "IdEstadoReserva");
            ViewData["MetodoPago"] = new SelectList(_context.MetodoPagos, "IdMetodoPago", "IdMetodoPago");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar([Bind("IdReserva,IdCliente,FechaReserva,FechaInicio,FechaFinalizacion,SubTotal,Iva,Descuento,MontoTotal,MetodoPago,IdEstadoReserva")] Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                // Asignar el IdPaquete a la entidad Reserva
                reserva.IdPaquete = IdPaquete;

                // Obtener el paquete seleccionado y su precio
                var paquete = await _context.Paquetes.FindAsync(IdPaquete);
                if (paquete == null)
                {
                    ModelState.AddModelError("IdPaquete", "El paquete seleccionado no existe.");
                    CargarDatosVista();
                    return View(reserva);
                }

                // Crear el detalle del paquete asociado a la reserva
                var detallePaquete = new DetalleReservaPaquete
                {
                    IdPaquete = paquete.IdPaquete,
                    Costo = paquete.Costo,
                    
                };

                // Obtener los servicios seleccionados y sumar sus precios
                decimal totalServicios;
                foreach (var servicioId in ServiciosSeleccionados)
                {
                    var servicio = await _context.Servicios.FindAsync(servicioId);
                    if (servicio != null)
                    {
                        reserva.DetalleReservaServicios.Add(new DetalleReservaServicio
                        {
                            IdServicio = servicio.IdServicio,
                            Costo = servicio.Costo,                            
                            Cantidad = 1 // Ajustar según tus necesidades
                        });
                        totalServicios += servicio.Costo;
                    }
                }

                // Calcular el subtotal con el precio del paquete y los servicios seleccionados
                var subtotalConDescuento = (paquete.Costo + totalServicios) * (1 - reserva.Descuento / 100);
                reserva.SubTotal = decimal.Round((decimal)subtotalConDescuento, 2, MidpointRounding.AwayFromZero);
                reserva.Iva = decimal.Round((decimal)(reserva.SubTotal * 0.19m), 2, MidpointRounding.AwayFromZero);
                reserva.MontoTotal = decimal.Round((decimal)(reserva.SubTotal + reserva.Iva), 2, MidpointRounding.AwayFromZero);

                // Agregar el detalle del paquete a la colección de DetallePaquetes de la reserva
                reserva.DetalleReservaPaquetes.Add(detallePaquete);

                // Guardar la reserva junto con los detalles de paquete y servicios seleccionados
                _context.Add(reserva);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Si el modelo no es válido, recargar los datos para la vista
            CargarDatosVista();
            return View(reserva);
        }





        public void CargarDatosVista()
        {
            // Cargar clientes disponibles
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "Nombres");

            // Cargar métodos de pago disponibles
            ViewData["IdMetodoPago"] = new SelectList(_context.MetodoPagos, "IdMetodoPago", "NomMetodoPago");

            // Cargar estados de reserva
            ViewData["IdEstadoReserva"] = new SelectList(_context.EstadosReservas, "IdEstadosReserva", "NombreEstadoReserva");
                       
        }
        public async Task<IActionResult> AgregarAbono(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }

            // Crear un nuevo abono y asignar la reserva
            var abono = new Abono { IdReserva = id, FechaAbono = DateTime.Now };
            return View("CreateAbono", abono); // Asegúrate de pasar el modelo de Abono con el IdReserva
        }


        /*-----------------------------------------------------------------------------------------------------------------------------------------------*/

        // GET: Reservas/Edit/5
        public async Task<IActionResult> Editar(int? id)/*--Viejo--Metodo-*/
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "IdCliente", reserva.IdCliente);
            ViewData["IdEstadoReserva"] = new SelectList(_context.EstadosReservas, "IdEstadoReserva", "IdEstadoReserva", reserva.IdEstadoReserva);
            ViewData["MetodoPago"] = new SelectList(_context.MetodoPagos, "IdMetodoPago", "IdMetodoPago", reserva.MetodoPago);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdReserva,IdCliente,FechaReserva,FechaInicio,FechaFinalizacion,SubTotal,Iva,Descuento,MontoTotal,MetodoPago,IdEstadoReserva")] Reserva reserva)
        {
            if (id != reserva.IdReserva)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.IdReserva))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "IdCliente", reserva.IdCliente);
            ViewData["IdEstadoReserva"] = new SelectList(_context.EstadosReservas, "IdEstadoReserva", "IdEstadoReserva", reserva.IdEstadoReserva);
            ViewData["MetodoPago"] = new SelectList(_context.MetodoPagos, "IdMetodoPago", "IdMetodoPago", reserva.MetodoPago);
            return View(reserva);
        }

        // GET: Reservas/Delete/5
        public async Task<IActionResult> Eliminar(int? id)/*--Viejo--Metodo-*/
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.IdClienteNavigation)
                .Include(r => r.IdEstadoReservaNavigation)
                .Include(r => r.MetodoPagoNavigation)
                .FirstOrDefaultAsync(m => m.IdReserva == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminacionConfirmada(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.IdReserva == id);
        }
    }
}
