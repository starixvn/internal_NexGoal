(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("UpdateEOQNomineeController", updateEOQNomineeController);

    updateEOQNomineeController.$inject = ["$modalInstance"];

    function updateEOQNomineeController($modalInstance) {
        var vm = this;
    }

    vm.submit = function () {
        $modalInstance.dismiss("cancel");
    };

    vm.cancel = function () {
        $modalInstance.dismiss("cancel");
    };

})(angular.module("assessmentModule"));