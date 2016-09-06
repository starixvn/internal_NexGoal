(function (assessmentModule) {
    "use strict";

    assessmentModule.factory("NominationModel", nominationModel);

    nominationModel.$inject = ["$modal", "notificationFactory"];

    function nominationModel($modal, notificationFactory) {
        var vm = {};

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