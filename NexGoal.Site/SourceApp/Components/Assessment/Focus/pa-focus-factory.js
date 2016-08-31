(function (assessmentModule) {
    "use strict";

    assessmentModule.factory("paFocusFactory", paFocusFactory);

    paFocusFactory.$inject = ["$http", "baseUrl"];

    function paFocusFactory($http, baseUrl) {
        return {
            getPAProcessSettingByPeriodId: function (periodId) {
                return $http.get(baseUrl + 'api/PAFocus/GetPAProcessSettingByPeriodId', { params: { periodId: periodId } });
            },
            updatePAProcessSettingByPeriodId: function (periodId, processes) {
                var model = {
                    periodId: periodId,
                    process: processes
                };

                return $http.post(baseUrl + 'api/PAFocus/UpdatePAProcessSetting', model);
            }
        };
    }
})(angular.module("assessmentModule"));