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

                    if ($grandParent.hasClass("has-sub") || $grandParent.find("ul").length > 0) {
                        $grandParent.addClass("active").addClass("marked");
                    }
                }

                scope.location = $location;

                scope.$watch("location.path()", function (newPath, oldPath) {
                    if (newPath) {
                        if (selfPath === newPath) {
                            $parent.addClass("active");

                            if ($grandParent.hasClass("has-sub")) {
                                $grandParent.addClass("active").addClass("marked");
                                $grandParent.siblings().removeClass("active").removeClass("marked");
                            }
                        } else {
                            $parent.removeClass("active");
                        }
                    }
                });

                element.bind("click", function () {
                    $location.path(selfPath).search({});

                    scope.$apply();
                })
            }
        };
    }

    menuTabDirective.$inject = ["$location"];
})(angular.module("menuModule"));