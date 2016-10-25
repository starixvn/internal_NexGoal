(function (reportModule) {
    "use strict";

    reportModule.controller("ModalMasterReportEvaluatorCommentController", modalMasterReportEvaluatorCommentController);

    modalMasterReportEvaluatorCommentController.$inject = ["$scope", "$modalInstance", "modalTitle", "evaluatorComments"];

    function modalMasterReportEvaluatorCommentController($scope, $modalInstance, modalTitle, evaluatorComments) {
        var vm = this;

        vm.evaluatorComments = evaluatorComments;
        vm.modalTitle = modalTitle;

        function initialze() {
            $scope.$on('$routeChangeStart', function () {
                $modalInstance.dismiss("cancel");
            });
        }

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };

        initialze();
    }
})(angular.module("reportModule"));