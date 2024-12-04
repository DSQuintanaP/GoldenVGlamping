using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GoldenV.Models;

namespace GoldenV.Controllers
{
    public class ReservasController : Controller
    {
        private readonly GoldenVglampingContext _context;

        private readonly Paquete _paquete;
        public ReservasController(GoldenVglampingContext context, Paquete paquete)
        {
            _context = context;
            _paquete = paquete;
        }
        

        
        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var goldenVglampingContext = _context.Reservas.Include(r => r.IdClienteNavigation).Include(r => r.IdEstadoReservaNavigation).Include(r => r.MetodoPagoNavigation);
            return View(await goldenVglampingContext.ToListAsync());
        }

        /*-----------------------------------------------------------------------------------------------------------------------------------------------*/



        /*-----------------------------------------------------------------------------------------------------------------------------------------------*/

        //// GET: Reservas/Details/5
        //public async Task<IActionResult> Detalles(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var reserva = await _context.Reservas
        //        .Include(r => r.IdClienteNavigation)
        //        .Include(r => r.IdEstadoReservaNavigation)
        //        .Include(r => r.MetodoPagoNavigation)
        //        .FirstOrDefaultAsync(m => m.IdReserva == id);
        //    if (reserva == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(reserva);
        //}

        /*-----------------------------------------------------------------------------------------------------------------------------------------------*/

        // GET: Reservas/Details/5
        public IActionResult Details(int id)
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

            if (reserva == null)
            {
                return NotFound();
            }

            // Cargar la lista de paquetes y servicios seleccionados
            var detallePaquetes = reserva.DetalleReservaPaquetes.Select(dp => new
            {
                Paquete = dp.IdPaqueteNavigation.NomPaquete,
                Precio = dp.Costo
            }).ToList();

            var detalleServicios = reserva.DetalleReservaServicios.Select(ds => new
            {
                Servicio = ds.IdServicioNavigation.NomServicio,
                Precio = ds.Costo,
                Cantidad = ds.Cantidad
            }).ToList();

            // Pasar los datos a la vista
            ViewBag.DetallePaquetes = detallePaquetes;
            ViewBag.DetalleServicios = detalleServicios;

