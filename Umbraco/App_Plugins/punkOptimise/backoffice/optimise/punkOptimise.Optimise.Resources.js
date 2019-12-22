(function () {
    'use strict';

    function OptimiseResources($http, umbRequestHelper) {
             
        function save(model) {
            return umbRequestHelper.resourcePromise($http.post('/umbraco/backoffice/punkoptimise/optimise/save', model),
                'Failed to update for nodeId: ' + model.id);
        }

        function isValid(id) {
            return umbRequestHelper.resourcePromise($http.get('/umbraco/backoffice/punkoptimise/optimise/isvalid?id=' + id),
                'Failed to validate for nodeId: ' + id);
        }

        var resource = {           
            save: save,
            isValid: isValid
        };

        return resource;
    }
    angular.module('umbraco.resources').factory('OptimiseResources', OptimiseResources);
})();