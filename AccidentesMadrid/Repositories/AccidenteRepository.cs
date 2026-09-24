using AccidentesMadrid.Mappers;
using AccidentesMadrid.Models;

namespace AccidentesMadrid.Repositories;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

public class AccidenteRepository
{
    public List<Accidente> GetAll(string path)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null
        };
        
        var lista = new List<Accidente>();

        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, config);

        while (csv.Read())
        {
            string[] partes = csv.Parser.Record ?? Array.Empty<string>();
            var x = AccidentesMapper.ToModel(partes);
            lista.Add(x);
        }
        return lista;
    }
}