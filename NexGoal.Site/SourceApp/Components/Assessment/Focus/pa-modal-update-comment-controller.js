(function (assessmentModule) {
    "use strict";

    assessmentModule.controller("PAModalUpdateCommentController", paModalUpdateCommentController);

    paModalUpdateCommentController.$inject = ["$modalInstance"];

    function paModalUpdateCommentController($modalInstance) {
        var vm = this;

        vm.ok = function () {
            $modalInstance.close();
        };

        vm.cancel = function () {
            $modalInstance.dismiss("cancel");
        };
    }
})(angular.module("assessmentModule"));