using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RepositoryContracts;
using Serilog;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    /*Applying OCP using inheritence instead of an interface*/
    public class PersonsGetterServiceChild : PersonsGetterService
    {
        public PersonsGetterServiceChild(IPersonsRepository personsRepository, ILogger<PersonsGetterService> logger, IDiagnosticContext diagnostic) : base(personsRepository, logger, diagnostic)
        {
        }

        public override async Task<MemoryStream> GetPersonsExcel()
        {
            MemoryStream memoryStream = new MemoryStream();
            //from EPPlus package..
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("PersonsSheet");
                workSheet.Cells["A1"].Value = "Person Name";
                workSheet.Cells["B1"].Value = "Person Email";
                workSheet.Cells["C1"].Value = "Date of Birth";
                //workSheet.Cells["D1"].Value = "Country";

                int row = 2;
                List<PersonResponse> persons = await GetAllPersons();

                foreach (PersonResponse person in persons)
                {
                    //[row, column]
                    workSheet.Cells[row, 1].Value = person.PersonName;
                    workSheet.Cells[row, 2].Value = person.Email;
                    if (person.DateOfBirth != null) { workSheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd"); }
                    //workSheet.Cells[row, 4].Value = person.Country;

                    //formatting header cells
                    using (ExcelRange headerCells = workSheet.Cells["A1,C1"])
                    {
                        headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        headerCells.Style.Font.Bold = true;
                    }

                    row++;
                }
                //### google epplus for documentation (more features etc...) ###

                workSheet.Cells[$"A1:C{row}"].AutoFitColumns();

                await excelPackage.SaveAsync();
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}
