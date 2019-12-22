(function () {
    "use strict";
    function OptimiseActionController(
        $scope,
        $route,
        entityResource,
        notificationsService,
        appState,
        navigationService,
        OptimiseResources) {

        var SUCCESS = 0;     
        var selectedNode = appState.getMenuState("currentNode");

        $scope.loading = false;
        $scope.isValid = false;
        $scope.isImage = true;

        $scope.nodeId = parseInt(selectedNode.id);
        $scope.nodeName = selectedNode.name;    

        $scope.init = function () {
            $scope.loading = true;

            entityResource.getById($scope.nodeId, "Media")
                .then(function (media) {

                    if (media.metaData.ContentTypeAlias !== "Image") {
                        $scope.isImage = false;
                        return;
                    }

                    OptimiseResources.isValid($scope.nodeId)
                        .then(function (response) {
                            $scope.isValid = (response.resultType === SUCCESS);
                            $scope.loading = false;
                        });
                });
        };

        $scope.save = function () {
            $scope.loading = true;

            var saveModel = { id: $scope.nodeId };

            OptimiseResources.save(saveModel)
                .then(function (response) {

                    $scope.loading = false;
                    navigationService.hideDialog();

                    if (response.resultType === SUCCESS) {
                        notificationsService.success("Success", response.message);
                        $route.reload();
                    }
                    else
                        notificationsService.error("Error", response.message);
                });
        };
    }
    angular.module('umbraco').controller("punkOptimise.Optimise.Controller", OptimiseActionController);
})();