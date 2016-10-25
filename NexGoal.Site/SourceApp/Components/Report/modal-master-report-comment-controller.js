(function (reportModule) {
    "use strict";

    reportModule.controller("ModalMasterReportCommentController", modalMasterReportCommentController);

    modalMasterReportCommentController.$inject = ["$scope", "$modalInstance", "modalTitle", "comment"];

    function modalMasterReportCommentController($scope, $modalInstance, modalTitle, comment) {
        var vm = this;

        vm.comment = comment;
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