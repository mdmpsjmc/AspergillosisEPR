using AspergillosisEPR.Data;
using AspergillosisEPR.Models.CaseReportForms;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.CaseReportForms
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
                case "CaseReportFormSection":
                    return _context.Query(typeof(CaseReportFormSection));
                case "CaseReportFormFieldType":
                    return _context.Query(typeof(CaseReportFormFieldType));
                case "CaseReportForm":
                    return _context.Query(typeof(CaseReportForm));
                case "CaseReportFormCategory":
                    return _context.Query(typeof(CaseReportFormCategory));

            }
            return null;
        }
    }
}
