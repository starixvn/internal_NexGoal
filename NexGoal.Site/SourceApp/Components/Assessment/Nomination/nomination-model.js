(function (assessmentModule) {
    "use strict";

    assessmentModule.factory("NominationModel", nominationModel);

    nominationModel.$inject = ["$modal", "nominationFactory", "errorFactory", "notificationFactory"];

    function nominationModel($modal, nominationFactory, errorFactory, notificationFactory) {
        var vm = {};

        vm.periods = [];

        vm.createEmployeeOfQuarterPeriod = function (period) {
            return nominationFactory.createEmployeeOfQuarterPeriod(period).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("CreatedSuccessfully");
                }
                else {
                    notificationFactory.error("", response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.updateEmployeeOfQuarterPeriod = function (period) {
            return nominationFactory.updateEmployeeOfQuarterPeriod(period).success(function (response) {
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

        vm.getEmployeeOfQuarterPeriod = function (year, status) {
            return nominationFactory.getEmployeeOfQuarterPeriod(year, status).success(function (periods) {
                vm.periods = periods;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.openPopupCreateNominationPeriod = function () {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/Assessment/Nomination/Templates/modalCreateNominationPeriod.html',
                controller: "CreateNominationPeriodController as createNominationPeriodCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center period-create-popup"
            });

            return vm.modalInstance.result;
        };

        vm.calendar = {
            startDateOpened: false,
            endDateOpened: false
        };

        vm.openPopupStartDate = function (event) {
            event.preventDefault();
            event.stopPropagation();

            vm.calendar.startDateOpened = true;
            vm.calendar.endDateOpened = false;
        };

        vm.openPopupEndDate = function (event) {
            event.preventDefault();
            event.stopPropagation();

            vm.calendar.startDateOpened = false;
            vm.calendar.endDateOpened = true;
        };

        vm.validatePeriod = function (period) {
            if (!period) {
                return false;
            } else {
                if (!period.Quarter) {
                    notificationFactory.error("QuarterRequired");
                    return false;
                }

                if (!period.Year) {
                    notificationFactory.error("YearRequired");
                    return false;
                }

                if (!period.RawStartDate) {
                    notificationFactory.error("StartDateRequired");
                    return false;
                }

                if (!period.RawEndDate) {
                    notificationFactory.error("EndDateRequired");
                    return false;
                }

                if (new Date(period.RawStartDate).setHours(0, 0, 0, 0) > new Date(period.RawEndDate).setHours(0, 0, 0, 0)) {
                    notificationFactory.error("StartDateGreaterThanEndDateWarning");
                    return false;
                }
            }

            return true;
        };

        return vm;
    }
})(angular.module("assessmentModule"));