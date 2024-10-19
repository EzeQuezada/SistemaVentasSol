using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;

using SistemasVentas.BLL.Interfaces;
using SistemaVentas.Entity;
using SistemasVentas.DAL.Interfaces;
using System.Text.Encodings.Web;


namespace SistemasVentas.BLL.Implementaciones
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> repositorio;
        private readonly IFireBaseService fireBaseService;
        private readonly IUtilidadesService utilidadesService;
        private readonly ICorreoService correoService;

        public UsuarioService(IGenericRepository<Usuario> repositorio,
            IFireBaseService fireBaseService,
            IUtilidadesService utilidadesService,
            ICorreoService correoService)
        {
            this.repositorio = repositorio;
            this.fireBaseService = fireBaseService;
            this.utilidadesService = utilidadesService;
            this.correoService = correoService;
        }
        public async Task<List<Usuario>> Lista()
        {
            IQueryable<Usuario> query = await repositorio.Consultar();
            return query.Include(rol=>rol.IdRolNavigation).ToList();
        }

        public async Task<Usuario> Crear(Usuario entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "")
        {
            Usuario usario_existe = await repositorio.Obtener(u=>u.Correo == entidad.Correo);
            if (usario_existe != null)
               throw new TaskCanceledException("El correo ya existe");

            try
            {
                string clave_generada = utilidadesService.GenerarClave();
                entidad.Clave = utilidadesService.ConvertirSha256(clave_generada);
                entidad.NombreFoto = NombreFoto;

                if (Foto != null)
                {
                    string urlFoto = await fireBaseService.SubirStorage(Foto, "carpeta_usuario", NombreFoto);
                    entidad.UrlFoto = urlFoto;
                }

                Usuario usuario_creado = await repositorio.Crear(entidad);
                if (usuario_creado.IdUsuario == 0)
                    throw new TaskCanceledException("No se pudo crear el usuario");
                if (UrlPlantillaCorreo !="")
                {
                    UrlPlantillaCorreo = UrlPlantillaCorreo.Replace
                                                           ("[correo]", usuario_creado.Correo)
                                                           .Replace("[clave]", clave_generada);

                    string htmlCorreo = "";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader streamReader = null;

                            if (response.CharacterSet==null)
                                streamReader = new StreamReader(dataStream);
                            else 
                                streamReader = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                            htmlCorreo = streamReader.ReadToEnd();
                            response.Close();
                            streamReader.Close();
                           
                            
                        }
                    }
                    if (htmlCorreo != "")
                        await correoService.EnviarCorreo(usuario_creado.Correo, "Cuenta Creada", htmlCorreo);
                }
                IQueryable<Usuario>query = await repositorio.Consultar(U => U.IdUsuario == usuario_creado.IdUsuario);
                usuario_creado = query.Include(r=>r.IdRolNavigation).First();

                return usuario_creado;

            }
            catch (Exception ex)
            {
                throw;
            }

            
        }

        public async Task<Usuario> Editar(Usuario entidad, Stream Foto = null, string NombreFoto = "")
        {
            Usuario usario_existe = await repositorio.Obtener(u => u.Correo == entidad.Correo && u.IdUsuario !=entidad.IdUsuario);

            if (usario_existe != null) 
            throw new TaskCanceledException("El correo ya existe");

            try
            {
                IQueryable<Usuario> queryUsuario = await repositorio.Consultar(u => u.IdUsuario == entidad.IdUsuario);
                
                Usuario usuario_editar = queryUsuario.First();
                usuario_editar.Nombre = entidad.Nombre;
                usuario_editar.Correo = entidad.Correo;
                usuario_editar.Telefono = entidad.Telefono;
                usuario_editar.IdRol = entidad.IdRol;

                if (usuario_editar.NombreFoto =="")
                    usuario_editar.NombreFoto =NombreFoto;
                if (Foto != null)
                {
                    string urlFoto = await fireBaseService.SubirStorage(Foto, "carpeta_usuario", usuario_editar.NombreFoto);
                    usuario_editar.UrlFoto = urlFoto;
                }
                bool respuesta = await repositorio.Editar(usuario_editar);

                if (respuesta)
                    throw new TaskCanceledException("No se pudo modificar el usuario");

                Usuario usuario_editado = queryUsuario.Include(r=>r.IdRolNavigation).First();
                return usuario_editado;
            }
            catch 
            {
                throw;
            }
            

        }

        public async Task<bool> Eliminar(int IdUsuario)
        {
            try
            {   
                Usuario usuario_encontrado = await repositorio.Obtener(u=>u.IdUsuario == IdUsuario);
                if (usuario_encontrado == null) 
                    throw new TaskCanceledException("El usuario no existe");
                
                string nombreFoto = usuario_encontrado.NombreFoto;
                bool respuesta = await repositorio.Eliminar(usuario_encontrado);

                if (respuesta)
                    await fireBaseService.EliminarStorage("carpeta_usuario", nombreFoto);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Usuario> ObtenerPorCredenciales(string correo, string clave)
        {
            string clave_encriptada = utilidadesService.ConvertirSha256(clave);

            Usuario usuario_encontrado = await repositorio.Obtener(u=>u.Correo.Equals(correo)
            &&u.Clave.Equals(clave_encriptada));

            return usuario_encontrado;
        }

        public async Task<Usuario> ObtenerPorId(int IdUsuario)
        {
            IQueryable<Usuario> query = await repositorio.Consultar(u => u.IdUsuario == IdUsuario);
            Usuario resultado = query.Include(r=>r.IdRolNavigation).FirstOrDefault();

            return resultado;
        }

        public async Task<bool> GuardarPerfil(Usuario entidad)
        {
            try
            {
                Usuario usuario_encontrado = await repositorio.Obtener(e => e.IdUsuario == entidad.IdUsuario);
                if (usuario_encontrado == null)
                    throw new TaskCanceledException("El usuario no existe");

                usuario_encontrado.Correo = entidad.Correo;
                usuario_encontrado.Telefono = entidad.Telefono;

                bool respuesta = await repositorio.Editar(usuario_encontrado);
                return respuesta;
            }
            catch 
            {
                throw;
            }
          
        }

        public async Task<bool> CambiarClave(int IdUsuario, string ClaveActual, string ClaveNueva)
        {
            try
            {
                Usuario usuario_encontrado = await repositorio.Obtener(u=>u.IdUsuario==IdUsuario);
                if(usuario_encontrado == null)
                 throw new TaskCanceledException("El usuario no existe");
                if (usuario_encontrado.Clave != utilidadesService.ConvertirSha256(ClaveNueva)) 
                    throw new TaskCanceledException("La contraseña ingresada como actual no es correcta");

                usuario_encontrado.Clave = utilidadesService.ConvertirSha256(ClaveActual);

                bool respuesta = await repositorio.Editar(usuario_encontrado);

                return respuesta;

            }
            catch 
            {

                throw;
            }
        }

        public async Task<bool> RestablecerClave(string correo, string UrlPlantillaCorreo)
        {
            try
            {
                Usuario usuario_encontrado = await repositorio.Obtener(u=>u.Correo ==  correo);

                if(usuario_encontrado == null)
                throw new TaskCanceledException("No encontramos ningun usuario asociado al correo");

                string clave_generada = utilidadesService.GenerarClave();
                usuario_encontrado.Clave = utilidadesService.ConvertirSha256(clave_generada);


                UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[clave]", clave_generada);

                string htmlCorreo = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader streamReader = null;

                        if (response.CharacterSet == null)
                            streamReader = new StreamReader(dataStream);
                        else
                            streamReader = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                        htmlCorreo = streamReader.ReadToEnd();
                        response.Close();
                        streamReader.Close();


                    }
                }
                 bool correo_enviado = false;

                if (htmlCorreo != "")
                    correo_enviado = await correoService.EnviarCorreo(correo,"Contraseña Restablecida", htmlCorreo);

                if(!correo_enviado)
                    throw new TaskCanceledException("Tenemos problemas. Por favor intentalo de nuevo más tenemos.");

                bool respuesta = await repositorio.Editar(usuario_encontrado);
                return respuesta;

            }
            catch 
            {
                throw;
            }
        }         
    }
}
