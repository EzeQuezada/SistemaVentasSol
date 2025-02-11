﻿namespace SistemaVentas.Web.Models.ViewModel
{
    public class VMVenta
    {
        public int IdVenta { get; set; }

        public string? NumeroVenta { get; set; }

        public string? Descripcion { get; set; }

        public int? IdTipoDocumentoVentaNavigation { get; set; }

        public int? IdUsuario { get; set; }

        public string? DocumentoCliente { get; set; }

        public string? NombreCliente { get; set; }

        public string? SubTotal { get; set; }

        public string? ImpuestoTotal { get; set; }

        public string? Total { get; set; }

        public string? FechaRegistro { get; set; }

        public virtual ICollection<VMDetalleVenta> DetalleVenta { get; set; }
        public virtual ICollection<VMTipoDocumentoVenta> TipoDocumentoVenta { get; set; }
        public virtual ICollection<VMUsuario> Usuario { get; set; }
    }
}
