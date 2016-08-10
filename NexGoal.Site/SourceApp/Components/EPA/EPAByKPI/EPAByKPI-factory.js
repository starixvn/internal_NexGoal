'use strict';
(function (angular) {
    function epaByKPIFactory($http, baseUrl) {
        function getEvaluatorsByTargetTemplate(periodId, targetTemplateId) {
            return $http.get(baseUrl + 'api/EPAByKPI/getEvaluatorsByTargetTemplate', {
                params: {
                    periodId: periodId,
                    targetTemplateId: targetTemplateId
                }
            });
        }

        function getEmployeeEPAToView(periodId, targetTemplateId, kpiId, evaluatorId) {
            return $http.get(baseUrl + 'api/EPAByKPI/GetEmployeeEPAToView', {
                params: {
                    periodId: periodId,
                    targetTemplateId: targetTemplateId,
                    kpiId: kpiId,
                    evaluatorId: evaluatorId
                }
            });
        }

        function getKPIToView(periodId, targetTemplateId, evaluatorId) {
            return $http.get(baseUrl + 'api/EPAByKPI/getKPIToView', {
                params: {
                    periodId: periodId,
                    targetTemplateId: targetTemplateId,
                    evaluatorId: evaluatorId
                }
            });
        }

        function getPeriodToView() {
            return $http.get(baseUrl + 'api/EPAByKPI/GetPeriodToView');
        }

        function getTargetTemplateToView(periodId) {
            return $http.get(baseUrl + 'api/EPAByKPI/GetTargetTemplateToView',
            {
                params: {
                    periodId: periodId
                }
            });
        }

        return {
            getPeriodToView: getPeriodToView,
            getTargetTemplateToView: getTargetTemplateToView,
            getKPIToView: getKPIToView,
            getEvaluatorsByTargetTemplate: getEvaluatorsByTargetTemplate,
            getEmployeeEPAToView: getEmployeeEPAToView
        };
    }

    epaByKPIFactory.$inject = ['$http', 'baseUrl'];

    angular
        .module('epaModule')
        .factory('epaByKPIFactory', epaByKPIFactory);
})(angular);