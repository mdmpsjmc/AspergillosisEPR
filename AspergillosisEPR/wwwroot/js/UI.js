var UI = function () {

    var initAjaxTab = function () {
        $('[data-toggle="tabajax"]').click(function (e) {
            var $this = $(this),
                loadurl = $this.attr('href'),
                targ = $this.attr('data-target');

            $.get(loadurl, function (data) {
                $(targ).html(data);
                Patients.onPatientTabChange();
            });

            $this.tab('show');
            return false;
        });
    }

    return {
        init: function () {
            initAjaxTab();            
        }, 

        initAjaxTab: function () {
            initAjaxTab();
        }

    }
}();