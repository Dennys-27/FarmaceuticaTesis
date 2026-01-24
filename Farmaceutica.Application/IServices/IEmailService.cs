using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmaceutica.Application.IServices
{
    public interface IEmailService
    {
        Task SendPasswordResetEmail(string email, string resetToken);
        Task SendVerificationEmail(string email, string verificationCode);
        Task SendWelcomeEmail(string email, string nombreUsuario);
    }
}