            return View(reserva);
        }

        /*-----------------------------------------------------------------------------------------------------------------------------------------------*/
        //// GET: Reservas/Create
        //public IActionResult Creacion()
        //{
        //    ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "IdCliente");
        //    ViewData["IdEstadoReserva"] = new SelectList(_context.EstadosReservas, "IdEstadoReserva", "IdEstadoReserva");
        //    ViewData["MetodoPago"] = new SelectList(_context.MetodoPagos, "IdMetodoPago", "IdMetodoPago");
        //    return View();
        //}

        //// POST: Reservas/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Creacion([Bind("IdReserva,IdCliente,FechaReserva,FechaInicio,FechaFinalizacion,SubTotal,Iva,Descuento,MontoTotal,MetodoPago,IdEstadoReserva")] Reserva reserva)
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

            ViewBag.IdPaquete = new SelectList(paquetes, "IdPaquete", "NomPaquete");
            ViewBag.Paquetes = paquetes; // Pasar paquetes para uso en JavaScript

            // Cargar los servicios disponibles con Id, Nombre y Precio
            var servicios = await _context.Servicios
                .Select(s => new { s.IdServicio, s.NomServicio, s.Costo })
                .ToListAsync();

            ViewBag.Servicios = servicios; // Pasar servicios a la vista

            var clientes = await _context.Clientes
                .Select(c => new { c.IdCliente, ClienteInfo = $"{c.Nombres} {c.Apellidos}" })
                .ToListAsync();

            ViewBag.IdCliente = new SelectList(clientes, "IdCliente", "ClienteInfo");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdReserva,FechaReserva,FechaInicio,FechaFin,Subtotal,Iva,Total,NoPersonas,IdCliente,IdUsuario,IdEstadoReserva,IdMetodoPago,Descuento")] Reserva reserva,List <int> PaquetesSeleccionados, List<int> ServiciosSeleccionados)
        {
            if (ModelState.IsValid)
            {
                // Obtener los paquetes seleccionados y sumar sus precios
                decimal totalPaquetes = 0;
                foreach (var PaqueteId in PaquetesSeleccionados)
                {
                    var paquete = await _context.Paquetes.FindAsync(PaqueteId);
                    if (paquete != null)
                    {
                        reserva.DetalleReservaPaquetes.Add(new DetalleReservaPaquete
                        {
                            IdPaquete = paquete.IdPaquete,
                            Costo = paquete.Costo,
                            Cantidad = 1 // Ajustar según tus necesidades
                        });
                        totalPaquetes += paquete.Costo;
                    }
                }

                // Obtener los servicios seleccionados y sumar sus precios
                decimal totalServicios = 0;
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
                var subtotalConDescuento = (_paquete.Costo + totalServicios) * (1 - reserva.Descuento / 100);
                reserva.SubTotal = decimal.Round(subtotalConDescuento, 2, MidpointRounding.AwayFromZero);
                reserva.Iva = decimal.Round(reserva.SubTotal * 0.19m, 2, MidpointRounding.AwayFromZero);
                reserva.MontoTotal = decimal.Round(reserva.SubTotal + reserva.Iva, 2, MidpointRounding.AwayFromZero);

                // Agregar el detalle del paquete a la colección de DetallePaquetes de la reserva
                reserva.DetalleReservaPaquetes.Add(DetalleReservaPaquete);
                reserva.DetalleReservaPaquetes.Add(DetalleReservaServicio);

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
            ViewData["IdEstadoReserva"] = new SelectList(_context.EstadosReservas, "IdEstadoReserva", "NombreEstadoReserva");

            
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
        public async Task<IActionResult> Editar(int? id)
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

        //// POST: Reservas/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Editar(int id, [Bind("IdReserva,IdCliente,FechaReserva,FechaInicio,FechaFinalizacion,SubTotal,Iva,Descuento,MontoTotal,MetodoPago,IdEstadoReserva")] Reserva reserva)
        //{
        //    if (id != reserva.IdReserva)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(reserva);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ReservaExists(reserva.IdReserva))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["IdCliente"] = new SelectList(_context.Clientes, "IdCliente", "IdCliente", reserva.IdCliente);
        //    ViewData["IdEstadoReserva"] = new SelectList(_context.EstadosReservas, "IdEstadoReserva", "IdEstadoReserva", reserva.IdEstadoReserva);
        //    ViewData["MetodoPago"] = new SelectList(_context.MetodoPagos, "IdMetodoPago", "IdMetodoPago", reserva.MetodoPago);
        //    return View(reserva);


        /*-----------------------------------------------------------------------------------------------------------------------------------------------*/

        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            return View(reserva);
        }
        // POST: Reservas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdReserva,FechaReserva,FechaInicio,FechaFin,Subtotal,Iva,Total,Descuento")] Reserva reserva)
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
            return View(reserva);
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.IdReserva == id);
        }


        /*-----------------------------------------------------------------------------------------------------------------------------------------------*/

        //// GET: Reservas/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var reserva = await _context.Reservas
        //        .Include(r => r.IdClienteNavigation)
        //        .Include(r => r.IdEstadoReservaNavigation)
        //        .Include(r => r.MetodoPagoNavigation)
        //        .FirstOrDefaultAsync(m => m.IdReserva == id);
        //    if (reserva == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(reserva);
        //}

        //// POST: Reservas/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var reserva = await _context.Reservas.FindAsync(id);
        //    if (reserva != null)
        //    {
        //        _context.Reservas.Remove(reserva);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        /*-----------------------------------------------------------------------------------------------------------------------------------------------*/


        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reservas == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);

            if (reserva != null)
            {
                var detallesPaquetes = _context.DetalleReservaPaquetes.Where(dp => dp.IdReserva == id);
                _context.DetalleReservaPaquetes.RemoveRange(detallesPaquetes);

                var detallesServicios = _context.DetalleReservaServicios.Where(ds => ds.IdReserva == id);
                _context.DetalleReservaServicios.RemoveRange(detallesServicios);

                _context.Reservas.Remove(reserva);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));





            /*-----------------------------------------------------------------------------------------------------------------------------------------------*/

            
        }
        //private bool ReservaExists(int id)
        //{
        //    return _context.Reservas.Any(e => e.IdReserva == id);
        //}
    }
}
