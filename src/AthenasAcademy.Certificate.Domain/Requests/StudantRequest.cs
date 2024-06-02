﻿namespace AthenasAcademy.Certificate.Domain.Requests;

public record class StudantRequest
{
    public string Name { get; set; }
    public DateTime BornDate { get; set; }
    public int Registration { get; set; }
    public DocumentRequest Document { get; set; }
}
