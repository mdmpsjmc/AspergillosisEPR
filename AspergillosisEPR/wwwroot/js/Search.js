var Search = function () {

    var initializeSearch = function () {

    }

    var onAddSearchCriteriaClick = function () {
        $(document).off("click.add-criteria").on("click.add-criteria", "a.add-search-criteria", function (e) {
            LoadingIndicator.show();
            e.preventDefault();
            var index = $("div.search-criteria-row:visible").length;
            $.get($(this).attr("href") + "?index=" + index, function (responseHtml) {
                LoadingIndicator.hide();
                $("fieldset#search-form-container").append(responseHtml);
            });
        });
    }
   
    return {
        init: function () {
            initializeSearch();
            onAddSearchCriteriaClick();
        }
    }
}();