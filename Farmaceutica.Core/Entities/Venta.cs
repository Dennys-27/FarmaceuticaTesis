using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;

namespace Farmaceutica.Core.Entities
{
    public class Venta
    {
        public int Id { get; set; }

        // Solo obligatorio
        public int EncargadoId { get; set; }
        public Usuario? Encargado { get; set; }

        // Todo lo demás opcional
        public int? ClienteId { get; set; }
        public Usuario? Cliente { get; set; }

        public string? CliRuc { get; set; }
        public string? CliDireccion { get; set; }
        public string? CliCorreo { get; set; }

        public MetodoPago? MetodoPago { get; set; }
        public Moneda? Moneda { get; set; }
        public Unidad? Unidad { get; set; }
        public DocumentoTipo? DocumentoTipo { get; set; }

        // NUEVOS CAMPOS
        public TipoVenta? TipoVenta { get; set; } // Web, Farmacia, Delivery
        public EstadoDelivery? EstadoDelivery { get; set; } // Solo para ventas con delivery

        public int? RepartidorId { get; set; } // ID del repartidor asignado
        public Usuario? Repartidor { get; set; } // Relación al repartidor

        public DateTime? FechaAsignacion { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string? DireccionEntrega { get; set; } // Dirección específica para delivery
                                                      // FIN NUEVOS CAMPOS

        public decimal? Subtotal { get; set; }
        public decimal? Igv { get; set; }
        public decimal? Total { get; set; }

        public string? Comentario { get; set; }

        // Para el delivery
        public string? ComentarioRepartidor { get; set; } // Observaciones del repartidor
        public string? CodigoSeguimiento { get; set; } // Código de seguimiento único

        public int IsActive { get; set; }
        public DateTime FechaCreacion { get; set; }

        public ICollection<DetalleVenta>? DetalleVentas { get; set; }
    }

    public enum MetodoPago
    {
        Efectivo = 1,
        Tarjeta = 2,
        Transferencia = 3
    }

    public enum Moneda
    {
        Dolar = 1,
        Euro = 2
    }

    public enum Unidad
    {
        Unidad = 1,
        Tableta = 2,
        Caja = 3,
        Tarro = 4
    }

    public enum DocumentoTipo
    {
        Boleta = 1,
        Factura = 2,
        Ticket = 3,
        NotaCrédito = 4,
        NotaDébito = 5
    }

    // Agrega estos enumerados junto a los que ya tienes

    public enum TipoVenta
    {
        Farmacia = 1,    // Venta física en farmacia
        Web = 2,         // Venta online
        Delivery = 3     // Para delivery directo
    }

    public enum EstadoDelivery
    {
        Pendiente = 1,    // Aún no asignado
        Asignado = 2,     // Asignado a repartidor
        EnCamino = 3,     // En proceso de entrega
        Entregado = 4,    // Entregado al cliente
        Cancelado = 5,    // Entrega cancelada
        Rechazado = 6     // Cliente rechazó entrega
    }
}
