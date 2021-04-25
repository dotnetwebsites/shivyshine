using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace Shivyshine.Utilities
{
    public interface IExcel
    {
        FileStreamResult Export<T>(List<T> res, string fileName, string sheetName);        
    }
}