/*global appModule, toastr*/
'use strict';

appModule.factory('notificationFactory', ['$translate', function ($translate) {
    var options = {
        showDuration: "10000",
        hideDuration: "1000",
        timeOut: "5000",
        extendedTimeOut: "1000"
    };
    var title = '';

    return {
        success: function (keyMessage, message) {
            if (message === '' || message === null || message === undefined) {
                $translate(keyMessage).then(function (responseMessage) {
                    toastr.success(responseMessage, title, options);
                });
            }
            else {
                toastr.success(message, title, options);
            }
        },
        error: function (keyMessage, message) {
            if (message === '' || message === null || message === undefined) {
                $translate(keyMessage).then(function (responseMessage) {
                    toastr.error(responseMessage, options);
                });
            }
            else {
                toastr.error(message, title, options);
            }
        },
        warning: function (keyMessage, message) {
            if (message === '' || message === null || message === undefined) {
                $translate(keyMessage).then(function (responseMessage) {
                    toastr.warning(responseMessage, title, options);
                });
            }
            else {
                toastr.warning(message, title, options);
            }
        },
        info: function (keyMessage, message) {
            if (message === '' || message === null || message === undefined) {
                $translate(keyMessage).then(function (responseMessage) {
                    toastr.info(responseMessage, title, options);
                });
            }
            else {
                toastr.info(message, title, options);
            }
        }
    };
}]);