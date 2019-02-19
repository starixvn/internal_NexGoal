import axios from 'axios';

const SetOverlayConfig = function(){
    axios.interceptors.request.use(function (config) {
        window.overlay.show();
        return config;
    }, function (error) {
        return Promise.reject(error);
    });
    axios.interceptors.response.use(function (response) {
        window.overlay.hide();
        return response;
    }, function (error) {
        return Promise.reject(error);
    });
}

export default SetOverlayConfig;