using System;
using System.Collections.Generic;
using AccidentesMadrid.Mappers;
using AccidentesMadrid.Models;
using AccidentesMadrid.Services;
using FluentAssertions;
using NUnit.Framework;

namespace AccidentesMadrid.Tests.Services;

[TestFixture]
public class AccidentesLinqAnalyzerTests
{
    private List<Accidente> _sampleData;
    private AccidentesLinqAnalyzer _analyzer;

    [SetUp]
    public void Setup()
    {
        _sampleData = new List<Accidente>
        {
            new Accidente
            {
                NumExpediente = "2023S00001",
                Fecha = new DateTime(2023, 5, 15, 10, 30, 0), 
                CodDistrito = 1,
                Distrito = "Centro",
                TipoAccidente = "Colisión",
                EstadoMeteorologico = "Despejado",
                TipoVehiculo = "Turismo",
                TipoPersona = TipoPersona.Conductor,
                RangoEdad = "25-29",
                Sexo = Sexo.Hombre,
                CodLesividad = 1,
                PositivaAlcohol = false,
                PositivaDroga = false
            },
            new Accidente
            {
                NumExpediente = "2023S00002",
                Fecha = new DateTime(2023, 5, 20, 22, 15, 0),
                CodDistrito = 2,
                Distrito = "Arganzuela",
                TipoAccidente = "Atropello",
                EstadoMeteorologico = "Lluvia",
                TipoVehiculo = "Furgoneta",
                TipoPersona = TipoPersona.Peaton,
                RangoEdad = "65-69",
                Sexo = Sexo.Mujer,
                CodLesividad = 2,
                PositivaAlcohol = true,
                PositivaDroga = true
            },
            new Accidente
            {
                NumExpediente = "2023S00003",
                Fecha = new DateTime(2023, 6, 10, 10, 30, 0),
                CodDistrito = 1,
                Distrito = "Centro",
                TipoAccidente = "Colisión",
                EstadoMeteorologico = "Despejado",
                TipoVehiculo = "Turismo",
                TipoPersona = TipoPersona.Conductor,
                RangoEdad = "30-34",
                Sexo = Sexo.Mujer,
                CodLesividad = 1,
                PositivaAlcohol = true,
                PositivaDroga = false
            }
        };

        _analyzer = new AccidentesLinqAnalyzer(_sampleData);
    }

    [Test]
    public void TotalAccidentesValido()
    {
        var resultado = _analyzer.TotalAccidentes();

        resultado.Should().Be(3);
    }

    [Test]
    public void TotalPorDistritoValido()
    {
        var resultado = _analyzer.TotalPorDistrito();

        resultado.Should().ContainKey("Centro");
        resultado["Centro"].Should().Be(2);
        resultado["Arganzuela"].Should().Be(1);
    }

    [Test]
    public void PositivosAlcoholValido()
    {
        var resultado = _analyzer.PositivosAlcohol();

        resultado.Should().HaveCount(2);
        resultado.Should().Contain("2023S00002");
        resultado.Should().Contain("2023S00003");
    }

    [Test]
    public void PositivosDrogasValido()
    {
        var resultado = _analyzer.PositivosDrogas();

        resultado.Should().ContainSingle();
        resultado.Should().Contain("2023S00002");
    }

    [Test]
    public void AlcoholDrogaValido()
    {
        var resultado = _analyzer.AlcoholDroga();

        resultado.Should().ContainSingle();
        resultado.Should().Contain("2023S00002");
    }

    [Test]
    public void FinDeSemanaVsEntreSemanaValido()
    {
        var resultado = _analyzer.FinDeSemanaVsEntreSemana();

        resultado["Entre semana"].Should().Be(1);
        resultado["Fin de semana"].Should().Be(2);
    }

    [Test]
    public void MediaAlDiaValida()
    {
        var media = _analyzer.MediaAlDia();

        media.Should().Be(1.0);
    }

    [Test]
    public void PorcentajePorSexoValido()
    {
        var resultado = _analyzer.PorcentajePorSexo();

        resultado[Sexo.Hombre].Should().Be(50.0);
        resultado[Sexo.Mujer].Should().Be(50.0);
    }

    [Test]
    public void TotalPorAnioValido()
    {
        var resultado = _analyzer.TotalPorAño();

        resultado.Should().ContainKey(2023);
        resultado[2023].Should().Be(3);
    }

    [Test]
    public void AccidentesPeatonesValido()
    {
        var resultado = _analyzer.AccidentesPeatones();

        resultado.Should().ContainSingle();
        resultado.Should().Contain("2023S00002");
    }

    [Test]
    public void AccidentesMapper_ToModel_Valido()
    {
        string[] csvRow = new string[19];
        csvRow[0] = "EXP2023";
        csvRow[1] = "15/05/2023";
        csvRow[2] = "14:30";
        csvRow[3] = "Calle Falsa";
        csvRow[4] = "12";
        csvRow[5] = "1";
        csvRow[6] = "Centro";
        csvRow[7] = "Colisión";
        csvRow[8] = "Despejado";
        csvRow[9] = "Turismo";
        csvRow[10] = "Conductor";
        csvRow[11] = "30-34";
        csvRow[12] = "Hombre";
        csvRow[13] = "1";
        csvRow[14] = "Leve";
        csvRow[15] = "440000";
        csvRow[16] = "4450000";
        csvRow[17] = "S";
        csvRow[18] = "N";

        var modelo = AccidentesMapper.ToModel(csvRow);

        modelo.Should().NotBeNull();
        modelo.NumExpediente.Should().Be("EXP2023");
        modelo.Fecha.Date.Should().Be(new DateTime(2023, 5, 15));
        modelo.Sexo.Should().Be(Sexo.Hombre);
        modelo.TipoPersona.Should().Be(TipoPersona.Conductor);
        modelo.PositivaAlcohol.Should().BeTrue();
        modelo.PositivaDroga.Should().BeFalse();
    }
}