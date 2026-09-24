using Microsoft.Data.Analysis;
using System;
using System.Linq;

namespace AccidentesMadrid.Services;

public class AccidentesDataFramesAnalyzer
{
    private readonly DataFrame _df;
    
    public AccidentesDataFramesAnalyzer(string path)
    {
        _df = DataFrame.LoadCsv(path, separator: ';', header: true);
    }
    
    public long TotalAccidentes()
    {
        return _df.Rows.Count;
    }
    
    public DataFrame TotalPorDistrito()
    {
        return _df.GroupBy("distrito").Count().OrderByDescending("Count").Head(5);
    }

    public DataFrame TotalPorTipo()
    {
        return _df.GroupBy("tipo_accidente").Count();
    }
    
    public DataFrame TotalPorEstadoMeteorologico()
    {
        return _df.GroupBy("estado_meteorológico").Count();
    }
    
    public DataFrame TotalPorSexo()
    {
        return _df.GroupBy("sexo").Count();
    }
    
    public DataFrame TotalPorRangoEdad()
    {
        return _df.GroupBy("rango_edad").Count();
    }
    
    public DataFrame PositivosAlcohol()
    {
        return _df.Filter(_df["positiva_alcohol"].ElementwiseEquals("S"));
    }

    public DataFrame PositivosDrogas()
    {
        return _df.Filter(_df["positiva_droga"].ElementwiseEquals("S"));
    }
    
    public IEnumerable<(DayOfWeek Dia, int Total)> TotalPorDiaSemana()
    {
        return _df.Rows.Select(r => Convert.ToDateTime(r["fecha"]).DayOfWeek).GroupBy(x => x).Select(g => (Dia: g.Key, Total: g.Count()));
    }
    
    public IEnumerable<(int Mes, int Total)> TotalPorMes()
    {
        return _df.Rows.Where(r => r["fecha"] != null).Select(r => Convert.ToDateTime(r["fecha"].ToString()).Month).GroupBy(x => x).Select(g => (Mes: g.Key, Total: g.Count())).OrderBy(x => x.Mes);
    }
    
    public (int Hora, int Total) HoraMasAccidentes()
    {
        return _df.Rows.Where(r => r["fecha"] != null).Select(r => Convert.ToDateTime(r["fecha"].ToString()).Hour).GroupBy(x => x).Select(g => (Hora: g.Key, Total: g.Count())).OrderByDescending(x => x.Total).First();
    }
    
    public DataFrame LesionesFrecuentes()
    {
        return _df.GroupBy("lesividad").Count().OrderByDescending("Count");
    }
    
    public DataFrame TipoVehiculoMasImplicado()
    {
        return _df.GroupBy("tipo_vehiculo").Count().OrderByDescending("Count");
    }
    
    public DataFrame AccidentesPeatones()
    {
        return _df.Filter(_df.Columns["tipo_persona"].ElementwiseEquals("Peatón"));
    }
    
    public Dictionary<string, double> PorcentajePorSexo()
    {
        var filtrado = _df.Rows.Where(r => r["sexo"]?.ToString() == "Hombre" || r["sexo"]?.ToString() == "Mujer").ToList();
            
        double total = filtrado.Count;
        
        return filtrado.GroupBy(r => r["sexo"].ToString()).ToDictionary(g => g.Key, g => Math.Round((g.Count() / total) * 100, 2));
    }
    
    public Dictionary<string, int> DistritosMasPeatones()
    {
        return _df.Rows.Where(r => r["tipo_persona"]?.ToString() == "Peatón").GroupBy(r => r["distrito"].ToString()).Select(g => (Distrito: g.Key, Total: g.Count()))
            .OrderByDescending(y => y.Total).ToDictionary(z => z.Distrito, z => z.Total);
    }

    public Dictionary<string, int> FinDeSemanaVsEntreSemana()
    {
        int finde = _df.Rows.Count(r => {
            var dia= Convert.ToDateTime(r["fecha"]).DayOfWeek;
            return dia == DayOfWeek.Saturday || dia == DayOfWeek.Sunday;
        });
        int total = (int)_df.Rows.Count;

        return new Dictionary<string, int>
        {
            { "Fin de semana", finde },
            { "Entre semana", total - finde }
        };
    }
    
    public double MediaAlDia()
    {
        int total = (int)_df.Rows.Count;
        int totalDia = _df.Rows.Select(r => Convert.ToDateTime(r["fecha"]).Date).Distinct().Count();

        return Math.Round(total / (double)totalDia, 2);
    }
    
    public long AlcoholDroga()
    {
        return _df.Rows.Count(r => r["positiva_alcohol"]?.ToString() == "S" && r["positiva_droga"]?.ToString() == "S");
    }
    
