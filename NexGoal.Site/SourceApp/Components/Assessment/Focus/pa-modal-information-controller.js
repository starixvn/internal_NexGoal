(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("PAModalInformationController", paModalInformationController);

    paModalInformationController.$inject = ["$modalInstance"];

    function paModalInformationController($modalInstance) {
        var vm = this;

        vm.close = function () {
            $modalInstance.dismiss("cancel");
        };
    }
})(angular.module("assessmentModule"));