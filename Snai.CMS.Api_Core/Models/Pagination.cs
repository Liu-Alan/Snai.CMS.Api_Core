using Microsoft.AspNetCore.Mvc;
using Snai.CMS.Api_Core.Common.Infrastructure.Validation;
using System.ComponentModel.DataAnnotations;

namespace Snai.CMS.Api_Core.Models
{
    public class PageIn
    {
        [ModelBinder(Name = "quiry_title")]
        public string? QuiryTitle { get; set; }

        [ModelBinder(Name = "page")]
        [PositiveIntegerAttribute(ErrorMessage = "页码须为数字")]
        public int? Page { get; set; }

        [ModelBinder(Name = "page_size")]
        [PositiveIntegerAttribute(ErrorMessage = "每页数量须为数字")]
        public int? PageSize { get; set; }
        
    }

    public class PageOut<T>
    {
        public int Page { get; set; }
        
        public int PageSize { get; set; }

        public long Total {  get; set; }
        public IList<T> PageList { get; set; }
    }
}
