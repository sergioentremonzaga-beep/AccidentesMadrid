using System;
using System.IO;
using AccidentesMadrid.Repositories;
using AccidentesMadrid.Services;
using FluentAssertions;
using NUnit.Framework;

namespace AccidentesMadrid.Tests.Services;

[TestFixture]
public class AccidentesDataFramesAnalyzerTests
{
    private string _tempCsvPath;
    private AccidentesDataFramesAnalyzer _analyzer;
    private AccidenteRepository _repository;

    [SetUp]
    public void Setup()
    {
        _tempCsvPath = Path.GetTempFileName();
        
        var csvContent = 
            "num_expediente;fecha;hora;localizacion;numero;cod_distrito;distrito;tipo_accidente;estado_meteorológico;tipo_vehiculo;tipo_persona;rango_edad;sexo;cod_lesividad;lesividad;coordenada_x_utm;coordenada_y_utm;positiva_alcohol;positiva_droga\n" +
            "2023S00001;15/05/2023;10:30;Calle A;1;1;Centro;Colisión;Despejado;Turismo;Conductor;25-29;Hombre;1;Leve;440000;4450000;N;N\n" +
            "2023S00002;20/05/2023;22:15;Calle B;2;2;Arganzuela;Atropello;Lluvia;Furgoneta;Peatón;65-69;Mujer;2;Grave;440001;4450001;S;S\n" +
            "2023S00003;10/06/2023;11:00;Calle C;3;1;Centro;Colisión;Despejado;Turismo;Conductor;30-34;Mujer;1;Leve;440002;4450002;S;N";

        File.WriteAllText(_tempCsvPath, csvContent);

        _analyzer = new AccidentesDataFramesAnalyzer(_tempCsvPath);
        _repository = new AccidenteRepository();
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_tempCsvPath))
        {
            File.Delete(_tempCsvPath);
        }
    }

    [Test]
    public void TotalAccidentes_DebeRetornarConteoCorrecto()
    {
        long total = _analyzer.TotalAccidentes();
        total.Should().Be(3);
    }

    [Test]
    public void PositivosAlcohol_DebeFiltrarCorrectamente()
    {
        var dfFiltrado = _analyzer.PositivosAlcohol();
        dfFiltrado.Rows.Count.Should().Be(2);
    }

    [Test]
    public void AlcoholDroga_DebeRetornarAmbosPositivos()
    {
        long total = _analyzer.AlcoholDroga();
        total.Should().Be(1);
    }

    [Test]
    public void MediaAlDia_DebeCalcularPromedioValido()
    {
        double media = _analyzer.MediaAlDia();
        media.Should().BeGreaterThan(0);
    }

    [Test]
    public void AccidenteRepository_GetAll_DebeLeerCsvCorrectamente()
    {
        var lista = _repository.GetAll(_tempCsvPath);

        lista.Should().NotBeNull();
        lista.Should().HaveCount(3);
        lista[0].NumExpediente.Should().Be("2023S00001");
        lista[1].Distrito.Should().Be("Arganzuela");
        lista[1].PositivaAlcohol.Should().BeTrue();
    }
}