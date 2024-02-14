using OfficeOpenXml;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    /*
 Note: the following is an example of applying Open Closed principle by re-implementing the GetPersonsExcel method
This can be done in another way using inheritance (see lec.299), but this way using interfaces is usually better
 */
    public class PersonsGetterServiceWithFewerExcelFields : IPersonsGetterService
    {
        private readonly PersonsGetterService _personsGetterService;

        public PersonsGetterServiceWithFewerExcelFields(PersonsGetterService personsGetterService)
        {
            _personsGetterService = personsGetterService;
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            return await _personsGetterService.GetAllPersons();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            return await GetFilteredPersons(searchBy, searchString);
        }

        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            return await _personsGetterService.GetPersonByPersonID(personID);
        }

        public async Task<MemoryStream> GetPersonsCSV()
        {
            return await _personsGetterService.GetPersonsCSV();
        }

        public async Task<MemoryStream> GetPersonsExcel()
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
