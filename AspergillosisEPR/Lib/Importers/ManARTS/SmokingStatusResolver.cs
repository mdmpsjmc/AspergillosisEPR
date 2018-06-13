using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspergillosisEPR.Data;
using AspergillosisEPR.Models.Patients;
using NPOI.SS.UserModel;

namespace AspergillosisEPR.Lib.Importers.ManARTS
{
    public class SmokingStatusResolver
    {
        private AspergillosisContext _context;
        private string propertyValue;
        private IRow row;
        private List<string> _headers;

        public SmokingStatusResolver(AspergillosisContext context, string propertyValue, IRow row, List<string> headers)
        {
            _context = context;
            this.propertyValue = propertyValue;
            this.row = row;
            _headers = headers;
        }

        internal PatientSmokingDrinkingStatus Resolve()
        {
            var smokingDrinkingStatus = new PatientSmokingDrinkingStatus();
            var statuses = _context.SmokingStatuses.Select(s => s.Name).ToList();

            return smokingDrinkingStatus;
        }
    }
}
