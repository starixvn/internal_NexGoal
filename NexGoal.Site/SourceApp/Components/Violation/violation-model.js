(function (violationModule) {
    "use strict";

    violationModule.factory("ViolationModel", violationModel);

    violationModel.$inject = ["$modal", "notificationFactory", "errorFactory", "violationFactory"];

    function violationModel($modal, notificationFactory, errorFactory, violationFactory) {
        var model = {};

        model.permission = 0;
        model.violation = {};
        model.violations = [];
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

        model.openPopupUpdateViolation = function (violation) {
            model.modalInstance = $modal.open({
                templateUrl: "SourceApp/Components/Violation/Templates/updateViolation.html",
                controller: "UpdateViolationController as updateViolationCtrl",
                backdrop: "static",
                windowClass: "modal-confirm-center",
                resolve: {
                    violation: function () {
                        return angular.copy(violation);
                    }
                }
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

                if (!violation.RawEffectedDate) {
                    notificationFactory.error("EffectedDateRequired");
                    return false;
                }

                if (!violation.Description) {
                    notificationFactory.error("WarningContentRequired");
                    return false;
                }
            }

            return true;
        };

        model.createViolation = function (employeeId, effectedDate, description, modalInstance) {
            return violationFactory.createViolation(employeeId, effectedDate, description).success(function (response) {
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

        model.updateViolation = function (violationId, effectedDate, description, modalInstance) {
            return violationFactory.updateViolation(violationId, effectedDate, description).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                    modalInstance.close();
                }
                else {
                    notificationFactory.error("", response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.modifyViolationStatus = function (violationId) {
            return violationFactory.modifyViolationStatus(violationId).success(function (response) {
                if (response.IsSuccessful) {
                    notificationFactory.success("UpdatedSuccessfully");
                }
                else {
                    notificationFactory.error("", response.Message);
                }
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getViolation = function (violationId) {
            return violationFactory.getViolation(violationId).success(function (violation) {
                model.violation = violation;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getViolations = function (fromDate, toDate, isActive, groupIds, employeeId) {
            return violationFactory.getViolations(fromDate, toDate, isActive, groupIds, employeeId).success(function (violations) {
                model.violations = violations;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        model.getUserPermissionOnViolation = function () {
            return violationFactory.getUserPermissionOnViolation().success(function (permission) {
                model.permission = permission;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        return model;
    }
})(angular.module("violationModule"));