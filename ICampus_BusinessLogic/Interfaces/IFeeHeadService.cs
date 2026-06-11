// ICampus_BusinessLogic.Interfaces/IFeeHeadService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using ICampus_Models.Requests;

public interface IFeeHeadService
{
    Task<IEnumerable<object>> GetHeadsAsync(string course);
    Task<int> SaveHeadAsync(FeeHeadRequest req);
    Task<int> DeleteHeadAsync(int id);
}
