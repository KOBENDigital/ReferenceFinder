angular
  .module("umbraco")
  .constant("referenceFinderContentPath",
    "/umbraco/backoffice/reference-finder/referenceFinderDashboard/FindContentReferences")
  .constant("referenceFinderMediaPath",
    "/umbraco/backoffice/reference-finder/referenceFinderDashboard/FindMediaReferences")
  .controller("referenceFinder.dashboardController",
    [
      "$scope", "$http", "$location", "referenceFinderContentPath", "referenceFinderMediaPath",
      function($scope, $http, $location, referenceFinderContentPath, referenceFinderMediaPath) {
        var vm = this;
        vm.targetUrl = null;
        vm.searchType = "content";
        vm.results = {
          References: []
        };

        vm.hasError = false;
        vm.errorMessage = null;

        var hostRegex =
          /^http(s?):\/\/(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])(:\d+)?(\/?)$/i;

        $scope.vm = vm;
        $scope.search = function() {
          if (!vm.targetUrl) {
            vm.hasError = true;
            vm.errorMessage = "Please enter a target content/media url.";
            return;
          }

          if (!hostRegex.test(vm.targetUrl)) {
            vm.hasError = true;
            vm.errorMessage =
              "Target content/media url not valid. Ensure the url provided starts with http:// or https://";
            return;
          }

          vm.hasError = false;
          vm.results = {
            References: []
          };

          $http({
              url: vm.searchType === "content" ? referenceFinderContentPath : referenceFinderMediaPath,
              method: "POST",
              data: { targetUrl: vm.targetUrl }
            })
            .success(function(result) {
              vm.results = result;
            })
            .error(function() {
              vm.hasError = true;
              vm.errorMessage = "Something went wrong. Please try again";
            });
        };
      }
    ]);