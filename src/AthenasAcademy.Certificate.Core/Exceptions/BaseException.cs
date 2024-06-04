﻿using System.Net;
using AthenasAcademy.Certificate.Domain.Configurations.Enums;


namespace AthenasAcademy.Certificate.Core.Exceptions;

public class BaseException : Exception
{
    public string Title { get; set; }
    public ResponseType Type { get; set; }
    public HttpStatusCode Code { get; set; }

    public BaseException(string title, string message, HttpStatusCode code = HttpStatusCode.Continue) : base(message)
    {
        Title = title;
        Code = code;
        Type =  (int)code >= 500 ? ResponseType.Fatal :
                (int)code >= 400 ? ResponseType.Warning : ResponseType.Warning;
    }

    public BaseException(string title, string message, Exception inner, HttpStatusCode code = HttpStatusCode.Continue) : base(message, inner)
    {
        Title = title;
        Code = code;
        Type = (int)code >= 500 ? ResponseType.Fatal :
                (int)code >= 400 ? ResponseType.Warning : ResponseType.Warning;
    }

    public BaseException(string message, Exception inner) : base(message, inner)
    {
        Title = "Error";
        Type = ResponseType.Fatal;
        Code = HttpStatusCode.InternalServerError;
    }
}