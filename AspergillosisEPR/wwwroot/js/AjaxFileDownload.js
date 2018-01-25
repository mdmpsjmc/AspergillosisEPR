var AjaxFileDownload = function () {
    return {

        execute: function (url, postData, fileName) {
            var req = new XMLHttpRequest();
            req.open("POST", url, true);

            req.responseType = "blob";

            req.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
            req.send(postData);
            req.onload = function (event) {
                var blob = req.response;
                download(blob, fileName, "application/pdf");
            };
        }

    }
}();    