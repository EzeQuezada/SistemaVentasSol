﻿namespace SistemaVentas.Web.Models.ViewModel
{
    public class VMDashBoard
    {
        public int TotalVentas { get; set; }
        public string? Totallngresos { get; set; }
        public int TotalProductos { get; set; }
        public int TotalCategorias { get; set; }
        public List<VMVentasSemana> VentasUltimaSemana { get; set; }
        public List<VMProductosSemana> ProductosTopUltimaSemana {get;  set; }
            }
}
