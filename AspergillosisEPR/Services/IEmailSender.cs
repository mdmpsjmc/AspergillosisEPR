using System.Threading.Tasks;

namespace AspergillosisEPR.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
