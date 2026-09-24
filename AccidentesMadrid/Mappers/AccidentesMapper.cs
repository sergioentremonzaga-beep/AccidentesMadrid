using System.Globalization;
using AccidentesMadrid.Models;

namespace AccidentesMadrid.Mappers;

public static class AccidentesMapper
{ 
    public static Accidente ToModel(string[] csv)
    {
        string partes(int indice) => indice < csv.Length ? csv[indice].Trim() : string.Empty;

        DateTime.TryParseExact(
            partes(1), 
            "dd/MM/yyyy", 
            CultureInfo.InvariantCulture, 
            DateTimeStyles.None, 
            out var fecha
        );

        return new Accidente
        {
            NumExpediente = partes(0),
            Fecha = fecha,
            Hora = TimeSpan.TryParse(partes(2), out var h) ? h : TimeSpan.Zero,
            Localizacion = partes(3),
            Numero = partes(4),
            CodDistrito = int.TryParse(partes(5), out var x) ? x : null,
            Distrito = partes(6),
            TipoAccidente = partes(7),
            EstadoMeteorologico = partes(8),
            TipoVehiculo = partes(9),
            TipoPersona = ParsePersona(partes(10)),
            RangoEdad = partes(11),
            Sexo = ParseSexo(partes(12)),
            CodLesividad = int.TryParse(partes(13), out var y) ? y : null,
            Lesividad = partes(14),
            CoordenadaXUtm = ParseCoordenada(partes(15)),
            CoordenadaYUtm = ParseCoordenada(partes(16)),
            PositivaAlcohol = ParsePositivos(partes(17)),
            PositivaDroga = ParsePositivos(partes(18))
        };
        
    }
    
    private static int? ParseCoordenada(string x)
    {
        if (string.IsNullOrWhiteSpace(x)) return null;
        
        string y = x.Split(',', '.')[0].Trim();

        if (int.TryParse(y, out int result))
            return result;

        return null;
    }

    private static TipoPersona ParsePersona(string x)
    {
        string y = x.ToLower();
        return y switch
        {
            "conductor" => TipoPersona.Conductor,
            "pasajero" => TipoPersona.Pasajero,
            "peatón" => TipoPersona.Peaton,
            _ => TipoPersona.Desconocido
        };
    }
    
    private static Sexo ParseSexo(string x)
    {
        string y = x.ToLower();
        return y switch
        {
            "hombre" => Sexo.Hombre,
            "mujer" => Sexo.Mujer,
            _ => Sexo.Desconocido
        };
    }
    
    private static bool? ParsePositivos(string x)
    {
        string y = x.Trim().ToUpper();
        return y switch
        {
            "S" => true,
            "N" => false,
            _ => null
        };
    }
}