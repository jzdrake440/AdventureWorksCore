using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdventureWorksCore.Models.DataTables
{
    public class DataTableServerSideResponse<T>
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public List<T> Data { get; set; }
        public string error { get; set; }
    }
}