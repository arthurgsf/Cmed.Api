using System.Text;
using HtmlAgilityPack;
using ClosedXML.Excel;

namespace Cmed.Scrapper;

public sealed class CmedScrapper(string siteUrl) : ICmedScrapper
{
    private readonly string _siteUrl = siteUrl;
    public async Task<string> GetLatestFileUrlAsync()
    {
        var web = new HtmlWeb();
        var doc = await web.LoadFromWebAsync(_siteUrl);

        var links = doc.DocumentNode.SelectNodes("//a[contains(@href, 'xls_conformidade_site')]");

        var link = links.FirstOrDefault()?.Attributes["href"].Value;

        if (link == null) throw new Exception($"No download link found in {_siteUrl}");

        return link;
    }

    public async Task<string> GetCsvFromUrlAsync(string fileUrl)
    {
        var http = new HttpClient();
        var fileStream = await http.GetStreamAsync(fileUrl);
        XLWorkbook xLWorkbook = new XLWorkbook(fileStream);

        var sheets = xLWorkbook.Worksheets;
        var worksheet = xLWorkbook.Worksheet(1);
        var rowCount = worksheet.RowsUsed().Count();
        var colCount = worksheet.ColumnsUsed().Count();

        int? headerRowNumber = GetHeaderRowNumber(worksheet, rowCount);
        if (headerRowNumber == null) throw new Exception("Arquivo Inválido");

        string csv = ConvertToCSV(
            worksheet, 
            rowStart: headerRowNumber.Value, 
            rowEnd: rowCount, 
            colStart:1,
            colEnd:colCount
        );

        return csv;
    }

    private static int? GetHeaderRowNumber(IXLWorksheet worksheet, int rowCount){
        for (int row = 1; row <= rowCount; row++)
        {
            var valueCol1 = worksheet.Cell(row, 1).GetValue<string>();
            var valueCol2 = worksheet.Cell(row, 2).GetValue<string>();
            /*
            O arquivo de conformidade pode ter indefinidas rows antes de começar o header e os dados de fato
            Caso a row atenda ao padrão definido, então provavelmente ela é o header.
            */
            var isHeaderRow = valueCol1 == "SUBSTÂNCIA" && valueCol2 == "CNPJ";
            if (isHeaderRow)
            {
                return row;
            }
        }
        return null;
    }

    /// <summary>
    /// Converts to CSV all the rows between <paramref name="rowStart"/> and <paramref name="rowEnd"/> and all the cols between <paramref name="colStart"/> and <paramref name="colEnd"/>
    /// </summary>
    /// <param name="worksheet"></param>
    /// <param name="rowStart"></param>
    /// <param name="rowEnd"></param>
    /// <param name="colStart"></param>
    /// <param name="colEnd"></param>
    /// <returns></returns>
    private static string ConvertToCSV(IXLWorksheet worksheet, int rowStart, int rowEnd, int colStart, int colEnd)
    {
       
        var csvBuilder = new StringBuilder();
        for (int row = rowStart; row <= rowEnd; row++)
        {
            List<string> values = [];
            for (int col = colStart; col <= colEnd; col++)
            {
                var value = worksheet.Cell(row, col).GetValue<string>();
                values.Add($"\"{value}\"" ?? "");
            }
            var line = string.Join(";", values);
            csvBuilder.AppendLine(line);
        }
        return csvBuilder.ToString();
    }
}

// TODO: add proper error handling