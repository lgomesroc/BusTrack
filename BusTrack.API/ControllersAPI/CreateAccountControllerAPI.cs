using System.Net;
using System.Net.Mail;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.ClassesDB;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("[controller]")]
    [ApiController]
    public class CreateAccountControllerAPI : ControllerBase
    {
        // Supondo que você tenha um serviço para lidar com confirmações de e-mail
        private readonly IEmailConfirmationServiceAPI _emailConfirmationService;

        public CreateAccountControllerAPI(IEmailConfirmationServiceAPI emailConfirmationService)
        {
            _emailConfirmationService = emailConfirmationService;
        }

        [HttpPost("enviar-email-confirmacao")]
        public async Task<IActionResult> EnviarEmailConfirmacao([FromBody] EmailConfirmationDB request)
        {
            try
            {
                // Adicionar a confirmação de e-mail ao banco de dados
                var confirmation = _emailConfirmationService.Create(request);

                // Configurar cliente SMTP
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("seu-email@gmail.com", "sua-senha-do-email");

                    // Construir e-mail de confirmação
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("seu-email@gmail.com"),
                        Subject = "Confirmação de Cadastro - Sistema Bus Track",
                        IsBodyHtml = true,
                        Body = $"Prezado(a), <br><br> Verificamos que você se cadastrou no nosso sistema e solicitamos que clique no link abaixo ou no botão \"Confirmar\" para finalizar o cadastro.<br><br> <a href=\"URL_DO_BACKEND/confirmar-cadastro/{request.Email}\">Confirmar</a><br><br> Você tem 30 minutos para confirmar. <br><br> Atenciosamente, <br> Equipe do Sistema Bus Track",
                    };
                    mailMessage.To.Add(request.Email);

                    // Enviar e-mail
                    await client.SendMailAsync(mailMessage);

                    return Ok("E-mail de confirmação enviado com sucesso");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao enviar e-mail de confirmação: {ex.Message}");
            }
        }


        [HttpGet("{id}")]
        public IActionResult Read(string id)
        {
            var confirmation = _emailConfirmationService.Read(id);
            return Ok(confirmation);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] EmailConfirmationDB confirmation)
        {
            _emailConfirmationService.Update(id, confirmation);
            return Ok(new { message = "Confirmação de e-mail atualizada com sucesso" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            _emailConfirmationService.Delete(id);
            return Ok(new { message = "Confirmação de e-mail deletada com sucesso" });
        }
    }

}