var Search = function () {

    var initializeSearch = function () {

    }

    var onAddSearchCriteriaClick = function () {
        $(document).off("click.add-criteria").on("click.add-criteria", "a.add-search-criteria", function (e) {
            $(this).remove();
            LoadingIndicator.show();
            e.preventDefault();
            var index = $("div.search-criteria-row:visible").length;
            $.get($(this).attr("href") + "?index=" + index, function (responseHtml) {
                LoadingIndicator.hide();
                $("fieldset:last").after(responseHtml);
                $("fieldset:last section:first").removeClass("hide");
            });
        });
    }

    var onSearchClassSelectChange = function () {
        $(document).off("change.select-searchclass").on("change.select-searchclass", "select.criteria-class", function () {
            var selectedValue = $(this).val();
            var requestUrl = $(this).data("url");
            var index = $("div.search-criteria-row:visible").length;
            var nextSelect = $(this).parents("section").next("section").children("label");

            $.get(requestUrl + "?searchClass=" + selectedValue + "&index=" + index, function (responseHtml) {
                LoadingIndicator.hide();
                nextSelect.html(responseHtml);
            });
        });
    }

    var onFieldSelectChangeAddDatepicker = function() {
        $(document).off("change.select-field").on("change.select-field", "select.criteria-field", function () {
            var selectedText = $(this).find("option:selected").text();
            var searchField = $(this).parents("section").next("section").next("section").find("input[type='text']");
            searchField.val("");
            if (selectedText.match(/Date/) !== null) {
                searchField.addClass("datepicker");
                $('input.datepicker').datetimepicker({
                    format: 'YYYY-MM-DD'
                });
            } else if (selectedText.match(/Date/) === null) {
                searchField.removeClass("datepicker");
                searchField.datetimepicker("destroy");
            }
        });
        
    }
   
    return {
        init: function () {
            initializeSearch();
            onAddSearchCriteriaClick();
            onSearchClassSelectChange();    
            onFieldSelectChangeAddDatepicker();
        }
    }
}();