(function (reportModule) {
    "use strict";

    reportModule.controller("ModalMasterReportEvaluatorCommentController", modalMasterReportEvaluatorCommentController);

    modalMasterReportEvaluatorCommentController.$inject = ["$modalInstance", "evaluatorComment"];

    function modalMasterReportEvaluatorCommentController($modalInstance, evaluatorComment) {
        var vm = this;

        vm.evaluatorComment = evaluatorComment;

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }
})(angular.module("reportModule"));