using Microsoft.AspNetCore.Mvc;

using AutoMapper;
using Newtonsoft.Json;
using SistemaVentas.Web.Models.ViewModel;
using SistemaVentas.Web.Utilidades.Response;
using SistemasVentas.BLL.Interfaces;
using SistemaVentas.Entity;

namespace SistemaVentas.Web.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService usuarioService;
        private readonly IRolService rolService;
        private readonly IMapper mapper;

        public UsuarioController(IUsuarioService usuarioService, IRolService rolService, IMapper mapper )
        {
            this.usuarioService = usuarioService;
            this.rolService = rolService;
            this.mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task <IActionResult> ListaRoles()
        {
            
            List<VMRol>vmListaRoles = mapper.Map<List<VMRol>>(await rolService.Lista());
            return StatusCode(StatusCodes.Status200OK,vmListaRoles);
        }

        public async Task<IActionResult> Lista()
        {

            List<VMUsuario> vmUsuarioLista = mapper.Map<List<VMUsuario>>(await rolService.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = vmUsuarioLista });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
        {

            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();
            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario> (modelo);
                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto!= null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombre_en_codigo, extension);
                    fotoStream = foto.OpenReadStream();
                }
                string UrlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";

                Usuario usuario_creado = await usuarioService.Crear(mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto, UrlPlantillaCorreo);

                vmUsuario = mapper.Map<VMUsuario>(usuario_creado);

                gResponse.Estado = true;
                gResponse.Objeto = vmUsuario;


            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
                throw;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
        {

            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();
            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);
                string nombreFoto = ""; 
                Stream fotoStream = null;

                if (foto != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombre_en_codigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                Usuario usuario_editado = await usuarioService.Editar(mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto);

                vmUsuario = mapper.Map<VMUsuario>(usuario_editado);

                gResponse.Estado = true;
                gResponse.Objeto = vmUsuario;


            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
               
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int IdUsuario)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await usuarioService.Eliminar(IdUsuario);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
