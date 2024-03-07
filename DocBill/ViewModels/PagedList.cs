using System.Collections.Generic;

namespace DocBill.ViewModels
{

    public record PagedList<T>(List<T> Records, int CurrentPage, int PageCount, int Total);

}
