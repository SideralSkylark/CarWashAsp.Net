using CarWashAsp.Net.Data;
using CarWashAsp.Net.Models;

namespace CarWashAsp.Net.Service
{
    public class UsuarioService
    {
        private readonly UsuarioRepository _usuarioRepository;

        public UsuarioService()
        {
            _usuarioRepository = new UsuarioRepository();
        }

        public Usuario Login(string email, string senha)
        {
            // Verificar se o email e a senha foram fornecidos
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                throw new ArgumentException("Email e senha são obrigatórios.");
            }

            // Realizar o login utilizando o repositório
            var usuario = _usuarioRepository.Login(email, senha);

            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Credenciais inválidas.");
            }

            return usuario;
        }
    }
}
