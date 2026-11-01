using VitalPetTech.Application.DTOs;
using VitalPetTech.Domain.Common;
using VitalPetTech.Domain.Entities;

namespace VitalPetTech.Application.Mappings;

public static class MappingExtensions
{
    public static PagedResult<TOut> Map<TIn, TOut>(this PagedResult<TIn> source, Func<TIn, TOut> map) =>
        new(source.Items.Select(map).ToList(), source.Page, source.PageSize, source.TotalCount);

    // ---------- Tutor ----------
    public static TutorResponse ToResponse(this Tutor t) =>
        new(t.Id, t.NomeTutor, t.CpfTutor, t.TelefoneTutor, t.EnderecoTutor,
            t.Pets.Select(p => new PetResumoResponse(p.Id, p.Nome, p.EspeciePet)).ToList());

    public static Tutor ToEntity(this TutorRequest r)
    {
        var tutor = new Tutor();
        r.ApplyTo(tutor);
        return tutor;
    }

    public static void ApplyTo(this TutorRequest r, Tutor t)
    {
        t.NomeTutor = r.NomeTutor;
        t.CpfTutor = r.CpfTutor;
        t.TelefoneTutor = r.TelefoneTutor;
        t.EnderecoTutor = r.EnderecoTutor;
    }

    // ---------- Pet ----------
    public static PetResponse ToResponse(this Pet p) =>
        new(p.Id, p.Nome, p.DataNascimento, p.PesoPet, p.EspeciePet,
            p.IdTutor, p.Tutor?.NomeTutor ?? string.Empty);

    public static Pet ToEntity(this PetRequest r)
    {
        var pet = new Pet();
        r.ApplyTo(pet);
        return pet;
    }

    public static void ApplyTo(this PetRequest r, Pet p)
    {
        p.Nome = r.Nome;
        p.DataNascimento = r.DataNascimento!.Value;
        p.PesoPet = r.PesoPet;
        p.EspeciePet = r.EspeciePet;
        p.IdTutor = r.IdTutor;
    }

    // ---------- EventoClinico ----------
    public static EventoClinicoResponse ToResponse(this EventoClinico e) =>
        new(e.Id, e.Tipo, e.DataEvento, e.Clinica, e.Descricao, e.ProximoRetorno,
            e.IdPet, e.Pet?.Nome ?? string.Empty);

    public static EventoClinico ToEntity(this EventoClinicoRequest r)
    {
        var evento = new EventoClinico();
        r.ApplyTo(evento);
        return evento;
    }

    public static void ApplyTo(this EventoClinicoRequest r, EventoClinico e)
    {
        e.Tipo = r.Tipo;
        e.DataEvento = r.DataEvento!.Value;
        e.Clinica = r.Clinica;
        e.Descricao = r.Descricao;
        e.ProximoRetorno = r.ProximoRetorno;
        e.IdPet = r.IdPet;
    }

    // ---------- Alerta ----------
    public static AlertaResponse ToResponse(this Alerta a) =>
        new(a.Id, a.TipoAlerta, a.Valor, a.Latitude, a.Longitude, a.Severidade,
            a.Resolvido, a.Timestamp, a.IdPet, a.Pet?.Nome ?? string.Empty);

    public static Alerta ToEntity(this AlertaRequest r)
    {
        var alerta = new Alerta { Resolvido = false }; // todo alerta novo nasce não resolvido
        r.ApplyTo(alerta);
        return alerta;
    }

    public static void ApplyTo(this AlertaRequest r, Alerta a)
    {
        a.TipoAlerta = r.TipoAlerta;
        a.Valor = r.Valor;
        a.Latitude = r.Latitude;
        a.Longitude = r.Longitude;
        a.Severidade = r.Severidade;
        a.Timestamp = r.Timestamp!.Value;
        a.IdPet = r.IdPet;
    }
}