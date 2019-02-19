import axios from 'axios';

const SetOverlayConfig = function(){
    let requestCount = 0;

    axios.interceptors.request.use(function (config) {
        requestCount++;
        window.overlay.show();
        return config;
    }, function (error) {
        return Promise.reject(error);
    });
    axios.interceptors.response.use(function (response) {
        requestCount--;
        if(requestCount === 0){
            window.overlay.hide();
        }
        return response;
    }, function (error) {
        return Promise.reject(error);
    });
}

SetOverlayConfig();