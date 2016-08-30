(function (assessmentModule) {
    "use strict";

    assessmentModule.factory("PAFocusModel", paFocusModel);

    paFocusModel.$inject = ["paFocusFactory", "errorFactory"];

    function paFocusModel(paFocusFactory, errorFactory) {
        var me = {};

        me.processSettings = [];

        me.getPAProcessSettingByPeriodId = function (periodId) {
            return paFocusFactory.getPAProcessSettingByPeriodId(periodId).success(function (processSettings) {
                me.processSettings = processSettings;
            }).error(function (response, status) {
                errorFactory.errorHandle(status);
            });
        };

        return me;
    }
})(angular.module("assessmentModule"));