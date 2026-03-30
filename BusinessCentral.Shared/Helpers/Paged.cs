using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCentral.Shared.Helper
{
    public class Paged<T>
    {
        public int TotalRecords { get; set; }
        public int CurrentPageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => Convert.ToInt32(Math.Ceiling((double)TotalRecords / PageSize));
        public bool HasNextPage => CurrentPageNumber < TotalPages;
        public bool HasPreviousPage => CurrentPageNumber > 1;
        public string PagingLabel => $"{(CurrentPageNumber - 1) * PageSize + 1}-{Math.Min(CurrentPageNumber * PageSize, TotalRecords)} de {TotalRecords} registros";
        public List<T> Records { get; set; }
    }
}
