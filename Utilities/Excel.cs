using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace Shivyshine.Utilities
{
    public class Excel : Controller, IExcel
    {
        public FileStreamResult Export<T>(List<T> res, string fileName, string sheetName)
        {
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);
                worksheet.Cells.LoadFromCollection<T>(res, true);
                package.Save();
            }
            stream.Position = 0;
            string excelName = fileName + ".xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}