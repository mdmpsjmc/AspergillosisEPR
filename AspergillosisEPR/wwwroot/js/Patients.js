var Patients = function() {

    var initPatientsDataTable = function () {
        $(document).ready(function () {
            $("#patients_datatable").DataTable({
                "processing": true, // for show progress bar
                "serverSide": true, // for process server side
                "filter": true, // this is for disable filter (search box)
                "orderMulti": false, // for disable multiple column at once
                "ajax": {
                    "url": "/Patients/LoadData",
                    "type": "POST",
                    "datatype": "json"
                },
                "columns": [
                    { "data": "rM2Number", "name": "RM2 Number", "autoWidth": true },
                    { "data": "firstName", "name": "RM2 Number", "autoWidth": true },
                    { "data": "lastName", "name": "Full Name", "autoWidth": true },
                    { "data": "gender", "name": "Gender", "autoWidth": true },
                    { "data": "dob", "name": "Date of Birth", "autoWidth": true },
                    {
                        "render": function (data, type, patient, meta) {
                            return '<a class="btn btn-info btn-xs" href="javascript:void(0)" onclick=\'$(\"#patient-modal\").modal(\"show\")\'"><i class=\'fa fa-edit\'></i>&nbsp;Edit</a>&nbsp;' +
                                '<a class="btn btn-primary btn-xs" href="/DemoGrid/Details/' + patient.id + '"><i class=\'fa fa-eye\' ></i>&nbsp;Details</a>&nbsp;' +
                                '<a class="btn btn-danger btn-xs" href="/DemoGrid/Details/' + patient.id + '"><i class=\'fa fa-trash\' ></i>&nbsp;Delete</a>&nbsp;';
                        }
                    }
                ]

            });
        });
    }

    return {
        init: function () {
            initPatientsDataTable();
        }
    }

}();