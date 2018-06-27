const path = require("path");
const webpack = require("webpack");
const CleanWebpackPlugin = require('clean-webpack-plugin');

const { environment } = require("./parts/constant");
const config = require("./webpack.config");

const devConfig = config(environment.development)[0];
devConfig.devServer = {
    compress: true,
    port: 9000,
    hot: true,
    watchContentBase: true,
    publicPath: "/dist/"
};

module.exports = devConfig;