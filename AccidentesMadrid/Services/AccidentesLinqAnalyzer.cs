
using System.ComponentModel;
using AccidentesMadrid.Models;

namespace AccidentesMadrid.Services;

public class AccidentesLinqAnalyzer(List<Accidente> lista)
{
    public int TotalAccidentes() 
    {
        return lista.Count; 
    }

    public Dictionary<string, int> TotalPorDistrito()
    {
        return lista.AsParallel().Where(x => !string.IsNullOrWhiteSpace(x.Distrito)).GroupBy(x => x.Distrito).Select(g => (Distrito: g.Key, Total: g.Count()))
            .OrderByDescending(y => y.Total).Take(5).ToDictionary(z => z.Distrito, z => z.Total);
    }
    
    public Dictionary<string, int> TotalPorTipo()
    {
        return lista.AsParallel().Where(x => !string.IsNullOrWhiteSpace(x.TipoAccidente)).GroupBy(x => x.TipoAccidente).ToDictionary(g => g.Key, g => g.Count());
    }
    
    public Dictionary<string, int> TotalPorEstadoMeteorologico()
    {
        return lista.AsParallel().Where(x => !string.IsNullOrWhiteSpace(x.EstadoMeteorologico)).GroupBy(x => x.EstadoMeteorologico).ToDictionary(g => g.Key, g => g.Count());
    }
    
    public Dictionary<Sexo, int> TotalPorSexo()
    {
        return lista.AsParallel().GroupBy(x => x.Sexo).ToDictionary(g => g.Key, g => g.Count());
    }
    
    public Dictionary<string, int> TotalPorRangoEdad()
    {
        return lista.AsParallel().Where(x => !string.IsNullOrWhiteSpace(x.RangoEdad)).GroupBy(x => x.RangoEdad).ToDictionary(g => g.Key, g => g.Count());
    }

    public List<string> PositivosAlcohol()
    {
        return lista.Where(x => x.PositivaAlcohol == true).Select(x => x.NumExpediente).ToList();
    }
    
    public List<string> PositivosDrogas()
    {
        return lista.Where(x => x.PositivaDroga == true).Select(x => x.NumExpediente).ToList();
    }

    public Dictionary<DayOfWeek, int> TotalPorDiaSemana()
    {
        return lista.AsParallel().GroupBy(x => x.DiaSemana).ToDictionary(g => g.Key, g => g.Count());
    }

    public Dictionary<int, int> TotalPorMes()
    {
        return lista.AsParallel().GroupBy(x => x.Mes).ToDictionary(g => g.Key, g => g.Count());
    }
    
    public (int Hora, int Total) HoraMasAccidentes()
    {
        return lista.AsParallel().GroupBy(x => x.Hora.Hours).Select(g => (Hora: g.Key, Total: g.Count())).OrderByDescending(y => y.Total).First();                
    }

    public Dictionary<int, int> LesionesFrecuentes()
    {
        return lista.AsParallel().Where(x => x.CodLesividad != null).GroupBy(x => (int)x.CodLesividad!).Select(g => (Lesion: g.Key, Total: g.Count()))
            .OrderByDescending(y => y.Total).ToDictionary(z => z.Lesion, z=> z.Total);
    }

    public (string TipoVehiculo, int Total) TipoVehiculoMasImplicado()
    {
        return lista.AsParallel().Where(x => !string.IsNullOrWhiteSpace(x.TipoVehiculo)).GroupBy(x => x.TipoVehiculo).Select(g => (TipoVehiculo: g.Key, Total: g.Count())).OrderByDescending(y => y.Total).First();  
    }

    public List<string> AccidentesPeatones()
    {
        return lista.Where(x => x.TipoPersona == TipoPersona.Peaton).Select(x => x.NumExpediente).ToList();
    }

    public Dictionary<Sexo, double> PorcentajePorSexo()
    {
        double total = lista.Count(x => x.Sexo == Sexo.Hombre || x.Sexo == Sexo.Mujer);
        
        return lista.AsParallel().Where(x => x.Sexo == Sexo.Hombre || x.Sexo == Sexo.Mujer).GroupBy(x => x.Sexo).ToDictionary(g => g.Key, g => Math.Round((g.Count() / total) * 100, 2));
    }
    
    public Dictionary<string, int> DistritosMasPeatones()
    {
        return lista.AsParallel().Where(x => x.TipoPersona == TipoPersona.Peaton && !string.IsNullOrWhiteSpace(x.Distrito)).GroupBy(x => x.Distrito).Select(g => (Distrito: g.Key, Total: g.Count()))
            .OrderByDescending(y => y.Total).ToDictionary(z => z.Distrito, z => z.Total);
    }

    public Dictionary<string, int> FinDeSemanaVsEntreSemana()
    {
        int finde = lista.Where(x => x.DiaSemana == DayOfWeek.Saturday || x.DiaSemana == DayOfWeek.Sunday).Count();
        int total = lista.Count;

        return new Dictionary<string, int>
        {
            { "Fin de semana", finde },
            { "Entre semana", total - finde }
        };
    }

