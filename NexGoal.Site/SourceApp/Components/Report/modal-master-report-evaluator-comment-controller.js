(function(reportModule){
    "use strict";

    reportModule.controller("ModalMasterReportEvaluatorCommentController", modalMasterReportEvaluatorCommentController);

    modalMasterReportEvaluatorCommentController.$inject = ["$modalInstance", "modalTitle", "evaluatorComments"];

    function modalMasterReportEvaluatorCommentController($modalInstance, modalTitle, evaluatorComments) {
        var vm = this;
        
        vm.evaluatorComments = evaluatorComments;
        vm.modalTitle = modalTitle;

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }
})(angular.module("reportModule"));