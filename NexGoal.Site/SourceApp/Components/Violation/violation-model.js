(function (violationModule) {
    "use strict";

    violationModule.factory("ViolationModel", violationModel);

    violationModel.$inject = ["$modal", "notificationFactory", "errorFactory", "violationFactory"];

    function violationModel($modal, notificationFactory, errorFactory, violationFactory) {
        var model = {};

        model.calendar = {
            fromDateOpened: false,
            toDateOpened: false
        };

        model.openPopupFromDate = function (event) {
            event.preventDefault();
            event.stopPropagation();

            model.calendar.fromDateOpened = true;
            model.calendar.toDateOpened = false;
        };

        model.openPopupToDate = function (event) {
            event.preventDefault();
            event.stopPropagation();

            model.calendar.fromDateOpened = false;
            model.calendar.toDateOpened = true;
        };

        model.openPopupCreateViolation = function () {
            model.modalInstance = $modal.open({
                templateUrl: "SourceApp/Components/Violation/Templates/createViolation.html",
                controller: "CreateViolationController as createViolationCtrl",
                backdrop: "static",
                windowClass: "modal-confirm-center"
            });

            return model.modalInstance.result;
        };

        model.validateViolation = function (violation) {
            if (!violation) {
                return false;
            } else {
                if (!violation.EmployeeId) {
                    notificationFactory.error("EmployeeRequired");
                    return false;
                }

                if (!violation.RawEffectiveDate) {
                    notificationFactory.error("EffectiveDateRequired");
                    return false;
                }

                if (!violation.WarningContent) {
                    notificationFactory.error("WarningContentRequired");
                    return false;
                }
            }

            return true;
        };

        model.createViolation = function (employeeId, effectiveDate, description, modalInstance) {
            return violationFactory.createViolation(employeeId, effectiveDate, description).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("CreatedSuccessfully");
                    modalInstance.close();
                }
                else {
                    notificationFactory.error("", response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        return model;
    }
})(angular.module("violationModule"));