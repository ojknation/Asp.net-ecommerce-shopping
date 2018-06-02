using Shop14.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shop14.Models.ViewModels.Pages
{
    public class LogVM
    {
        public LogVM()
        {
        }

        public LogVM(LogDTO row)
        {
            Id = row.Id;
            ProductType = row.ProductType;
            ProductCategory = row.ProductCategory;
            LogTrackNumber = row.LogTrackNumber;
        }
        public int Id { get; set; }
        public string ProductType { get; set; }
        public string ProductCategory { get; set; }
        public int LogTrackNumber { get; set; }
    }
}