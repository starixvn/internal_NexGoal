(function (menuModule) {
    "use strict";

    menuModule.directive("menuTab", menuTabDirective);

    function menuTabDirective($location) {
        return {
            restrict: "A",
            link: function (scope, element, attrs) {
                var selfPath = attrs.link;
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
        };
    }

    menuTabDirective.$inject = ["$location"];
})(angular.module("menuModule"));