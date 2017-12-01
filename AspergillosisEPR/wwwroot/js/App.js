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
        }
    }

}();