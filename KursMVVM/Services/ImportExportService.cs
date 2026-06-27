using KursMVVM.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using CsvHelper;
using CsvHelper.Configuration;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KursMVVM.Services
{
    public class ImportExportService
    {
        public List<string> Logs { get; set; } = new List<string>();

        public string DetectDelimiter(string path)
        {
            using (var sr = new StreamReader(path))
            {
                var firstLine = sr.ReadLine();
                if (firstLine == null) return ";";
                var semicolonCount = firstLine.Count(c => c == ';');
                var commaCount = firstLine.Count(c => c == ',');
                return semicolonCount >= commaCount ? ";" : ",";
            }
        }

        public async Task ImportWorkshopsCSV(string path)
        {
            Logs.Clear();
            Logs.Add("Начало импорта цехов из CSV");
            int line = 1;
            int imported = 0;
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = DetectDelimiter(path) }))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    line++;
                    try
                    {
                        var idStr = csv.GetField("код_цеха");
                        var name = csv.GetField("наименование_цеха") ?? string.Empty;
                        var manager = csv.GetField("начальник_цеха") ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(manager)) { Logs.Add($"Строка {line}: не заполнены поля"); continue; }
                        if (!int.TryParse(idStr, out int id) || id <= 0) { Logs.Add($"Строка {line}: неверный код"); continue; }
                        using (var db = new KursContext())
                        {
                            if (db.Workshops.Any(w => w.Id == id)) { Logs.Add($"Строка {line}: дубль {id}"); continue; }
                            db.Workshops.Add(new Workshop { Id = id, Name = name, Manager = manager });
                            db.SaveChanges();
                            imported++;
                        }
                    }
                    catch (Exception ex) { Logs.Add($"Строка {line}: {ex.Message}"); }
                }
            }
            Logs.Add($"Импортировано {imported} цехов");
        }

        public async Task ImportSpecialClothingCSV(string path)
        {
            Logs.Clear();
            Logs.Add("Начало импорта спецодежды");
            int line = 1, imported = 0;
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = DetectDelimiter(path) }))
            {
                csv.Read(); csv.ReadHeader();
                while (csv.Read())
                {
                    line++;
                    try
                    {
                        var idStr = csv.GetField("код_спецодежды");
                        var type = csv.GetField("вид_спецодежды") ?? string.Empty;
                        if (!int.TryParse(csv.GetField("срок_носки_мес"), out int wear) || wear <= 0) { Logs.Add($"Строка {line}: срок носки <= 0"); continue; }
                        if (!double.TryParse(csv.GetField("стоимость_единицы_руб"), out double cost) || cost <= 0) { Logs.Add($"Строка {line}: стоимость <= 0"); continue; }
                        if (!int.TryParse(idStr, out int id) || id <= 0) { Logs.Add($"Строка {line}: неверный код"); continue; }
                        using (var db = new KursContext())
                        {
                            if (db.SpecialClothings.Any(c => c.Id == id)) { Logs.Add($"Строка {line}: дубль {id}"); continue; }
                            db.SpecialClothings.Add(new SpecialClothing { Id = id, Type = type, WearPeriod = wear, UnitCost = cost });
                            db.SaveChanges();
                            imported++;
                        }
                    }
                    catch (Exception ex) { Logs.Add($"Строка {line}: {ex.Message}"); }
                }
            }
            Logs.Add($"Импортировано {imported} видов");
        }

        public async Task ImportWorkersCSV(string path)
        {
            Logs.Clear();
            Logs.Add("Начало импорта работников");
            int line = 1, imported = 0;
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = DetectDelimiter(path) }))
            {
                csv.Read(); csv.ReadHeader();
                while (csv.Read())
                {
                    line++;
                    try
                    {
                        var idStr = csv.GetField("код_работника");
                        var fio = csv.GetField("фио") ?? string.Empty;
                        var position = csv.GetField("должность") ?? string.Empty;
                        if (!int.TryParse(csv.GetField("скидка_проц"), out int discount) || discount < 0 || discount > 100) { Logs.Add($"Строка {line}: скидка вне диапазона"); continue; }
                        if (!int.TryParse(csv.GetField("код_цеха"), out int workshopId)) { Logs.Add($"Строка {line}: неверный код цеха"); continue; }
                        if (!int.TryParse(idStr, out int id) || id <= 0) { Logs.Add($"Строка {line}: неверный код работника"); continue; }
                        using (var db = new KursContext())
                        {
                            if (!db.Workshops.Any(w => w.Id == workshopId)) { Logs.Add($"Строка {line}: цех не найден"); continue; }
                            if (db.Workers.Any(w => w.Id == id)) { Logs.Add($"Строка {line}: дубль {id}"); continue; }
                            db.Workers.Add(new Worker { Id = id, FIO = fio, Position = position, Discount = discount, WorkshopId = workshopId });
                            db.SaveChanges();
                            imported++;
                        }
                    }
                    catch (Exception ex) { Logs.Add($"Строка {line}: {ex.Message}"); }
                }
            }
            Logs.Add($"Импортировано {imported} работников");
        }

        public async Task ImportReceiptsCSV(string path)
        {
            Logs.Clear();
            Logs.Add("Начало импорта получений");
            int line = 1, imported = 0;
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = DetectDelimiter(path) }))
            {
                csv.Read(); csv.ReadHeader();
                while (csv.Read())
                {
                    line++;
                    try
                    {
                        if (!int.TryParse(csv.GetField("код_работника"), out int workerId)) { Logs.Add($"Строка {line}: неверный код работника"); continue; }
                        if (!int.TryParse(csv.GetField("код_спецодежды"), out int clothingId)) { Logs.Add($"Строка {line}: неверный код спецодежды"); continue; }
                        if (!DateTime.TryParse(csv.GetField("дата_получения"), out DateTime date) || date > DateTime.Now) { Logs.Add($"Строка {line}: дата в будущем"); continue; }
                        var signature = csv.GetField("роспись") ?? string.Empty;
                        using (var db = new KursContext())
                        {
                            if (!db.Workers.Any(w => w.Id == workerId)) { Logs.Add($"Строка {line}: работник не найден"); continue; }
                            if (!db.SpecialClothings.Any(c => c.Id == clothingId)) { Logs.Add($"Строка {line}: спецодежда не найдена"); continue; }
                            db.Receipts.Add(new Receipt { WorkerId = workerId, ClothingId = clothingId, DateReceived = date, Signature = signature });
                            db.SaveChanges();
                            imported++;
                        }
                    }
                    catch (Exception ex) { Logs.Add($"Строка {line}: {ex.Message}"); }
                }
            }
            Logs.Add($"Импортировано {imported} получений");
        }

        public async Task ImportWorkersExcel(string path)
        {
            Logs.Clear();
            Logs.Add("Начало импорта работников из Excel");
            int imported = 0;
            ExcelPackage.License.SetNonCommercialPersonal("KBK");
            using (var package = new ExcelPackage(new FileInfo(path)))
            {
                var ws = package.Workbook.Worksheets[0];
                if (ws == null || ws.Dimension == null) { Logs.Add("Лист пустой"); return; }
                for (int i = 2; i <= ws.Dimension.Rows; i++)
                {
                    try
                    {
                        var fio = ws.Cells[i, 1].Text ?? string.Empty;
                        var position = ws.Cells[i, 2].Text ?? string.Empty;
                        if (!int.TryParse(ws.Cells[i, 3].Text, out int discount) || discount < 0 || discount > 100) { Logs.Add($"Строка {i}: скидка"); continue; }
                        if (!int.TryParse(ws.Cells[i, 4].Text, out int workshopId)) { Logs.Add($"Строка {i}: цех"); continue; }
                        using (var db = new KursContext())
                        {
                            if (!db.Workshops.Any(w => w.Id == workshopId)) { Logs.Add($"Строка {i}: цех не найден"); continue; }
                            db.Workers.Add(new Worker { FIO = fio, Position = position, Discount = discount, WorkshopId = workshopId });
                            db.SaveChanges();
                            imported++;
                        }
                    }
                    catch (Exception ex) { Logs.Add($"Строка {i}: {ex.Message}"); }
                }
            }
            Logs.Add($"Импортировано {imported} работников");
        }

        public async Task ImportReceiptsExcel(string path)
        {
            Logs.Clear();
            Logs.Add("Начало импорта получений из Excel");
            int imported = 0;
            ExcelPackage.License.SetNonCommercialPersonal("KBK");
            using (var package = new ExcelPackage(new FileInfo(path)))
            {
                var ws = package.Workbook.Worksheets[0];
                if (ws == null || ws.Dimension == null) { Logs.Add("Лист пустой"); return; }
                for (int i = 2; i <= ws.Dimension.Rows; i++)
                {
                    try
                    {
                        if (!int.TryParse(ws.Cells[i, 1].Text, out int workerId)) { Logs.Add($"Строка {i}: работник"); continue; }
                        if (!int.TryParse(ws.Cells[i, 2].Text, out int clothingId)) { Logs.Add($"Строка {i}: спецодежда"); continue; }
                        if (!DateTime.TryParse(ws.Cells[i, 3].Text, out DateTime date) || date > DateTime.Now) { Logs.Add($"Строка {i}: дата"); continue; }
                        var signature = ws.Cells[i, 4].Text ?? string.Empty;
                        using (var db = new KursContext())
                        {
                            if (!db.Workers.Any(w => w.Id == workerId)) { Logs.Add($"Строка {i}: работник не найден"); continue; }
                            if (!db.SpecialClothings.Any(c => c.Id == clothingId)) { Logs.Add($"Строка {i}: спецодежда не найдена"); continue; }
                            db.Receipts.Add(new Receipt { WorkerId = workerId, ClothingId = clothingId, DateReceived = date, Signature = signature });
                            db.SaveChanges();
                            imported++;
                        }
                    }
                    catch (Exception ex) { Logs.Add($"Строка {i}: {ex.Message}"); }
                }
            }
            Logs.Add($"Импортировано {imported} получений");
        }

        public async Task ImportJSON(string path)
        {
            Logs.Clear();
            Logs.Add("Начало импорта из JSON");
            var json = await File.ReadAllTextAsync(path);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
            {
                var first = root[0];
                using (var db = new KursContext())
                {
                    int imported = 0;
                    if (first.TryGetProperty("код_цеха", out _))
                    {
                        var list = JsonSerializer.Deserialize<List<Workshop>>(json);
                        if (list != null) foreach (var w in list) { if (!db.Workshops.Any(x => x.Id == w.Id)) { db.Workshops.Add(w); imported++; } }
                    }
                    else if (first.TryGetProperty("код_спецодежды", out _))
                    {
                        var list = JsonSerializer.Deserialize<List<SpecialClothing>>(json);
                        if (list != null) foreach (var c in list) { if (!db.SpecialClothings.Any(x => x.Id == c.Id)) { db.SpecialClothings.Add(c); imported++; } }
                    }
                    else if (first.TryGetProperty("код_работника", out _))
                    {
                        var list = JsonSerializer.Deserialize<List<Worker>>(json);
                        if (list != null) foreach (var w in list) { if (!db.Workers.Any(x => x.Id == w.Id)) { db.Workers.Add(w); imported++; } }
                    }
                    else if (first.TryGetProperty("дата_получения", out _))
                    {
                        var list = JsonSerializer.Deserialize<List<Receipt>>(json);
                        if (list != null) foreach (var r in list) { if (!db.Receipts.Any(x => x.Id == r.Id)) { db.Receipts.Add(r); imported++; } }
                    }
                    db.SaveChanges();
                    Logs.Add($"Импортировано {imported} записей");
                }
            }
            else
            {
                Logs.Add("JSON должен быть массивом объектов");
            }
        }

        public Task ImportXML(string path)
        {
            Logs.Clear();
            Logs.Add("Начало импорта из XML");
            var doc = System.Xml.Linq.XDocument.Load(path);
            var root = doc.Root;
            if (root == null) { Logs.Add("XML пустой"); return Task.CompletedTask; }
            using (var db = new KursContext())
            {
                int imported = 0;
                foreach (var elem in root.Elements())
                {
                    var e = elem;
                    var id = (int?)e.Element("код_цеха") ?? (int?)e.Element("код_спецодежды") ?? (int?)e.Element("код_работника") ?? 0;
                    var type = e.Element("вид_спецодежды")?.Value;
                    if (type != null)
                    {
                        var wear = (int?)e.Element("срок_носки_мес") ?? 0;
                        var cost = (double?)e.Element("стоимость_единицы_руб") ?? 0;
                        if (!db.SpecialClothings.Any(x => x.Id == id)) { db.SpecialClothings.Add(new SpecialClothing { Id = id, Type = type, WearPeriod = wear, UnitCost = cost }); imported++; }
                    }
                    else
                    {
                        var name = e.Element("наименование_цеха")?.Value;
                        if (name != null)
                        {
                            var manager = e.Element("начальник_цеха")?.Value ?? "";
                            if (!db.Workshops.Any(x => x.Id == id)) { db.Workshops.Add(new Workshop { Id = id, Name = name, Manager = manager }); imported++; }
                            continue;
                        }
                        var fio = e.Element("фио")?.Value;
                        if (fio != null)
                        {
                            var position = e.Element("должность")?.Value ?? "";
                            var discount = (int?)e.Element("скидка_проц") ?? 0;
                            var workshopId = (int?)e.Element("код_цеха") ?? 0;
                            if (!db.Workers.Any(x => x.Id == id)) { db.Workers.Add(new Worker { Id = id, FIO = fio, Position = position, Discount = discount, WorkshopId = workshopId }); imported++; }
                            continue;
                        }
                        var dateStr = e.Element("дата_получения")?.Value;
                        if (dateStr != null && DateTime.TryParse(dateStr, out var date))
                        {
                            var workerId = (int?)e.Element("код_работника") ?? 0;
                            var clothingId = (int?)e.Element("код_спецодежды") ?? 0;
                            var signature = e.Element("роспись")?.Value ?? "";
                            db.Receipts.Add(new Receipt { WorkerId = workerId, ClothingId = clothingId, DateReceived = date, Signature = signature }); imported++;
                        }
                    }
                }
                db.SaveChanges();
                Logs.Add($"Импортировано {imported} записей");
            }
            return Task.CompletedTask;
        }

        // ==================== Export ====================
        public async Task ExportWorkersCSV(string path)
        {
            using (var db = new KursContext())
            {
                var list = db.Workers.Include(w => w.Workshop).ToList();
                using (var writer = new StreamWriter(path))
                using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" }))
                {
                    csv.WriteField("код_работника"); csv.WriteField("фио"); csv.WriteField("должность"); csv.WriteField("скидка_проц"); csv.WriteField("код_цеха"); csv.NextRecord();
                    foreach (var item in list) { csv.WriteField(item.Id); csv.WriteField(item.FIO); csv.WriteField(item.Position); csv.WriteField(item.Discount); csv.WriteField(item.WorkshopId); csv.NextRecord(); }
                }
            }
        }

        public async Task ExportSpecialClothingCSV(string path)
        {
            using (var db = new KursContext())
            {
                var list = db.SpecialClothings.ToList();
                using (var writer = new StreamWriter(path))
                using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" }))
                {
                    csv.WriteField("код_спецодежды"); csv.WriteField("вид_спецодежды"); csv.WriteField("срок_носки_мес"); csv.WriteField("стоимость_единицы_руб"); csv.NextRecord();
                    foreach (var item in list) { csv.WriteField(item.Id); csv.WriteField(item.Type); csv.WriteField(item.WearPeriod); csv.WriteField(item.UnitCost); csv.NextRecord(); }
                }
            }
        }

        public async Task ExportWorkshopsCSV(string path)
        {
            using (var db = new KursContext())
            {
                var list = db.Workshops.ToList();
                using (var writer = new StreamWriter(path))
                using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" }))
                {
                    csv.WriteField("код_цеха"); csv.WriteField("наименование_цеха"); csv.WriteField("начальник_цеха"); csv.NextRecord();
                    foreach (var item in list) { csv.WriteField(item.Id); csv.WriteField(item.Name); csv.WriteField(item.Manager); csv.NextRecord(); }
                }
            }
        }

        public async Task ExportReceiptsCSV(string path)
        {
            using (var db = new KursContext())
            {
                var list = db.Receipts.ToList();
                using (var writer = new StreamWriter(path))
                using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" }))
                {
                    csv.WriteField("код_работника"); csv.WriteField("код_спецодежды"); csv.WriteField("дата_получения"); csv.WriteField("роспись"); csv.NextRecord();
                    foreach (var item in list) { csv.WriteField(item.WorkerId); csv.WriteField(item.ClothingId); csv.WriteField(item.DateReceived.ToString("yyyy-MM-dd")); csv.WriteField(item.Signature); csv.NextRecord(); }
                }
            }
        }

        public async Task ExportReportExcel(string path, string sheetName, List<string> headers, List<List<object>> rows)
        {
            ExcelPackage.License.SetNonCommercialPersonal("KBK");
            var file = new FileInfo(path);
            using (var package = new ExcelPackage(file))
            {
                var ws = package.Workbook.Worksheets.Add(sheetName);
                for (int h = 0; h < headers.Count; h++) { ws.Cells[1, h + 1].Value = headers[h]; ws.Cells[1, h + 1].Style.Font.Bold = true; }
                for (int r = 0; r < rows.Count; r++) for (int c = 0; c < rows[r].Count; c++) ws.Cells[r + 2, c + 1].Value = rows[r][c];
                ws.Cells.AutoFitColumns();
                await package.SaveAsync();
            }
        }

        public async Task ExportReportPDF(string path, string title, List<string> headers, List<List<string>> rows)
        {
            using (var pdfDoc = new PdfDocument(new PdfWriter(path)))
            using (var doc = new Document(pdfDoc))
            {
                var cyrillicFont = PdfFontFactory.CreateFont(@"C:\Windows\Fonts\arial.ttf", "Identity-H");
                doc.SetFont(cyrillicFont);
                var bold = PdfFontFactory.CreateFont(@"C:\Windows\Fonts\arialbd.ttf", PdfEncodings.IDENTITY_H);
                var paragraph = new Paragraph(new Text(title).SetFont(bold).SetFontSize(16));
                paragraph.SetTextAlignment(TextAlignment.CENTER);
                doc.Add(paragraph);
                var widths = new float[headers.Count];
                for (int i = 0; i < headers.Count; i++) widths[i] = 100f / headers.Count;
                var table = new Table(UnitValue.CreatePercentArray(widths));
                table.SetMarginTop(10);
                table.SetTextAlignment(TextAlignment.CENTER);
                foreach (var h in headers) table.AddCell(new Cell().Add(new Paragraph(h).SetFont(bold)));
                foreach (var row in rows) foreach (var cell in row) table.AddCell(cell);
                doc.Add(table);
                doc.Close();
            }
        }
    }
}
