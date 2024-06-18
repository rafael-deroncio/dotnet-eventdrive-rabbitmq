using System.Net;

namespace AthenasAcademy.Certificate.Core.Exceptions;

public class CertificateException : BaseException
{
    public CertificateException(string title, string message, HttpStatusCode code = HttpStatusCode.Continue) : base(title, message, code)
    {
    }

    public CertificateException(string title, string message, Exception inner, HttpStatusCode code = HttpStatusCode.Continue) : base(title, message)
    {
    }

    public CertificateException(string message, Exception inner) : base(message, inner)
    {
    }
}
