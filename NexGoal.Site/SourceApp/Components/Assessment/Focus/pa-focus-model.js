(function (assessmentModule) {
    "use strict";

    assessmentModule.factory("PAFocusModel", paFocusModel);

    paFocusModel.$inject = ["$modal", "paFocusFactory", "errorFactory"];

    function paFocusModel($modal, paFocusFactory, errorFactory) {
        var vm = {};

        vm.processSettings = [];

        vm.getPAProcessSettingByPeriodId = function (periodId) {
            return paFocusFactory.getPAProcessSettingByPeriodId(periodId).success(function (processSettings) {
                vm.processSettings = processSettings;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.openPopupUpdateComment = function (member) {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/Assessment/Focus/Templates/modalUpdateComment.html',
                controller: "PAModalUpdateCommentController as paModalUpdateCommentCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center"
            });

            return vm.modalInstance.result;
        };

        return vm;
    }
})(angular.module("assessmentModule"));