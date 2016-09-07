(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("UpdateEOQController", updateEOQController);

    updateEOQController.$inject = [];

    function updateEOQController() {
        var vm = this;

        vm.Title = "Update EOQ Controller";
    }
})(angular.module("assessmentModule"));