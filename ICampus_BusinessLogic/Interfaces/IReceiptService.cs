using System.Collections.Generic;
using System.Threading.Tasks;
using ICampus_Models.Requests;

public interface IReceiptService
{
    Task<IEnumerable<object>> LoadReceiptCollectionAsync(ReceiptCollectionRequest request);
    Task<object> LoadReceiptDetailAsync(ReceiptDetailRequest request);
    Task<IEnumerable<object>> SearchByRegnoAsync(string regno);
}
