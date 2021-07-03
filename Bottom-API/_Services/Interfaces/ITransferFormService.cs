using System.Collections.Generic;
using System.Threading.Tasks;
using Bottom_API.DTO;
using Bottom_API.DTO.TransferForm;
using Bottom_API.Helpers;

namespace Bottom_API._Services.Interfaces
{
    public interface ITransferFormService
    {
        Task<PagedList<Transfer_Form_Generate_Dto>> GetTransferFormGerenate(string fromTime, string toTime, string moNo, string isSubcont, string t2Supplier, string t3Supplier, int pageNumber = 1, int pageSize = 10);
        Task<bool> GenerateTransferForm(List<Transfer_Form_Generate_Dto> generateTransferForm, string updateBy);
        Task<PagedList<Transfer_Form_Generate_Dto>> GetTransferFormPrint(string fromTime, string toTime, string moNo, string isRelease, string t2Supplier, string t3Supplier, int pageNumber = 1, int pageSize = 10);
        Task<bool> ReleaseTransferForm(List<Transfer_Form_Generate_Dto> generateTransferForm, string updateBy);
        Task<List<Transfer_Form_Print_Dto>> GetInfoTransferFormPrintDetail(List<Transfer_Form_Generate_Dto> generateTransferForm);
        Task SendEmail(Transfer_Form_Generate_Dto generateTransferForm,string pathFileExcel);
        Task<List<Transfer_Form_Excel_Dto>> GetDataExcelTransferForm(List<Transfer_Form_Generate_Dto> generateTransferForm);
    }
}