(function (menuModule) {
    "use strict";

    menuModule.directive("menuTab", menuTabDirective);

    function menuTabDirective($location) {
        return {
            restrict: "A",
            link: function (scope, element, attrs) {
                var selfPath = attrs.link;
                var $parent = element.parent();
                var $grandParent = element.parent().parent().parent();

                if (selfPath === $location.path()) {
                    $parent.addClass("active");

                    if ($grandParent.hasClass("has-sub")) {
                        $grandParent.addClass("active");
                    }
                }

                scope.location = $location;

                scope.$watch("location.path()", function (newPath, oldPath) {
                    if (newPath) {
                        if (selfPath === newPath) {
                            $parent.addClass("active");

                            if ($grandParent.hasClass("has-sub")) {
                                $grandParent.addClass("active");
                            }
                        } else {
                            $parent.removeClass("active");
                            $grandParent.removeClass("active");
                        }
                    }
                });
            }
        };
    }

    menuTabDirective.$inject = ["$location"];
})(angular.module("menuModule"));