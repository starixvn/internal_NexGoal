(function (reportModule) {
    "use strict";

    reportModule.controller("ModalMasterReportProjectController", modalMasterReportProjectController);

    modalMasterReportProjectController.$inject = ["$modalInstance", "project"];

    function modalMasterReportProjectController($modalInstance, project) {
        var vm = this;

        vm.project = project;

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }
})(angular.module("reportModule"));