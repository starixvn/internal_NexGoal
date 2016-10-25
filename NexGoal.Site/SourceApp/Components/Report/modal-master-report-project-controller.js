(function (reportModule) {
    "use strict";

    reportModule.controller("ModalMasterReportProjectController", modalMasterReportProjectController);

    modalMasterReportProjectController.$inject = ["$scope", "$modalInstance", "project"];

    function modalMasterReportProjectController($scope, $modalInstance, project) {
        var vm = this;

        vm.project = project;

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