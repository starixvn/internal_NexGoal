(function (assessmentModule) {
    "use strict";

    assessmentModule.factory("paFocusFactory", paFocusFactory);

    paFocusFactory.$inject = ["$http", "baseUrl"];

    function paFocusFactory($http, baseUrl) {
        return {
            getPAFocusFullPermission:function(){
                return $http.get(baseUrl + 'api/PAFocus/GetPAFocusFullPermission');
            },
            getPAProcessSettingByPeriodId: function (periodId) {
                return $http.get(baseUrl + 'api/PAFocus/GetPAProcessSettingByPeriodId', { params: { periodId: periodId } });
            },
            getPAFocus: function (periodId, groupIds) {
                return $http.get(baseUrl + 'api/PAFocus/GetPAFocus', { params: { periodId: periodId, groupIds : groupIds} });
            },
            updatePAFocus: function(evaluations){
                return $http.post(baseUrl + 'api/PAFocus/UpdatePAFocus', evaluations);
            },
            updatePAProcessSettingByPeriodId: function (periodId, processes) {
                var model = {
                    PeriodId: periodId,
                    Processes: processes
                };

                return $http.post(baseUrl + 'api/PAFocus/UpdatePAProcessSetting', model);
            }
        };
    }
})(angular.module("assessmentModule"));