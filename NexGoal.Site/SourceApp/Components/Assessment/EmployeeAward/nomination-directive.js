(function (assessmentModule) {
    'use strict';

    assessmentModule.directive('exportEmployeeOfTheQuarter', ['baseUrl', function (baseUrl) {
        return {
            link: function (scope) {
                scope.$on('exportEmployeeOfTheQuarter', function () {
                    var data = $.param(scope.eoqAccessCtrl.exportCriteria),
                        url = baseUrl + 'ExportNomination/ExportEOQ',
                        downloadUrl = url + "?" + data,
                        iframe = $('body iframe');

                    if (iframe.length === 0) {
                        iframe = $('<iframe width="0" height="0" class="display-none"></iframe>');
                        iframe.appendTo('body');
                    }

                    iframe.attr('src', downloadUrl);
                });
            }
        };
    }]);

    assessmentModule.directive('exportEmployeeOfTheYear', ['baseUrl', function (baseUrl) {
        return {
            link: function (scope) {
                scope.$on('exportEmployeeOfTheYear', function () {
                    var data = $.param(scope.eoyAccessCtrl.exportCriteria),
                        url = baseUrl + 'ExportNomination/ExportEOY',
                        downloadUrl = url + "?" + data,
                        iframe = $('body iframe');

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