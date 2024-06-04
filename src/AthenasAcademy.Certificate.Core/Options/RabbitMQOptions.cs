namespace AthenasAcademy.Certificate.Core.Options;

public record RabbitMQOptions
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}
