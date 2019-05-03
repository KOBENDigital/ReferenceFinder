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
  .constant("referenceFinderContentByDocumentTypePath",
    "/umbraco/backoffice/ReferenceFinder/ReferenceFinderDashboard/FindContentReferencesByDocumentType")
  .constant("referenceFinderDocumentTypeAliasesPath",
    "/umbraco/backoffice/ReferenceFinder/ReferenceFinderDashboard/GetDocumentTypeAliases")
  .constant("referenceFinderDocumentTypePickerTemplatePath",
    "/App_Plugins/Koben.ReferenceFinder/html/DocumentTypePicker.html")
  .controller("referenceFinder.documentTypeDialogController", ["$scope", function ($scope) {
    var vm = this;
    
    vm.documentTypes = $scope.dialogData;
    vm.selectedDocumentType = null;
    vm.hasError = false;
    vm.errorMessage = null;
      
    $scope.vm = vm;

    $scope.documentTypeSelected = function () {
      var docType = null;

      if (isNaN(vm.selectedDocumentType)) {
        return;
      }

      var selectedId = parseInt(vm.selectedDocumentType);

      for (var i = 0; i < vm.documentTypes.length; i++) {
        if (selectedId === vm.documentTypes[i].Id) {
          docType = vm.documentTypes[i];
          break;
        }
      }

      if (!docType) {
        return;
      }

      $scope.submit({
        id: docType.Id,
        name: docType.Name,
        alias: docType.Alias
      });
    }
  }])
  .controller("referenceFinder.dashboardController",
    [
      "$scope", "$http", "referenceFinderContentByUrlPath", "referenceFinderMediaByUrlPath", "referenceFinderContentByIdPath", "referenceFinderMediaByIdPath", 
      "referenceFinderContentByDocumentTypePath", "referenceFinderDocumentTypePickerTemplatePath", "referenceFinderDocumentTypeAliasesPath", "dialogService",
      function ($scope, $http, referenceFinderContentByUrlPath, referenceFinderMediaByUrlPath, referenceFinderContentByIdPath, referenceFinderMediaByIdPath, 
               referenceFinderContentByDocumentTypePath, referenceFinderDocumentTypePickerTemplatePath, referenceFinderDocumentTypeAliasesPath, dialogService) {
        var vm = this;
        vm.targetId = null;
        vm.targetUrl = null;
        vm.targetDocumentType = {
          id: null,
          name: null,
          alias: null
        };
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

          if (vm.searchType !== "documentType") {
            if (vm.targetId === null || typeof vm.targetId === "undefined") {
              if (!vm.targetUrl) {
                vm.hasError = true;
                vm.errorMessage = "Please enter a target content/media url.";
                return;
              }

              switch (vm.searchType) {
                case "content":
                  if (!contentRegex.test(vm.targetUrl)) {
                    vm.hasError = true;
                    vm.errorMessage =
                      "Target content url not valid. Ensure the url provided starts with http:// or https://";
                    return;
                  }
                  
                  url = referenceFinderContentByUrlPath;
                  break;

                case "":
                  if (!mediaRegex.test(vm.targetUrl)) {
                    vm.hasError = true;
                    vm.errorMessage =
                      "Target media url not valid. Ensure the url provided starts with http:// or https:// and ends with a file extenstion eg. .png";
                    return;
                  }
                  
                  url = referenceFinderMediaByUrlPath;
                  break;

                default:
                  vm.hasError = true;
                  vm.errorMessage = "Please select a search mode";
                  return;
              }

              request = { targetUrl: vm.targetUrl };
            } else {
              url = vm.searchType === "content" ? referenceFinderContentByIdPath : referenceFinderMediaByIdPath;
              request = { targetId: vm.targetId };
            }
          }
          else {
            if (vm.targetDocumentType === null || typeof vm.targetDocumentType === "undefined" || vm.targetDocumentType.length < 1) {
              vm.hasError = true;
              vm.errorMessage = "Target content document type alias not valid. Ensure the alias has been provided";
              return;
            }

            url = referenceFinderContentByDocumentTypePath;
            request = { targetDocumentType: vm.targetDocumentType.alias };
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
          var badOverlayElement = angular.element("umb-overlay");
          var badOverlay =
            badOverlayElement[0] !== null && typeof badOverlayElement[0] !== "undefined" && 
            badOverlayElement[0].id === "krf_treepicker";

          switch (vm.searchType) {
            case "content":
              if (badOverlay) {
                dialogService.contentPicker({
                  multipicker: false,
                  callback: function (model) {
                    vm.targetId = model.id;
                    vm.targetUrl= model.name;
                  }
                });
                break;
              }
              
              $scope.treePickerOverlay = {
                entityType: "Document",
                idType: "int",
                minNumber: 1,
                maxNumber: 1,
                multiPicker: false,
                section: "content",
                show: true,
                startNode: {
                  query: "",
                  type: "content",
                  id: "-1"
                },
                startNodeId: "-1",
                submit: function (model) {
                  vm.targetId = model.selection[0].id;
                  vm.targetUrl= model.selection[0].name;
                  $scope.treePickerOverlay.show = false;
                  $scope.treePickerOverlay = null;
                },
                treeAlias: "content",
                view: "treepicker"
              };
              break;

            case "media":
              if (badOverlay) {
                dialogService.mediaPicker({
                  callback: function (model) {
                    vm.targetId = model.id;
                    vm.targetUrl= model.name;
                  }
                });
                break;
              }

              $scope.treePickerOverlay = {
                entityType: "Media",
                idType: "int",
                minNumber: 1,
                maxNumber: 1,
                multiPicker: false,
                section: "media",
                show: true,
                startNode: {
                  query: "",
                  type: "media",
                  id: "-1"
                },
                startNodeId: "-1",
                submit: function (model) {
                  vm.targetId = model.selection[0].id;
                  vm.targetUrl= model.selection[0].url;
                  $scope.treePickerOverlay.show = false;
                  $scope.treePickerOverlay = null;
                },
                treeAlias: "media",
                view: "treepicker"
              };
              break;

            case "documentType":
              $http({
                  url: referenceFinderDocumentTypeAliasesPath,
                  method: "GET"
                })
                .success(function (result) {
                  dialogService.open({
                    template: referenceFinderDocumentTypePickerTemplatePath,
                    show: true,
                    dialogData: result,
                    callback: function (data) {
                      vm.targetDocumentType = data;
                    }
                  });
                })
                .error(function () {
                  vm.hasError = true;
                  vm.errorMessage = "Something went wrong. Please try again";
                });

              break;
          }
        };

        $scope.getIdString = function () {
          if (vm.targetId !== null) {
            return "Id: " + vm.targetId;
          }

          if (vm.targetDocumentType.id !== null) {
            return "Id: " + vm.targetDocumentType.id;
          }

          return "";
        }
      }
    ]);