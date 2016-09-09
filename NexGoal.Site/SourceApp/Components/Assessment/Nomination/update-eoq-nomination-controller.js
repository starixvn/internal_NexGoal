(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("UpdateEOQNominationController", updateEOQNominationController);

    updateEOQNominationController.$inject = ["$modalInstance"];

    function updateEOQNominationController($modalInstance) {
        var vm = this;

        vm.submit = function () {
            $modalInstance.dismiss("cancel");
        };

        vm.cancel = function () {
            $modalInstance.dismiss("cancel");
        };
    }

})(angular.module("assessmentModule"));