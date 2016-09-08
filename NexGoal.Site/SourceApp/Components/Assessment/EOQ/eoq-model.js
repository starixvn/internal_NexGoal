(function (assessmentModule) {
    "use strict";

    assessmentModule.factory("EOQModel", eoqModel);

    eoqModel.$inject = ["$modal", "eoqFactory", "errorFactory", "notificationFactory", "utilitiesFactory"];

    function eoqModel($modal, eoqFactory, errorFactory, notificationFactory, utilitiesFactory) {
        var vm = {};

        vm.periods = [];
        vm.period = [];
        vm.nominatorSettings = [];

        vm.getStatusName = function (statusId) {
            var statusName = "";

            switch (statusId) {
                case 0:
                    statusName = "Not Started";
                    break;
                case 1:
                    statusName = "In Progress";
                    break;
                case 2:
                    statusName = "Closed";
                    break;
            }

            return statusName;
        };

        vm.getQuarterName = function (quarter) {
            var quarterName = "";

            switch (quarter) {
                case 1:
                    quarterName = quarter + "st";
                    break;
                case 2:
                    quarterName = quarter + "nd";
                    break;
                case 3:
                    quarterName = quarter + "rd";
                    break;
                default:
                    quarterName = quarter + "th";
                    break;
            }

            return quarterName;
        };

        vm.createEOQ = function (period) {
            return eoqFactory.createEOQ(period).success(function (response) {
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

        vm.updateEOQ = function (period) {
            return eoqFactory.updateEOQ(period).success(function (response) {
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

        vm.getEOQ = function (year, statusId) {
            return eoqFactory.getEOQ(year, statusId).success(function (periods) {
                vm.periods = periods;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.getEOQById = function (eoqId) {
            return eoqFactory.getEOQById(eoqId).success(function (period) {
                vm.period = period;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.GetEOQNominatorSettingById = function (query, groupIds, roleId, showAllEmployee) {
            return eoqFactory.GetEOQNominatorSettingById(query, groupIds, roleId, showAllEmployee).success(function (nominatorSettings) {
                vm.nominatorSettings = nominatorSettings;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        vm.openPopupCreateEOQ = function () {
            vm.modalInstance = $modal.open({
                templateUrl: 'SourceApp/Components/Assessment/EOQ/Templates/modalCreateEOQ.html',
                controller: "CreateEOQController as createEOQCtrl",
                backdrop: 'static',
                windowClass: "modal-confirm-center period-create-popup"
            });

            return vm.modalInstance.result;
        };

        vm.openPopupDeleteEOQ = function (vm, period) {
            var modalInstance = $modal.open({
                templateUrl: '/SourceApp/Shared/Templates/ModalConfirmContent.html',
                controller: "ModalConfirmController as modalConfirmCtrl",
                backdrop: 'static',
                resolve: {
                    name: function () {
                        return period.QuarterName + " quarter of " + period.Year;
                    },
                    action: function () {
                        return 'delete';
                    }
                },
                windowClass: "modal-confirm-center"
            });

            modalInstance.result.then(function () {
                return eoqFactory.deleteEOQ(period.EOQYId).success(function (response) {
                    if (response.IsSuccessful) {
                        var index = vm.periods.indexOf(period);
                        vm.periods.splice(index, 1);
                        utilitiesFactory.searchOnPage(vm, vm.periods);
                        notificationFactory.success("DeletedSuccessfully");
                    }
                    else {
                        notificationFactory.error("", response.Message);
                    }
                }).error(function (response, status) {
                    errorFactory.errorHandle(status);
                });
            })["finally"](function () {
                modalInstance = null;
            });
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