    public double MediaAlDia()
    {
        int total = lista.Count;
        
        int totalDia = lista.Select(x => x.Fecha.Date).Distinct().Count();

        return Math.Round(total / (double)totalDia, 2);
    }

    public List<string> AlcoholDroga()
    {
        return lista.Where(x => x.PositivaAlcohol == true && x.PositivaDroga == true).Select(x => x.NumExpediente).ToList();
    }

    public Dictionary<string, int> RangosEdadMasVulnerable()
    {
        return lista.AsParallel().Where(x => x.TipoPersona == TipoPersona.Peaton && !string.IsNullOrWhiteSpace(x.RangoEdad)).GroupBy(x => x.RangoEdad).Select(g => (RangoEdad: g.Key, Total: g.Count()))
            .OrderByDescending(y => y.Total).ToDictionary(z => z.RangoEdad, z => z.Total);
    }
    
    public Dictionary<string, int> DistritosMasAlcohol()
    {
        return lista.AsParallel().Where(x => x.PositivaAlcohol == true && !string.IsNullOrWhiteSpace(x.Distrito)).GroupBy(x => x.Distrito).Select(g => (Distrito: g.Key, Total: g.Count()))
            .OrderByDescending(y => y.Total).ToDictionary(z => z.Distrito, z => z.Total);
    }
    
    public Dictionary<int, int> TotalPorCodigoDistrito()
    {
        return lista.AsParallel().Where(x => x.CodDistrito != null).GroupBy(x => (int)x.CodDistrito!).ToDictionary(g => g.Key, g => g.Count());
    }

    public Dictionary<int, int> TotalPorAño()
    {
        return lista.AsParallel().GroupBy(x => x.Año).ToDictionary(g => g.Key, g => g.Count());
    }
    
    public Dictionary<(int Año, int Mes), int> EvolucionMensualPorAño()
    {
        return lista.AsParallel().GroupBy(x => (x.Fecha.Year, x.Fecha.Month)).ToDictionary(g => g.Key, g => g.Count());
    }
    
    public Dictionary<int, (string Distrito, int Total)> DistritoMasPorAño()
    {
        return lista.AsParallel().Where(x => !string.IsNullOrWhiteSpace(x.Distrito)).GroupBy(x => (Año: x.Fecha.Year, Distrito: x.Distrito)).Select(g => (g.Key.Año, g.Key.Distrito, Total: g.Count()))
            .ToList().GroupBy(x => x.Año).ToDictionary(g => g.Key, g => g.Select(x => (x.Distrito, x.Total)).MaxBy(x => x.Total));
    }
    
    public Dictionary<int, double> TendenciaAlcoholAño()
    {
        return lista.AsParallel().GroupBy(x => x.Fecha.Year).ToDictionary(g => g.Key, g => Math.Round((g.Count(x => x.PositivaAlcohol == true) / (double)g.Count()) * 100, 2));
    }
    
    public Dictionary<int, (int EntreSemana, int FinDeSemana)> ComparativaFinDeSemanaAño()
    {
        return lista.AsParallel().GroupBy(x => x.Fecha.Year).ToDictionary(g => g.Key, g => (
                    EntreSemana: g.Count(x => x.Fecha.DayOfWeek != DayOfWeek.Saturday && x.Fecha.DayOfWeek != DayOfWeek.Sunday),
                    FinDeSemana: g.Count(x => x.Fecha.DayOfWeek == DayOfWeek.Saturday || x.Fecha.DayOfWeek == DayOfWeek.Sunday)));
    }
    
    public Dictionary<int, (int Hora, int TotalAccidentes)> HoraPicoAño()
    {
        return lista.AsParallel().GroupBy(x => (Año: x.Fecha.Year, Hora: x.Fecha.Hour)).Select(g => (g.Key.Año, g.Key.Hora, Total: g.Count())).ToList().GroupBy(x => x.Año)
            .ToDictionary(g => g.Key, g => g.Select(x => (x.Hora, x.Total)).MaxBy(x => x.Total));
    }
    
    public Dictionary<int, (int CodLesividad, int Total)> LesionFrecuentePorAño()
    {
        return lista.AsParallel().Where(x => x.CodLesividad != null).GroupBy(x => (Año: x.Fecha.Year, Lesion: (int)x.CodLesividad!)).Select(g => (g.Key.Año, g.Key.Lesion, Total: g.Count())).ToList()
            .GroupBy(x => x.Año).ToDictionary(g => g.Key, g => g.Select(x => (x.Lesion, x.Total)).MaxBy(x => x.Total));
    }
    
    public Dictionary<int, int> EvolucionPeatonesAño()
    {
        return lista.AsParallel().Where(x => x.TipoPersona == TipoPersona.Peaton).GroupBy(x => x.Fecha.Year).ToDictionary(g => g.Key, g => g.Count());
    }
}
