(function (assessmentModule) {
    'use strict';

    assessmentModule.directive('exportFocus', ['baseUrl', function (baseUrl) {
        return {
            link: function (scope) {
                scope.$on('exportPAFocus', function () {
                    var data = scope.paFocusCtrl.filter,
                        url = baseUrl + 'ExportPAFocusReport/ExportPAFocus',
                        iframe = $('body iframe'),
                        parameters = { periodId: data.periodId, groupIds: data.groupIds, employeeId: data.employee.EmployeeId },
                        downloadUrl = url + "?" + $.param(parameters);

                    if (iframe.length === 0) {
                        iframe = $('<iframe width="0" height="0" class="display-none"></iframe>');
                        iframe.appendTo('body');
                    }

                    iframe.attr('src', downloadUrl);
                });
            }
        };
    }]);
})(angular.module('assessmentModule'));