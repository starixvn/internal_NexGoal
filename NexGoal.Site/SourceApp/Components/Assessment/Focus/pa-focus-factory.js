(function (assessmentModule) {
    "use strict";

    assessmentModule.factory("paFocusFactory", paFocusFactory);

    paFocusFactory.$inject = ["$http", "baseUrl"];

    function paFocusFactory($http, baseUrl) {
        return {
            getPAProcessSettingByPeriodId: function (periodId) {
                return $http.get(baseUrl + 'api/PAFocus/GetPAProcessSettingByPeriodId', { params: { periodId: periodId } });
            }
        };
    }
})(angular.module("assessmentModule"));