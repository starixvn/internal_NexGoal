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

        vm.updatePAProcessSettingByPeriodId = function (processes) {
            return paFocusFactory.updatePAProcessSettingByPeriodId(processes).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                }
                else {
                    notificationFactory.error("UpdatedUnsuccessfully");
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.openPopupObjective = function (member) {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/Assessment/Focus/Templates/modalObjective.html',
                controller: "PAModalObjectiveController as paModalObjectiveCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center",
                resolve: {
                    member: function () {
                        return angular.copy(member);
                    }
                }
            });

            return vm.modalInstance.result;
        };

        return vm;
    }
})(angular.module("assessmentModule"));