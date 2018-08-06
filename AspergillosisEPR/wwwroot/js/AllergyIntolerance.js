var AllergyIntolerance = function() {

    var initializeIntoleranceTypesSelect = function () {
        $(document).off("change.select-intolerance").on("change.select-intolerance", "select.intolerance-type-select", function () {
            var partial = $(this).val();
            var realIndex = $(this).attr("id").replace("Allergies_", "").replace("__AllergyIntoleranceItemType", "");
            var index = $(this).parents("div.patient-allergy:visible").length - 1;
            if (index < 0) index = 0;
            var currentItemIndex = $(this).data("index");
            if (realIndex.length > 20) index = realIndex;
            $.get("/Partials/ByName/?partialName=" + partial + "&index=" + index, function (responseHtml) {
                $("[data-index='" + currentItemIndex + "']#allergy-intolerance-item-id").html(responseHtml);
                $("select.selectize").selectize();
            });
        });
    }

    return {
        init: function() {
            initializeIntoleranceTypesSelect();
        }
    }


}();