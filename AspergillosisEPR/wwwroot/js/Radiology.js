var Radiology = function () {

    return {
        init: function () {
            SimpleDataTable.initialize("findingsDT", "table#findings_datatable", "Finding");
            SimpleDataTable.initialize("chestLocationsDT", "table#chest_locations_datatable", "ChestLocation");
            SimpleDataTable.initialize("chestDistributionsDT", "table#chest_distributions_datatable", "ChestDistribution");
            SimpleDataTable.initialize("gradesDT", "table#grades_datatable", "Grade");
            SimpleDataTable.initialize("responsesDT", "table#responses_datatable", "Response");
            SimpleDataTable.initialize("radiologyTypesDT", "table#radiology_types_datatable", "RadiologyType");
        }
    }
}();