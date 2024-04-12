using Entities.Entities;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using WebAPIs.Models;
using WebAPIs.Token;

namespace WebAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //esses UsereManager e o SignIn são nativos do Identity
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("/api/CriarTokenIdentity")]
        public async Task<IActionResult> CriarTokenIdentity([FromBody]Login login)
        {
            if(string.IsNullOrWhiteSpace(login.email) || string.IsNullOrWhiteSpace(login.senha))
            {
                return Unauthorized();
            }

            //Verifica se existe o usuario baseado no login e senha
            var resultado = await
                _signInManager.PasswordSignInAsync(login.email, login.senha, false, lockoutOnFailure: false);

            if(resultado.Succeeded)
            {
                // Recupera Usuário Logado com base no email e pego o id dele
                var userCurrent = await _userManager.FindByEmailAsync(login.email);
                var idUsuario = userCurrent.Id;

                //Crio o token para esse cara logado passando o id dele
                var token = new TokenJWTBuilder()
                    .AddSecurityKey(JwtSecurityKey.Create("43443FDFDF34DF34343fdf344SDFSDFSDFSDFSDF4545354345SDFGDFGDFGDFGdffgfdGDFGDGR%"))
                .AddSubject("Empresa - Canal Dev Net Core")
                .AddIssuer("Teste.Securiry.Bearer")
                .AddAudience("Teste.Securiry.Bearer")
                .AddClaim("idUsuario", idUsuario)
                .AddExpiry(5)
                .Builder();

                return Ok(token.value);
            }
            else
            {
                return Unauthorized();
            }
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("/api/AdicionaUsuarioIdentity")]
        public async Task<IActionResult> AdicionaUsuarioIdentity([FromBody] Login login)
        {
            if (string.IsNullOrWhiteSpace(login.email) || string.IsNullOrWhiteSpace(login.senha))
                return Ok("Falta alguns dados");


            var user = new ApplicationUser
            {
                UserName = login.email,
                Email = login.email,
                CPF = login.cpf,
                Tipo = TipoUsuario.Comum,
            };

            var resultado = await _userManager.CreateAsync(user, login.senha);

            if (resultado.Errors.Any())
            {
                return Ok(resultado.Errors);
            }


            // Geração de Confirmação automatica do usuario
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            // retorno email 
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var resultado2 = await _userManager.ConfirmEmailAsync(user, code);

            if (resultado2.Succeeded)
                return Ok("Usuário Adicionado com Sucesso");
            else
                return Ok("Erro ao confirmar usuários");

        }
    }
}
