import path from 'path';
import * as webpack from 'webpack';

const config: webpack.Configuration = {
    mode: 'development',
    entry: {
        background: './src/background/background.fsproj',
        contentScript: './src/content/content.fsproj',
    },
    devtool: 'inline-source-map',
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: '[name].js',
    },
    module: {
        rules: [
            {
                test: /\.fs(x|proj)?$/,
                loader: 'fable-loader',
            }
        ],
    },
};

export default config;
