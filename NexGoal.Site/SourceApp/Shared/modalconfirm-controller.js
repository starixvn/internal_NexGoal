/*global appModule*/
'use strict';

appModule.controller("ModalConfirmController", ['$scope', '$modalInstance', 'name', 'action', 'message',
 function ($scope, $modalInstance, name, action, message) {
     var vm = this;

     vm.message = !message ? "Do you want to " + action + " " + name + " ?" : message;

     function initialize() {
         $scope.$on('$routeChangeStart', function () {
             $modalInstance.dismiss('cancel');
         });
     }

     vm.ok = function () {
         $modalInstance.close();
     };

     vm.cancel = function () {
         $modalInstance.dismiss("cancel");
     };

     initialize();
 }]);