(function (reportModule) {
    "use strict";

    reportModule.controller("ModalMasterReportCommentController", modalMasterReportCommentController);

    modalMasterReportCommentController.$inject = ["$modalInstance", "modalTitle", "comment"];

    function modalMasterReportCommentController($modalInstance, modalTitle, comment) {
        var vm = this;

        vm.comment = comment;
        vm.modalTitle = modalTitle;

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }
})(angular.module("reportModule"));