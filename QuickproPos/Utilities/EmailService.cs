using MailKit.Net.Smtp;
using MimeKit;

public class EmailService
{
    public string VerificationCode { get; private set; }

    public void SendVerificationEmail(string fromEmail, string toEmail, string smtpServer, int port, string password)
    {
        try
        {
            Random random = new Random();
            VerificationCode = random.Next(100000, 999999).ToString(); // Generate a 6-digit code

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Verification", fromEmail));
            message.To.Add(new MailboxAddress("User", toEmail));
            message.Subject = "Email Verification";
            message.Body = new TextPart("plain") { Text = $"Your verification code is: {VerificationCode}" };

            using var client = new SmtpClient();
            client.Connect(smtpServer, port, MailKit.Security.SecureSocketOptions.StartTls);
            client.Authenticate(fromEmail, password);
            client.Send(message);
            client.Disconnect(true);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to send email: {ex.Message}");
        }
    }
}
