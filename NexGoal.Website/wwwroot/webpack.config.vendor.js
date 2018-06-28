const path = require("path");
const webpack = require("webpack");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const merge = require("webpack-merge");
const OptimizeCssAssetsPlugin = require("optimize-css-assets-webpack-plugin");
const CleanWebpackPlugin = require("clean-webpack-plugin");

const { environment, projectPath } = require("./parts/constant");

let pathsToClean = ["dist/*.*"];

let cleanOptions = {
    root: path.resolve(__dirname, "."),
    //exclude: ["bootstrap.css"],
    verbose: true,
    dry: false
};

module.exports = env => {
    env = environment[env] || environment.development;
    const isDevBuild = !(env === environment.production);
    const relativeOutputRootPath = projectPath.clientOutputPath;

    const sharedConfig = {
        mode: env,
        stats: { modules: false },
        module: {
            rules: [
                {
                    test: /\.(png|woff|woff2|eot|ttf)(\?|$)/,
                    use: [
                        {
                            loader: "url-loader",
                            options: {
                                limit: 100000
                            }
                        }
                    ]
                }
            ]
        },
        entry: {
            vendor: ["react", "react-dom"]
        },
        output: {
            publicPath: `dist/`,
            filename: "[name].js",
            library: "[name]_[hash]"
        },
        plugins: [
            new webpack.NormalModuleReplacementPlugin(
                /\/iconv-loader$/,
                require.resolve("node-noop")
            ), // Workaround for https://github.com/andris9/encoding/issues/16
            new webpack.DefinePlugin({
                "process.env.NODE_ENV": isDevBuild ? '"development"' : '"production"'
            }),
            new OptimizeCssAssetsPlugin()
        ].concat(isDevBuild ? [] : [new CleanWebpackPlugin(pathsToClean, cleanOptions)])
    };

    const clientBundleConfig = merge(sharedConfig, {
        output: { path: relativeOutputRootPath },
        module: {
            rules: [
                {
                    test: /\.s?[ac]ss$/,
                    use: [MiniCssExtractPlugin.loader, "css-loader", "sass-loader"]
                }
            ]
        },
        plugins: [
            new MiniCssExtractPlugin({
                // Options similar to the same options in webpackOptions.output
                // both options are optional
                filename: "[name].css"
            }),
            new webpack.DllPlugin({
                path: path.join(relativeOutputRootPath, "[name]-manifest.json"),
                name: "[name]_[hash]"
            })
        ]
    });

    return [clientBundleConfig];
};