'use strict';
/*global epaModule*/

epaModule.factory('LastRatingModel',
['employeeEPAFactory', 'utilitiesFactory', 'errorFactory',
function (employeeEPAFactory, utilitiesFactory, errorFactory) {
    var LastRating = this;

    LastRating.groups = [];

    LastRating.getLastRating = function (employeeId, targetTypeId) {
        return employeeEPAFactory.getLastRating(employeeId, targetTypeId)
        .success(function (lastRating) {
            LastRating.groups = lastRating;
        }).error(function (response, status) {
            errorFactory.errorHandle(status);
        });
    };

    LastRating.getLastScore = function (groupId, kpiId) {
        var lastScore = 0, count = 0;

        angular.forEach(LastRating.groups, function (group) {
            if (group.KPIGroupId === groupId) {
                angular.forEach(group.KPIs, function (kpi) {
                    if (kpi.KPIId === kpiId) {
                        angular.forEach(kpi.EvaluationByEvaluators, function (evaluator) {
                            if (evaluator.Score) {
                                lastScore += evaluator.Score;
                                count++;
                            }
                        });
                    }
                });
            }
        });

        return count > 0 ? lastScore / count : "";
    };

    LastRating.getLastWeight = function (groupId, kpiId) {
        var lastWeight = "";

        angular.forEach(LastRating.groups, function (group) {
            if (group.KPIGroupId === groupId) {
                angular.forEach(group.KPIs, function (kpi) {
                    if (kpi.KPIId === kpiId) {
                        lastWeight = kpi.Weight;
                    }
                });
            }
        });

        return lastWeight;
    };

    LastRating.getSubLastWeight = function (kpiGroupId) {
        var subWeight = 0;
        angular.forEach(LastRating.groups, function (group) {
            if (group.KPIGroupId === kpiGroupId) {
                subWeight = group.Weight;
            }
        });

        return subWeight;
    };

    LastRating.getSubLastScore = function (kpiGroupId) {
        var subScore = 0;
        angular.forEach(LastRating.groups, function (group) {
            if (group.KPIGroupId === kpiGroupId) {
                angular.forEach(group.KPIs, function (kpi) {
                    subScore += LastRating.getLastScore(kpiGroupId, kpi.KPIId);
                });
            }
        });

        return utilitiesFactory.splitNumber(subScore, 2);
    };

    LastRating.getLastTotalScore = function () {
        var totalScore = 0;
        angular.forEach(LastRating.groups, function (group) {
            totalScore += LastRating.getSubLastScore(group.KPIGroupId);
        });

        return totalScore.toFixed(2);
    };

    return LastRating;
}]);