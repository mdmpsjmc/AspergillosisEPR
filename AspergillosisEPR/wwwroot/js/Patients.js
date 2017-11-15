var Patients = function () {

    var initPatientsDataTable = function () {
        $(document).ready(function () {
            window.patientsTable = $("#patients_datatable").DataTable({
                "processing": true, 
                "serverSide": true,
                "filter": true,
                "orderMulti": false, 
                "initComplete": function (settings, json) {
                    newPatientsModalShow();
                },
                "ajax": {
                    "url": "/Patients/LoadData",
                    "type": "POST",
                    "datatype": "json"
                },
                "columns": [
                    { "data": "rM2Number", "name": "RM2Number", "autoWidth": true },
                    { "data": "firstName", "name": "FirstName", "autoWidth": true },
                    { "data": "lastName", "name": "LastName", "autoWidth": true },
                    { "data": "gender", "name": "Gender", "autoWidth": true },
                    { "data": "dob", "name": "DOB", "autoWidth": true },
                    {
                        "render": function (data, type, patient, meta) {
                            return '<a class="btn btn-info btn-xs patient-details" href="/Patients/Details/' + patient.id + '"><i class=\'fa fa-eye\'></i>&nbsp;Details</a>&nbsp;' +
                                '<a class="btn btn-warning btn-xs" href="/DemoGrid/Details/' + patient.id + '"><i class=\'fa fa-edit\' ></i>&nbsp;Edit</a>&nbsp;' +
                                '<a class="btn btn-danger btn-xs" href="/DemoGrid/Details/' + patient.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;';
                        }
                    }
                ]

            });
        });
    }

    var submitNewPatient = function () {
        $(document).off("click.save-patient").on("click.save-patient", "button.submit-new-patient", function () {
            $("label.text-danger").remove();
            $.ajax({
                url: $("form#new-patient-form").attr("action"),
                type: "POST",
                data: $("form#new-patient-form").serialize(),
                contentType: "application/x-www-form-urlencoded",
                dataType: 'json'
            }).done(function (data, textStatus) {
                if (textStatus === "success") {
                    if (data.errors) {
                        displayErrors(data.errors);
                    } else {
                        $("form#new-patient-form")[0].reset();
                        $("div#new-patient-modal").modal("hide");
                        window.patientsTable.ajax.reload();
                    }
                 }
                }).fail(function (data) {
                    $("form#new-patient-form")[0].reset();
                    $("div#new-patient-modal").modal("hide");
                    alert("There was a problem saving this patient. Please contact administrator");
            });

        });
    }

    var displayErrors = function(errors) {
        for (var i = 0; i < Object.keys(errors).length; i++) {
            var field = Object.keys(errors)[i];
            var htmlCode = "<label for='" + field + "' class='text-danger'></label>";
            var fieldError = errors[Object.keys(errors)[i]];
            $(htmlCode).html(fieldError).appendTo($("input#" + field + ", select#" + field).parent());
        }
    }

    var enableAntiForgeryProtectionWithAjax = function () {
        $(document)
            .ajaxSend(function (event, jqxhr, settings) {
                if (settings.type.toUpperCase() !== "POST") return;
                jqxhr.setRequestHeader('RequestVerificationToken', $(".AntiForge" + " input").val())
            })
    };

    var newPatientsModalShow = function () {
        $(document).off("click.launch-new-patient-modal").on("click.launch-new-patient-modal", "a.new-patient-modal-show", function () {
            $.get("/Patients/New", function (responseHtml){
                $("div#modal-container").html(responseHtml);
                $("div#new-patient-modal").modal("show");
            });
        });
    }

    var bindDiagnosisFormOnClick = function () {
        $(document).off("click.add-diagnosis").on("click.add-diagnosis", "a.add-diagnosis", function (e) {
            e.preventDefault();
            $.get($(this).attr("href"), function (responseHtml) {
                $("div.diagnosis-form").append(responseHtml);
            });
         })
    }

    var bindDrugsFormOnClick = function () {
        $(document).off("click.add-drug").on("click.add-drug", "a.add-drug", function (e) {
            e.preventDefault();
            $.get($(this).attr("href"), function (responseHtml) {
                $("div.drug-form").append(responseHtml);
            });
        })
    }

    var bindPatientDetailsShow = function () {
        $(document).off("click.patient-details").on("click.patient-details", "a.patient-details", function (e) {
            e.preventDefault();
            $.get($(this).attr("href"), function (responseHtml) {
                $("div#modal-container").html(responseHtml);
                $("div#details-modal").modal("show");
            });
        });
    }

    return {

        bindShowPatientsModal: function () {
            newPatientsModalShow();
            bindDiagnosisFormOnClick();
            bindDrugsFormOnClick();
            bindPatientDetailsShow();
        },

        setupForm: function () {
            submitNewPatient();
            enableAntiForgeryProtectionWithAjax();
        },

        init: function () {
            initPatientsDataTable();
            submitNewPatient();
            enableAntiForgeryProtectionWithAjax();
        }
    }

}();