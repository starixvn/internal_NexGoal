(function (reportModule) {
    "use strict";

    reportModule.directive("exportPa", ["baseUrl", function (baseUrl) {
        return {
            link: function (scope) {
                scope.$on("exportPAReport", function () {
                    var data = $.param(scope.paReportCtrl.filter),
                        url = baseUrl + "ExportReport/ExportEPADetailReport",
                        downloadUrl = url + "?" + data,
                        iframe = $("body iframe");

                    if (iframe.length === 0) {
                        iframe = $("<iframe width='0' height='0' class='display-none'></iframe>");
                        iframe.appendTo("body");
                    }

                    iframe.attr("src", downloadUrl);
                });
            }
        };
    }]);
})(angular.module("reportModule"));