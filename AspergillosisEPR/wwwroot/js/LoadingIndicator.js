var LoadingIndicator = function () {

    return {
        show: function () {
           $('#loading-indicator').show();
           $('#loading').show();
        },

        hide: function () {
            $('#loading-indicator').hide();
            $('#loading').hide();
        }
    }

}();