    public Dictionary<string, int> RangosEdadMasVulnerable()
    {
        return _df.Rows.Where(r => r["tipo_persona"]?.ToString() == "Peatón" && !string.IsNullOrWhiteSpace(r["rango_edad"]?.ToString())).GroupBy(r => r["rango_edad"].ToString())
            .Select(g => (RangoEdad: g.Key, Total: g.Count())).OrderByDescending(y => y.Total).ToDictionary(z => z.RangoEdad, z => z.Total);
    }
    
    
    public Dictionary<string, int> DistritosMasAlcohol()
    {
        return _df.Rows.Where(r => r["positiva_alcohol"]?.ToString() == "S" && !string.IsNullOrWhiteSpace(r["distrito"]?.ToString())).GroupBy(r => r["distrito"].ToString())
            .Select(g => (Distrito: g.Key, Total: g.Count())).OrderByDescending(y => y.Total).ToDictionary(z => z.Distrito, z => z.Total);
    }
    
    public Dictionary<int, int> TotalPorCodigoDistrito()
    {
        return _df.Rows.Where(r => r["cod_distrito"] != null).GroupBy(r => (int)r["cod_distrito"]).ToDictionary(g => g.Key, g => g.Count());
    }

    public Dictionary<int, int> TotalPorAño()
    {
        return _df.Rows.GroupBy(r => Convert.ToDateTime(r["fecha"]).Year).ToDictionary(g => g.Key, g => g.Count());
    }
    
    public IEnumerable<object> EvolucionMensualPorAño()
    {
        return _df.Rows
            .GroupBy(r => { var dt = Convert.ToDateTime(r["fecha"]); return new { dt.Year, dt.Month }; }).Select(g => new { g.Key.Year, g.Key.Month, Total = g.Count() })
            .OrderBy(x => x.Year).ThenBy(x => x.Month);
    }
    
    public IEnumerable<object> DistritoMasPorAño()
    {
        return _df.Rows.GroupBy(r => new { Año = Convert.ToDateTime(r["fecha"]).Year, Distrito = r["distrito"] }).Select(g => new { g.Key.Año, g.Key.Distrito, Total = g.Count() }).GroupBy(x => x.Año)
            .Select(g => g.OrderByDescending(x => x.Total).First());
    }
    
    public Dictionary<int, double> TendenciaAlcoholAño()
    {
        return _df.Rows.GroupBy(r => Convert.ToDateTime(r["fecha"]).Year)
            .ToDictionary(g => g.Key, g => Math.Round((g.Count(r => r["positiva_alcohol"]?.ToString() == "S") / (double)g.Count()) * 100, 2));
    }
    
    public Dictionary<int, (int EntreSemana, int FinDeSemana)> ComparativaFinDeSemanaAño()
    {
        return _df.Rows.GroupBy(r => Convert.ToDateTime(r["fecha"]).Year)
            .ToDictionary(g => g.Key, g => (
                EntreSemana: g.Count(r => { var dia = Convert.ToDateTime(r["fecha"]).DayOfWeek; return dia != DayOfWeek.Saturday && dia != DayOfWeek.Sunday; }),
                FinDeSemana: g.Count(r => { var dia = Convert.ToDateTime(r["fecha"]).DayOfWeek; return dia == DayOfWeek.Saturday || dia == DayOfWeek.Sunday; })));
    }
    
    public Dictionary<int, (int Hora, int TotalAccidentes)> HoraPicoAño()
    {
        return _df.Rows.GroupBy(r => { var dt = Convert.ToDateTime(r["fecha"]); return (Año: dt.Year, Hora: dt.Hour); }).Select(g => (g.Key.Año, g.Key.Hora, Total: g.Count())).ToList()
            .GroupBy(x => x.Año).ToDictionary(g => g.Key, g => g.Select(x => (x.Hora, x.Total)).MaxBy(x => x.Total));
    }
    
    public Dictionary<int, (int Lesion, int Total)> LesionFrecuentePorAño()
    {
        return _df.Rows.Where(r => r["cod_lesividad"] != null).GroupBy(r => (Año: Convert.ToDateTime(r["fecha"]).Year, Lesion: Convert.ToInt32(r["cod_lesividad"])))
            .Select(g => (g.Key.Año, g.Key.Lesion, Total: g.Count())).ToList().GroupBy(x => x.Año)
            .ToDictionary(g => g.Key, g => g.Select(x => (x.Lesion, x.Total)).MaxBy(x => x.Total));
    }
    
    public Dictionary<int, int> EvolucionPeatonesAño()
    {
        return _df.Rows.Where(r => r["tipo_persona"]?.ToString() == "Peatón").GroupBy(r => Convert.ToDateTime(r["fecha"]).Year).ToDictionary(g => g.Key, g => g.Count());
    }
}

