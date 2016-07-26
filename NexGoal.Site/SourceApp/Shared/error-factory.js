/*global appModule*/
'use strict';

appModule.factory("errorFactory",
['$location', '$window', 'notificationFactory', 'baseUrl', 
function ($location, $window, notificationFactory, baseUrl) {
    return {
        errorHandle: function (status, internalPageRedirect) {
            if (status === 405) {
                $location.path(baseUrl + 'Home/AccessDenied');
            } if (status === 404) {
                $location.path(baseUrl + 'Home/PageNotFound');
            }
            else {
                notificationFactory.error("ErrorMessage");

                if (internalPageRedirect) {
                    $location.path(baseUrl + 'Home/InternalError');
                }
            }
        }
    };
} ]);