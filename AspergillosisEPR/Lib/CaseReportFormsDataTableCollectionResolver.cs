using AspergillosisEPR.Data;
using AspergillosisEPR.Models.CaseReportForms;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib
{
    public class CaseReportFormsDataTableCollectionResolver
    {
        private AspergillosisContext _context;
        private string _collectionName;

        public CaseReportFormsDataTableCollectionResolver(AspergillosisContext context, string collectionName)
        {
            _collectionName = collectionName;
            _context = context;
        }

        public IQueryable Resolve()
        {
            switch (_collectionName)
            {
                case "CaseReportFormOptionGroup":
                    return _context.Query(typeof(CaseReportFormOptionGroup));
                case "CaseReportFormSection":
                    return _context.Query(typeof(CaseReportFormSection));
                case "CaseReportFormFieldType":
                    return _context.Query(typeof(CaseReportFormFieldType));
                case "CaseReportFormResult":
                    return _context.Query(typeof(CaseReportFormResult));
            }
            return null;
        }
    }
}
