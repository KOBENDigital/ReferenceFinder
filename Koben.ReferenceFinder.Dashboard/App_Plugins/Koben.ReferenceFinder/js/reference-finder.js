angular
  .module("umbraco")
  .constant("referenceFinderContentByUrlPath",
    "/umbraco/backoffice/ReferenceFinder/ReferenceFinderDashboard/FindContentReferencesByUrl")
  .constant("referenceFinderMediaByUrlPath",
    "/umbraco/backoffice/ReferenceFinder/ReferenceFinderDashboard/FindMediaReferencesByUrl")
  .constant("referenceFinderContentByIdPath",
    "/umbraco/backoffice/ReferenceFinder/ReferenceFinderDashboard/FindContentReferencesById")
  .constant("referenceFinderMediaByIdPath",
    "/umbraco/backoffice/ReferenceFinder/ReferenceFinderDashboard/FindMediaReferencesById")
  .controller("referenceFinder.dashboardController",
    [
      "$scope", "$http", "referenceFinderContentByUrlPath", "referenceFinderMediaByUrlPath", "referenceFinderContentByIdPath", "referenceFinderMediaByIdPath", "dialogService",
      function ($scope, $http, referenceFinderContentByUrlPath, referenceFinderMediaByUrlPath, referenceFinderContentByIdPath, referenceFinderMediaByIdPath, dialogService) {
        var vm = this;
        vm.targetId = null;
        vm.targetUrl = null;
        vm.searchType = "content";
        vm.results = {
          References: []
        };

        vm.hasError = false;
        vm.errorMessage = null;

        var contentRegex =
          /^http(s?):\/\/(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])(:\d+)?(\/?)([A-Za-z0-9\-\/_]*)$/i;

        var mediaRegex =
          /^http(s?):\/\/(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])(:\d+)?(\/?)([A-Za-z0-9\/_]+\.[A-Za-z0-9]*)$/i;

        $scope.vm = vm;
        $scope.search = function () {
          var url;
          var request;

          if (vm.targetId === null || typeof vm.targetId === "undefined") {
            if (!vm.targetUrl) {
              vm.hasError = true;
              vm.errorMessage = "Please enter a target content/media url.";
              return;
            }

            if (vm.searchType === "content") {
              if (!contentRegex.test(vm.targetUrl)) {
                vm.hasError = true;
                vm.errorMessage =
                  "Target content url not valid. Ensure the url provided starts with http:// or https://";
                return;
              }
            } else {
              if (!mediaRegex.test(vm.targetUrl)) {
                vm.hasError = true;
                vm.errorMessage =
                  "Target media url not valid. Ensure the url provided starts with http:// or https:// and ends with a file extenstion eg. .png";
                return;
              }
            }

            url = vm.searchType === "content" ? referenceFinderContentByUrlPath : referenceFinderMediaByUrlPath;
            request = { targetUrl: vm.targetUrl };
          } else {
            url = vm.searchType === "content" ? referenceFinderContentByIdPath : referenceFinderMediaByIdPath;
            request = { targetId: vm.targetId };
          }

          vm.hasError = false;
          vm.results = {
            References: []
          };

          $http({
            url: url,
            method: "POST",
            data: request
          })
            .success(function (result) {
              vm.results = result;
            })
            .error(function () {
              vm.hasError = true;
              vm.errorMessage = "Something went wrong. Please try again";
            });
        };

        $scope.browse = function () {
          if (vm.searchType === "content") {
            dialogService.contentPicker({
              callback: function (data) {
                vm.targetId = data.id;
              }
            });
          } else {
            dialogService.mediaPicker({
              callback: function (data) {
                vm.targetId = data.id;
              }
            });
          }
        };
      }
    ]);