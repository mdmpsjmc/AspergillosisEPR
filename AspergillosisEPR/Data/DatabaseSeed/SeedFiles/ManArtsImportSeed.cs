using AspergillosisEPR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Data.DatabaseSeed.SeedFiles
{
    public static class ManArtsImportSeed
    {
        public static void Seed(AspergillosisContext context)
        {
            BuildPulmonaryFunctionTests(context);
            BuildSurgeries(context);
            BuildSmokingStatuses(context);
            context.SaveChanges();
        }

        private static void BuildSmokingStatuses(AspergillosisContext context)
        {
            if (context.SmokingStatuses.Any())
            {
                return;
            }

            var sts = new SmokingStatus[]
            {
                new SmokingStatus { Name = "Current" },
                new SmokingStatus { Name = "Ex-Smoker" },
                new SmokingStatus { Name = "Don't know" },
                new SmokingStatus { Name = "Never" }
            };
            foreach (var s in sts)
            {
                context.Add(s);
            }
        }

        private static void BuildSurgeries(AspergillosisContext context)
        {
            if (context.Surgeries.Any())
            {
                return;
            }

            var sgurgeries = new Surgery[]
            {
                new Surgery { Name = "Pneumonectomy" },
                new Surgery { Name = "Lobectomy" },
                new Surgery { Name = "Thoracotomy" },
                new Surgery { Name = "Wedge resection" },
                new Surgery { Name = "Pleurectomy" },
                new Surgery { Name = "Bullectomy" }
            };
            foreach (var s in sgurgeries)
            {
                context.Add(s);
            }
        }

        private static void BuildPulmonaryFunctionTests(AspergillosisContext context)
        {
            if (context.PulmonaryFunctionTests.Any())
            {
                return;
            }

            var tests = new PulmonaryFunctionTest[]
            {
                new PulmonaryFunctionTest { Name = "Total lung capacity", ShortName = "TLC", Description = "The volume in the lungs at maximal inflation, the sum of VC and RV"},
                new PulmonaryFunctionTest { Name = "Tidal Volume", ShortName = "TV", Description = "That volume of air moved into or out of the lungs during quiet breathing (TV indicates a subdivision of the lung; when tidal volume is precisely measured, as in gas exchange calculation, the symbol TV or VT is used.)"},
                new PulmonaryFunctionTest { Name = "Residual Volume", ShortName = "RV", Description = "The volume of air remaining in the lungs after a maximal exhalation" },
                new PulmonaryFunctionTest { Name = "Expiratory Reserve Volume", ShortName = "ERV", Description = "The maximal volume of air that can be exhaled from the end-expiratory position"},
                new PulmonaryFunctionTest { Name = "Inspiratory Reserve Volume", ShortName = "IRV", Description = "The maximal volume that can be inhaled from the end-inspiratory level" },
                new PulmonaryFunctionTest { Name = "Inspiratory Capacity", ShortName = "IC", Description = "The sum of IRV and TV"},
                new PulmonaryFunctionTest { Name = "Inspiratory Vital Capacity", ShortName = "IVC", Description = "The maximum volume of air inhaled from the point of maximum expiration"},
                new PulmonaryFunctionTest { Name = "Vital Capacity", ShortName = "VC", Description = "The volume of air breathed out after the deepest inhalation." },
                new PulmonaryFunctionTest { Name = "Functional Residual Capacity", ShortName = "FRC", Description = "The volume in the lungs at the end-expiratory position" },
                new PulmonaryFunctionTest { Name = "Residual volume expressed as percent of TLC", ShortName = "RV/TLC%"},
                new PulmonaryFunctionTest { Name = "Alveolar gas volume", ShortName = "VA"},
                new PulmonaryFunctionTest { Name = "Actual volume of the lung including the volume of the conducting airway.", ShortName = "VL" },
                new PulmonaryFunctionTest { Name = "Forced Vital Capacity", ShortName = "FVC", Description = "The determination of the vital capacity from a maximally forced expiratory effort" },
                new PulmonaryFunctionTest { Name = "Forced Expiratory Volume (time)", ShortName = "FEVt", Description = "The generic term indicating the volume of air exhaled under forced conditions in the first t seconds" },
                new PulmonaryFunctionTest { Name = "Forced Expiratory Volume 1", ShortName = "FEV1", Description = "Volume that has been exhaled at the end of the first second of forced expiration" },
                new PulmonaryFunctionTest { Name = "Forced Expiratory Flow", ShortName = "FEFx", Description = "Related to some portion of the FVC curve; modifiers refer to amount of FVC already exhaled"},
                new PulmonaryFunctionTest { Name = "Maximum Instantaneous Flow", ShortName = "FEFmax", Description = "Maximum Instantaneous Flow achieved during a FVC maneuver"},
                new PulmonaryFunctionTest { Name = "Forced Inspiratory Flow", ShortName = "FIF", Description = "Specific measurement of the forced inspiratory curve is denoted by nomenclature analogous to that for the forced expiratory curve. For example, maximum inspiratory flow is denoted FIFmax. Unless otherwise specified, volume qualifiers indicate the volume inspired from RV at the point of measurement" },
                new PulmonaryFunctionTest { Name = "Peak Expiratory Flow", ShortName = "PEF", Description = " The highest forced expiratory flow measured with a peak flow meter" },
                new PulmonaryFunctionTest { Name = "Maximal voluntary ventilation", ShortName = "MVV", Description = "Volume of air expired in a specified period during repetitive maximal effort" },
            };

            foreach (var test in tests)
            {
                context.Add(test);
            }
        }

        public static void AddOtherPFTs(AspergillosisContext context)
        {

            var fefTest = context.PulmonaryFunctionTests
                                 .FirstOrDefault(pft => pft.ShortName == "FEFx");
            if (fefTest == null) return;
            context.Remove(fefTest);

            var tests = new PulmonaryFunctionTest[]
            {
                new PulmonaryFunctionTest { Name = "Forced Expiratory Flow - 25%", ShortName = "FEF25", Description = "Related to some portion of the FVC curve; modifiers refer to amount of FVC already exhaled" },
                new PulmonaryFunctionTest { Name = "Forced Expiratory Flow - 50%", ShortName = "FEF50", Description = "Related to some portion of the FVC curve; modifiers refer to amount of FVC already exhaled" },
                new PulmonaryFunctionTest { Name = "Forced Expiratory Flow - 75%", ShortName = "FEF75", Description = "Related to some portion of the FVC curve; modifiers refer to amount of FVC already exhaled" },
                new PulmonaryFunctionTest { Name = "The diffusing capacity for carbon monoxide ", ShortName = "DLCO", Description = "Extent to which oxygen passes from the air sacs of the lungs into the blood" },
                new PulmonaryFunctionTest { Name = "The carbon monoxide transfer coefficient ", ShortName = "KCO", Description = "(KCO is approximately kCO/barometric pressure in mL/minute/ mmHg/L) is often written as DLCO/VA. It is an index of the efficiency of alveolar transfer of carbon monoxide" },
                new PulmonaryFunctionTest { Name = "Slow Vital Capacity ", ShortName = "SVC", Description = "The full excursion of the maneuver gives a measure of the change in volume of gas in the lungs from complete inspiration to complete expiration or vice versa. " },
            };
            foreach (var test in tests)
            {
                context.Add(test);
            }
            context.SaveChanges();
        }
    }
}