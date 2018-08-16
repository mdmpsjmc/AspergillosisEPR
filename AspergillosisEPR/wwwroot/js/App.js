var App = function() {

    var onLogoutLinkClick = function () {
        $(document).off("click.logout").on("click.logout", "a.logout-link", function () {
            var logoutUrl = $(this).attr("href");
            $.ajax({
                url: logoutUrl,
                method: "POST", 
                data: {},
                contentType: "application/x-www-form-urlencoded"
            }).done(function () {
                window.location.href = "/Account/Login";
            });
        });
    }

    return {
        init: function () {
            onLogoutLinkClick();
            Patients.init();
            Settings.init()
            Drugs.init();
            Users.init();
            Imports.init();
            DiagnosisTypes.init();
            SideEffects.init();
            Charts.init();
            Search.init();
            Radiology.init();
            PatientVisits.init();
            CaseReportForms.init();
            AnonPatients.init();
            MedicalTrials.init();
            AllergyIntolerance.init();
            UI.init();
            Reports.init();
        }
    }

}();