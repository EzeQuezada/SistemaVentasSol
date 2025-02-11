﻿using SistemaVentas.Web.Models.ViewModel;
using System.Globalization;
using AutoMapper;
using SistemaVentas.Entity;

using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
namespace SistemaVentas.Web.Utilidades.Automapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region Rol
            CreateMap<Rol, VMRol>().ReverseMap();
            #endregion

            #region Usuario
            CreateMap<Usuario, VMUsuario>()
                .ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0)
                ).ForMember(destino =>
                destino.NombreRol,
                opt => opt.MapFrom(origen => origen.IdRolNavigation.Descripcion)
                );

            CreateMap<VMUsuario, Usuario>().ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false)
                ).ForMember(destino =>
                destino.IdRolNavigation,
                opt => opt.Ignore());
            #endregion

            #region Negocio
            CreateMap<Negocio, VMNegocio>().ForMember(destino =>
                destino.PorcentajeImpuesto,
                opt => opt.MapFrom(origen => Convert.ToString(origen.PorcentajeImpuesto.Value, new CultureInfo("es-ES")))
                );
            CreateMap<VMNegocio, Negocio>().
                ForMember(destino =>
                destino.PorcentajeImpuesto,
                opt => opt.MapFrom(origen => Convert.ToDecimal(origen.PorcentajeImpuesto, new CultureInfo("es-ES")))
               );
            #endregion

            #region Categoria 
            CreateMap<Categoria, VMCategoria>()
                .ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0)
                );

            CreateMap<VMCategoria, Categoria>().ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false)
                );

            #endregion

            #region Producto
            CreateMap<Producto, VMProducto>()
            .ForMember(destino => destino.EsActivo,
            opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0)
            )
            .ForMember(destino =>
                destino.NombreCategoria,//Vigilar esta tabla puede que traega problema con la base de datos
                opt => opt.MapFrom(origen => origen.IdCategoriaNavigation.Descripcion)
            )
            .ForMember(destino =>
                destino.Precio,
                opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-ES")))
                );


            //CAMBIO DE LOGICA

            CreateMap<VMProducto, Producto>()
          .ForMember(destino => destino.EsActivo,
          opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false)
          )
          .ForMember(destino =>
              destino.IdCategoriaNavigation,//Vigilar esta tabla puede que traega problema con la base de datos
              opt => opt.Ignore()
          )
          .ForMember(destino =>
              destino.Precio,
              opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Precio.Value, new CultureInfo("es-ES")))
              );

            #endregion

            #region TipoDocumentoVenta
            CreateMap<TipoDocumentoVenta, VMTipoDocumentoVenta>().ReverseMap();
            #endregion

            #region Venta
            CreateMap<Venta, VMVenta>()
            .ForMember(destino => destino.TipoDocumentoVenta,
            opt => opt.MapFrom(origen => origen.IdTipoDocumentoVentaNavigation.Descripcion)
            ).ForMember(destino => destino.Usuario,
            opt => opt.MapFrom(origen => origen.IdUsuarioNavigation.Nombre)
            )
            .ForMember(destino =>
                destino.SubTotal,
                opt => opt.MapFrom(origen => Convert.ToString(origen.SubTotal.Value, new CultureInfo("es-ES")))
                )
            .ForMember(destino =>
                destino.ImpuestoTotal,
                opt => opt.MapFrom(origen => Convert.ToString(origen.ImpuestoTotal.Value, new CultureInfo("es-ES")))
                )
            .ForMember(destino =>
                destino.Total,
                opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-ES")))
                )
            .ForMember(destino =>
                destino.FechaRegistro,
                opt => opt.MapFrom(origen => origen.FechaRegistro.Value.ToString("dd/MM/yyyy"))
                );

            CreateMap<VMVenta, Venta>()

         .ForMember(destino =>
             destino.SubTotal,
             opt => opt.MapFrom(origen => Convert.ToDecimal(origen.SubTotal, new CultureInfo("es-ES")))
             )
         .ForMember(destino =>
             destino.ImpuestoTotal,
             opt => opt.MapFrom(origen => Convert.ToDecimal(origen.ImpuestoTotal, new CultureInfo("es-ES")))
             )
         .ForMember(destino =>
             destino.Total,
             opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Total, new CultureInfo("es-ES")))
             );

            #endregion

            #region DetalleVenta
            CreateMap<DetalleVenta, VMDetalleVenta>()
                .ForMember(destino => destino.Precio,
                opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-Es")))
                )
                .ForMember(destino => destino.Total,
                opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-Es")))
                );

            CreateMap<VMDetalleVenta, DetalleVenta>()
                .ForMember(destino => destino.Precio,
                opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Precio, new CultureInfo("es-Es")))
                )
                .ForMember(destino => destino.Total,
                opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Total, new CultureInfo("es-Es")))
                );

            #endregion

            #region Reporte de venta

            CreateMap<DetalleVenta, VMReporteVenta>()
                .ForMember(destino => destino.FechaRegistro,
                opt => opt.MapFrom(origen => origen.IdVentaNavigation.FechaRegistro.Value.ToString("dd/MMyyyy"))
                )
                .ForMember(destino => destino.NumeroVenta,
                opt => opt.MapFrom(origen => origen.IdVentaNavigation.NumeroVenta)
                )
                .ForMember(destino => destino.TipoDocumento,
                opt => opt.MapFrom(origen => origen.IdVentaNavigation.IdTipoDocumentoVentaNavigation.Descripcion)
                )
                .ForMember(destino => destino.DocumentoCliente,
                opt => opt.MapFrom(origen => origen.IdVentaNavigation.DocumentoCliente)
                )
                .ForMember(destino => destino.NombreCliente,
                opt => opt.MapFrom(origen => origen.IdVentaNavigation.NombreCliente)
                )
                .ForMember(destino => destino.SubTotal,
                opt => opt.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.SubTotal.Value, new CultureInfo("es-Es")))
                )
                .ForMember(destino => destino.ImpuestoTotal,
                opt => opt.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.ImpuestoTotal.Value, new CultureInfo("es-Es")))
                )
                .ForMember(destino => destino.Total,
                opt => opt.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.Total.Value, new CultureInfo("es-Es")))
                )
                 .ForMember(destino => destino.Producto,
                opt => opt.MapFrom(origen => origen.DescripcionProducto)
                )
                 .ForMember(destino => destino.Precio,
                opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-Es")))
                )
                 .ForMember(destino => destino.TotalVenta,
                opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-Es")))
                );

            #endregion

            #region Menu
            CreateMap<Menu, VMMenu>()
                .ForMember(destino => destino.SubMenus,
                opt => opt.MapFrom(origen => origen.InverseIdMenuPadreNavigation)
                );
            #endregion
            
        }

    }
}
