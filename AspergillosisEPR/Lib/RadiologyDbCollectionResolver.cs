using AspergillosisEPR.Data;
using AspergillosisEPR.Models;
using AspergillosisEPR.Models.Radiology;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib
{
    public class RadiologyDbCollectionResolver
    {
        private AspergillosisContext _context;
        private string _collectionName;

        public RadiologyDbCollectionResolver(AspergillosisContext context, string collectionName)
        {
            _collectionName = collectionName;
            _context = context;
        }

        public IQueryable Resolve()
        {
            switch (_collectionName)
            {
                case "Finding":
                    return _context.Query(typeof(Finding));
                case "ChestLocation":
                    return _context.Query(typeof(ChestLocation));
                case "ChestDistribution":
                    return _context.Query(typeof(ChestDistribution));
                case "Grade":
                    return _context.Query(typeof(Grade));
                case "TreatmentResponse":
                    return _context.Query(typeof(TreatmentResponse));
                case "RadiologyType":
                    return _context.Query(typeof(RadiologyType));
            }
            return null;
        }
    }
}
