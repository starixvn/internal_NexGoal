"use strict";

(function (appModule) {
    var menuTabDirective = appModule.directive("menuTab", function ($location) {
        return {
            restrict: "A",
            link: function (scope, element) {
                var selfPath = element.attr("href");
                var $parent = element.parent();

                if (selfPath === $location.path()) {
                    $parent.addClass("active");
                }

                scope.location = $location;

                scope.$watch("location.path()", function (newPath) {
                    if (newPath) {
                        if (selfPath === newPath) {
                            $parent.addClass("active");
                        } else {
                            $parent.removeClass("active");
                        }
                    }
                });
            }
        }
    });

    menuTabDirective.$inject = ["$location"];
})(angular.module("appModule"));