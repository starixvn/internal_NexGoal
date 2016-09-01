(function (assessmentModule) {
    "use strict";

    assessmentModule.factory("PAFocusModel", paFocusModel);

    paFocusModel.$inject = ["$modal", "paFocusFactory", "errorFactory", "notificationFactory"];

    function paFocusModel($modal, paFocusFactory, errorFactory, notificationFactory) {
        var vm = {};

        vm.processSettings = [];
        vm.evaluations = [];

        vm.validatePAFocusForm = function (groups) {
            var isValid = true;

            groups.forEach(function (group) {
                if (!isValid) {
                    return;
                }

                if (group.TotalBonus !== group.MaxBonus) {
                    isValid = false;
                    notificationFactory.error("TotalBonusGivenInvalidMessage");
                    return;
                }

                group.members.forEach(function (member) {
                    if (!member.NextDescriptionObjective) {
                        isValid = false;
                        notificationFactory.error("ObjectivesInvalidMessage");
                        return;
                    }
                });
            });

            return isValid;
        };

        vm.getPAProcessSettingByPeriodId = function (periodId) {
            return paFocusFactory.getPAProcessSettingByPeriodId(periodId).success(function (processSettings) {
                vm.processSettings = processSettings;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.getPAFocus = function (periodId, groupIds, employeeId) {
            return paFocusFactory.getPAFocus(periodId, groupIds, employeeId).success(function (evaluations) {
                vm.evaluations = evaluations;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.updatePAFocus = function (evaluations) {
            return paFocusFactory.updatePAFocus(evaluations).success(function (response) {
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

        vm.updatePAProcessSettingByPeriodId = function (periodId, processes) {
            return paFocusFactory.updatePAProcessSettingByPeriodId(periodId, processes).success(function (response) {
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

        vm.openPopupInformation = function () {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/Assessment/Focus/Templates/modalInformation.html',
                controller: "PAModalInformationController as paModalInformationCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center"
            });

            return vm.modalInstance.result;
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