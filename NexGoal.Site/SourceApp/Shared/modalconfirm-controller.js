/*global appModule*/
'use strict';

appModule.controller("ModalConfirmController", ['$modalInstance', 'name', 'action', 'message',
 function ($modalInstance, name, action, message) {
     var vm = this;

     vm.message = !message ? "Do you want to " + action + " " + name + " ?" : message;

     vm.ok = function () {
         $modalInstance.close();
     };

     vm.cancel = function () {
         $modalInstance.dismiss("cancel");
     };
 }]);