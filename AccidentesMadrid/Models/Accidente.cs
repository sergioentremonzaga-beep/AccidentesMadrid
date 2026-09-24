namespace AccidentesMadrid.Models;

public class Accidente
{
    public string NumExpediente { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public string Localizacion { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public int? CodDistrito { get; set; } 
    public string Distrito { get; set; } = string.Empty;
    public string TipoAccidente { get; set; } = string.Empty;
    public string EstadoMeteorologico { get; set; } = string.Empty;
    public string TipoVehiculo { get; set; } = string.Empty;
    public TipoPersona TipoPersona { get; set; } = TipoPersona.Desconocido;
    public string RangoEdad { get; set; } = string.Empty;
    public Sexo Sexo { get; set; } = Sexo.Desconocido;
    public int? CodLesividad { get; set; }
    public string Lesividad { get; set; } = string.Empty;
    public int? CoordenadaXUtm { get; set; }
    public int? CoordenadaYUtm { get; set; }
    public bool? PositivaAlcohol { get; set; }
    public bool? PositivaDroga { get; set; }
    
    public int Año => Fecha.Year;
    public int Mes => Fecha.Month;
    public int Dia => Fecha.Day;
    public DayOfWeek DiaSemana => Fecha.DayOfWeek;
}
