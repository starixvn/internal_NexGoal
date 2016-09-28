(function (assessmentModule) {
    "use strict";

    assessmentModule.factory("NominationModel", nominationModel);

    nominationModel.$inject = ["errorFactory", "nominationFactory"];

    function nominationModel(errorFactory, nominationFactory) {
        var model = {};

        model.employeeNominations = [];

        model.getNominationByPeriodId = function (employeeId, periodId) {
            return nominationFactory.getNominationByPeriodId(employeeId, periodId).success(function (employeeNominations) {
                model.employeeNominations = employeeNominations;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            })
        };

        return model;
    }
})(angular.module("assessmentModule"));