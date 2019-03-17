const webpack = require('webpack');
const path = require('path');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CleanWebpackPlugin = require('clean-webpack-plugin');
const UglifyJsPlugin = require("uglifyjs-webpack-plugin");
const OptimizeCSSAssetsPlugin = require("optimize-css-assets-webpack-plugin");

module.exports = {
    entry: {
        js: './wwwroot-src/app.js'
        //,materialize: './wwwroot-src/materialize.js'
    },
    output: {
        path: path.resolve(__dirname, 'wwwroot/dist'),
        filename: '[name].bundle.js',
        //Queste 2 configurazioni sono per richiamare gli export del bundle dall pagine HTML
        //vedi: https://itnext.io/calling-a-es6-webpacked-class-from-outside-js-scope-f36dc77ea130
        libraryTarget: 'var',
        library: 'EntryPoint'
    },
    optimization: {
        splitChunks: {
            chunks: 'all'
        },
        minimizer: [
            new UglifyJsPlugin({
                cache: true,
                parallel: true,
                sourceMap: true
            }),
            new OptimizeCSSAssetsPlugin({})
        ]
    },
    module: {
        rules: [
            // BABEL
            {
                test: /\.jsx?$/,
                exclude: /(node_modules|bower_components)/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        presets: [['@babel/preset-env', {"useBuiltIns":"entry"} ]]
                    },
                }
            },
            // SASS
            {
                test: /\.scss$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    {
                        loader: 'css-loader',
                        options: {
                            sourceMap: true,
                            importLoaders: 1
                        }
                    },
                    //'postcss-loader',
                    {
                        loader: 'sass-loader',
                        options: {
                            sourceMap: true
                        }
                    }
                ]
            }
        ]
    },
    plugins: [
        new CleanWebpackPlugin(['wwwroot/dist']),
        new MiniCssExtractPlugin({
            // Options similar to the same options in webpackOptions.output
            // both options are optional
            //filename: "styles-[contenthash].css"
            filename: "styles.css"
            //,chunkFilename: "[id].css"
        }),
        new webpack.ProvidePlugin({
            Promise: 'es6-promise-promise'
        })
    ],
    resolve: {
        alias: {
            // Use compiled pica files from /dist folder
            pica: 'pica/dist/pica.js',
        },
    }
};
